using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coinbase.AdvancedTrade.ExchangeManagers;
using Coinbase.AdvancedTrade.Models;
using Xunit;

namespace Coinbase.AdvancedTrade.Tests.ExchangeManagers
{
    /// <summary>
    /// Unit tests for AccountsManager - focusing on new endpoints (ServerTime and ApiKeyPermissions).
    /// </summary>
    public class AccountsManagerNewEndpointsTests
    {
        private readonly TestCoinbaseAuthenticator _mockAuthenticator;
        private readonly AccountsManager _accountsManager;

        public AccountsManagerNewEndpointsTests()
        {
            _mockAuthenticator = new TestCoinbaseAuthenticator();
            _accountsManager = new AccountsManager(_mockAuthenticator);
        }

        #region GetServerTimeAsync Tests

        [Fact]
        public async Task GetServerTimeAsync_ShouldReturnServerTime_WhenApiReturnsValidData()
        {
            // Arrange
            var expectedResponse = new Dictionary<string, object>
            {
                { "iso", "2026-03-01T12:00:00Z" },
                { "epochSeconds", 1772467200L },
                { "epochMillis", 1772467200000L }
            };

            _mockAuthenticator.SetupSendAuthenticatedRequestAsync((method, path, queryParams, bodyObj) =>
            {
                Assert.Equal("GET", method);
                Assert.Equal("/api/v3/brokerage/time", path);
                return Task.FromResult(expectedResponse);
            });

            // Act
            var result = await _accountsManager.GetServerTimeAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("2026-03-01T12:00:00Z", result.Iso);
            Assert.Equal(1772467200L, result.EpochSeconds);
            Assert.Equal(1772467200000L, result.EpochMillis);
        }

        [Fact]
        public async Task GetServerTimeAsync_ShouldHaveCorrectDateTime_WhenEpochIsValid()
        {
            // Arrange
            var expectedResponse = new Dictionary<string, object>
            {
                { "iso", "2026-03-01T12:00:00Z" },
                { "epochSeconds", 1772467200L },
                { "epochMillis", 1772467200000L }
            };

            _mockAuthenticator.SetupSendAuthenticatedRequestAsync((method, path, queryParams, bodyObj) =>
                Task.FromResult(expectedResponse));

            // Act
            var result = await _accountsManager.GetServerTimeAsync();

            // Assert
            var expectedDateTime = DateTimeOffset.FromUnixTimeSeconds(1772467200L);
            Assert.Equal(expectedDateTime, result.DateTime);
        }

        [Fact]
        public async Task GetServerTimeAsync_ShouldThrowInvalidOperationException_WhenApiCallFails()
        {
            // Arrange
            _mockAuthenticator.SetupSendAuthenticatedRequestAsync((method, path, queryParams, bodyObj) =>
                throw new Exception("Network error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _accountsManager.GetServerTimeAsync());

            Assert.Equal("Failed to get server time", exception.Message);
            Assert.IsType<Exception>(exception.InnerException);
        }

        #endregion

        #region GetApiKeyPermissionsAsync Tests

        [Fact]
        public async Task GetApiKeyPermissionsAsync_ShouldReturnPermissions_WhenApiReturnsValidData()
        {
            // Arrange
            var expectedResponse = new Dictionary<string, object>
            {
                { "can_view", true },
                { "can_trade", true },
                { "can_transfer", false },
                { "portfolio_uuid", null },
                { "portfolio_type", null }
            };

            _mockAuthenticator.SetupSendAuthenticatedRequestAsync((method, path, queryParams, bodyObj) =>
            {
                Assert.Equal("GET", method);
                Assert.Equal("/api/v3/brokerage/key_permissions", path);
                return Task.FromResult(expectedResponse);
            });

            // Act
            var result = await _accountsManager.GetApiKeyPermissionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.CanView);
            Assert.True(result.CanTrade);
            Assert.False(result.CanTransfer);
            Assert.Null(result.PortfolioUuid);
        }

        [Fact]
        public async Task GetApiKeyPermissionsAsync_ShouldHandleRestrictedPortfolio_WhenApiReturnsPortfolioRestriction()
        {
            // Arrange
            var portfolioUuid = "12345678-1234-1234-1234-123456789012";
            var expectedResponse = new Dictionary<string, object>
            {
                { "can_view", true },
                { "can_trade", false },
                { "can_transfer", false },
                { "portfolio_uuid", portfolioUuid },
                { "portfolio_type", "DEFAULT" }
            };

            _mockAuthenticator.SetupSendAuthenticatedRequestAsync((method, path, queryParams, bodyObj) =>
                Task.FromResult(expectedResponse));

            // Act
            var result = await _accountsManager.GetApiKeyPermissionsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.CanView);
            Assert.False(result.CanTrade);
            Assert.False(result.CanTransfer);
            Assert.Equal(portfolioUuid, result.PortfolioUuid);
            Assert.Equal("DEFAULT", result.PortfolioType);
        }

        [Fact]
        public async Task GetApiKeyPermissionsAsync_ShouldThrowInvalidOperationException_WhenApiCallFails()
        {
            // Arrange
            _mockAuthenticator.SetupSendAuthenticatedRequestAsync((method, path, queryParams, bodyObj) =>
                throw new Exception("Authentication error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _accountsManager.GetApiKeyPermissionsAsync());

            Assert.Equal("Failed to get API key permissions", exception.Message);
            Assert.IsType<Exception>(exception.InnerException);
        }

        [Fact]
        public async Task GetApiKeyPermissionsAsync_ShouldReturnReadOnlyPermissions_WhenKeyIsReadOnly()
        {
            // Arrange
            var expectedResponse = new Dictionary<string, object>
            {
                { "can_view", true },
                { "can_trade", false },
                { "can_transfer", false },
                { "portfolio_uuid", null },
                { "portfolio_type", null }
            };

            _mockAuthenticator.SetupSendAuthenticatedRequestAsync((method, path, queryParams, bodyObj) =>
                Task.FromResult(expectedResponse));

            // Act
            var result = await _accountsManager.GetApiKeyPermissionsAsync();

            // Assert
            Assert.True(result.CanView);
            Assert.False(result.CanTrade);
            Assert.False(result.CanTransfer);
        }

        #endregion
    }
}
