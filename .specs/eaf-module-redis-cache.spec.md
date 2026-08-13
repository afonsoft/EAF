# EAF — Eaf.RedisCache Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.RedisCache Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Runtime Caching |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-redis-cache` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF ships `Eaf.SqlServerCache` and `Eaf.SqliteCache` as distributed/local cache providers. There is no `Eaf.RedisCache` module, which is the standard high-performance distributed cache backend for ABP/ASP.NET Core applications running in multi-instance deployments.

### Objective

Create a reusable `Eaf.RedisCache` middleware module that implements `ICacheManager` and plugs into the ABP `CacheBase` infrastructure, mirroring the existing SQL cache modules.

### Expected outcome

- New `src/Eaf.RedisCache/` project with `EafRedisCache`, `EafRedisCacheManager`, `EafRedisCacheModule`, and `RedisCacheConfigurationExtensions`.
- `IDistributedCache` backed by Redis using `StackExchange.Redis`.
- Unit and integration tests following the `Eaf.SqlServerCache` pattern.
- Generated templates can enable Redis cache by calling `Configuration.Caching.UseRedis(...)`.

### Out of scope

- Redis clustering topology configuration beyond connection string.
- Cache invalidation over SignalR (covered by `eaf-module-signalr.spec.md`).
- UI changes; this is a backend-only module.

## 2. Agent Role

Senior .NET/ABP engineer. Implement the module following the exact pattern of `Eaf.SqlServerCache` and `Eaf.SqliteCache`.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not push directly to remote; do not change `ICacheManager` public contracts; do not publish NuGet packages automatically.

## 4. Product Context

### Functional context

The module is a drop-in cache provider. Consumers replace `Configuration.Caching.UseSqlServer(...)` with `Configuration.Caching.UseRedis(...)` in the `Startup` / `EafModule` configuration.

### Technical context

- ABP `CacheManagerBase<T>` and `ICacheManager`.
- Existing `Eaf.SqlServerCache` and `Eaf.SqliteCache` as reference implementations.
- `IDistributedCache` abstraction from `Microsoft.Extensions.Caching.Distributed`.

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5 runtime caching
- `StackExchange.Redis` (or `Microsoft.Extensions.Caching.StackExchangeRedis`)
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.SqlServerCache/
src/Eaf.SqliteCache/
common.props
test/
```

### Context files the agent must read before implementation

- `src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCache.cs`
- `src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCacheManager.cs`
- `src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCacheModule.cs`
- `src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/SqlServerCacheConfigurationExtensions.cs`
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.RedisCache` module and tests, enabling Redis as a distributed cache provider for EAF.

### Subtasks

1. Create `src/Eaf.RedisCache/` project.
2. Implement `EafRedisCache` extending `CacheBase`.
3. Implement `EafRedisCacheManager` implementing `ICacheManager`.
4. Create `EafRedisCacheModule`.
5. Create `RedisCacheConfigurationExtensions.UseRedis`.
6. Add `Eaf.RedisCache.Tests`.
7. Wire the module into `Eaf.sln` and update `common.props` if needed.
8. Add a `README.md` for the module.

### Do not do

- Do not change the public `ICacheManager` / `CacheBase` contracts.
- Do not add UI code to this backend module.
- Do not introduce non-open-source Redis clients.

## 6. Functional Requirements

### FR-001: Redis cache provider

**Description:** The module must register `IDistributedCache` backed by Redis and use it inside an ABP `CacheBase` implementation.

**Rules:**

- Use `Microsoft.Extensions.Caching.StackExchangeRedis` or `StackExchange.Redis`.
- Serialize values using the same JSON-with-type-prefix scheme as `EafSqlServerCache`.
- Compress data with `GZipStream` as in `EafSqlServerCache`.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `ConnectionString` | `string` | yes | Redis connection string or `appsettings.json` key |
| `InstanceName` | `string` | no | Prefix for cache keys; default `"EAF"` |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `ICache` | `EafRedisCache` | Cache implementation per named cache |
| `ICacheManager` | `EafRedisCacheManager` | Cache manager resolving named caches |

**Acceptance criteria:**

- [ ] `Configuration.Caching.UseRedis(...)` replaces the cache manager.
- [ ] `EafRedisCache` stores and retrieves objects correctly.
- [ ] Redis keys are prefixed with the cache name and `InstanceName`.

