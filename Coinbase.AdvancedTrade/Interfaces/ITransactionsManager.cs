using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models.Transactions;

namespace Coinbase.AdvancedTrade.Interfaces
{
    /// <summary>
    /// Interface for transaction-related operations.
    /// </summary>
    public interface ITransactionsManager
    {
        /// <summary>
        /// Lists transactions for a specific account.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="request">Pagination and filtering parameters.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paginated list of transactions.</returns>
        Task<ListTransactionsResponse> ListTransactionsAsync(
            string accountId,
            ListTransactionsRequest request = null,
            CancellationToken ct = default);

        /// <summary>
        /// Gets a specific transaction by ID.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Transaction details.</returns>
        Task<GetTransactionResponse> GetTransactionAsync(
            string accountId,
            string transactionId,
            CancellationToken ct = default);
    }
}
