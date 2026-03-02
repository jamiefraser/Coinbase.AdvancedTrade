using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models;
using Coinbase.AdvancedTrade.Utilities;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for batch operations on orders.
    /// </summary>
    public sealed class BatchOperationsManager : BaseManager, IBatchOperationsManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BatchOperationsManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public BatchOperationsManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<BatchOrdersResponse> PlaceOrdersAsync(
            BatchOrdersRequest request,
            CancellationToken ct = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Orders == null || request.Orders.Length == 0)
                throw new ArgumentException("Orders array cannot be empty", nameof(request));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    "/api/v3/brokerage/orders/batch",
                    bodyObj: request) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<BatchOrdersResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to place batch orders", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BatchCancelOrdersResponse> CancelOrdersAsync(
            string[] orderIds,
            CancellationToken ct = default)
        {
            if (orderIds == null || orderIds.Length == 0)
                throw new ArgumentException("Order IDs array cannot be empty", nameof(orderIds));

            try
            {
                var request = new BatchCancelOrdersRequest { OrderIds = orderIds };
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "DELETE",
                    "/api/v3/brokerage/orders/batch",
                    bodyObj: request) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<BatchCancelOrdersResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to cancel batch orders", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<BatchOrdersResponse> EditOrdersAsync(
            BatchEditOrdersRequest request,
            CancellationToken ct = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Orders == null || request.Orders.Length == 0)
                throw new ArgumentException("Orders array cannot be empty", nameof(request));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "PUT",
                    "/api/v3/brokerage/orders/batch",
                    bodyObj: request) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<BatchOrdersResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to edit batch orders", ex);
            }
        }
    }

    /// <summary>
    /// Interface for batch operations.
    /// </summary>
    public interface IBatchOperationsManager
    {
        /// <summary>
        /// Places multiple orders in a single request.
        /// </summary>
        Task<BatchOrdersResponse> PlaceOrdersAsync(BatchOrdersRequest request, CancellationToken ct = default);

        /// <summary>
        /// Cancels multiple orders in a single request.
        /// </summary>
        Task<BatchCancelOrdersResponse> CancelOrdersAsync(string[] orderIds, CancellationToken ct = default);

        /// <summary>
        /// Edits multiple orders in a single request.
        /// </summary>
        Task<BatchOrdersResponse> EditOrdersAsync(BatchEditOrdersRequest request, CancellationToken ct = default);
    }

    /// <summary>
    /// Request for batch order placement.
    /// </summary>
    public sealed class BatchOrdersRequest
    {
        public BatchOrderRequest[] Orders { get; set; }
    }

    /// <summary>
    /// Single order in a batch request.
    /// </summary>
    public sealed class BatchOrderRequest
    {
        public string ClientOrderId { get; set; }
        public string ProductId { get; set; }
        public string Side { get; set; }
        public OrderConfiguration OrderConfiguration { get; set; }
    }

    /// <summary>
    /// Response from batch order operations.
    /// </summary>
    public sealed class BatchOrdersResponse
    {
        public BatchOrderResult[] Results { get; set; }
    }

    /// <summary>
    /// Result for a single order in a batch operation.
    /// </summary>
    public sealed class BatchOrderResult
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string ClientOrderId { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }

    /// <summary>
    /// Request for batch order cancellation.
    /// </summary>
    public sealed class BatchCancelOrdersRequest
    {
        public string[] OrderIds { get; set; }
    }

    /// <summary>
    /// Response from batch order cancellation.
    /// </summary>
    public sealed class BatchCancelOrdersResponse
    {
        public BatchCancelResult[] Results { get; set; }
    }

    /// <summary>
    /// Result for a single cancellation in a batch operation.
    /// </summary>
    public sealed class BatchCancelResult
    {
        public bool Success { get; set; }
        public string OrderId { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Request for batch order edits.
    /// </summary>
    public sealed class BatchEditOrdersRequest
    {
        public BatchEditOrderRequest[] Orders { get; set; }
    }

    /// <summary>
    /// Single order edit in a batch request.
    /// </summary>
    public sealed class BatchEditOrderRequest
    {
        public string OrderId { get; set; }
        public string Price { get; set; }
        public string Size { get; set; }
    }
}