### FR-002: Connection resilience

**Description:** The module must handle transient Redis failures gracefully.

**Rules:**

- Log errors and return `null`/default on read failures (same as SQL cache).
- Do not crash the application if Redis is unavailable on startup.
- Support reconnect policy via `StackExchange.Redis` defaults.

**Acceptance criteria:**

- [ ] Redis unavailable does not crash the host.
- [ ] Cache read failures are logged and return default values.

### FR-003: Clear operation

**Description:** Provide a way to clear a named cache.

**Rules:**

- Use `StackExchange.Redis` `IServer`/`IDatabase` to scan and delete keys matching the cache prefix.
- If scan is not available in the abstraction, `Clear` may be documented as best-effort.

**Acceptance criteria:**

- [ ] `cache.Clear()` removes keys with the cache name prefix.
- [ ] Integration test confirms keys are removed.

## 7. Business Rules

### BR-001: Backward compatibility

The module must not change existing cache consumer code. It only replaces the `ICacheManager` implementation.

### BR-002: Configuration via `appsettings.json`

Connection string must be read from standard `ConnectionStrings:Redis` or `Eaf:RedisCache:ConnectionString`.

### BR-003: Same serialization format

`EafRedisCache` must reuse the JSON + type-prefix + GZip format from `EafSqlServerCache` so existing cached entries can be read if a deployment switches provider.

## 8. Domain Modeling

### Bounded Context

Runtime Caching

### Aggregates

N/A — infrastructure module.

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `EafRedisCache` | cache name | Stores/retrieves compressed serialized values in Redis |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `RedisCacheOptions` | `ConnectionString`, `InstanceName` | Connection string required |

### Domain Events

N/A.

### Expected C# style

```csharp
public sealed class RedisCacheOptions
{
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; } = "EAF";
}
```

## 9. Expected Architecture

### Architectural style

ABP modular infrastructure.

### Layers

```text
src/Eaf.RedisCache/
  Runtime/Caching/Redis/
    EafRedisCache.cs
    EafRedisCacheManager.cs
    EafRedisCacheModule.cs
    RedisCacheConfigurationExtensions.cs
  README.md
  Eaf.RedisCache.csproj
test/Eaf.RedisCache.Tests/
  EafRedisCache_Tests.cs
  EafRedisCacheModule_Tests.cs
```

### Allowed dependencies

- `Abp`
- `Abp.Runtime.Caching`
- `Microsoft.Extensions.Caching.Distributed`
- `Microsoft.Extensions.Caching.StackExchangeRedis` or `StackExchange.Redis`
- `Microsoft.Extensions.Options`

### Forbidden dependencies

- UI frameworks.
- EF Core (not needed for Redis cache).

## 10. API Contracts

No new HTTP endpoints. The module exposes configuration API:

```csharp
Configuration.Caching.UseRedis(options =>
{
    options.ConnectionString = "localhost:6379";
    options.InstanceName = "EAF";
});
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public class RedisCacheOptions
{
    public string ConnectionString { get; set; }
    public string InstanceName { get; set; } = "EAF";
}
```

### Expected service style

```csharp
public class EafRedisCacheManager : CacheManagerBase<ICache>, ICacheManager
{
    // mirrors EafSqlServerCacheManager
}
```

## 12. Persistence and Data

### Persisted entities

N/A — Redis is the store.

### Migration required

No.

### Compatibility

- [ ] Does not break existing data.
- [ ] No migration needed.
- [ ] Does not expose sensitive data in Redis keys.

## 13. Integrations

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `ICacheManager` | Resolve named caches | In-process | — | no |
| `IDistributedCache` | Redis read/write | TCP | configured by StackExchange.Redis | via StackExchange.Redis |

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| Redis | serialized cache values | serialized cache values | TLS optional via connection string |

### Expected failures

- Redis unreachable.
- Serialization mismatch.

### Resilience strategy

- Log and return default on read failures.
- Connection retry handled by `StackExchange.Redis`.

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Redis unavailable | connection failure | Log error; cache reads return default; writes are no-ops |
| Null value | `Set("key", null)` | Store empty byte array or no-op; reads return null |
| Empty cache name | `GetCache("")` | Use `Default` cache name or raise validation error |
| Key with special characters | `:` in key | Escape or sanitize using `InstanceName` prefix only |

