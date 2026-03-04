using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models;
using Coinbase.AdvancedTrade.Models.Public;

namespace Coinbase.AdvancedTrade.Resilience
{

/// <summary>
/// P2 Task 9: Resilience integration wrapper for Coinbase managers.
/// Applies retry and circuit breaker policies by default to all API calls,
/// making resilience opt-out rather than opt-in.
/// </summary>
public static class ResilientManagerFactory
{
    /// <summary>
    /// Creates a resilient wrapper around an IPublicManager that applies automatic retry
    /// and circuit breaker policies to all API calls.
    /// </summary>
    public static IPublicManager CreateResilientPublicManager(
        IPublicManager baseManager,
        RetryPolicy? retryPolicy = null,
        CircuitBreakerPolicy? circuitBreakerPolicy = null)
    {
        if (baseManager == null)
            throw new ArgumentNullException(nameof(baseManager));

        return new ResilientPublicManagerWrapper(
            baseManager,
            retryPolicy ?? RetryPolicy.Default,
            circuitBreakerPolicy ?? CircuitBreakerPolicy.Default);
    }

    /// <summary>
    /// Creates a resilient wrapper around an IProductsManager that applies automatic retry
    /// and circuit breaker policies to all API calls.
    /// </summary>
    public static IProductsManager CreateResilientProductsManager(
        IProductsManager baseManager,
        RetryPolicy? retryPolicy = null,
        CircuitBreakerPolicy? circuitBreakerPolicy = null)
    {
        if (baseManager == null)
            throw new ArgumentNullException(nameof(baseManager));

        return new ResilientProductsManagerWrapper(
            baseManager,
            retryPolicy ?? RetryPolicy.Default,
            circuitBreakerPolicy ?? CircuitBreakerPolicy.Default);
    }

    /// <summary>
    /// Resilience policy configuration for retries (exponential backoff with jitter).
    /// </summary>
    public sealed class RetryPolicy
    {
        public int MaxRetries { get; }
        public TimeSpan InitialDelay { get; }
        public double BackoffMultiplier { get; }

        public RetryPolicy(int maxRetries = 3, TimeSpan? initialDelay = null, double backoffMultiplier = 2.0)
        {
            MaxRetries = Math.Max(1, maxRetries);
            InitialDelay = initialDelay ?? TimeSpan.FromMilliseconds(100);
            BackoffMultiplier = Math.Max(1.1, backoffMultiplier); // Minimum 10% increase
        }

        /// <summary>
        /// Default retry policy: 3 retries with exponential backoff starting at 100ms.
        /// </summary>
        public static RetryPolicy Default => new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(100), backoffMultiplier: 2.0);

        /// <summary>
        /// Aggressive retry policy: 5 retries for critical operations.
        /// </summary>
        public static RetryPolicy Aggressive => new RetryPolicy(maxRetries: 5, initialDelay: TimeSpan.FromMilliseconds(50), backoffMultiplier: 1.5);

        /// <summary>
        /// Conservative retry policy: 1 retry only, fail fast.
        /// </summary>
        public static RetryPolicy Conservative => new RetryPolicy(maxRetries: 1, initialDelay: TimeSpan.FromMilliseconds(200), backoffMultiplier: 2.0);
    }

    /// <summary>
    /// Resilience policy configuration for circuit breaker state machine.
    /// </summary>
    public sealed class CircuitBreakerPolicy
    {
        public int FailureThreshold { get; }
        public TimeSpan OpenDuration { get; }
        public int SuccessThresholdInHalfOpen { get; }

        public CircuitBreakerPolicy(int failureThreshold = 5, TimeSpan? openDuration = null, int successThresholdInHalfOpen = 2)
        {
            FailureThreshold = Math.Max(1, failureThreshold);
            OpenDuration = openDuration ?? TimeSpan.FromSeconds(30);
            SuccessThresholdInHalfOpen = Math.Max(1, successThresholdInHalfOpen);
        }

        /// <summary>
        /// Default circuit breaker: opens after 5 failures, stays open 30s, closes after 2 successes.
        /// </summary>
        public static CircuitBreakerPolicy Default => new CircuitBreakerPolicy(failureThreshold: 5, openDuration: TimeSpan.FromSeconds(30), successThresholdInHalfOpen: 2);

