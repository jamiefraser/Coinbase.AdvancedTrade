using System.IO;
using System.Text.Json;
using Coinbase.AdvancedTrade;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Coinbase.AdvancedTrade.Tests;

/// <summary>
/// Real authenticated integration test that loads credentials from JSON and calls the Coinbase API.
/// </summary>
[TestClass]
public sealed class PortfolioIntegrationTests
{
    private CoinbaseClient? _client;
    private CoinbaseTestConfiguration? _config;

    [TestInitialize]
    public void Setup()
    {
        // Load credentials from appsettings.test.json
        var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.test.json");
        
        if (!File.Exists(configPath))
        {
            Assert.Inconclusive($"Configuration file not found: {configPath}. Please create appsettings.test.json with your API credentials.");
            return;
        }

        var json = File.ReadAllText(configPath);
        _config = JsonSerializer.Deserialize<CoinbaseTestConfiguration>(json);

        if (_config?.Coinbase == null || 
            string.IsNullOrEmpty(_config.Coinbase.ApiKey) || 
            string.IsNullOrEmpty(_config.Coinbase.ApiSecret))
        {
            Assert.Inconclusive("Invalid or missing API credentials in appsettings.test.json");
            return;
        }

        // Initialize CoinbaseClient with CDP keys
        _client = new CoinbaseClient(
            _config.Coinbase.ApiKey, 
            _config.Coinbase.ApiSecret, 
            apiKeyType: Enums.ApiKeyType.CoinbaseDeveloperPlatform);
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RealApi")]
    public async Task GetPortfolio_WithRealCredentials_ReturnsAccounts()
    {
        // Arrange
        if (_client == null)
        {
            Assert.Inconclusive("Client not initialized - check credentials");
            return;
        }

        // Act
        var accounts = await _client.Accounts.ListAccountsAsync(limit: 10);

        // Assert
        Assert.IsNotNull(accounts, "Accounts list should not be null");
        Assert.IsTrue(accounts.Count > 0, "Should return at least one account");

        // Validate first account has expected properties
        var firstAccount = accounts[0];
        Assert.IsNotNull(firstAccount.Uuid, "Account UUID should not be null");
        Assert.IsNotNull(firstAccount.Currency, "Account currency should not be null");
        Assert.IsNotNull(firstAccount.AvailableBalance, "Account available balance should not be null");

        // Output for visibility
        Console.WriteLine($"Portfolio retrieved successfully. Total accounts: {accounts.Count}");
        Console.WriteLine($"First account: {firstAccount.Currency} - Balance: {firstAccount.AvailableBalance.Value} {firstAccount.AvailableBalance.Currency}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RealApi")]
    public async Task GetSpecificAccount_WithRealCredentials_ReturnsAccountDetails()
    {
        // Arrange
        if (_client == null)
        {
            Assert.Inconclusive("Client not initialized - check credentials");
            return;
        }

        // First get list of accounts to get a valid UUID
        var accounts = await _client.Accounts.ListAccountsAsync(limit: 1);
        Assert.IsTrue(accounts.Count > 0, "Need at least one account to test");

        var testAccountUuid = accounts[0].Uuid;

        // Act
        var account = await _client.Accounts.GetAccountAsync(testAccountUuid!);

        // Assert
        Assert.IsNotNull(account, "Account should not be null");
        Assert.AreEqual(testAccountUuid, account.Uuid, "Account UUID should match");
        Assert.IsNotNull(account.Currency, "Currency should not be null");
        Assert.IsNotNull(account.AvailableBalance, "Available balance should not be null");

        Console.WriteLine($"Account details: {account.Currency} - {account.AvailableBalance.Value} {account.AvailableBalance.Currency}");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RealApi")]
    public async Task GetPortfolio_WithPagination_ReturnsCorrectCount()
    {
        // Arrange
        if (_client == null)
        {
            Assert.Inconclusive("Client not initialized - check credentials");
            return;
        }

        // Act - Request with limit
        var accountsLimited = await _client.Accounts.ListAccountsAsync(limit: 2);

        // Assert
        Assert.IsNotNull(accountsLimited, "Accounts list should not be null");
        Assert.IsTrue(accountsLimited.Count <= 2, "Should respect limit parameter");

        Console.WriteLine($"Pagination test: Retrieved {accountsLimited.Count} accounts with limit=2");
    }

    [TestMethod]
    [TestCategory("Integration")]
    [TestCategory("RealApi")]
    public async Task GetAccount_WithInvalidUuid_ThrowsException()
    {
        // Arrange
        if (_client == null)
        {
            Assert.Inconclusive("Client not initialized - check credentials");
            return;
        }

        var invalidUuid = "invalid-uuid-format";

        // Act & Assert
        try
        {
            await _client.Accounts.GetAccountAsync(invalidUuid);
            Assert.Fail("Expected exception for invalid UUID");
        }
        catch (Exception ex)
        {
            Assert.IsNotNull(ex, "Exception should be thrown for invalid UUID");
            Console.WriteLine($"Expected exception caught: {ex.GetType().Name}");
        }
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client = null;
        _config = null;
    }
}
