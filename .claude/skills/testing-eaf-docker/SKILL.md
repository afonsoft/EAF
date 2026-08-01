---
name: testing-eaf-docker
description: How to run the complete end-to-end test of the EAF Docker full stack, including multi-tenancy, shared/unique users and cross-tenant SignalR chat.
metadata:
  version: '1.0.0'
---

# Testing EAF Docker Full Stack

## Scope
Use this skill when asked to end-to-end test the `afonsoft/EAF` repository, especially PRs that touch the Angular UI, middleware CORS, public errors, SignalR, multi-tenancy or tenant data isolation.

## Quick start

Set the required passwords as environment variables and start the full stack:

```bash
cd /home/ubuntu/repos/EAF
export MSSQL_SA_PASSWORD='<your-sql-sa-password>'
docker compose -f docker-compose.all.yml up -d --build
```

Verify the five containers are healthy:

```bash
docker ps --format 'table {{.Names}}\t{{.Status}}'
```

Expected: `eaf-sqlserver`, `eaf-migrator` (exited), `eaf-api`, `eaf-worker` and `eaf-angular` all healthy.

## Automated smoke test

A single Python script runs the whole scenario. It needs the admin password that will be used for all test users and, on the first run, the current initial admin password (for a fresh ABP seed this is the default sample password):

```bash
pip3 install signalrcore   # only needed for the chat step
cd /home/ubuntu/repos/EAF
export EAF_INITIAL_PASSWORD='<current-admin-password>'
export EAF_DEFAULT_PASSWORD='<desired-admin-password>'
python3 .claude/skills/testing-eaf-docker/scripts/eaf-fullstack-test.py
```

The script exercises:

1. API and Angular health checks.
2. Host admin login (resets the admin password on the first run).
3. Tenant CRUD: creates `tenantA` and `tenantB`.
4. Tenant admin login with `Abp-TenantId` header.
5. Enables `App.ChatFeature`, `App.ChatFeature.TenantToTenant` and `App.ChatFeature.GroupChat` for both tenants.
6. Creates a shared user (`shareduser`) in both tenants and unique users (`alice`, `bob`) in their own tenant.
7. Verifies login for each user/tenant and decodes the JWT `tenantid` claim.
8. Checks tenant data isolation (tenantA user list must not contain `bob`).
9. Creates a cross-tenant friendship from tenantA admin to tenantB admin.
10. Sends a SignalR chat message from tenantA to tenantB and verifies the receiver sees it persisted.

## Manual step-by-step routine

### 1. Start the full stack

```bash
cd /home/ubuntu/repos/EAF
export MSSQL_SA_PASSWORD='<your-sql-sa-password>'
docker compose -f docker-compose.all.yml up -d --build
```

### 2. Health and environment

```bash
curl -s -o /dev/null -w '%{http_code}\n' http://localhost:5000/AbpUserConfiguration/GetAll
curl -s -o /dev/null -w '%{http_code}\n' http://localhost:4200
```

### 3. Host admin first login

The seeded admin password forces a reset on the first login. Use the current initial password to authenticate and get the `passwordResetCode`:

```bash
export INITIAL_PASSWORD='<current-admin-password>'
export ADMIN_PASSWORD='<desired-admin-password>'

curl -s -X POST http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Content-Type: application/json' \
  -d "{\"userNameOrEmailAddress\":\"admin\",\"password\":\"$INITIAL_PASSWORD\",\"rememberClient\":false}"
```

Reset the password:

```bash
curl -s -X POST http://localhost:5000/api/services/app/Account/ResetPassword \
  -H 'Content-Type: application/json' \
  -d "{\"userId\":2,\"password\":\"$ADMIN_PASSWORD\",\"resetCode\":\"<passwordResetCode>\"}"
```

Then login with the new password:

```bash
curl -s -X POST http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Content-Type: application/json' \
  -d "{\"userNameOrEmailAddress\":\"admin\",\"password\":\"$ADMIN_PASSWORD\",\"rememberClient\":false}"
```

