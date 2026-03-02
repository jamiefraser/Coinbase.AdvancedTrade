# Comprehensive Gap Analysis: .NET vs Python SDK

**Generated:** March 1, 2026  
**Authority Model:**
- **API Specification Authority:** Coinbase Documentation MCP Server
- **Behavioral & Functional Authority:** `coinbase_advanced_trade_sdk` Python Project

---

## Executive Summary

This gap analysis compares the `Coinbase.AdvancedTrade` .NET library against the authoritative `coinbase_advanced_trade_sdk` Python reference implementation. The analysis identifies missing functionality, divergent behaviors, and areas requiring alignment to achieve full behavioral parity.

### Key Findings

| Category | Python SDK | .NET Library | Gap Status |
|----------|-----------|--------------|------------|
| **REST Managers** | 8 managers | 15 managers | ⚠️ Different scope |
| **Exception Hierarchy** | Typed, granular | Typed, granular | ✅ Similar structure |
| **Retry/Resilience** | Manual exponential backoff | Polly-based | ⚠️ Different implementation |
| **Circuit Breaker** | Custom implementation | Not present | ❌ Missing |
| **Rate Limit Handling** | Automatic backpressure | Manual | ⚠️ Partial |
| **Structured Logging** | JSON with correlation | Basic logging | ⚠️ Needs enhancement |
| **WebSocket Channels** | 9 channels | 8 channels | ✅ Similar |
| **OAuth2 Support** | Full support | Full support | ✅ Present |
| **Environment Support** | Production + Sandbox | Production only | ❌ Missing sandbox |

---

## 1. REST API Coverage

### 1.1 Python SDK Managers

The Python SDK organizes functionality into 8 focused managers:

1. **MarketDataManager** - Products, candles, order books, trades
2. **PortfolioManager** - List, get, create, delete portfolios
3. **OrderManager** - Create, list, get, cancel orders (market & limit)
4. **OrderHistoryManager** - Historical fills and order tracking
5. **PaymentMethodManager** - List and get payment methods
6. **ConvertManager** - Currency conversion operations
7. **AccountManager** - API key permissions, server time
8. **TransfersManager** - Transfer funds between portfolios/accounts

### 1.2 .NET Library Managers

The .NET library has 15 managers with broader scope:

1. **AccountsManager**
2. **ProductsManager**
3. **OrdersManager**
4. **FeesManager**
5. **PublicManager**
6. **PortfoliosManager**
7. **ConvertManager**
8. **PaymentMethodsManager**
9. **AddressesManager**
10. **TransactionsManager**
11. **TransfersManager**
12. **BatchOperationsManager**
13. **FuturesManager**
14. **PerpetualsManager**
15. **WebSocketManager**

### 1.3 Gaps and Divergences

#### ✅ **Present in Both**
- Market data (products, candles, order books)
- Orders (create, list, get, cancel)
- Portfolios (list, get, create, delete)
- Conversions
- Payment methods
- Transfers

#### ⚠️ **Different Implementation Patterns**

**Python SDK Pattern:**
```python
# Clean separation of concerns
client.market_data.get_product("BTC-USD")
client.orders.create_market_order(...)
client.portfolio.list_portfolios()
client.account.get_api_key_permissions()
```

**.NET Library Pattern:**
```csharp
// More granular managers
client.Products.GetProduct("BTC-USD");
client.Orders.CreateMarketOrder(...);
client.Portfolios.ListPortfolios();
// No direct equivalent for account/permissions
```

#### ❌ **Missing in .NET**

1. **OrderHistoryManager equivalent**
   - Python: `client.order_history.list_fills()` with pagination and filtering
   - .NET: Partial support in OrdersManager, but not separate concept

2. **Server Time Endpoint**
   - Python: `client.account.get_server_time()` → ServerTime model
   - .NET: Not present

3. **API Key Permissions Endpoint**
   - Python: `client.account.get_api_key_permissions()` → ApiKeyPermissions model
   - .NET: Not present

4. **Sandbox Environment Support**
   - Python: `Environment.SANDBOX` with automatic URL switching
   - .NET: Only production URL hardcoded

#### ➕ **Extra in .NET (Not in Python)**

1. **FeesManager** - Fee-related operations
2. **AddressesManager** - Cryptocurrency address operations
3. **TransactionsManager** - Transaction history
4. **BatchOperationsManager** - Batch order operations
5. **FuturesManager** - Futures-specific operations
6. **PerpetualsManager** - Perpetuals-specific operations

