using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models.Portfolios;

namespace Coinbase.AdvancedTrade.Interfaces
{
    /// <summary>
    /// Interface for managing portfolios through the Coinbase API.
    /// </summary>
    public interface IPortfoliosManager
    {
        /// <summary>
        /// Lists all portfolios for the current user.
        /// </summary>
        /// <param name="request">Optional request parameters for filtering and pagination.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A paginated list of portfolios.</returns>
        Task<ListPortfoliosResponse> ListPortfoliosAsync(
            ListPortfoliosRequest request = null,
            CancellationToken ct = default);

        /// <summary>
        /// Creates a new portfolio.
        /// </summary>
        /// <param name="request">The portfolio creation request.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created portfolio.</returns>
        Task<CreatePortfolioResponse> CreatePortfolioAsync(
            CreatePortfolioRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Gets a specific portfolio by its UUID.
        /// </summary>
        /// <param name="portfolioUuid">The UUID of the portfolio.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The requested portfolio.</returns>
        Task<Portfolio> GetPortfolioAsync(
            string portfolioUuid,
            CancellationToken ct = default);

        /// <summary>
        /// Edits an existing portfolio.
        /// </summary>
        /// <param name="portfolioUuid">The UUID of the portfolio to edit.</param>
        /// <param name="request">The edit request with new values.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The updated portfolio.</returns>
        Task<EditPortfolioResponse> EditPortfolioAsync(
            string portfolioUuid,
            EditPortfolioRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Deletes a portfolio.
        /// </summary>
        /// <param name="portfolioUuid">The UUID of the portfolio to delete.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A response indicating success or failure.</returns>
        Task<DeletePortfolioResponse> DeletePortfolioAsync(
            string portfolioUuid,
            CancellationToken ct = default);

        /// <summary>
        /// Moves funds between two portfolios.
        /// </summary>
        /// <param name="portfolioUuid">The source portfolio UUID.</param>
        /// <param name="request">The move funds request.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A response with source and target portfolio UUIDs.</returns>
        Task<MoveFundsResponse> MoveFundsAsync(
            string portfolioUuid,
            MoveFundsRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Gets a detailed breakdown of a portfolio including all positions.
        /// </summary>
        /// <param name="portfolioUuid">The UUID of the portfolio.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Detailed portfolio breakdown.</returns>
        Task<PortfolioBreakdown> GetPortfolioBreakdownAsync(
            string portfolioUuid,
            CancellationToken ct = default);
    }
}
