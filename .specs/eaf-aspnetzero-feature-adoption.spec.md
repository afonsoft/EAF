# EAF — Adopt ASP.NET Zero Enterprise Features

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Adopt ASP.NET Zero Enterprise Features |
| Product / System | EAF Middleware + Angular Template |
| Module / Bounded Context | Cross-cutting / Enterprise |
| Change type | Feature / Roadmap |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-zero-adoption` |
| Technical owner | Core Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

ASP.NET Zero (commercial) ships enterprise features beyond open-source ABP. EAF has already closed several gaps but is missing subscription lifecycle, SMS, MailKit, Blob, Redis, SignalR module, and push notifications.

### Objective

Identify high-value ASP.NET Zero features, implement the missing ones as open-source EAF modules, and avoid licensing issues by building independently.

### Expected outcome

- Documented comparison of Zero features vs EAF.
- Missing features turned into module SPECs.
- Several new modules implemented.

### Out of scope

- Copying ASP.NET Zero code, names, or assets.
- Metronic 8 visual redesign.

## 2. Agent Role

Senior .NET/ABP architect and engineer. Research public Zero docs, implement from scratch, and verify against EAF source.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

EAF is open-source middleware built on ABP. ASP.NET Zero is the commercial version. The goal is parity for infrastructure features, not visual assets.

### Relevant stack

- .NET 10, ABP 10.5, EF Core 10, Angular 20, PrimeNG 17

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md`

## 5. Task Definition

### Main task

Implement missing ASP.NET Zero-equivalent features as independent EAF modules and Angular pages.

### Subtasks

- Maintain the comparison/gap analysis.
- Implement subscription/payment lifecycle.
- Implement `Eaf.Sms`, `Eaf.MailKit`, `Eaf.BlobStoring`, `Eaf.RedisCache`, `Eaf.SignalR`, `Eaf.PushNotifications`.
- Add Angular admin pages.

### Do not do

- Do not copy Zero code or names.
- Do not implement features already present unless extending them.

## 6. Functional Requirements

### FR-001: Feature comparison

**Description:** Maintain a comparison of ASP.NET Zero vs EAF features with status.

**Acceptance criteria:**

- [ ] Matrix updated in `.specs/eaf-aspnetzero-functional-gap.spec.md`.
- [ ] Each EAF status cites source files.

### FR-002: Missing modules

**Description:** Create and implement the missing modules listed in the gap analysis.

**Acceptance criteria:**

- [ ] Each module has its own SPEC and branch.
- [ ] Unit and integration tests pass.

### FR-003: Angular admin pages

**Description:** Add Angular pages for admin-facing features (e.g. audit logs UI, language management).

**Acceptance criteria:**

- [ ] Pages use PrimeNG components.
- [ ] Permissions enforced.

## 7. Business Rules

### BR-001: Independent implementation

All features must be implemented from public ABP patterns and EAF conventions, not copied from Zero.

### BR-002: Backward compatibility

Existing EAF features (payment gateway, organization units, etc.) must remain functional.

## 8. Domain Modeling

N/A — roadmap SPEC.

## 9. Expected Architecture

ABP modular monolith with new `Eaf.*` modules and Angular admin modules.

## 10. API Contracts

N/A — per-module.

## 11. Application Contracts

N/A — per-module.

## 12. Persistence and Data

N/A — per-module.

## 13. Integrations

N/A — per-module.

## 14. Edge Cases and Error Scenarios

N/A.

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

- Keep comparisons evidence-based and updated.
- Language: en-us.

## 17. Mandatory Guardrails

Do not copy Zero; do not violate licenses; do not break existing features.

## 18. Expected Tests

| Module | Test type |
|---|---|
| Each new module | Unit + Integration |
| Angular admin page | Component + e2e |

## 19. Acceptance Criteria

- [ ] Gap analysis updated.
- [ ] At least one missing module implemented.
- [ ] Tests pass and coverage maintained.

## 20. Implementation Plan

1. Update gap analysis.
2. Prioritize modules.
3. Implement `Eaf.RedisCache`, `Eaf.MailKit`, `Eaf.BlobStoring`.
4. Implement subscription lifecycle.
5. Add Angular pages.

## 21. Rollback Strategy

Disable modules by removing `[DependsOn]` if issues arise.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Accidental license violation | High | Medium | Implement from ABP docs, not Zero |
| Scope creep | High | Medium | Use per-module SPECs |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Gap analysis updated.
- [ ] Modules implemented and tested.

## 24. Key Reminder

> The SPEC is the contract. Build from ABP patterns, not Zero sources.

## Already Implemented in EAF

- `OrganizationUnitAppService` + `admin/organization-units`
- `MassNotificationAppService` + `admin/mass-notifications`
- `UserDelegationAppService` + `admin/user-delegations`
- `TenantJoinRequest` + `admin/tenant-join-requests`
- `DashboardAppService` + `main/dashboard`
- Payment gateway abstraction (`IPaymentGateway`, resolver, Stripe/PayPal/PagSeguro/MercadoPago)
- LDAP / Azure AD modules
- `Eaf.KeyVault`, `Eaf.OpenTelemetry`, `Eaf.Castle.Serilog`, `Eaf.SqlServerCache`, `Eaf.SqliteCache`

## Gaps / Next Steps

1. SMS module (`Eaf.Sms`)
2. MailKit module (`Eaf.MailKit`)
3. BlobStoring module (`Eaf.BlobStoring`)
4. RedisCache module (`Eaf.RedisCache`)
5. SignalR module (`Eaf.SignalR`)
6. Push notifications
7. Subscription lifecycle
8. Tenant impersonation
9. Audit logs UI
10. Language management UI

## References

- <https://aspnetzero.com/Features>
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
