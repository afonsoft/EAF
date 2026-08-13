# EAF Backend — New Middleware Modules

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Backend Modularization |
| Product / System | EAF Middleware |
| Module / Bounded Context | Core modules |
| Change type | Feature / Infrastructure |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-backend-modules` |
| Technical owner | Core Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF currently lacks reusable middleware modules for common enterprise infrastructure concerns such as distributed caching, blob storage, email, real-time messaging, and OIDC.

### Objective

Create new EAF middleware modules following the existing `EafModule` pattern so generated projects can opt-in to enterprise features without vendor lock-in.

### Expected outcome

- New modules are published as NuGet packages.
- Each module has unit and integration tests.
- Angular admin pages are provided where applicable.

### Out of scope

- Frontend redesign.
- Breaking changes to existing modules.

## 2. Agent Role

Senior .NET/ABP engineer. Conservative, backward-compatible, test-driven.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

EAF middleware modules live under `src/`. They are consumed by `Templates/Api` and by NuGet consumers.

### Relevant stack

- .NET 10, ABP 10.5, Castle Windsor, EF Core 10
- xUnit, Shouldly, NSubstitute, coverlet

### Relevant files or directories

```text
/src/Eaf.Middleware.Core
/src/Eaf.Middleware.Web.Core
/src/Eaf.Middleware.Application
/test
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-abp-feature-parity.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`

## 5. Task Definition

### Main task

Implement the following new EAF middleware modules: `Eaf.RedisCache`, `Eaf.MailKit`, `Eaf.BlobStoring`, `Eaf.SignalR`, `Eaf.Sms`, `Eaf.PushNotifications`.

### Subtasks

- Define module interfaces and options.
- Implement providers (Redis, FileSystem, Azure, Twilio, etc.).
- Add DI registration and ABP module classes.
- Add tests and template integration.

### Do not do

- Do not copy Zero code.
- Do not force consumers to enable the modules.

## 6. Functional Requirements

### FR-001: Redis cache module

**Description:** `Eaf.RedisCache` must implement ABP `ICacheManager` as a distributed cache provider.

**Acceptance criteria:**

- [ ] `IRedisCacheManager` interface defined.
- [ ] Castle Windsor registration works via `[DependsOn]`.
- [ ] Integration tests pass with Testcontainers Redis or StackExchange.Redis mock.

### FR-002: MailKit module

**Description:** `Eaf.MailKit` must provide rich email sending with Razor/Scriban templates and multiple providers.

**Acceptance criteria:**

- [ ] `IMailKitEmailSender` interface defined.
- [ ] SendGrid and Mailgun providers supported.
- [ ] Template engine supports localization.

### FR-003: Blob storing module

**Description:** `Eaf.BlobStoring` must abstract file storage with Azure Blob, AWS S3, FileSystem, and Database providers.

**Acceptance criteria:**

- [ ] `IBlobContainer` and `IBlobProvider` abstractions.
- [ ] Multi-tenant container isolation.
- [ ] Stream-based upload/download APIs.

### FR-004: SignalR module

**Description:** `Eaf.SignalR` must formalize real-time notifications and chat with backplane support.

**Acceptance criteria:**

- [ ] `EafSignalRModule` registers hubs and services.
- [ ] Backplane abstraction for Redis or Azure SignalR Service.
- [ ] Cross-tenant chat follows existing `FriendshipState` rules.

### FR-005: SMS module

**Description:** `Eaf.Sms` must provide `ISmsSender` with Twilio and AWS SNS providers.

**Acceptance criteria:**

- [ ] Interface and providers implemented.
- [ ] Template support for OTP and notifications.

### FR-006: Push notifications module

**Description:** `Eaf.PushNotifications` must support Web Push VAPID for the Angular PWA.

**Acceptance criteria:**

- [ ] `IPushNotificationService` interface.
- [ ] VAPID keys configurable.
- [ ] Service Worker integration in Angular.

## 7. Business Rules

### BR-001: Opt-in modules

Every new module must be optional and not loaded by default in generated templates.

### BR-002: Provider abstraction

Concrete cloud providers must be replaceable by mock providers for tests.

## 8. Domain Modeling

N/A — this is a module catalog SPEC.

## 9. Expected Architecture

ABP modular monolith. Each module follows `EafModule` lifecycle with `.Core`, `.Application`, and optional `.Web` projects.

## 10. API Contracts

N/A — per-module contracts in follow-up SPECs.

## 11. Application Contracts

N/A — per-module contracts in follow-up SPECs.

## 12. Persistence and Data

N/A — per-module persistence in follow-up SPECs.

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| Redis | Distributed cache | TCP | 5000ms | Yes |
| SMTP/SendGrid/Mailgun | Email | SMTP/REST | 30000ms | Yes |
| Azure Blob / S3 | File storage | REST | 30000ms | Yes |
| Twilio / SNS | SMS | REST | 15000ms | Yes |
| VAPID push service | Web push | HTTPS | 15000ms | No |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Redis unavailable | Connection failure | Log warning, fallback to in-memory cache if configured |
| Blob provider not configured | Missing connection string | Throw `UserFriendlyException` at startup |
| SMS provider returns 429 | Rate limit | Retry with exponential backoff |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

- Cache operations < 5ms p95 local, < 20ms p95 network.

### Security

- No secrets logged; use `Eaf.KeyVault` for connection strings.

### Observability

- Add OpenTelemetry metrics and logs.

### Maintainability

- Each module has its own `README.md` and sample `appsettings` section.

## 17. Mandatory Guardrails

Do not invent requirements; do not copy Zero; preserve backward compatibility; do not publish packages automatically.

## 18. Expected Tests

| Module | Test type |
|---|---|
| `Eaf.RedisCache` | Unit + Integration (Testcontainers) |
| `Eaf.MailKit` | Unit + integration with local SMTP mock |
| `Eaf.BlobStoring` | Unit + integration with FileSystem provider |
| `Eaf.SignalR` | Integration with in-memory backplane |
| `Eaf.Sms` | Unit with mocked provider |
| `Eaf.PushNotifications` | Unit with VAPID keys fixture |

## 19. Acceptance Criteria

- [ ] All modules listed have interfaces and at least one provider.
- [ ] Tests pass and coverage does not decrease.
- [ ] Modules are documented in `.specs/`.

## 20. Implementation Plan

1. Implement `Eaf.RedisCache`.
2. Implement `Eaf.BlobStoring` (FileSystem + Azure).
3. Implement `Eaf.MailKit`.
4. Implement `Eaf.SignalR`.
5. Implement `Eaf.Sms` and `Eaf.PushNotifications`.
6. Update templates and documentation.

## 21. Rollback Strategy

Disable modules by removing `[DependsOn]` and revert the branch.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Module dependencies on cloud SDKs | Medium | Medium | Make SDKs optional provider packages |
| Multi-tenancy leaks in cache/blob | High | Medium | Tenant-scoped keys and containers |
| Integration tests require cloud accounts | Medium | High | Use emulators / mocks in CI |

## 23. Definition of Done

- [ ] Module implemented and tested.
- [ ] Documentation and sample config provided.
- [ ] PR created and reviewed.

## 24. Key Reminder

> The SPEC is the contract. Implement the smallest useful provider first.

## Module Catalog

| Module | Priority | Status (2026-08) | Description |
|---|---|---|---|
| `Eaf.BlobStoring` | High | Not started | File/image upload abstraction |
| `Eaf.RedisCache` | High | Not started | Distributed cache provider |
| `Eaf.MailKit` | High | Not started | Rich email templates |
| `Eaf.SignalR` | High | Not started | Real-time notifications/chat |
| `Eaf.OpenIddict` | Medium | Not started | OAuth2/OIDC server |
| `Eaf.HtmlSanitizer` | Medium | Not started | XSS-safe rich content |
| `Eaf.Dapper` | Medium | Not started | Complex query support |
| `Eaf.FluentValidation` | Medium | Not started | Fluent DTO validation |
| `Eaf.MongoDB` | Low | Not started | NoSQL option |
| `Eaf.Quartz` | Low | Not started | Alternative scheduler |
| `Eaf.Sms` | Medium | Not started | SMS provider |
| `Eaf.PushNotifications` | Medium | Not started | Web Push for PWA |

## References

- `.specs/eaf-abp-feature-parity.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`
- ABP module docs: <https://abp.io/docs/latest/modules/index>
