# EAF — Implementation Plan Q3 2026

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | EAF Implementation Plan Q3 2026 |
| Product / System | EAF |
| Module / Bounded Context | Cross-cutting / Program Management |
| Change type | Roadmap / Implementation Plan |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/implementation-plan-q3-2026` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-14 |
| Target agent | Any |

## 1. Executive Summary

### Problem

Multiple `.specs/` files describe individual EAF gaps, but there is no single time-bound implementation plan that sequences backend modules, Angular modernization, subscription payments, and quality improvements while respecting dependencies and team capacity.

### Objective

Provide a consolidated Q3 2026 implementation plan that orders the work, assigns waves/priorities, defines entry/exit criteria, and links each work item to its dedicated SPEC.

### Expected outcome

- A living plan document under `.specs/`.
- Clear execution waves (P0 foundations → P1 core modules → P2 Angular → P3 quality/docs).
- Traceability from each work item to a SPEC and a target milestone.

### Out of scope

- Detailed designs per work item (those live in individual SPECs).
- Work assigned beyond Q3 2026 (move to next quarter roadmap).

## 2. Agent Role

Technical program owner. Synthesize SPECs into an actionable plan. Do not invent new requirements; only schedule existing SPECs.

## 3. Agent Autonomy Level

**0 — Research/Roadmap**

This is a planning document; no code changes are authorized by this SPEC alone.

## 4. Product Context

### Functional context

The EAF middleware and Angular template need to close parity gaps with ABP and ASP.NET Zero while improving quality, observability, and developer experience.

### Technical context

- Existing modules: 14 under `src/`.
- Template stack: .NET 10, ABP 10.5, EF Core 10, Castle Windsor, Angular 20, PrimeNG 17, `ngx-bootstrap` 12.
- Specs folder: `.specs/`.

### Relevant files or directories

```text
.specs/
src/
Templates/Angular/Eaf.ProjectName.UI/
```

### Context files the agent must read before implementation

- `.specs/eaf-specs-index-and-roadmap-2026.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`

## 5. Task Definition

### Main task

Create a consolidated Q3 2026 implementation plan that schedules all pending EAF work.

### Subtasks

- Group pending SPECs into execution waves.
- Define dependencies and prerequisites for each wave.
- Define entry/exit criteria and validation gates.
- Assign target milestones and risk mitigations.

### Do not do

- Do not add new feature requirements not already in a SPEC.
- Do not assign unrealistic dates or dependencies.

## 6. Backlog by Area

### 6.1 Backend Modules

| # | Module | Priority | Spec | Status (2026-08) | Notes |
|---|---|---|---|---|---|
| 1 | `Eaf.RedisCache` | P1 | `eaf-module-redis-cache.spec.md` | Implemented | Drop-in distributed cache; no other dependencies |
| 2 | `Eaf.BlobStoring` | P1 | `eaf-module-blob-storage.spec.md` | Implemented (Azure + AWS S3 generic cloud provider) | Required for file uploads across templates |
| 3 | `Eaf.MailKit` | P1 | `eaf-module-mailkit.spec.md` | Implemented | Rich email templates; prerequisite for some notifications |
| 4 | `Eaf.SignalR` | P1 | `eaf-module-signalr.spec.md` | Implemented | Real-time notifications/chat; separate from `Web.Core` |
| 5 | `Eaf.Webhooks` | P2 | `eaf-module-webhooks.spec.md` | Implemented | Outgoing webhooks with EAF signature and secret protection |
| 6 | `Eaf.FluentValidation` | P2 | `eaf-module-fluent-validation.spec.md` | Implemented | Optional validation provider integrated with ABP pipeline |
| 7 | `Eaf.HtmlSanitizer` | P2 | `eaf-module-html-sanitizer.spec.md` | Implemented | Security primitive for rich content |
| 8 | `Eaf.DynamicEntityProperties` | P2 | `eaf-module-dynamic-entity-properties.spec.md` | Not started | Dynamic entity fields + Angular manager |
| 9 | `Eaf.OpenIddict` | P2 | `eaf-module-openiddict.spec.md` | Not started | OAuth2/OIDC server |
| 10 | `Eaf.Notifications.Sms` / `Push` | P2 | `eaf-module-sms-push-notifications.spec.md` | Not started | SMS and Web Push channels |
| 11 | `Eaf.Dapper` | P3 | `eaf-module-dapper.spec.md` | Spec created | Complex query complement to EF Core |
| 12 | `Eaf.RateLimiting` | P3 | `eaf-module-rate-limiting.spec.md` | Spec created | API throttling/resilience |
| 13 | `Eaf.MongoDB` | P4 | `eaf-module-mongodb.spec.md` | Spec created | Optional NoSQL backend |
| 14 | `Eaf.Quartz` | P4 | `eaf-module-quartz.spec.md` | Spec created | Alternative scheduler to Hangfire |

### 6.2 Subscription Payments

| # | Work item | Priority | Spec | Status | Notes |
|---|---|---|---|---|---|
| 1 | `PaymentManager` + `SubscriptionPaymentProduct` backend | P1 | `eaf-module-subscription-payments.spec.md` | Implemented | Already in `src/Eaf.Middleware.Application` and `Core` |
| 2 | Angular `account/gateway-selection` | P2 | `eaf-module-subscription-payments.spec.md` | Not started | Frontend gateway selection page |
| 3 | Angular `admin/subscriptions` | P2 | `eaf-module-subscription-payments.spec.md` | Not started | Subscription administration UI |
| 4 | Recurring billing reminders | P2 | `eaf-module-subscription-payments.spec.md` | Not started | Hangfire/Quartz job |
| 5 | Webhook handlers for Stripe/PayPal/MercadoPago/PagSeguro | P2 | `eaf-module-subscription-payments.spec.md` | Not started | Depends on `Eaf.Webhooks` |

### 6.3 Angular UI Modernization

| # | Work item | Priority | Spec | Status | Notes |
|---|---|---|---|---|---|
| 1 | Full PrimeNG adoption (remove `ngx-bootstrap`) | P1 | `eaf-angular-modern-primeng-components.spec.md` | Partial | ~49 files still import `ngx-bootstrap` |
| 2 | Dark mode + CSS design tokens | P2 | `eaf-angular-dark-mode-theming.spec.md` | Not started | Use CSS custom properties |
| 3 | Mobile-first responsive layout | P2 | `eaf-angular-mobile-responsive-layout.spec.md` | Partial | CSS exists but incomplete |
| 4 | Bootstrap 5 / Metronic 8 layout spike | P2 | `eaf-angular-metronic8-bootstrap5-migration.spec.md` | Not started | Replace legacy Bootstrap 4/Metronic layout |
| 5 | PWA offline queue + push notifications | P2 | `eaf-angular-pwa-offline.spec.md` | Partial | Service Worker registered; offline queue missing |
| 6 | WCAG 2.1 AA a11y audit | P3 | `eaf-angular-accessibility-a11y.spec.md` | Not started | Axe/Lighthouse a11y checks |
| 7 | Customizable dashboard | P3 | `eaf-angular-customizable-dashboard.spec.md` | Not started | Drag-drop widgets, persisted layout |
| 8 | Audit logs / entity history UI modernization | P3 | `eaf-angular-audit-logs-ui.spec.md` | Not started | Replace legacy audit log views |

### 6.4 Quality / Performance / Docs

| # | Work item | Priority | Spec / File | Status | Notes |
|---|---|---|---|---|---|
| 1 | OpenTelemetry / Serilog coverage review | P2 | `eaf-performance-memory-optimization-plan.md` | Partial | Ensure all new modules emit metrics/traces |
| 2 | Bundle budgets and lazy loading | P2 | `eaf-angular-remaining-modernization-features.spec.md` | Partial | Add Angular `budgets` and route lazy loading |
| 3 | Spec index and migration guides | P3 | `eaf-specs-index-and-roadmap-2026.md` | Ongoing | Keep index updated |
| 4 | Agent skills for EAF development | P3 | `.claude/skills/` / `.devin/skills/` | Partial | Add skills for common EAF tasks |

## 7. Execution Waves

### Wave 0 — Pre-requisites (Week 1, immediately)

**Goal:** Establish baseline and remove blockers for parallel module work.

| Work item | Owner | Exit criteria |
|---|---|---|
| Update `.specs/` index with all current gaps | Core Team | `eaf-specs-index-and-roadmap-2026.md` references all active SPECs |
| Agree on module naming conventions | Core Team | Conventions documented in `CLAUDE.md` and `.specs/` |
| Verify `dotnet build` and `dotnet test` baseline | Core Team | CI green on `develop` |
| Bootstrap container/Redis/SMTP test infrastructure | Core Team | Testcontainers or docker-compose fixtures ready |

### Wave 1 — Backend Foundations (Weeks 2–5)

**Goal:** Deliver the four highest-value backend modules.

| Order | Module | Rationale |
|---|---|---|
| 1 | `Eaf.RedisCache` | No dependencies; unblocks distributed cache and SignalR backplane testing |
| 2 | `Eaf.BlobStoring` | File upload primitive; other features depend on it |
| 3 | `Eaf.MailKit` | Rich email; notification system depends on it |
| 4 | `Eaf.SignalR` | Real-time notifications; depends on Redis cache for backplane |

**Dependencies:**

- `Eaf.SignalR` depends on `Eaf.RedisCache` for Redis backplane tests.
- `Eaf.MailKit` can be done in parallel with `Eaf.RedisCache` and `Eaf.BlobStoring`.

**Exit criteria:**

- All four modules compile, pack as NuGet, and have ≥ 90% test coverage.
- `Templates/Api` can optionally enable each module with a single configuration call.
- Existing tests still pass.

### Wave 2 — Application Features (Weeks 4–8)

**Goal:** Close subscription payment UI and notification channels.

| Order | Work item | Depends on |
|---|---|---|
| 1 | Angular `account/gateway-selection` and `admin/subscriptions` | Payment backend (done) |
| 2 | `Eaf.Webhooks` | Background jobs (Hangfire/Quartz) |
| 3 | `Eaf.Notifications.Sms` / `Push` | `Eaf.MailKit` for email parity; `Eaf.SignalR` for push |
| 4 | `Eaf.FluentValidation` | None (validation provider) |
| 5 | `Eaf.HtmlSanitizer` | None (security primitive) |

**Exit criteria:**

- Subscription UI flows tested end-to-end with at least one gateway.
- Webhooks delivered and signed in integration tests.
- SMS/Push unit tests with mocked providers.

### Wave 3 — Enterprise & Security Modules (Weeks 6–10)

**Goal:** Add optional enterprise capabilities.

| Order | Module | Depends on |
|---|---|---|
| 1 | `Eaf.DynamicEntityProperties` | EF Core migrations; Angular dynamic forms |
| 2 | `Eaf.OpenIddict` | None (OIDC server) |
| 3 | `Eaf.RateLimiting` | None (middleware) |
| 4 | `Eaf.Dapper` | EF Core `IDbContextProvider` patterns |

**Exit criteria:**

- Modules are opt-in and do not break existing templates.
- Integration tests for `Eaf.OpenIddict` token endpoints pass.
- `Eaf.RateLimiting` returns 429 in integration tests.

### Wave 4 — Angular Modernization (Weeks 8–12)

**Goal:** Modernize the Angular template and remove legacy `ngx-bootstrap`.

| Order | Work item | Depends on |
|---|---|---|
| 1 | Complete `ngx-bootstrap` → PrimeNG migration | None |
| 2 | Dark mode + CSS design tokens | PrimeNG migration |
| 3 | Mobile-first responsive layout | PrimeNG migration |
| 4 | Bootstrap 5 / Metronic 8 layout spike | PrimeNG migration |
| 5 | PWA offline queue + push notifications | `Eaf.SignalR`, `Eaf.Notifications.Push` |
| 6 | Customizable dashboard | None |
| 7 | Audit logs UI modernization | None |
| 8 | WCAG 2.1 AA a11y audit | All UI work above |

**Exit criteria:**

- `ngx-bootstrap` removed from `package.json` and all imports.
- Lighthouse mobile score ≥ 80.
- No a11y critical errors in automated scan.

### Wave 5 — Optional / Future (Weeks 12+)

| Module | Notes |
|---|---|
| `Eaf.MongoDB` | Optional NoSQL backend; low priority |
| `Eaf.Quartz` | Optional Hangfire alternative; low priority |

## 8. Dependencies and Critical Path

```text
Week 1:  Baseline / specs / CI
Week 2:  Eaf.RedisCache  ──┐
Week 3:  Eaf.BlobStoring    │
Week 4:  Eaf.MailKit        │
Week 5:  Eaf.SignalR  <────┘ (uses Redis cache)
Week 6:  Eaf.Webhooks, Eaf.Notifications.Sms/Push
Week 7:  Angular subscription UI
Week 8:  Eaf.DynamicEntityProperties, Eaf.OpenIddict
Week 9:  Eaf.FluentValidation, Eaf.HtmlSanitizer
Week 10: Eaf.RateLimiting, Eaf.Dapper
Week 11-12: Angular PrimeNG completion + dark mode
Week 13:   Angular responsive + PWA
Week 14:   Metronic 8 / Bootstrap 5 spike
Week 15:   Customizable dashboard + audit logs UI
Week 16:   WCAG audit + final QA
```

## 9. Milestones

| Milestone | Target | Deliverables |
|---|---|---|
| M1 — Foundation ready | End of week 1 | CI green, spec index updated, conventions agreed |
| M2 — Core backend modules | End of week 5 | `Eaf.RedisCache`, `Eaf.BlobStoring`, `Eaf.MailKit`, `Eaf.SignalR` merged and tested |
| M3 — Subscriptions & notifications | End of week 8 | Angular gateway/subscription UI, webhooks, SMS/push specs implemented |
| M4 — Enterprise modules | End of week 11 | `Eaf.DynamicEntityProperties`, `Eaf.OpenIddict`, `Eaf.RateLimiting`, `Eaf.Dapper` merged |
| M5 — Angular modernization | End of week 14 | `ngx-bootstrap` removed, dark mode, mobile responsive, PWA offline queue |
| M6 — Final QA & docs | End of week 16 | Lighthouse ≥ 80, a11y audit, docs updated, release candidate |

## 10. Validation Gates

Each wave must pass the following before the next wave starts:

- [ ] `dotnet build EAF.sln --configuration Release` passes.
- [ ] `dotnet test EAF.sln --collect:"XPlat Code Coverage"` passes with coverage ≥ baseline.
- [ ] Angular `npx ng build --configuration=production` passes.
- [ ] Angular `npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox` passes.
- [ ] SonarCloud quality gate passes.
- [ ] No new secrets or connection strings committed.
- [ ] PR reviewed and merged to `develop`.

## 11. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| `ngx-bootstrap` removal takes longer than expected | High | Medium | Parallelize component-by-component; keep fallback imports during migration |
| SignalR/Redis backplane complexity | Medium | Medium | Start with in-memory backplane; Redis as opt-in |
| Test infrastructure (Redis, SMTP, Mongo) unavailable | Medium | Medium | Provide `docker-compose.test.yml` and Testcontainers fixtures |
| OpenIddict scope creep | High | Low | Follow SPEC strictly; defer advanced OIDC flows |
| Coverage drop due to many new modules | Medium | Medium | Enforce ≥ 90% per new module; block merge on drop |

## 12. Definition of Done for the Plan

- [ ] All known gaps are represented in the backlog.
- [ ] Each work item links to a SPEC.
- [ ] Waves are ordered by priority and dependency.
- [ ] Milestones and validation gates are defined.
- [ ] Risks and mitigations are documented.
- [ ] Plan is reviewed and approved.

## 13. References

- `.specs/eaf-specs-index-and-roadmap-2026.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
- `.specs/eaf-module-redis-cache.spec.md`
- `.specs/eaf-module-mailkit.spec.md`
- `.specs/eaf-module-blob-storage.spec.md`
- `.specs/eaf-module-signalr.spec.md`
- `.specs/eaf-module-subscription-payments.spec.md`
- `.specs/eaf-module-dynamic-entity-properties.spec.md`
- `.specs/eaf-module-openiddict.spec.md`
- `.specs/eaf-module-sms-push-notifications.spec.md`
- `.specs/eaf-module-html-sanitizer.spec.md`
- `.specs/eaf-module-dapper.spec.md`
- `.specs/eaf-module-fluent-validation.spec.md`
- `.specs/eaf-module-mongodb.spec.md`
- `.specs/eaf-module-quartz.spec.md`
- `.specs/eaf-module-webhooks.spec.md`
- `.specs/eaf-module-rate-limiting.spec.md`
- `.specs/eaf-angular-dark-mode-theming.spec.md`
- `.specs/eaf-angular-modern-primeng-components.spec.md`
- `.specs/eaf-angular-mobile-responsive-layout.spec.md`
- `.specs/eaf-angular-metronic8-bootstrap5-migration.spec.md`
- `.specs/eaf-angular-accessibility-a11y.spec.md`
- `.specs/eaf-angular-pwa-offline.spec.md`
- `.specs/eaf-angular-customizable-dashboard.spec.md`
- `.specs/eaf-angular-audit-logs-ui.spec.md`
