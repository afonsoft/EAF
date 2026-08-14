# EAF — Eaf.Quartz Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.Quartz Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Background Jobs / Scheduling |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-quartz` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF currently uses Hangfire for background jobs. Hangfire works well but some deployments prefer Quartz.NET for cron scheduling, clustering, and job store flexibility. There is no `Eaf.Quartz` module.

### Objective

Create `Eaf.Quartz` middleware module that registers `IScheduler` from Quartz.NET, maps ABP `IBackgroundJob<T>` jobs to Quartz jobs, and supports persistent job stores (RAM, ADO.NET, MongoDB).

### Expected outcome

- New `src/Eaf.Quartz/` project.
- `EafQuartzModule`, `EafQuartzJobFactory`, `EafQuartzOptions`.
- `IBackgroundJobScheduler` adapter that can use Quartz instead of Hangfire.
- Tests for job scheduling, execution, and clustering configuration.

### Out of scope

- Replacing Hangfire.
- Quartz dashboard UI.
- Native job authoring outside ABP `IBackgroundJob<T>`.

## 2. Agent Role

Senior .NET/ABP engineer. Implement Quartz as an alternative scheduler, not a replacement.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not remove Hangfire; do not change ABP background job contracts.

## 4. Product Context

### Functional context

Generated templates can opt-in to Quartz for cron-based scheduling and clustered job execution.

### Technical context

- ABP `IBackgroundJob<T>` and `IBackgroundJobManager`.
- Quartz.NET 3.x `IScheduler`, `IJob`, `JobBuilder`, `TriggerBuilder`.
- Castle Windsor `IScopedIocManager` for job lifetime.

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5
- Quartz 3.x
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.Quartz/
src/Eaf.Middleware.Worker/
common.props
EAF.sln
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Worker/`
- `common.props`
- `EAF.sln`

## 5. Task Definition

### Main task

Create the `Eaf.Quartz` module and tests.

### Subtasks

1. Create `src/Eaf.Quartz/` project.
2. Define `EafQuartzOptions` for scheduler configuration.
3. Implement `EafQuartzJobFactory` for DI-based job creation.
4. Implement `EafQuartzJobAdapter<TJob, TArgs>` that runs `IBackgroundJob<TArgs>`.
5. Implement `EafQuartzBackgroundJobManager` implementing `IBackgroundJobManager`.
6. Create `EafQuartzModule`.
7. Add `Eaf.Quartz.Tests`.
8. Wire into `EAF.sln` and `common.props`.

### Do not do

- Do not remove Hangfire.
- Do not change `IBackgroundJob<T>` contracts.
- Do not add UI code to this backend module.

## 6. Functional Requirements

### FR-001: Quartz scheduler registration

**Description:** Register a Quartz `IScheduler` with Castle Windsor and start it on module post-initialize.

**Rules:**

- Use `Quartz.Extensions.DependencyInjection` or manual Windsor registration.
- Support RAM job store by default; ADO.NET store via configuration.
- Singleton `IScheduler`.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `schedulerName` | `string` | no | Defaults to `"EafScheduler"` |
| `jobStoreType` | `string` | no | `Ram`, `Ado`, `Mongo` |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `scheduler` | `IScheduler` | Quartz scheduler instance |

**Acceptance criteria:**

- [ ] `IScheduler` resolves from DI.
- [ ] Scheduler starts on `PostInitialize` and shuts down on `Shutdown`.

### FR-002: Background job adapter

**Description:** Map ABP `IBackgroundJob<TArgs>` to Quartz jobs and triggers.

**Rules:**

- `EnqueueAsync` creates an immediate one-time job.
- `ScheduleAsync` with cron expression creates a recurring job.
- Jobs are executed in a scoped Windsor lifestyle.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `args` | `TArgs` | yes | Job arguments serialized to `JobDataMap` |
| `cron` | `string` | no | Cron expression for recurring jobs |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `jobKey` | `JobKey` | Scheduled job identifier |

**Acceptance criteria:**

- [ ] `EnqueueAsync` runs the job once.
- [ ] `ScheduleAsync` with cron runs the job on schedule.
- [ ] Failed jobs are logged and retried based on Quartz misfire policy.

## 7. Business Rules

### BR-001: Hangfire remains default

Quartz is opt-in. Generated templates choose between `Eaf.Hangfire` and `Eaf.Quartz` modules.

### BR-002: Job serialization

Args are serialized with ABP `IJsonSerializer` and stored in `JobDataMap`. Do not store secrets.

## 8. Domain Modeling

### Bounded Context

Background Jobs / Scheduling

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `EafQuartzBackgroundJobManager` | singleton | Adapts ABP background job API to Quartz |
| `EafQuartzJobFactory` | scoped | Creates job instances from Windsor |
| `EafQuartzJobAdapter<T>` | job | Runs `IBackgroundJob<TArgs>` |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `EafQuartzOptions` | `SchedulerName`, `JobStoreType`, `ConnectionString`, `InstanceId` | Valid job store type |

## 9. Expected Architecture

### Architectural style

ABP modular infrastructure.

### Layers

```text
src/Eaf.Quartz/
  Quartz/
    EafQuartzModule.cs
    EafQuartzOptions.cs
    EafQuartzJobFactory.cs
    EafQuartzBackgroundJobManager.cs
    EafQuartzJobAdapter.cs
  README.md
  Eaf.Quartz.csproj
