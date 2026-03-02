using System;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.Models
{
    /// <summary>
    /// Represents the current server time from Coinbase API.
    /// Used for clock synchronization and timestamp generation.
    /// </summary>
    public sealed class ServerTime
    {
        /// <summary>
        /// Gets or sets the server time in ISO 8601 format.
        /// </summary>
        [JsonProperty("iso")]
        public string Iso { get; set; }

        /// <summary>
        /// Gets or sets the server time as Unix epoch (seconds since January 1, 1970 UTC).
        /// </summary>
        [JsonProperty("epochSeconds")]
        public long EpochSeconds { get; set; }

        /// <summary>
        /// Gets or sets the server time as Unix epoch in milliseconds.
        /// </summary>
        [JsonProperty("epochMillis")]
        public long EpochMillis { get; set; }

        /// <summary>
        /// Gets the server time as a DateTimeOffset.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset DateTime => DateTimeOffset.FromUnixTimeSeconds(EpochSeconds);

        /// <summary>
        /// Returns a string representation of the server time.
        /// </summary>
        public override string ToString()
        {
            return $"ServerTime: {Iso} (Epoch: {EpochSeconds})";
        }
    }

    /// <summary>
    /// Represents the permissions and capabilities of the current API key.
    /// </summary>
    public sealed class ApiKeyPermissions
    {
        /// <summary>
        /// Gets or sets whether the API key has view/read permissions.
        /// </summary>
        [JsonProperty("can_view")]
        public bool CanView { get; set; }

        /// <summary>
        /// Gets or sets whether the API key has trade permissions.
        /// </summary>
        [JsonProperty("can_trade")]
        public bool CanTrade { get; set; }

        /// <summary>
        /// Gets or sets whether the API key has transfer permissions.
        /// </summary>
        [JsonProperty("can_transfer")]
        public bool CanTransfer { get; set; }

        /// <summary>
        /// Gets or sets the portfolio UUID this key is restricted to (if any).
        /// Null if the key has access to all portfolios.
        /// </summary>
        [JsonProperty("portfolio_uuid")]
        public string PortfolioUuid { get; set; }

        /// <summary>
        /// Gets or sets the portfolio type this key is restricted to (if any).
        /// </summary>
        [JsonProperty("portfolio_type")]
        public string PortfolioType { get; set; }

        /// <summary>
        /// Returns a summary of the API key permissions.
        /// </summary>
        public override string ToString()
        {
            return $"ApiKeyPermissions: View={CanView}, Trade={CanTrade}, Transfer={CanTransfer}, Portfolio={PortfolioUuid ?? "All"}";
        }
    }

    /// <summary>
    /// Represents rate limit information from API response headers.
    /// </summary>
    public sealed class RateLimitInfo
    {
        /// <summary>
        /// Gets or sets the total requests allowed per time window.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets the remaining requests in the current window.
        /// </summary>
        public int Remaining { get; set; }

        /// <summary>
        /// Gets or sets when the rate limit window resets (UTC).
        /// </summary>
        public DateTimeOffset ResetAt { get; set; }

        /// <summary>
        /// Gets whether the rate limit is exhausted.
        /// </summary>
        [JsonIgnore]
        public bool IsExhausted => Remaining <= 0;

        /// <summary>
        /// Gets the time until the rate limit resets.
        /// </summary>
        [JsonIgnore]
        public TimeSpan TimeUntilReset => ResetAt - DateTimeOffset.UtcNow;

        /// <summary>
        /// Returns a string representation of the rate limit info.
        /// </summary>
        public override string ToString()
        {
            return $"RateLimit: {Remaining}/{Limit} remaining, resets at {ResetAt:u}";
        }
    }
}