## 15. Few-Shot Examples

### Example 1: Happy path

```csharp
Configuration.Caching.UseRedis(o => o.ConnectionString = "localhost:6379");
var cacheManager = Resolve<ICacheManager>();
var cache = cacheManager.GetCache("Users");
cache.Set("user:1", new UserDto { Id = 1, Name = "Alice" });
var user = cache.Get("user:1", "user:1");
```

**Expected output:** `user` is the stored `UserDto`.

### Example 2: Redis unavailable

```csharp
Configuration.Caching.UseRedis(o => o.ConnectionString = "invalid:6379");
var cache = Resolve<ICacheManager>().GetCache("Users");
var user = cache.Get("user:1", "fallback");
```

**Expected output:** `user` is `"fallback"` and an error is logged.

## 16. Non-Functional Requirements

### Performance

- P95 read latency < 5 ms when Redis is local.
- Serialização/compressão deve adicionar < 1 ms para payloads pequenos.

### Security

- Do not log Redis connection strings.
- Support Redis TLS via connection string.

### Observability

- Structured logs via `ILogger` for Redis errors.
- OpenTelemetry traces for cache get/set if `Eaf.OpenTelemetry` is present.

### Reliability

- Fail-open on Redis failures.
- Idempotent writes.

### Maintainability

- Mirror `Eaf.SqlServerCache` structure.
- README.md with installation and usage.

## 17. Mandatory Guardrails

- Do not modify `ICacheManager` / `CacheBase` public contracts.
- Do not publish NuGet packages automatically.
- Do not add UI or frontend code.
- Stop and ask if `StackExchange.Redis` license/dependency is ambiguous.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `EafRedisCache` | Serialization round-trip, compression, key prefixing |
| `EafRedisCacheManager` | Creates caches on demand, disposes caches |
| `RedisCacheConfigurationExtensions` | Replaces `ICacheManager` and `IDistributedCache` |

### Integration tests

| Flow | Validation |
|---|---|
| Redis container available | Set/Get/Clear flow works end-to-end |
| Redis unavailable | Cache falls back to default values and logs error |

### Contract tests

- [ ] `ICacheManager` consumers continue to compile.
- [ ] `IDistributedCache` is registered as singleton or per the Redis provider.

### xUnit example

```csharp
public class EafRedisCache_Tests : AbpIntegratedTestBase<EafRedisCacheModule>
{
    private readonly ICacheManager _cacheManager;

    public EafRedisCache_Tests()
    {
        _cacheManager = Resolve<ICacheManager>();
    }

    [Fact]
    public void Dado_CacheConfigurado_Quando_GravarEler_Entao_DeveRetornarValor()
    {
        var cache = _cacheManager.GetCache("Test");
        cache.Set("key", "value");
        var result = cache.Get("key", "default");
        result.ShouldBe("value");
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.RedisCache` compiles and packs as NuGet.
- [ ] `UseRedis` replaces the cache manager in a test host.
- [ ] Integration tests pass against a local Redis instance.
- [ ] README.md explains installation and configuration.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. **Discovery** — read `Eaf.SqlServerCache` and `Eaf.SqliteCache`.
2. **Design** — decide Redis client package and `Clear` strategy.
3. **Project setup** — create `src/Eaf.RedisCache/` and `test/Eaf.RedisCache.Tests/`.
4. **Implementation** — `EafRedisCache`, `EafRedisCacheManager`, module, extensions.
5. **Tests** — unit and integration tests.
6. **Documentation** — README.md and spec index update.
7. **Validation** — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Disable by removing `[DependsOn(typeof(EafRedisCacheModule))]` and reverting to `Eaf.SqlServerCache` or in-memory cache.
- Remove Redis connection string from `appsettings.json`.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| `StackExchange.Redis` v2/v3 API changes | Medium | Low | Pin package version in `common.props` |
| Redis `Clear` scans large key spaces | Medium | Medium | Document best-effort clear and offer prefix-based delete |
| Different serialization from SQL cache | High | Low | Reuse exact `EafSqlServerCache` serializer |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> This is a backend infrastructure module. Follow the `Eaf.SqlServerCache` pattern exactly. Do not add UI or change cache public contracts.
