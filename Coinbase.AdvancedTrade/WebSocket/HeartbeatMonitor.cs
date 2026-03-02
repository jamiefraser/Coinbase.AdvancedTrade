using System;
using System.Threading;

namespace Coinbase.AdvancedTrade.WebSocket
{
    /// <summary>
    /// Monitors WebSocket heartbeats and detects missed heartbeats.
    /// </summary>
    public sealed class HeartbeatMonitor : IDisposable
    {
        private readonly TimeSpan _heartbeatInterval;
        private readonly TimeSpan _heartbeatTimeout;
        private DateTime _lastHeartbeat;
        private Timer _monitorTimer;
        private bool _disposed;
        private bool _isRunning;

        /// <summary>
        /// Event raised when a heartbeat is missed.
        /// </summary>
        public event EventHandler HeartbeatMissed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HeartbeatMonitor"/> class.
        /// </summary>
        /// <param name="heartbeatInterval">Expected interval between heartbeats.</param>
        /// <param name="heartbeatTimeout">Grace period before considering heartbeat missed.</param>
        public HeartbeatMonitor(TimeSpan heartbeatInterval, TimeSpan heartbeatTimeout)
        {
            _heartbeatInterval = heartbeatInterval;
            _heartbeatTimeout = heartbeatTimeout;
            _lastHeartbeat = DateTime.UtcNow;
        }

        /// <summary>
        /// Starts monitoring heartbeats.
        /// </summary>
        public void Start()
        {
            if (_isRunning)
                return;

            _isRunning = true;
            _lastHeartbeat = DateTime.UtcNow;
            _monitorTimer = new Timer(CheckHeartbeat, null, _heartbeatInterval, _heartbeatInterval);
        }

        /// <summary>
        /// Stops monitoring heartbeats.
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _monitorTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Records that a heartbeat was received.
        /// </summary>
        public void RecordHeartbeat()
        {
            _lastHeartbeat = DateTime.UtcNow;
        }

        private void CheckHeartbeat(object state)
        {
            if (!_isRunning)
                return;

            var timeSinceLastHeartbeat = DateTime.UtcNow - _lastHeartbeat;
            if (timeSinceLastHeartbeat > _heartbeatInterval + _heartbeatTimeout)
            {
                HeartbeatMissed?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Stop();
            _monitorTimer?.Dispose();
        }
    }
}
