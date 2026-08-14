# EAF — Eaf.Dapper Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.Dapper Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Data Access / Complex Queries |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-dapper` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EF Core is not optimal for complex read-only reports, raw SQL, bulk operations, or legacy query tuning. EAF currently has no first-class Dapper integration for these scenarios.

### Objective

Create `Eaf.Dapper` middleware module that registers `IDapperConnectionProvider` and `IDapperRepository<T>` abstractions, integrates with ABP `IDbContextProvider`, and respects multi-tenancy and unit-of-work.

### Expected outcome

- New `src/Eaf.Dapper/` project.
- `IDapperRepository<TEntity>` with `Query`, `Execute`, `ExecuteScalar` methods.
- `DapperConnectionProvider` resolving the active `DbConnection` from ABP `IDbContextProvider`.
- Tests for query execution and connection lifecycle.

### Out of scope

- Dapper.Contrib CRUD (EF Core remains the default).
- Changing `IRepository<T>` public contracts.
- Stored procedure migration tooling.

## 2. Agent Role

Senior .NET/ABP engineer. Implement Dapper as an opt-in complement to EF Core, not a replacement.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not replace EF Core; do not modify `IRepository<T>`; do not push directly to remote.

## 4. Product Context

### Functional context

Allow application services to run optimized SQL queries and bulk operations alongside EF Core repositories.

### Technical context

- ABP `IDbContextProvider<TDbContext>`.
- Castle Windsor `IScopedIocManager` for UoW lifetime.
- `DbContext.Database.GetDbConnection()`.

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5
- Dapper 2.x or later
- EF Core 10
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.Dapper/
src/Eaf.Middleware.EntityFrameworkCore/
common.props
EAF.sln
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.EntityFrameworkCore/EntityFrameworkCore/EafMiddlewareDbContext.cs`
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.Dapper` module and tests.

### Subtasks

1. Create `src/Eaf.Dapper/` project.
2. Define `IDapperConnectionProvider` and `IDapperRepository<TEntity>`.
3. Implement `DapperConnectionProvider` using `IDbContextProvider<TDbContext>`.
4. Implement `DapperRepositoryBase<TEntity>` with `QueryAsync`, `ExecuteAsync`, `ExecuteScalarAsync`.
5. Create `EafDapperModule` with `[DependsOn(typeof(AbpEntityFrameworkCoreModule))]`.
6. Add `Eaf.Dapper.Tests`.
7. Wire into `EAF.sln` and `common.props`.

### Do not do

- Do not replace EF Core repositories.
- Do not write migration files for Dapper.
- Do not add UI code to this backend module.

## 6. Functional Requirements

### FR-001: Dapper connection provider

**Description:** Provide a way to obtain the active `IDbConnection` from the current ABP `DbContext`.

**Rules:**

- Use existing EF Core transaction/connection to avoid connection leaks.
- Respect multi-tenancy via `IDbContextProvider`.
- Throw `UserFriendlyException` if no active UoW exists.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `dbContextType` | `Type` | yes | Concrete `DbContext` registered with ABP |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `connection` | `IDbConnection` | Active connection from current UoW |

**Acceptance criteria:**

- [ ] `IDapperConnectionProvider.GetConnection()` returns a non-null open connection.
- [ ] Connection belongs to the same transaction as the current UoW.
- [ ] Throws when called outside a UoW.

### FR-002: Dapper repository

**Description:** Provide a generic repository abstraction for raw SQL queries returning domain entities or DTOs.

**Rules:**

- `QueryAsync<T>` maps raw SQL to `T`.
- `ExecuteAsync` runs non-query commands.
- `ExecuteScalarAsync<T>` runs scalar commands.
- SQL must be passed by caller; repository does not build SQL.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `sql` | `string` | yes | Valid SQL or stored procedure name |
| `parameters` | `object` | no | Anonymous or `DynamicParameters` |
| `commandType` | `CommandType` | no | `Text` or `StoredProcedure` |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `result` | `IEnumerable<T>` / `int` / `T` | Query, rows affected, or scalar |

**Acceptance criteria:**

- [ ] `QueryAsync` returns mapped results.
- [ ] `ExecuteAsync` returns rows affected.
- [ ] `ExecuteScalarAsync` returns the scalar value.

## 7. Business Rules

### BR-001: EF Core remains primary

Dapper is opt-in and secondary. Domain writes must still go through `IRepository<T>` and UoW.

### BR-002: Tenant isolation

`DapperConnectionProvider` must use the same connection string resolution as `IDbContextProvider` to preserve tenant isolation.

## 8. Domain Modeling

### Bounded Context

Data Access

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `DapperRepository<TEntity>` | generic | Executes raw SQL for `TEntity`-related reads |
| `DapperConnectionProvider` | singleton/transient | Resolves active `IDbConnection` |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `DapperOptions` | `DefaultCommandTimeout` | Positive integer or null |

## 9. Expected Architecture

