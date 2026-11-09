# EAF — Session Security Hardening (W1)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Session Security Hardening (idle timeout, active sessions, 2FA provider cache) |
| Product / System | EAF Middleware + Api/Angular Templates |
| Module / Bounded Context | Security / Sessions |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-session-security` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W1 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

ASP.NET Zero added: session idle timeout (v15.5), active user sessions list/revocation (v15.2), per-user cached 2FA provider selection (v15.5). EAF has refresh-token sessions but no idle timeout, no session listing/revocation UI, and concurrent logins can overwrite each other's 2FA provider selection.

### Objective

Add configurable idle timeout, an "Active Sessions" page (list + revoke own/all sessions), and per-user (not per-device-shared) 2FA provider caching.

### Expected outcome

- Users are signed out after configurable inactivity.
- Users and admins see/revoke active sessions.
- Concurrent logins no longer clobber 2FA provider selection.

### Out of scope

- QR login (separate spec `eaf-qr-login.spec.md`).
- Device fingerprinting/geolocation display beyond IP + user agent.

## 2. Agent Role

Senior .NET/ABP engineer; reuse existing `RefreshTokenStore`/`TokenAuthConfiguration` infrastructure.

## 3. Agent Autonomy Level

**2 — Reliable**.

## 4. Product Context

`src/Eaf.Middleware.Web.Core/Authentication/JwtBearer/` already has `RefreshTokenStore`, `RefreshTokenInfo`, `TokenAuthConfiguration`, `JwtTokenMiddleware`. Extend rather than rebuild.

### Context files the agent must read before implementation

- `CLAUDE.md`
- `src/Eaf.Middleware.Web.Core/Authentication/JwtBearer/*`
- `src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs`
- Angular `account/` + `admin/` structure

## 5. Task Definition

### Main task

Idle timeout middleware, session registry/revocation, 2FA provider selection scoping.

### Subtasks

- Setting `SessionIdleTimeoutMinutes` (host/tenant, 0 = disabled); sliding validation on token/refresh use.
- Extend `RefreshTokenInfo` into a session record: session id, user, tenant, IP, user agent, created, last-activity, expiration.
- `ISessionAppService`: list my sessions, revoke one/all (keep current optionally); admin list/revoke per tenant.
- Token claims carry `session_id`; revocation = delete/invalidate store entry + deny refresh.
- 2FA: cache selected provider keyed by (user, login attempt/nonce) instead of single per-user slot — prevent concurrent overwrite.
- Angular: "Active Sessions" page in user menu; admin page under Administration; idle-timeout warning + auto logout UX.

### Do not do

- Do not break existing refresh-token rotation.
- Do not log full tokens.

## 6. Functional Requirements

### FR-001: Idle timeout

**Acceptance criteria:**
- [ ] Inactivity beyond setting invalidates the session (401 on next call/refresh).
- [ ] Activity slides the window; disabled (0) keeps current behavior.

### FR-002: Active sessions

**Acceptance criteria:**
- [ ] Page lists sessions with device/IP/last activity.
- [ ] Revoking kills refresh + subsequent access promptly (cache invalidation).
- [ ] Admin sees all tenant sessions; permission-gated.

### FR-003: 2FA provider cache

**Acceptance criteria:**
- [ ] Two concurrent logins for same user keep independent provider selections.
- [ ] Selection expires with the login attempt.

## 7. Business Rules

- BR-001: Session revocation never widens access; revoked sessions fail fast.
- BR-002: Idle timeout applies to interactive sessions; API-key auth (spec `eaf-api-key-authentication`) is exempt.
- BR-003: Revoking "all" may keep current session (UI option).

## 8–12. Domain / Persistence

Extend `RefreshTokenStore` schema → migration for session fields (`EafUserSessions` or extended store). Index `(UserId)`, `(SessionId)` unique.

## 16. Non-Functional Requirements

- Last-activity update cheap (cache write, no DB per request — throttle updates, e.g. ≥30s granularity).
- No sensitive payloads in session list responses.

## 18. Expected Tests

- Unit: sliding expiration math, revocation semantics, concurrent 2FA selection isolation.
- Integration: idle expiry across refresh boundary; revoke then reuse → 401.

## Pending Questions and Ambiguities

- Reuse `RefreshTokenStore` table vs new `EafUserSessions` — prefer extending existing store to avoid duplicate truth.
- Access-token (short-lived) behavior on revoked session — accept up to token lifetime or add denylist check (recommend denylist for admin revocations).
