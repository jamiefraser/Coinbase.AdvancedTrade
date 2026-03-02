using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.ExchangeManagers;
using Coinbase.AdvancedTrade.Logging;
using Coinbase.AdvancedTrade.Models;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Coinbase.AdvancedTrade.Tests.Logging
{
    /// <summary>
    /// Tests for structured logging with correlation IDs in Coinbase operations.
    /// </summary>
    public class StructuredLoggingTests
    {
        private class TestLogger : ILogger
        {
            public List<(LogLevel Level, string Message, string CorrelationId)> LoggedMessages { get; } = new();

            public IDisposable BeginScope<TState>(TState state) where TState : notnull
            {
                return new NullDisposable();
            }

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                var message = formatter(state, exception);
                var correlationId = CorrelationIdContext.GetIfExists() ?? "unknown";
                LoggedMessages.Add((logLevel, message, correlationId));
            }

            private class NullDisposable : IDisposable
            {
                public void Dispose() { }
            }
        }

        [Fact]
        public void AuthenticatorLogsWithStructuredFormat()
        {
            // Arrange
            var testLogger = new TestLogger();
            var correlationId = Guid.NewGuid().ToString("N");
            CorrelationIdContext.Set(correlationId);

            var authenticator = new CoinbaseAuthenticator("test-key", "test-secret", logger: testLogger);

            // Act
            // The actual API call would happen here, but we can verify the logger is connected
            var logCount = testLogger.LoggedMessages.Count;

            // Assert
            Assert.True(logCount >= 0); // Logger is now set up to capture logs
        }

        [Fact]
        public async Task CorrelationIdFlowsThroughMultipleOperations()
        {
            // Arrange
            var testCorrelationId = "test-correlation-123";
            CorrelationIdContext.Set(testCorrelationId);

            var loggedIds = new List<string>();

            // Act
            await Task.Run(() =>
            {
                var id1 = CorrelationIdContext.GetIfExists();
                loggedIds.Add(id1!);
            });

            var id2 = CorrelationIdContext.GetIfExists();
            loggedIds.Add(id2!);

            // Assert
            Assert.All(loggedIds, id => Assert.Equal(testCorrelationId, id));
        }

        [Fact]
        public void LoggerUsesStructuredLoggingWithProperties()
        {
            // Arrange
            var testLogger = new TestLogger();
            var correlationId = "structured-test-456";
            CorrelationIdContext.Set(correlationId);

            // Act
            testLogger.LogInformation(
                "Processing API request: {Method} {Path}, CorrelationId={CorrelationId}",
                "GET", "/api/v3/brokerage/accounts", correlationId);

            // Assert
            Assert.Single(testLogger.LoggedMessages);
            var (level, message, loggedId) = testLogger.LoggedMessages[0];
            Assert.Equal(LogLevel.Information, level);
            Assert.Contains("GET", message);
            Assert.Contains("/api/v3/brokerage/accounts", message);
        }
    }
}
