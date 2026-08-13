# EAF — Eaf.OpenIddict Integration

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Eaf.OpenIddict Integration |
| Product / System | EAF Middleware / Templates |
| Module / Bounded Context | Authentication / Identity |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-openiddict` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF relies on ABP token-based authentication but does not expose an OAuth2/OIDC server. ASP.NET Zero documents an OpenIddict integration that turns the host application into an authorization server, enabling third-party clients and SPAs to authenticate via standard OAuth2/OIDC flows.

### Objective

Create `Eaf.OpenIddict` middleware module that integrates `OpenIddict` with EAF identity and tenants, exposing `/connect/token`, `/connect/authorize`, `/connect/userinfo`, and `/connect/logout` endpoints.

### Expected outcome

- `src/Eaf.OpenIddict/` project and module.
- `EafOpenIddictModule` with configuration from `appsettings.json`.
- Application/scope/authorization/token stores using EF Core.
- Dynamic API client registration via settings.
- Sample console client (ConsoleApiClient) demonstrating password and client credentials flows.

### Out of scope

- UI for consent screen (use a minimal server-rendered page or API-only flows).
- IdentityServer4 (legacy; OpenIddict only).

## 2. Agent Role

Senior .NET security engineer. Follow OpenIddict and ASP.NET Zero docs; do not invent custom security logic.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not remove existing `TokenAuthController`; do not change JWT validation; do not push packages.

## 4. Product Context

### Functional context

Third-party clients and SPAs can obtain tokens from EAF using standard OAuth2/OIDC. This is the foundation for API-first integrations and mobile apps.

### Technical context

- ABP 10.5 has OpenIddict integration patterns.
- EAF uses `Eaf.Middleware.Web.Core` for pipeline.
- Identity is ABP Zero identity (`User`, `Role`, `Permission`).

### Relevant stack

- C# 14 / .NET 10 / ABP 10.5
- OpenIddict 6.x (or compatible)
- EF Core 10
- xUnit / Shouldly / NSubstitute

### Relevant files or directories

```text
src/Eaf.Middleware.Web.Core/
src/Eaf.Middleware.Core/Authorization/Users/
Templates/Api/
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Web.Core/Startup/`
- `src/Eaf.Middleware.Core/Authorization/Users/User.cs`
- `.specs/eaf-aspnetzero-docs-gap-analysis.spec.md`

## 5. Task Definition

### Main task

Create `Eaf.OpenIddict` module and expose standard OAuth2/OIDC endpoints.

### Subtasks

1. Create `src/Eaf.OpenIddict/` project.
2. Implement `EafOpenIddictModule` and configuration.
3. Add EF Core `DbContext` or entities for OpenIddict stores.
4. Add `AuthorizationController` / `TokenController` / `UserInfoController`.
5. Add scope/permission claims mapping.
6. Add `appsettings.json` sample and README.
7. Add sample client.
8. Add integration tests.

### Do not do

- Do not replace existing `TokenAuthController`.
- Do not disable existing JWT validation.
- Do not implement custom cryptography.

## 6. Functional Requirements

### FR-001: OAuth2 token endpoint

**Description:** Support `password` and `client_credentials` grant types.

**Rules:**

- Endpoint `/connect/token`.
- Validate user credentials against EAF identity.
- Return access token and optionally refresh token.

**Acceptance criteria:**

- [ ] Token endpoint returns JWT for valid credentials.
- [ ] Client credentials flow works for registered applications.

### FR-002: Authorization endpoint

**Description:** Support authorization code flow.

**Rules:**

- Endpoint `/connect/authorize`.
- Requires user authentication (cookie or existing JWT).
- Returns authorization code to registered redirect URI.

**Acceptance criteria:**

- [ ] Authorization code flow works with a test client.

### FR-003: UserInfo endpoint

**Description:** Return claims for the authenticated user.

**Rules:**

- Endpoint `/connect/userinfo`.
- Requires valid access token.
- Returns `sub`, `name`, `email`, `roles`, `permissions`.

**Acceptance criteria:**

- [ ] UserInfo returns correct claims.

### FR-004: Application registration

**Description:** Register OpenIddict applications via `appsettings.json`.

**Rules:**

- JSON config with `ClientId`, `ClientSecret`, `DisplayName`, `ConsentType`, `RedirectUris`, `PostLogoutRedirectUris`, `Scopes`, `Permissions`.
- Seeded on module initialization.

**Acceptance criteria:**

- [ ] Applications seeded from config.
- [ ] Unknown clients rejected.

## 7. Business Rules

### BR-001: Multi-tenancy

Token requests must respect `TenantId` (header, subdomain, or claim).

### BR-002: Existing JWT compatibility

Tokens issued by OpenIddict must be valid for existing `[AbpAuthorize]` controllers.

### BR-003: Least privilege

Default permissions should follow least-privilege; admin must explicitly grant sensitive scopes.

## 8. Domain Modeling

### Bounded Context

Authentication / Identity

### Entities

N/A — uses OpenIddict EF Core stores and ABP `User`.

### Value Objects

| Value Object | Fields | Validations |
|---|---|---|
| `OpenIddictApplicationConfig` | `ClientId`, `ClientSecret`, `Scopes`, `Permissions` | `ClientId` required |

## 9. Expected Architecture

```text
src/Eaf.OpenIddict/
  EafOpenIddictModule.cs
  Configuration/
    EafOpenIddictOptions.cs
  Stores/
    (OpenIddict EF Core stores if not using built-in)
  Controllers/
    AuthorizationController.cs
    TokenController.cs
    UserInfoController.cs
    LogoutController.cs
