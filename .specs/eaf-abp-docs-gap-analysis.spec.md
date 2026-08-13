# EAF — ABP Boilerplate Docs Gap Analysis

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | ABP Boilerplate Docs Gap Analysis |
| Product / System | EAF |
| Module / Bounded Context | Cross-cutting / Documentation Analysis |
| Change type | Roadmap / Analysis |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/abp-gap-analysis` |
| Technical owner | Core Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

`https://aspnetboilerplate.com/Pages/Documents` documents the ASP.NET Boilerplate (ABP) framework features, many of which are implemented in ABP commercial modules. EAF is built on ABP, but it is not clear which documented ABP subsystems already have a dedicated EAF module, which are covered indirectly, and which are missing.

### Objective

Map the ABP documentation topics to EAF middleware modules and templates, identify missing open-source building blocks, and generate agent skills and implementation SPECs for the most reusable gaps.

### Expected outcome

- A matrix of ABP docs topics with their EAF implementation status and evidence.
- A list of recommended agent skills and per-module SPECs to close gaps.
- Cross-references to existing `Eaf.*` modules and `.specs/` files.

### Out of scope

- Copying ABP commercial (vNext/volosoft) module source code.
- Re-implementing ABP core framework features that EAF already consumes through `Abp.*` packages.
- One-shot implementation of all gaps.

## 2. Agent Role

Technical analyst and spec author. Must verify every mapping against the EAF `src/` tree and the ABP public docs. Do not assume a feature is missing without searching the codebase.

## 3. Agent Autonomy Level

**0 — Research / Analysis**

Restrictions: deliver analysis, matrix, and specs; do not generate implementation code.

## 4. Product Context

### Functional context

ABP provides the foundation (DI, UoW, repositories, localization, settings, authorization, etc.). EAF adds middleware modules and project templates. This analysis identifies which ABP-adjacent modules EAF still needs to expose as reusable packages.

### Technical context

- EAF modules live under `src/Eaf.*`.
- EAF tests mirror `src/` under `test/`.
- Templates consume EAF packages from source or NuGet.

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5 / EF Core 10 / Castle Windsor
- Angular 20 / TypeScript 5.8 / PrimeNG 17
- SQL Server, SQLite, PostgreSQL, Hangfire, OpenTelemetry, Serilog

### Relevant files or directories

```text
/src
/test
/Templates
/.specs
/docs
```

### Context files the agent must read before implementation

- `.specs/eaf-abp-feature-parity.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-aspnetzero-docs-gap-analysis.spec.md`

## 5. Task Definition

### Main task

Read `https://aspnetboilerplate.com/Pages/Documents` and create a gap analysis between documented ABP subsystems and EAF modules/templates.

### Subtasks

1. Extract major ABP docs topics (navigation + content pages).
2. Map each topic to an existing EAF module, service, or template artifact.
3. Mark status and provide evidence.
4. Identify reusable gaps that should become `Eaf.*` modules or Angular features.
5. Produce agent skills and per-feature SPECs for the selected gaps.

### Do not do

- Do not duplicate existing EAF specs.
- Do not rely on ABP commercial (vNext) docs unless explicitly linked.
- Do not generate implementation code without a dedicated SPEC.

## 6. Functional Requirements

### FR-001: ABP docs topic inventory

**Description:** The analysis must cover the major ABP docs topics listed in the navigation.

**Rules:**

- Group related sub-pages where appropriate.
- Skip obsolete framework versions unless still relevant to ABP 10.5.

**Acceptance criteria:**

- [ ] Matrix contains at least 30 ABP topics.
- [ ] Each row links to an ABP docs page.

### FR-002: EAF mapping and status

**Description:** Each topic must be mapped to an EAF artifact or marked as missing.

**Rules:**

- `Implemented` requires an EAF file path.
- `Partial` requires a note.
- `Not started` means no equivalent EAF module or template feature.

**Acceptance criteria:**

- [ ] At least 80% of rows have an explicit status.
- [ ] Statuses cite real files or new specs.

### FR-003: Skill and spec recommendations

**Description:** For reusable gaps, recommend an agent skill or a per-module SPEC.

**Rules:**

