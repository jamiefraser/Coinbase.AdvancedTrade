using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models.PaymentMethods;

namespace Coinbase.AdvancedTrade.Interfaces
{
    /// <summary>
    /// Interface for managing payment methods through the Coinbase API.
    /// </summary>
    public interface IPaymentMethodsManager
    {
        /// <summary>
        /// Lists all payment methods for the current user.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of payment methods.</returns>
        Task<ListPaymentMethodsResponse> ListPaymentMethodsAsync(CancellationToken ct = default);

        /// <summary>
        /// Gets a specific payment method by its ID.
        /// </summary>
        /// <param name="paymentMethodId">The ID of the payment method.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The requested payment method.</returns>
        Task<GetPaymentMethodResponse> GetPaymentMethodAsync(
            string paymentMethodId,
            CancellationToken ct = default);
    }
}
