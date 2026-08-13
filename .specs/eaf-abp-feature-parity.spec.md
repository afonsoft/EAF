# EAF Middleware — ABP Feature Parity

## Summary

Compare EAF modules with ASP.NET Boilerplate (ABP) and identify modules/strategies that can be incorporated into EAF to broaden storage, cache, ORM, validation, jobs and notification support.

## Motivation

- EAF is built on ASP.NET Boilerplate but has fewer optional modules.
- ABP provides ready-made integrations that reduce manual work on new enterprise projects.
- Keeping relevant parity eases migrations and reduces custom code.

## Module Comparison

| ABP Module | EAF Equivalent | Status | Proposed Action |
|------------|----------------|--------|-----------------|
| `Abp.BlobStoring` / `Azure` / `FileSystem` | Not found | **Missing** | Add `Eaf.BlobStoring.*` for profile pictures, chat attachments and documents |
| `Abp.HtmlSanitizer` | Not found | **Missing** | Create `Eaf.HtmlSanitizer` for chat, email and rich content |
| `Abp.MailKit` | Not found (uses `System.Net.Mail`?) | **Missing/Partial** | `Eaf.MailKit` for templated emails with attachments |
| `Abp.FluentValidation` | Not found | **Missing** | Integrate FluentValidation into DTOs and Application Services |
| `Abp.Dapper` | Not found | **Missing** | `Eaf.Dapper` for complex queries and reports |
| `Abp.RedisCache` / `ProtoBuf` | `Eaf.SqlServerCache`, `Eaf.SqliteCache` | **Partial** | Add Redis distributed-cache provider |
| `Abp.MongoDB` | Not found | **Missing** | `Eaf.MongoDB` as an EF Core alternative |
| `Abp.NHibernate` | Not found | **Missing** | Evaluate relevance (.NET 10 targets EF Core) |
| `Abp.HangFire` / `Quartz` | `Eaf.Middleware.Worker` | **Partial** | Add `Eaf.Hangfire` and `Eaf.Quartz` as worker options |
| `Abp.MemoryDb` | Not found | **Missing** | `Eaf.MemoryDb` for tests and prototyping |
| `Abp.FluentMigrator` | Not found | **Missing** | Alternative to `dotnet ef migrations` in legacy environments |
| `Abp.AspNetCore.SignalR` | `Eaf.Middleware.Web.Core` uses SignalR | **Partial** | Dedicated `Eaf.SignalR` module with hubs and `IOnlineClientManager` |
| `Abp.AspNetCore.OData` | Not found | **Missing** | `Eaf.OData` for admin endpoints |
| `Abp.AspNetCore.OpenIddict` | `Eaf.Middleware.AzureActiveDirectory`, `Ldap` | **Partial** | Modernize auth with native OpenIddict server |

## Implementation Status Update (2026-08)

- **Implemented**: `OrganizationUnitAppService`, `PaymentAppService` with Stripe/PayPal/PagSeguro/MercadoPago, `MassNotificationAppService`, `UserDelegationAppService`, `TenantJoinRequest` flow, `DashboardAppService`.
- **Still missing**: Blob storage, Redis cache, MailKit, SignalR module, OpenIddict, HtmlSanitizer, Dapper, FluentValidation, MongoDB, Quartz, OData.

## Proposed Priority

### High Priority
1. `Eaf.BlobStoring.FileSystem` + `Azure` + `OCI` — file upload is required for chat and profiles.
2. `Eaf.RedisCache` — distributed cache for multi-tenant and container scenarios.
3. `Eaf.MailKit` — transactional emails and campaigns.
4. `Eaf.HtmlSanitizer` — security for chat and dynamic content.

### Medium Priority
5. `Eaf.Dapper` — reports and performance queries.
6. `Eaf.FluentValidation` — more expressive validation.
7. `Eaf.SignalR` — isolated realtime module.
8. `Eaf.Quartz` — alternative scheduling to Hangfire.

### Low Priority / Evaluate
9. `Eaf.MongoDB` — if NoSQL is needed.
10. `Eaf.OData` — if APIs require OData support.
11. `Eaf.FluentMigrator` — if legacy without EF Core exists.

## Migration Plan
1. Create issues/features for each high-priority module.
2. Copy/adapt ABP module structure (`Abp.*`) to `Eaf.*`.
3. Add xUnit tests for each new module.
4. Update `Eaf.sln`, `common.props` and templates.
5. Document usage in README and `AGENTS.md`.

## Impact
- **High**: expands EAF capability.
- **Medium**: increases test and maintenance surface.
- **Medium**: requires architectural decisions on multi-tenancy and cache.

## Risks
- Syncing with upstream ABP can be hard if APIs change.
- Each module needs specific providers (Azure, OCI, Redis, Mongo).
- Complexity and build time grow.

## References
- `/home/ubuntu/repos/abp-aspnetboilerplate/src` — ABP module list
- `/home/ubuntu/repos/EAF/src` — current EAF modules
- <https://aspnetboilerplate.com/Pages/Documents/Module-System>
