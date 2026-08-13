# EAF — Next Steps Roadmap (Q3 2026)

## Summary

Consolidated roadmap for the EAF project in Q3 2026, based on the updated spec analysis and comparison with ASP.NET Boilerplate and ASP.NET Zero. The roadmap focuses on backend middleware modules, Angular template modernization and quality improvements.

## Themes

1. **Close infrastructure gaps** with independent open-source middleware modules.
2. **Modernize the Angular template** (Bootstrap 5 / Metronic 8, PrimeNG, dark mode, PWA, mobile-first).
3. **Improve quality and performance** (tests, coverage, caching, bundle size).
4. **Sustain parity** with ABP and ASP.NET Zero enterprise features without copying proprietary assets.

## Roadmap

### P1 — Backend Middleware Modules

| Module | Status | Why | Owner |
|---|---|---|---|
| `Eaf.RedisCache` | Not started | Distributed cache; required for scale and SignalR backplane. | Core Team |
| `Eaf.MailKit` | Not started | Rich email templates and providers; replaces basic email. | Core Team |
| `Eaf.BlobStoring` | Not started | File uploads, tenant assets, email attachments. | Core Team |
| `Eaf.SignalR` | Not started | Formalize real-time backend with backplane support. | Core Team |
| `Eaf.Sms` | Not started | SMS for 2FA, passwordless, mass notifications. | Community / Core |
| `Eaf.PushNotifications` | Not started | Web Push for PWA. | Core Team |

### P2 — Subscription & Payment Lifecycle

| Feature | Status | Why | Owner |
|---|---|---|---|
| `SubscriptionPayment` entities | Not started | Close the largest ASP.NET Zero gap. | Core Team |
| `SubscriptionAppService` | Not started | Tenant subscription CRUD, upgrades, renewals. | Core Team |
| Invoice / receipt worker | Not started | Billing automation. | Core Team |
| Tenant fallback edition on expire | Not started | Graceful degradation. | Core Team |

### P3 — Angular Template Modernization

| Feature | Status | Why | Owner |
|---|---|---|---|
| PrimeNG full adoption | Partial | Reduce `ngx-bootstrap` and jQuery. | Frontend Team |
| Bootstrap 5 / Metronic 8 migration | Not started | Mobile-first, modern CSS, dark mode ready. | Frontend Team |
| Dark mode and design tokens | Not started | Expected enterprise feature; Zero has 13+ themes. | Frontend Team |
| Mobile responsive layout | Partial | Off-canvas, bottom nav, touch targets. | Frontend Team |
| PWA offline + push | Partial | Service Worker exists; offline UX missing. | Frontend Team |
| Accessibility (WCAG 2.1 AA) | Not started | Compliance and better UX. | Frontend Team |
| Lazy-loaded admin modules | Not started | Reduce initial bundle. | Frontend Team |

### P4 — Quality & Performance

| Feature | Status | Why | Owner |
|---|---|---|---|
| Performance instrumentation | Not started | Measure before optimizing. | Core Team |
| Angular bundle budgets | Not started | Enforce size limits. | Frontend Team |
| Redis cache integration | Not started | Permission/settings cache. | Core Team |
| API N+1 audit | Not started | Faster reads, lower DB load. | Core Team |
| A11y + Lighthouse CI | Not started | Automated regression. | DevEx |

### P5 — Documentation & Developer Experience

| Feature | Status | Why | Owner |
|---|---|---|---|
| Spec index maintenance | In progress | Keep `.specs/` as roadmap source. | DevEx |
| Version migration guides | In progress | Reduce upgrade friction. | DevEx |
| Skill/rules for agents | Not started | Improve agent onboarding. | DevEx |

## Milestones

- **End of July 2026**: `Eaf.RedisCache` PoC + Angular dark mode spike.
- **End of August 2026**: `Eaf.MailKit` and `Eaf.BlobStoring` modules; PrimeNG migration 50% complete.
- **End of September 2026**: `Eaf.SignalR` module; Bootstrap 5/Metronic 8 layout spike; PWA offline MVP.

## Success Criteria

- All P1 modules compile and have unit/integration tests with ≥ 90% coverage.
- Angular template Lighthouse score ≥ 80 on mobile.
- No increase in existing CI build/test time beyond 20%.
- No reduction in overall code coverage.
- All new modules documented in `.specs/` and `docs/`.

## Risks and Mitigations

| Risk | Mitigation |
|---|---|
| Large Angular rewrite breaks existing projects | Create new opt-in theme (`theme13`) and keep legacy themes for one major release. |
| New modules add maintenance burden | Follow ABP `EafModule` pattern and require tests + docs. |
| Copying Zero features inadvertently | Use only public ABP docs and implement from scratch with original naming. |
| Redis/SMS providers require cloud accounts | Provide mock/in-memory providers for local dev and tests. |

## References

- `.specs/eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
