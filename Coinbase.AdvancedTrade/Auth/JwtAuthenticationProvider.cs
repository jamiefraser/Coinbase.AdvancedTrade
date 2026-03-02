using System;
using System.Collections.Generic;

namespace Coinbase.AdvancedTrade.Auth
{
    /// <summary>
    /// Provides JWT-based authentication for Coinbase Developer Platform (CDP) API keys.
    /// Uses JWT tokens with ES256 signing for authenticating requests.
    /// </summary>
    public sealed class JwtAuthenticationProvider : IAuthenticationProvider
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthenticationProvider"/> class.
        /// </summary>
        /// <param name="apiKey">The CDP API key (e.g., "organizations/{org_id}/apiKeys/{key_id}").</param>
        /// <param name="apiSecret">The CDP API secret (PEM-formatted EC private key).</param>
        public JwtAuthenticationProvider(string apiKey, string apiSecret)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _apiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GenerateHeaders(string method, string path, string body = null)
        {
            string jwtToken = JwtTokenGenerator.GenerateJwt(_apiKey, _apiSecret, "retail_rest_api_proxy", method, path);
            return new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {jwtToken}" }
            };
        }
    }
}
