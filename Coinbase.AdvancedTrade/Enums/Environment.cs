using System;

namespace Coinbase.AdvancedTrade.Enums
{
    /// <summary>
    /// Coinbase API environment (production or sandbox).
    /// </summary>
    public enum Environment
    {
        /// <summary>
        /// Production environment (live trading with real funds).
        /// </summary>
        Production,

        /// <summary>
        /// Sandbox environment (testing with simulated funds).
        /// </summary>
        Sandbox
    }

    /// <summary>
    /// Extension methods for Environment enum.
    /// </summary>
    public static class EnvironmentExtensions
    {
        /// <summary>
        /// Gets the base API URL for the environment.
        /// </summary>
        public static string GetApiUrl(this Environment environment)
        {
            if (environment == Environment.Production)
            {
                return "https://api.coinbase.com";
            }
            else if (environment == Environment.Sandbox)
            {
                return "https://api-sandbox.coinbase.com";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(environment));
            }
        }

        /// <summary>
        /// Gets the WebSocket URL for the environment.
        /// </summary>
        public static string GetWebSocketUrl(this Environment environment)
        {
            if (environment == Environment.Production)
            {
                return "wss://advanced-trade-ws.coinbase.com";
            }
            else if (environment == Environment.Sandbox)
            {
                return "wss://advanced-trade-ws-sandbox.coinbase.com";
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(environment));
            }
        }
    }
}
