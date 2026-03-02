using System;
using System.Collections.Generic;
using System.Threading;

namespace Coinbase.AdvancedTrade.Logging
{
    /// <summary>
    /// Provides correlation ID management for tracking requests across the SDK.
    /// </summary>
    public static class CorrelationContext
    {
        private static readonly AsyncLocal<string> _correlationId = new AsyncLocal<string>();

        /// <summary>
        /// Gets or sets the current correlation ID for this async context.
        /// </summary>
        public static string CorrelationId
        {
            get => _correlationId.Value ?? GenerateCorrelationId();
            set => _correlationId.Value = value;
        }

        /// <summary>
        /// Generates a new correlation ID.
        /// </summary>
        /// <returns>A new GUID-based correlation ID.</returns>
        public static string GenerateCorrelationId()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 16);
        }

        /// <summary>
        /// Creates a new correlation scope with a generated ID.
        /// </summary>
        /// <returns>A disposable correlation scope.</returns>
        public static IDisposable BeginScope()
        {
            return BeginScope(GenerateCorrelationId());
        }

        /// <summary>
        /// Creates a new correlation scope with a specific ID.
        /// </summary>
        /// <param name="correlationId">The correlation ID to use.</param>
        /// <returns>A disposable correlation scope.</returns>
        public static IDisposable BeginScope(string correlationId)
        {
            var previousId = _correlationId.Value;
            _correlationId.Value = correlationId;
            return new CorrelationScope(previousId);
        }

        private sealed class CorrelationScope : IDisposable
        {
            private readonly string _previousId;
            private bool _disposed;

            public CorrelationScope(string previousId)
            {
                _previousId = previousId;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _correlationId.Value = _previousId;
                    _disposed = true;
                }
            }
        }
    }
}
