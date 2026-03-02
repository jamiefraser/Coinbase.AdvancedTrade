using System.Text.Json.Serialization;

namespace Coinbase.AdvancedTrade.Tests;

/// <summary>
/// Configuration model for loading Coinbase API credentials from JSON.
/// </summary>
public sealed class CoinbaseTestConfiguration
{
    [JsonPropertyName("Coinbase")]
    public CoinbaseCredentials? Coinbase { get; set; }
}

/// <summary>
/// Represents Coinbase API credentials.
/// </summary>
public sealed class CoinbaseCredentials
{
    [JsonPropertyName("ApiKey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("ApiSecret")]
    public string ApiSecret { get; set; } = string.Empty;
}