- Recommend skills when the gap is about a recurring pattern (e.g., creating a new EAF module, modernizing Angular templates).
- Recommend SPECs when the gap is a concrete feature.

**Acceptance criteria:**

- [ ] Top 10 gaps link to a new or existing `.specs/` file or `.claude/skills/` entry.

## 7. Business Rules

### BR-001: Evidence-based mapping

Every mapping must be backed by a path in `src/`, `Templates/`, `.specs/`, or a public ABP docs URL.

### BR-002: Open-source patterns only

Gaps must be closed using open-source ABP patterns, not by copying commercial ABP module code.

## 8. Domain Modeling

N/A — analysis SPEC.

## 9. Expected Architecture

N/A — per-gap architecture will be defined in follow-up SPECs.

## 10. API Contracts

N/A.

## 11. Application Contracts

N/A.

## 12. Persistence and Data

N/A.

## 13. Integrations

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| `aspnetboilerplate.com/Pages/Documents` | HTTP GET | Public documentation pages | HTTPS only |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| ABP docs show a legacy .NET Framework page | OWIN / .NET 4.6.1 | Ignore or note as not applicable to .NET 10 EAF |
| Topic is ABP core, not a module | Unit of Work, Repositories | Mark as `Consumed via Abp` and do not create a new module |
| Topic overlaps with ASP.NET Zero | Zero-specific feature | Reference `eaf-aspnetzero-docs-gap-analysis.spec.md` |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Accuracy

- Every status must be verifiable by a reviewer.

### Maintainability

- Store the matrix in one file and update it quarterly.

### Traceability

- Link every gap to a concrete `.specs/` or `.claude/skills/` artifact.

## 17. Mandatory Guardrails

- Do not invent EAF capabilities.
- Do not propose a new `Eaf.*` module when ABP core already provides the feature.
- Do not copy commercial ABP module code.
- Stop and ask when a topic is ambiguous.

## 18. Expected Tests

N/A. This SPEC produces documentation and follow-up SPECs/skills.

## 19. Acceptance Criteria

- [ ] Matrix covers at least 30 ABP docs topics.
- [ ] Statuses are evidence-based.
- [ ] Top gaps are linked to new or existing specs/skills.
- [ ] The spec index references this analysis.

## 20. Implementation Plan

1. **Discovery** — fetch ABP docs pages and navigation.
2. **Mapping** — compare each topic to EAF `src/`, `Templates/`, `.specs/`.
3. **Selection** — choose top gaps that should become modules, template features, or agent skills.
4. **Spec/skill creation** — write implementation SPECs and update skills.
5. **Review** — update the spec index and cross-reference analyses.

## 21. Rollback Strategy

Update this SPEC with corrections if mapping errors are found; no code rollback needed.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| ABP docs describe vNext/commercial features | Medium | Medium | Skip Volosoft-specific packages; focus on ABP Boilerplate OSS |
| EAF already has a hidden implementation | Medium | Medium | Search `src/` and `Templates/` before marking `Not started` |
| Outdated ABP docs | Low | Medium | Use conceptual mapping and note version differences |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Matrix complete and traceable.
- [ ] Follow-up SPECs and/or skills created for top gaps.
- [ ] Spec index updated.

## 24. Key Reminder

> This SPEC is a research deliverable. Do not generate implementation code. Every claim must be verifiable against `src/`, `Templates/`, or the ABP docs URL.

---

## ABP Docs to EAF Gap Matrix

