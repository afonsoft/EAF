# EAF — ABP Boilerplate vs ASP.NET Zero vs EAF Feature Comparison

## Summary

Side-by-side comparison of **ASP.NET Boilerplate (ABP)**, **ASP.NET Zero** (commercial) and **EAF** (open-source middleware/template) across core framework, identity, multi-tenancy, infrastructure and UI capabilities. The goal is to identify missing features and prioritize them for EAF.

## Comparison Matrix

| Area | ABP Framework (open source) | ASP.NET Zero (commercial) | EAF (2026-08) | Gap Level |
|---|---|---|---|---|
| **Runtime** | .NET Standard / .NET 6+ | .NET 6+ | .NET 10 | — |
| **Architecture** | N-Layer, DDD, modular | N-Layer, DDD, modular | N-Layer, DDD, modular (ABP 10.5) | None |
| **DI** | Castle Windsor / MS DI | Castle Windsor / MS DI | Castle Windsor | None |
| **ORM** | EF Core / NHibernate | EF Core | EF Core 10 | None |
| **Multi-tenancy** | Database per tenant / shared | Same + tenant dashboard | Shared/host with tenant join requests | Low |
| **Users / Roles / Permissions** | Full | Full + OU + delegation | Full + OU + delegation | None |
| **Organization Units** | Backend entities | Tree UI + member/role mgmt | **Implemented** | None |
| **User Delegation / Impersonation** | Impersonation | Time-bounded delegation | **Implemented** | None |
| **Mass Notifications** | Basic | Admin mass-sending | **Implemented** | None |
| **Tenant Join Requests** | No | Yes | **Implemented** | None |
| **Dashboard** | Empty | Host + tenant dashboards | **Implemented** | None |
| **Payment / Subscription** | No | Stripe/PayPal + subscription lifecycle | Payment gateways only; no subscription lifecycle | High |
| **SMS** | No | Yes (Twilio) | Not implemented | Medium |
| **MailKit / Rich Email** | Basic | MailKit templates | Not implemented | Medium |
| **Blob Storage** | No | Azure/AWS/FileSystem | Not implemented | Medium |
| **Redis Cache** | No | Distributed cache | Not implemented | Medium |
| **SignalR Module** | No | Real-time notifications/chat | `ChatHub` only; no module | Medium |
| **Push Notifications** | No | Yes | Not implemented | Medium |
| **Passwordless Login** | No | Email/SMS code | Not implemented | Medium |
| **QR Login** | No | Mobile app QR | Not implemented | Low |
| **Social Account Linking** | No | Profile link/unlink | Not implemented | Low |
| **Setup Page** | No | Web-based setup | Not implemented | Low |
| **Audit Logs UI** | Backend + basic UI | Advanced UI | Basic UI exists | Low |
| **Rate Limiting** | No | Core + UI | Core exists; UI missing | Low |
| **Background Jobs** | Hangfire/Quartz | Hangfire | Hangfire | None |
| **OpenTelemetry** | Optional | Optional | **Implemented** | None |
| **Key Vault** | No | Azure Key Vault | **Implemented** | None |
| **Serilog** | Optional | Optional | **Implemented** | None |
| **Angular UI** | Plain Bootstrap | Metronic 8 + Bootstrap 5 + PrimeNG | Metronic legacy + PrimeNG 17 + ngx-bootstrap | High (modernization) |
| **Dark Mode** | No | 13+ themes incl. dark | Not implemented | High |
| **PWA** | No | MAUI + PWA | SW configured; no offline/push | Medium |
| **Mobile Responsive** | Bootstrap responsive | Mobile-first Metronic 8 | Desktop-first legacy | Medium |
| **Accessibility** | Basic | Better a11y | Partial | Medium |
| **Localization** | XML/JSON | XML/JSON | XML/JSON | None |
| **Swagger** | Swashbuckle | Swashbuckle | Swashbuckle | None |

## Key Findings

1. **EAF is closer to ABP than to ASP.NET Zero** in feature breadth. Many Zero enterprise features (payment lifecycle, SMS, blob, Redis, SignalR module, push) are missing.
2. **EAF has already closed several Zero gaps**: Organization Units, Mass Notifications, User Delegation, Tenant Join Requests, Dashboard, Payment Gateway abstraction, Key Vault, OpenTelemetry.
3. **UI modernization is the largest gap**: ASP.NET Zero uses Metronic 8 + Bootstrap 5 + dark mode; EAF still relies on legacy Metronic and `ngx-bootstrap`.
4. **Infrastructure modules should be the next backend focus**: Redis, Blob, MailKit, SignalR and SMS can be built as independent open-source modules without copying Zero.

## Recommended Next Steps

1. Implement `Eaf.RedisCache` (distributed cache).
2. Implement `Eaf.MailKit` (rich email).
3. Implement `Eaf.BlobStoring` (file storage).
4. Implement `Eaf.SignalR` module.
5. Implement `Eaf.Sms` (Twilio / AWS SNS).
6. Build subscription lifecycle on top of existing payment gateways.
7. Modernize Angular template: PrimeNG theming, Bootstrap 5/Metronic 8, dark mode, PWA offline, mobile-first layout.

## Risks

- Do not copy ASP.NET Zero code, names or assets.
- Metronic 8 is a commercial product; choose an independent design system or obtain a license.
- Each new module increases maintenance and CI time; prioritize by community demand.

## References

- ABP Framework: <https://abp.io/>
- ASP.NET Zero: <https://aspnetzero.com/>
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-aspnetzero-feature-adoption.spec.md`
