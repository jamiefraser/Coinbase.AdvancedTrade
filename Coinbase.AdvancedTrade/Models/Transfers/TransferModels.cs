using System;
using System.Collections.Generic;
using Coinbase.AdvancedTrade.Models;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.Models.Transfers
{
    /// <summary>
    /// Represents a transfer between accounts or to/from external addresses.
    /// </summary>
    public sealed class Transfer
    {
        /// <summary>
        /// Gets or sets the transfer ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the transfer type (e.g., "deposit", "withdraw").
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the transfer status (e.g., "pending", "completed", "failed").
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the transfer amount.
        /// </summary>
        [JsonProperty("amount")]
        public TransferAmount Amount { get; set; }

        /// <summary>
        /// Gets or sets the account ID involved in the transfer.
        /// </summary>
        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets the user ID associated with the transfer.
        /// </summary>
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets when the transfer was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the transfer was completed.
        /// </summary>
        [JsonProperty("completed_at")]
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Gets or sets when the transfer was canceled.
        /// </summary>
        [JsonProperty("canceled_at")]
        public DateTime? CanceledAt { get; set; }

        /// <summary>
        /// Gets or sets the payment method ID used for the transfer.
        /// </summary>
        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets whether the transfer is instant.
        /// </summary>
        [JsonProperty("instant")]
        public bool? Instant { get; set; }

        /// <summary>
        /// Gets or sets the transfer fee.
        /// </summary>
        [JsonProperty("fee")]
        public TransferAmount Fee { get; set; }

        /// <summary>
        /// Gets or sets the payout date for the transfer.
        /// </summary>
        [JsonProperty("payout_at")]
        public DateTime? PayoutAt { get; set; }

        /// <summary>
        /// Gets or sets the detailed description of the transfer.
        /// </summary>
        [JsonProperty("details")]
        public TransferDetails Details { get; set; }
    }

    /// <summary>
    /// Represents an amount in a transfer.
    /// </summary>
    public sealed class TransferAmount
    {
        /// <summary>
        /// Gets or sets the amount value.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    /// <summary>
    /// Represents additional details about a transfer.
    /// </summary>
    public sealed class TransferDetails
    {
        /// <summary>
        /// Gets or sets the title of the transfer.
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the subtitle of the transfer.
        /// </summary>
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets additional metadata as a string.
        /// </summary>
        [JsonProperty("header")]
        public string Header { get; set; }

        /// <summary>
        /// Gets or sets the health status (if applicable).
        /// </summary>
        [JsonProperty("health")]
        public string Health { get; set; }
    }

    /// <summary>
    /// Request to create a deposit transfer from a payment method to an account.
    /// </summary>
    public sealed class CreateDepositRequest
    {
        /// <summary>
        /// Gets or sets the amount to deposit.
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the payment method ID to deposit from.
        /// </summary>
        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets whether this should be committed immediately.
        /// </summary>
        [JsonProperty("commit")]
        public bool? Commit { get; set; }
    }

    /// <summary>
    /// Request to create a withdrawal transfer from an account to a payment method.
    /// </summary>
    public sealed class CreateWithdrawalRequest
    {
        /// <summary>
        /// Gets or sets the amount to withdraw.
        /// </summary>
        [JsonProperty("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets the payment method ID to withdraw to.
        /// </summary>
        [JsonProperty("payment_method_id")]
        public string PaymentMethodId { get; set; }

        /// <summary>
        /// Gets or sets whether this should be committed immediately.
        /// </summary>
        [JsonProperty("commit")]
        public bool? Commit { get; set; }
    }

    /// <summary>
    /// Request to commit a pending transfer.
    /// </summary>
    public sealed class CommitTransferRequest
    {
        /// <summary>
        /// Gets or sets the transfer ID to commit.
        /// </summary>
        [JsonProperty("transfer_id")]
        public string TransferId { get; set; }
    }

    /// <summary>
    /// Response containing a list of transfers.
    /// </summary>
    public sealed class ListTransfersResponse : IPaginatedResponse<Transfer>
    {
        /// <summary>
        /// Gets or sets the array of transfers.
        /// </summary>
        [JsonProperty("transfers")]
        public Transfer[] Transfers { get; set; }

        /// <summary>
        /// Gets or sets the pagination information.
        /// </summary>
        [JsonProperty("pagination")]
        public TransferPaginationInfo Pagination { get; set; }

        // IPaginatedResponse implementation
        System.Collections.Generic.IReadOnlyList<Transfer> IPaginatedResponse<Transfer>.Items => Transfers;
        bool IPaginatedResponse<Transfer>.HasMore => Pagination?.NextUri != null;
        string IPaginatedResponse<Transfer>.Cursor => Pagination?.NextUri;
        int? IPaginatedResponse<Transfer>.TotalCount => null;
    }

    /// <summary>
    /// Pagination information for transfers.
    /// </summary>
    public sealed class TransferPaginationInfo
    {
        /// <summary>
        /// Gets or sets the URI for the next page of results.
        /// </summary>
        [JsonProperty("next_uri")]
        public string NextUri { get; set; }
    }

    /// <summary>
    /// Request parameters for listing transfers.
    /// </summary>
    public sealed class ListTransfersRequest : PaginationRequest
    {
        /// <summary>
        /// Gets or sets the transfer type filter.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the start date filter.
        /// </summary>
        [JsonProperty("start_date")]
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date filter.
        /// </summary>
        [JsonProperty("end_date")]
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// Response containing a single transfer.
    /// </summary>
    public sealed class TransferResponse
    {
        /// <summary>
        /// Gets or sets the transfer data.
        /// </summary>
        [JsonProperty("transfer")]
        public Transfer Transfer { get; set; }
    }

    /// <summary>
    /// Response from creating a deposit.
    /// </summary>
    public sealed class CreateDepositResponse
    {
        /// <summary>
        /// Gets or sets the deposit transfer data.
        /// </summary>
        [JsonProperty("data")]
        public Transfer Data { get; set; }
    }

    /// <summary>
    /// Response from creating a withdrawal.
    /// </summary>
    public sealed class CreateWithdrawalResponse
    {
        /// <summary>
        /// Gets or sets the withdrawal transfer data.
        /// </summary>
        [JsonProperty("data")]
        public Transfer Data { get; set; }
    }

    /// <summary>
    /// Response from committing a transfer.
    /// </summary>
    public sealed class CommitTransferResponse
    {
        /// <summary>
        /// Gets or sets the committed transfer data.
        /// </summary>
        [JsonProperty("data")]
        public Transfer Data { get; set; }
    }
}
