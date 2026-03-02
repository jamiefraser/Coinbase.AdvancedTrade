using System;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.Models.Convert
{
    /// <summary>
    /// Request to create a conversion quote.
    /// </summary>
    public sealed class CreateConvertQuoteRequest
    {
        /// <summary>
        /// Gets or sets the source account UUID to convert from.
        /// </summary>
        [JsonProperty("from_account")]
        public string FromAccount { get; set; }

        /// <summary>
        /// Gets or sets the target account UUID to convert to.
        /// </summary>
        [JsonProperty("to_account")]
        public string ToAccount { get; set; }

        /// <summary>
        /// Gets or sets the amount to convert.
        /// </summary>
        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets optional trade incentive metadata.
        /// </summary>
        [JsonProperty("trade_incentive_metadata")]
        public TradeIncentiveMetadata TradeIncentiveMetadata { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateConvertQuoteRequest"/> class.
        /// </summary>
        /// <param name="fromAccount">Source account UUID.</param>
        /// <param name="toAccount">Target account UUID.</param>
        /// <param name="amount">Amount to convert.</param>
        public CreateConvertQuoteRequest(string fromAccount, string toAccount, decimal amount)
        {
            FromAccount = fromAccount ?? throw new ArgumentNullException(nameof(fromAccount));
            ToAccount = toAccount ?? throw new ArgumentNullException(nameof(toAccount));
            Amount = amount;
        }
    }

    /// <summary>
    /// Trade incentive metadata for conversions.
    /// </summary>
    public sealed class TradeIncentiveMetadata
    {
        /// <summary>
        /// Gets or sets the user incentive ID.
        /// </summary>
        [JsonProperty("user_incentive_id")]
        public string UserIncentiveId { get; set; }

        /// <summary>
        /// Gets or sets the code value.
        /// </summary>
        [JsonProperty("code_val")]
        public string CodeVal { get; set; }
    }

    /// <summary>
    /// Response containing a conversion quote.
    /// </summary>
    public sealed class ConvertQuoteResponse
    {
        /// <summary>
        /// Gets or sets the conversion trade details.
        /// </summary>
        [JsonProperty("trade")]
        public ConvertTrade Trade { get; set; }
    }

    /// <summary>
    /// Represents a conversion trade or quote.
    /// </summary>
    public sealed class ConvertTrade
    {
        /// <summary>
        /// Gets or sets the trade or quote ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the trade status (e.g., "PENDING", "COMPLETED", "FAILED").
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the user-facing ID for display.
        /// </summary>
        [JsonProperty("user_entered_amount")]
        public ConvertAmount UserEnteredAmount { get; set; }

        /// <summary>
        /// Gets or sets the amount being converted from.
        /// </summary>
        [JsonProperty("amount")]
        public ConvertAmount Amount { get; set; }

        /// <summary>
        /// Gets or sets the amount being converted to.
        /// </summary>
        [JsonProperty("to_amount")]
        public ConvertAmount ToAmount { get; set; }

        /// <summary>
        /// Gets or sets the source currency.
        /// </summary>
        [JsonProperty("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the target currency.
        /// </summary>
        [JsonProperty("to")]
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the conversion price/rate.
        /// </summary>
        [JsonProperty("price")]
        public decimal? Price { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        [JsonProperty("unit_price")]
        public ConvertAmount UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the fees for the conversion.
        /// </summary>
        [JsonProperty("fees")]
        public ConvertAmount[] Fees { get; set; }

        /// <summary>
        /// Gets or sets the total fees.
        /// </summary>
        [JsonProperty("total_fee")]
        public ConvertAmount TotalFee { get; set; }

        /// <summary>
        /// Gets or sets the USD value.
        /// </summary>
        [JsonProperty("usd_value")]
        public ConvertAmount UsdValue { get; set; }

        /// <summary>
        /// Gets or sets the total value.
        /// </summary>
        [JsonProperty("total")]
        public ConvertAmount Total { get; set; }

        /// <summary>
        /// Gets or sets the subtotal value.
        /// </summary>
        [JsonProperty("subtotal")]
        public ConvertAmount Subtotal { get; set; }

        /// <summary>
        /// Gets or sets the cancellation reason (if cancelled).
        /// </summary>
        [JsonProperty("cancellation_reason")]
        public string CancellationReason { get; set; }

        /// <summary>
        /// Gets or sets when the quote was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the quote expires.
        /// </summary>
        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the source account.
        /// </summary>
        [JsonProperty("source")]
        public ConvertAccount Source { get; set; }

        /// <summary>
        /// Gets or sets the target account.
        /// </summary>
        [JsonProperty("target")]
        public ConvertAccount Target { get; set; }
    }

    /// <summary>
    /// Represents an amount in a conversion.
    /// </summary>
    public sealed class ConvertAmount
    {
        /// <summary>
        /// Gets or sets the numeric value.
        /// </summary>
        [JsonProperty("value")]
        public decimal Value { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    /// <summary>
    /// Represents an account in a conversion.
    /// </summary>
    public sealed class ConvertAccount
    {
        /// <summary>
        /// Gets or sets the account ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }
    }

    /// <summary>
    /// Response from committing a conversion quote.
    /// </summary>
    public sealed class CommitConvertTradeResponse
    {
        /// <summary>
        /// Gets or sets the committed trade.
        /// </summary>
        [JsonProperty("trade")]
        public ConvertTrade Trade { get; set; }
    }

    /// <summary>
    /// Response from getting a convert trade.
    /// </summary>
    public sealed class GetConvertTradeResponse
    {
        /// <summary>
        /// Gets or sets the trade.
        /// </summary>
        [JsonProperty("trade")]
        public ConvertTrade Trade { get; set; }
    }
}
