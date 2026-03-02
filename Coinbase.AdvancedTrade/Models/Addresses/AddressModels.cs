using System;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.Models.Addresses
{
    /// <summary>
    /// Represents a cryptocurrency address.
    /// </summary>
    public sealed class Address
    {
        /// <summary>
        /// Gets or sets the unique identifier for the address.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the cryptocurrency address string.
        /// </summary>
        [JsonProperty("address")]
        public string AddressString { get; set; }

        /// <summary>
        /// Gets or sets the address name/label.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets when this address was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when this address was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the network name.
        /// </summary>
        [JsonProperty("network")]
        public string Network { get; set; }

        /// <summary>
        /// Gets or sets the URI scheme (e.g., "bitcoin:", "ethereum:").
        /// </summary>
        [JsonProperty("uri_scheme")]
        public string UriScheme { get; set; }

        /// <summary>
        /// Gets or sets the resource type.
        /// </summary>
        [JsonProperty("resource")]
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the resource path.
        /// </summary>
        [JsonProperty("resource_path")]
        public string ResourcePath { get; set; }

        /// <summary>
        /// Gets or sets address warnings.
        /// </summary>
        [JsonProperty("warnings")]
        public AddressWarning[] Warnings { get; set; }

        /// <summary>
        /// Gets or sets whether this is a legacy address.
        /// </summary>
        [JsonProperty("legacy_address")]
        public string LegacyAddress { get; set; }

        /// <summary>
        /// Gets or sets the destination tag (for certain cryptocurrencies like XRP).
        /// </summary>
        [JsonProperty("destination_tag")]
        public string DestinationTag { get; set; }

        /// <summary>
        /// Gets or sets the callback URL for webhooks.
        /// </summary>
        [JsonProperty("callback_url")]
        public string CallbackUrl { get; set; }
    }

    /// <summary>
    /// Represents a warning about an address.
    /// </summary>
    public sealed class AddressWarning
    {
        /// <summary>
        /// Gets or sets the warning title.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the warning details.
        /// </summary>
        [JsonProperty("details")]
        public string Details { get; set; }

        /// <summary>
        /// Gets or sets the image URL for the warning.
        /// </summary>
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
    }

    /// <summary>
    /// Request to create a new address.
    /// </summary>
    public sealed class CreateAddressRequest
    {
        /// <summary>
        /// Gets or sets the account ID.
        /// </summary>
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the address name/label.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateAddressRequest"/> class.
        /// </summary>
        /// <param name="accountId">The account ID.</param>
        /// <param name="name">Optional name/label for the address.</param>
        public CreateAddressRequest(string accountId, string name = null)
        {
            AccountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
            Name = name;
        }
    }

    /// <summary>
    /// Response from listing addresses.
    /// </summary>
    public sealed class ListAddressesResponse
    {
        /// <summary>
        /// Gets or sets the list of addresses.
        /// </summary>
        [JsonProperty("addresses")]
        public Address[] Addresses { get; set; }
    }

    /// <summary>
    /// Response from getting or creating a single address.
    /// </summary>
    public sealed class AddressResponse
    {
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        [JsonProperty("address")]
        public Address Address { get; set; }
    }
}
