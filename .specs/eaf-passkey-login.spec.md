# EAF — Passkey (WebAuthn) Login (W1)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Passkey (WebAuthn) Login |
| Product / System | EAF Middleware + Api/Angular Templates |
| Module / Bounded Context | Security / Authentication |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-passkey-login` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W1 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

Users can only sign in with password + optional 2FA. ASP.NET Zero v15.5 ships passkey login on "ABP 11.4" — which is **not publicly available** (latest public `Abp.*` is 10.5.1). EAF must implement passkeys directly on .NET 10 ASP.NET Core Identity passkey support over Module Zero 10.5.1.

### Objective

Opt-in passwordless sign-in via WebAuthn (fingerprint, face, screen lock, security keys) on the Angular UI, with per-user passkey registration/management.

### Expected outcome

- Setting "Allow users to sign in with passkeys" enables registration + login.
- Users register/remove passkeys from a "Passkeys" page in the user menu.
- Failed passkey logins recorded in login attempts; forced password change still enforced.

### Out of scope

- Waiting for public ABP 11.x.
- Passkey on React-Native template.
- Synced vs device-bound attestation policy management (default options only).

## 2. Agent Role

Senior .NET/ABP engineer. Base implementation on .NET 10 `SignInManager`/`IdentityUser` passkey APIs; adapt to Module Zero 10.5.1 user store (custom `UserPasskey` store may be required).

## 3. Agent Autonomy Level

**2 — Reliable**. PR with tests; no main pushes.

## 4. Product Context

.NET 10 adds passkey support to ASP.NET Core Identity (`AddPasskeys`, `SignInManager.Passkey*` APIs). Module Zero 10.5.1 does not ship a passkey store; implement an EAF `UserPasskey` entity + store, wire into `TokenAuthController`/login flow.

### Relevant stack

- .NET 10 (Identity passkey APIs), ABP 10.5.1, Angular 20, WebAuthn browser APIs (`navigator.credentials`).

### Context files the agent must read before implementation

- `CLAUDE.md`
- `src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs`
- `src/Eaf.Middleware.Web.Core/Authentication/`
- Templates Angular login/account pages
- Microsoft docs: "Implement passkeys in ASP.NET Core Identity (.NET 10)"

## 5. Task Definition

### Main task

Implement passkey registration and authentication end-to-end: `UserPasskey` entity + EF migration, WebAuthn attestation/assertion endpoints, Angular registration UI and passkey login button.

### Subtasks

- Entity `UserPasskey` (user id, credential id, public key, sign count, attestation data, friendly name, created/last-used).
- Endpoints: `Passkey/RegisterOptions`, `Passkey/Register`, `Passkey/AssertionOptions`, `Passkey/Authenticate`, `Passkey/Delete` (own keys only).
- User-management setting (host/tenant): enable/disable passkeys.
- RP ID / origin config per host (and per tenant domain where applicable); HTTPS enforcement documented.
- Angular: "Passkeys" page in user menu (list, register with friendly name, delete); passkey button on login screen.
- Login attempts integration: failed assertions logged; successful passkey login respects forced-password-change and lockout policies.

### Do not do

- Do not weaken 2FA/lockout policies.
- Do not store attestation trust decisions beyond defaults (accept platform + cross-platform authenticators).
- Do not break existing password/social login flows.

## 6. Functional Requirements

### FR-001: Registration

**Description:** Authenticated user registers a passkey; server issues `PublicKeyCredentialCreationOptions`, verifies attestation, stores credential.

**Acceptance criteria:**
- [ ] Registered credential appears in user's passkey list with friendly name.
- [ ] Duplicate credential id rejected.

### FR-002: Authentication

**Description:** Login screen offers passkey sign-in; server issues assertion options, verifies signature + sign count, logs the user in.

**Acceptance criteria:**
- [ ] Valid assertion issues the same session/JWT as password login.
- [ ] Failed assertion recorded in login attempts.
- [ ] Disabled setting hides/blocks passkey login.

### FR-003: Management

**Acceptance criteria:**
- [ ] User deletes own passkey; admin cannot register passkeys for others.
- [ ] Deleted credential can no longer authenticate.

## 7. Business Rules

- BR-001: Opt-in feature — disabled by default unless enabled in settings.
- BR-002: Passkey login enforces the same forced-password-change flag.
- BR-003: One user, many passkeys; keys are user-owned and tenant-scoped implicitly via the user.

## 8. Domain Modeling

| Entity | Responsibility |
|---|---|
| `UserPasskey` | Credential id (unique), public key + algorithm, sign count (replay protection), transports, friendly name, timestamps |

## 9. Expected Architecture

Domain: `Eaf.Middleware.Core` entity. App layer: passkey app service. Web: endpoints in `Eaf.Middleware.Web.Core` + template `Web.Core`. Angular under `account/` + user menu.

## 12. Persistence and Data

New table `EafUserPasskeys`; unique index on credential id; FK to users. Migration up/down.

## 14. Edge Cases

| Scenario | Expected |
|---|---|
| Replay (stale sign count) | Reject + audit |
| Credential deleted then reused | 401 |
| HTTP origin (non-HTTPS) | WebAuthn unavailable — document + graceful UI |
| User with only passkey + forced password change | Password change still required post-login |

## 16. Non-Functional Requirements

- Challenge TTL ≤ 5 min, single-use.
- No PII beyond user handle in options; attestation blobs not logged.

## 18. Expected Tests

- Unit: options issuance, attestation verification (fixture vectors), sign-count validation, delete authorization.
- Integration: register → authenticate → delete round-trip; disabled setting blocks.

## 19–24. Acceptance / Risks

- Tests + docs; risk: Module Zero lacks store → implement minimal `IUserPasskeyStore`-style adapter (mitigation: isolate in one class with tests).

### Pending Questions and Ambiguities

- Exact Identity passkey API surface in .NET 10 GA — confirm against installed SDK at implementation.
- Whether to gate per-tenant independently — default: tenant-level setting inherits host toggle.
