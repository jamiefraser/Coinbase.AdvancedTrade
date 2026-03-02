using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Interfaces;
using Coinbase.AdvancedTrade.Models.Portfolios;
using Coinbase.AdvancedTrade.Utilities;

namespace Coinbase.AdvancedTrade.ExchangeManagers
{
    /// <summary>
    /// Manager for portfolio-related API operations.
    /// </summary>
    public sealed class PortfoliosManager : BaseManager, IPortfoliosManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PortfoliosManager"/> class.
        /// </summary>
        /// <param name="authenticator">The authenticator for API requests.</param>
        public PortfoliosManager(CoinbaseAuthenticator authenticator) : base(authenticator)
        {
        }

        /// <inheritdoc/>
        public async Task<ListPortfoliosResponse> ListPortfoliosAsync(
            ListPortfoliosRequest request = null,
            CancellationToken ct = default)
        {
            try
            {
                var parameters = new Dictionary<string, string>();

                if (request != null)
                {
                    if (request.Limit.HasValue)
                        parameters["limit"] = request.Limit.Value.ToString();

                    if (!string.IsNullOrEmpty(request.Cursor))
                        parameters["cursor"] = request.Cursor;

                    if (!string.IsNullOrEmpty(request.PortfolioType))
                        parameters["portfolio_type"] = request.PortfolioType;
                }

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET", 
                    "/api/v3/brokerage/portfolios", 
                    parameters) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<ListPortfoliosResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to list portfolios", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<CreatePortfolioResponse> CreatePortfolioAsync(
            CreatePortfolioRequest request,
            CancellationToken ct = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var body = new Dictionary<string, object>
                {
                    ["name"] = request.Name
                };

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    "/api/v3/brokerage/portfolios",
                    null,
                    body) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<CreatePortfolioResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create portfolio", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<Portfolio> GetPortfolioAsync(
            string portfolioUuid,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioUuid))
                throw new ArgumentNullException(nameof(portfolioUuid));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/portfolios/{portfolioUuid}") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeJsonElement<Portfolio>(response, "portfolio");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get portfolio {portfolioUuid}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<EditPortfolioResponse> EditPortfolioAsync(
            string portfolioUuid,
            EditPortfolioRequest request,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioUuid))
                throw new ArgumentNullException(nameof(portfolioUuid));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var body = new Dictionary<string, object>
                {
                    ["name"] = request.Name
                };

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "PUT",
                    $"/api/v3/brokerage/portfolios/{portfolioUuid}",
                    null,
                    body) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<EditPortfolioResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to edit portfolio {portfolioUuid}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<DeletePortfolioResponse> DeletePortfolioAsync(
            string portfolioUuid,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioUuid))
                throw new ArgumentNullException(nameof(portfolioUuid));

            try
            {
                await _authenticator.SendAuthenticatedRequestAsync(
                    "DELETE",
                    $"/api/v3/brokerage/portfolios/{portfolioUuid}");

                return new DeletePortfolioResponse { Success = true };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to delete portfolio {portfolioUuid}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<MoveFundsResponse> MoveFundsAsync(
            string portfolioUuid,
            MoveFundsRequest request,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioUuid))
                throw new ArgumentNullException(nameof(portfolioUuid));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            try
            {
                var body = new Dictionary<string, object>
                {
                    ["from"] = request.From,
                    ["to"] = request.To,
                    ["amount"] = request.Amount.ToString(),
                    ["currency"] = request.Currency
                };

                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "POST",
                    $"/api/v3/brokerage/portfolios/{portfolioUuid}/move_funds",
                    null,
                    body) ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeDictionary<MoveFundsResponse>(response);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to move funds from portfolio {portfolioUuid}", ex);
            }
        }

        /// <inheritdoc/>
        public async Task<PortfolioBreakdown> GetPortfolioBreakdownAsync(
            string portfolioUuid,
            CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(portfolioUuid))
                throw new ArgumentNullException(nameof(portfolioUuid));

            try
            {
                var response = await _authenticator.SendAuthenticatedRequestAsync(
                    "GET",
                    $"/api/v3/brokerage/portfolios/{portfolioUuid}/breakdown") ?? new Dictionary<string, object>();

                return UtilityHelper.DeserializeJsonElement<PortfolioBreakdown>(response, "breakdown");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get portfolio breakdown for {portfolioUuid}", ex);
            }
        }
    }
}
