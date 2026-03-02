using System;

namespace Coinbase.AdvancedTrade.WebSocket
{
    /// <summary>
    /// Defines the reconnection policy for WebSocket connections.
    /// </summary>
    public sealed class WebSocketReconnectPolicy
    {
        /// <summary>
        /// Gets the default reconnect policy with exponential backoff.
        /// </summary>
        public static WebSocketReconnectPolicy Default { get; } = new WebSocketReconnectPolicy(
            maxAttempts: 10,
            baseDelay: TimeSpan.FromSeconds(1),
            maxDelay: TimeSpan.FromSeconds(60),
            useExponentialBackoff: true,
            jitterPercent: 25);

        /// <summary>
        /// Gets the maximum number of reconnection attempts.
        /// </summary>
        public int MaxAttempts { get; }

        /// <summary>
        /// Gets the base delay between reconnection attempts.
        /// </summary>
        public TimeSpan BaseDelay { get; }

        /// <summary>
        /// Gets the maximum delay between reconnection attempts.
        /// </summary>
        public TimeSpan MaxDelay { get; }

        /// <summary>
        /// Gets whether to use exponential backoff.
        /// </summary>
        public bool UseExponentialBackoff { get; }

        /// <summary>
        /// Gets the jitter percentage (0-100).
        /// </summary>
        public int JitterPercent { get; }

        private readonly Random _random = new Random();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketReconnectPolicy"/> class.
        /// </summary>
        /// <param name="maxAttempts">Maximum number of reconnection attempts.</param>
        /// <param name="baseDelay">Base delay between attempts.</param>
        /// <param name="maxDelay">Maximum delay between attempts.</param>
        /// <param name="useExponentialBackoff">Whether to use exponential backoff.</param>
        /// <param name="jitterPercent">Jitter percentage (0-100).</param>
        public WebSocketReconnectPolicy(
            int maxAttempts,
            TimeSpan baseDelay,
            TimeSpan maxDelay,
            bool useExponentialBackoff,
            int jitterPercent)
        {
            if (maxAttempts <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts));
            if (baseDelay <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(baseDelay));
            if (maxDelay < baseDelay)
                throw new ArgumentOutOfRangeException(nameof(maxDelay));
            if (jitterPercent < 0 || jitterPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(jitterPercent));

            MaxAttempts = maxAttempts;
            BaseDelay = baseDelay;
            MaxDelay = maxDelay;
            UseExponentialBackoff = useExponentialBackoff;
            JitterPercent = jitterPercent;
        }

        /// <summary>
        /// Determines whether a reconnection should be attempted.
        /// </summary>
        /// <param name="failureCount">The number of consecutive failures.</param>
        /// <returns>True if reconnection should be attempted, false otherwise.</returns>
        public bool ShouldReconnect(int failureCount)
        {
            return failureCount < MaxAttempts;
        }

        /// <summary>
        /// Calculates the next delay before a reconnection attempt.
        /// </summary>
        /// <param name="failureCount">The number of consecutive failures.</param>
        /// <returns>The delay to wait before the next attempt.</returns>
        public TimeSpan GetNextDelay(int failureCount)
        {
            TimeSpan delay;

            if (UseExponentialBackoff)
            {
                var exponentialDelay = BaseDelay.TotalSeconds * Math.Pow(2, failureCount);
                delay = TimeSpan.FromSeconds(Math.Min(exponentialDelay, MaxDelay.TotalSeconds));
            }
            else
            {
                delay = BaseDelay;
            }

            if (JitterPercent > 0)
            {
                var jitterRange = delay.TotalMilliseconds * JitterPercent / 100.0;
                var jitter = (_random.NextDouble() * 2 - 1) * jitterRange;
                delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds + jitter);
            }

            return delay;
        }
    }
}
