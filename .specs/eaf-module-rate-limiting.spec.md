# EAF — Eaf.RateLimiting Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.RateLimiting Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Security / Resilience |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-rate-limiting` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF does not ship a built-in rate limiting module for API endpoints. Without throttling, endpoints are vulnerable to abuse and cascading failures.

### Objective

Create `Eaf.RateLimiting` middleware module that integrates `AspNetCoreRateLimit` or .NET 8+ `System.Threading.RateLimiting` with ABP, providing IP and user-based rate limits configurable per endpoint.

### Expected outcome

- New `src/Eaf.RateLimiting/` project.
- `EafRateLimitingModule` registering middleware and options.
- `IRateLimitingStore` abstraction backed by Redis or in-memory.
- Tests for limit enforcement and store behavior.

### Out of scope

- DDoS protection at network edge.
- Billing/quotas per tenant.
- UI for rate limit configuration.

## 2. Agent Role

Senior .NET/ABP engineer. Implement non-invasive API rate limiting middleware.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not modify existing `Authorization` filters; do not push directly to remote.

## 4. Product Context

### Functional context

Protect public and admin API endpoints by limiting requests per IP or authenticated user.

### Technical context

- `IStartupConfiguration` / `IApplicationBuilder` middleware.
- `AspNetCoreRateLimit` 5.x or .NET 8 `System.Threading.RateLimiting`.
- Redis for distributed rate limiting (optional; in-memory default).

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5
- `AspNetCoreRateLimit` 5.x or `System.Threading.RateLimiting`
- Redis (optional)
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.RateLimiting/
src/Eaf.Middleware.Web.Core/
Templates/Api/
common.props
EAF.sln
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Web.Core/Startup/`
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.RateLimiting` module and tests.

### Subtasks

1. Create `src/Eaf.RateLimiting/` project.
2. Define `EafRateLimitingOptions` with general and endpoint rules.
3. Implement middleware or `IEndpointFilter` that enforces rate limits.
4. Implement `IRateLimitingStore` with in-memory and Redis backends.
5. Create `EafRateLimitingModule`.
6. Add `Eaf.RateLimiting.Tests`.
7. Wire into `EAF.sln` and `common.props`.

### Do not do

- Do not modify authorization logic.
- Do not add per-tenant billing quotas.
- Do not add UI code to this backend module.

## 6. Functional Requirements

### FR-001: IP and user-based limits

**Description:** Enforce request limits by client IP or authenticated user identity.

**Rules:**

- IP limits use `HttpContext.Connection.RemoteIpAddress`.
- User limits use `AbpSession.UserId` when authenticated, falling back to IP.
- Limits are configurable per endpoint or global.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `period` | `string` | yes | `1s`, `1m`, `1h`, `1d` |
| `limit` | `long` | yes | Max requests in period |
| `endpoint` | `string` | no | Path pattern or `*` for global |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `rateLimitResult` | `RateLimitLease` / `bool` | Allowed or throttled |

**Acceptance criteria:**

- [ ] Requests within limit succeed.
- [ ] Requests over limit return 429 with `Retry-After` header.
- [ ] Different identities have separate counters.

### FR-002: Distributed store abstraction

**Description:** Provide `IRateLimitingStore` that can use in-memory or Redis counters.

**Rules:**

- In-memory store for single-instance.
- Redis store for multi-instance deployments.
- Store keys include identity, endpoint, and period.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `key` | `string` | yes | Composite rate limit key |
| `period` | `TimeSpan` | yes | Counter window |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `count` | `long` | Current request count |

**Acceptance criteria:**

- [ ] `IRateLimitingStore.IncrementAsync` increments and returns count.
- [ ] Redis store uses sliding window or fixed window.

## 7. Business Rules

### BR-001: Opt-in middleware

Rate limiting is disabled by default. Enable by adding `EafRateLimitingModule` and middleware.

### BR-002: Least-privileged defaults

Default global rules are permissive and documented. Admin can override per endpoint.

## 8. Domain Modeling

### Bounded Context

Security / Resilience

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `EafRateLimitingMiddleware` | middleware | Inspects requests and enforces limits |
| `RateLimitingStore` | service | Maintains counters |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `EafRateLimitingOptions` | `GlobalRules`, `EndpointRules`, `StoreType` | At least one rule if enabled |
| `RateLimitRule` | `Endpoint`, `Period`, `Limit` | Valid period format, positive limit |

## 9. Expected Architecture

### Architectural style

ASP.NET Core middleware / ABP module.

### Layers