**Status:** These may be valid API endpoints not yet implemented in Python SDK, or they may be deprecated/legacy endpoints.

---

## 2. Authentication & Security

### 2.1 Python SDK Authentication

```python
# HMAC-SHA256 (Legacy)
client = CoinbaseAdvancedTradeClient(
    api_key="key",
    api_secret="secret"
)

# EC Private Key (Modern)
client = CoinbaseAdvancedTradeClient(
    api_key="organizations/{org_id}/apiKeys/{key_id}",
    ec_private_key="-----BEGIN EC PRIVATE KEY-----\n..."
)

# OAuth2 Delegated
oauth_client = CoinbaseOAuth2Client(...)
client = CoinbaseAdvancedTradeClient(oauth_client=oauth_client)
```

### 2.2 .NET Library Authentication

```csharp
// HMAC or EC
var client = new CoinbaseClient(apiKey, apiSecret, 
    apiKeyType: ApiKeyType.CoinbaseDeveloperPlatform);

// OAuth2
var authenticator = new CoinbaseAuthenticator(oAuth2AccessToken);
```

### 2.3 Gaps

#### ✅ **Both Support:**
- HMAC-SHA256 signing
- EC private key signing
- OAuth2 tokens
- JWT token generation

#### ⚠️ **Behavioral Differences:**
- **Python** uses separate `CoinbaseOAuth2Client` class with full OAuth flow
- **.NET** uses simplified `CoinbaseOAuth2Client` but may lack full refresh token flow

#### ❌ **Missing in .NET:**
- Explicit environment selection in authenticator (sandbox vs production)
- OAuth2 refresh token automatic renewal

---

## 3. Exception Hierarchy

### 3.1 Python SDK Exceptions

```python
# Base
CoinbaseHttpException
CoinbaseOAuth2Exception
CircuitBreakerException

# Hierarchy in http_client
class CoinbaseHttpException(Exception):
    def __init__(self, status_code, message, error_code, error_details, headers):
        self.status_code = status_code
        self.error_code = error_code
        self.error_details = error_details
        self.headers = headers
```

**Key Features:**
- Captures HTTP status code
- Captures API error code
- Captures error details from response
- Captures headers (including rate limit info)
- Single exception type with rich attributes

### 3.2 .NET Library Exceptions

```csharp
// Base
CoinbaseException (abstract base)

// HTTP exceptions
BadRequestException (400)
UnauthorizedException (401)
ForbiddenException (403)
NotFoundException (404)
TooManyRequestsException (429)
InternalServerErrorException (500)
ServiceUnavailableException (503)

// Other
ValidationException
NetworkException (connection errors)
WebSocketException
```

**Key Features:**
- Typed exception per HTTP status code
- Correlation ID tracking
- Timestamp capture
- Error code and details
- Retry-After header capture (429)

### 3.3 Gaps

#### ✅ **Both Have:**
- Base exception class
- HTTP status code exceptions
- Error code and message capture
- Correlation/tracking IDs

#### ⚠️ **Behavioral Differences:**

**Python Approach:** Single exception type with attributes
```python
try:
    await client.orders.create_market_order(...)
except CoinbaseHttpException as e:
    if e.status_code == 429:
        # Handle rate limit
    elif e.status_code == 404:
        # Handle not found
```

**.NET Approach:** Specific exception types
```csharp
try {
    await client.Orders.CreateMarketOrder(...);
}
catch (TooManyRequestsException ex) {
    // Handle rate limit
}
catch (NotFoundException ex) {
    // Handle not found
}
```

**Assessment:** Both approaches are valid. .NET is more idiomatic to C# exception handling patterns.

#### ❌ **Missing in .NET:**
- `CircuitBreakerException` (because circuit breaker not implemented)

---

## 4. Retry Logic & Resilience

### 4.1 Python SDK Retry Logic

**Location:** `core/http_client.py`

```python
async def _request_with_retry(self, method, path, retries=None, **kwargs):
    retries = retries or self.max_retries
    
    for attempt in range(retries + 1):
        try:
            # Apply rate limit backpressure
            await self._maybe_wait_for_rate_limit()
            
            response = await self._make_request(method, path, **kwargs)
            return response
        
        except httpx.HTTPStatusError as e:
            if e.response.status_code not in self.RETRYABLE_STATUS_CODES:
                raise
            
            if attempt < retries:
                # Exponential backoff with jitter
                delay = self.backoff_base * (2 ** attempt)
                jitter = random.uniform(-0.25, 0.25) * delay
                total_delay = max(0, delay + jitter)
                
                logger.warning(f"Retrying after {total_delay}s (attempt {attempt + 1})")
                await self._sleep(total_delay)
            else:
                raise
```

