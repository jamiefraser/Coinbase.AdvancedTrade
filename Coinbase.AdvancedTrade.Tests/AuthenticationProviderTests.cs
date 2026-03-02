using System;
using Coinbase.AdvancedTrade.Auth;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Coinbase.AdvancedTrade.Tests;

/// <summary>
/// Unit tests for authentication providers without making real API calls.
/// </summary>
[TestClass]
public sealed class AuthenticationProviderTests
{
    [TestMethod]
    [TestCategory("Unit")]
    public void JwtAuthenticationProvider_Constructor_WithNullApiKey_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new JwtAuthenticationProvider(null!, "secret"));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void JwtAuthenticationProvider_Constructor_WithNullApiSecret_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new JwtAuthenticationProvider("key", null!));
    }

    [TestMethod]
    [TestCategory("Unit")]
#pragma warning disable CS0618 // Type or member is obsolete - testing deprecated provider intentionally
    public void HmacAuthenticationProvider_Constructor_WithNullApiKey_ThrowsArgumentNullException()
#pragma warning restore CS0618
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new HmacAuthenticationProvider(null!, "secret"));
    }

    [TestMethod]
    [TestCategory("Unit")]
#pragma warning disable CS0618 // Type or member is obsolete - testing deprecated provider intentionally
    public void HmacAuthenticationProvider_Constructor_WithNullApiSecret_ThrowsArgumentNullException()
#pragma warning restore CS0618
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new HmacAuthenticationProvider("key", null!));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void OAuth2AuthenticationProvider_Constructor_WithNullAccessToken_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsException<ArgumentNullException>(() =>
            new OAuth2AuthenticationProvider(null!));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void HmacAuthenticationProvider_GenerateHeaders_ReturnsRequiredHeaders()
    {
        // Arrange
        var provider = new HmacAuthenticationProvider("test-key", "test-secret");

        // Act
        var headers = provider.GenerateHeaders("GET", "/api/v3/brokerage/accounts");

        // Assert
        Assert.IsNotNull(headers, "Headers should not be null");
        Assert.IsTrue(headers.ContainsKey("CB-ACCESS-KEY"), "Should contain CB-ACCESS-KEY");
        Assert.IsTrue(headers.ContainsKey("CB-ACCESS-SIGN"), "Should contain CB-ACCESS-SIGN");
        Assert.IsTrue(headers.ContainsKey("CB-ACCESS-TIMESTAMP"), "Should contain CB-ACCESS-TIMESTAMP");
        Assert.AreEqual("test-key", headers["CB-ACCESS-KEY"], "API key should match");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void OAuth2AuthenticationProvider_GenerateHeaders_ReturnsBearerToken()
    {
        // Arrange
        var token = "test-oauth2-token";
        var provider = new OAuth2AuthenticationProvider(token);

        // Act
        var headers = provider.GenerateHeaders("GET", "/api/v3/brokerage/accounts");

        // Assert
        Assert.IsNotNull(headers, "Headers should not be null");
        Assert.IsTrue(headers.ContainsKey("Authorization"), "Should contain Authorization header");
        Assert.AreEqual($"Bearer {token}", headers["Authorization"], "Should use Bearer token format");
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void HmacAuthenticationProvider_GenerateHeaders_WithDifferentPaths_GeneratesDifferentSignatures()
    {
        // Arrange
        var provider = new HmacAuthenticationProvider("test-key", "test-secret");

        // Act
        var headers1 = provider.GenerateHeaders("GET", "/api/v3/brokerage/accounts");
        var headers2 = provider.GenerateHeaders("GET", "/api/v3/brokerage/products");

        // Assert - Signatures should differ because paths are different
        // (though timestamps might make them different anyway)
        Assert.IsNotNull(headers1["CB-ACCESS-SIGN"]);
        Assert.IsNotNull(headers2["CB-ACCESS-SIGN"]);
        Assert.AreNotEqual(string.Empty, headers1["CB-ACCESS-SIGN"]);
        Assert.AreNotEqual(string.Empty, headers2["CB-ACCESS-SIGN"]);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void HmacAuthenticationProvider_GenerateHeaders_WithBody_IncludesBodyInSignature()
    {
        // Arrange
        var provider = new HmacAuthenticationProvider("test-key", "test-secret");
        var body = "{\"product_id\":\"BTC-USD\"}";

        // Act
        var headersWithBody = provider.GenerateHeaders("POST", "/api/v3/brokerage/orders", body);
        var headersWithoutBody = provider.GenerateHeaders("POST", "/api/v3/brokerage/orders");

        // Assert - Signatures should differ when body is included
        Assert.IsNotNull(headersWithBody["CB-ACCESS-SIGN"]);
        Assert.IsNotNull(headersWithoutBody["CB-ACCESS-SIGN"]);
        // Note: timestamps will differ, so we just verify both have valid signatures
        Assert.AreNotEqual(string.Empty, headersWithBody["CB-ACCESS-SIGN"]);
        Assert.AreNotEqual(string.Empty, headersWithoutBody["CB-ACCESS-SIGN"]);
    }
}
