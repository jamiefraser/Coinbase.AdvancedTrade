using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models.PaymentMethods;
using Coinbase.AdvancedTrade.Utilities;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for payment method API operations.
    /// </summary>
    public sealed class PaymentMethodsManager : BaseManager, IPaymentMethodsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentMethodsManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public PaymentMethodsManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<ListPaymentMethodsResponse> ListPaymentMethodsAsync(CancellationToken ct = default)
        {
            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    "/api/v3/brokerage/payment_methods") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ListPaymentMethodsResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to list payment methods", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<GetPaymentMethodResponse> GetPaymentMethodAsync(
            string paymentMethodId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(paymentMethodId))
                throw new ArgumentNullException(nameof(paymentMethodId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/payment_methods/{paymentMethodId}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<GetPaymentMethodResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get payment method {paymentMethodId}", ex);
            }
        }
    }
}