**Features:**
- Exponential backoff: `base * 2^attempt`
- Jitter: ±25% random variance
- Retryable status codes: 408, 429, 500, 502, 503, 504
- Rate limit awareness
- Structured logging with attempt number
- Configurable max retries and backoff base

### 4.2 .NET Library Retry Logic

**Location:** `Resilience/RetryPolicyFactory.cs`

```csharp
public IAsyncPolicy<RestResponse> CreateRestApiPolicy()
{
    return Policy<RestResponse>
        .HandleResult(response => ShouldRetryResponse(response))
        .Or<HttpRequestException>()
        .Or<TaskCanceledException>()
        .Or<ConnectionException>()
        .WaitAndRetryAsync(
            retryCount: _config.MaxRetries,
            sleepDurationProvider: (retryAttempt, result, context) =>
            {
                var delay = CalculateDelay(retryAttempt, result.Result);
                return delay;
            },
            onRetryAsync: (outcome, timespan, attemptNumber, context) =>
            {
                LogRetry(attemptNumber, outcome.Result, timespan);
                return Task.CompletedTask;
            });
}

private TimeSpan CalculateDelay(int retryAttempt, RestResponse response)
{
    // Check for Retry-After header
    var retryAfter = GetRetryAfterSeconds(response);
    if (retryAfter.HasValue) {
        return TimeSpan.FromSeconds(retryAfter.Value);
    }
    
    // Exponential backoff: baseDelay * 2^(attempt-1)
    var exponentialDelay = _config.BaseDelaySeconds * Math.Pow(2, retryAttempt - 1);
    var cappedDelay = Math.Min(exponentialDelay, _config.MaxDelaySeconds);
    
    // Apply jitter
    if (_config.UseJitter) {
        double jitterValue = (_jitterRandom.NextDouble() * 2.0 - 1.0) * _config.JitterFactor;
        cappedDelay = cappedDelay * (1.0 + jitterValue);
    }
    
    return TimeSpan.FromSeconds(Math.Max(0, cappedDelay));
}
```

**Features:**
- Uses Polly library for retry policies
- Exponential backoff: `base * 2^(attempt-1)`
- Jitter: Configurable percentage (default ±25%)
- Respects Retry-After header
- Configurable max delay cap
- Structured retry configuration

### 4.3 Gaps

#### ✅ **Both Have:**
- Exponential backoff
- Jitter
- Configurable max retries
- Retry on transient errors (5xx, timeouts, connection errors)
- Retry on rate limit (429)
- Retry-After header support (.NET)

#### ⚠️ **Behavioral Differences:**

| Feature | Python SDK | .NET Library |
|---------|-----------|--------------|
| **Backoff formula** | `base * 2^attempt` | `base * 2^(attempt-1)` |
| **Jitter application** | `±25%` random | Configurable `±25%` |
| **Rate limit backpressure** | Automatic wait before request | Manual handling |
| **Library** | Built-in (httpx + asyncio) | Polly 7.x |
| **Configuration** | Constructor params | `RetryConfiguration` object |

#### ❌ **Missing in .NET:**

1. **Automatic Rate Limit Backpressure**
   - Python: Automatically waits when rate limit exhausted
   - .NET: Only retries with backoff, no preemptive wait

2. **Rate Limit Info Tracking**
   ```python
   # Python
   info = client.http_client.get_rate_limit_info()
   print(f"{info.remaining}/{info.limit} requests remaining")
   ```
   - .NET: No equivalent

---

## 5. Circuit Breaker

### 5.1 Python SDK Circuit Breaker

**Location:** `core/circuit_breaker.py`

```python
class CircuitState(str, Enum):
    CLOSED = "closed"       # Normal operation
    OPEN = "open"           # Failing, reject requests
    HALF_OPEN = "half_open" # Testing recovery

class CircuitBreaker:
    def __init__(
        self,
        service_name: str,
        failure_threshold: int = 5,
        failure_timeout: float = 60.0,
        recovery_timeout: float = 30.0,
    ):
        ...
    
    async def call(self, func: Callable, *args, **kwargs) -> Any:
        # Check circuit state
        if self.state == CircuitState.OPEN:
            raise CircuitBreakerException(self.service_name, self.state)
        
        try:
            result = await func(*args, **kwargs)
            await self._on_success()
            return result
        except self.expected_exception as e:
            await self._on_failure()
            raise
```

