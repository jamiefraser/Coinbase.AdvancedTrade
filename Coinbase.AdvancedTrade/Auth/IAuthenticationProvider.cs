using System.Collections.Generic;

namespace Coinbase.AdvancedTrade.Auth
{
    /// <summary>
    /// Defines the contract for authentication providers that generate headers for Coinbase API requests.
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Generates authentication headers for an HTTP request.
        /// </summary>
        /// <param name="method">The HTTP method (GET, POST, etc.).</param>
        /// <param name="path">The API endpoint path.</param>
        /// <param name="body">The request body, if any.</param>
        /// <returns>A dictionary of headers to add to the request.</returns>
        Dictionary<string, string> GenerateHeaders(string method, string path, string body = null);
    }
}
