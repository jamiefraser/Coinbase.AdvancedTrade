using System;

namespace Coinbase.AdvancedTrade.Models
{
    /// <summary>
    /// Base class for paginated API requests.
    /// </summary>
    public abstract class PaginationRequest
    {
        private int? _limit;

        /// <summary>
        /// Gets or sets the cursor for pagination.
        /// Use the cursor from the previous response to fetch the next page.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of items to return per page.
        /// Default and maximum values vary by endpoint.
        /// </summary>
        public int? Limit
        {
            get => _limit;
            set
            {
                if (value.HasValue && value.Value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Limit must be greater than zero.");
                _limit = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationRequest"/> class.
        /// </summary>
        /// <param name="limit">Optional page size limit.</param>
        /// <param name="cursor">Optional pagination cursor.</param>
        protected PaginationRequest(int? limit = null, string cursor = null)
        {
            Limit = limit;
            Cursor = cursor;
        }
    }
}
