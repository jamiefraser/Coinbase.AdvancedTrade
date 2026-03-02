using System;
using Microsoft.Extensions.Logging;

namespace Coinbase.AdvancedTrade.Logging
{
    /// <summary>
    /// A no-op logger implementation for backward compatibility when no ILogger is provided.
    /// </summary>
    internal class NullLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new NullDisposable();
        }

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // No-op
        }

        private class NullDisposable : IDisposable
        {
            public void Dispose()
            {
                // No-op
            }
        }
    }
}