**Features:**
- Three-state FSM: CLOSED → OPEN → HALF_OPEN → CLOSED
- Configurable failure threshold
- Automatic recovery testing (half-open)
- Time-based failure window
- Per-service tracking
- Structured logging of state changes

### 5.2 .NET Library Circuit Breaker

**Status:** ❌ **NOT IMPLEMENTED**

**Gap:** The .NET library does not have an equivalent circuit breaker implementation.

**Impact:**
- No protection against cascading failures
- No automatic service recovery detection
- Retries continue even when service is known to be down

**Required Implementation:**
1. Port `CircuitBreaker` class from Python
2. Integrate with Polly's circuit breaker policies (recommended)
3. Add circuit breaker configuration to `RetryConfiguration`
4. Track circuit state per endpoint or service
5. Emit structured logs on state transitions

---

## 6. Structured Logging

### 6.1 Python SDK Logging

```python
logger = logging.getLogger(__name__)

# Example structured log
logger.info(
    "Created market order",
    extra={
        "order_id": response.get("order_id"),
        "product_id": product_id,
        "side": side_value,
        "timestamp": datetime.utcnow().isoformat(),
    },
)

# Rate limit warning
logger.warning(
    "Rate limit exhausted, applying backpressure",
    extra={
        "wait_seconds": wait_seconds,
        "reset_at": reset_at.isoformat(),
        "remaining": 0,
        "limit": self.last_rate_limit.limit,
    },
)

# Circuit breaker state change
logger.warning(
    f"Circuit breaker opened",
    extra={
        "service": self.service_name,
        "failures": self.failure_count,
        "threshold": self.failure_threshold,
    },
)
```

**Features:**
- Uses Python's `logging` module
- Structured `extra` fields for machine parsing
- Correlation IDs via `extra` (not automatic)
- Log levels: DEBUG, INFO, WARNING, ERROR
- Timestamps automatic via logging framework

### 6.2 .NET Library Logging

```csharp
// Current implementation
private readonly ILogger _logger;

_logger?.LogInformation("Retrying request (attempt {Attempt})", attemptNumber);
_logger?.LogWarning("Rate limit hit, waiting {Delay}s", delay.TotalSeconds);
```

**Features:**
- Uses `Microsoft.Extensions.Logging.ILogger`
- Structured logging via message templates
- Optional `ILogger` injection
- Not consistent across all managers

### 6.3 Gaps

#### ⚠️ **Behavioral Differences:**

| Feature | Python SDK | .NET Library |
|---------|-----------|--------------|
| **Correlation IDs** | Manual via `extra` | Via `CoinbaseException.CorrelationId` |
| **Consistency** | All managers log consistently | Inconsistent logging |
| **JSON output** | Configurable via logging config | Depends on logging provider |
| **Request/response logging** | Optional debug logs | Not present |
| **Error context** | Rich `extra` fields | Basic message templates |

#### ❌ **Missing in .NET:**

1. **Consistent Logging Across All Managers**
   - Python: Every manager logs key operations
   - .NET: Only some managers log

2. **Request Context Logging**
   ```python
   # Python logs request details
   logger.debug(
       "HTTP request",
       extra={
           "method": method,
           "path": path,
           "headers": headers,
           "body_size": len(body) if body else 0,
       },
   )
   ```

3. **Retry/Backoff Logging**
   - Python: Logs every retry attempt with delay
   - .NET: Logs in `RetryPolicyFactory` but not consistent

4. **Rate Limit Logging**
   - Python: Logs rate limit state from headers
   - .NET: No rate limit header tracking

---

## 7. WebSocket Implementation

### 7.1 Python SDK WebSocket

**Location:** `websocket_client.py`

```python
class WebSocketChannel(str, Enum):
    TICKER = "ticker"
    TICKER_BATCH = "ticker_batch"
    LEVEL2 = "level2"
    LEVEL2_BATCH = "level2_batch"
    MATCHES = "matches"
    FULL = "full"
    STATUS = "status"
    USER = "user"
    HEARTBEATS = "heartbeats"

class MarketDataWebSocketClient:
    WS_URL = "wss://advanced-trade-ws.coinbase.com"
    SANDBOX_URL = "wss://advanced-trade-ws-sandbox.coinbase.com"
    
    async def subscribe(self, product_ids, channels=None):
        message = {
            "type": "subscribe",
            "product_ids": product_ids,
            "channel": channels[0],
        }
        await self.ws.send(json.dumps(message))
    
    def on_message(self, channel, handler):
        # Register callback for channel
        ...
```

