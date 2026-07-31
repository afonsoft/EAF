# EAF — ASP.NET Zero Functional Gap Analysis

> Analysis of [ASP.NET Zero Angular documentation](https://docs.aspnetzero.com/aspnet-core-angular/latest/) against the current EAF middleware and `Templates/Angular/Eaf.ProjectName.UI`.
> Scope: **functional features only**. New layouts, themes and visual redesign are explicitly out of scope because Metronic licensing does not allow copying Zero's Metronic 8 assets.

## 1. Already present in EAF (no work required)

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
| Edition management (basic) | `EditionsComponent`, `EditionAppService` | Recently added: CRUD list only, no features/prices integration yet. |
| Rate limiting | `Eaf.Middleware.Core/RateLimiting`, `IRateLimitManager` | Core engine exists; admin UI not present. |

## 2. Functional gaps compared to ASP.NET Zero

### 2.1 Subscription & Payment System (high value)

ASP.NET Zero exposes `IPaymentManager`, `PaymentAppService`, `SubscriptionAppService` and full `SubscriptionPayment`/`SubscriptionPaymentProduct` entities. It provides:

- Tenant subscription page with **payment history** and **invoice generation**.
- **PayPal** and **Stripe** gateway integrations.
- Recurring / proration payments.
- **Subscription expiration workers**: `SubscriptionExpireEmailNotifierWorker`, `SubscriptionExpirationCheckWorker`.
- Edition → tenant assignment with **trial days**, **waiting period after expire** and **fallback edition**.

EAF currently has:

- `SubscribableEdition` and `PaymentPeriodType`/`EditionPaymentType` enums in `Eaf.Middleware.Core` (legacy remnants).
- No `PaymentAppService`, no `SubscriptionAppService`, no tenant subscription UI, no payment gateway integration, no invoice worker.

**Recommended approach:**

1. Add `SubscriptionPayment` and `SubscriptionPaymentProduct` entities.
2. Implement `IPaymentManager` with `CreatePayment`.
3. Add PayPal and Stripe gateway abstractions (`IPaymentGateway` + concrete providers).
4. Create tenant subscription Angular page (`/app/admin/subscription` or `/app/main/subscription`).
5. Add background workers for expiration notifications and tenant deactivation/fallback.

### 2.2 Organization Units (high value, low risk)

ASP.NET Zero has an **Organization Units** page that lets host admins:

- Manage a hierarchical tree of OUs (create, edit, delete, move).
- Add/remove **members** (users) of an OU.
- Add/remove **roles** of an OU (inherited permissions).
- Generic `common-lookup-modal` for selecting users/roles.

EAF already uses ABP Zero's `OrganizationUnit` / `UserOrganizationUnit` / `OrganizationUnitRole` entities in `UserManager`/`RoleManager`, but there is **no AppService and no UI** for OUs.

**Recommended approach:**

1. Add `OrganizationUnitAppService` with tree CRUD, members and roles management.
2. Add Angular page `src/app/admin/organization-units/` with a tree view and members/roles tabs.
3. Reuse existing ABP `IRepository<OrganizationUnit>` and `UserManager`.

### 2.3 Passwordless Login (medium value)

ASP.NET Zero supports passwordless login via email or SMS verification code. It is disabled by default and can be toggled in host settings. A 6-digit single-use code is sent and verified.

EAF has no passwordless flow.

**Recommended approach:**

1. Add `PasswordlessLoginManager` / `PasswordlessCodeCacheItem`.
2. Add settings under `SecuritySettingsEditDto`/`HostSettingsAppService`.
3. Add Angular pages under `/account/passwordless` and `/account/passwordless-verify`.
4. Integrate with existing `UserEmailer` and SMS provider.

### 2.4 QR Login (medium value)

ASP.NET Zero allows a user already authenticated in the MAUI mobile app to scan a QR code on the web login page and log in without credentials.

EAF has no QR login flow.

**Recommended approach:**

1. Add backend QR session generation endpoint (`TokenAuthController`/`QrLoginManager`).
2. Add WebSocket/SignalR channel to poll QR session status.
3. Add Angular login page QR widget.
4. Optional: add QR endpoint for mobile app (separate from this Angular template work).

### 2.5 User Delegation (medium value)

ASP.NET Zero lets a user delegate their account to another user for a limited period. Audit logs keep impersonator info. It is similar to impersonation with expiry.

EAF has impersonation but no time-bounded delegation and no self-service delegation UI.

**Recommended approach:**

1. Add `UserDelegation` entity and `UserDelegationManager`.
2. Extend login/impersonation flow to validate delegation end time.
3. Add Angular modal in the user profile menu for managing delegations.

### 2.6 Mass Notifications (medium value)

ASP.NET Zero has an admin page to send mass notifications to selected users and/or OUs via SMS, email and in-app notification.

EAF has per-user notifications and in-app notifications but no mass-sending UI.

**Recommended approach:**

1. Add `MassNotificationAppService` with target filters (users, OUs, roles, tenants).
2. Add `MassNotification` entity and background job dispatcher.
3. Add Angular page `/app/admin/mass-notifications`.

### 2.7 Host Dashboard & Tenant Dashboard (low-medium value)

ASP.NET Zero ships with a **host dashboard** showing tenant/edition/income statistics and a **tenant dashboard** as starting point. The dashboards are fully implemented with sample widgets and are **customizable**.

EAF `DashboardComponent` is empty (only `AppComponentBase` constructor). There is no customizable dashboard engine.

**Recommended approach:**

1. Add `DashboardAppService` returning stats.
2. Implement host dashboard (`/app/main/dashboard` when host) and tenant dashboard.
3. Optional later phase: widget-based customizable dashboard (separate from this gap list because it is UI-heavy).

### 2.8 Social Account Linking (low-medium value)

ASP.NET Zero allows a logged-in user to connect/disconnect external providers (Google, Facebook, Microsoft, OIDC, WsFederation, Twitter) from their profile.

EAF supports external login at authentication time but does not have a "linked accounts" management page.

**Recommended approach:**

1. Add `UserLogin` management AppService methods to link/unlink external providers.
2. Add Angular page or section in user profile settings.

### 2.9 Setup Page (low value)

ASP.NET Zero has a `/setup` page to create the initial database, apply migrations and configure the app from a web UI.

EAF has no setup page. Usually migrations are run via `dotnet ef` or deploy scripts.

**Recommended approach:**

Consider keeping this as documentation/tooling rather than a runtime page unless product requirements demand it.

## 3. Priority order for the EAF roadmap

Order balances implementation effort and business value:

1. **Organization Units** — ABP entities already exist; mostly AppService + UI.
2. **Host Dashboard / Tenant Dashboard** — Empty dashboard; low risk, high visibility.
3. **Subscription & Payment System** — Large but high business value for SaaS.
4. **Mass Notifications** — Extends existing notification infrastructure.
5. **User Delegation** — Extends existing impersonation.
6. **Passwordless Login** — Security/UX improvement.
7. **Social Account Linking** — Nice-to-have; depends on external auth usage.
8. **QR Login** — Requires mobile app; do after MAUI planning.
9. **Setup Page** — Only if required for zero-config deployment.

## 4. Out of scope

The following ASP.NET Zero capabilities are intentionally excluded because they depend on Metronic 8 or proprietary Zero assets that cannot be reused without a license:

- New Metronic themes / visual design system.
- Quick theme switcher UI beyond existing `ui-customization`.
- MAUI-specific UI layouts.

## 5. References

- ASP.NET Zero Angular docs: https://docs.aspnetzero.com/aspnet-core-angular/latest/
- EAF repository: `afonsoft/EAF`
- Existing EAF docs: `.specs/eaf-aspnetzero-feature-adoption.spec.md` (edition-centric), `.specs/eaf-angular-remaining-modernization-features.spec.md` (layout/theme/PWA gap)
