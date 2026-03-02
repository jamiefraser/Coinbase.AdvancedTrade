using System;
using System.Collections.Generic;

namespace Coinbase.AdvancedTrade.Models
{
    /// <summary>
    /// Interface for paginated API responses.
    /// </summary>
    /// <typeparam name="T">The type of items in the page.</typeparam>
    public interface IPaginatedResponse<T>
    {
        /// <summary>
        /// Gets the items in the current page.
        /// </summary>
        IReadOnlyList<T> Items { get; }

        /// <summary>
        /// Gets the cursor for fetching the next page.
        /// </summary>
        string Cursor { get; }

        /// <summary>
        /// Gets a value indicating whether more pages are available.
        /// </summary>
        bool HasMore { get; }

        /// <summary>
        /// Gets the total number of items across all pages, if available.
        /// </summary>
        int? TotalCount { get; }
    }

    /// <summary>
    /// Standard implementation of paginated response.
    /// </summary>
    /// <typeparam name="T">The type of items in the page.</typeparam>
    public sealed class PaginatedResponse<T> : IPaginatedResponse<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedResponse{T}"/> class.
        /// </summary>
        /// <param name="items">The items in this page.</param>
        /// <param name="cursor">The cursor for the next page.</param>
        /// <param name="hasMore">Whether more pages are available.</param>
        /// <param name="totalCount">Optional total count of items across all pages.</param>
        public PaginatedResponse(
            IReadOnlyList<T> items,
            string cursor,
            bool hasMore,
            int? totalCount = null)
        {
            Items = items ?? Array.Empty<T>();
            Cursor = cursor;
            HasMore = hasMore;
            TotalCount = totalCount;
        }

        /// <inheritdoc/>
        public IReadOnlyList<T> Items { get; }

        /// <inheritdoc/>
        public string Cursor { get; }

        /// <inheritdoc/>
        public bool HasMore { get; }

        /// <inheritdoc/>
        public int? TotalCount { get; }
    }
}
