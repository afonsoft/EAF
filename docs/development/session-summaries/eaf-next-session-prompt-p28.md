# EAF Next Session Prompt — Priority 28 Coverage Audit

**Goal:** Continue the test coverage audit for `afonsoft/EAF` by targeting the remaining low-coverage classes, then open a PR to `main` and ensure coverage does not decrease from the P27 final baseline.

**Baseline (P27):** Line 75.6%, Branch 54.0%, Method 90.8%.

## 1. Context

- Repository: `afonsoft/EAF`
- Base branch: `main`
- Branch naming: `devin/<timestamp>-priority28-coverage-audit`
- Test stack: xUnit + Shouldly + NSubstitute
- Language for docs/test names: Portuguese (`Dado_..._Quando_..._Entao_...`)
- Commit message template: `test: priority 28 coverage audit — cover remaining low-coverage paths`

## 2. Remaining high-value targets

### 2.1 Web.Core controllers and middleware

- `Eaf.Middleware.Web.Controllers.TokenAuthController` (0%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (45.5%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (0%)
- `Eaf.Middleware.Web.Configuration.CacheConfigurer` (41.1%)
- `Eaf.Middleware.Web.Swagger.SwaggerExtensions` (33.3%)

### 2.2 Host builder and startup extensions

- `Eaf.Middleware.Configuration.EafHostBuilderExtensions` (40.7%)
- `Eaf.Middleware.Web.Configuration.EafHostBuilderExtensions` (0%)
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (18.6%)
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (19%)
- `Eaf.Hangfire.EafDisplayNameExtensions` (0%)
- `Eaf.Hangfire.EafHangfireConfigurationExtensions` (0%)

### 2.3 External authentication providers

- `Eaf.Middleware.Core.Authentication.External.AuthZero.AuthZeroAuthProviderApi` (0%)
- `Eaf.Middleware.Core.Authentication.External.Google.GoogleAuthProviderApi` (0%)
- `Eaf.Middleware.Core.Authentication.External.Microsoft.MicrosoftAuthProviderApi` (0%)
- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (0%)

### 2.4 Identity and email

- `Eaf.Middleware.Identity.LogInManager` (0%)
- `Eaf.Middleware.Identity.SecurityStampValidator` (0%)
- `Eaf.Middleware.Identity.SignInManager` (0%)
- `Eaf.Middleware.Net.Emailing.MiddlewareMailKitSmtpBuilder` (42.8%)
- `Eaf.Middleware.Net.Emailing.MiddlewareSmtpEmailSenderConfiguration` (42.8%)

### 2.5 SignalR and health checks

- `Eaf.AspNetCore.SignalR.Chat.ChatHub` (0%)
- `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)
- `Microsoft.AspNetCore.Builder.EafHealthCheckApplicationBuilderExtensions` (0%)

### 2.6 Cache and key vault modules

- `Eaf.KeyVault.EafKeyVaultModule` (0%)
- `Eaf.KeyVault.OCIKeyVaultManager` (19.2%)
- `Eaf.KeyVault.AspNetCore.EafKeyVaultAspNetCoreModule` (0%)
- `Eaf.Runtime.Caching.Sqlite.EafSqliteCacheModule` (0%)
- `Eaf.Runtime.Caching.SqlServer.OracleCacheConfigurationExtensions` (0%)

### 2.7 ServiceBus appender

- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (38.5%)
  - The main `AppendBuffer` branch with a valid connection string creates a real `QueueClient` and sends messages. Consider a safe integration guard or propose a factory refactor before changing production code.

## 3. Constraints

- Do not modify production code except to fix real blocking bugs.
- Do not edit `.github/workflows/`.
- Do not push directly to `main` or `develop`.
- Never reduce coverage relative to the P27 baseline.

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
2. All tests green and coverage not lower than P27.
3. PR to `main`.
4. `docs/development/session-summaries/eaf-session-summary-p28.md`
5. `docs/development/session-summaries/eaf-next-session-prompt-p29.md` (if further work remains).
6. Updated `.agents/MEMORY.md` with new coverage numbers and mocking gotchas.
