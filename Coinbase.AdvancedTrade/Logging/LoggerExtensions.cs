using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Coinbase.AdvancedTrade.Logging
{
    /// <summary>
    /// Extension methods for structured logging in the Coinbase SDK.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs an API request with correlation ID and performance metrics.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="endpoint">API endpoint.</param>
        /// <param name="correlationId">Correlation ID for tracking.</param>
        public static void LogApiRequest(
            this ILogger logger,
            string method,
            string endpoint,
            string correlationId = null)
        {
            if (logger == null) return;

            logger.LogDebug(
                "[{CorrelationId}] API Request: {Method} {Endpoint}",
                correlationId ?? CorrelationContext.CorrelationId,
                method,
                endpoint);
        }

        /// <summary>
        /// Logs an API response with status code and duration.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="method">HTTP method.</param>
        /// <param name="endpoint">API endpoint.</param>
        /// <param name="statusCode">Response status code.</param>
        /// <param name="durationMs">Request duration in milliseconds.</param>
        /// <param name="correlationId">Correlation ID for tracking.</param>
        public static void LogApiResponse(
            this ILogger logger,
            string method,
            string endpoint,
            int statusCode,
            long durationMs,
            string correlationId = null)
        {
            if (logger == null) return;

            var logLevel = statusCode >= 500 ? LogLevel.Error :
                          statusCode >= 400 ? LogLevel.Warning :
                          LogLevel.Debug;

            logger.Log(
                logLevel,
                "[{CorrelationId}] API Response: {Method} {Endpoint} - {StatusCode} ({DurationMs}ms)",
                correlationId ?? CorrelationContext.CorrelationId,
                method,
                endpoint,
                statusCode,
                durationMs);
        }

        /// <summary>
        /// Logs rate limit information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="endpoint">API endpoint.</param>
        /// <param name="limit">Rate limit quota.</param>
        /// <param name="remaining">Remaining requests.</param>
        /// <param name="percentRemaining">Percentage of quota remaining.</param>
        public static void LogRateLimit(
            this ILogger logger,
            string endpoint,
            int limit,
            int remaining,
            double percentRemaining)
        {
            if (logger == null) return;

            if (percentRemaining < 10)
            {
                logger.LogWarning(
                    "Rate limit critically low for {Endpoint}: {Remaining}/{Limit} ({PercentRemaining:F1}%)",
                    endpoint,
                    remaining,
                    limit,
                    percentRemaining);
            }
            else if (percentRemaining < 20)
            {
                logger.LogInformation(
                    "Rate limit low for {Endpoint}: {Remaining}/{Limit} ({PercentRemaining:F1}%)",
                    endpoint,
                    remaining,
                    limit,
                    percentRemaining);
            }
        }

        /// <summary>
        /// Logs a retry attempt with details.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="attemptNumber">The retry attempt number.</param>
        /// <param name="maxRetries">Maximum number of retries.</param>
        /// <param name="delayMs">Delay before retry in milliseconds.</param>
        /// <param name="reason">Reason for retry.</param>
        /// <param name="correlationId">Correlation ID for tracking.</param>
        public static void LogRetryAttempt(
            this ILogger logger,
            int attemptNumber,
            int maxRetries,
            long delayMs,
            string reason,
            string correlationId = null)
        {
            if (logger == null) return;

            logger.LogWarning(
                "[{CorrelationId}] Retry attempt {AttemptNumber}/{MaxRetries} after {DelayMs}ms - Reason: {Reason}",
                correlationId ?? CorrelationContext.CorrelationId,
                attemptNumber,
                maxRetries,
                delayMs,
                reason);
        }

        /// <summary>
        /// Logs WebSocket connection events.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="eventType">Event type (Connected, Disconnected, Error).</param>
        /// <param name="channel">WebSocket channel.</param>
        /// <param name="message">Optional message.</param>
        public static void LogWebSocketEvent(
            this ILogger logger,
            string eventType,
            string channel,
            string message = null)
        {
            if (logger == null) return;

            var logLevel = eventType.Equals("Error", StringComparison.OrdinalIgnoreCase) 
                ? LogLevel.Error 
                : LogLevel.Information;

            logger.Log(
                logLevel,
                "WebSocket {EventType} - Channel: {Channel}{Message}",
                eventType,
                channel,
                string.IsNullOrEmpty(message) ? "" : $" - {message}");
        }

        /// <summary>
        /// Logs authentication events with sanitized information.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="authType">Authentication type (JWT, HMAC, OAuth2).</param>
        /// <param name="success">Whether authentication succeeded.</param>
        /// <param name="keyId">Optional key identifier (last 4 chars only for security).</param>
        public static void LogAuthenticationEvent(
            this ILogger logger,
            string authType,
            bool success,
            string keyId = null)
        {
            if (logger == null) return;

            var logLevel = success ? LogLevel.Debug : LogLevel.Warning;
            var sanitizedKeyId = string.IsNullOrEmpty(keyId) || keyId.Length <= 4 
                ? keyId 
                : "***" + keyId.Substring(keyId.Length - 4);

            logger.Log(
                logLevel,
                "Authentication {Status} - Type: {AuthType}, KeyId: {KeyId}",
                success ? "Succeeded" : "Failed",
                authType,
                sanitizedKeyId ?? "N/A");
        }

        /// <summary>
        /// Logs performance metrics for an operation.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="operationName">Name of the operation.</param>
        /// <param name="durationMs">Duration in milliseconds.</param>
        /// <param name="additionalMetrics">Optional additional metrics.</param>
        public static void LogPerformanceMetrics(
            this ILogger logger,
            string operationName,
            long durationMs,
            Dictionary<string, object> additionalMetrics = null)
        {
            if (logger == null) return;

            var logLevel = durationMs > 5000 ? LogLevel.Warning : 
                          durationMs > 1000 ? LogLevel.Information : 
                          LogLevel.Debug;

            if (additionalMetrics != null && additionalMetrics.Count > 0)
            {
                logger.Log(
                    logLevel,
                    "Performance: {OperationName} completed in {DurationMs}ms - Metrics: {@Metrics}",
                    operationName,
                    durationMs,
                    additionalMetrics);
            }
            else
            {
                logger.Log(
                    logLevel,
                    "Performance: {OperationName} completed in {DurationMs}ms",
                    operationName,
                    durationMs);
            }
        }

        /// <summary>
        /// Creates a timed scope that logs performance when disposed.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="operationName">Name of the operation being timed.</param>
        /// <returns>A disposable timing scope.</returns>
        public static IDisposable BeginTimedScope(this ILogger logger, string operationName)
        {
            return new TimedScope(logger, operationName);
        }

        private sealed class TimedScope : IDisposable
        {
            private readonly ILogger _logger;
            private readonly string _operationName;
            private readonly Stopwatch _stopwatch;
            private bool _disposed;

            public TimedScope(ILogger logger, string operationName)
            {
                _logger = logger;
                _operationName = operationName;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _stopwatch.Stop();
                    _logger?.LogPerformanceMetrics(_operationName, _stopwatch.ElapsedMilliseconds);
                    _disposed = true;
                }
            }
        }
    }
}
