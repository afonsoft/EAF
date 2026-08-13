# EAF Backend — Modularization and New Providers

## Summary

Modularize the EAF backend by introducing optional modules for blob storage, Redis cache, email, realtime and modern authentication, reducing per-project customization.

## Motivation

- EAF projects repeatedly re-implement file upload, cache, email and push notifications.
- ASP.NET Boilerplate already provides equivalent modules that can serve as templates.
- ABP module architecture allows providers to be activated/deactivated via DI.

## Proposed Changes

### 1. Eaf.BlobStoring
- `IBlobContainer<T>` for abstract storage.
- Providers:
  - `Eaf.BlobStoring.FileSystem` — development/tests.
  - `Eaf.BlobStoring.Azure` — Azure Blob Storage.
  - `Eaf.BlobStoring.Oci` — Oracle Cloud Object Storage.
- Use cases: profile pictures, chat attachments, documents.

### 2. Eaf.RedisCache
- `IDistributedCache` provider using `StackExchange.Redis`.
- JSON and fallback support to `Eaf.SqlServerCache`.

### 3. Eaf.MailKit
- `IEmailSender` based on MailKit/MimeKit.
- Razor or Scriban email templates.
- Integration with `Eaf.BlobStoring` for attachments.

### 4. Eaf.SignalR
- Dedicated module with `HubBase<T>` and `IOnlineClientManager` integration.
- Hubs: chat, notifications, presence.
- Redis backplane support for multiple instances.

### 5. Eaf.OpenIddict
- Authentication module based on OpenIddict as an alternative to IdentityServer4/manual JWT.
- OAuth2/OIDC support, clients, scopes and consent.

### 6. Eaf.HtmlSanitizer
- HTML sanitization pipeline for chat, email and rich-text content.
- Integrate with `IHtmlSanitizer` from AngleSharp or HtmlSanitizer.

## Implementation Status Update (2026-08)

- **Implemented**: Payment gateway abstraction (`IPaymentGateway`, `PaymentGatewayResolver`, Stripe/PayPal/PagSeguro/MercadoPago), `MassNotificationJob`, `TenantJoinRequest`, `OrganizationUnitAppService`, `UserDelegationAppService`.
- **Still missing**: `Eaf.BlobStoring`, `Eaf.RedisCache`, `Eaf.MailKit`, `Eaf.SignalR` module, `Eaf.OpenIddict`, `Eaf.HtmlSanitizer`, `Eaf.Dapper`, `Eaf.FluentValidation`.

## Migration Plan
1. Create `Eaf.*` projects for each module, following existing module patterns.
2. Add interfaces, implementations and xUnit tests.
3. Update `Eaf.Middleware.Web.Core` to register providers when present.
4. Document configuration in `appsettings.json`.
5. Create usage templates for Api and Angular projects.

## Impact
- **High**: adds new modules and dependencies.
- **Medium**: increases maintenance surface.
- **High**: reduces customization in new projects.

## Risks
- New NuGet dependencies may conflict with current versions.
- Multi-tenancy requires isolation of cache, blob and connections.
- Integration tests need infrastructure (Redis, email container).

## References
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.BlobStoring*`
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.RedisCache*`
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.MailKit`
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.AspNetCore.SignalR`
- `/home/ubuntu/repos/EAF/src` — current EAF module structure
