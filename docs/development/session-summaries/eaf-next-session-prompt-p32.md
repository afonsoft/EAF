# EAF Next Session Prompt — Priority 32 Coverage Audit

**Goal:** Continue the test coverage audit for `afonsoft/EAF` by targeting the remaining low-coverage classes and branches after P31, then open a PR to `main` and ensure coverage does not decrease from the P31 final baseline.

**Baseline (P31):** Line 83.4%, Branch 62.6%, Method 94.3%.

## 1. Context

- Repository: `afonsoft/EAF`
- Base branch: `main`
- Branch naming: `devin/<timestamp>-priority32-coverage-audit`
- Test stack: xUnit + Shouldly + NSubstitute
- Language for docs/test names: Portuguese (`Dado_..._Quando_..._Entao_...`)
- Commit message template: `test: priority 32 coverage audit — cover remaining low-coverage paths`

## 2. Remaining high-value targets

### 2.1 Web.Core controllers and middleware

- `Eaf.Middleware.Web.Controllers.TokenAuthController` (11.7%) — remaining branches for `Authenticate`, `ExternalAuthenticate`, `ImpersonatedAuthenticate`, `SwitchedAccountAuthenticate`, and `SendTwoFactorAuthCode`.
- `Eaf.Middleware.Web.Controllers.AntiForgeryController` (57.1%) — remaining validation and cookie branches.
- `Eaf.Middleware.Web.Controllers.ProfileControllerBase` (77.3%) — remaining update/change password branches.
- `Eaf.Middleware.Web.Controllers.FileController` (89.1%) — remaining upload/download branches.
- `Eaf.Middleware.Web.Controllers.MiddlewareControllerBase` (90%) — remaining exception handling branches.
- `Eaf.Middleware.Web.Controllers.ChatControllerBase` (88.2%) — remaining helper branches.
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%) — remaining `PostInitialize` branches (BackgroundJobs, Hangfire, Redis, SignalR).
- `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (77.4%) — remaining token validation and refresh branches.
- `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter` (76.4%) — remaining operation/parameter filtering branches.

### 2.2 Host builder, startup and configurers

- `Eaf.Middleware.Web.Configuration.EafHostBuilderExtensions` (83.3%) — remaining branches.
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (60.4%) — remaining branches.
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (60.7%) — remaining branches.
- `Eaf.Middleware.Web.Startup.HangFireConfigurer` (77.5%) — remaining storage branches.
- `Eaf.Middleware.Web.Startup.RedisConfigurer` (84.6%) — remaining branches.
- `Eaf.Middleware.Worker.EafCastleWindsorHostBuilderExtensions` (66.6%) — remaining branches.
- `Eaf.Middleware.Core.Configuration.EafHostBuilderExtensions` (66.6%) — remaining branches.

### 2.3 Key vault, cache and logging

- `Eaf.KeyVault.OCIKeyVaultManager` (25.3%) — remaining `GetSecret`, `ListSecrets`, `CreateSecret`, and exception handling branches.
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%) — remaining `Append` and `SendMessage` branches.
- `Eaf.SqliteCache.DbCommandPool` (69.7%) — remaining initialization and cleanup branches.
- `Eaf.SqliteCache.EafSqliteCache` (77.5%) — remaining cache expiration and serialization branches.

### 2.4 Core and application services

- `Eaf.Middleware.Core.Authorization.AuthorizationExtensions` (50%) — remaining branches.
- `Eaf.Middleware.Core.Localization.CultureHelper` (78.5%) — remaining branches.
- `Eaf.Middleware.Core.Friendships.Cache.UserFriendsCache` (87.2%) — remaining branches.
- `Eaf.Middleware.Application.MultiTenancy.TenantAppService` (79.6%) — remaining CRUD and feature branches.
- `Eaf.Middleware.Application.Authorization.Users.ProfileAppService` (81.3%) — remaining update/picture branches.
- `Eaf.Middleware.Application.Configuration.UiCustomizationSettingsAppService` (69.3%) — remaining theme branches.
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (70.8%) — remaining exception and path branches.

### 2.5 Observability

- `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (71.2%) — remaining `AddEafOpenTelemetry` and `MapEafOpenTelemetryMetrics` branches.

### 2.6 External authentication providers

- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (17.4%) — remaining `GetUserInfo` and `GetClaimsFromPayload` branches.

## 3. Constraints

- Do not modify production code except to fix real blocking bugs.
- Do not edit `.github/workflows/`.
- Do not push directly to `main` or `develop`.
- Never reduce coverage relative to the P31 baseline.

## 4. Verification commands

```bash
dotnet build Eaf.sln --configuration Release
bash run-tests-with-coverage.sh
```

The coverage script requires:

```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## 5. Deliverables

1. BDD tests for the targets above.
2. All tests green and coverage not lower than P31.
3. PR to `main`.
4. `docs/development/session-summaries/eaf-session-summary-p32.md`
5. `docs/development/session-summaries/eaf-next-session-prompt-p33.md` (if further work remains).
6. Updated `.agents/MEMORY.md` with new coverage numbers and mocking gotchas.
