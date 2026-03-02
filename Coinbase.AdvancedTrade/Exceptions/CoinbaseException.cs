using System;

namespace Coinbase.AdvancedTrade.Exceptions
{
    /// <summary>
    /// Base exception for all Coinbase SDK errors.
    /// </summary>
    public abstract class CoinbaseException : Exception
    {
        /// <summary>
        /// Gets the correlation ID for this request (if available).
        /// </summary>
        public string CorrelationId { get; }

        /// <summary>
        /// Gets the timestamp when the error occurred.
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="correlationId">Optional correlation ID for tracking this error across systems.</param>
        protected CoinbaseException(string message, string correlationId = null)
            : base(message)
        {
            CorrelationId = correlationId;
            Timestamp = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseException"/> class with a specified inner exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// <param name="correlationId">Optional correlation ID for tracking this error across systems.</param>
        protected CoinbaseException(string message, Exception innerException, string correlationId = null)
            : base(message, innerException)
        {
            CorrelationId = correlationId;
            Timestamp = DateTimeOffset.UtcNow;
        }
    }
}
