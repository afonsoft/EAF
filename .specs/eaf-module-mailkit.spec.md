# EAF — Eaf.MailKit Module

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.MailKit Module |
| Product / System | EAF Middleware |
| Module / Bounded Context | Email / Notifications |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-mailkit` |
| Technical owner | Core Team |
| Status | Completed |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF already depends on `Abp.MailKit` through `Eaf.Middleware.Core` and `Eaf.Middleware.Worker`, but there is no dedicated `Eaf.MailKit` module that centralizes email settings, template management, and MailKit-specific configuration for EAF consumers.

### Objective

Create `Eaf.MailKit` as a reusable middleware module that wraps and extends ABP's email sending infrastructure with EAF conventions (settings, templates, retry, observability).

### Expected outcome

- `src/Eaf.MailKit/` project.
- `EafMailKitModule`, `EafMailKitSmtpBuilder`, `EafMailKitEmailSender`.
- Email template engine and settings provider.
- Unit/integration tests and README.md.

### Out of scope

- UI for email templates (covered by `eaf-angular-remaining-modernization-features.spec.md`).
- SMS or push notifications (separate specs).

## 2. Agent Role

Senior .NET/ABP engineer. Reuse `Abp.MailKit` contracts; add EAF-specific configuration and templates.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not change `IEmailSender` ABP contract; do not push NuGet packages.

## 4. Product Context

### Functional context

Email is used for registration confirmation, password reset, 2FA, mass notifications, and background jobs. ABP's `IEmailSender` is already consumed; this module makes EAF email configuration explicit and reusable.

### Technical context

- `Eaf.Middleware.Core` references `Abp.MailKit`.
- `Eaf.Middleware.Worker` uses email in background jobs.
- ABP exposes `IEmailSender`, `ISmtpEmailSenderConfiguration`, `IMailKitSmtpBuilder`.

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5
- `MailKit` / `MimeKit`
- `Abp.MailKit`
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.Middleware.Core/
src/Eaf.Middleware.Worker/
src/Eaf.Castle.Serilog/
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Core/EafMiddlewareCoreModule.cs`
- `src/Eaf.Middleware.Worker/EafMiddlewareWorkerModule.cs`
- `common.props`

## 5. Task Definition

### Main task

Create `Eaf.MailKit` module with EAF conventions.

### Subtasks

1. Create project and module.
2. Implement `EafMailKitSmtpBuilder` with safe defaults and SSL/TLS options.
3. Implement `EafMailKitEmailSender` implementing `IEmailSender`.
4. Add email template abstraction (`IEmailTemplateManager`, `EmailTemplate`).
5. Add settings provider for EAF-specific keys.
6. Add unit/integration tests.
7. Add README.md.

### Do not do

- Do not replace ABP `IEmailSender` contract.
- Do not add UI code.

## 6. Functional Requirements

### FR-001: SMTP email sending

**Description:** Send emails via `IEmailSender` using MailKit.

**Rules:**

- Read settings from ABP `ISettingManager`.
- Support HTML and plain text bodies.
- Support attachments via `MimeKit`.

**Inputs:**

| Field | Type | Required | Rule |
|---|---|---|---|
| `to` | `string` or `List<string>` | yes | Valid email(s) |
| `subject` | `string` | yes | Non-empty |
| `body` | `string` | yes | HTML or plain text |
| `isBodyHtml` | `bool` | no | default `true` |
| `attachments` | `List<Attachment>` | no | Optional |

**Outputs:**

| Field | Type | Description |
|---|---|---|
| `SendAsync` | `Task` | Sends email asynchronously |

**Acceptance criteria:**

- [ ] `IEmailSender.SendAsync` delivers via MailKit.
- [ ] Settings are read from `ISettingManager`.
- [ ] HTML emails render correctly.

### FR-002: Email templates

**Description:** Allow named email templates with placeholders.

**Rules:**

- Templates stored as embedded resources or database rows.
- Placeholders replaced via model dictionary.
- Tenant-aware fallback to default template.

**Acceptance criteria:**

- [ ] `IEmailTemplateManager.GetAsync("Welcome", tenantId)` returns rendered body.
- [ ] Unknown placeholders are left empty or logged.

### FR-003: Retry and observability

**Description:** Retry transient SMTP failures and emit logs/traces.

**Rules:**

- Retry up to 3 times with exponential backoff.
- Log each attempt without exposing credentials.
- Emit OpenTelemetry spans when `Eaf.OpenTelemetry` is referenced.

**Acceptance criteria:**

- [ ] Transient failures retry.
- [ ] No credentials in logs.

## 7. Business Rules

### BR-001: Backward compatibility

Existing `IEmailSender` consumers must continue to work unchanged.

### BR-002: No credential exposure

Connection strings and passwords must never be logged.

### BR-003: Tenant-aware templates

Templates can be overridden per tenant.

## 8. Domain Modeling

### Bounded Context

