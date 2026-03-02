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
    /// Manager for futures trading operations.
    /// </summary>
    public sealed class FuturesManager : BaseManager, IFuturesManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FuturesManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public FuturesManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<FuturesBalanceResponse> GetBalanceSummaryAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    "/api/v3/brokerage/cfm/balance_summary") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<FuturesBalanceResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to get futures balance summary", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<ListFuturesPositionsResponse> ListPositionsAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    "/api/v3/brokerage/cfm/positions") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ListFuturesPositionsResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to list futures positions", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<FuturesPositionResponse> GetPositionAsync(
            string productId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentNullException(nameof(productId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/cfm/positions/{productId}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<FuturesPositionResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get futures position for {productId}", ex);
            }
        }
    }

    /// <summary>
    /// Interface for futures trading operations.
    /// </summary>
    public interface IFuturesManager
    {
        /// <summary>
        /// Gets the futures balance summary.
        /// </summary>
        Task<FuturesBalanceResponse> GetBalanceSummaryAsync(CancellationToken ct = default);

        /// <summary>
        /// Lists all futures positions.
        /// </summary>
        Task<ListFuturesPositionsResponse> ListPositionsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a specific futures position.
        /// </summary>
        Task<FuturesPositionResponse> GetPositionAsync(string productId, CancellationToken ct = default);
    }

    /// <summary>
    /// Response containing futures balance summary.
    /// </summary>
    public sealed class FuturesBalanceResponse
    {
        [JsonProperty("balance")]
        public FuturesBalance Balance { get; set; }
    }

    /// <summary>
    /// Futures balance details.
    /// </summary>
    public sealed class FuturesBalance
    {
        [JsonProperty("total_balance")]
        public string TotalBalance { get; set; }

        [JsonProperty("available_balance")]
        public string AvailableBalance { get; set; }

        [JsonProperty("holds")]
        public string Holds { get; set; }

        [JsonProperty("unrealized_pnl")]
        public string UnrealizedPnl { get; set; }
    }

    /// <summary>
    /// Response containing list of futures positions.
    /// </summary>
    public sealed class ListFuturesPositionsResponse
    {
        [JsonProperty("positions")]
        public FuturesPosition[] Positions { get; set; }
    }

    /// <summary>
    /// Response containing a single futures position.
    /// </summary>
    public sealed class FuturesPositionResponse
    {
        [JsonProperty("position")]
        public FuturesPosition Position { get; set; }
    }

    /// <summary>
    /// Futures position details.
    /// </summary>
    public sealed class FuturesPosition
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("expiration_time")]
        public DateTime? ExpirationTime { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("number_of_contracts")]
        public string NumberOfContracts { get; set; }

        [JsonProperty("current_price")]
        public string CurrentPrice { get; set; }

        [JsonProperty("avg_entry_price")]
        public string AvgEntryPrice { get; set; }

        [JsonProperty("unrealized_pnl")]
        public string UnrealizedPnl { get; set; }

        [JsonProperty("daily_realized_pnl")]
        public string DailyRealizedPnl { get; set; }
    }
}
