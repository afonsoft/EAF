# EAF — Eaf.MongoDB Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.MongoDB Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | NoSQL Data Access |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-mongodb` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF is built entirely on EF Core with SQL Server/SQLite/PostgreSQL. Some workloads (logs, telemetry, IoT, catalogs) would benefit from a MongoDB repository option, but there is no EAF MongoDB module.

### Objective

Create `Eaf.MongoDB` middleware module that provides `IMongoRepository<T>` and `IMongoDbContext` abstractions, integrates with ABP `IRepository<T>`, and supports multi-tenancy via collection/database name isolation.

### Expected outcome

- New `src/Eaf.MongoDB/` project.
- `EafMongoDbContext` and `EafMongoDbModule`.
- `MongoDbRepository<T>` implementing `IRepository<T>`.
- Tests with a MongoDB in-memory/testcontainers provider.

### Out of scope

- EF Core provider for MongoDB.
- Replacing existing SQL-based modules.
- MongoDB transactions in sharded clusters.

## 2. Agent Role

Senior .NET/ABP engineer. Implement MongoDB as an optional NoSQL repository layer, preserving existing EF Core modules.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not replace EF Core; do not change `IRepository<T>` contracts; do not push directly to remote.

## 4. Product Context

### Functional context

Allow generated templates to use MongoDB for selected aggregates while keeping relational modules on EF Core.

### Technical context

- ABP `IRepository<T, TKey>`.
- `MongoDB.Driver` 2.x.
- ABP MongoDB patterns (`AbpMongoDbContext`, `IMongoCollection`).

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5
- `MongoDB.Driver` 2.x
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.MongoDB/
src/Eaf.Middleware.Core/
common.props
EAF.sln
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Core/Domain/`
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.MongoDB` module and tests.

### Subtasks

1. Create `src/Eaf.MongoDB/` project.
2. Define `IEafMongoDbContext` and `IMongoRepository<T>`.
3. Implement `EafMongoDbContext` with collection mapping and tenant resolver.
4. Implement `MongoDbRepository<T>` for CRUD and query operations.
5. Create `EafMongoDbModule`.
6. Add `Eaf.MongoDB.Tests`.
7. Wire into `EAF.sln` and `common.props`.

### Do not do

- Do not replace EF Core modules.
- Do not modify `IRepository<T>` public contracts.
- Do not add UI code to this backend module.

## 6. Functional Requirements

### FR-001: MongoDB context

**Description:** Provide an `IEafMongoDbContext` that resolves collections and applies tenant/database name isolation.

**Rules:**

- `IMongoClient` and `IMongoDatabase` resolved from DI.
- Tenant isolation via database or collection prefix.
- `GetCollection<T>` returns `IMongoCollection<T>`.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `connectionString` | `string` | yes | MongoDB connection string |
| `databaseName` | `string` | yes | Default database name |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `collection` | `IMongoCollection<T>` | Tenant-scoped collection |

**Acceptance criteria:**

- [ ] `IEafMongoDbContext.GetCollection<T>()` returns a non-null collection.
- [ ] Tenant resolution changes collection/database name when multi-tenancy is enabled.

### FR-002: MongoDB repository

**Description:** Provide a generic repository implementing ABP `IRepository<T, TKey>` over MongoDB.

**Rules:**

- CRUD operations: `GetAsync`, `InsertAsync`, `UpdateAsync`, `DeleteAsync`.
- Query via `IQueryable<T>` using MongoDB LINQ provider.
- `InsertAsync` sets entity `Id` if it is `Guid` or `ObjectId`.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `id` | `TKey` | yes | Entity identity |
| `entity` | `T` | yes | Valid entity |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `entity` | `T` | Persisted/retrieved entity |

**Acceptance criteria:**

- [ ] `InsertAsync` persists entity.
- [ ] `GetAsync` retrieves by id.
- [ ] `GetAll()` returns `IQueryable<T>`.
- [ ] `DeleteAsync` removes entity.

## 7. Business Rules

### BR-001: EF Core remains primary

MongoDB is opt-in. Existing modules and templates must not depend on MongoDB.

### BR-002: Tenant isolation

Default tenant isolation uses database-per-tenant or collection-prefix; configurable in `EafMongoDbOptions`.

## 8. Domain Modeling

### Bounded Context

NoSQL Data Access

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `MongoDbRepository<T>` | generic | MongoDB-backed CRUD and queries |
| `EafMongoDbContext` | singleton/scoped | Resolves collections and tenant database |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `EafMongoDbOptions` | `ConnectionString`, `DatabaseName`, `TenantIsolationMode` | Connection string and database required |

