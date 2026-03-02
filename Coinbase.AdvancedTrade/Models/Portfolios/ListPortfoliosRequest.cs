namespace Coinbase.AdvancedTrade.Models.Portfolios
{
    /// <summary>
    /// Request parameters for listing portfolios.
    /// </summary>
    public sealed class ListPortfoliosRequest : PaginationRequest
    {
        /// <summary>
        /// Gets or sets the portfolio type filter (optional).
        /// </summary>
        public string PortfolioType { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListPortfoliosRequest"/> class.
        /// </summary>
        /// <param name="limit">Maximum number of portfolios to return.</param>
        /// <param name="cursor">Pagination cursor from previous response.</param>
        /// <param name="portfolioType">Optional portfolio type filter.</param>
        public ListPortfoliosRequest(int? limit = null, string cursor = null, string portfolioType = null)
            : base(limit, cursor)
        {
            PortfolioType = portfolioType;
        }
    }

    /// <summary>
    /// Response from listing portfolios.
    /// </summary>
    public sealed class ListPortfoliosResponse : IPaginatedResponse<Portfolio>
    {
        /// <summary>
        /// Gets or sets the portfolios in this page.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("portfolios")]
        public Portfolio[] Portfolios { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether more pages exist.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("has_next")]
        public bool HasNext { get; set; }

        /// <summary>
        /// Gets or sets the cursor for the next page.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("cursor")]
        public string Cursor { get; set; }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("size")]
        public int Size { get; set; }

        // IPaginatedResponse implementation
        System.Collections.Generic.IReadOnlyList<Portfolio> IPaginatedResponse<Portfolio>.Items => Portfolios;
        bool IPaginatedResponse<Portfolio>.HasMore => HasNext;
        string IPaginatedResponse<Portfolio>.Cursor => Cursor;
        int? IPaginatedResponse<Portfolio>.TotalCount => null;
    }
}
