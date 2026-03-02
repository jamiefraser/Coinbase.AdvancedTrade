using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models.Transfers;
using Coinbase.AdvancedTrade.Utilities;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for transfer API operations.
    /// </summary>
    public sealed class TransfersManager : BaseManager, ITransfersManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransfersManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public TransfersManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<ListTransfersResponse> ListTransfersAsync(
            ListTransfersRequest request = null,
            CancellationToken ct = default)
        {
            try
            {
                var parameters = new Dictionary<string, string>();

                if (request != null)
                {
                    if (!string.IsNullOrEmpty(request.Cursor))
                        parameters["cursor"] = request.Cursor;

                    if (request.Limit.HasValue)
                        parameters["limit"] = request.Limit.Value.ToString();

                    if (!string.IsNullOrEmpty(request.Type))
                        parameters["type"] = request.Type;

                    if (request.StartDate.HasValue)
                        parameters["start_date"] = request.StartDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");

                    if (request.EndDate.HasValue)
                        parameters["end_date"] = request.EndDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    "/api/v3/brokerage/transfers",
                    parameters) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ListTransfersResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to list transfers", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CreateDepositResponse> CreateDepositAsync(
            string accountId,
            CreateDepositRequest request,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(request.Amount))
                throw new ArgumentException("Amount is required", nameof(request));

            if (string.IsNullOrEmpty(request.Currency))
                throw new ArgumentException("Currency is required", nameof(request));

            if (string.IsNullOrEmpty(request.PaymentMethodId))
                throw new ArgumentException("PaymentMethodId is required", nameof(request));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    $"/api/v3/brokerage/accounts/{accountId}/deposits",
                    bodyObj: request) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<CreateDepositResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create deposit for account {accountId}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
            string accountId,
            CreateWithdrawalRequest request,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrEmpty(request.Amount))
                throw new ArgumentException("Amount is required", nameof(request));

            if (string.IsNullOrEmpty(request.Currency))
                throw new ArgumentException("Currency is required", nameof(request));

            if (string.IsNullOrEmpty(request.PaymentMethodId))
                throw new ArgumentException("PaymentMethodId is required", nameof(request));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    $"/api/v3/brokerage/accounts/{accountId}/withdrawals",
                    bodyObj: request) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<CreateWithdrawalResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create withdrawal for account {accountId}", ex);
            }
        }
    }
}
