---
name: testing-eaf-template
description: End-to-end test setup and verification for the EAF Api + Angular template registration flow, including public tenant creation, join requests and approvals.
---

# Testing the EAF Api + Angular template registration flow

## Scope

This skill covers the local end-to-end test setup for:

- `Templates/Api/src/Eaf.ProjectName.Web.Host`
- `Templates/Angular/Eaf.ProjectName.UI`

It focuses on the public tenant registration flow:

- Create a new tenant (linked to the `Free` edition)
- Register a user in an existing tenant (pending approval)
- Approve/reject join requests as tenant admin
- Tenant-scoped login and user lockout

## Devin Secrets Needed

None, but a local SQL Server container or instance is required.

## Local environment variables

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ASPNETCORE_URLS=http://+:8001
export Database__Provider=SqlServer
export ConnectionStrings__Default="Server=127.0.0.1,1433;Database=EafRegTest;User Id=sa;Password=<SA_PASSWORD>;TrustServerCertificate=True;MultipleActiveResultSets=True;"
export SqlServerCache__IsEnabled=false
```

> The `EAF_` prefix can also be used for configuration overrides if the harness sets other provider defaults.

## Backend setup

1. Ensure a SQL Server instance is reachable. Example Docker command:

   ```bash
   docker run -d --name eaf-mssql-test -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<SA_PASSWORD>" -p 1433:1433 mcr.microsoft.com/mssql/server:2022-latest
   ```

2. Build the API project:

   ```bash
   cd Templates/Api
   dotnet build Eaf.ProjectName.sln -c Release
   ```

3. Run migrations:

   ```bash
   cd src/Eaf.ProjectName.EntityFrameworkCore
   dotnet ef database update --startup-project ../Eaf.ProjectName.Web.Host
   ```

4. Run the host:

   ```bash
   cd src/Eaf.ProjectName.Web.Host
   dotnet run --no-build -c Release
   ```

## Angular setup

```bash
cd Templates/Angular/Eaf.ProjectName.UI
nvm use 22
npm install --legacy-peer-deps
```

Create/update `proxy.conf.json`:

```json
{
  "/api": { "target": "http://localhost:8001", "secure": false, "changeOrigin": true, "logLevel": "debug" },
  "/AbpUserConfiguration": { "target": "http://localhost:8001", "secure": false, "changeOrigin": true, "logLevel": "debug" },
  "/signalr": { "target": "http://localhost:8001", "secure": false, "changeOrigin": true, "logLevel": "debug" }
}
```

Update `src/assets/appconfig.json`:

```json
{
  "remoteServiceBaseUrl": "http://localhost:8001",
  "appBaseUrl": "http://localhost:8000"
}
```

Run the dev server:

```bash
npx ng serve --host localhost --port 8000 --proxy-config proxy.conf.json
```

## Known runtime issues and workarounds

- **Angular `ng build` / `ng serve` is slow on low-resource VMs.** If the build exceeds the available time, run a TypeScript-only check to catch compile errors before a full build:

  ```bash
  npx tsc -p src/tsconfig.app.json --noEmit
  ```

- **Angular dev server source-map JSON parse error** (`Unexpected token '﻿'`) may be caused by a BOM in a webpack source map. A temporary guard can be added to `node_modules/@angular-devkit/build-angular/src/tools/webpack/plugins/devtools-ignore-plugin.js`.

## Verification commands

Create a new tenant (API):

```bash
curl -s -X POST http://localhost:8001/api/services/app/Account/Register \
  -H "Content-Type: application/json" \
  -d '{"tenantSelectionMode":"CreateNew","tenancyName":"testtenant","tenantName":"Test Tenant","userName":"testadmin","name":"Test","surname":"Admin","emailAddress":"test@example.com","password":"P@ssw0rd123"}'
```

Join an existing tenant:

```bash
curl -s -X POST http://localhost:8001/api/services/app/Account/Register \
  -H "Content-Type: application/json" \
  -d '{"tenantSelectionMode":"JoinExisting","existingTenantId":2,"joinRequestMessage":"Please approve","userName":"newuser","name":"New","surname":"User","emailAddress":"new@example.com","password":"P@ssw0rd123"}'
```

List tenants available for public registration:

```bash
curl -s http://localhost:8001/api/services/app/TenantJoinRequest/GetAvailableTenants
```

Select a tenant after host login:

```bash
curl -s -X POST http://localhost:8001/api/TokenAuth/SelectTenant \
  -H "Content-Type: application/json" \
  -d '{"userNameOrEmailAddress":"<user>","password":"<pass>","tenantId":2}'
```

Or log in directly to a tenant:

```bash
curl -s -X POST http://localhost:8001/api/TokenAuth/Authenticate \
  -H "Content-Type: application/json" \
  -d '{"userNameOrEmailAddress":"<user>","password":"<pass>","tenancyName":"testtenant","rememberClient":true}'
```

## Functional test script

The script `/tmp/eaf_functional_test.py` (generated during the original implementation) automates:

1. Host admin login
2. Public creation of a new tenant
3. Tenant admin login with `tenancyName`
4. Create inactive tenant user
5. Activate tenant user
6. Tenant user login
7. Block user and verify login fails
8. Join-existing request
9. List pending join requests as tenant admin
10. Approve join request
11. Approved user login
12. Verify new tenant has `EditionId = 1` (`Free` edition)

## Notes

- The registration settings `AllowSelfRegistration`, `AllowTenantCreation` and `AllowJoinRequests` can be toggled in **Admin > Settings > Tenant management** (host or tenant scope).
- `TenantJoinRequestAppService` is exposed automatically by `MiddlewareWebCoreModule` (`CreateControllersForAppServices` over `Eaf.Middleware.Application`).