        /// <summary>
        /// Strict circuit breaker: opens after 2 failures, stays open 60s.
        /// </summary>
        public static CircuitBreakerPolicy Strict => new CircuitBreakerPolicy(failureThreshold: 2, openDuration: TimeSpan.FromSeconds(60), successThresholdInHalfOpen: 3);
    }
}

/// <summary>
/// Wrapper around IPublicManager that applies resilience policies.
/// </summary>
internal sealed class ResilientPublicManagerWrapper : IPublicManager
{
    private readonly IPublicManager _baseManager;
    private readonly ResilientManagerFactory.RetryPolicy _retryPolicy;
    private readonly ResilientManagerFactory.CircuitBreakerPolicy _cbPolicy;
    private int _consecutiveFailures;
    private bool _circuitOpen;
    private DateTimeOffset _circuitOpenedAt;

    public ResilientPublicManagerWrapper(
        IPublicManager baseManager,
        ResilientManagerFactory.RetryPolicy retryPolicy,
        ResilientManagerFactory.CircuitBreakerPolicy cbPolicy)
    {
        _baseManager = baseManager;
        _retryPolicy = retryPolicy;
        _cbPolicy = cbPolicy;
        _consecutiveFailures = 0;
        _circuitOpen = false;
    }

    public async Task<List<PublicProduct>> ListPublicProductsAsync(
        int? limit = null,
        int? offset = null,
        string productType = null,
        List<string> productIds = null)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.ListPublicProductsAsync(limit, offset, productType, productIds)).ConfigureAwait(false);
    }

    public async Task<Coinbase.AdvancedTrade.Models.Public.ServerTime> GetCoinbaseServerTimeAsync()
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetCoinbaseServerTimeAsync()).ConfigureAwait(false);
    }

    public async Task<PublicProduct> GetPublicProductAsync(string productId)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetPublicProductAsync(productId)).ConfigureAwait(false);
    }

    public async Task<PublicProductBook> GetPublicProductBookAsync(string productId, int? limit = null)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetPublicProductBookAsync(productId, limit)).ConfigureAwait(false);
    }

    public async Task<PublicMarketTrades> GetPublicMarketTradesAsync(string productId, int limit, long? start = null, long? end = null)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetPublicMarketTradesAsync(productId, limit, start, end)).ConfigureAwait(false);
    }

    public async Task<List<PublicCandle>> GetPublicProductCandlesAsync(string productId, long start, long end, Enums.Granularity granularity)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetPublicProductCandlesAsync(productId, start, end, granularity)).ConfigureAwait(false);
    }

    private async Task<T> ExecuteWithResilienceAsync<T>(Func<Task<T>> operation)
    {
        // Check circuit breaker state
        if (_circuitOpen && DateTimeOffset.UtcNow - _circuitOpenedAt < _cbPolicy.OpenDuration)
        {
            throw new InvalidOperationException(
                "Circuit breaker is open. Service is unavailable. Retry in " +
                (_cbPolicy.OpenDuration - (DateTimeOffset.UtcNow - _circuitOpenedAt)).TotalSeconds + "s");
        }

        if (_circuitOpen)
        {
            _circuitOpen = false; // Transition to half-open
            _consecutiveFailures = 0;
        }

        // Retry loop with exponential backoff
        Exception? lastException = null;
        var delay = _retryPolicy.InitialDelay;

        for (int attempt = 0; attempt <= _retryPolicy.MaxRetries; attempt++)
        {
            try
            {
                var result = await operation().ConfigureAwait(false);
                
                if (_consecutiveFailures > 0)
                {
                    _consecutiveFailures = 0; // Reset on success
                }

                return result;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _consecutiveFailures++;

                if (_consecutiveFailures >= _cbPolicy.FailureThreshold)
                {
                    _circuitOpen = true;
                    _circuitOpenedAt = DateTimeOffset.UtcNow;
                }

                if (attempt < _retryPolicy.MaxRetries)
                {
                    // Add jitter: delay * 0.8 to 1.2
                    var jitter = 0.8 + (new Random().NextDouble() * 0.4);
                    var nextDelay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * jitter);
                    await Task.Delay(nextDelay).ConfigureAwait(false);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * _retryPolicy.BackoffMultiplier);
                }
            }
        }

        throw new InvalidOperationException(
            $"Operation failed after {_retryPolicy.MaxRetries} retries.",
            lastException);
    }
}

