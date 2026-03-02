using System;
using System.Collections.Generic;
using System.Threading;

namespace Coinbase.AdvancedTrade.Logging
{
    /// <summary>
    /// Provides correlation ID context management for distributed tracing across async operations.
    /// </summary>
    public static class CorrelationIdContext
    {
        private static readonly AsyncLocal<string> CorrelationIdStorage = new AsyncLocal<string>();

        /// <summary>
        /// Gets the current correlation ID, or generates a new one if none exists.
        /// </summary>
        public static string GetOrCreate()
        {
            if (string.IsNullOrEmpty(CorrelationIdStorage.Value))
            {
                CorrelationIdStorage.Value = Guid.NewGuid().ToString("N");
            }
            return CorrelationIdStorage.Value;
        }

        /// <summary>
        /// Sets the correlation ID for the current async context.
        /// </summary>
        public static void Set(string correlationId)
        {
            CorrelationIdStorage.Value = correlationId;
        }

        /// <summary>
        /// Gets the current correlation ID, or null if none has been set.
        /// </summary>
        public static string GetIfExists()
        {
            return CorrelationIdStorage.Value;
        }

        /// <summary>
        /// Creates a disposable scope that assigns a correlation ID for the duration of the operation.
        /// </summary>
        public static IDisposable BeginScope(string correlationId = null)
        {
            return new CorrelationIdScope(correlationId ?? Guid.NewGuid().ToString("N"));
        }

        /// <summary>
        /// Disposable scope for managing correlation ID lifetime.
        /// </summary>
        private class CorrelationIdScope : IDisposable
        {
            private readonly string _previousValue;
            private readonly string _newValue;

            public CorrelationIdScope(string newValue)
            {
                _newValue = newValue;
                _previousValue = CorrelationIdStorage.Value ?? string.Empty;
                CorrelationIdStorage.Value = newValue;
            }

            public void Dispose()
            {
                CorrelationIdStorage.Value = _previousValue;
            }
        }
    }

    /// <summary>
    /// Structured logging context that includes correlation ID as a standard property.
    /// </summary>
    public class CorrelationIdScope : IDisposable
    {
        private readonly IDisposable _disposable;

        public CorrelationIdScope(string correlationId = null)
        {
            var id = correlationId ?? CorrelationIdContext.GetOrCreate();
            _disposable = CorrelationIdContext.BeginScope(id);
        }

        public void Dispose()
        {
            if (_disposable != null)
            {
                _disposable.Dispose();
            }
        }
    }
}
