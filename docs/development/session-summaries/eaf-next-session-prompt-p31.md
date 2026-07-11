# EAF Next Session Prompt — Priority 31 Coverage Audit

**Goal:** Continue the test coverage audit for `afonsoft/EAF` by targeting the remaining heavy, low-coverage classes, then open a PR to `main` and ensure coverage does not decrease from the P30 final baseline.

**Baseline (P30):** Line 81.3%, Branch 60.6%, Method 93.7%.

## 1. Context

- Repository: `afonsoft/EAF`
- Base branch: `main`
- Branch naming: `devin/<timestamp>-priority31-coverage-audit`
- Test stack: xUnit + Shouldly + NSubstitute
- Language for docs/test names: Portuguese (`Dado_..._Quando_..._Entao_...`)
- Commit message template: `test: priority 31 coverage audit — cover remaining low-coverage paths`

## 2. Remaining high-value targets

### 2.1 Web.Core controllers, middleware and SignalR

- `Eaf.Middleware.Web.Controllers.TokenAuthController` (0%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (45.5%)
- `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)

### 2.2 Identity

- `Eaf.Middleware.Identity.LogInManager` (0%)
- `Eaf.Middleware.Identity.SecurityStampValidator` (0%)
- `Eaf.Middleware.Identity.SignInManager` (0%)

### 2.3 Host builder, startup and configurers

- `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (53.4%) — remaining branches
- `Eaf.Middleware.Web.Startup.AuthConfigurer` (69.3%) — remaining branches
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (60.4%) — remaining branches
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (60.7%) — remaining branches

### 2.4 Key vault, cache and logging

- `Eaf.KeyVault.OCIKeyVaultManager` (19.2%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%) — remaining branches
  - The real `QueueClient` send path still creates a live client. Consider an integration guard or test the branch with a valid-looking dummy connection string.

### 2.5 Observability

- `Eaf.OpenTelemetry` (71.4%)
  - `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (57.5%) — remaining branches

### 2.6 External authentication providers

- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (17.4%) — remaining branches

### 2.7 Hangfire workers

- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker` (85%) — remaining branches
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%) — remaining branches

## 3. Constraints

- Do not modify production code except to fix real blocking bugs.
- Do not edit `.github/workflows/`.
- Do not push directly to `main` or `develop`.
- Never reduce coverage relative to the P30 baseline.

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
2. All tests green and coverage not lower than P30.
3. PR to `main`.
4. `docs/development/session-summaries/eaf-session-summary-p31.md`
5. `docs/development/session-summaries/eaf-next-session-prompt-p32.md` (if further work remains).
6. Updated `.agents/MEMORY.md` with new coverage numbers and mocking gotchas.
