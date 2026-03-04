using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models;
using Coinbase.AdvancedTrade.Models.Public;

namespace Coinbase.AdvancedTrade.Helpers
{

/// <summary>
/// P2 Task 8: Helper methods for Canadian market symbol discovery in Coinbase add-in.
/// Makes it easy to filter products by Canadian tradeable status without hand-rolling predicate logic.
/// </summary>
public static class CanadianProductFilters
{
    /// <summary>
    /// Gets all products available on Coinbase with CAD as quote currency and trading enabled.
    /// </summary>
    public static async Task<IReadOnlyList<PublicProduct>> ListTradeableCanadianProductsAsync(
        this IPublicManager publicManager)
    {
        if (publicManager == null)
            throw new ArgumentNullException(nameof(publicManager));

        var products = await publicManager.ListPublicProductsAsync(
            limit: 1000,
            productType: "SPOT").ConfigureAwait(false);

        return products
            .Where(IsTradeableCanadianProduct)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Gets all deprecated or disabled products on Coinbase that were previously tradeable with CAD.
    /// </summary>
    public static async Task<IReadOnlyList<PublicProduct>> ListDeprecatedCanadianProductsAsync(
        this IPublicManager publicManager)
    {
        if (publicManager == null)
            throw new ArgumentNullException(nameof(publicManager));

        var products = await publicManager.ListPublicProductsAsync(
            limit: 1000,
            productType: "SPOT").ConfigureAwait(false);

        return products
            .Where(p => IsCanadianPair(p) && !IsTradeableCanadianProduct(p))
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Normalizes a product ID to a canonical trading symbol (e.g., "BTC-CAD").
    /// </summary>
    public static string ToTradingSymbol(this PublicProduct product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        return product.ProductId?.ToUpperInvariant() ?? $"{product.BaseCurrencyId}-{product.QuoteCurrencyId}".ToUpperInvariant();
    }

    /// <summary>
    /// Checks if a product can be traded (not disabled, not view-only, has trading enabled).
    /// </summary>
    public static bool IsTradeableCanadianProduct(PublicProduct product)
    {
        if (product == null)
            return false;

        return IsCanadianPair(product) &&
               !product.IsDisabled &&
               !product.TradingDisabled &&
               !product.ViewOnly;
    }

    /// <summary>
    /// Checks if a product has CAD as the quote currency.
    /// </summary>
    public static bool IsCanadianPair(PublicProduct product)
    {
        if (product == null)
            return false;

        var quoteId = product.QuoteCurrencyId ?? "";
         return quoteId.Equals("CAD", StringComparison.OrdinalIgnoreCase) ||
             product.ProductId?.EndsWith("-CAD", StringComparison.OrdinalIgnoreCase) == true;
    }
}

/// <summary>
/// P2 Task 8 (alternative API): Extension methods on IProductsManager for authenticated product discovery.
/// </summary>
public static class AuthenticatedCanadianProductFilters
{
    /// <summary>
    /// Gets all products available via authenticated products endpoint with CAD as quote currency and trading enabled.
    /// Useful when the public endpoint doesn't have sufficient product catalog.
    /// </summary>
    public static async Task<IReadOnlyList<Product>> ListAuthenticatedCanadianProductsAsync(
        this IProductsManager productsManager)
    {
        if (productsManager == null)
            throw new ArgumentNullException(nameof(productsManager));

        var products = await productsManager.ListProductsAsync("SPOT").ConfigureAwait(false);

        return products
            .Where(IsTradeableCanadianProduct)
            .ToList()
            .AsReadOnly();
    }

    /// <summary>
    /// Checks if an authenticated product can be traded with CAD.
    /// </summary>
    public static bool IsTradeableCanadianProduct(Product product)
    {
        if (product == null)
            return false;

        var isCanadian = product.QuoteCurrencyId?.Equals("CAD", StringComparison.OrdinalIgnoreCase) == true ||
                product.ProductId?.EndsWith("-CAD", StringComparison.OrdinalIgnoreCase) == true;

        var isTradeableStatus = product.Status?.Equals("Online", StringComparison.OrdinalIgnoreCase) == true;

        return isCanadian && isTradeableStatus;
    }

    /// <summary>
    /// Normalizes a product ID to a canonical trading symbol (e.g., "BTC-CAD").
    /// </summary>
    public static string ToTradingSymbol(this Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        return product.ProductId?.ToUpperInvariant() ?? $"{product.BaseCurrencyId}-{product.QuoteCurrencyId}".ToUpperInvariant();
    }
}
}
