using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Coinbase.AdvancedTrade.Auth
{
    /// <summary>
    /// Provides HMAC SHA-256 authentication for legacy Coinbase API keys.
    /// This authentication method is deprecated and will be removed in future versions.
    /// </summary>
    [Obsolete("Legacy API key authentication is deprecated and will be removed in future versions. Use JwtAuthenticationProvider instead.")]
    public sealed class HmacAuthenticationProvider : IAuthenticationProvider
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="HmacAuthenticationProvider"/> class.
        /// </summary>
        /// <param name="apiKey">The legacy API key.</param>
        /// <param name="apiSecret">The legacy API secret.</param>
        public HmacAuthenticationProvider(string apiKey, string apiSecret)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GenerateHeaders(string method, string path, string body = null)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            var message = $"{timestamp}{method.ToUpper()}{path}{body}";
            var signature = GenerateSignature(message);

            return new Dictionary<string, string>
            {
                { "CB-ACCESS-KEY", _apiKey },
                { "CB-ACCESS-SIGN", signature },
                { "CB-ACCESS-TIMESTAMP", timestamp }
            };
        }

        private string GenerateSignature(string message)
        {
            var encoding = new UTF8Encoding();
            var messageBytes = encoding.GetBytes(message);
            var keyBytes = encoding.GetBytes(_apiSecret);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(messageBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
