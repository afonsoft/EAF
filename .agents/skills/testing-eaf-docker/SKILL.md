---
name: testing-eaf-docker
description: How to run end-to-end tests against the EAF Docker full stack (API + Angular + SQL Server + Worker).
---

# Testing EAF Docker Full Stack

## Scope
Use this skill when asked to end-to-end test the `afonsoft/EAF` repository, especially PRs that touch the Angular UI, middleware CORS, public errors, SignalR, or multi-tenancy.

## Quick start
```bash
cd /home/ubuntu/repos/EAF
export MSSQL_SA_PASSWORD='EafDocker2026!'
docker compose -f docker-compose.all.yml up -d --build
```

Verify health:
```bash
curl -s http://localhost:5000/AbpUserConfiguration/GetAll > /dev/null && echo "API OK"
curl -s http://localhost:4200 > /dev/null && echo "Angular OK"
```

## Endpoints
- API: `http://localhost:5000`
- Angular: `http://localhost:4200`
- SignalR chat hub: `http://localhost:5000/signalr-chat`
- CORS origins configured in `docker-compose.all.yml`: `http://localhost:4200`

## Common test probes

### Login and JWT claims
```bash
# Host login
curl -s -X POST http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Content-Type: application/json' \
  -d '{"userNameOrEmailAddress":"admin","password":"TenantPass123!","rememberClient":false}'

# Tenant login
curl -s -X POST http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 2' \
  -d '{"userNameOrEmailAddress":"admin","password":"TenantPass123!","rememberClient":false}'
```
Decode the token payload with `base64` and look for `tenantid`.

### Public error contract
```bash
curl -s -X POST http://localhost:5000/api/TokenAuth/GetAvailableTenants \
  -H 'Content-Type: application/json' \
  -d '{"userNameOrEmailAddress":"admin","password":"wrong"}'
```
Expected: `400` JSON with `code: validation_failed`.

### SignalR negotiate
```bash
TOKEN=<valid-jwt>
curl -s -X POST "http://localhost:5000/signalr-chat/negotiate?access_token=$TOKEN"
```
Expected: `200` JSON with `connectionId` and `availableTransports`.

### CORS preflight
```bash
curl -s -X OPTIONS http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Origin: http://localhost:4200' \
  -H 'Access-Control-Request-Method: POST' \
  -H 'Access-Control-Request-Headers: Content-Type,Abp-TenantId'
```
Expected: `204` with `Access-Control-Allow-Origin`, `Access-Control-Allow-Methods`, and `Access-Control-Allow-Headers`.

## Known gotchas
- The running DB may not match the requested sample passwords. If `admin/NewPass123!` fails, try `admin/TenantPass123!` or check `AbpUsers` directly.
- Tenant header/cookie is now `Abp-TenantId` (dash) everywhere: `EafHttpInterceptor`, `AppPreBootstrap`, `app-auth.service`, `eaf.js`, `MiddlewareControllerBase` and `EafCorsConfiguration`. The header is omitted when no tenant is selected to keep the host context; if you see `Abp-TenantId: null` in requests, a client is still using the old hardcoded header.
- The Angular app lazy-loads the account module. If the login page is blank after navigation, force a hard navigation with `window.location.replace('/account/login')` and wait for the chunk.
- `topbar.component.ts` depends on `appSessionService` being re-initialized after login; if it stays on the loading spinner, the session was not refreshed.

## Devin Secrets Needed
None.
