using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Utilities;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for perpetuals trading operations.
    /// </summary>
    public sealed class PerpetualsManager : BaseManager, IPerpetualsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PerpetualsManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public PerpetualsManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<PerpetualsPortfolioResponse> GetPortfolioAsync(
            string portfolioId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioId))
                throw new ArgumentNullException(nameof(portfolioId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/intx/portfolio/{portfolioId}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<PerpetualsPortfolioResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get perpetuals portfolio {portfolioId}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ListPerpetualsPositionsResponse> ListPositionsAsync(
            string portfolioId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioId))
                throw new ArgumentNullException(nameof(portfolioId));

            try
            {
                var parameters = new Dictionary<string, string>
                {
                    ["portfolio_id"] = portfolioId
                };

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    "/api/v3/brokerage/intx/positions",
                    parameters) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ListPerpetualsPositionsResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to list perpetuals positions for portfolio {portfolioId}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<PerpetualsPositionResponse> GetPositionAsync(
            string portfolioId,
            string symbol,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioId))
                throw new ArgumentNullException(nameof(portfolioId));
            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentNullException(nameof(symbol));

            try
            {
                var parameters = new Dictionary<string, string>
                {
                    ["portfolio_id"] = portfolioId,
                    ["symbol"] = symbol
                };

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/intx/positions/{symbol}",
                    parameters) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<PerpetualsPositionResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get perpetuals position for {symbol}", ex);
            }
        }
    }

    /// <summary>
    /// Interface for perpetuals trading operations.
    /// </summary>
    public interface IPerpetualsManager
    {
        /// <summary>
        /// Gets a perpetuals portfolio.
        /// </summary>
        Task<PerpetualsPortfolioResponse> GetPortfolioAsync(string portfolioId, CancellationToken ct = default);

        /// <summary>
        /// Lists perpetuals positions for a portfolio.
        /// </summary>
        Task<ListPerpetualsPositionsResponse> ListPositionsAsync(string portfolioId, CancellationToken ct = default);

        /// <summary>
        /// Gets a specific perpetuals position.
        /// </summary>
        Task<PerpetualsPositionResponse> GetPositionAsync(string portfolioId, string symbol, CancellationToken ct = default);
    }

    /// <summary>
    /// Response containing perpetuals portfolio details.
    /// </summary>
    public sealed class PerpetualsPortfolioResponse
    {
        [JsonProperty("portfolio")]
        public PerpetualsPortfolio Portfolio { get; set; }
    }

    /// <summary>
    /// Perpetuals portfolio details.
    /// </summary>
    public sealed class PerpetualsPortfolio
    {
        [JsonProperty("portfolio_id")]
        public string PortfolioId { get; set; }

        [JsonProperty("collateral")]
        public string Collateral { get; set; }

        [JsonProperty("position_notional")]
        public string PositionNotional { get; set; }

        [JsonProperty("open_position_notional")]
        public string OpenPositionNotional { get; set; }

        [JsonProperty("pending_fees")]
        public string PendingFees { get; set; }

        [JsonProperty("borrow")]
        public string Borrow { get; set; }

        [JsonProperty("accrued_interest")]
        public string AccruedInterest { get; set; }

        [JsonProperty("rolling_debt")]
        public string RollingDebt { get; set; }

        [JsonProperty("portfolio_initial_margin")]
        public string PortfolioInitialMargin { get; set; }

        [JsonProperty("portfolio_im_notional")]
        public string PortfolioImNotional { get; set; }

        [JsonProperty("liquidation_percentage")]
        public string LiquidationPercentage { get; set; }

        [JsonProperty("liquidation_buffer")]
        public string LiquidationBuffer { get; set; }

        [JsonProperty("margin_type")]
        public string MarginType { get; set; }

        [JsonProperty("margin_flags")]
        public string MarginFlags { get; set; }

        [JsonProperty("portfolio_maintenance_margin")]
        public string PortfolioMaintenanceMargin { get; set; }
    }

    /// <summary>
    /// Response containing list of perpetuals positions.
    /// </summary>
    public sealed class ListPerpetualsPositionsResponse
    {
        [JsonProperty("positions")]
        public PerpetualsPosition[] Positions { get; set; }
    }

    /// <summary>
    /// Response containing a single perpetuals position.
    /// </summary>
    public sealed class PerpetualsPositionResponse
    {
        [JsonProperty("position")]
        public PerpetualsPosition Position { get; set; }
    }

    /// <summary>
    /// Perpetuals position details.
    /// </summary>
    public sealed class PerpetualsPosition
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("size")]
        public string Size { get; set; }

        [JsonProperty("entry_vwap")]
        public string EntryVwap { get; set; }

        [JsonProperty("unrealized_pnl")]
        public string UnrealizedPnl { get; set; }

        [JsonProperty("net_funding")]
        public string NetFunding { get; set; }

        [JsonProperty("mark_price")]
        public string MarkPrice { get; set; }

        [JsonProperty("liquidation_price")]
        public string LiquidationPrice { get; set; }

        [JsonProperty("leverage")]
        public string Leverage { get; set; }

        [JsonProperty("im_contribution")]
        public string ImContribution { get; set; }

        [JsonProperty("unrealized_funding")]
        public string UnrealizedFunding { get; set; }

        [JsonProperty("asset")]
        public string Asset { get; set; }

        [JsonProperty("collateral_used")]
        public string CollateralUsed { get; set; }
    }
}