### Architectural style

ABP modular infrastructure.

### Layers

```text
src/Eaf.Dapper/
  Dapper/
    IDapperConnectionProvider.cs
    DapperConnectionProvider.cs
    IDapperRepository.cs
    DapperRepositoryBase.cs
    DapperOptions.cs
  EafDapperModule.cs
  README.md
  Eaf.Dapper.csproj
test/Eaf.Dapper.Tests/
  DapperRepository_Tests.cs
  DapperConnectionProvider_Tests.cs
```

### Allowed dependencies

- `Abp`
- `Dapper`
- `Microsoft.EntityFrameworkCore`
- `System.Data.Common`

### Forbidden dependencies

- UI frameworks.
- Specific database drivers (use `System.Data.Common`).

## 10. API Contracts

No new HTTP endpoints. The module exposes configuration API:

```csharp
Configuration.Modules.Configure<EafDapperOptions>(options =>
{
    options.DefaultCommandTimeout = 30;
});
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public interface IDapperConnectionProvider
{
    IDbConnection GetConnection();
}

public interface IDapperRepository<TEntity> where TEntity : class, IEntity
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CommandType? commandType = null);
    Task<int> ExecuteAsync(string sql, object parameters = null, CommandType? commandType = null);
    Task<T> ExecuteScalarAsync<T>(string sql, object parameters = null, CommandType? commandType = null);
}
```

## 12. Persistence and Data

### Persisted entities

N/A — Dapper uses existing EF Core connection and schemas.

### Migration required

No.

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `IDbContextProvider<TDbContext>` | Resolve active connection | In-process | — | no |
| Database | Execute SQL | TCP | configured by provider | via Dapper |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| No active UoW | connection request | Throw `AbpException` or `UserFriendlyException` |
| Null SQL | `null` | Throw `ArgumentNullException` |
| Query returns no rows | valid SQL | Return empty enumerable |
| SQL timeout | long-running query | Throw `SqlException`/timeout exception wrapped in `UserFriendlyException` |

## 15. Few-Shot Examples

### Example 1: Happy path

```csharp
public class ReportAppService : ApplicationService
{
    private readonly IDapperRepository<MyEntity> _dapper;

    public ReportAppService(IDapperRepository<MyEntity> dapper)
    {
        _dapper = dapper;
    }

    public async Task<List<MyDto>> GetReportAsync()
    {
        var sql = "SELECT Id, Name FROM MyEntities WHERE IsActive = 1";
        return (await _dapper.QueryAsync<MyDto>(sql)).ToList();
    }
}
```

## 16. Non-Functional Requirements

### Performance

- Query execution overhead < 1 ms beyond raw ADO.NET.

### Security

- Do not build SQL from untrusted input.
- Document that callers are responsible for parameterization.

### Observability

- Structured logs via `ILogger` for SQL execution errors only.
- Do not log SQL parameters that may contain PII.

## 17. Mandatory Guardrails

- Do not replace EF Core.
- Do not modify `IRepository<T>` contracts.
- Do not add UI code.
- Stop and ask if Dapper version or licensing is ambiguous.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `DapperConnectionProvider` | Returns connection from current UoW, throws outside UoW |
| `DapperRepository` | Query, execute, scalar with in-memory SQLite |

### Integration tests

| Flow | Validation |
|---|---|
| Raw SQL report | Returns DTOs via EAF middleware DB |
| Bulk update | Rows affected via `ExecuteAsync` |

### xUnit example

```csharp
public class DapperRepository_Tests : AbpIntegratedTestBase<EafDapperModule>
{
    [Fact]
    public async Task Dado_QueryValida_Quando_Executar_Entao_RetornaResultados()
    {
        var repo = Resolve<IDapperRepository<MyEntity>>();
        var result = await repo.QueryAsync<MyDto>("SELECT 1 AS Id, 'Test' AS Name");
        result.Count().ShouldBe(1);
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.Dapper` compiles and packs as NuGet.
- [ ] `IDapperRepository<T>` resolves from DI in a test host.
- [ ] Query/execute/scalar methods return expected results.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Discovery — read EF Core integration and `IDbContextProvider` usage.
2. Design — decide Dapper version and `DapperConnectionProvider` lifetime.
3. Project setup — create `src/Eaf.Dapper/` and `test/Eaf.Dapper.Tests/`.
4. Implementation — connection provider, repository base, module, options.
5. Tests — unit tests with in-memory SQLite.
6. Documentation — `README.md` and spec index update.
7. Validation — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafDapperModule))]`.
- Remove usages of `IDapperRepository` in app services.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Connection lifecycle mismatch with EF Core | High | Medium | Always get connection from `IDbContextProvider` |
| SQL injection in application code | High | Low | Document parameterization; add analyzer warning |
| Dapper version conflicts | Low | Low | Pin version in `common.props` |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> Dapper is a complement, not a replacement. Keep all business writes in EF Core repositories and UoW.
