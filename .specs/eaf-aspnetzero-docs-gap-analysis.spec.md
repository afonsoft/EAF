# EAF — ASP.NET Zero Docs Gap Analysis

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | ASP.NET Zero Docs Gap Analysis |
| Product / System | EAF |
| Module / Bounded Context | Cross-cutting / Documentation Analysis |
| Change type | Roadmap / Analysis |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/aspnetzero-gap-analysis` |
| Technical owner | Core Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

`https://docs.aspnetzero.com/aspnet-core-angular/latest/` describes the ASP.NET Zero feature set built on top of ASP.NET Boilerplate. EAF is an open-source ABP-based middleware platform, and it is not always clear which Zero features are already present, partially present, or completely missing from EAF modules and templates.

### Objective

Map every major Zero documentation feature to the EAF codebase, classify implementation status, and produce separate, implementation-aware specs for the highest-value gaps so the project can close parity gaps incrementally.

### Expected outcome

- A traceable matrix linking Zero docs sections to EAF source files or templates.
- Clear status labels (`Implemented`, `Partial`, `Not started`) with evidence.
- A prioritized backlog and at least one spec per top-priority gap.

### Out of scope

- Copying or reverse-engineering proprietary ASP.NET Zero source code or assets.
- Implementing all identified gaps in a single PR.
- Features that belong exclusively to ASP.NET Zero commercial tooling (Power Tools Visual Studio extension features that are closed-source).

## 2. Agent Role

Technical analyst. Must verify every status against the real EAF codebase (`src/`, `Templates/`, `.specs/`) and external docs. Do not invent implementation status.

## 3. Agent Autonomy Level

**0 — Research / Analysis**

Restrictions: produce documentation and specs only; no implementation code generated from this SPEC.

## 4. Product Context

### Functional context

EAF provides middleware modules (`src/Eaf.*`) and project templates (`Templates/Api`, `Templates/Angular`). ASP.NET Zero extends ABP with enterprise features. This analysis is the input for module and template roadmaps.

### Technical context

- EAF middleware packages: `Eaf.Middleware.*`, `Eaf.KeyVault`, `Eaf.OpenTelemetry`, `Eaf.SqlServerCache`, `Eaf.SqliteCache`, etc.
- Angular template: `Templates/Angular/Eaf.ProjectName.UI/`
- API template: `Templates/Api/`

### Relevant stack

- .NET 10, ABP 10.5, EF Core 10, Castle Windsor
- Angular 20, TypeScript 5.8, PrimeNG 17, `ngx-bootstrap` 12
- SQL Server / SQLite / PostgreSQL, Hangfire, OpenTelemetry, Serilog

### Relevant files or directories

```text
/.specs
/src
/Templates
/docs
```

### Context files the agent must read before implementation

- `.specs/eaf-abp-feature-parity.spec.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`
- `.specs/eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md`

## 5. Task Definition

### Main task

Read `https://docs.aspnetzero.com/aspnet-core-angular/latest/` and create a gap analysis between ASP.NET Zero documented features and EAF.

### Subtasks

1. Extract a feature list from the Zero docs navigation and pages.
2. For each feature, locate the equivalent EAF module, service, Angular page, or template.
3. Assign status and provide a file/reference as evidence.
4. Identify the highest-value gaps for modules and templates.
5. Create or update individual `.specs/*.spec.md` files for the top gaps.

### Do not do

- Do not claim a feature is implemented without a file reference.
- Do not copy Zero snippets verbatim into EAF code.
- Do not produce implementation code before a dedicated SPEC is approved.

## 6. Functional Requirements

### FR-001: Feature inventory

**Description:** Every major Zero docs section must appear in the analysis matrix.

**Rules:**

- Use Zero docs navigation as the source of truth.
- Group related sub-pages into one feature row when appropriate.

**Acceptance criteria:**

- [ ] The matrix covers all top-level Zero docs sections.
- [ ] Each row has a short description and a link to the Zero docs page.

### FR-002: Implementation status mapping

**Description:** Each feature must be mapped to EAF with an evidence-based status.

**Rules:**

