# AD-0001 — ASP.NET Zero v15.5 Parity: New Auth & Workflow Modules

## Context

In November 2026, EAF 9.5.x was audited against ASP.NET Zero v15.5 (docs + changelog). The audit produced the parity plan `/home/ubuntu/eaf-aspnetzero-v155-improvement-plan.md` and a series of specs in `.specs/` covering four waves (W1 auth/security, W2 data/mapper, W3 workflow engine, W4 remaining items). ABP open-source remains at 10.5.1 on NuGet (the "ABP 11.4" referenced by ASP.NET Zero is an internal/commercial build).

Forces at play:

- New capabilities (API keys, passkeys, session registry, OpenIddict, workflow engine) must not break the pinned `Eaf.ProjectName.sln` template build, which consumes published NuGet packages, not source projects.
- Migrations must be hand-written because the EF Core 10 differ fails on the repo's csproj shape (`MSB4057 ResolvePackageAssets`).
- The UI template must stay on PrimeNG — ASP.NET Zero migrated to ng-zorro, a full redesign we deliberately rejected.

## Decision

1. **Auth/Security (W1)** — `EafApiKey` (SHA-256 hash-only storage, `X-API-Key` header, `EafApiKey` auth scheme + `UseApiKeyTokenMiddleware`), `UserPasskey` via the Fido2 library (the .NET 10 `IUserPasskeyStore` path was rejected as too invasive for `AbpUserStore`), and `UserSessionInfo` + `SessionRegistry` cache with `eaf_session_id` JWT claim, revocation denylist and idle-timeout sliding. Alternatives rejected: Identity's built-in passkey store (invasive), per-token revocation list (session-id denylist is simpler and O(1)).
2. **Workflow engine (W3)** — standalone `src/Eaf.Workflow` module with definition/version/instance/task/timeline aggregates, serialized JSON step graph, and the `IWorkflowUserDirectory` seam for assignee resolution (the concrete directory lives in the template and resolves roles/OUs/managers through ABP tables). Version snapshots are immutable; instances pin the version started (BR-001/BR-002).
3. **OpenIddict (W4.3)** — `src/Eaf.OpenIddict` module exposing `/connect/token` (password, client_credentials, authorization_code + PKCE, refresh_token), `/connect/authorize`, `/connect/userinfo`, `/connect/logout`. Access tokens share the same symmetric HMAC-SHA256 key as `TokenAuthController` and `DisableAccessTokenEncryption()` so `[AbpAuthorize]` keeps working; `Authentication:JwtBearer:AdditionalIssuers` allows the OpenIddict issuer.
4. **Template gating (new constant `EAF_USE_SOURCE_PROJECTS`)** — all template references to `Eaf.OpenIddict` (and other modules with no published package) are compiled only when `UseSourceProjects == 'true'` in `Templates/Api/Directory.Build.props`. This keeps `Eaf.ProjectName.sln` compiling against NuGet packages while `Eaf.ApiWithSrc.sln` gets the new modules.
5. **Angular 21 + PrimeNG 21 (W4)** — upgraded instead of migrating to ng-zorro. New theming via `@primeuix/themes` (`providePrimeNG` + Lara preset); legacy `theme.css` removed; component renames applied (Calendar→DatePicker, Dropdown→Select, TabView→Tabs). Alternative rejected: ng-zorro (full UI redesign, no parity with existing pages).
6. **Mapperly (W2)** — AutoMapper→Mapperly migration validated as a spike (GO); `EafMapperlyObjectMapper` registered `IsDefault` routes mapped pairs to generated code and delegates everything else to the still-registered AutoMapper. Full rollout (~2.5 days) deferred to a follow-up spec.

## Consequences

- Positive: feature parity with ASP.NET Zero v15.5 on ABP 10.5.1; template builds stay green in both package and source modes; `[AbpAuthorize]` accepts tokens from both `TokenAuthController` and OpenIddict; workflow engine is reusable outside the template.
- Trade-off: template carries conditional compilation (`#if EAF_USE_SOURCE_PROJECTS`); migrations for the new modules are hand-written and must be regenerated manually when the schema evolves; Angular 21's stricter `checkNoChanges` required rewriting mid-cycle directive specs.

## Related SPECs

- [.specs/eaf-api-key-authentication.spec.md](../../.specs/eaf-api-key-authentication.spec.md) — PR #448
- [.specs/eaf-passkey-login.spec.md](../../.specs/eaf-passkey-login.spec.md) — PR #450
- [.specs/eaf-session-security.spec.md](../../.specs/eaf-session-security.spec.md) — PR #451
- [.specs/eaf-mandatory-dynamic-properties.spec.md](../../.specs/eaf-mandatory-dynamic-properties.spec.md) — PR #452
- [.specs/eaf-mapperly-migration.spec.md](../../.specs/eaf-mapperly-migration.spec.md) — PR #453
- [.specs/eaf-workflow-engine.spec.md](../../.specs/eaf-workflow-engine.spec.md) — PRs #454–#456
- [.specs/eaf-email-template-management.spec.md](../../.specs/eaf-email-template-management.spec.md) — PR #457
- [.specs/eaf-qr-login.spec.md](../../.specs/eaf-qr-login.spec.md) — PR #458
- [.specs/eaf-module-openiddict.spec.md](../../.specs/eaf-module-openiddict.spec.md) — PR #459
- [.specs/eaf-angular-21-primeng-upgrade.spec.md](../../.specs/eaf-angular-21-primeng-upgrade.spec.md) — PR #460