Email / Notifications

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `EmailTemplate` | `Id` (int or GUID) | Stores template name, subject, body, tenant id |

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `EmailAddress` | `Address`, `DisplayName` | Valid email format |

### Domain Events

- `EmailSentEvent` — after successful send.
- `EmailFailedEvent` — after retries exhausted.

## 9. Expected Architecture

```text
src/Eaf.MailKit/
  EafMailKitModule.cs
  EafMailKitEmailSender.cs
  EafMailKitSmtpBuilder.cs
  EmailTemplateManager.cs
  IEmailTemplateManager.cs
  Domain/
    EmailTemplate.cs
  Configuration/
    EafMailKitConfiguration.cs
test/Eaf.MailKit.Tests/
```

## 10. API Contracts

No new HTTP endpoints. Programmatic API:

```csharp
var emailSender = Resolve<IEmailSender>();
await emailSender.SendAsync(
    to: "user@example.com",
    subject: "Welcome",
    body: "<h1>Welcome</h1>",
    isBodyHtml: true
);
```

## 11. Application Contracts

```csharp
public interface IEmailTemplateManager
{
    Task<string> GetTemplateAsync(string name, int? tenantId = null);
    Task<string> RenderAsync(string name, object model, int? tenantId = null);
}
```

## 12. Persistence and Data

### Persisted entities

| Table | Purpose |
|---|---|
| `EafEmailTemplates` | Stores email templates per tenant |

### Migration required

Yes — create `EafEmailTemplates` table.

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `IX_EafEmailTemplates_Name_TenantId` | `Name`, `TenantId` | Fast lookup by name and tenant |

### Compatibility

- [ ] New table does not conflict with ABP `AbpEmailTemplates` if any.
- [ ] Migration is reversible.

## 13. Integrations

### External services

| Service | Data sent | Data received | Security |
|---|---|---|---|
| SMTP server | email envelope + body | SMTP status | TLS/STARTTLS, no logged credentials |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Invalid email address | malformed `to` | Throw `UserFriendlyException` with validation message |
| SMTP unavailable | timeout | Retry 3 times then log failure; return gracefully for background jobs |
| Missing template | unknown name | Fallback to default or throw `UserFriendlyException` |
| Empty body | empty string | Log warning and do not send |

## 15. Few-Shot Examples

### Example 1: Send with template

```csharp
var templateManager = Resolve<IEmailTemplateManager>();
var body = await templateManager.RenderAsync("Welcome", new { Name = "Alice" });
await _emailSender.SendAsync("alice@example.com", "Welcome", body, isBodyHtml: true);
```

### Example 2: Missing template

```csharp
await templateManager.RenderAsync("DoesNotExist", null);
```

**Expected output:** `UserFriendlyException` with "Template not found".

## 16. Non-Functional Requirements

### Performance

- Email send must be asynchronous; sync overload optional.
- Template rendering < 10 ms.

### Security

- No secrets in logs.
- Validate email addresses.
- HTML bodies are allowed but must not be used to render untrusted content without sanitization.

### Observability

- Logs: attempts, failures, retries.
- OpenTelemetry spans for SMTP calls.

### Maintainability

- README.md with settings keys and template examples.

## 17. Mandatory Guardrails

- Do not expose SMTP credentials.
- Do not change `IEmailSender` ABP interface.
- Do not add UI code.
- Stop and ask if MailKit license changes.

## 18. Expected Tests

### Unit tests

| Class | Scenarios |
|---|---|
| `EafMailKitEmailSender` | Send async, sync, retry logic |
| `EafMailKitSmtpBuilder` | SSL/TLS configuration, custom builder |
| `EmailTemplateManager` | Render with model, tenant fallback, missing template |

### Integration tests

| Flow | Validation |
|---|---|
| Send via test SMTP (MailHog/Mailpit) | Email received with expected subject/body |
| Template stored in DB | Render returns correct body |

## 19. Acceptance Criteria

- [ ] `Eaf.MailKit` compiles and packs.
- [ ] `IEmailSender` resolves to `EafMailKitEmailSender` when module is enabled.
- [ ] Integration tests pass with a local SMTP server.
- [ ] README.md documents settings and templates.
- [ ] Existing tests still pass.

## 20. Implementation Plan

1. Create `src/Eaf.MailKit/` and `test/Eaf.MailKit.Tests/`.
2. Implement module, email sender, SMTP builder, template manager.
3. Add migration for `EafEmailTemplates`.
4. Add unit/integration tests.
5. Add README.md and update spec index.
6. Build and test.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafMailKitModule))]` from templates.
- Revert migration if not used in production.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| `Abp.MailKit` version mismatch | High | Low | Pin version in `common.props` |
| Email templates conflict with ABP | Medium | Low | Use `Eaf` prefix and tenant isolation |
| SMTP credentials in settings | High | Medium | Use `ISettingManager` encryption / KeyVault integration |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] Migration and README present.
- [ ] Build and tests pass.

## 24. Key Reminder

> Wrap `Abp.MailKit`, do not replace it. Add EAF-specific configuration, templates, and retry logic.
