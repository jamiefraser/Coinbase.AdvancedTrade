using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models.Transactions;
using Coinbase.AdvancedTrade.Utilities;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for transaction API operations.
    /// </summary>
    public sealed class TransactionsManager : BaseManager, ITransactionsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public TransactionsManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<ListTransactionsResponse> ListTransactionsAsync(
            string accountId,
            ListTransactionsRequest request = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            try
            {
                var parameters = new Dictionary<string, string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.Cursor))
                        parameters["cursor"] = request.Cursor;

                    if (request.Limit.HasValue)
                        parameters["limit"] = request.Limit.Value.ToString();
                }

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/accounts/{accountId}/transactions",
                    parameters) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ListTransactionsResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to list transactions for account {accountId}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<GetTransactionResponse> GetTransactionAsync(
            string accountId,
            string transactionId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            if (string.IsNullOrEmpty(transactionId))
                throw new ArgumentNullException(nameof(transactionId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/accounts/{accountId}/transactions/{transactionId}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<GetTransactionResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get transaction {transactionId} for account {accountId}", ex);
            }
        }
    }
}