```text
src/Eaf.RateLimiting/
  RateLimiting/
    EafRateLimitingModule.cs
    EafRateLimitingOptions.cs
    EafRateLimitingMiddleware.cs
    IRateLimitingStore.cs
    InMemoryRateLimitingStore.cs
    RedisRateLimitingStore.cs
    RateLimitRule.cs
  README.md
  Eaf.RateLimiting.csproj
test/Eaf.RateLimiting.Tests/
  EafRateLimitingMiddleware_Tests.cs
  RateLimitingStore_Tests.cs
```

### Allowed dependencies

- `Abp`
- `Microsoft.AspNetCore.Http.Abstractions`
- `System.Threading.RateLimiting` or `AspNetCoreRateLimit`
- `StackExchange.Redis` (optional)

### Forbidden dependencies

- UI frameworks.
- EF Core.

## 10. API Contracts

No new HTTP endpoints. The module exposes configuration API:

```csharp
Configuration.Modules.Configure<EafRateLimitingOptions>(options =>
{
    options.Enabled = true;
    options.GlobalRules.Add(new RateLimitRule { Period = "1m", Limit = 100 });
    options.EndpointRules.Add(new RateLimitRule { Endpoint = "/api/services/app/Account/Login", Period = "1m", Limit = 5 });
});
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public class EafRateLimitingOptions
{
    public bool Enabled { get; set; }
    public RateLimitingStoreType StoreType { get; set; } = RateLimitingStoreType.InMemory;
    public List<RateLimitRule> GlobalRules { get; set; } = new List<RateLimitRule>();
    public List<RateLimitRule> EndpointRules { get; set; } = new List<RateLimitRule>();
}

public class RateLimitRule
{
    public string Endpoint { get; set; } = "*";
    public string Period { get; set; } = "1m";
    public long Limit { get; set; } = 100;
}

public enum RateLimitingStoreType
{
    InMemory,
    Redis
}
```

## 12. Persistence and Data

N/A — counters are ephemeral.

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| Redis | Distributed counters | TCP | driver default | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Store unavailable | Redis failure | Fail-open (allow request) or use in-memory fallback |
| Missing identity | no IP/user | Use anonymous fallback key |
| Rule conflict | global + endpoint rules | Most restrictive endpoint rule wins |

## 15. Few-Shot Examples

### Example 1: Limit exceeded

```http
POST /api/services/app/Account/Login
// 5 requests in 1 minute from same IP
```

**Expected output:**

```http
HTTP/1.1 429 Too Many Requests
Retry-After: 45
```

## 16. Non-Functional Requirements

### Performance

- Rate limiting decision < 1 ms for in-memory store.
- Redis store < 5 ms P95.

### Security

- Do not leak stack traces on 429 responses.
- Do not log client IPs in plain text.

### Observability

- Metrics for throttled requests.
- Structured logs for store failures.

## 17. Mandatory Guardrails

- Do not modify authorization filters.
- Do not make rate limiting mandatory by default.
- Do not log sensitive identity data.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `EafRateLimitingMiddleware` | Allows under limit, returns 429 over limit |
| `InMemoryRateLimitingStore` | Sliding/fixed window counts |
| `RedisRateLimitingStore` | Distributed counter (mock) |

### Integration tests

| Flow | Validation |
|---|---|
| Middleware pipeline | 429 returned after limit reached |
| Redis store | Counter shared across instances (integration with Redis container) |

### xUnit example

```csharp
public class EafRateLimitingMiddleware_Tests
{
    [Fact]
    public async Task Dado_LimiteExcedido_Quando_Requisitar_Entao_Retorna429()
    {
        var client = _factory.CreateClient();
        for (int i = 0; i < 6; i++)
        {
            var response = await client.PostAsync("/api/services/app/Account/Login", null);
            if (i == 5) response.StatusCode.ShouldBe(HttpStatusCode.TooManyRequests);
        }
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.RateLimiting` compiles and packs as NuGet.
- [ ] Middleware returns 429 when limit exceeded.
- [ ] In-memory and Redis stores pass tests.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Discovery — inspect `Startup.cs` and existing middleware order.
2. Design — choose .NET built-in rate limiting vs `AspNetCoreRateLimit`.
3. Project setup — create `src/Eaf.RateLimiting/` and `test/Eaf.RateLimiting.Tests/`.
4. Implementation — options, store, middleware, module.
5. Tests — unit and integration.
6. Documentation — `README.md` and spec index update.
7. Validation — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Remove `app.UseEafRateLimiting()` from `Startup`.
- Remove `[DependsOn(typeof(EafRateLimitingModule))]`.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| False positives blocking legitimate users | High | Medium | Default limits permissive; allow override per endpoint |
| Store failure causing outage | High | Low | Fail-open or fallback to in-memory |
| Middleware order conflicts | Medium | Medium | Place after authentication, before controllers |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> Rate limiting is a safety net, not a primary security boundary. Keep defaults permissive and make limits configurable.
