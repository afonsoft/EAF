# EAF — Adopt ASP.NET Zero Enterprise Features

## Summary

Identify the high-value enterprise features available in **ASP.NET Zero** and implement the ones missing in EAF, focusing on features that add value to the open-source middleware and templates without violating licensing.

## Motivation

- ASP.NET Zero is the paid commercial version of ASP.NET Boilerplate and adds enterprise modules and templates.
- EAF already implements some Zero features (see below); others can be implemented as open-source modules and Angular pages to increase parity.

## Comparison — ASP.NET Zero vs EAF

### Identity / User Management
| Feature | ASP.NET Zero | EAF Status (2026-08) |
|---|---|---|
| Users, Roles, Permissions | Yes | Implemented |
| Organization Units | Yes | **Implemented** (`OrganizationUnitAppService`) |
| User Delegation (impersonation) | Yes | **Implemented** (`UserDelegationAppService`) |
| Mass Notifications | Yes | **Implemented** (`MassNotificationAppService`) |
| Tenant Join Requests / Registration | Yes | **Implemented** (`TenantJoinRequest`) |
| Dashboard | Yes | **Implemented** (`DashboardAppService` + widgets) |
| Two-factor authentication | Yes | Implemented via ASP.NET Identity / ABP |
| LDAP / Azure AD | Yes | **Implemented** (`Eaf.Middleware.Ldap`, `Eaf.Middleware.AzureActiveDirectory`) |
| Payment gateway abstraction | Yes | **Implemented** (`IPaymentGateway`, Stripe, PayPal, PagSeguro, MercadoPago) |
| Subscription / edition management | Yes | **Implemented** (`Editions` + payment gateway) |
| SMS provider | Yes | Not implemented |
| Chat | Yes | Partial (SignalR Hub exists; UI in Angular) |

### Multi-Tenancy
| Feature | ASP.NET Zero | EAF Status (2026-08) |
|---|---|---|
| Tenant isolation (database/host) | Yes | Implemented |
| Tenant registration / join request | Yes | **Implemented** |
| Tenant dashboard | Yes | **Implemented** |
| Tenant-specific settings | Yes | Implemented via ABP `ISettingManager` |
| Tenant impersonation | Yes | Not implemented |

### CMS / Content
| Feature | ASP.NET Zero | EAF Status (2026-08) |
|---|---|---|
| Dynamic forms / survey | Yes | Not implemented |
| File management | Yes | Not implemented (no `BlobStoring` module) |
| Email templates | Yes | Basic (no `MailKit` module) |
| Push notifications | Yes | Not implemented |

### DevOps / Infra
| Feature | ASP.NET Zero | EAF Status (2026-08) |
|---|---|---|
| Hangfire background jobs | Yes | **Implemented** |
| SignalR | Yes | Partial (backend Hub + Angular service exist) |
| OpenTelemetry / logs | Yes | **Implemented** (`Eaf.OpenTelemetry`, `Eaf.Castle.Serilog`) |
| Key Vault integration | Yes | **Implemented** (`Eaf.KeyVault`) |
| Redis cache | Yes | Not implemented as a module |
| Blob storage | Yes | Not implemented as a module |

## Already Implemented in EAF

- `OrganizationUnitAppService` + Angular `admin/organization-units`.
- `MassNotificationAppService` + Angular `admin/mass-notifications`.
- `UserDelegationAppService` + Angular `admin/user-delegations`.
- `TenantJoinRequest` + Angular `admin/tenant-join-requests`.
- `DashboardAppService` + Angular `main/dashboard`.
- Payment gateway abstraction (`IPaymentGateway`, resolver, gateways, `PaymentAppService`).
- LDAP / Azure AD modules.
- `Eaf.KeyVault`, `Eaf.OpenTelemetry`, `Eaf.Castle.Serilog`, `Eaf.SqlServerCache`, `Eaf.SqliteCache`.

## Gaps / Next Steps

1. **SMS module** (`Eaf.Sms` + Twilio / AWS SNS provider).
2. **MailKit module** (`Eaf.MailKit`) for richer email templates and providers (SendGrid, Mailgun).
3. **BlobStoring module** (`Eaf.BlobStoring` + Azure/AWS/FileSystem providers) for file uploads and tenant assets.
4. **Redis cache module** (`Eaf.RedisCache`) for distributed caching.
5. **SignalR module** (`Eaf.SignalR` / `Eaf.SignalR.AspNetCore`) with notification and chat backends.
6. **Push notifications** (Web Push + `libnpush`) for PWA.
7. **Dynamic forms / surveys** (low priority).
8. **Tenant impersonation** for support scenarios.
9. **Audit logs UI** (ABP already writes logs; Angular UI missing).
10. **Language management** UI for localization resources.

## Proposed Modules

- `Eaf.Sms` — SMS service interface + Twilio provider.
- `Eaf.MailKit` — email service interface + MailKit provider + templates.
- `Eaf.BlobStoring` — file storage abstraction + container/provider pattern.
- `Eaf.RedisCache` — distributed cache with ABP `ICacheManager`.
- `Eaf.SignalR` — real-time notifications and chat backend.
- `Eaf.PushNotifications` — Web Push + PWA integration.

## Implementation Status (2026-08)

Partial. Several Zero features are already in EAF, but distributed cache, blob storage, MailKit, SignalR, SMS and push notifications are missing.

## Migration Plan
1. Prioritize the missing modules by impact (Redis > Blob > MailKit > SignalR > SMS > Push).
2. Create each module following the existing EAF middleware module pattern.
3. Add Angular admin pages and service proxies.
4. Update templates to include the new modules and configurations.

## Impact
- **High**: increases EAF parity with commercial ASP.NET Zero.
- **Medium**: grows the middleware package surface.
- **High**: improves value for template users.

## Risks
- Some ASP.NET Zero features may be patented/licensed; implement independently using ABP patterns only.
- Avoid copying names, code or assets from ASP.NET Zero.

## References
- <https://aspnetzero.com/Features> — ASP.NET Zero feature list.
- `src/Eaf.Middleware.Application/Authorization/Users/UserDelegationAppService.cs`
- `src/Eaf.Middleware.Application/Organizations/OrganizationUnitAppService.cs`
- `src/Eaf.Middleware.Application/Payments/PaymentAppService.cs`