- `Implemented` requires an EAF file path or spec reference.
- `Partial` requires a note about what is missing.
- `Not started` means no equivalent EAF artifact was found.

**Acceptance criteria:**

- [ ] At least 80% of rows have an explicit status.
- [ ] Every `Implemented`/`Partial` row cites a file or spec.

### FR-003: Gap prioritization

**Description:** Missing or partial features must be prioritized by value and feasibility for EAF.

**Rules:**

- Prioritize features that align with existing EAF architecture and open-source dependencies.
- Deprioritize closed-source Zero-only tooling.

**Acceptance criteria:**

- [ ] Top 10 gaps are ranked.
- [ ] Each top gap links to a follow-up SPEC.

## 7. Business Rules

### BR-001: Evidence-based statuses

Every implementation status must be traceable to a real file, spec, or documented absence.

### BR-002: Open-source only

Gaps must be solvable using open-source libraries and ABP public APIs; do not rely on Zero proprietary packages.

## 8. Domain Modeling

N/A — this is an analysis SPEC.

## 9. Expected Architecture

N/A — architecture will be defined in per-gap implementation SPECs.

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
| `docs.aspnetzero.com` | HTTP GET | Public documentation pages | HTTPS only |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Zero docs version differs from EAF stack | v15.x docs | Note version and map to conceptual feature, not exact API |
| Feature is commercial-only | Power Tools closed-source extensions | Mark as out-of-scope with explanation |
| Feature already has an open EAF spec | Existing `.specs/` file | Reference the existing spec instead of duplicating |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Accuracy

- Claims must be verifiable by a third party reading the referenced file.

### Maintainability

- The matrix must be stored in a single `.specs/` file and updated as EAF evolves.

### Traceability

- Every gap links to a Zero docs URL and an EAF artifact or new SPEC.

## 17. Mandatory Guardrails

- Do not invent implementation statuses.
- Do not copy proprietary Zero assets.
- Do not propose breaking changes to existing public APIs without a migration note.
- Stop and ask for clarification if a feature is ambiguous.

## 18. Expected Tests

N/A. This SPEC produces documentation and follow-up SPECs.

## 19. Acceptance Criteria

- [ ] Matrix covers all major Zero docs sections.
- [ ] Statuses are evidence-based and cite files/specs.
- [ ] Top 10 gaps are prioritized.
- [ ] At least one new implementation SPEC is created for a top gap.
- [ ] The analysis is reviewed and linked from the spec index.

## 20. Implementation Plan

1. **Discovery** — fetch Zero docs navigation and key pages.
2. **Mapping** — compare each feature against `src/`, `Templates/`, `.specs/`.
3. **Prioritization** — rank gaps by value, feasibility, and architectural fit.
4. **Spec creation** — write implementation SPECs for top gaps.
5. **Review** — update index and cross-reference existing specs.

## 21. Rollback Strategy

If the analysis contains errors, update this SPEC with corrections and add a `Corrections` section; no code rollback is needed.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Zero docs differ from actual Zero source | Medium | Medium | Focus on documented behavior, not implementation details |
| EAF code changes before analysis is used | Medium | High | Date the analysis and re-verify before implementation |
| Misclassifying a feature as missing | Medium | Medium | Require file reference for every status |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Gap matrix is complete and traceable.
- [ ] Follow-up SPECs created for top gaps.
- [ ] Spec index updated.
- [ ] No invented or unverifiable claims.

## 24. Key Reminder

> This SPEC is a research deliverable. Do not generate implementation code. Every claim must be verifiable against `src/`, `Templates/`, or the Zero docs URL.

---

## ASP.NET Zero Docs to EAF Gap Matrix