/// <summary>
/// Wrapper around IProductsManager that applies resilience policies.
/// </summary>
internal sealed class ResilientProductsManagerWrapper : IProductsManager
{
    private readonly IProductsManager _baseManager;
    private readonly ResilientManagerFactory.RetryPolicy _retryPolicy;
    private readonly ResilientManagerFactory.CircuitBreakerPolicy _cbPolicy;
    private int _consecutiveFailures;
    private bool _circuitOpen;
    private DateTimeOffset _circuitOpenedAt;

    public ResilientProductsManagerWrapper(
        IProductsManager baseManager,
        ResilientManagerFactory.RetryPolicy retryPolicy,
        ResilientManagerFactory.CircuitBreakerPolicy cbPolicy)
    {
        _baseManager = baseManager;
        _retryPolicy = retryPolicy;
        _cbPolicy = cbPolicy;
        _consecutiveFailures = 0;
        _circuitOpen = false;
    }

    public async Task<List<Product>> ListProductsAsync(string productType = "SPOT")
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.ListProductsAsync(productType)).ConfigureAwait(false);
    }

    public async Task<Product> GetProductAsync(string productId)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetProductAsync(productId)).ConfigureAwait(false);
    }

    public async Task<List<Candle>> GetProductCandlesAsync(string productId, string start, string end, Enums.Granularity granularity)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetProductCandlesAsync(productId, start, end, granularity)).ConfigureAwait(false);
    }

    public async Task<MarketTrades> GetMarketTradesAsync(string productId, int limit)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetMarketTradesAsync(productId, limit)).ConfigureAwait(false);
    }

    public async Task<ProductBook> GetProductBookAsync(string productId, int limit = 25)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetProductBookAsync(productId, limit)).ConfigureAwait(false);
    }

    public async Task<List<ProductBook>> GetBestBidAskAsync(List<string> productIds)
    {
        return await ExecuteWithResilienceAsync(
            () => _baseManager.GetBestBidAskAsync(productIds)).ConfigureAwait(false);
    }

    private async Task<T> ExecuteWithResilienceAsync<T>(Func<Task<T>> operation)
    {
        // Check circuit breaker state
        if (_circuitOpen && DateTimeOffset.UtcNow - _circuitOpenedAt < _cbPolicy.OpenDuration)
        {
            throw new InvalidOperationException(
                "Circuit breaker is open. Service is unavailable. Retry in " +
                (_cbPolicy.OpenDuration - (DateTimeOffset.UtcNow - _circuitOpenedAt)).TotalSeconds + "s");
        }

        if (_circuitOpen)
        {
            _circuitOpen = false; // Transition to half-open
            _consecutiveFailures = 0;
        }

        // Retry loop with exponential backoff
        Exception? lastException = null;
        var delay = _retryPolicy.InitialDelay;

        for (int attempt = 0; attempt <= _retryPolicy.MaxRetries; attempt++)
        {
            try
            {
                var result = await operation().ConfigureAwait(false);
                
                if (_consecutiveFailures > 0)
                {
                    _consecutiveFailures = 0; // Reset on success
                }

                return result;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _consecutiveFailures++;

                if (_consecutiveFailures >= _cbPolicy.FailureThreshold)
                {
                    _circuitOpen = true;
                    _circuitOpenedAt = DateTimeOffset.UtcNow;
                }

                if (attempt < _retryPolicy.MaxRetries)
                {
                    // Add jitter: delay * 0.8 to 1.2
                    var jitter = 0.8 + (new Random().NextDouble() * 0.4);
                    var nextDelay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * jitter);
                    await Task.Delay(nextDelay).ConfigureAwait(false);
                    delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * _retryPolicy.BackoffMultiplier);
                }
            }
        }

        throw new InvalidOperationException(
            $"Operation failed after {_retryPolicy.MaxRetries} retries.",
            lastException);
    }
}
}