| ABP Docs Topic | ABP Concept | EAF Equivalent | Status (2026-08) | Notes / Evidence |
|---|---|---|---|---|
| Introduction | N-layer DDD modular architecture | EAF middleware + templates | Implemented | Core design of EAF |
| Module System | `AbpModule` lifecycle | `EafModule` pattern in all `src/Eaf.*` | Implemented | Every module has `Eaf*Module.cs` |
| Dependency Injection | Castle Windsor / Microsoft DI | `IocManager`, `IIocManager` | Implemented | `Eaf.Middleware.Core` |
| Session | `IAbpSession` | `IAbpSession` (ABP) | Consumed via Abp | No dedicated wrapper needed |
| Caching | `ICacheManager` | `EafSqlServerCache`, `EafSqliteCache` | Partial | Redis provider missing. See `eaf-module-redis-cache.spec.md` |
| Unit of Work | `IUnitOfWorkManager` | ABP UoW | Consumed via Abp | No new module needed |
| Repositories | `IRepository<T>` | ABP generic repositories | Consumed via Abp | No new module needed |
| Domain Events | `IEventBus` / `EventBus` | ABP event bus | Consumed via Abp | No new module needed |
| DTOs / Object Mapping | AutoMapper integration | `ApplicationService` base | Consumed via Abp | No new module needed |
| Validating Data Transfer Objects | DataAnnotations | ABP validation | Consumed via Abp | `Eaf.Middleware.Application` uses DTO validation |
| Authorization | Permissions, roles, users | `AuthorizationProvider` | Implemented | ABP core consumed |
| Multi-Tenancy | `IMultiTenant` | `TenantId`, `AbpTenantManager` | Implemented | Core ABP features |
| Entities | `Entity<T>`, `FullAuditedEntity` | EAF domain entities | Implemented | `Eaf.Middleware.Core` |
| Unit of Work | Transaction / UoW | ABP | Consumed via Abp | No new module |
| Feature Management | `IFeatureManager` | `FeatureAppService` | Partial | UI and dynamic features incomplete |
| Setting Management | `ISettingManager` | `ISettingManager` | Consumed via Abp | No new module |
| Notification System | `IRealTimeNotifier` | SignalR in Web.Core | Partial | Dedicated `Eaf.SignalR` module missing. See `eaf-module-signalr.spec.md` |
| Background Jobs | Hangfire / Quartz | `Eaf.Middleware.Worker` | Implemented | Hangfire integration exists |
| Email Sending | `IEmailSender` | Uses `Abp.MailKit` | Partial | No `Eaf.MailKit` module. See `eaf-module-mailkit.spec.md` |
| BLOB Storing | `IBlobContainer` | Not found | Not started | See `eaf-module-blob-storage.spec.md` |
| Dynamic Parameter System | Dynamic entity parameters | Not found | Not started | See `eaf-module-dynamic-entity-properties.spec.md` |
| SignalR Integration | Real-time hubs | Partial in Web.Core | Partial | See `eaf-module-signalr.spec.md` |
| Web API Controllers | Dynamic Web API | ABP dynamic API | Consumed via Abp | No new module |
| Swagger UI | Swashbuckle integration | `Swagger` in Web.Core | Implemented | Existing startup config |
| OData Integration | OData endpoints | Not found | Not started | Low priority |
| EF Core MySQL | MySQL provider | Not found | Not started | Provider package only; low priority |
| EF Core SQLite | SQLite provider | `Eaf.SqliteCache` is cache-only | Partial | Full EF SQLite provider missing |
| EF Core Oracle | Oracle provider | Not found | Not started | Low priority |
| XSRF / CSRF Protection | Anti-forgery | ABP anti-forgery | Consumed via Abp | No new module |
| Embedded Resource Files | Localization / embedded files | `EmbeddedResource` patterns | Consumed via Abp | No new module |
| Object-To-Object Mapping | AutoMapper | ABP AutoMapper module | Consumed via Abp | No new module |
| Caching | `ICacheManager` providers | SQL/SQLite cache implemented; Redis missing | Partial | See `eaf-module-redis-cache.spec.md` |
| Logging | `ILogger`, Serilog | `Eaf.Castle.Serilog` | Implemented | Exists |
| Unit Tests / Testing | xUnit / NUnit | `test/` projects with xUnit + Shouldly + NSubstitute | Implemented | `Eaf.*.Tests` |
| Navigation / Menus | `INavigationProvider` | ABP navigation | Consumed via Abp | No new module |
| Localization | `ILocalizationManager` | ABP localization | Consumed via Abp | Language management UI incomplete. See `eaf-angular-language-management.spec.md` |
| Webhooks | Outgoing webhooks | Not found | Not started | Medium priority |
| Health Checks | `IHealthCheck` | Basic endpoints | Partial | UI/dashboard not present |

## References

- `https://aspnetboilerplate.com/Pages/Documents`
- `.specs/eaf-aspnetzero-docs-gap-analysis.spec.md`
- `.specs/eaf-abp-feature-parity.spec.md`
