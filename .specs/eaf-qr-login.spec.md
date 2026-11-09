# EAF — QR Login (W4)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | QR Code Login |
| Product / System | EAF Middleware + Angular Template |
| Module / Bounded Context | Security / Authentication |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-qr-login` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W4 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

ASP.NET Zero supports QR login (since 14.1) — an authenticated session (e.g. mobile) approves a browser session by scanning a QR code. EAF has none.

### Objective

Generate short-lived login tokens rendered as QR on the login screen; an already-authenticated client scans and approves; the browser session is established with the IP/user-agent of the displaying browser (v15.5 fix semantics).

### Expected outcome

- Optional "Login with QR" on login page.
- Authenticated user approves the session from another device.
- Session binds to the displaying browser's IP + user agent.

### Out of scope

- Mobile template integration (React-Native template is empty).
- WebAuthn/passkey (separate spec).

## 2–4. Context

Reuse SignalR (`Eaf.SignalR`) for real-time approval push to the waiting browser, with polling fallback. Read `TokenAuthController`, `JwtBearer/` infra, `Eaf.SignalR` module.

## 5. Task Definition

- `QrLogin` token entity: one-time, TTL ≤ 2 min, states pending/approved/consumed/expired.
- Endpoints: `QrLogin/Create` (anonymous, returns token id), `QrLogin/Status` (poll) , `QrLogin/Approve` (authenticated, from scanning device).
- On approval: issue JWT/refresh bound to displaying browser's IP + UA (record both).
- Angular: QR image on login page (canvas, regenerated on expiry), approve-confirmation page for the scanner.
- Audit login attempts (QR path marked).

### Do not do

- Do not issue session to the scanning device.
- Do not allow replay (token single-use, server-side state only).
- Do not embed credentials inside the QR payload itself (QR carries only the token id).

## 6. Functional Requirements

- [ ] Unauthenticated browser displays QR; approval → browser session established.
- [ ] Expired/unapproved token → cannot authenticate.
- [ ] Session IP/UA = displaying browser, not scanner.
- [ ] Feature flag/setting to disable.

## 12. Persistence and Data

`EafQrLoginTokens` table or cache-backed store (prefer distributed cache — Redis/SqlServerCache modules exist — over a table).

## 18. Expected Tests

- Unit: token lifecycle transitions, binding rules.
- Integration: create → approve → status → tokens issued; expiry; single-use.

## Pending Questions and Ambiguities

- Transport for the waiting browser: SignalR (preferred, module exists) vs polling — support polling fallback regardless.
