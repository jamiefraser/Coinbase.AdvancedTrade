using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coinbase.AdvancedTrade.Tests.ExchangeManagers
{
    /// <summary>
    /// Test implementation of CoinbaseAuthenticator for unit testing.
    /// </summary>
    public class TestCoinbaseAuthenticator : CoinbaseAuthenticator
    {
        private Func<string, string, Dictionary<string, string>, object, Task<Dictionary<string, object>>>? _sendAuthenticatedRequestAsyncImplementation;

        public TestCoinbaseAuthenticator() : base("test-key", "test-secret")
        {
        }

        /// <summary>
        /// Allows tests to provide a custom implementation for SendAuthenticatedRequestAsync.
        /// </summary>
        public void SetupSendAuthenticatedRequestAsync(Func<string, string, Dictionary<string, string>, object, Task<Dictionary<string, object>>> implementation)
        {
            _sendAuthenticatedRequestAsyncImplementation = implementation;
        }

        public override async Task<Dictionary<string, object>> SendAuthenticatedRequestAsync(string method, string path, Dictionary<string, string> queryParams = null, object bodyObj = null)
        {
            if (_sendAuthenticatedRequestAsyncImplementation != null)
            {
                return await _sendAuthenticatedRequestAsyncImplementation(method, path, queryParams, bodyObj);
            }

            // Default implementation returns empty dictionary
            return new Dictionary<string, object>();
        }
    }
}
