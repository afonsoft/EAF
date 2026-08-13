# EAF — ASP.NET Zero Functional Gap Analysis

## Summary

Analysis of [ASP.NET Zero Angular documentation](https://docs.aspnetzero.com/aspnet-core-angular/latest/) against the current EAF middleware and `Templates/Angular/Eaf.ProjectName.UI`. Scope: **functional features only**. Layout, themes and visual redesign are out of scope because Metronic 8 assets cannot be reused without a license.

## 1. Already present in EAF (2026-08)

| Feature | EAF evidence | Notes |
|---|---|---|
| User / Role / Tenant management | `src/app/admin/users`, `roles`, `tenants` | Complete UI + backend. |
| Language / localization management | `src/app/admin/languages` | Backend `LanguageAppService` exists. |
| Host / Tenant settings | `src/app/admin/settings` | Host and tenant settings pages exist. |
| Audit logs & Entity history | `src/app/admin/audit-logs`, `Auditing/EntityChange*` | Fully implemented. |
| Maintenance | `src/app/admin/maintenance/maintenance.component.*` | Cache, logs, Web site logs. |
| Notifications | `NotificationAppService`, notifications components | In-app and SignalR. |
| Webhooks | `WebHooks` services and tests | Subscription, send attempts, events. |
| Chat | `chat-bar.component`, `ChatSignalrService`, `ChatAppService` | Real-time chat. |
| Token-based authentication | `TokenAuthController`, `TokenAuthServiceProxy` | JWT + refresh tokens. |
| Two-factor authentication | `TwoFactorCodeCacheExtensions`, `HostSettingsAppService` TwoFactor settings | Backend ready. |
| Social / external logins | `ExternalAuthConfigurer`, `TokenAuthController` external auth | Google, Azure AD, OIDC, etc. |
| LDAP / Active Directory | `Eaf.Middleware.Ldap` | Exists. |
| Azure Key Vault | `Eaf.KeyVault` | Exists. |
| Background jobs / Hangfire | `MiddlewareWebCoreModule` Hangfire config | Exists. |
| Real-time SignalR | Chat / Notifications | Present. |
| Web API Swagger UI | `Swashbuckle` integration | Present. |
| Visual settings / UI customization | `src/app/admin/ui-customization` | Theme selection exists. |
| Organization Units | `src/Eaf.Middleware.Application/Organizations/OrganizationUnitAppService.cs` + `src/app/admin/organization-units` | **Implemented** after this analysis. |
| Mass Notifications | `MassNotificationAppService` + `src/app/admin/mass-notifications` | **Implemented** after this analysis. |
| User Delegation | `UserDelegationAppService` + `src/app/admin/user-delegations` | **Implemented** after this analysis. |
| Tenant Join Requests | `TenantJoinRequest` + `src/app/admin/tenant-join-requests` | **Implemented** after this analysis. |
| Dashboard | `DashboardAppService` + `src/app/main/dashboard` | **Implemented** after this analysis. |
| Payment gateway abstraction | `IPaymentGateway`, `PaymentGatewayResolver`, `PaymentAppService`, Stripe/PayPal/PagSeguro/MercadoPago + `src/app/admin/payments` | **Implemented** after this analysis. |
| Edition management | `EditionsComponent`, `EditionAppService` | CRUD list exists; no feature/price integration yet. |
| Rate limiting | `Eaf.Middleware.Core/RateLimiting`, `IRateLimitManager` | Core engine exists; admin UI not present. |

## 2. Functional gaps compared to ASP.NET Zero (2026-08)

### 2.1 Subscription & Payment System (high value)

ASP.NET Zero exposes a full subscription lifecycle (`IPaymentManager`, `SubscriptionAppService`, `SubscriptionPayment`/`SubscriptionPaymentProduct`, invoice generation, recurring/proration, expiration workers, edition → tenant assignment with trial days and fallback edition).

EAF currently has:

- `SubscribableEdition`, `PaymentPeriodType`/`EditionPaymentType` enums as legacy remnants.
- **Payment gateway abstraction** implemented (`IPaymentGateway` + Stripe/PayPal/PagSeguro/MercadoPago).
- **No** `SubscriptionAppService`, tenant subscription UI, invoice worker, recurring payments or fallback edition logic.

**Recommended approach:**

1. Add `SubscriptionPayment` and `SubscriptionPaymentProduct` entities.
2. Build `SubscriptionAppService` on top of `PaymentAppService`.
3. Create tenant subscription Angular page (`/app/main/subscription` or `/app/admin/subscription`).
4. Add background workers for expiration notifications and tenant deactivation/fallback.

### 2.2 SMS Provider (medium value)

ASP.NET Zero supports passwordless login and mass notifications by SMS.

EAF has no SMS module.

**Recommended approach:** add `Eaf.Sms` module with Twilio / AWS SNS providers and an `ISmsSender` interface.

### 2.3 MailKit Module (medium value)

ASP.NET Zero has rich email templates and providers.

EAF has basic email but no `MailKit` module.

**Recommended approach:** add `Eaf.MailKit` module with templates, SendGrid / Mailgun providers and attachment support.

### 2.4 Blob Storage (medium value)

ASP.NET Zero has file management and tenant assets.

EAF has no `BlobStoring` module.

**Recommended approach:** add `Eaf.BlobStoring` with Azure Blob, AWS S3, FileSystem and Database providers.

### 2.5 Redis Cache (medium value)

ASP.NET Zero supports distributed Redis cache out of the box.

EAF has `Eaf.SqlServerCache` and `Eaf.SqliteCache` but no Redis module.

**Recommended approach:** add `Eaf.RedisCache` implementing `ICacheManager`.

### 2.6 SignalR Module (medium value)

ASP.NET Zero has dedicated real-time notifications and chat backends.

EAF has a `ChatHub` but no formal `Eaf.SignalR` middleware module.

**Recommended approach:** create `Eaf.SignalR` / `Eaf.SignalR.AspNetCore` module with notification and chat backplane.

### 2.7 Push Notifications (medium value)

ASP.NET Zero supports push for mobile apps.

EAF PWA has Service Worker configured but no push logic.

**Recommended approach:** add `Eaf.PushNotifications` (Web Push VAPID) and PWA integration.

### 2.8 Passwordless Login (low-medium value)

ASP.NET Zero supports passwordless login via email/SMS code.

EAF has no passwordless flow.

**Recommended approach:** add `PasswordlessLoginManager` and UI pages.

### 2.9 QR Login (low-medium value)

ASP.NET Zero allows QR login from the mobile app.

EAF has no QR login.

**Recommended approach:** deferred until mobile app planning.

### 2.10 Tenant Impersonation (low-medium value)

ASP.NET Zero allows host admins to impersonate a tenant user.

EAF has user delegation but no tenant impersonation.

**Recommended approach:** extend `UserDelegation`/`PermissionChecker` for cross-tenant support.

### 2.11 Social Account Linking (low value)

ASP.NET Zero lets users connect/disconnect external providers from their profile.

EAF supports external login at authentication time but no linked accounts page.

**Recommended approach:** add `UserLogin` management AppService and profile section.

### 2.12 Setup Page (low value)

ASP.NET Zero has a `/setup` page for initial DB creation.

EAF has no setup page; migrations are run via CLI/deploy scripts.

**Recommended approach:** keep as documentation/tooling unless product requires it.

## 3. Priority Order for the EAF Roadmap

Order balances implementation effort and business value:

1. **Redis Cache** — foundational for scale; aligns with ABP `ICacheManager`.
2. **MailKit Module** — improves email deliverability and templates.
3. **Blob Storage** — enables file uploads and tenant assets.
4. **SignalR Module** — formalizes real-time backend.
5. **Subscription & Payment System** — high business value for SaaS; builds on existing payment gateways.
6. **SMS Provider** — enables passwordless and mass notifications.
7. **Push Notifications** — depends on SignalR/PWA.
8. **Passwordless Login** — security/UX improvement.
9. **Tenant Impersonation** — support scenarios.
10. **Social Account Linking** — nice-to-have.
11. **QR Login** — deferred until mobile app.
12. **Setup Page** — only if required for zero-config deployment.

## 4. Out of Scope

The following ASP.NET Zero capabilities are intentionally excluded because they depend on Metronic 8 or proprietary Zero assets:

- New Metronic themes / visual design system.
- Quick theme switcher UI beyond existing `ui-customization`.
- MAUI-specific UI layouts.

## 5. Implementation Status (2026-08)

Several ASP.NET Zero features (Organization Units, Mass Notifications, User Delegation, Tenant Join Requests, Dashboard, Payment Gateway) have been implemented since the original gap analysis. Remaining gaps are concentrated in infrastructure modules (Redis, Blob, MailKit, SignalR, SMS, Push) and subscription lifecycle.

## 6. References

- ASP.NET Zero Angular docs: <https://docs.aspnetzero.com/aspnet-core-angular/latest/>
- EAF repository: `afonsoft/EAF`
- `.specs/eaf-aspnetzero-feature-adoption.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