**Features:**
- 9 channel types
- Sandbox URL support
- Automatic reconnection with exponential backoff
- Message routing via callbacks
- Subscription management
- Heartbeat handling

### 7.2 .NET Library WebSocket

**Location:** `ExchangeManagers/WebSocketManager.cs`

```csharp
public enum ChannelType {
    Candles,
    Heartbeats,
    MarketTrades,
    Status,
    Ticker,
    TickerBatch,
    Level2,
    User
}

public sealed class WebSocketManager : IDisposable {
    public async Task ConnectAsync();
    public async Task SubscribeAsync(string[] productIds, ChannelType channelType);
    public async Task UnsubscribeAsync(string[] productIds, ChannelType channelType);
    
    // Events
    public event WebSocketMessageEventHandler CandleMessageReceived;
    public event WebSocketMessageEventHandler HeartbeatMessageReceived;
    public event WebSocketMessageEventHandler MarketTradeMessageReceived;
    // ... etc
}
```

**Features:**
- 8 channel types (similar to Python)
- Event-based message dispatching
- Subscription tracking
- Reconnection policy (via `WebSocketReconnectPolicy.cs`)
- Typed message models

### 7.3 Gaps

#### ✅ **Both Have:**
- Ticker channel
- Ticker batch channel
- Level2 order book
- Market trades (matches)
- User channel
- Heartbeats
- Status channel
- Candles channel (Python may have this differently)

#### ⚠️ **Behavioral Differences:**

| Feature | Python SDK | .NET Library |
|---------|-----------|--------------|
| **Message dispatch** | Callback functions | C# events |
| **Reconnection** | Manual exponential backoff | `WebSocketReconnectPolicy` |
| **Sandbox URL** | Supported | Not supported |
| **Connection management** | Context manager (`async with`) | Explicit connect/dispose |

#### ❌ **Missing in .NET:**

1. **Sandbox WebSocket URL**
   - Python: `wss://advanced-trade-ws-sandbox.coinbase.com`
   - .NET: Hardcoded production URL

2. **Full Channel**
   - Python: `FULL` channel for full order book snapshot + updates
   - .NET: Not present in enum

3. **Level2 Batch**
   - Python: `LEVEL2_BATCH` for batched order book updates
   - .NET: May be present but not explicitly documented

---

## 8. Models & Data Types

### 8.1 Python SDK Models

**Location:** `models/core_models.py`

**Key Models:**
```python
@dataclass
class Product:
    id: str
    base_currency: str
    quote_currency: str
    price: str
    # ... 20+ fields
    
@dataclass
class Portfolio:
    uuid: str
    name: str
    type: PortfolioType
    deleted: bool
    created_at: str

@dataclass
class PortfolioBreakdown:
    portfolio_uuid: str
    spot_positions: List[BreakdownBalance]
    perp_positions: List[BreakdownBalance]
    futures_positions: List[BreakdownBalance]
    prediction_markets_positions: List[BreakdownBalance]

@dataclass
class ApiKeyPermissions:
    can_view: bool
    can_trade: bool
    can_transfer: bool
    portfolio_uuid: Optional[str]
    portfolio_type: Optional[str]

@dataclass
class ServerTime:
    iso: str
    epoch: int
```

**Pagination:**
```python
@dataclass
class PaginatedResponse:
    items: List[Any]
    has_next: bool
    cursor: Optional[str]
    num_products: Optional[int]
```

### 8.2 .NET Library Models

**Location:** `Models/`

**Key Models:**
- `Product`
- `Portfolio`
- `Order`
- `Account`
- `Transaction`
- `Fee`
- WebSocket message models (typed)

### 8.3 Gaps

#### ❌ **Missing in .NET:**

1. **ApiKeyPermissions Model**
   ```python
   # Python
   permissions = await client.account.get_api_key_permissions()
   if permissions.can_trade:
       # Execute trade
   ```

2. **ServerTime Model**
   ```python
   # Python
   server_time = await client.account.get_server_time()
   print(f"Server time: {server_time.iso}")
   ```

3. **PortfolioBreakdown Model**
   - Python has detailed breakdown: spot, perp, futures, prediction markets
   - .NET may have simpler portfolio model

4. **PaginatedResponse Generic**
   - Python uses consistent pagination pattern across all endpoints
   - .NET may handle pagination differently per manager

---

## 9. Configuration & Initialization

### 9.1 Python SDK Initialization

