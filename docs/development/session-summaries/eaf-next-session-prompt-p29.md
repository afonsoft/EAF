# EAF Next Session Prompt — Priority 29 Coverage Audit

**Goal:** Continue the test coverage audit for `afonsoft/EAF` by targeting the remaining low-coverage classes, then open a PR to `main` and ensure coverage does not decrease from the P28 final baseline.

**Baseline (P28):** Line 78.3%, Branch 56.1%, Method 92.8%.

## 1. Context

- Repository: `afonsoft/EAF`
- Base branch: `main`
- Branch naming: `devin/<timestamp>-priority29-coverage-audit`
- Test stack: xUnit + Shouldly + NSubstitute
- Language for docs/test names: Portuguese (`Dado_..._Quando_..._Entao_...`)
- Commit message template: `test: priority 29 coverage audit — cover remaining low-coverage paths`

## 2. Remaining high-value targets

### 2.1 Web.Core controllers and middleware

- `Eaf.Middleware.Web.Controllers.TokenAuthController` (0%)
- `Eaf.Middleware.Web.Controllers.FileController` (8.6%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (45.5%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (70.8%)
- `Eaf.Middleware.Web.Configuration.SqlServerCacheConfigurer` (66.6%)
- `Eaf.Middleware.Web.Startup.AuthConfigurer` (67.3%)
- `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (53.4%)

### 2.2 Host builder and startup extensions

- `Eaf.Middleware.Configuration.EafHostBuilderExtensions` (66.6%) — `MiddlewareCore`
- `Eaf.Middleware.Configuration.HostingEnvironmentExtensions` (50%) — `MiddlewareCore`
- `Eaf.Middleware.Web.Configuration.EafHostBuilderExtensions` (83.3%) — remaining branches
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (45.3%)
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (45.2%) — `Worker`
- `Eaf.Middleware.Worker.EafHostBuilderExtensions` (40.7%)
- `Eaf.Middleware.Worker.EafWorkerBase` (71.6%)

### 2.3 Identity and Hangfire

- `Eaf.Middleware.Identity.LogInManager` (0%)
- `Eaf.Middleware.Identity.SecurityStampValidator` (0%)
- `Eaf.Middleware.Identity.SignInManager` (0%)
- `Eaf.AspNetCore.Hangfire.EafHangfireAuthorizationFilter` (6.2%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker` (65.6%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (9.8%)

### 2.4 External authentication providers

- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (11.1%)
- `Eaf.Middleware.Core.Authentication.External.Microsoft.MicrosoftAuthProviderApi` (79.1%) — remaining branches

### 2.5 SignalR and cache

- `Eaf.AspNetCore.SignalR.Chat.ChatHub` (45.8%)
- `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)

### 2.6 Key vault, cache and logging modules

- `Eaf.KeyVault.OCIKeyVaultManager` (19.2%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (38.5%)
  - The main `AppendBuffer` branch with a valid connection string creates a real `QueueClient` and sends messages. Consider a safe integration guard or propose a factory refactor before changing production code.

### 2.7 Observability

- `Eaf.OpenTelemetry` (70.1%)
  - `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (55.6%)

## 3. Constraints

- Do not modify production code except to fix real blocking bugs.
- Do not edit `.github/workflows/`.
- Do not push directly to `main` or `develop`.
- Never reduce coverage relative to the P28 baseline.

## 4. Verification commands

```bash
dotnet build Eaf.sln --configuration Release
bash run-tests-with-coverage.sh
```

The coverage script requires:

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## 5. Deliverables

1. BDD tests for the targets above.
2. All tests green and coverage not lower than P28.
3. PR to `main`.
4. `docs/development/session-summaries/eaf-session-summary-p29.md`
5. `docs/development/session-summaries/eaf-next-session-prompt-p30.md` (if further work remains).
6. Updated `.agents/MEMORY.md` with new coverage numbers and mocking gotchas.
