using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models.Convert;

namespace Coinbase.AdvancedTrade.Interfaces
{
    /// <summary>
    /// Interface for currency conversion operations through the Coinbase API.
    /// </summary>
    public interface IConvertManager
    {
        /// <summary>
        /// Creates a conversion quote between two currencies.
        /// </summary>
        /// <param name="request">The conversion quote request.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A conversion quote that can be committed.</returns>
        Task<ConvertQuoteResponse> CreateConvertQuoteAsync(
            CreateConvertQuoteRequest request,
            CancellationToken ct = default);

        /// <summary>
        /// Gets details of a specific conversion trade.
        /// </summary>
        /// <param name="tradeId">The trade ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The conversion trade details.</returns>
        Task<GetConvertTradeResponse> GetConvertTradeAsync(
            string tradeId,
            CancellationToken ct = default);

        /// <summary>
        /// Commits a conversion quote to execute the trade.
        /// </summary>
        /// <param name="tradeId">The quote/trade ID to commit.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The committed trade details.</returns>
        Task<CommitConvertTradeResponse> CommitConvertTradeAsync(
            string tradeId,
            CancellationToken ct = default);
    }
}
