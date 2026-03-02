using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;

namespace Coinbase.AdvancedTrade.Resilience
{
    /// <summary>
    /// Configuration for retry policies.
    /// </summary>
    public sealed class RetryConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum number of retry attempts.
        /// Default: 3
        /// </summary>
        public int MaxRetries { get; set; } = 3;

        /// <summary>
        /// Gets or sets the base delay for exponential backoff (in seconds).
        /// Default: 1 second
        /// </summary>
        public double BaseDelaySeconds { get; set; } = 1.0;

        /// <summary>
        /// Gets or sets the maximum delay between retries (in seconds).
        /// Default: 16 seconds
        /// </summary>
        public double MaxDelaySeconds { get; set; } = 16.0;

        /// <summary>
        /// Gets or sets whether to apply jitter to retry delays.
        /// Default: true
        /// </summary>
        public bool UseJitter { get; set; } = true;

        /// <summary>
        /// Gets or sets the jitter factor (percentage variance).
        /// Default: 0.25 (±25%)
        /// </summary>
        public double JitterFactor { get; set; } = 0.25;

        /// <summary>
        /// Gets or sets whether to retry on rate limit (429) errors.
        /// Default: true
        /// </summary>
        public bool RetryOnRateLimit { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to retry on server errors (5xx).
        /// Default: true
        /// </summary>
        public bool RetryOnServerErrors { get; set; } = true;

        /// <summary>
        /// Gets or sets whether to retry on network errors.
        /// Default: true
        /// </summary>
        public bool RetryOnNetworkErrors { get; set; } = true;
    }

    /// <summary>
    /// Retry policy factory for API requests compatible with Polly 7.x.
    /// </summary>
    public sealed class RetryPolicyFactory
    {
        private readonly RetryConfiguration _config;
        private readonly ILogger _logger;
        private static readonly Random _jitterRandom = new Random();
        private static readonly object _jitterLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicyFactory"/> class.
        /// </summary>
        /// <param name="config">Optional retry configuration. If null, default configuration is used.</param>
        /// <param name="logger">Optional logger for retry events.</param>
        public RetryPolicyFactory(RetryConfiguration config = null, ILogger logger = null)
        {
            _config = config ?? new RetryConfiguration();
            _logger = logger;
        }

        /// <summary>
        /// Creates a retry policy for REST API calls using Polly 7.x.
        /// </summary>
        public IAsyncPolicy<RestResponse> CreateRestApiPolicy()
        {
            return Policy<RestResponse>
                .HandleResult(response => ShouldRetryResponse(response))
                .Or<System.Net.Http.HttpRequestException>()
                .Or<TaskCanceledException>()
                .Or<ConnectionException>()
                .WaitAndRetryAsync(
                    retryCount: _config.MaxRetries,
                    sleepDurationProvider: (retryAttempt, result, context) =>
                    {
                        var delay = CalculateDelay(retryAttempt, result.Result);
                        return delay;
                    },
                    onRetryAsync: (outcome, timespan, attemptNumber, context) =>
                    {
                        LogRetry(attemptNumber, outcome.Result, timespan);
                        return Task.CompletedTask;
                    });
        }

        /// <summary>
        /// Calculates delay with exponential backoff and optional jitter.
        /// Aligned with Python SDK: base * 2^attempt (not attempt-1).
        /// </summary>
        private TimeSpan CalculateDelay(int retryAttempt, RestResponse response)
        {
            // Check for Retry-After header first
            var retryAfter = GetRetryAfterSeconds(response);
            if (retryAfter.HasValue)
            {
                var retryAfterDelay = TimeSpan.FromSeconds(retryAfter.Value);
                return retryAfterDelay > TimeSpan.FromSeconds(_config.MaxDelaySeconds)
                    ? TimeSpan.FromSeconds(_config.MaxDelaySeconds)
                    : retryAfterDelay;
            }

            // Exponential backoff: baseDelay * 2^attempt (matches Python SDK formula)
            var exponentialDelay = _config.BaseDelaySeconds * Math.Pow(2, retryAttempt);

            // Cap at max delay
            var cappedDelay = Math.Min(exponentialDelay, _config.MaxDelaySeconds);

            // Apply jitter if configured
            if (_config.UseJitter)
            {
                double jitterValue;
                lock (_jitterLock)
                {
                    // Random jitter: ±jitterFactor (e.g., ±25%)
                    jitterValue = (_jitterRandom.NextDouble() * 2.0 - 1.0) * _config.JitterFactor;
                }
                cappedDelay = cappedDelay * (1.0 + jitterValue);
            }

            return TimeSpan.FromSeconds(Math.Max(0, cappedDelay));
        }

        /// <summary>
        /// Determines whether a response should be retried.
        /// </summary>
        private bool ShouldRetryResponse(RestResponse response)
        {
            if (response == null)
                return true;

            // Always retry on rate limit (429)
            if (_config.RetryOnRateLimit && (int)response.StatusCode == 429)
                return true;

            // Retry on server errors (500, 502, 503, 504)
            if (_config.RetryOnServerErrors)
            {
                var statusCode = (int)response.StatusCode;
                if (statusCode >= 500 && statusCode <= 504)
                    return true;
            }

            // Retry on specific network-related status codes
            if (_config.RetryOnNetworkErrors)
            {
                if (response.StatusCode == HttpStatusCode.RequestTimeout ||
                    response.StatusCode == HttpStatusCode.GatewayTimeout)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Logs retry attempts.
        /// </summary>
        private void LogRetry(int attemptNumber, RestResponse response, TimeSpan delay)
        {
            if (_logger == null)
                return;

            var statusCode = response?.StatusCode ?? 0;
            var errorMessage = response?.ErrorMessage ?? "Network error";

            _logger.LogWarning(
                "Retry attempt {AttemptNumber} after {DelayMs}ms. Status: {StatusCode}, Error: {ErrorMessage}",
                attemptNumber,
                delay.TotalMilliseconds,
                statusCode,
                errorMessage);
        }

        /// <summary>
        /// Extracts Retry-After value from response headers (in seconds).
        /// </summary>
        public static int? GetRetryAfterSeconds(RestResponse response)
        {
            if (response?.Headers == null)
                return null;

            foreach (var header in response.Headers)
            {
                if (header.Name?.Equals("Retry-After", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (int.TryParse(header.Value?.ToString(), out var seconds))
                        return seconds;

                    // Some APIs use HTTP-date format
                    if (DateTimeOffset.TryParse(header.Value?.ToString(), out var date))
                    {
                        var delay = date - DateTimeOffset.UtcNow;
                        return Math.Max(0, (int)delay.TotalSeconds);
                    }
                }
            }

            return null;
        }
    }
}
