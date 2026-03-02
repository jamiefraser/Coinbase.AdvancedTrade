using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Coinbase.AdvancedTrade.RateLimiting
{
    /// <summary>
    /// Represents rate limit information for an endpoint.
    /// </summary>
    public sealed class RateLimitInfo
    {
        /// <summary>
        /// Gets the maximum number of requests allowed in the rate limit window.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets the number of requests remaining in the current window.
        /// </summary>
        public int Remaining { get; set; }

        /// <summary>
        /// Gets the time when the rate limit window resets.
        /// </summary>
        public DateTimeOffset Reset { get; set; }

        /// <summary>
        /// Gets whether the rate limit has been exceeded.
        /// </summary>
        public bool IsExceeded => Remaining <= 0;

        /// <summary>
        /// Gets the time until the rate limit resets.
        /// </summary>
        public TimeSpan TimeUntilReset => Reset - DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets the percentage of quota remaining.
        /// </summary>
        public double PercentageRemaining => Limit > 0 ? (double)Remaining / Limit * 100 : 0;
    }

    /// <summary>
    /// Tracks rate limit information across API endpoints.
    /// </summary>
    public sealed class RateLimitTracker
    {
        private readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimits = new ConcurrentDictionary<string, RateLimitInfo>();
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RateLimitTracker"/> class.
        /// </summary>
        /// <param name="logger">Optional logger for rate limit warnings.</param>
        public RateLimitTracker(ILogger logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Updates rate limit information from a response.
        /// </summary>
        public void UpdateFromResponse(string endpoint, RestResponse response)
        {
            if (response?.Headers == null)
                return;

            var limit = GetHeaderValue(response, "X-RateLimit-Limit");
            var remaining = GetHeaderValue(response, "X-RateLimit-Remaining");
            var reset = GetHeaderValue(response, "X-RateLimit-Reset");

            if (limit == null && remaining == null && reset == null)
                return; // No rate limit headers present

            var info = new RateLimitInfo
            {
                Limit = limit ?? 0,
                Remaining = remaining ?? 0,
                Reset = reset.HasValue 
                    ? DateTimeOffset.FromUnixTimeSeconds(reset.Value)
                    : DateTimeOffset.UtcNow.AddSeconds(60) // Default to 60s if not provided
            };

            _rateLimits[NormalizeEndpoint(endpoint)] = info;

            if (info.IsExceeded)
            {
                _logger?.LogWarning(
                    "Rate limit exceeded for endpoint {Endpoint}. Reset at {ResetTime}",
                    endpoint,
                    info.Reset);
            }
            else if (info.PercentageRemaining < 20)
            {
                var percentage = info.PercentageRemaining.ToString("F1");
                _logger?.LogInformation(
                    "Rate limit low for endpoint {Endpoint}: {Remaining}/{Limit} remaining ({Percentage}%)",
                    endpoint,
                    info.Remaining,
                    info.Limit,
                    percentage);
            }
        }

        /// <summary>
        /// Gets rate limit information for an endpoint.
        /// </summary>
        public RateLimitInfo GetRateLimitInfo(string endpoint)
        {
            _rateLimits.TryGetValue(NormalizeEndpoint(endpoint), out var info);
            return info;
        }

        /// <summary>
        /// Gets whether an endpoint is currently rate limited.
        /// </summary>
        public bool IsRateLimited(string endpoint)
        {
            var info = GetRateLimitInfo(endpoint);
            return info != null && info.IsExceeded && info.TimeUntilReset.TotalSeconds > 0;
        }

        /// <summary>
        /// Gets all tracked rate limit information.
        /// </summary>
        public IReadOnlyDictionary<string, RateLimitInfo> GetAllRateLimits()
        {
            return _rateLimits.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Clears all rate limit tracking data.
        /// </summary>
        public void Clear()
        {
            _rateLimits.Clear();
        }

        /// <summary>
        /// Extracts an integer header value.
        /// </summary>
        private int? GetHeaderValue(RestResponse response, string headerName)
        {
            var header = response.Headers
                ?.FirstOrDefault(h => h.Name?.Equals(headerName, StringComparison.OrdinalIgnoreCase) == true);

            if (header?.Value == null)
                return null;

            if (int.TryParse(header.Value.ToString(), out var value))
                return value;

            return null;
        }

        /// <summary>
        /// Normalizes endpoint paths for consistent tracking.
        /// Removes query parameters and normalizes path separators.
        /// </summary>
        private string NormalizeEndpoint(string endpoint)
        {
            if (string.IsNullOrEmpty(endpoint))
                return string.Empty;

            // Remove query string
            var path = endpoint.Split('?')[0];

            // Remove leading/trailing slashes
            return path.Trim('/').ToLowerInvariant();
        }
    }
}
