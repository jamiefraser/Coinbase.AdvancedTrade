using System;
using System.Collections.Generic;

namespace Coinbase.AdvancedTrade.Auth
{
    /// <summary>
    /// Provides OAuth2 bearer token authentication for Coinbase API requests.
    /// Note: OAuth2 does not support WebSocket connections.
    /// </summary>
    public sealed class OAuth2AuthenticationProvider : IAuthenticationProvider
    {
        private readonly string _accessToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuth2AuthenticationProvider"/> class.
        /// </summary>
        /// <param name="accessToken">The OAuth2 access token.</param>
        public OAuth2AuthenticationProvider(string accessToken)
        {
            _accessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
        }

        /// <inheritdoc/>
        public Dictionary<string, string> GenerateHeaders(string method, string path, string body = null)
        {
            return new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {_accessToken}" }
            };
        }
    }
}
