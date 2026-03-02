using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models.Transfers;

namespace Coinbase.AdvancedTrade.Interfaces
{
    /// <summary>
    /// Interface for transfer-related operations.
    /// </summary>
    public interface ITransfersManager
    {
        /// <summary>
        /// Lists transfers for the authenticated user.
        /// </summary>
        /// <param name="request">Pagination and filtering parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paginated list of transfers.</returns>
        Task<ListTransfersResponse> ListTransfersAsync(
            ListTransfersRequest request = null,
            CancellationToken ct = default);

        /// <summary>
        /// Creates a deposit transfer from a payment method to an account.
        /// </summary>
        /// <param name="accountId">The account ID to deposit into.</param>
        /// <param name="request">The deposit request details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created deposit transfer.</returns>
        Task<CreateDepositResponse> CreateDepositAsync(
            string accountId,
            CreateDepositRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Creates a withdrawal transfer from an account to a payment method.
        /// </summary>
        /// <param name="accountId">The account ID to withdraw from.</param>
        /// <param name="request">The withdrawal request details.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created withdrawal transfer.</returns>
        Task<CreateWithdrawalResponse> CreateWithdrawalAsync(
            string accountId,
            CreateWithdrawalRequest request,
            CancellationToken ct = default);
    }
}
