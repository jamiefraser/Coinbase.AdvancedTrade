using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models.Addresses;
using Coinbase.AdvancedTrade.Utilities;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for cryptocurrency address API operations.
    /// </summary>
    public sealed class AddressesManager : BaseManager, IAddressesManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddressesManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public AddressesManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<ListAddressesResponse> ListAddressesAsync(
            string accountId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/accounts/{accountId}/addresses") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ListAddressesResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to list addresses for account {accountId}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<AddressResponse> GetAddressAsync(
            string accountId,
            string addressId,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(accountId))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(addressId))
                throw new ArgumentNullException(nameof(addressId));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/accounts/{accountId}/addresses/{addressId}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<AddressResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get address {addressId}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<AddressResponse> CreateAddressAsync(
            CreateAddressRequest request,
            CancellationToken ct = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var body = new Dictionary<string, object>
                {
                    ["account_id"] = request.AccountId
                };

                if (!string.IsNullOrEmpty(request.Name))
                    body["name"] = request.Name;

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    $"/api/v3/brokerage/accounts/{request.AccountId}/addresses",
                    null,
                    body) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<AddressResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create address for account {request.AccountId}", ex);
            }
        }
    }
}