Decode the JWT payload (second dot-separated segment) and confirm `tenantid` is absent (host context).

### 4. Tenant management

Create two tenants:

```bash
TOKEN=<host-jwt>
TENANT_ADMIN_PASSWORD='<tenant-admin-password>'
curl -s -X POST http://localhost:5000/api/services/app/Tenant/CreateTenant \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"tenancyName\":\"tenantA\",\"name\":\"tenantA\",\"adminEmailAddress\":\"admin@tenantA.com\",\"adminPassword\":\"$TENANT_ADMIN_PASSWORD\",\"isActive\":true,\"shouldChangePasswordOnNextLogin\":false}"

curl -s -X POST http://localhost:5000/api/services/app/Tenant/CreateTenant \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"tenancyName\":\"tenantB\",\"name\":\"tenantB\",\"adminEmailAddress\":\"admin@tenantB.com\",\"adminPassword\":\"$TENANT_ADMIN_PASSWORD\",\"isActive\":true,\"shouldChangePasswordOnNextLogin\":false}"
```

List tenants:

```bash
curl -s -X GET 'http://localhost:5000/api/services/app/Tenant/GetTenants?MaxResultCount=10&SkipCount=0' \
  -H "Authorization: Bearer $TOKEN"
```

### 5. Single-tenant login

Login to `tenantA` and `tenantB` and decode the JWT `tenantid` claim:

```bash
curl -s -X POST http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 2' \
  -d "{\"userNameOrEmailAddress\":\"admin\",\"password\":\"$ADMIN_PASSWORD\",\"rememberClient\":false}"
```

### 6. Various users per tenant

As the tenant admin, create a shared user in both tenants and unique users in each tenant:

```bash
USER_PASSWORD='<test-user-password>'

# tenantA (Abp-TenantId: 2)
curl -s -X POST http://localhost:5000/api/services/app/User/CreateOrUpdateUser \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 2' \
  -H "Authorization: Bearer $tenantA_token" \
  -d "{\"assignedRoleNames\":[\"Admin\"],\"setRandomPassword\":false,\"sendActivationEmail\":false,\"user\":{\"userName\":\"shareduser\",\"name\":\"Shared\",\"surname\":\"User\",\"emailAddress\":\"shared@tenantA.com\",\"isActive\":true,\"password\":\"$USER_PASSWORD\",\"shouldChangePasswordOnNextLogin\":false}}"

curl -s -X POST http://localhost:5000/api/services/app/User/CreateOrUpdateUser \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 2' \
  -H "Authorization: Bearer $tenantA_token" \
  -d "{\"assignedRoleNames\":[\"Admin\"],\"setRandomPassword\":false,\"sendActivationEmail\":false,\"user\":{\"userName\":\"alice\",\"name\":\"Alice\",\"surname\":\"A\",\"emailAddress\":\"alice@tenantA.com\",\"isActive\":true,\"password\":\"$USER_PASSWORD\",\"shouldChangePasswordOnNextLogin\":false}}"
```

Repeat for `tenantB` with `shareduser` and `bob`.

### 7. User with access to two tenants

The same username (`shareduser`) can exist independently in multiple tenants. Login with the same credentials but different `Abp-TenantId` values:

```bash
curl -s -X POST http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 2' \
  -d "{\"userNameOrEmailAddress\":\"shareduser\",\"password\":\"$USER_PASSWORD\",\"rememberClient\":false}"

curl -s -X POST http://localhost:5000/api/TokenAuth/Authenticate \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 3' \
  -d "{\"userNameOrEmailAddress\":\"shareduser\",\"password\":\"$USER_PASSWORD\",\"rememberClient\":false}"
```

Each token has a different `sub` and a different `tenantid`, confirming that tenants are isolated identity stores.

### 8. Tenant data isolation

Login as `alice` in `tenantA`, fetch the user list and confirm `bob` (tenantB) is not present:

