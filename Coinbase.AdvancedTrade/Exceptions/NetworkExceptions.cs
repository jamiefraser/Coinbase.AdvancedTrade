using System;

namespace Coinbase.AdvancedTrade.Exceptions
{
    /// <summary>
    /// Base exception for network-related errors.
    /// </summary>
    public class CoinbaseNetworkException : CoinbaseException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseNetworkException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public CoinbaseNetworkException(string message, string correlationId = null)
            : base(message, correlationId)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseNetworkException"/> class with an inner exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public CoinbaseNetworkException(string message, Exception innerException, string correlationId = null)
            : base(message, innerException, correlationId)
        {
        }
    }

    /// <summary>
    /// Exception thrown when a connection to the API cannot be established.
    /// </summary>
    public sealed class ConnectionException : CoinbaseNetworkException
    {
        /// <summary>
        /// Gets the endpoint that could not be reached.
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionException"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint that could not be reached.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">Optional inner exception.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public ConnectionException(string endpoint, string message, Exception innerException = null, string correlationId = null)
            : base(message, innerException, correlationId)
        {
            Endpoint = endpoint;
        }
    }

    /// <summary>
    /// Exception thrown when a request times out.
    /// </summary>
    public sealed class TimeoutException : CoinbaseNetworkException
    {
        /// <summary>
        /// Gets the timeout duration that was exceeded.
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeoutException"/> class.
        /// </summary>
        /// <param name="timeout">The timeout duration that was exceeded.</param>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public TimeoutException(TimeSpan timeout, string message, string correlationId = null)
            : base(message, correlationId)
        {
            Timeout = timeout;
        }
    }

    /// <summary>
    /// Exception thrown when an SSL/TLS error occurs.
    /// </summary>
    public sealed class SSLException : CoinbaseNetworkException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SSLException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The underlying SSL/TLS exception.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public SSLException(string message, Exception innerException, string correlationId = null)
            : base(message, innerException, correlationId)
        {
        }
    }
}
