using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models.Convert;
using Coinbase.AdvancedTrade.Utilities;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for currency conversion API operations.
    /// </summary>
    public sealed class ConvertManager : BaseManager, IConvertManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public ConvertManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<ConvertQuoteResponse> CreateConvertQuoteAsync(
            CreateConvertQuoteRequest request,
            CancellationToken ct = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var body = new Dictionary<string, object>
                {
                    ["from_account"] = request.FromAccount,
                    ["to_account"] = request.ToAccount,
                    ["amount"] = request.Amount.ToString()
                };

                if (request.TradeIncentiveMetadata != null)
                {
                    body["trade_incentive_metadata"] = new Dictionary<string, object>
                    {
                        ["user_incentive_id"] = request.TradeIncentiveMetadata.UserIncentiveId,
                        ["code_val"] = request.TradeIncentiveMetadata.CodeVal
                    };
                }

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    "/api/v3/brokerage/convert/quote",
                    null,
                    body) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ConvertQuoteResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create convert quote", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<GetConvertTradeResponse> GetConvertTradeAsync(
            string tradeId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(tradeId))
                throw new ArgumentNullException(nameof(tradeId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/convert/trade/{tradeId}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<GetConvertTradeResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get convert trade {tradeId}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CommitConvertTradeResponse> CommitConvertTradeAsync(
            string tradeId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(tradeId))
                throw new ArgumentNullException(nameof(tradeId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    $"/api/v3/brokerage/convert/trade/{tradeId}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<CommitConvertTradeResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to commit convert trade {tradeId}", ex);
            }
        }
    }
}
