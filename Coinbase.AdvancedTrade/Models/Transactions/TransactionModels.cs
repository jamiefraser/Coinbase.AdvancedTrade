using System;
using Coinbase.AdvancedTrade.Models;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.Models.Transactions
{
    /// <summary>
    /// Represents a transaction in the account.
    /// </summary>
    public sealed class Transaction
    {
        /// <summary>
        /// Gets or sets the transaction ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the transaction type (e.g., "send", "buy", "sell", "fiat_deposit", "fiat_withdrawal").
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the transaction status (e.g., "pending", "completed", "failed", "expired", "canceled").
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the transaction amount.
        /// </summary>
        [JsonProperty("amount")]
        public TransactionAmount Amount { get; set; }

        /// <summary>
        /// Gets or sets the native amount (in user's local currency).
        /// </summary>
        [JsonProperty("native_amount")]
        public TransactionAmount NativeAmount { get; set; }

        /// <summary>
        /// Gets or sets the transaction description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets when the transaction was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the transaction was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

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
        /// Gets or sets whether this transaction is instant.
        /// </summary>
        [JsonProperty("instant_exchange")]
        public bool InstantExchange { get; set; }

        /// <summary>
        /// Gets or sets the network information.
        /// </summary>
        [JsonProperty("network")]
        public TransactionNetwork Network { get; set; }

        /// <summary>
        /// Gets or sets the sender information (for receives).
        /// </summary>
        [JsonProperty("from")]
        public TransactionParty From { get; set; }

        /// <summary>
        /// Gets or sets the recipient information (for sends).
        /// </summary>
        [JsonProperty("to")]
        public TransactionParty To { get; set; }

        /// <summary>
        /// Gets or sets additional transaction details.
        /// </summary>
        [JsonProperty("details")]
        public TransactionDetails Details { get; set; }

        /// <summary>
        /// Gets or sets whether the transaction hides the native amount.
        /// </summary>
        [JsonProperty("hide_native_amount")]
        public bool HideNativeAmount { get; set; }
    }

    /// <summary>
    /// Represents a transaction amount.
    /// </summary>
    public sealed class TransactionAmount
    {
        /// <summary>
        /// Gets or sets the amount value.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    /// <summary>
    /// Represents network information for a transaction.
    /// </summary>
    public sealed class TransactionNetwork
    {
        /// <summary>
        /// Gets or sets the transaction status in the network.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the transaction hash.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the number of confirmations.
        /// </summary>
        [JsonProperty("confirmations")]
        public int Confirmations { get; set; }
    }

    /// <summary>
    /// Represents a party in a transaction (sender or recipient).
    /// </summary>
    public sealed class TransactionParty
    {
        /// <summary>
        /// Gets or sets the party ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the party resource type.
        /// </summary>
        [JsonProperty("resource")]
        public string Resource { get; set; }

        /// <summary>
        /// Gets or sets the party resource path.
        /// </summary>
        [JsonProperty("resource_path")]
        public string ResourcePath { get; set; }

        /// <summary>
        /// Gets or sets the cryptocurrency address (for external addresses).
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    /// <summary>
    /// Additional transaction details.
    /// </summary>
    public sealed class TransactionDetails
    {
        /// <summary>
        /// Gets or sets the title of the transaction.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle.
        /// </summary>
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        [JsonProperty("header")]
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the health status.
        /// </summary>
        [JsonProperty("health")]
        public string Health { get; set; }

        /// <summary>
        /// Gets or sets the payment method name.
        /// </summary>
        [JsonProperty("payment_method_name")]
        public string PaymentMethodName { get; set; }
    }

    /// <summary>
    /// Response from listing transactions.
    /// </summary>
    public sealed class ListTransactionsResponse : IPaginatedResponse<Transaction>
    {
        /// <summary>
        /// Gets or sets the list of transactions.
        /// </summary>
        [JsonProperty("transactions")]
        public Transaction[] Transactions { get; set; }

        /// <summary>
        /// Gets or sets pagination information.
        /// </summary>
        [JsonProperty("pagination")]
        public PaginationInfo Pagination { get; set; }

        // IPaginatedResponse implementation
        System.Collections.Generic.IReadOnlyList<Transaction> IPaginatedResponse<Transaction>.Items => Transactions;
        bool IPaginatedResponse<Transaction>.HasMore => Pagination?.NextUri != null;
        string IPaginatedResponse<Transaction>.Cursor => Pagination?.NextUri;
        int? IPaginatedResponse<Transaction>.TotalCount => null;
    }

    /// <summary>
    /// Pagination information for transactions.
    /// </summary>
    public sealed class PaginationInfo
    {
        /// <summary>
        /// Gets or sets the ending before cursor.
        /// </summary>
        [JsonProperty("ending_before")]
        public string EndingBefore { get; set; }

        /// <summary>
        /// Gets or sets the starting after cursor.
        /// </summary>
        [JsonProperty("starting_after")]
        public string StartingAfter { get; set; }

        /// <summary>
        /// Gets or sets the limit per page.
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets the sort order.
        /// </summary>
        [JsonProperty("order")]
        public string Order { get; set; }

        /// <summary>
        /// Gets or sets the previous page URI.
        /// </summary>
        [JsonProperty("previous_uri")]
        public string PreviousUri { get; set; }

        /// <summary>
        /// Gets or sets the next page URI.
        /// </summary>
        [JsonProperty("next_uri")]
        public string NextUri { get; set; }
    }

    /// <summary>
    /// Request parameters for listing transactions.
    /// </summary>
    public sealed class ListTransactionsRequest : PaginationRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListTransactionsRequest"/> class.
        /// </summary>
        /// <param name="limit">Maximum number of transactions to return.</param>
        /// <param name="cursor">Pagination cursor from previous response.</param>
        public ListTransactionsRequest(int? limit = null, string cursor = null)
            : base(limit, cursor)
        {
        }
    }

    /// <summary>
    /// Response from getting a single transaction.
    /// </summary>
    public sealed class GetTransactionResponse
    {
        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        [JsonProperty("transaction")]
        public Transaction Transaction { get; set; }
    }
}
