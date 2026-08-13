# EAF — ASP.NET Zero Functional Gap Analysis

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | ASP.NET Zero Functional Gap Analysis |
| Product / System | EAF Middleware + Angular Template |
| Module / Bounded Context | Cross-cutting / Analysis |
| Change type | Roadmap / Analysis |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/zero-gap-analysis` |
| Technical owner | Core Team |
| Status | Approved |
| Date | 2026-08-13 |
| Target agent | Any |

## 1. Executive Summary

### Problem

There is no public, evidence-based mapping of ASP.NET Zero functional features against the current EAF implementation, making it hard to decide what to build next.

### Objective

Maintain a living functional gap analysis based on ASP.NET Zero public documentation and the actual EAF codebase.

### Expected outcome

- A documented list of already implemented and missing features.
- A prioritized backlog of missing features.
- Clear evidence from `src/` and `Templates/` for each implemented item.

### Out of scope

- Layout, themes, and visual redesign (Metronic 8 licensing).
- Copying Zero code or assets.

## 2. Agent Role

Analyst / architect. Use public Zero docs and EAF source; do not invent features.

## 3. Agent Autonomy Level

**0 — Research**

## 4. Product Context

EAF is an open-source ABP middleware. The gap analysis compares it to the commercial ASP.NET Zero Angular template.

### Relevant stack

- .NET 10, ABP 10.5, Angular 20

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-aspnetzero-feature-adoption.spec.md`
- `.specs/eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`

## 5. Task Definition

### Main task

Analyze ASP.NET Zero public features and compare them to the current EAF implementation.

### Subtasks

- Inventory implemented features with source evidence.
- List missing features with business value.
- Maintain priority order.

### Do not do

- Do not copy Zero code.
- Do not claim implementation without source evidence.

## 6. Functional Requirements

### FR-001: Implemented features

**Description:** List features already in EAF with evidence.

**Acceptance criteria:**

- [ ] Each implemented feature links to a source file or directory.
- [ ] Status is current.

### FR-002: Missing features

**Description:** List missing features, grouped by area, with priority.

**Acceptance criteria:**

- [ ] Each gap has a recommended approach.
- [ ] Priority is justified by business value and effort.

## 7. Business Rules

### BR-001: Evidence-based

All EAF status claims must cite source files or pages.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

N/A.

## 10. API Contracts

N/A.

## 11. Application Contracts

N/A.

## 12. Persistence and Data

N/A.

## 13. Integrations

N/A.

## 14. Edge Cases and Error Scenarios

N/A.

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

- Update monthly or after each release.
- Language: en-us.

## 17. Mandatory Guardrails

Do not use Zero proprietary information beyond public docs.

## 18. Expected Tests

N/A.

## 19. Acceptance Criteria

- [ ] Implemented list is evidence-based.
- [ ] Gap list is prioritized.
- [ ] Out-of-scope items are explicit.

## 20. Implementation Plan

1. Review Zero public docs.
2. Map to EAF source.
3. Document implemented and missing features.
4. Update priority order.

## 21. Rollback Strategy

N/A.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Zero docs change | Low | Medium | Reference version and update |
| Claims become stale | Medium | High | Update per release |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Evidence cross-checked.
- [ ] Roadmap references this analysis.

## 24. Key Reminder

> The SPEC is the contract. Use public docs and source evidence only.

## Already Implemented in EAF (2026-08)

| Feature | EAF evidence |
|---|---|
| Users/Roles/Tenants | `src/app/admin/users`, `roles`, `tenants` |
| Localization management | `src/app/admin/languages` |
| Host/tenant settings | `src/app/admin/settings` |
| Audit logs & entity history | `src/app/admin/audit-logs`, `Auditing/EntityChange*` |
| Maintenance | `src/app/admin/maintenance` |
| Notifications | `NotificationAppService`, notifications components |
| Webhooks | `WebHooks` services and tests |
| Chat | `chat-bar.component`, `ChatSignalrService`, `ChatAppService` |
| Token-based authentication | `TokenAuthController`, `TokenAuthServiceProxy` |
| Two-factor authentication | `TwoFactorCodeCacheExtensions`, `HostSettingsAppService` |
| Social/external logins | `ExternalAuthConfigurer`, `TokenAuthController` |
| LDAP / Azure AD | `Eaf.Middleware.Ldap`, `Eaf.Middleware.AzureActiveDirectory` |
| Key Vault | `Eaf.KeyVault` |
| Background jobs | Hangfire in `MiddlewareWebCoreModule` |
| SignalR (chat/notifications) | Hubs and services exist |
| Swagger | `Swashbuckle` integration |
| UI customization | `src/app/admin/ui-customization` |
| Organization Units | `OrganizationUnitAppService`, `admin/organization-units` |
| Mass Notifications | `MassNotificationAppService`, `admin/mass-notifications` |
| User Delegation | `UserDelegationAppService`, `admin/user-delegations` |
| Tenant Join Requests | `TenantJoinRequest`, `admin/tenant-join-requests` |
| Dashboard | `DashboardAppService`, `main/dashboard` |
| Payment gateway | `IPaymentGateway`, resolver, gateways, `admin/payments` |

## Functional Gaps (2026-08)

| # | Feature | Priority | Recommended approach |
|---|---|---:|---|
| 1 | Subscription & payment lifecycle | High | `SubscriptionPayment` entities + `SubscriptionAppService` |
| 2 | SMS provider | Medium | `Eaf.Sms` (Twilio / SNS) |
| 3 | MailKit module | Medium | `Eaf.MailKit` |
| 4 | Blob storage | Medium | `Eaf.BlobStoring` |
| 5 | Redis cache | Medium | `Eaf.RedisCache` |
| 6 | SignalR module | Medium | `Eaf.SignalR` |
| 7 | Push notifications | Medium | `Eaf.PushNotifications` |
| 8 | Passwordless login | Low-medium | `PasswordlessLoginManager` |
| 9 | QR login | Low-medium | Deferred until mobile app |
| 10 | Tenant impersonation | Low-medium | Extend `UserDelegation` |
| 11 | Social account linking | Low | Profile settings |
| 12 | Setup page | Low | Documentation/tooling |

## Out of Scope

- Metronic 8 themes / visual design system.
- Quick theme switcher beyond existing `ui-customization`.
- MAUI-specific UI layouts.

## References

- ASP.NET Zero Angular docs: <https://docs.aspnetzero.com/aspnet-core-angular/latest/>
- `.specs/eaf-aspnetzero-feature-adoption.spec.md`
- `.specs/eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md`
