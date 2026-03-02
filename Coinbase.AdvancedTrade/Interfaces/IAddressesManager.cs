using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models.Addresses;

namespace Coinbase.AdvancedTrade.Interfaces
{
    /// <summary>
    /// Interface for managing cryptocurrency addresses through the Coinbase API.
    /// </summary>
    public interface IAddressesManager
    {
        /// <summary>
        /// Lists all addresses for a specific account.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>A list of addresses.</returns>
        Task<ListAddressesResponse> ListAddressesAsync(
            string accountId,
            CancellationToken ct = default);

        /// <summary>
        /// Gets a specific address by its ID.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="addressId">The address ID.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The requested address.</returns>
        Task<AddressResponse> GetAddressAsync(
            string accountId,
            string addressId,
            CancellationToken ct = default);

        /// <summary>
        /// Creates a new address for an account.
        /// </summary>
        /// <param name="request">The create address request.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The created address.</returns>
        Task<AddressResponse> CreateAddressAsync(
            CreateAddressRequest request,
            CancellationToken ct = default);
    }
}
