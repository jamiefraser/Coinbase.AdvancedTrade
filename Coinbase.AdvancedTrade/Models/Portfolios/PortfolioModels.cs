using System;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.Models.Portfolios
{
    /// <summary>
    /// Represents a trading portfolio in Coinbase.
    /// </summary>
    public sealed class Portfolio
    {
        /// <summary>
        /// Gets or sets the unique identifier for the portfolio.
        /// </summary>
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        /// <summary>
        /// Gets or sets the name of the portfolio.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the portfolio type (e.g., "DEFAULT", "CONSUMER").
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets whether this portfolio has been deleted.
        /// </summary>
        [JsonProperty("deleted")]
        public bool Deleted { get; set; }
    }

    /// <summary>
    /// Request to create a new portfolio.
    /// </summary>
    public sealed class CreatePortfolioRequest
    {
        /// <summary>
        /// Gets or sets the name for the new portfolio.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreatePortfolioRequest"/> class.
        /// </summary>
        /// <param name="name">The name for the portfolio.</param>
        public CreatePortfolioRequest(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    /// <summary>
    /// Response from creating a portfolio.
    /// </summary>
    public sealed class CreatePortfolioResponse
    {
        /// <summary>
        /// Gets or sets the created portfolio.
        /// </summary>
        [JsonProperty("portfolio")]
        public Portfolio Portfolio { get; set; }
    }

    /// <summary>
    /// Request to edit an existing portfolio.
    /// </summary>
    public sealed class EditPortfolioRequest
    {
        /// <summary>
        /// Gets or sets the new name for the portfolio.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditPortfolioRequest"/> class.
        /// </summary>
        /// <param name="name">The new name for the portfolio.</param>
        public EditPortfolioRequest(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    /// <summary>
    /// Response from editing a portfolio.
    /// </summary>
    public sealed class EditPortfolioResponse
    {
        /// <summary>
        /// Gets or sets the updated portfolio.
        /// </summary>
        [JsonProperty("portfolio")]
        public Portfolio Portfolio { get; set; }
    }

    /// <summary>
    /// Response from deleting a portfolio.
    /// </summary>
    public sealed class DeletePortfolioResponse
    {
        /// <summary>
        /// Gets or sets a value indicating whether the deletion was successful.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    /// <summary>
    /// Request to move funds between portfolios.
    /// </summary>
    public sealed class MoveFundsRequest
    {
        /// <summary>
        /// Gets or sets the source portfolio UUID.
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the target portfolio UUID.
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the amount to transfer.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency to transfer.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveFundsRequest"/> class.
        /// </summary>
        /// <param name="from">Source portfolio UUID.</param>
        /// <param name="to">Target portfolio UUID.</param>
        /// <param name="amount">Amount to transfer.</param>
        /// <param name="currency">Currency to transfer.</param>
        public MoveFundsRequest(string from, string to, decimal amount, string currency)
        {
            From = from ?? throw new ArgumentNullException(nameof(from));
            To = to ?? throw new ArgumentNullException(nameof(to));
            Amount = amount;
            Currency = currency ?? throw new ArgumentNullException(nameof(currency));
        }
    }

    /// <summary>
    /// Response from moving funds between portfolios.
    /// </summary>
    public sealed class MoveFundsResponse
    {
        /// <summary>
        /// Gets or sets the source portfolio UUID.
        /// </summary>
        [JsonProperty("source_portfolio_uuid")]
        public string SourcePortfolioUuid { get; set; }

        /// <summary>
        /// Gets or sets the target portfolio UUID.
        /// </summary>
        [JsonProperty("target_portfolio_uuid")]
        public string TargetPortfolioUuid { get; set; }
    }

    /// <summary>
    /// Detailed breakdown of a portfolio.
    /// </summary>
    public sealed class PortfolioBreakdown
    {
        /// <summary>
        /// Gets or sets the portfolio information.
        /// </summary>
        [JsonProperty("portfolio")]
        public Portfolio Portfolio { get; set; }

        /// <summary>
        /// Gets or sets the overall portfolio balances.
        /// </summary>
        [JsonProperty("portfolio_balances")]
        public PortfolioBalances PortfolioBalances { get; set; }

        /// <summary>
        /// Gets or sets the spot positions in this portfolio.
        /// </summary>
        [JsonProperty("spot_positions")]
        public SpotPosition[] SpotPositions { get; set; }

        /// <summary>
        /// Gets or sets the perpetual positions in this portfolio.
        /// </summary>
        [JsonProperty("perp_positions")]
        public PerpPosition[] PerpPositions { get; set; }

        /// <summary>
        /// Gets or sets the futures positions in this portfolio.
        /// </summary>
        [JsonProperty("futures_positions")]
        public FuturesPosition[] FuturesPositions { get; set; }
    }

    /// <summary>
    /// Portfolio balance information.
    /// </summary>
    public sealed class PortfolioBalances
    {
        /// <summary>
        /// Gets or sets the total balance across all positions.
        /// </summary>
        [JsonProperty("total_balance")]
        public PortfolioBalance TotalBalance { get; set; }

        /// <summary>
        /// Gets or sets the total futures balance.
        /// </summary>
        [JsonProperty("total_futures_balance")]
        public PortfolioBalance TotalFuturesBalance { get; set; }

        /// <summary>
        /// Gets or sets the total cash equivalent balance.
        /// </summary>
        [JsonProperty("total_cash_equivalent_balance")]
        public PortfolioBalance TotalCashEquivalentBalance { get; set; }

        /// <summary>
        /// Gets or sets the total crypto balance.
        /// </summary>
        [JsonProperty("total_crypto_balance")]
        public PortfolioBalance TotalCryptoBalance { get; set; }
    }

    /// <summary>
    /// Individual balance amount with value and currency.
    /// </summary>
    public sealed class PortfolioBalance
    {
        /// <summary>
        /// Gets or sets the balance value.
        /// </summary>
        [JsonProperty("value")]
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    /// <summary>
    /// Represents a spot trading position.
    /// </summary>
    public sealed class SpotPosition
    {
        /// <summary>
        /// Gets or sets the asset symbol.
        /// </summary>
        [JsonProperty("asset")]
        public string Asset { get; set; }

        /// <summary>
        /// Gets or sets the account UUID.
        /// </summary>
        [JsonProperty("account_uuid")]
        public string AccountUuid { get; set; }

        /// <summary>
        /// Gets or sets the total balance of this position.
        /// </summary>
        [JsonProperty("total_balance_fiat")]
        public decimal TotalBalanceFiat { get; set; }

        /// <summary>
        /// Gets or sets the total balance in local currency.
        /// </summary>
        [JsonProperty("total_balance_crypto")]
        public decimal TotalBalanceCrypto { get; set; }

        /// <summary>
        /// Gets or sets the available balance.
        /// </summary>
        [JsonProperty("available_to_trade_fiat")]
        public decimal AvailableToTradeFiat { get; set; }
    }

    /// <summary>
    /// Represents a perpetual futures position.
    /// </summary>
    public sealed class PerpPosition
    {
        /// <summary>
        /// Gets or sets the product ID.
        /// </summary>
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the portfolio UUID.
        /// </summary>
        [JsonProperty("portfolio_uuid")]
        public string PortfolioUuid { get; set; }

        /// <summary>
        /// Gets or sets the symbol.
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        /// Gets or sets the position side (LONG/SHORT).
        /// </summary>
        [JsonProperty("side")]
        public string Side { get; set; }

        /// <summary>
        /// Gets or sets the net size of the position.
        /// </summary>
        [JsonProperty("net_size")]
        public decimal NetSize { get; set; }

        /// <summary>
        /// Gets or sets the unrealized profit/loss.
        /// </summary>
        [JsonProperty("unrealized_pnl")]
        public PortfolioBalance UnrealizedPnl { get; set; }
    }

    /// <summary>
    /// Represents a futures position.
    /// </summary>
    public sealed class FuturesPosition
    {
        /// <summary>
        /// Gets or sets the product ID.
        /// </summary>
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        [JsonProperty("expiration")]
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the position side (LONG/SHORT).
        /// </summary>
        [JsonProperty("side")]
        public string Side { get; set; }

        /// <summary>
        /// Gets or sets the number of contracts.
        /// </summary>
        [JsonProperty("number_of_contracts")]
        public decimal NumberOfContracts { get; set; }

        /// <summary>
        /// Gets or sets the unrealized profit/loss.
        /// </summary>
        [JsonProperty("unrealized_pnl")]
        public PortfolioBalance UnrealizedPnl { get; set; }
    }
}
