using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Coinbase.AdvancedTrade.WebSocket
{
    /// <summary>
    /// Manages WebSocket connection state and automatic reconnection.
    /// </summary>
    public sealed class WebSocketConnectionManager : IDisposable
    {
        private readonly WebSocketManager _webSocketManager;
        private readonly WebSocketReconnectPolicy _reconnectPolicy;
        private readonly HeartbeatMonitor _heartbeatMonitor;
        private CancellationTokenSource _connectionCts;
        private Task _reconnectTask;
        private bool _disposed;
        private DateTime _lastSuccessfulConnection;
        private int _consecutiveFailures;

        /// <summary>
        /// Gets the current connection state.
        /// </summary>
        public ConnectionState State { get; private set; }

        /// <summary>
        /// Event raised when connection state changes.
        /// </summary>
        public event EventHandler<ConnectionStateChangedEventArgs> StateChanged;

        /// <summary>
        /// Event raised when a reconnection attempt is made.
        /// </summary>
        public event EventHandler<ReconnectAttemptEventArgs> ReconnectAttempt;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketConnectionManager"/> class.
        /// </summary>
        /// <param name="webSocketManager">The WebSocket manager to wrap.</param>
        /// <param name="reconnectPolicy">The reconnect policy to use.</param>
        public WebSocketConnectionManager(
            WebSocketManager webSocketManager,
            WebSocketReconnectPolicy reconnectPolicy = null)
        {
            _webSocketManager = webSocketManager ?? throw new ArgumentNullException(nameof(webSocketManager));
            _reconnectPolicy = reconnectPolicy ?? WebSocketReconnectPolicy.Default;
            _heartbeatMonitor = new HeartbeatMonitor(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
            State = ConnectionState.Disconnected;

            // Subscribe to heartbeat events
            _webSocketManager.HeartbeatMessageReceived += OnHeartbeatReceived;
            _heartbeatMonitor.HeartbeatMissed += OnHeartbeatMissed;
        }

        /// <summary>
        /// Connects to the WebSocket server with automatic reconnection.
        /// </summary>
        public async Task ConnectAsync()
        {
            if (State == ConnectionState.Connected || State == ConnectionState.Connecting)
                return;

            _connectionCts = new CancellationTokenSource();
            ChangeState(ConnectionState.Connecting);

            try
            {
                await _webSocketManager.ConnectAsync().ConfigureAwait(false);
                _lastSuccessfulConnection = DateTime.UtcNow;
                _consecutiveFailures = 0;
                ChangeState(ConnectionState.Connected);
                _heartbeatMonitor.Start();
            }
            catch (Exception ex)
            {
                _consecutiveFailures++;
                ChangeState(ConnectionState.Failed);
                
                if (_reconnectPolicy.ShouldReconnect(_consecutiveFailures))
                {
                    StartReconnectLoop();
                }
                else
                {
                    throw new WebSocketConnectionException("Failed to connect and max retries exceeded", ex);
                }
            }
        }

        /// <summary>
        /// Disconnects from the WebSocket server.
        /// </summary>
        public async Task DisconnectAsync()
        {
            _heartbeatMonitor.Stop();
            _connectionCts?.Cancel();

            if (_reconnectTask != null)
            {
                try
                {
                    await _reconnectTask.ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    // Expected when canceling reconnection
                }
            }

            ChangeState(ConnectionState.Disconnecting);
            await _webSocketManager.DisconnectAsync().ConfigureAwait(false);
            ChangeState(ConnectionState.Disconnected);
        }

        private void StartReconnectLoop()
        {
            if (_reconnectTask != null && !_reconnectTask.IsCompleted)
                return;

            ChangeState(ConnectionState.Reconnecting);
            _reconnectTask = Task.Run(async () => await ReconnectLoopAsync().ConfigureAwait(false));
        }

        private async Task ReconnectLoopAsync()
        {
            while (!_connectionCts.IsCancellationRequested && 
                   _reconnectPolicy.ShouldReconnect(_consecutiveFailures))
            {
                var delay = _reconnectPolicy.GetNextDelay(_consecutiveFailures);
                
                ReconnectAttempt?.Invoke(this, new ReconnectAttemptEventArgs(
                    _consecutiveFailures + 1, delay));

                try
                {
                    await Task.Delay(delay, _connectionCts.Token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                try
                {
                    await _webSocketManager.DisconnectAsync().ConfigureAwait(false);
                    await _webSocketManager.ConnectAsync().ConfigureAwait(false);
                    
                    _lastSuccessfulConnection = DateTime.UtcNow;
                    _consecutiveFailures = 0;
                    ChangeState(ConnectionState.Connected);
                    _heartbeatMonitor.Start();
                    break;
                }
                catch (Exception)
                {
                    _consecutiveFailures++;
                }
            }

            if (_consecutiveFailures >= _reconnectPolicy.MaxAttempts)
            {
                ChangeState(ConnectionState.Failed);
            }
        }

        private void OnHeartbeatReceived(object sender, WebSocketMessageEventArgs<Models.WebSocket.HeartbeatMessage> e)
        {
            _heartbeatMonitor.RecordHeartbeat();
        }

        private void OnHeartbeatMissed(object sender, EventArgs e)
        {
            if (State == ConnectionState.Connected)
            {
                StartReconnectLoop();
            }
        }

        private void ChangeState(ConnectionState newState)
        {
            var oldState = State;
            State = newState;
            StateChanged?.Invoke(this, new ConnectionStateChangedEventArgs(oldState, newState));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _heartbeatMonitor?.Dispose();
            _connectionCts?.Cancel();
            _connectionCts?.Dispose();
        }
    }

    /// <summary>
    /// Defines the connection state of the WebSocket.
    /// </summary>
    public enum ConnectionState
    {
        Disconnected,
        Connecting,
        Connected,
        Reconnecting,
        Disconnecting,
        Failed
    }

    /// <summary>
    /// Event args for connection state changes.
    /// </summary>
    public sealed class ConnectionStateChangedEventArgs : EventArgs
    {
        public ConnectionState OldState { get; }
        public ConnectionState NewState { get; }

        public ConnectionStateChangedEventArgs(ConnectionState oldState, ConnectionState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }

    /// <summary>
    /// Event args for reconnection attempts.
    /// </summary>
    public sealed class ReconnectAttemptEventArgs : EventArgs
    {
        public int AttemptNumber { get; }
        public TimeSpan Delay { get; }

        public ReconnectAttemptEventArgs(int attemptNumber, TimeSpan delay)
        {
            AttemptNumber = attemptNumber;
            Delay = delay;
        }
    }

    /// <summary>
    /// Exception thrown when WebSocket connection fails.
    /// </summary>
    public sealed class WebSocketConnectionException : Exception
    {
        public WebSocketConnectionException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
