using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Coinbase.AdvancedTrade.Webhooks
{
    /// <summary>
    /// Validates Coinbase webhook signatures.
    /// </summary>
    public sealed class WebhookSignatureValidator
    {
        private readonly string _webhookSecret;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebhookSignatureValidator"/> class.
        /// </summary>
        /// <param name="webhookSecret">The webhook secret from Coinbase.</param>
        public WebhookSignatureValidator(string webhookSecret)
        {
            _webhookSecret = webhookSecret ?? throw new ArgumentNullException(nameof(webhookSecret));
        }

        /// <summary>
        /// Validates a webhook signature.
        /// </summary>
        /// <param name="payload">The raw webhook payload.</param>
        /// <param name="signature">The signature from the X-CB-Signature header.</param>
        /// <param name="timestamp">The timestamp from the X-CB-Timestamp header.</param>
        /// <param name="toleranceSeconds">Tolerance for timestamp validation (default 300 seconds).</param>
        /// <returns>True if the signature is valid, false otherwise.</returns>
        public bool ValidateSignature(
            string payload,
            string signature,
            string timestamp,
            int toleranceSeconds = 300)
        {
            if (string.IsNullOrEmpty(payload))
                throw new ArgumentNullException(nameof(payload));
            if (string.IsNullOrEmpty(signature))
                throw new ArgumentNullException(nameof(signature));
            if (string.IsNullOrEmpty(timestamp))
                throw new ArgumentNullException(nameof(timestamp));

            // Validate timestamp
            if (!long.TryParse(timestamp, out var timestampSeconds))
                return false;

            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (Math.Abs(currentTime - timestampSeconds) > toleranceSeconds)
                return false;

            // Compute expected signature
            var signedPayload = $"{timestamp}{payload}";
            var expectedSignature = ComputeHmacSha256(signedPayload, _webhookSecret);

            // Constant-time comparison to prevent timing attacks
            return ConstantTimeEquals(expectedSignature, signature);
        }

        /// <summary>
        /// Validates webhook headers and payload.
        /// </summary>
        /// <param name="headers">The HTTP headers dictionary.</param>
        /// <param name="payload">The raw webhook payload.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public bool ValidateWebhook(IDictionary<string, string> headers, string payload)
        {
            if (headers == null)
                throw new ArgumentNullException(nameof(headers));

            if (!headers.TryGetValue("X-CB-Signature", out var signature))
                return false;

            if (!headers.TryGetValue("X-CB-Timestamp", out var timestamp))
                return false;

            return ValidateSignature(payload, signature, timestamp);
        }

        private static string ComputeHmacSha256(string data, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static bool ConstantTimeEquals(string a, string b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var result = 0;
            for (var i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }

            return result == 0;
        }
    }

    /// <summary>
    /// Base class for all webhook events.
    /// </summary>
    public abstract class WebhookEventBase
    {
        /// <summary>
        /// Gets or sets the event ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets when the event was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Webhook event for order updates.
    /// </summary>
    public sealed class OrderWebhookEvent : WebhookEventBase
    {
        /// <summary>
        /// Gets or sets the order data.
        /// </summary>
        public OrderWebhookData Data { get; set; }
    }

    /// <summary>
    /// Order data in webhook events.
    /// </summary>
    public sealed class OrderWebhookData
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Side { get; set; }
        public string Status { get; set; }
        public string Price { get; set; }
        public string Size { get; set; }
        public string FilledSize { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? CompletionTime { get; set; }
    }

    /// <summary>
    /// Webhook event for account updates.
    /// </summary>
    public sealed class AccountWebhookEvent : WebhookEventBase
    {
        /// <summary>
        /// Gets or sets the account data.
        /// </summary>
        public AccountWebhookData Data { get; set; }
    }

    /// <summary>
    /// Account data in webhook events.
    /// </summary>
    public sealed class AccountWebhookData
    {
        public string AccountId { get; set; }
        public string Currency { get; set; }
        public string AvailableBalance { get; set; }
        public string HoldBalance { get; set; }
    }
}