```python
# API Key + Secret
client = CoinbaseAdvancedTradeClient(
    api_key="key",
    api_secret="secret",
    environment=Environment.SANDBOX,
    timeout=30.0,
    max_retries=3,
    user_agent="CustomAgent/1.0"
)

# EC Private Key
client = CoinbaseAdvancedTradeClient(
    api_key="organizations/{org_id}/apiKeys/{key_id}",
    ec_private_key="-----BEGIN EC PRIVATE KEY-----\n...",
    environment=Environment.PRODUCTION
)

# OAuth2
oauth = CoinbaseOAuth2Client(...)
client = CoinbaseAdvancedTradeClient(oauth_client=oauth)

# Module-level convenience
client = create_client(api_key, api_secret, sandbox=True)
```

### 9.2 .NET Library Initialization

```csharp
// API Key
var client = new CoinbaseClient(
    apiKey,
    apiSecret,
    websocketBufferSize: 5 * 1024 * 1024,
    apiKeyType: ApiKeyType.CoinbaseDeveloperPlatform
);

// OAuth2
var authenticator = new CoinbaseAuthenticator(oAuth2AccessToken);
// Then manually construct managers
```

### 9.3 Gaps

#### ❌ **Missing in .NET:**

1. **Environment Configuration**
   - Python: Explicit `Environment.SANDBOX` or `Environment.PRODUCTION`
   - .NET: Hardcoded production URLs

2. **Timeout Configuration**
   - Python: `timeout=30.0` parameter
   - .NET: Default HTTP client timeout

3. **Retry Configuration at Client Level**
   - Python: `max_retries=3` in constructor
   - .NET: Configured via `RetryPolicyFactory`, not at client level

4. **Custom User-Agent**
   - Python: `user_agent="CustomAgent/1.0"`
   - .NET: Default user agent

5. **Convenience Factory Function**
   - Python: `create_client(api_key, api_secret, sandbox=True)`
   - .NET: No equivalent

---

## 10. OAuth2 Support

### 10.1 Python SDK OAuth2

**Location:** `core/oauth2_client.py`

```python
class CoinbaseOAuth2Client:
    def __init__(
        self,
        client_id: str,
        client_secret: str,
        redirect_uri: str,
        scope: List[str],
    ):
        ...
    
    async def get_authorization_url(self, state: str) -> str:
        # Generate OAuth2 authorization URL
        ...
    
    async def exchange_code_for_tokens(self, code: str) -> dict:
        # Exchange authorization code for access + refresh tokens
        ...
    
    async def refresh_access_token(self, refresh_token: str) -> dict:
        # Refresh expired access token
        ...
    
    async def revoke_token(self, token: str) -> None:
        # Revoke access or refresh token
        ...
```

**Features:**
- Full OAuth2 authorization code flow
- Automatic token refresh
- Token revocation
- PKCE support (optional)
- State parameter for CSRF protection

### 10.2 .NET Library OAuth2

**Location:** `CoinbaseOauth2Client.cs`

```csharp
public sealed class CoinbaseOauth2Client {
    public Task<Dictionary<string, object>> GetAuthorizationUrlAsync(...);
    public Task<Dictionary<string, object>> ExchangeCodeForTokensAsync(...);
    public Task<Dictionary<string, object>> RefreshAccessTokenAsync(...);
    public Task RevokeTokenAsync(...);
}
```

**Features:**
- Authorization URL generation
- Code exchange
- Token refresh
- Token revocation

### 10.3 Gaps

#### ✅ **Both Support:**
- Authorization code flow
- Token exchange
- Token refresh
- Token revocation

#### ⚠️ **Potential Behavioral Differences:**
- Automatic refresh handling: Python may have more integrated refresh logic
- Error handling: Python uses `CoinbaseOAuth2Exception`, .NET may use generic exceptions
- PKCE support: Need to verify if .NET supports PKCE

---

## 11. Test Coverage

### 11.1 Python SDK Tests

**TBD:** Need to analyze `tests/` directory structure when available.

Expected test patterns:
- Unit tests for each manager
- Mock HTTP responses
- Mock WebSocket connections
- Exception handling tests
- Retry logic tests
- Circuit breaker tests

### 11.2 .NET Library Tests

**Location:** `Coinbase.AdvancedTrade.Tests/`, `Coinbase.AdvancedTrade.IntegrationTests/`

**Test Projects:**
- `Coinbase.AdvancedTrade.Tests` - Unit tests
- `Coinbase.AdvancedTrade.IntegrationTests` - Integration tests
- Multiple WebSocket test apps (Heartbeat, Ticker, Level2, etc.)

### 11.3 Gaps

**Assessment TBD** - Requires reading Python test files to determine coverage gaps.

