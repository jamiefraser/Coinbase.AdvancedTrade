using System;
using Newtonsoft.Json;

namespace Coinbase.AdvancedTrade.Models.PaymentMethods
{
    /// <summary>
    /// Represents a payment method for the user.
    /// </summary>
    public sealed class PaymentMethod
    {
        /// <summary>
        /// Gets or sets the unique identifier for the payment method.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the payment method type (e.g., "ach_bank_account", "fiat_account").
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the payment method.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Gets or sets whether this is the primary payment method.
        /// </summary>
        [JsonProperty("primary_buy")]
        public bool PrimaryBuy { get; set; }

        /// <summary>
        /// Gets or sets whether this payment method allows buying.
        /// </summary>
        [JsonProperty("allow_buy")]
        public bool AllowBuy { get; set; }

        /// <summary>
        /// Gets or sets whether this payment method allows selling.
        /// </summary>
        [JsonProperty("allow_sell")]
        public bool AllowSell { get; set; }

        /// <summary>
        /// Gets or sets whether this payment method allows deposits.
        /// </summary>
        [JsonProperty("allow_deposit")]
        public bool AllowDeposit { get; set; }

        /// <summary>
        /// Gets or sets whether this payment method allows withdrawals.
        /// </summary>
        [JsonProperty("allow_withdraw")]
        public bool AllowWithdraw { get; set; }

        /// <summary>
        /// Gets or sets when this payment method was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when this payment method was last updated.
        /// </summary>
        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets whether the payment method is verified.
        /// </summary>
        [JsonProperty("verified")]
        public bool Verified { get; set; }

        /// <summary>
        /// Gets or sets the payment method limits.
        /// </summary>
        [JsonProperty("limits")]
        public PaymentMethodLimits Limits { get; set; }

        /// <summary>
        /// Gets or sets whether instant buy is available.
        /// </summary>
        [JsonProperty("instant_buy")]
        public bool InstantBuy { get; set; }

        /// <summary>
        /// Gets or sets whether instant sell is available.
        /// </summary>
        [JsonProperty("instant_sell")]
        public bool InstantSell { get; set; }
    }

    /// <summary>
    /// Payment method transaction limits.
    /// </summary>
    public sealed class PaymentMethodLimits
    {
        /// <summary>
        /// Gets or sets the buy limit.
        /// </summary>
        [JsonProperty("buy")]
        public LimitAmount[] Buy { get; set; }

        /// <summary>
        /// Gets or sets the instant buy limit.
        /// </summary>
        [JsonProperty("instant_buy")]
        public LimitAmount[] InstantBuy { get; set; }

        /// <summary>
        /// Gets or sets the sell limit.
        /// </summary>
        [JsonProperty("sell")]
        public LimitAmount[] Sell { get; set; }

        /// <summary>
        /// Gets or sets the deposit limit.
        /// </summary>
        [JsonProperty("deposit")]
        public LimitAmount[] Deposit { get; set; }
    }

    /// <summary>
    /// Represents a limit amount.
    /// </summary>
    public sealed class LimitAmount
    {
        /// <summary>
        /// Gets or sets the period type (e.g., "day", "month").
        /// </summary>
        [JsonProperty("period_in_days")]
        public int PeriodInDays { get; set; }

        /// <summary>
        /// Gets or sets the total amount limit.
        /// </summary>
        [JsonProperty("total")]
        public MoneyAmount Total { get; set; }

        /// <summary>
        /// Gets or sets the remaining amount.
        /// </summary>
        [JsonProperty("remaining")]
        public MoneyAmount Remaining { get; set; }
    }

    /// <summary>
    /// Represents a money amount with currency.
    /// </summary>
    public sealed class MoneyAmount
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
    /// Response from listing payment methods.
    /// </summary>
    public sealed class ListPaymentMethodsResponse
    {
        /// <summary>
        /// Gets or sets the list of payment methods.
        /// </summary>
        [JsonProperty("payment_methods")]
        public PaymentMethod[] PaymentMethods { get; set; }
    }

    /// <summary>
    /// Response from getting a single payment method.
    /// </summary>
    public sealed class GetPaymentMethodResponse
    {
        /// <summary>
        /// Gets or sets the payment method.
        /// </summary>
        [JsonProperty("payment_method")]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