## 9. Expected Architecture

### Architectural style

ABP modular infrastructure.

### Layers

```text
src/Eaf.MongoDB/
  MongoDb/
    IEafMongoDbContext.cs
    EafMongoDbContext.cs
    IMongoRepository.cs
    MongoDbRepository.cs
    EafMongoDbModule.cs
    EafMongoDbOptions.cs
  README.md
  Eaf.MongoDB.csproj
test/Eaf.MongoDB.Tests/
  MongoDbRepository_Tests.cs
  EafMongoDbContext_Tests.cs
```

### Allowed dependencies

- `Abp`
- `MongoDB.Driver`
- `System.Linq`

### Forbidden dependencies

- EF Core.
- UI frameworks.

## 10. API Contracts

No new HTTP endpoints. The module exposes configuration API:

```csharp
Configuration.Modules.Configure<EafMongoDbOptions>(options =>
{
    options.ConnectionString = "mongodb://localhost:27017";
    options.DatabaseName = "EAF";
    options.TenantIsolationMode = TenantIsolationMode.Database;
});
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public interface IEafMongoDbContext
{
    IMongoCollection<T> GetCollection<T>() where T : class, IEntity;
}

public enum TenantIsolationMode
{
    None,
    CollectionPrefix,
    Database
}

public class EafMongoDbOptions
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
    public TenantIsolationMode TenantIsolationMode { get; set; } = TenantIsolationMode.CollectionPrefix;
}
```

## 12. Persistence and Data

### Persisted entities

Collections per entity type. No EF migrations; use MongoDB schema-on-read.

### Migration required

No.

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `_id` | `Id` | Primary key |
| `tenant` | `TenantId` (if exists) | Multi-tenancy queries |

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| MongoDB | Persist and query documents | TCP | driver default | via driver |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Duplicate key | insert existing id | Throw `MongoWriteException` wrapped in `UserFriendlyException` |
| Unavailable server | connection failure | Log and throw `UserFriendlyException` |
| Null entity | `null` | Throw `ArgumentNullException` |

## 15. Few-Shot Examples

### Example 1: Happy path

```csharp
public class LogRepository : MongoDbRepository<AuditLog>, IRepository<AuditLog, Guid>
{
    public LogRepository(IEafMongoDbContext context) : base(context) { }
}
```

## 16. Non-Functional Requirements

### Performance

- CRUD operations < 20 ms P95 local.

### Security

- Do not log connection strings.
- Support MongoDB TLS via connection string.

### Observability

- Structured logs via `ILogger` for connection and command errors.

## 17. Mandatory Guardrails

- Do not replace EF Core modules.
- Do not modify `IRepository<T>` contracts.
- Do not add UI code.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `MongoDbRepository<T>` | CRUD with in-memory `IMongoCollection` mock |
| `EafMongoDbContext` | Collection name resolution, tenant prefix |

### Integration tests

| Flow | Validation |
|---|---|
| MongoDB testcontainer | Insert/Get/Delete round-trip |
| Tenant isolation | Separate collections/databases per tenant |

### xUnit example

```csharp
public class MongoDbRepository_Tests : AbpIntegratedTestBase<EafMongoDbModule>
{
    [Fact]
    public async Task Dado_Entidade_Quando_Inserir_Entao_RecuperaPorId()
    {
        var repo = Resolve<IRepository<AuditLog, Guid>>();
        var log = new AuditLog { Id = Guid.NewGuid(), UserName = "test" };
        await repo.InsertAsync(log);
        var found = await repo.GetAsync(log.Id);
        found.UserName.ShouldBe("test");
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.MongoDB` compiles and packs as NuGet.
- [ ] `IRepository<T, Guid>` backed by MongoDB resolves from DI.
- [ ] CRUD operations pass integration tests with MongoDB.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Discovery — inspect ABP MongoDB patterns in ABP source if available.
2. Design — decide `IRepository<T>` adapter strategy.
3. Project setup — create `src/Eaf.MongoDB/` and `test/Eaf.MongoDB.Tests/`.
4. Implementation — context, repository, module, options.
5. Tests — unit and integration tests.
6. Documentation — `README.md` and spec index update.
7. Validation — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafMongoDbModule))]`.
- Revert repositories to EF Core.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| `IRepository<T>` contract mismatch | High | Medium | Implement full `IRepository<T>` carefully; integration tests |
| MongoDB.Driver major version changes | Medium | Low | Pin version in `common.props` |
| Multi-tenancy isolation bugs | High | Medium | Integration tests per tenant mode |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> MongoDB is an optional NoSQL repository backend. Do not change relational modules or `IRepository<T>` public contracts.