---

## 12. Summary of Critical Gaps

### 12.1 Missing Functionality (Must Implement)

| Priority | Gap | Impact |
|----------|-----|--------|
| **P0** | Sandbox environment support | Cannot test against sandbox |
| **P0** | Circuit breaker implementation | No protection against cascading failures |
| **P0** | Rate limit backpressure | Doesn't respect rate limits proactively |
| **P1** | API key permissions endpoint | Cannot validate key capabilities |
| **P1** | Server time endpoint | Clock sync issues |
| **P1** | Structured logging consistency | Difficult to debug/monitor |
| **P2** | Rate limit info tracking | No visibility into rate limit state |
| **P2** | Timeout configuration | Cannot tune for slow networks |
| **P2** | Retry configuration at client level | Awkward retry policy setup |

### 12.2 Behavioral Alignment (Should Align)

| Priority | Gap | Recommendation |
|----------|-----|----------------|
| **P1** | Backoff formula difference | Align to Python: `base * 2^attempt` |
| **P1** | Rate limit header tracking | Track from `RateLimit-*` headers |
| **P2** | User-agent configuration | Add to `CoinbaseClient` constructor |
| **P2** | Convenience factory function | Add `CoinbaseClient.Create(...)` |
| **P3** | Environment enum | Add `Environment` enum similar to Python |

### 12.3 Extra Functionality (Evaluate)

| Manager | Status | Action Required |
|---------|--------|-----------------|
| **FeesManager** | Not in Python | Verify if valid Coinbase API endpoint |
| **AddressesManager** | Not in Python | Verify if valid Coinbase API endpoint |
| **TransactionsManager** | Not in Python | Verify if valid Coinbase API endpoint |
| **BatchOperationsManager** | Not in Python | Verify if valid Coinbase API endpoint |
| **FuturesManager** | Not in Python | Verify if valid Coinbase API endpoint |
| **PerpetualsManager** | Not in Python | Verify if valid Coinbase API endpoint |

**Action:** Query Coinbase Documentation MCP server to confirm if these endpoints are:
1. Valid and documented
2. Deprecated or legacy
3. Not part of Advanced Trade API

If valid, Python SDK should also implement them. If deprecated, .NET library should remove or mark as obsolete.

---

## 13. Implementation Roadmap

### Phase 1: Foundational Alignment (Critical)

1. **Add Environment Support**
   - Add `Environment` enum (Production, Sandbox)
   - Update all URLs to support sandbox
   - Add environment parameter to `CoinbaseClient` constructor
   - Update `CoinbaseAuthenticator` to handle environment

2. **Implement Circuit Breaker**
   - Port `CircuitBreaker` class from Python
   - Integrate with Polly circuit breaker policies
   - Add circuit breaker configuration
   - Add structured logging for state transitions

3. **Implement Rate Limit Tracking**
   - Parse `RateLimit-Limit`, `RateLimit-Remaining`, `RateLimit-Reset` headers
   - Store in `RateLimitInfo` class
   - Add `GetRateLimitInfo()` method to client
   - Implement automatic backpressure via `_maybe_wait_for_rate_limit()`

4. **Add Missing Endpoints**
   - Implement `GetApiKeyPermissions()` → `ApiKeyPermissions` model
   - Implement `GetServerTime()` → `ServerTime` model
   - Add to `AccountsManager` or new `AccountManager`

### Phase 2: Behavioral Alignment

1. **Align Retry Logic**
   - Change backoff formula to match Python: `base * 2^attempt`
   - Ensure jitter matches: ±25%
   - Add rate limit backpressure before requests

2. **Enhance Structured Logging**
   - Add consistent logging to all managers
   - Add request/response debug logs
   - Add retry attempt logs
   - Add rate limit state logs
   - Add correlation ID to all logs

3. **Configuration Improvements**
   - Add timeout configuration to client
   - Add max retries to client constructor
   - Add user-agent configuration
   - Add convenience factory function

### Phase 3: Testing & Validation

1. **Write Comprehensive Tests**
   - Test all new endpoints
   - Test circuit breaker state transitions
   - Test rate limit backpressure
   - Test retry logic with mock failures
   - Test sandbox vs production URLs

2. **Integration Testing**
   - Test against Coinbase sandbox environment
   - Validate all endpoints match Python SDK behavior
   - Validate exception types match expected errors
   - Validate WebSocket reconnection

### Phase 4: Extra Functionality Validation

1. **Query Coinbase Documentation**
   - Verify each extra .NET manager against official API docs
   - Determine if deprecated or valid
   - Decide whether to keep, remove, or mark obsolete