| Zero Docs Section | Zero Feature | EAF Equivalent | Status (2026-08) | Notes / Evidence |
|---|---|---|---|---|
| Login / Two Factor Authentication | 2FA with email/SMS | `TwoFactorAuthManager` in `Eaf.Middleware.Core` | Partial | Email/SMS templates exist but SMS provider abstraction is missing |
| Login / Social & External Logins | Google, Facebook, etc. | `ExternalAuthManager`, Azure AD, LDAP modules | Implemented | `Eaf.Middleware.Web.Core` / `Eaf.Middleware.AzureActiveDirectory` / `Ldap` |
| Login / Passwordless Login | Magic link / passwordless | Not found | Not started | New module candidate |
| Login / QR Login | QR-code login | Not found | Not started | Low priority |
| Host Settings / Google Authenticator | TOTP 2FA | Partial | Partial | Settings exist; UI modernization pending |
| Dynamic Property System | Runtime entity properties | Not found | Not started | See `eaf-module-dynamic-entity-properties.spec.md` |
| Customizable Dashboard | Per-user dashboard widgets | `DashboardAppService` | Partial | Backend exists; customizable widget engine missing. See `eaf-angular-customizable-dashboard.spec.md` |
| Subscription / PayPal Integration | PayPal gateway | `PayPalGateway` in `Eaf.Middleware.Core` | Implemented | `PaymentAppService` + `IPaymentGateway` |
| Subscription / Stripe Integration | Stripe gateway + recurring | `StripeGateway` | Implemented | Recurring subscriptions not yet end-to-end. See `eaf-module-subscription-payments.spec.md` |
| Subscription / Edition Management | Edition/Feature management | `EditionAppService`, `FeatureAppService` | Implemented | `src/Eaf.Middleware.Core/Editions`, `admin/editions` Angular page |
| Tenant Management | Tenant CRUD, features | `TenantAppService` | Implemented | Existing admin pages |
| Organization Units | OU management | `OrganizationUnitAppService` | Implemented | Admin UI present |
| Role / User Management | RBAC | `RoleAppService`, `UserAppService` | Implemented | Core ABP features |
| Language Management | Add/edit languages/localization | `LanguageAppService` | Partial | Management UI incomplete |
| Audit Logs | Audit log UI / entity history | `AuditLogAppService` | Partial | Backend exists; PrimeNG UI incomplete. See `eaf-angular-audit-logs.spec.md` |
| Active Sessions | Online user sessions | Not found | Not started | Low priority |
| Notifications | Real-time notification system | `NotificationAppService`, SignalR in Web.Core | Partial | Real-time delivery works; desktop push and notification settings UI incomplete |
| Mass Notifications | Broadcast messages | `MassNotificationAppService` + Hangfire job | Implemented | `admin/mass-notifications` |
| Chat | Real-time chat | `ChatAppService` | Partial | Backend exists; UI on legacy Metronic |
| SignalR Integration | Real-time hub backplane | SignalR in `Eaf.Middleware.Web.Core` | Partial | No dedicated `Eaf.SignalR` module / backplane. See `eaf-module-signalr.spec.md` |
| OpenIddict Integration | OAuth2/OIDC server | Not found | Not started | See `eaf-module-openiddict.spec.md` |
| Webhooks | Outgoing webhooks | Not found | Not started | Medium priority |
| Rate Limiting | IP/user throttling | Not found | Not started | Medium priority |
| Health Checks | Monitoring endpoints | `HealthChecks` in `Eaf.Middleware.Web.Core` | Partial | Basic endpoints exist; UI/dashboard not present |
| BLOB Storing | File/image abstraction | Not found | Not started | ABP `IBlobContainer` pattern missing. See `eaf-module-blob-storage.spec.md` |
| Redis Cache | Distributed cache provider | `Eaf.SqlServerCache` / `Eaf.SqliteCache` | Partial | Redis provider missing. See `eaf-module-redis-cache.spec.md` |
| MailKit | Rich email sender | Uses `Abp.MailKit` implicitly | Partial | No dedicated `Eaf.MailKit` module / template settings. See `eaf-module-mailkit.spec.md` |
| SMS / Push | SMS and push providers | Not found | Not started | See `eaf-module-sms-push-notifications.spec.md` |
| UI Testing | Playwright/e2e | Not found | Not started | Quality initiative |
| MAUI Development | Mobile app | Not found | Not started | Out of scope for EAF templates |

## References

- `https://docs.aspnetzero.com/aspnet-core-angular/latest/`
- `.specs/eaf-abp-feature-parity.spec.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`
