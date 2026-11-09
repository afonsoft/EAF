# EAF — API Key Authentication (W1)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | API Key Authentication |
| Product / System | EAF Middleware + Api/Angular Templates |
| Module / Bounded Context | Security / Authentication |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-api-key-auth` |
| Technical owner | Core Team |
| Status | Completed|
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W1 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

Machine-to-machine callers (CI/CD, scripts, third-party services) must use interactive JWT login or keep refresh tokens alive. ASP.NET Zero v15.5 added built-in API key authentication; EAF has none (only provider keys for SMS/Push).

### Objective

Add user-scoped, tenant-aware API key authentication to EAF middleware and templates, implemented from scratch on ABP 10.5.1 / ASP.NET Core.

### Expected outcome

- `X-API-Key` header authenticates requests against any app service endpoint.
- Users manage own keys; admins manage all tenant keys.
- Keys stored as hashes only; shown once at creation.

### Out of scope

- Copying ASP.NET Zero code.
- OAuth2/OIDC client-credentials flow (covered by `eaf-module-openiddict.spec.md`).
- Key-based billing/metering.

## 2. Agent Role

Senior .NET/ABP engineer. Implement on existing EAF conventions (`Eaf.Middleware.Web.Core` authentication folder, templates).

## 3. Agent Autonomy Level

**2 — Reliable**. No push to main; PR per feature with tests.

## 4. Product Context

EAF `Eaf.Middleware.Web.Core` already hosts JWT/refresh-token auth (`Authentication/JwtBearer/`), `TokenAuthController`, external auth services. API keys plug into the same authentication pipeline.

### Relevant stack

- .NET 10, ABP 10.5.1, EF Core 10, Castle Windsor, Angular 20, PrimeNG 17
- Tests: xUnit + Shouldly + NSubstitute

### Context files the agent must read before implementation

- `CLAUDE.md`
- `src/Eaf.Middleware.Web.Core/Authentication/`
- `src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs`
- `Templates/Api/src/Eaf.ProjectName.Web.Core/` (AuthConfigurer)
- `.specs/eaf-specs-index-and-roadmap-2026.md`

## 5. Task Definition

### Main task

Implement API key authentication end-to-end: domain entity + hashing, authentication handler, permission scoping, expiration/revocation/rotation, EF migration, Angular management pages.

### Subtasks

- Domain: `ApiKey` entity (user, tenant, name, key hash, scopes, IP allowlist, expiration, status, created/last-used timestamps).
- Crypto: key generation (`eak_` + 256-bit random, base64url), SHA-256 (or stronger) hash storage; plaintext never persisted or logged.
- Auth: `ApiKeyAuthenticationHandler` reading `X-API-Key`, resolving user+tenant, applying permission scope, rejecting expired/revoked/out-of-scope-IP keys; integrated in `AuthConfigurer` as additional scheme.
- App services: `IApiKeyAppService` (create/list/revoke/rotate own keys) and admin `ITenantApiKeyAppService` (list/revoke all keys in tenant).
- Settings: enable/disable feature, max keys per user, max lifetime, mandatory scope (host + tenant level).
- EF migration for `EafApiKeys` table.
- Angular: "My API Keys" page (user menu) + "Administration > API Keys" page; creation modal shows full key once with copy warning.
- Audit: login attempts / audit log entries for key creation, use failures, revocation.

### Do not do

- Do not log key material.
- Do not bypass existing permission system — a key can only narrow, never widen, the user's permissions.
- Do not change existing JWT flows.

## 6. Functional Requirements

### FR-001: Key issuance

**Description:** Authenticated user creates a named key with optional permission subset, IP allowlist (single IPs/CIDR) and expiration.

**Rules:**
- Full key returned once; only `KeyPrefix` (first 8 chars) + hash stored.
- Enforce per-user/host/tenant max active keys and max lifetime from settings.

**Acceptance criteria:**
- [ ] Created key authenticates a request via `X-API-Key`.
- [ ] Hash in DB cannot be reversed to the key.
- [ ] Exceeding limits returns `UserFriendlyException`.

### FR-002: Authentication

**Description:** Requests with `X-API-Key` run as the owning user in that user's tenant, narrowed to the key's permission scope.

**Acceptance criteria:**
- [ ] Valid key → principal with user claims + scoped permissions only.
- [ ] Expired/revoked key → 401.
- [ ] IP outside allowlist → 403.
- [ ] Key without explicit scope behaves as full user permissions only when scope not mandatory.

### FR-003: Management

**Description:** Users list/revoke/rotate own keys; tenant admins list/revoke any key in the tenant.

**Acceptance criteria:**
- [ ] Rotate returns new key once and revokes old atomically.
- [ ] Revoke takes effect immediately (cache invalidation).
- [ ] Angular pages enforce permissions.

## 7. Business Rules

- BR-001: Keys belong to a user and run in that user's tenant; host users' keys have no tenant.
- BR-002: Permission scope can only be a subset of the user's granted permissions at evaluation time.
- BR-003: When host/tenant setting requires scope, creating a scopeless key is rejected.
- BR-004: Failed key authentications are auditable (same pipeline as login attempts where feasible).

## 8. Domain Modeling

| Entity | Identity | Responsibility |
|---|---|---|
| `ApiKey` | `long` / `Guid` | One issued credential: owner user, tenant, name, `KeyHash`, `KeyPrefix`, `AllowedPermissions`, `AllowedIpRanges`, `ExpirationTime`, `Status`, `LastUsedTime` |

## 9. Expected Architecture

ABP layered: entity + domain service in `Eaf.Middleware.Core` (or `Eaf.Middleware.Web.Core` if web-coupled), app services in `Eaf.Middleware.Application`, handler + registration in `Eaf.Middleware.Web.Core`, EF config in templates' `EntityFrameworkCore` project, Angular in `Templates/Angular/.../src/app`.

## 10. API Contracts

`X-API-Key: eak_<secret>` header on any existing endpoint. Management via ABP app services (`/api/services/app/ApiKey/...`).

## 12. Persistence and Data

New table `EafApiKeys`; migration required (up + down). Indexes: `(KeyHash)` unique, `(UserId)`, `(TenantId)`.

## 16. Non-Functional Requirements

- Hash lookup O(1) via unique index; cache key→principal with short TTL + revocation invalidation.
- No plaintext keys in logs, audit payloads, or DB.
- Key generation uses `RandomNumberGenerator`.

## 18. Expected Tests

- Unit: key generation/hash, scope evaluator, IP matcher (CIDR), expiration/status transitions, rotation atomicity.
- Integration: authenticated request via handler for valid/expired/revoked/out-of-scope-IP keys; admin listing respects tenant boundary.

## 19–24. Acceptance / Risks

- All FRs covered by tests; coverage not reduced; docs updated (`docs/`).
- Risks: cache staleness on revoke (mitigate: short TTL + explicit invalidation); permission drift after role change (scope evaluated against current grants).

### Pending Questions and Ambiguities

- Naming: `EafApiKeys` table vs `AbpApiKeys` — decide at implementation (prefer `Eaf` prefix per convention).
- Scope storage format: JSON array of permission names vs join table — recommend JSON for simplicity.

## Delivered

Implementado e mergeado em `main` (PR #448).
