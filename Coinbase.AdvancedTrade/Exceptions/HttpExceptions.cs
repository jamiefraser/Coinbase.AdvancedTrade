using System;
using System.Net;

namespace Coinbase.AdvancedTrade.Exceptions
{
    /// <summary>
    /// Exception thrown when the API returns a 400 Bad Request error.
    /// Indicates invalid request parameters or malformed request.
    /// </summary>
    public sealed class BadRequestException : CoinbaseApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BadRequestException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errorCode">Optional API error code.</param>
        /// <param name="errorDetails">Optional error details.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public BadRequestException(string message, string errorCode = null, string errorDetails = null, string correlationId = null)
            : base(HttpStatusCode.BadRequest, message, errorCode, errorDetails, null, correlationId)
        {
        }
    }

    /// <summary>
    /// Exception thrown when the API returns a 401 Unauthorized error.
    /// Indicates missing or invalid authentication credentials.
    /// </summary>
    public sealed class UnauthorizedException : CoinbaseApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnauthorizedException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errorCode">Optional API error code.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public UnauthorizedException(string message, string errorCode = null, string correlationId = null)
            : base(HttpStatusCode.Unauthorized, message, errorCode, null, null, correlationId)
        {
        }
    }

    /// <summary>
    /// Exception thrown when the API returns a 403 Forbidden error.
    /// Indicates the authenticated user lacks permission for the requested operation.
    /// </summary>
    public sealed class ForbiddenException : CoinbaseApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForbiddenException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="errorCode">Optional API error code.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public ForbiddenException(string message, string errorCode = null, string correlationId = null)
            : base(HttpStatusCode.Forbidden, message, errorCode, null, null, correlationId)
        {
        }
    }

    /// <summary>
    /// Exception thrown when the API returns a 404 Not Found error.
    /// Indicates the requested resource does not exist.
    /// </summary>
    public sealed class NotFoundException : CoinbaseApiException
    {
        /// <summary>
        /// Gets the type of resource that was not found.
        /// </summary>
        public string ResourceType { get; }

        /// <summary>
        /// Gets the identifier of the resource that was not found.
        /// </summary>
        public string ResourceId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotFoundException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="resourceType">The type of resource (e.g., "Portfolio", "Order").</param>
        /// <param name="resourceId">The identifier of the missing resource.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public NotFoundException(string message, string resourceType = null, string resourceId = null, string correlationId = null)
            : base(HttpStatusCode.NotFound, message, null, null, null, correlationId)
        {
            ResourceType = resourceType;
            ResourceId = resourceId;
        }
    }

    /// <summary>
    /// Exception thrown when the API returns a 429 Too Many Requests error.
    /// Indicates rate limit has been exceeded.
    /// </summary>
    public sealed class TooManyRequestsException : CoinbaseApiException
    {
        /// <summary>
        /// Gets the number of seconds to wait before retrying (from Retry-After header).
        /// </summary>
        public int? RetryAfterSeconds { get; }

        /// <summary>
        /// Gets the rate limit quota.
        /// </summary>
        public int? RateLimit { get; }

        /// <summary>
        /// Gets the remaining requests in the current window.
        /// </summary>
        public int? RateLimitRemaining { get; }

        /// <summary>
        /// Gets the time when the rate limit resets.
        /// </summary>
        public DateTimeOffset? RateLimitReset { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyRequestsException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="retryAfterSeconds">Number of seconds to wait before retrying.</param>
        /// <param name="rateLimit">The rate limit quota.</param>
        /// <param name="rateLimitRemaining">Remaining requests in current window.</param>
        /// <param name="rateLimitReset">When the rate limit resets.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public TooManyRequestsException(
            string message,
            int? retryAfterSeconds = null,
            int? rateLimit = null,
            int? rateLimitRemaining = null,
            DateTimeOffset? rateLimitReset = null,
            string correlationId = null)
            : base((HttpStatusCode)429, message, "rate_limit_exceeded", null, null, correlationId)
        {
            RetryAfterSeconds = retryAfterSeconds;
            RateLimit = rateLimit;
            RateLimitRemaining = rateLimitRemaining;
            RateLimitReset = rateLimitReset;
        }
    }

    /// <summary>
    /// Exception thrown when the API returns a 500 Internal Server Error.
    /// Indicates an unexpected server-side error.
    /// </summary>
    public sealed class InternalServerErrorException : CoinbaseApiException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalServerErrorException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public InternalServerErrorException(string message, string correlationId = null)
            : base(HttpStatusCode.InternalServerError, message, null, null, null, correlationId)
        {
        }
    }

    /// <summary>
    /// Exception thrown when the API returns a 503 Service Unavailable error.
    /// Indicates the API is temporarily unavailable (maintenance, overload, etc.).
    /// </summary>
    public sealed class ServiceUnavailableException : CoinbaseApiException
    {
        /// <summary>
        /// Gets the number of seconds to wait before retrying (from Retry-After header).
        /// </summary>
        public int? RetryAfterSeconds { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="retryAfterSeconds">Number of seconds to wait before retrying.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public ServiceUnavailableException(string message, int? retryAfterSeconds = null, string correlationId = null)
            : base(HttpStatusCode.ServiceUnavailable, message, null, null, null, correlationId)
        {
            RetryAfterSeconds = retryAfterSeconds;
        }
    }
}