2. **Update Python SDK if Necessary**
   - If extra .NET managers are valid, implement in Python
   - Maintain parity between both SDKs

---

## 14. Testing Strategy (TDD)

For each gap identified, the following TDD workflow must be followed:

### 14.1 Test-First Development

1. **Write Failing Test**
   ```csharp
   [Fact]
   public async Task GetServerTime_ShouldReturnServerTime()
   {
       // Arrange
       var mock = new Mock<IAuthenticator>();
       var client = new CoinbaseClient(mock.Object);
       
       // Act
       var serverTime = await client.Account.GetServerTime();
       
       // Assert
       Assert.NotNull(serverTime);
       Assert.NotEmpty(serverTime.Iso);
       Assert.True(serverTime.Epoch > 0);
   }
   ```

2. **Implement Minimum Code to Pass**
   ```csharp
   public async Task<ServerTime> GetServerTime()
   {
       var path = "/api/v3/brokerage/time";
       var response = await _authenticator.SendAuthenticatedRequestAsync("GET", path);
       return ServerTime.FromDictionary(response);
   }
   ```

3. **Refactor & Verify**
   - Ensure code matches Python SDK behavior
   - Add error handling
   - Add logging
   - Re-run tests

### 14.2 Test Categories

**Unit Tests:**
- Mock all HTTP calls
- Test request/response serialization
- Test exception handling
- Test retry logic
- Test circuit breaker state transitions

**Integration Tests:**
- Test against Coinbase sandbox
- Validate actual API responses
- Test WebSocket connections
- Test OAuth2 flows

**Behavior Tests:**
- Compare .NET output to Python SDK output
- Validate retry timing
- Validate rate limit handling
- Validate exception types

---

## 15. Conclusion

This gap analysis has identified **significant alignment work** required to achieve full behavioral parity between the .NET library and the Python SDK:

**Critical Missing Features:**
- Sandbox environment support
- Circuit breaker pattern
- Rate limit backpressure
- API key permissions endpoint
- Server time endpoint

**Behavioral Differences:**
- Retry backoff formula
- Rate limit tracking
- Structured logging consistency

**Extra Functionality (Needs Validation):**
- 6 additional managers not in Python SDK

**Estimated Effort:**
- **Phase 1 (Foundational):** 2-3 days
- **Phase 2 (Behavioral):** 2-3 days
- **Phase 3 (Testing):** 2-3 days
- **Phase 4 (Validation):** 1-2 days

**Total:** ~7-11 days of focused development

All work must be done using strict TDD, with tests written first, implementation second, and validation against Python SDK behavior throughout.

---

## Appendices

### Appendix A: Python SDK Manager Method Inventory

**MarketDataManager:**
- `get_product(product_id)`
- `get_public_product(product_id)`
- `list_products(...)`
- `list_public_products(...)`
- `get_product_book(product_id, limit)`
- `get_best_bid_ask(product_ids)`
- `get_candles(product_id, start, end, granularity)`
- `get_market_trades(product_id, limit)`

**PortfolioManager:**
- `list_portfolios(portfolio_type, limit, after)`
- `get_portfolio(portfolio_uuid)`
- `create_portfolio(name)`
- `delete_portfolio(portfolio_uuid)`

**OrderManager:**
- `create_market_order(client_order_id, product_id, side, amount)`
- `create_limit_order(client_order_id, product_id, side, limit_price, base_size, ...)`
- `list_orders(product_id, order_status, limit, start_date, end_date, ...)`
- `get_order(order_id)`
- `cancel_orders(order_ids)`

**OrderHistoryManager:**
- `list_fills(order_id, product_id, start_date, end_date, limit, ...)`

**PaymentMethodManager:**
- `list_payment_methods()`
- `get_payment_method(payment_method_id)`

**ConvertManager:**
- `commit_convert_trade(from_account, to_account, amount, ...)`
- `get_convert_trade(trade_id)`

**AccountManager:**
- `get_api_key_permissions()`
- `get_server_time()`

**TransfersManager:**
- `transfer(from_portfolio_id, to_portfolio_id, amount, currency)`
- `get_transfer(transfer_id)`
- `list_transfers(limit, after)`

### Appendix B: .NET Library Manager Method Inventory

**TBD** - Requires systematic enumeration of all interface methods.

### Appendix C: Coinbase API Endpoint Reference

**TBD** - Query Coinbase Documentation MCP server for authoritative endpoint list.

---

**End of Gap Analysis**
