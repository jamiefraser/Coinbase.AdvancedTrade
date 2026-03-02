using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Enums;

namespace Coinbase.AdvancedTrade.WebSocket
{
    /// <summary>
    /// Manages WebSocket subscription state and resubscription on reconnect.
    /// </summary>
    public sealed class SubscriptionStateManager
    {
        private readonly WebSocketManager _webSocketManager;
        private readonly WebSocketConnectionManager _connectionManager;
        private readonly ConcurrentDictionary<SubscriptionKey, SubscriptionState> _subscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionStateManager"/> class.
        /// </summary>
        /// <param name="webSocketManager">The WebSocket manager.</param>
        /// <param name="connectionManager">The connection manager.</param>
        public SubscriptionStateManager(
            WebSocketManager webSocketManager,
            WebSocketConnectionManager connectionManager)
        {
            _webSocketManager = webSocketManager ?? throw new ArgumentNullException(nameof(webSocketManager));
            _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            _subscriptions = new ConcurrentDictionary<SubscriptionKey, SubscriptionState>();

            _connectionManager.StateChanged += OnConnectionStateChanged;
        }

        /// <summary>
        /// Subscribes to a channel and tracks the subscription.
        /// </summary>
        /// <param name="products">Product IDs to subscribe to.</param>
        /// <param name="channelType">The channel type.</param>
        public async Task SubscribeAsync(string[] products, ChannelType channelType)
        {
            var key = new SubscriptionKey(products, channelType);
            var state = new SubscriptionState(products, channelType, SubscriptionStatus.Subscribing);
            
            _subscriptions[key] = state;

            try
            {
                await _webSocketManager.SubscribeAsync(products, channelType).ConfigureAwait(false);
                _subscriptions[key] = state.WithStatus(SubscriptionStatus.Subscribed);
            }
            catch (Exception ex)
            {
                _subscriptions[key] = state.WithStatus(SubscriptionStatus.Failed, ex);
                throw;
            }
        }

        /// <summary>
        /// Unsubscribes from a channel and removes from tracking.
        /// </summary>
        /// <param name="products">Product IDs to unsubscribe from.</param>
        /// <param name="channelType">The channel type.</param>
        public async Task UnsubscribeAsync(string[] products, ChannelType channelType)
        {
            var key = new SubscriptionKey(products, channelType);

            if (_subscriptions.TryRemove(key, out var state))
            {
                try
                {
                    await _webSocketManager.UnsubscribeAsync(products, channelType).ConfigureAwait(false);
                }
                catch
                {
                    // Re-add to subscriptions if unsubscribe fails
                    _subscriptions[key] = state;
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets all active subscriptions.
        /// </summary>
        public IReadOnlyCollection<SubscriptionState> GetAllSubscriptions()
        {
            return _subscriptions.Values.ToList();
        }

        /// <summary>
        /// Gets the status of a specific subscription.
        /// </summary>
        public SubscriptionStatus? GetSubscriptionStatus(string[] products, ChannelType channelType)
        {
            var key = new SubscriptionKey(products, channelType);
            return _subscriptions.TryGetValue(key, out var state) ? state.Status : (SubscriptionStatus?)null;
        }

        private async void OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            if (e.NewState == ConnectionState.Connected && e.OldState == ConnectionState.Reconnecting)
            {
                await ResubscribeAllAsync().ConfigureAwait(false);
            }
        }

        private async Task ResubscribeAllAsync()
        {
            var subscriptionsToResubscribe = _subscriptions.Values
                .Where(s => s.Status == SubscriptionStatus.Subscribed || s.Status == SubscriptionStatus.Failed)
                .ToList();

            foreach (var subscription in subscriptionsToResubscribe)
            {
                try
                {
                    await _webSocketManager.SubscribeAsync(
                        subscription.Products,
                        subscription.ChannelType).ConfigureAwait(false);

                    var key = new SubscriptionKey(subscription.Products, subscription.ChannelType);
                    _subscriptions[key] = subscription.WithStatus(SubscriptionStatus.Subscribed);
                }
                catch (Exception ex)
                {
                    var key = new SubscriptionKey(subscription.Products, subscription.ChannelType);
                    _subscriptions[key] = subscription.WithStatus(SubscriptionStatus.Failed, ex);
                }
            }
        }
    }

    /// <summary>
    /// Represents the state of a WebSocket subscription.
    /// </summary>
    public sealed class SubscriptionState
    {
        public string[] Products { get; }
        public ChannelType ChannelType { get; }
        public SubscriptionStatus Status { get; }
        public Exception LastError { get; }
        public DateTime LastUpdated { get; }

        public SubscriptionState(
            string[] products,
            ChannelType channelType,
            SubscriptionStatus status,
            Exception lastError = null)
        {
            Products = products;
            ChannelType = channelType;
            Status = status;
            LastError = lastError;
            LastUpdated = DateTime.UtcNow;
        }

        public SubscriptionState WithStatus(SubscriptionStatus newStatus, Exception error = null)
        {
            return new SubscriptionState(Products, ChannelType, newStatus, error);
        }
    }

    /// <summary>
    /// Status of a WebSocket subscription.
    /// </summary>
    public enum SubscriptionStatus
    {
        Subscribing,
        Subscribed,
        Unsubscribing,
        Failed
    }

    /// <summary>
    /// Key for tracking subscriptions.
    /// </summary>
    internal sealed class SubscriptionKey : IEquatable<SubscriptionKey>
    {
        private readonly string _productsKey;
        private readonly ChannelType _channelType;

        public SubscriptionKey(string[] products, ChannelType channelType)
        {
            _productsKey = products != null ? string.Join(",", products.OrderBy(p => p)) : string.Empty;
            _channelType = channelType;
        }

        public override bool Equals(object obj)
        {
            return obj is SubscriptionKey other && Equals(other);
        }

        public bool Equals(SubscriptionKey other)
        {
            if (other == null)
                return false;

            return _productsKey == other._productsKey && _channelType == other._channelType;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_productsKey.GetHashCode() * 397) ^ _channelType.GetHashCode();
            }
        }
    }
}
