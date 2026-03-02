using System;
#if NET8_0_OR_GREATER
using System.Collections.Concurrent;
using System.Threading.Channels;
using Coinbase.AdvancedTrade.Models.WebSocket;

namespace Coinbase.AdvancedTrade.WebSocket
{
    /// <summary>
    /// Provides typed message dispatch with backpressure handling for WebSocket messages.
    /// Available only in .NET 8 and later.
    /// </summary>
    public sealed class TypedMessageDispatcher : IDisposable
    {
        private readonly WebSocketManager _webSocketManager;
        private readonly ConcurrentDictionary<Type, object> _channels;
        private readonly int _channelCapacity;
        private readonly BoundedChannelFullMode _overflowBehavior;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedMessageDispatcher"/> class.
        /// </summary>
        /// <param name="webSocketManager">The WebSocket manager.</param>
        /// <param name="channelCapacity">Maximum channel capacity (default 1000).</param>
        /// <param name="overflowBehavior">Behavior when channel is full (default DropOldest).</param>
        public TypedMessageDispatcher(
            WebSocketManager webSocketManager,
            int channelCapacity = 1000,
            BoundedChannelFullMode overflowBehavior = BoundedChannelFullMode.DropOldest)
        {
            _webSocketManager = webSocketManager ?? throw new ArgumentNullException(nameof(webSocketManager));
            _channelCapacity = channelCapacity;
            _overflowBehavior = overflowBehavior;
            _channels = new ConcurrentDictionary<Type, object>();

            WireUpEventHandlers();
        }

        /// <summary>
        /// Gets a channel for a specific message type.
        /// </summary>
        /// <typeparam name="T">The message type.</typeparam>
        /// <returns>A channel reader for the message type.</returns>
        public ChannelReader<T> GetChannel<T>() where T : class
        {
            var channel = (Channel<T>)_channels.GetOrAdd(typeof(T), _ =>
            {
                var options = new BoundedChannelOptions(_channelCapacity)
                {
                    FullMode = _overflowBehavior
                };
                return Channel.CreateBounded<T>(options);
            });

            return channel.Reader;
        }

        private void WireUpEventHandlers()
        {
            _webSocketManager.CandleMessageReceived += (s, e) => TryWriteToChannel(e.Message);
            _webSocketManager.HeartbeatMessageReceived += (s, e) => TryWriteToChannel(e.Message);
            _webSocketManager.MarketTradeMessageReceived += (s, e) => TryWriteToChannel(e.Message);
            _webSocketManager.StatusMessageReceived += (s, e) => TryWriteToChannel(e.Message);
            _webSocketManager.TickerMessageReceived += (s, e) => TryWriteToChannel(e.Message);
            _webSocketManager.TickerBatchMessageReceived += (s, e) => TryWriteToChannel(e.Message);
            _webSocketManager.Level2MessageReceived += (s, e) => TryWriteToChannel(e.Message);
            _webSocketManager.UserMessageReceived += (s, e) => TryWriteToChannel(e.Message);
        }

        private void TryWriteToChannel<T>(T message) where T : class
        {
            if (_disposed || message == null)
                return;

            if (_channels.TryGetValue(typeof(T), out var channelObj))
            {
                var channel = (Channel<T>)channelObj;
                channel.Writer.TryWrite(message);
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (var channelObj in _channels.Values)
            {
                var channelType = channelObj.GetType();
                var writerProperty = channelType.GetProperty("Writer");
                var writer = writerProperty?.GetValue(channelObj);
                var completeMethod = writer?.GetType().GetMethod("Complete", new Type[] { });
                completeMethod?.Invoke(writer, null);
            }

            _channels.Clear();
        }
    }
}
#endif
