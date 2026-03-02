using System;
using System.Net;

namespace Coinbase.AdvancedTrade.Exceptions
{
    /// <summary>
    /// Exception thrown when the Coinbase API returns an error response.
    /// </summary>
    public class CoinbaseApiException : CoinbaseException
    {
        /// <summary>
        /// Gets the HTTP status code returned by the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the error code from the API response (if available).
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets additional error details from the API response (if available).
        /// </summary>
        public string ErrorDetails { get; }

        /// <summary>
        /// Gets the raw response body.
        /// </summary>
        public string ResponseBody { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseApiException"/> class.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned by the API.</param>
        /// <param name="message">The error message.</param>
        /// <param name="errorCode">Optional API-specific error code.</param>
        /// <param name="errorDetails">Optional additional error details.</param>
        /// <param name="responseBody">Optional raw response body.</param>
        /// <param name="correlationId">Optional correlation ID for tracking.</param>
        public CoinbaseApiException(
            HttpStatusCode statusCode,
            string message,
            string errorCode = null,
            string errorDetails = null,
            string responseBody = null,
            string correlationId = null)
            : base(message, correlationId)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            ErrorDetails = errorDetails;
            ResponseBody = responseBody;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseApiException"/> class with an inner exception.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned by the API.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The exception that caused this exception.</param>
        /// <param name="correlationId">Optional correlation ID for tracking.</param>
        public CoinbaseApiException(
            HttpStatusCode statusCode,
            string message,
            Exception innerException,
            string correlationId = null)
            : base(message, innerException, correlationId)
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Returns a string representation of the exception including status code and correlation ID.
        /// </summary>
        /// <returns>A formatted string with exception details.</returns>
        public override string ToString()
        {
            return $"{base.ToString()}, StatusCode: {(int)StatusCode} ({StatusCode}), ErrorCode: {ErrorCode}, CorrelationId: {CorrelationId}";
        }
    }
}
