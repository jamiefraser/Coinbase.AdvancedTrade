# Coinbase.AdvancedTrade.Tests

Real authenticated integration tests for the Coinbase SDK.

## Setup

1. Copy `appsettings.test.json.example` to `appsettings.test.json`
2. Add your Coinbase Developer Platform (CDP) API credentials:
   ```json
   {
     "Coinbase": {
       "ApiKey": "organizations/{org_id}/apiKeys/{key_id}",
       "ApiSecret": "-----BEGIN EC PRIVATE KEY-----\nYOUR_PRIVATE_KEY_HERE\n-----END EC PRIVATE KEY-----\n"
     }
   }
   ```

## Running Tests

```bash
dotnet test --filter "TestCategory=Integration"
```

## Test Categories

- `Integration` - Real API calls (requires credentials)
- `RealApi` - Tests that hit live Coinbase endpoints

## Security

⚠️ **NEVER commit `appsettings.test.json` to source control.** It contains sensitive API credentials.

The `.gitignore` file already excludes this pattern:
```
**/appsettings.test.json
```
