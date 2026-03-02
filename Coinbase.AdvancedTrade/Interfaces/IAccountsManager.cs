using System.Collections.Generic;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.Models;

namespace Coinbase.AdvancedTrade.Interfaces
{
    /// <summary>
    /// Provides methods to manage accounts on the Coinbase Advanced Trade API.
    /// </summary>
    public interface IAccountsManager
    {
        /// <summary>
        /// Asynchronously retrieves a list of accounts.
        /// </summary>
        /// <param name="limit">The maximum number of accounts to retrieve. Default is 49.</param>
        /// <param name="cursor">The cursor for pagination. Null by default.</param>
        /// <returns>A list of accounts or null if none are found.</returns>
        Task<List<Account>> ListAccountsAsync(int limit = 49, string cursor = null);

        /// <summary>
        /// Asynchronously retrieves a specific account by its UUID.
        /// </summary>
        /// <param name="accountUuid">The UUID of the account.</param>
        /// <returns>The account corresponding to the given UUID or null if not found.</returns>
        Task<Account> GetAccountAsync(string accountUuid);

        /// <summary>
        /// Asynchronously retrieves the current server time from Coinbase.
        /// Useful for synchronizing client clock with server and generating timestamps.
        /// </summary>
        /// <returns>A ServerTime object with ISO 8601 and Unix epoch formats.</returns>
        Task<ServerTime> GetServerTimeAsync();

        /// <summary>
        /// Asynchronously retrieves the permissions and capabilities of the current API key.
        /// Useful for validating that your API key has the required permissions before attempting operations.
        /// </summary>
        /// <returns>An ApiKeyPermissions object with view, trade, and transfer capabilities.</returns>
        Task<ApiKeyPermissions> GetApiKeyPermissionsAsync();
    }
}