```bash
curl -s -X GET 'http://localhost:5000/api/services/app/User/GetUsers?MaxResultCount=100&SkipCount=0' \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 2' \
  -H "Authorization: Bearer $alice_token"
```

### 9. Cross-tenant chat

Enable chat features for both tenants (host context):

```bash
curl -s -X PUT http://localhost:5000/api/services/app/Tenant/UpdateTenantFeatures \
  -H 'Content-Type: application/json' \
  -H "Authorization: Bearer $host_token" \
  -d '{"id":2,"featureValues":[{"name":"App.ChatFeature","value":"true"},{"name":"App.ChatFeature.TenantToTenant","value":"true"},{"name":"App.ChatFeature.GroupChat","value":"true"}]}'
```

Create a friendship from tenantA admin to tenantB admin:

```bash
curl -s -X POST http://localhost:5000/api/services/app/Friendship/CreateFriendshipRequestByUserName \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 2' \
  -H "Authorization: Bearer $tenantA_token" \
  -d '{"tenancyName":"tenantB","userName":"admin"}'
```

Use the automated script or the `signalrcore` snippet below to send a SignalR message from tenantA to tenantB:

```python
from signalrcore.hub_connection_builder import HubConnectionBuilder
connection = HubConnectionBuilder().with_url(
    f"http://localhost:5000/signalr-chat?access_token={tenantA_token}"
).build()
connection.start()
time.sleep(2)
connection.send("SendMessage", [{
    "userId": 4,       # tenantB admin id
    "tenantId": 3,     # tenantB id
    "tenancyName": "tenantB",
    "userName": "admin",
    "message": "Hello from tenantA"
}])
```

Verify the receiver sees the message:

```bash
curl -s -X GET 'http://localhost:5000/api/services/app/Chat/GetUserChatMessages?UserId=3&TenantId=2' \
  -H 'Content-Type: application/json' \
  -H 'Abp-TenantId: 3' \
  -H "Authorization: Bearer $tenantB_token"
```

### 10. Cleanup

```bash
docker compose -f docker-compose.all.yml down -v
```

## Common test probes

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
Expected: `204` with `Access-Control-Allow-Origin`, `Access-Control-Allow-Methods` and `Access-Control-Allow-Headers`.

## UI automation

- Native `computer` mouse clicks on the Angular UI may not trigger buttons reliably because of the `busyIf` overlay and coordinate scaling. Use **Playwright Python** for UI automation and network capture.
- Use Chrome for Testing binary when launching headless Chromium:
  ```bash
  export CHROME_BIN=/opt/.devin/chrome/chrome/linux-133.0.6943.126/chrome-linux64/chrome
  ```
- Playwright `context.route`/`page.on('response')` can capture `Abp-TenantId` headers and decode JWTs to verify the tenant flow.

## Known gotchas
- The running DB may not match the requested sample passwords. If the admin login fails, verify the current `AbpUsers` password hash directly or reset it through `/api/services/app/Account/ResetPassword`.
- Tenant header/cookie is now `Abp-TenantId` (dash) everywhere: `EafHttpInterceptor`, `AppPreBootstrap`, `app-auth.service`, `eaf.js`, `MiddlewareControllerBase` and `EafCorsConfiguration`. The header is omitted when no tenant is selected to keep the host context; if you see `Abp-TenantId: null` in requests, a client is still using the old hardcoded header.
- The Angular app lazy-loads the account module. If the login page is blank after navigation, force a hard navigation with `window.location.replace('/account/login')` and wait for the chunk.
- `topbar.component.ts` depends on `appSessionService` being re-initialized after login; if it stays on the loading spinner, the session was not refreshed.
- Cross-tenant chat requires the tenant-level chat features to be enabled. If `CreateFriendshipRequestByUserName` returns `TenantToTenantChatFeatureIsNotEnabledForSender`, call `UpdateTenantFeatures` first.

## Devin Secrets Needed
None.