test/Eaf.Quartz.Tests/
  EafQuartz_Tests.cs
```

### Allowed dependencies

- `Abp`
- `Quartz`
- `Castle.Windsor`

### Forbidden dependencies

- Hangfire.
- UI frameworks.

## 10. API Contracts

No new HTTP endpoints. The module exposes configuration API:

```csharp
Configuration.Modules.Configure<EafQuartzOptions>(options =>
{
    options.SchedulerName = "EafQuartz";
    options.JobStoreType = EafQuartzJobStoreType.Ram;
});
```

## 11. Application Contracts

### DTO / Input / Output

```csharp
public enum EafQuartzJobStoreType
{
    Ram,
    Ado,
    Mongo
}

public class EafQuartzOptions
{
    public string SchedulerName { get; set; } = "EafQuartz";
    public EafQuartzJobStoreType JobStoreType { get; set; } = EafQuartzJobStoreType.Ram;
    public string ConnectionString { get; set; }
    public string InstanceId { get; set; } = "AUTO";
}
```

## 12. Persistence and Data

### Persisted entities

Quartz handles its own job store tables when `Ado` is selected. No EAF migrations required for RAM store.

### Migration required

Only if ADO.NET job store is used; Quartz provides scripts.

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| Quartz scheduler | Job scheduling | In-process | — | misfire policy |
| Job store | Persistence | ADO.NET / Mongo / RAM | driver default | via store |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Job throws exception | invalid job args | Log error; Quartz retry per misfire policy |
| Invalid cron | bad cron expression | Throw `UserFriendlyException` at schedule time |
| Scheduler already started | double start | Idempotent — no-op or log warning |

## 15. Few-Shot Examples

### Example 1: Enqueue background job

```csharp
public class MyJob : IBackgroundJob<MyJobArgs>, ITransientDependency
{
    public async Task ExecuteAsync(MyJobArgs args)
    {
        // work
    }
}

await _backgroundJobManager.EnqueueAsync(new MyJobArgs { TenantId = 1 });
```

## 16. Non-Functional Requirements

### Performance

- Job scheduling latency < 50 ms.

### Security

- Do not serialize secrets or PII into `JobDataMap`.

### Observability

- Structured logs via `ILogger` for job execution and failures.

## 17. Mandatory Guardrails

- Do not remove Hangfire.
- Do not change ABP background job contracts.
- Do not add UI code.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `EafQuartzBackgroundJobManager` | Enqueue, schedule, delete job |
| `EafQuartzJobFactory` | Creates job from Windsor |

### Integration tests

| Flow | Validation |
|---|---|
| RAM job store | Enqueued job executes within timeout |
| Cron job | Job fires on scheduled time (use short interval) |

### xUnit example

```csharp
public class EafQuartz_Tests : AbpIntegratedTestBase<EafQuartzModule>
{
    [Fact]
    public async Task Dado_JobEnfileirado_Quando_Executar_Entao_Completa()
    {
        await _manager.EnqueueAsync(new MyJobArgs());
        await Task.Delay(1000);
        // assert side effect
    }
}
```

## 19. Acceptance Criteria

- [ ] `Eaf.Quartz` compiles and packs as NuGet.
- [ ] `IBackgroundJobManager` backed by Quartz resolves from DI.
- [ ] Enqueued jobs execute in tests.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Discovery — inspect Hangfire integration and ABP background job API.
2. Design — choose Quartz DI package and job store strategy.
3. Project setup — create `src/Eaf.Quartz/` and `test/Eaf.Quartz.Tests/`.
4. Implementation — module, options, job factory, manager, adapter.
5. Tests — unit and integration tests.
6. Documentation — `README.md` and spec index update.
7. Validation — `dotnet build` and `dotnet test`.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafQuartzModule))]`.
- Switch back to `Eaf.Hangfire` module.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Quartz API differences in v4 | Medium | Low | Pin v3.x in `common.props` |
| Conflicts with Hangfire in same host | High | Medium | Block both modules being loaded together |
| Clustering configuration complexity | Medium | Medium | Document ADO/Mongo setup only for advanced users |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] README.md and index updated.
- [ ] Build and tests pass.
- [ ] PR created with evidence.

## 24. Key Reminder

> Quartz is an alternative scheduler, not a replacement for Hangfire. Keep Hangfire integration untouched.
