using System;

namespace Coinbase.AdvancedTrade.Exceptions
{
    /// <summary>
    /// Base exception for validation errors.
    /// </summary>
    public class CoinbaseValidationException : CoinbaseException
    {
        /// <summary>
        /// Gets the name of the parameter that failed validation.
        /// </summary>
        public string ParameterName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoinbaseValidationException"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter that failed validation.</param>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public CoinbaseValidationException(string parameterName, string message, string correlationId = null)
            : base(message, correlationId)
        {
            ParameterName = parameterName;
        }
    }

    /// <summary>
    /// Exception thrown when a parameter value is invalid.
    /// </summary>
    public sealed class InvalidParameterException : CoinbaseValidationException
    {
        /// <summary>
        /// Gets the invalid value that was provided.
        /// </summary>
        public object InvalidValue { get; }

        /// <summary>
        /// Gets the expected format or constraint for the parameter.
        /// </summary>
        public string ExpectedFormat { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidParameterException"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the invalid parameter.</param>
        /// <param name="invalidValue">The invalid value that was provided.</param>
        /// <param name="message">The error message.</param>
        /// <param name="expectedFormat">Optional description of the expected format.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public InvalidParameterException(
            string parameterName,
            object invalidValue,
            string message,
            string expectedFormat = null,
            string correlationId = null)
            : base(parameterName, message, correlationId)
        {
            InvalidValue = invalidValue;
            ExpectedFormat = expectedFormat;
        }
    }

    /// <summary>
    /// Exception thrown when a required parameter is missing.
    /// </summary>
    public sealed class MissingParameterException : CoinbaseValidationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingParameterException"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the missing required parameter.</param>
        /// <param name="message">The error message.</param>
        /// <param name="correlationId">Optional correlation ID.</param>
        public MissingParameterException(string parameterName, string message, string correlationId = null)
            : base(parameterName, message, correlationId)
        {
        }
    }
}
