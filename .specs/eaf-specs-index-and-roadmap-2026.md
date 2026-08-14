# EAF Specs Index & Roadmap 2026

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | EAF Specs Index & Roadmap 2026 |
| Product / System | EAF |
| Module / Bounded Context | Cross-cutting / Program Management |
| Change type | Roadmap / Index |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/specs-index` |
| Technical owner | Core Team |
| Status | Approved |
| Date | 2026-08-13 |
| Target agent | Any |

## 1. Scope

This document tracks all living specs under `.specs/`, their current implementation status in `afonsoft/EAF`, and the roadmap derived from comparing EAF against **ASP.NET Boilerplate (ABP)** and **ASP.NET Zero**.

All new specs should follow the structure in `.specs/eaf-spec-template.md`.

## 2. Spec Index

| Spec | Domain | Status (2026-08) |
|---|---|---|
| `eaf-spec-template.md` | Spec engineering template | Approved (template) |
| `eaf-abp-feature-parity.spec.md` | Backend modules vs ABP | Updated (en-us); partial parity, modules missing |
| `eaf-aspnetzero-docs-gap-analysis.spec.md` | ASP.NET Zero docs gap analysis | Created (en-us) |
| `eaf-abp-docs-gap-analysis.spec.md` | ABP Boilerplate docs gap analysis | Created (en-us) |
| `eaf-module-redis-cache.spec.md` | `Eaf.RedisCache` module | Implemented |
| `eaf-module-mailkit.spec.md` | `Eaf.MailKit` module | Implemented |
| `eaf-module-blob-storage.spec.md` | `Eaf.BlobStoring` module | Implemented |
| `eaf-module-signalr.spec.md` | `Eaf.SignalR` module | Created (en-us) |
| `eaf-module-subscription-payments.spec.md` | Subscription payment lifecycle | Created (en-us) |
| `eaf-module-dynamic-entity-properties.spec.md` | Dynamic entity properties | Created (en-us) |
| `eaf-module-openiddict.spec.md` | `Eaf.OpenIddict` integration | Created (en-us) |
| `eaf-module-sms-push-notifications.spec.md` | SMS and push notifications | Created (en-us) |
| `eaf-module-html-sanitizer.spec.md` | `Eaf.HtmlSanitizer` module | Created (en-us) |
| `eaf-module-dapper.spec.md` | `Eaf.Dapper` module | Created (en-us) |
| `eaf-module-fluent-validation.spec.md` | `Eaf.FluentValidation` module | Created (en-us) |
| `eaf-module-mongodb.spec.md` | `Eaf.MongoDB` module | Created (en-us) |
| `eaf-module-quartz.spec.md` | `Eaf.Quartz` module | Created (en-us) |
| `eaf-module-webhooks.spec.md` | `Eaf.Webhooks` module | Created (en-us) |
| `eaf-module-rate-limiting.spec.md` | `Eaf.RateLimiting` module | Created (en-us) |
| `eaf-angular-customizable-dashboard.spec.md` | Customizable dashboard UI | Created (en-us) |
| `eaf-angular-audit-logs-ui.spec.md` | Audit logs / entity history UI | Created (en-us) |
| `eaf-angular-accessibility-a11y.spec.md` | Angular a11y/WCAG | Updated (en-us); not started |
| `eaf-angular-dark-mode-theming.spec.md` | Angular dark mode | Updated (en-us); not started |
| `eaf-angular-metronic8-bootstrap5-migration.spec.md` | Angular layout | Updated (en-us); not started |
| `eaf-angular-mobile-responsive-layout.spec.md` | Angular responsive | Updated (en-us); partial |
| `eaf-angular-modern-primeng-components.spec.md` | Angular components | Updated (en-us); partial |
| `eaf-angular-pwa-offline.spec.md` | Angular PWA | Updated (en-us); partial |
| `eaf-angular-remaining-modernization-features.spec.md` | Angular modernization | Updated (en-us); partial |
| `eaf-aspnetzero-feature-adoption.spec.md` | Zero feature adoption | Updated (en-us); several implemented |
| `eaf-aspnetzero-functional-gap.spec.md` | Zero gap analysis | Updated (en-us); several closed |
| `eaf-backend-modularization.spec.md` | Backend modules roadmap | Updated (en-us); partial |
| `eaf-performance-memory-optimization-plan.md` | Performance | Updated (en-us); partial |
| `eaf-session-summary-p70.md` | Session summary | Updated (en-us); done |
| `eaf-template-migration-9.4.1.md` | Migration guide | Updated (en-us); done |
| `eaf-template-migration-and-update.spec.md` | Migration spec | Updated (en-us); in progress |
| `eaf-next-steps-q3-2026.spec.md` | Q3 2026 roadmap | Created (en-us) |
| `eaf-implementation-plan-q3-2026.spec.md` | Q3 2026 implementation plan | Created (en-us) |
| `eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md` | Cross-project comparison | Created (en-us) |
| `prompt-migracao-eaf-9.4.0-para-9.4.3.md` | Migration prompt | Updated (en-us); done |

## 3. Implementation Status by Area

### 3.1 Backend Modules (`src/`)

Existing modules: 15
- `Eaf.Castle.Serilog`
- `Eaf.KeyVault` / `Eaf.KeyVault.AspNetCore`
- `Eaf.Log4NetServiceBus`
- `Eaf.Middleware.Core`
- `Eaf.Middleware.Application`
- `Eaf.Middleware.AzureActiveDirectory`
- `Eaf.Middleware.Ldap`
- `Eaf.Middleware.Web.Core`
- `Eaf.Middleware.Worker`
- `Eaf.OpenTelemetry`
- `Eaf.SqlServerCache`
- `Eaf.SqliteCache`
- `Eaf.RedisCache`
- `Eaf.BlobStoring`

Implemented since last review:
- `OrganizationUnitAppService` + Angular `admin/organization-units`
- `PaymentAppService`, `IPaymentGateway`, Stripe/PayPal/PagSeguro/MercadoPago gateways + Angular `admin/payments`
- `MassNotificationAppService` + `MassNotificationJob` + Angular `admin/mass-notifications`
- `UserDelegationAppService` + Angular `admin/user-delegations`
- `TenantJoinRequest` flow + Angular `admin/tenant-join-requests`
- `DashboardAppService` + Angular `main/dashboard`

Missing ABP-equivalent modules (with dedicated specs):
- `Eaf.MailKit` — `eaf-module-mailkit.spec.md`
- `Eaf.SignalR` — `eaf-module-signalr.spec.md`
- `Eaf.OpenIddict` — `eaf-module-openiddict.spec.md`
- `Eaf.Notifications.Sms` / `Eaf.Notifications.Push` — `eaf-module-sms-push-notifications.spec.md`
- `Eaf.DynamicEntityProperties` — `eaf-module-dynamic-entity-properties.spec.md`
- Subscription payment lifecycle — `eaf-module-subscription-payments.spec.md`

Other missing modules (specs created):
- `Eaf.HtmlSanitizer` — `eaf-module-html-sanitizer.spec.md`
- `Eaf.Dapper` — `eaf-module-dapper.spec.md`
- `Eaf.FluentValidation` — `eaf-module-fluent-validation.spec.md`
- `Eaf.MongoDB` / `Eaf.Middleware.MongoDB` — `eaf-module-mongodb.spec.md`
- `Eaf.Quartz` — `eaf-module-quartz.spec.md`
- `Eaf.Webhooks` — `eaf-module-webhooks.spec.md`
- `Eaf.RateLimiting` — `eaf-module-rate-limiting.spec.md`

### 3.2 Angular UI (`Templates/Angular/Eaf.ProjectName.UI`)

Already present:
- Angular 20 / TypeScript 5.8
- PrimeNG 17 partially adopted (`p-table`, `p-dialog`, `p-fileUpload`, `p-paginator`, `p-progressbar`)
- Service Worker (`ngsw-config.json`, `ServiceWorkerModule.register`) and `manifest.json` references
- `admin/organization-units`, `admin/mass-notifications`, `admin/payments`, `admin/user-delegations`, `admin/tenant-join-requests`, `admin/editions`, `admin/audit-logs`
- Mobile responsive CSS in `styles.css` (partial)

Still open (with dedicated specs):
- Full dark mode / CSS design tokens — `eaf-angular-dark-mode-theming.spec.md`
- Complete `ngx-bootstrap` → PrimeNG migration — `eaf-angular-modern-primeng-components.spec.md`
- Mobile-first layout — `eaf-angular-mobile-responsive-layout.spec.md`
- Metronic 8 / Bootstrap 5 migration — `eaf-angular-metronic8-bootstrap5-migration.spec.md`
- WCAG 2.1 AA audit — `eaf-angular-accessibility-a11y.spec.md`
- PWA offline queue, push notifications and install prompt — `eaf-angular-pwa-offline.spec.md`
- Customizable dashboard — `eaf-angular-customizable-dashboard.spec.md`
- Audit logs / entity history UI modernization — `eaf-angular-audit-logs-ui.spec.md`

## 4. ABP vs ASP.NET Zero vs EAF Comparison

See `eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md` for the full matrix.

Key takeaways:
- EAF is closest to ABP; many Zero enterprise features are still missing.
- EAF has already closed several Zero gaps (Organization Units, Mass Notifications, User Delegation, Tenant Join Requests, Dashboard, Payment Gateway).
- Largest gaps now have dedicated specs: MailKit, Blob, SignalR, OpenIddict, SMS/Push, Dynamic Properties, Subscription lifecycle, Customizable Dashboard, Audit Logs UI. (`Eaf.RedisCache` implemented.)

## 5. Recommended Next Steps (Q3 2026)

See `eaf-next-steps-q3-2026.spec.md` for the detailed roadmap.

High-level priorities:
1. `Eaf.RedisCache` PoC and distributed cache integration.
2. `Eaf.MailKit` and `Eaf.BlobStoring` modules.
3. `Eaf.SignalR` module with backplane support.
4. Subscription lifecycle on top of existing payment gateways.
5. `Eaf.DynamicEntityProperties` backend + Angular manager.
6. Angular PrimeNG completion, Bootstrap 5/Metronic 8 layout spike, dark mode design tokens.
7. PWA offline MVP and push notifications.
8. `Eaf.OpenIddict` OAuth2/OIDC server.
9. SMS and Web Push notification channels.
10. `Eaf.Webhooks` outgoing webhooks.
11. `Eaf.HtmlSanitizer`, `Eaf.FluentValidation`, `Eaf.RateLimiting`, `Eaf.Dapper` security/validation/query modules.
12. `Eaf.MongoDB` and `Eaf.Quartz` optional backends (future waves).
13. Customizable dashboard and audit logs UI modernization.

See `.specs/eaf-implementation-plan-q3-2026.spec.md` for the sequenced wave plan.

## 6. References

- `.specs/` directory
- `.specs/eaf-spec-template.md`
- `src/Eaf.*` middleware modules
- `Templates/Angular/Eaf.ProjectName.UI`
- `Templates/Api`
- ABP docs: <https://abp.io/docs/latest/modules/index>
- ASP.NET Zero docs: <https://docs.aspnetzero.com/aspnet-core-angular/latest/>
- ABP Boilerplate docs: <https://aspnetboilerplate.com/Pages/Documents>
