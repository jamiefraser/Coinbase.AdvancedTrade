using Coinbase.AdvancedTrade.Enums;
using Coinbase.AdvancedTrade.ExchangeManagers;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models;

namespace Coinbase.AdvancedTrade
{
    /// <summary>
    /// Provides access to various functionalities of the Coinbase API.
    /// </summary>
    public sealed class CoinbaseClient
    {
        private readonly CoinbaseAuthenticator _authenticator;

        /// <summary>
        /// Gets the accounts manager, responsible for account-related operations.
        /// </summary>
        public IAccountsManager Accounts { get; }

        /// <summary>
        /// Gets the products manager, responsible for product-related operations.
        /// </summary>
        public IProductsManager Products { get; }

        /// <summary>
        /// Gets the orders manager, responsible for order-related operations.
        /// </summary>
        public IOrdersManager Orders { get; }

        /// <summary>
        /// Gets the fees manager, responsible for fee-related operations.
        /// </summary>
        public IFeesManager Fees { get; }

        /// <summary>
        /// Gets the public manager, responsible for public-related operations.
        /// </summary>
        public IPublicManager Public { get; }

        /// <summary>
        /// Gets the portfolios manager, responsible for portfolio-related operations.
        /// </summary>
        public IPortfoliosManager Portfolios { get; }

        /// <summary>
        /// Gets the convert manager, responsible for currency conversion operations.
        /// </summary>
        public IConvertManager Convert { get; }

        /// <summary>
        /// Gets the payment methods manager, responsible for payment method operations.
        /// </summary>
        public IPaymentMethodsManager PaymentMethods { get; }

        /// <summary>
        /// Gets the addresses manager, responsible for cryptocurrency address operations.
        /// </summary>
        public IAddressesManager Addresses { get; }

        /// <summary>
        /// Gets the transactions manager, responsible for transaction operations.
        /// </summary>
        public ITransactionsManager Transactions { get; }

        /// <summary>
        /// Gets the transfers manager, responsible for deposit and withdrawal operations.
        /// </summary>
        public ITransfersManager Transfers { get; }

        /// <summary>
        /// Gets the batch operations manager, responsible for batch order operations.
        /// </summary>
        public IBatchOperationsManager BatchOperations { get; }

        /// <summary>
        /// Gets the futures manager, responsible for futures trading operations.
        /// </summary>
        public IFuturesManager Futures { get; }

        /// <summary>
        /// Gets the perpetuals manager, responsible for perpetuals trading operations.
        /// </summary>
        public IPerpetualsManager Perpetuals { get; }

        /// <summary>
        /// Gets the WebSocket manager, responsible for managing WebSocket connections.
        /// </summary>
        public WebSocketManager WebSocket { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseClient"/> class for interacting with the Coinbase API.
        /// </summary>
        /// <param name="apiKey">The API key for authentication with Coinbase.</param>
        /// <param name="apiSecret">The API secret for authentication with Coinbase.</param>
        /// <param name="websocketBufferSize">The buffer size for WebSocket messages in bytes (Default 5,242,880 bytes/ 5 MB).</param>
        /// <param name="apiKeyType">Specifies the type of API key used. This can be either a Legacy key (Depricated) or a Coinbase Developer Platform (CDP) key.</param>
        /// <param name="environment">The environment to use (Production or Sandbox). Default is Production.</param>
        public CoinbaseClient(
            string apiKey, 
            string apiSecret, 
            int websocketBufferSize = 5 * 1024 * 1024, 
            ApiKeyType apiKeyType = ApiKeyType.CoinbaseDeveloperPlatform,
            Enums.Environment environment = Enums.Environment.Production)
        {
            // Store environment
            Environment = environment;

            // Create an instance of CoinbaseAuthenticator with the provided credentials and API key type
            _authenticator = new CoinbaseAuthenticator(apiKey, apiSecret, apiKeyType);

            // Initialize various service managers with the authenticator
            Accounts = new AccountsManager(_authenticator);
            Products = new ProductsManager(_authenticator);
            Orders = new OrdersManager(_authenticator);
            Fees = new FeesManager(_authenticator);
            Public = new PublicManager(_authenticator);
            Portfolios = new PortfoliosManager(_authenticator);
            Convert = new ConvertManager(_authenticator);
            PaymentMethods = new PaymentMethodsManager(_authenticator);
            Addresses = new AddressesManager(_authenticator);
            Transactions = new TransactionsManager(_authenticator);
            Transfers = new TransfersManager(_authenticator);
            BatchOperations = new BatchOperationsManager(_authenticator);
            Futures = new FuturesManager(_authenticator);
            Perpetuals = new PerpetualsManager(_authenticator);

            // Initialize WebSocketManager for real-time data feed with environment-specific URL
            var wsUrl = environment.GetWebSocketUrl();
            WebSocket = new WebSocketManager(wsUrl, apiKey, apiSecret, websocketBufferSize, apiKeyType);
        }

        /// <summary>
        /// Gets the environment this client is configured for (Production or Sandbox).
        /// </summary>
        public Enums.Environment Environment { get; }

        /// <summary>
        /// Checks if the client is configured for sandbox environment.
        /// </summary>
        /// <returns>True if sandbox, false if production.</returns>
        public bool IsSandbox() => Environment == Enums.Environment.Sandbox;

        /// <summary>
        /// Checks if the client is configured for production environment.
        /// </summary>
        /// <returns>True if production, false if sandbox.</returns>
        public bool IsProduction() => Environment == Enums.Environment.Production;

        /// <summary>
        /// Gets the most recent rate limit information from API responses.
        /// Returns null if no rate limit headers have been captured yet.
        /// </summary>
        /// <returns>RateLimitInfo with limit, remaining, and reset fields, or null if not available.</returns>
        public RateLimitInfo GetRateLimitInfo()
        {
            return _authenticator.LastRateLimitInfo;
        }

    }
}
