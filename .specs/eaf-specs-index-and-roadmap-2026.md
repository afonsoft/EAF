# EAF Specs Index & Roadmap 2026

## 1. Scope

This document tracks all living specs under `.specs/`, their current implementation status in `afonsoft/EAF`, and the roadmap derived from comparing EAF against **ASP.NET Boilerplate (ABP)** and **ASP.NET Zero**.

## 2. Spec Index

| Spec | Domain | Status (2026-08) |
|---|---|---|
| `eaf-abp-feature-parity.spec.md` | Backend modules vs ABP | Updated (en-us); partial parity, modules missing |
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
| `prompt-migracao-eaf-9.4.0-para-9.4.3.md` | Migration prompt | Updated (en-us); done |
| `eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md` | Cross-project comparison | Created (en-us) |
| `eaf-next-steps-q3-2026.spec.md` | Q3 2026 roadmap | Created (en-us) |

## 3. Implementation Status by Area

### 3.1 Backend Modules (`src/`)

Existing modules: 14
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

Implemented since last review:
- `OrganizationUnitAppService` + Angular `admin/organization-units`
- `PaymentAppService`, `IPaymentGateway`, Stripe/PayPal/PagSeguro/MercadoPago gateways + Angular `admin/payments`
- `MassNotificationAppService` + `MassNotificationJob` + Angular `admin/mass-notifications`
- `UserDelegationAppService` + Angular `admin/user-delegations`
- `TenantJoinRequest` flow + Angular `admin/tenant-join-requests`
- `DashboardAppService` + Angular `main/dashboard`

Missing ABP-equivalent modules:
- `Eaf.BlobStoring.*` — file/image upload abstraction
- `Eaf.RedisCache` — distributed cache provider
- `Eaf.MailKit` — transactional email with templates
- `Eaf.SignalR` — dedicated realtime module
- `Eaf.OpenIddict` — OAuth2/OIDC server
- `Eaf.HtmlSanitizer` — XSS-safe rich content
- `Eaf.Dapper` — complex query support
- `Eaf.FluentValidation` — fluent DTO validation
- `Eaf.MongoDB` / `Eaf.Middleware.MongoDB` — NoSQL option
- `Eaf.Quartz` — alternative scheduler
- `Eaf.Sms` — SMS provider
- `Eaf.PushNotifications` — Web Push for PWA

### 3.2 Angular UI (`Templates/Angular/Eaf.ProjectName.UI`)

Already present:
- Angular 20 / TypeScript 5.8
- PrimeNG 17 partially adopted (`p-table`, `p-dialog`, `p-fileUpload`, `p-paginator`, `p-progressbar`)
- Service Worker (`ngsw-config.json`, `ServiceWorkerModule.register`) and `manifest.json` references
- `admin/organization-units`, `admin/mass-notifications`, `admin/payments`, `admin/user-delegations`, `admin/tenant-join-requests`, `admin/editions`
- Mobile responsive CSS in `styles.css` (partial)

Still open:
- Full dark mode / CSS design tokens
- Complete `ngx-bootstrap` → PrimeNG migration
- Mobile-first layout (off-canvas, bottom nav, touch targets)
- Metronic 8 / Bootstrap 5 migration
- WCAG 2.1 AA audit
- PWA offline queue, push notifications and install prompt

## 4. ABP vs ASP.NET Zero vs EAF Comparison

See `eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md` for the full matrix.

Key takeaways:
- EAF is closest to ABP; many Zero enterprise features are still missing.
- EAF has already closed several Zero gaps (Organization Units, Mass Notifications, User Delegation, Tenant Join Requests, Dashboard, Payment Gateway).
- Largest gaps: subscription lifecycle, Redis, Blob, MailKit, SignalR module, SMS, Push, modern Angular UI (dark mode, Bootstrap 5, PWA offline).

## 5. Recommended Next Steps (Q3 2026)

See `eaf-next-steps-q3-2026.spec.md` for the detailed roadmap.

High-level priorities:
1. `Eaf.RedisCache` PoC and distributed cache integration.
2. `Eaf.MailKit` and `Eaf.BlobStoring` modules.
3. `Eaf.SignalR` module with backplane support.
4. Subscription lifecycle on top of existing payment gateways.
5. Angular PrimeNG completion, Bootstrap 5/Metronic 8 layout spike, dark mode design tokens.
6. PWA offline MVP and push notifications.

## 6. References

- `.specs/` directory
- `src/Eaf.*` middleware modules
- `Templates/Angular/Eaf.ProjectName.UI`
- `Templates/Api`
- ABP docs: <https://abp.io/docs/latest/modules/index>
- ASP.NET Zero docs: <https://docs.aspnetzero.com/aspnet-core-angular/latest/>
