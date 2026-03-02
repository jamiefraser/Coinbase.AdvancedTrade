using System;
using System.Collections.Generic;
#if NET8_0_OR_GREATER
using System.Runtime.CompilerServices;
#endif
using System.Threading;
using System.Threading.Tasks;

namespace Coinbase.AdvancedTrade.Extensions
{
    /// <summary>
    /// Extension methods for paginated responses.
    /// </summary>
    public static class PaginationExtensions
    {
#if NET8_0_OR_GREATER
        /// <summary>
        /// Enumerates all items across all pages asynchronously (NET8+ only).
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="firstPageFactory">Factory function to fetch the first page.</param>
        /// <param name="nextPageFactory">Factory function to fetch subsequent pages using a cursor.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An async enumerable of all items across all pages.</returns>
        public static async IAsyncEnumerable<T> EnumerateAllPagesAsync<T>(
            Func<Task<Models.IPaginatedResponse<T>>> firstPageFactory,
            Func<string, Task<Models.IPaginatedResponse<T>>> nextPageFactory,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (firstPageFactory == null)
                throw new ArgumentNullException(nameof(firstPageFactory));
            if (nextPageFactory == null)
                throw new ArgumentNullException(nameof(nextPageFactory));

            // Fetch first page
            var page = await firstPageFactory().ConfigureAwait(false);

            while (page != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Yield all items in current page
                foreach (var item in page.Items)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return item;
                }

                // Check if more pages exist
                if (!page.HasMore || string.IsNullOrEmpty(page.Cursor))
                    break;

                // Fetch next page
                page = await nextPageFactory(page.Cursor).ConfigureAwait(false);
            }
        }
#endif

        /// <summary>
        /// Fetches all pages and returns a complete list of items.
        /// Warning: This loads all data into memory. Use with caution for large datasets.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="firstPageFactory">Factory function to fetch the first page.</param>
        /// <param name="nextPageFactory">Factory function to fetch subsequent pages using a cursor.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list containing all items from all pages.</returns>
        public static async Task<List<T>> FetchAllPagesAsync<T>(
            Func<Task<Models.IPaginatedResponse<T>>> firstPageFactory,
            Func<string, Task<Models.IPaginatedResponse<T>>> nextPageFactory,
            CancellationToken cancellationToken = default)
        {
            if (firstPageFactory == null)
                throw new ArgumentNullException(nameof(firstPageFactory));
            if (nextPageFactory == null)
                throw new ArgumentNullException(nameof(nextPageFactory));

            var allItems = new List<T>();

            // Fetch first page
            var page = await firstPageFactory().ConfigureAwait(false);

            while (page != null)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Add all items from current page
                allItems.AddRange(page.Items);

                // Check if more pages exist
                if (!page.HasMore || string.IsNullOrEmpty(page.Cursor))
                    break;

                // Fetch next page
                page = await nextPageFactory(page.Cursor).ConfigureAwait(false);
            }

            return allItems;
        }

        /// <summary>
        /// Fetches a specified number of pages and returns all items from those pages.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="firstPageFactory">Factory function to fetch the first page.</param>
        /// <param name="nextPageFactory">Factory function to fetch subsequent pages using a cursor.</param>
        /// <param name="maxPages">Maximum number of pages to fetch.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list containing items from the specified number of pages.</returns>
        public static async Task<List<T>> FetchPagesAsync<T>(
            Func<Task<Models.IPaginatedResponse<T>>> firstPageFactory,
            Func<string, Task<Models.IPaginatedResponse<T>>> nextPageFactory,
            int maxPages,
            CancellationToken cancellationToken = default)
        {
            if (firstPageFactory == null)
                throw new ArgumentNullException(nameof(firstPageFactory));
            if (nextPageFactory == null)
                throw new ArgumentNullException(nameof(nextPageFactory));
            if (maxPages <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxPages), "Must be greater than zero.");

            var allItems = new List<T>();
            var pageCount = 0;

            // Fetch first page
            var page = await firstPageFactory().ConfigureAwait(false);

            while (page != null && pageCount < maxPages)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Add all items from current page
                allItems.AddRange(page.Items);
                pageCount++;

                // Check if more pages exist and we haven't reached the limit
                if (!page.HasMore || string.IsNullOrEmpty(page.Cursor) || pageCount >= maxPages)
                    break;

                // Fetch next page
                page = await nextPageFactory(page.Cursor).ConfigureAwait(false);
            }

            return allItems;
        }
    }
}
