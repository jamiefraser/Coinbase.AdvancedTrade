using System;

namespace Coinbase.AdvancedTrade.Configuration
{
    /// <summary>
    /// Configuration for Coinbase SDK environment.
    /// </summary>
    public sealed class CoinbaseConfiguration
    {
        /// <summary>
        /// Gets the production configuration.
        /// </summary>
        public static CoinbaseConfiguration Production { get; } = new CoinbaseConfiguration(
            apiUrl: "https://api.coinbase.com",
            webSocketUrl: "wss://advanced-trade-ws.coinbase.com",
            environment: CoinbaseEnvironment.Production);

        /// <summary>
        /// Gets the sandbox configuration.
        /// </summary>
        public static CoinbaseConfiguration Sandbox { get; } = new CoinbaseConfiguration(
            apiUrl: "https://api-public.sandbox.exchange.coinbase.com",
            webSocketUrl: "wss://ws-feed-public.sandbox.exchange.coinbase.com",
            environment: CoinbaseEnvironment.Sandbox);

        /// <summary>
        /// Gets the API base URL.
        /// </summary>
        public string ApiUrl { get; }

        /// <summary>
        /// Gets the WebSocket URL.
        /// </summary>
        public string WebSocketUrl { get; }

        /// <summary>
        /// Gets the environment type.
        /// </summary>
        public CoinbaseEnvironment Environment { get; }

        /// <summary>
        /// Gets or sets the request timeout.
        /// </summary>
        public TimeSpan RequestTimeout { get; set; }

        /// <summary>
        /// Gets or sets whether to enable request logging.
        /// </summary>
        public bool EnableLogging { get; set; }

        /// <summary>
        /// Gets or sets whether to enable automatic retry.
        /// </summary>
        public bool EnableAutoRetry { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of retry attempts.
        /// </summary>
        public int MaxRetryAttempts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseConfiguration"/> class.
        /// </summary>
        /// <param name="apiUrl">The API base URL.</param>
        /// <param name="webSocketUrl">The WebSocket URL.</param>
        /// <param name="environment">The environment type.</param>
        public CoinbaseConfiguration(
            string apiUrl,
            string webSocketUrl,
            CoinbaseEnvironment environment)
        {
            ApiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
            WebSocketUrl = webSocketUrl ?? throw new ArgumentNullException(nameof(webSocketUrl));
            Environment = environment;

            // Default values
            RequestTimeout = TimeSpan.FromSeconds(30);
            EnableLogging = false;
            EnableAutoRetry = true;
            MaxRetryAttempts = 3;
        }

        /// <summary>
        /// Creates a custom configuration.
        /// </summary>
        /// <param name="apiUrl">The API base URL.</param>
        /// <param name="webSocketUrl">The WebSocket URL.</param>
        /// <returns>A new configuration instance.</returns>
        public static CoinbaseConfiguration Custom(string apiUrl, string webSocketUrl)
        {
            return new CoinbaseConfiguration(apiUrl, webSocketUrl, CoinbaseEnvironment.Custom);
        }
    }

    /// <summary>
    /// Coinbase environment types.
    /// </summary>
    public enum CoinbaseEnvironment
    {
        Production,
        Sandbox,
        Custom
    }
}
