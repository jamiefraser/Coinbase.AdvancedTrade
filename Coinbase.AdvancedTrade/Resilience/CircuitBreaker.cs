using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Coinbase.AdvancedTrade.Resilience
{
    /// <summary>
    /// Circuit breaker states.
    /// </summary>
    public enum CircuitState
    {
        /// <summary>
        /// Normal operation - requests pass through.
        /// </summary>
        Closed,

        /// <summary>
        /// Circuit is open - requests are rejected immediately.
        /// Too many failures detected.
        /// </summary>
        Open,

        /// <summary>
        /// Testing if service has recovered - limited requests allowed.
        /// </summary>
        HalfOpen
    }

    /// <summary>
    /// Configuration for circuit breaker behavior.
    /// </summary>
    public sealed class CircuitBreakerConfiguration
    {
        /// <summary>
        /// Gets or sets the name of the service being protected.
        /// </summary>
        public string ServiceName { get; set; } = "CoinbaseAPI";

        /// <summary>
        /// Gets or sets the number of failures before opening the circuit.
        /// Default: 5
        /// </summary>
        public int FailureThreshold { get; set; } = 5;

        /// <summary>
        /// Gets or sets the time window for counting failures (in seconds).
        /// Default: 60 seconds
        /// </summary>
        public double FailureTimeoutSeconds { get; set; } = 60.0;

        /// <summary>
        /// Gets or sets the time to wait before attempting recovery (in seconds).
        /// Default: 30 seconds
        /// </summary>
        public double RecoveryTimeoutSeconds { get; set; } = 30.0;
    }

    /// <summary>
    /// Exception thrown when circuit breaker is open.
    /// </summary>
    public sealed class CircuitBreakerException : Exceptions.CoinbaseException
    {
        /// <summary>
        /// Gets the name of the service protected by the circuit breaker.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Gets the current circuit state.
        /// </summary>
        public CircuitState State { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircuitBreakerException"/> class.
        /// </summary>
        /// <param name="serviceName">The service name.</param>
        /// <param name="state">The circuit state.</param>
        public CircuitBreakerException(string serviceName, CircuitState state)
            : base($"Circuit breaker for {serviceName} is {state}")
        {
            ServiceName = serviceName;
            State = state;
        }
    }

    /// <summary>
    /// Circuit breaker implementation for preventing cascading failures.
    /// Tracks failure rates and temporarily opens circuit to prevent requests
    /// to failing services, allowing them time to recover.
    /// </summary>
    public sealed class CircuitBreaker
    {
        private readonly CircuitBreakerConfiguration _config;
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private int _failureCount;
        private DateTimeOffset? _failureTimestamp;
        private CircuitState _state = CircuitState.Closed;
        private DateTimeOffset? _lastFailureTime;

        /// <summary>
        /// Gets the current circuit state.
        /// </summary>
        public CircuitState State
        {
            get
            {
                _lock.Wait();
                try
                {
                    CheckTimeout();
                    return _state;
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        /// <summary>
        /// Gets the current failure count.
        /// </summary>
        public int FailureCount
        {
            get
            {
                _lock.Wait();
                try
                {
                    return _failureCount;
                }
                finally
                {
                    _lock.Release();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CircuitBreaker"/> class.
        /// </summary>
        /// <param name="config">Circuit breaker configuration.</param>
        /// <param name="logger">Optional logger.</param>
        public CircuitBreaker(CircuitBreakerConfiguration config = null, ILogger logger = null)
        {
            _config = config ?? new CircuitBreakerConfiguration();
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Executes a function through the circuit breaker.
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="func">Function to execute.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Function result.</returns>
        /// <exception cref="CircuitBreakerException">If circuit is open.</exception>
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default)
        {
            await _lock.WaitAsync(cancellationToken);
            try
            {
                CheckTimeout();

                if (_state == CircuitState.Open)
                {
                    throw new CircuitBreakerException(_config.ServiceName, _state);
                }
            }
            finally
            {
                _lock.Release();
            }

            try
            {
                var result = await func();
                await OnSuccessAsync();
                return result;
            }
            catch (Exception)
            {
                await OnFailureAsync();
                throw;
            }
        }

        /// <summary>
        /// Records a successful call.
        /// </summary>
        private async Task OnSuccessAsync()
        {
            await _lock.WaitAsync();
            try
            {
                _failureCount = 0;

                if (_state == CircuitState.HalfOpen)
                {
                    _state = CircuitState.Closed;
                    _logger.LogInformation(
                        "Circuit breaker recovered: Service={ServiceName}, State=Closed",
                        _config.ServiceName);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Records a failed call.
        /// </summary>
        private async Task OnFailureAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var now = DateTimeOffset.UtcNow;
                _lastFailureTime = now;

                // Reset failure count if outside time window
                if (_failureTimestamp.HasValue &&
                    (now - _failureTimestamp.Value).TotalSeconds > _config.FailureTimeoutSeconds)
                {
                    _failureCount = 0;
                }

                _failureTimestamp = now;
                _failureCount++;

                if (_failureCount >= _config.FailureThreshold)
                {
                    _state = CircuitState.Open;
                    _logger.LogWarning(
                        "Circuit breaker opened: Service={ServiceName}, Failures={Failures}, Threshold={Threshold}",
                        _config.ServiceName,
                        _failureCount,
                        _config.FailureThreshold);
                }
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Checks if it's time to transition to half-open state.
        /// </summary>
        private void CheckTimeout()
        {
            if (_state != CircuitState.Open)
            {
                return;
            }

            if (!_lastFailureTime.HasValue)
            {
                return;
            }

            var now = DateTimeOffset.UtcNow;
            if ((now - _lastFailureTime.Value).TotalSeconds > _config.RecoveryTimeoutSeconds)
            {
                _state = CircuitState.HalfOpen;
                _logger.LogInformation(
                    "Circuit breaker testing recovery: Service={ServiceName}, State=HalfOpen",
                    _config.ServiceName);
            }
        }
    }
}
