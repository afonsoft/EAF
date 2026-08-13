# EAF — ABP Feature Parity

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | ABP Feature Parity |
| Product / System | EAF Middleware |
| Module / Bounded Context | Core modules and middleware packages |
| Change type | Infrastructure / Roadmap |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-abp-parity` |
| Technical owner | Core Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF is built on top of ASP.NET Boilerplate (ABP) for .NET 10 but does not yet provide open-source middleware modules equivalent to many ABP commercial/open-source modules (BlobStoring, RedisCache, MailKit, SignalR, OpenIddict, etc.). This limits what projects generated from the EAF template can do out of the box.

### Objective

Increase feature parity between EAF and ABP/ASP.NET Zero by implementing the missing middleware modules as independent, well-tested EAF packages, while preserving the existing architecture and backward compatibility.

### Expected outcome

- EAF ships one or more new modules per quarter.
- Each new module follows the `EafModule` pattern, has unit/integration tests, and is listed in `.specs/eaf-specs-index-and-roadmap-2026.md`.
- Existing modules and generated templates continue to compile and pass tests.

### Out of scope

- Frontend redesign or Metronic 8 assets.
- Copying ASP.NET Zero proprietary code.
- Breaking changes to existing public APIs.

## 2. Agent Role

Senior .NET/ABP engineer implementing according to this SPEC. Conservative with architecture, preserves backward compatibility, makes uncertainty explicit, and does not introduce dependencies without justification.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not push directly to remote; do not change public contracts without documenting breaking changes; do not publish NuGet packages automatically.

## 4. Product Context

### Functional context

EAF middleware modules are consumed by the `Templates/Api` project and by external consumers via NuGet. Missing modules force consumers to build their own infrastructure or buy ASP.NET Zero.

### Technical context

EAF uses ABP 10.5, Castle Windsor DI, EF Core 10, and the `EafModule` lifecycle (`PreInitialize`, `Initialize`, `PostInitialize`). Each module is a separate project under `src/`.

### Relevant stack

- C# 14 / .NET 10
- ABP 10.5, EF Core 10, Castle Windsor
- xUnit / Shouldly / NSubstitute / coverlet
- SQL Server / SQLite / PostgreSQL
- Hangfire, OpenTelemetry, Serilog

### Relevant files or directories

```text
/src/Eaf.Middleware.Core
/src/Eaf.Middleware.Application
/src/Eaf.Middleware.Web.Core
/test
/common.props
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`

## 5. Task Definition

### Main task

Track and implement ABP-equivalent middleware modules for EAF to increase parity with ABP Framework and ASP.NET Zero.

### Subtasks

- Map existing EAF modules to ABP/Zero modules.
- Identify gaps and prioritize by impact.
- Implement new modules following the `EafModule` pattern.
- Add Angular admin pages where applicable.
- Add tests and documentation.

### Do not do

- Do not copy code or assets from ASP.NET Zero.
- Do not remove existing modules.
- Do not break public contracts without versioning.

## 6. Functional Requirements

### FR-001: Module list and status

**Description:** Each ABP-equivalent module must have a clear implementation status and gap description in `.specs/`.

**Rules:**

- Status must be one of: `Implemented`, `Partial`, `Not started`.
- Evidence must reference actual source files.

**Acceptance criteria:**

- [ ] The `.specs/eaf-specs-index-and-roadmap-2026.md` table lists every ABP-equivalent module.
- [ ] Each module status is traceable to `src/` or `Templates/`.

## 7. Business Rules

### BR-001: Backward compatibility

New modules must not change existing public APIs, interfaces, or settings unless versioned and documented.

### BR-002: Independent packaging

Each middleware module must be packable as a separate NuGet package.

## 8. Domain Modeling

N/A — this is a roadmap and parity tracking SPEC.

## 9. Expected Architecture

ABP modular monolith. New modules are added under `src/` with their own `.Core`, `.Application`, and `.Web` projects when necessary, following existing EAF conventions.

## 10. API Contracts

N/A — API contracts will be defined per module in follow-up SPECs.

## 11. Application Contracts

N/A — contracts will be defined per module in follow-up SPECs.

## 12. Persistence and Data

N/A — per-module persistence will be specified in follow-up SPECs.

## 13. Integrations

N/A — per-module integrations will be specified in follow-up SPECs.

## 14. Edge Cases and Error Scenarios

N/A.

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

New modules must not degrade startup time or request latency of existing templates.

### Security

Do not expose secrets in configuration examples or logs.

### Maintainability

Each module must have a `README.md` explaining how to enable it in a generated project.

## 17. Mandatory Guardrails

The agent must not invent requirements, must not copy Zero code, must not break public contracts, must not remove tests, and must not publish NuGet packages automatically.

## 18. Expected Tests

| Module | Test type |
|---|---|
| Each new module | Unit + Integration tests |
| Angular page | Component + e2e smoke |

## 19. Acceptance Criteria

- [ ] All ABP-equivalent gaps are documented in `.specs/`.
- [ ] At least one new module is implemented following this SPEC.
- [ ] Existing tests still pass.
- [ ] No test coverage reduction.

## 20. Implementation Plan

1. Inventory existing ABP/Zero modules and EAF status.
2. Prioritize `Eaf.RedisCache`, `Eaf.MailKit`, `Eaf.BlobStoring`, `Eaf.SignalR`.
3. Implement each module with tests and Angular pages where applicable.
4. Update `.specs/eaf-specs-index-and-roadmap-2026.md`.

## 21. Rollback Strategy

If a new module introduces failures, disable it via `[DependsOn]` removal or feature flag and revert the branch.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Over-scoping new modules | High | Medium | Deliver smallest useful module first |
| Breaking ABP DI conventions | High | Low | Follow existing `EafModule` pattern |
| Duplicating ASP.NET Zero features | Medium | Low | Implement from public ABP docs only |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Parity matrix updated.
- [ ] At least one module implemented and tested.
- [ ] Documentation updated.
- [ ] PR created with evidence.

## 24. Key Reminder

> The SPEC is the contract. Do not optimize or expand the scope. In case of ambiguity, stop and propose options.

## ABP / ASP.NET Zero Module Parity Matrix

| ABP / Zero Module | EAF Equivalent | Status (2026-08) | Notes |
|---|---|---|---|
| Blob Storing | `Eaf.BlobStoring` | Not started | File/image upload abstraction |
| Redis Cache | `Eaf.RedisCache` | Not started | Distributed cache provider |
| MailKit | `Eaf.MailKit` | Not started | Rich email templates |
| SignalR Module | `Eaf.SignalR` | Not started | Real-time notifications/chat |
| OpenIddict | `Eaf.OpenIddict` | Not started | OAuth2/OIDC server |
| HtmlSanitizer | `Eaf.HtmlSanitizer` | Not started | XSS-safe rich content |
| Dapper | `Eaf.Dapper` | Not started | Complex query support |
| FluentValidation | `Eaf.FluentValidation` | Not started | DTO validation |
| MongoDB | `Eaf.MongoDB` | Not started | NoSQL option |
| Quartz | `Eaf.Quartz` | Not started | Alternative scheduler |
| Background Jobs | Hangfire | Implemented | `MiddlewareWebCoreModule` |
| OpenTelemetry | `Eaf.OpenTelemetry` | Implemented | Exists |
| Key Vault | `Eaf.KeyVault` | Implemented | Exists |
| Serilog | `Eaf.Castle.Serilog` | Implemented | Exists |
| SqlServer/Sqlite Cache | `Eaf.SqlServerCache` / `Eaf.SqliteCache` | Implemented | Exists |
| Organization Units | `OrganizationUnitAppService` | Implemented | UI + backend |
| Mass Notifications | `MassNotificationAppService` | Implemented | UI + backend |
| User Delegation | `UserDelegationAppService` | Implemented | UI + backend |
| Tenant Join Requests | `TenantJoinRequest` flow | Implemented | UI + backend |
| Dashboard | `DashboardAppService` | Implemented | UI + backend |
| Payment Gateway | `PaymentAppService` | Implemented | Stripe/PayPal/PagSeguro/MercadoPago |

## References

- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`
- ABP docs: <https://abp.io/docs/latest/modules/index>