test/Eaf.OpenIddict.Tests/
sample/ConsoleApiClient/
```

## 10. API Contracts

### Token

```http
POST /connect/token
Content-Type: application/x-www-form-urlencoded

grant_type=password&username=...&password=...&client_id=...&client_secret=...
```

### UserInfo

```http
GET /connect/userinfo
Authorization: Bearer {access_token}
```

## 11. Application Contracts

No new application services. OpenIddict controllers call existing `LogInManager` and `UserManager`.

## 12. Persistence and Data

### Persisted entities

Uses OpenIddict EF Core entities (`OpenIddictApplications`, `OpenIddictAuthorizations`, `OpenIddictTokens`, `OpenIddictScopes`).

### Migration required

Yes.

### Indexes

N/A — provided by OpenIddict.

## 13. Integrations

### External services

N/A.

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `LogInManager` | Validate credentials | In-process | default | no |
| `UserManager` | Claims / roles | In-process | default | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Invalid client | unknown client_id | Return `invalid_client` OAuth error |
| Wrong tenant | missing tenant context | Reject or use host default based on config |
| Expired token | expired access token | Return 401 with `invalid_token` |

## 15. Few-Shot Examples

### Example 1: Password flow

```bash
curl -X POST https://localhost:44301/connect/token \
  -d "grant_type=password" \
  -d "username=admin" \
  -d "password=..." \
  -d "client_id=client" \
  -d "client_secret=..." \
  -d "scope=default-api profile"
```

## 16. Non-Functional Requirements

### Security

- Use OpenIddict built-in token validation.
- Rotate client secrets via settings.
- HTTPS only in production.

### Performance

- Token endpoint P95 < 300 ms.

### Observability

- Log token issuance and failures (without secrets).

## 17. Mandatory Guardrails

- Do not invent custom crypto.
- Do not disable existing JWT auth.
- Do not expose client secrets in logs or UI.

## 18. Expected Tests

### Integration tests

| Flow | Validation |
|---|---|
| Password flow | Returns valid access token |
| Client credentials | Returns token for configured client |
| UserInfo | Returns claims for token |
| Invalid credentials | Returns OAuth error |

## 19. Acceptance Criteria

- [ ] `Eaf.OpenIddict` compiles and packs.
- [ ] `/connect/token` and `/connect/userinfo` work.
- [ ] Sample client authenticates.
- [ ] Existing JWT auth still works.
- [ ] README.md with setup.

## 20. Implementation Plan

1. Create module and configure OpenIddict.
2. Add EF Core stores and migration.
3. Implement token/authorize/userinfo/logout endpoints.
4. Add application seeding from config.
5. Add sample client.
6. Add integration tests.
7. Update index.

## 21. Rollback Strategy

- Remove `[DependsOn(typeof(EafOpenIddictModule))]`.
- Remove OpenIddict migration if not deployed.
- Existing `TokenAuthController` remains functional.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Misconfigured OpenIddict breaks auth | High | Low | Run in parallel with existing auth, feature-flag optional |
| Complex PKCE/consent screens | Medium | Low | Start with password and client_credentials only |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Module implemented and tested.
- [ ] Sample client works.
- [ ] Index updated.

## 24. Key Reminder

> OpenIddict is a security feature. Follow the framework and do not implement custom cryptography. Keep existing JWT auth intact.
