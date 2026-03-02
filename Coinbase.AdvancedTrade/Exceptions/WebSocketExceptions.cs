using System;

namespace Coinbase.AdvancedTrade.Exceptions
{
    /// <summary>
    /// Base exception for WebSocket-related errors.
    /// </summary>
    public class CoinbaseWebSocketException : CoinbaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseWebSocketException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public CoinbaseWebSocketException(string message, string correlationId = null)
            : base(message, correlationId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseWebSocketException"/> class with an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public CoinbaseWebSocketException(string message, Exception innerException, string correlationId = null)
            : base(message, innerException, correlationId)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a WebSocket connection is unexpectedly closed.
    /// </summary>
    public sealed class ConnectionClosedException : CoinbaseWebSocketException
    {
        /// <summary>
        /// Gets the WebSocket close status code.
        /// </summary>
        public int? CloseStatusCode { get; }

        /// <summary>
        /// Gets the WebSocket close status description.
        /// </summary>
        public string CloseStatusDescription { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionClosedException"/> class.
        /// </summary>
        /// <param name="closeStatusCode">The WebSocket close status code.</param>
        /// <param name="closeStatusDescription">The close status description.</param>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public ConnectionClosedException(
            int? closeStatusCode,
            string closeStatusDescription,
            string message,
            string correlationId = null)
            : base(message, correlationId)
        {
            CloseStatusCode = closeStatusCode;
            CloseStatusDescription = closeStatusDescription;
        }
    }

    /// <summary>
    /// Exception thrown when a WebSocket subscription fails.
    /// </summary>
    public sealed class SubscriptionFailedException : CoinbaseWebSocketException
    {
        /// <summary>
        /// Gets the channel that failed to subscribe.
        /// </summary>
        public string Channel { get; }

        /// <summary>
        /// Gets the product IDs that were attempted to subscribe to.
        /// </summary>
        public string[] ProductIds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionFailedException"/> class.
        /// </summary>
        /// <param name="channel">The channel that failed.</param>
        /// <param name="productIds">The product IDs that were attempted.</param>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public SubscriptionFailedException(
            string channel,
            string[] productIds,
            string message,
            string correlationId = null)
            : base(message, correlationId)
        {
            Channel = channel;
            ProductIds = productIds;
        }
    }

    /// <summary>
    /// Exception thrown when a WebSocket message cannot be parsed.
    /// </summary>
    public sealed class MessageParseException : CoinbaseWebSocketException
    {
        /// <summary>
        /// Gets the raw message that could not be parsed.
        /// </summary>
        public string RawMessage { get; }

        /// <summary>
        /// Gets the expected or detected message type.
        /// </summary>
        public string MessageType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageParseException"/> class.
        /// </summary>
        /// <param name="rawMessage">The raw message that could not be parsed.</param>
        /// <param name="messageType">The message type.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">Optional inner parsing exception.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public MessageParseException(
            string rawMessage,
            string messageType,
            string message,
            Exception innerException = null,
            string correlationId = null)
            : base(message, innerException, correlationId)
        {
            RawMessage = rawMessage;
            MessageType = messageType;
        }
    }
}
