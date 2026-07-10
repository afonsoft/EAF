# EAF Next Session Prompt — Priority 27 Coverage Audit

**Goal:** Continue the test coverage audit for `afonsoft/EAF` by adding BDD unit tests for the remaining low-coverage classes from P26, then open a PR to `main` and ensure coverage does not decrease from the P26 final baseline.

**Baseline (P26):** Line 68.0%, Branch 52.1%, Method 86.4%.

## 1. Context

- Repository: `afonsoft/EAF`
- Base branch for new work: `main`
- Branch naming: `devin/<timestamp>-priority27-coverage-audit`
- Test stack: xUnit + Shouldly + NSubstitute
- Language for docs/test names: Portuguese (`Dado_..._Quando_..._Entao_...`)
- Commit message template: `test: priority 27 coverage audit — cover remaining core, application and module low-coverage paths`

## 2. Remaining high-value targets

### 2.1 Module classes (integration-style smoke tests)

These require a real ABP bootstrapper or module initialization; prefer focused smoke tests that call `PreConfigureServices`/`ConfigureServices`/`PostConfigureServices` with a real `ServiceCollection`.

- `Eaf.Middleware.MiddlewareCoreModule` (0%)
- `Eaf.Middleware.MiddlewareApplicationModule` (0%)
- `Eaf.Middleware.Web.Core.MiddlewareWebCoreModule` (not shown, but still low if not covered)
- `Eaf.Middleware.Worker.MiddlewareWorkerModule` (16.4%)
- `Eaf.AspNetCore.Hangfire.Configuration.EafHangfireApplicationBuilderExtensions` (0%)
- `Eaf.Configuration.EafWebHostBuilderExtensions` (0%)
- `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (0%)

### 2.2 Worker full pipeline

- `Eaf.Middleware.Worker.EafServiceCollectionExtensions` (0%)
  - The `AddEaf`/`AddEafWithoutCreatingServiceProvider` methods bootstrap a full `AbpBootstrapper`.
  - Options:
    1. Add an integration-style test that uses a dedicated in-memory `TestModule : AbpModule` and a fresh Windsor container, ensuring `AbpBootstrapper` does not reuse a static container.
    2. If static state conflicts, wrap calls in a separate `AppDomain`/process or use a minimal reflection-based smoke test that only verifies the public API surface.
  - Do not let a single failing bootstrapper test break the others.

### 2.3 Webhooks / notifications

- `Eaf.Notifications.EmailRealTimeNotifier` (0%)
- `Eaf.WebHooks.EafWebHookReceiver` (0%)
- `Eaf.Middleware.Web.WebHooks.EafWebhookDefinitionProvider` (0%)

### 2.4 UI customizers

- `Eaf.Middleware.Web.UiCustomization.Metronic.Theme2UiCustomizer` (0%)
- `Eaf.Middleware.Web.UiCustomization.Metronic.Theme3UiCustomizer` (0%)
- `Eaf.Middleware.Web.UiCustomization.Metronic.Theme4UiCustomizer` (0%)
- `Eaf.Middleware.Web.UiCustomization.Metronic.ThemeDefaultUiCustomizer` (0%)
- `Eaf.Middleware.Web.UiCustomization.Metronic.UiThemeCustomizerBase` (0%)

### 2.5 Remaining classes to bring above 50%

- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (38.5%)
  - The main `AppendBuffer` branch with a valid connection string creates a real `QueueClient` and sends messages. Consider a safe integration guard or refactor the production method to accept an `IQueueClient` factory. **Do not modify production code without explicit confirmation.** If refactoring is needed, propose it to the user first.

## 3. Constraints

- Do not modify production code except to fix real blocking bugs.
- Do not edit `.github/workflows/`.
- Do not push directly to `main` or `develop`.
- Never reduce coverage relative to the P26 baseline.

## 4. Verification commands

```bash
dotnet build Eaf.sln --configuration Release
bash run-tests-with-coverage.sh
```

The coverage script requires:

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

If `reportgenerator` is missing, install it with:

```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

## 5. Deliverables

1. BDD tests for the targets above.
2. All tests green and coverage not lower than P26.
3. PR to `main`.
4. `docs/development/session-summaries/eaf-session-summary-p27.md`
5. `docs/development/session-summaries/eaf-next-session-prompt-p28.md` (if further work remains).
6. Updated `.agents/MEMORY.md` with new coverage numbers and mocking gotchas.

## 6. Hints from P26

- Use `new HostBuilder()` and real `IConfigurationBuilder` with `AddInMemoryCollection` for extension method tests; NSubstitute `IConfigurationSection` does not work with `Microsoft.Extensions.Configuration` extension methods.
- `EafWorkerBase` is abstract; create a private `TestWorker` that implements `protected override Task ExecuteAsync(CancellationToken)`.
- `IHubClients<IClientProxy>` is the correct type for SignalR chat tests; mock `IClientProxy` and assert via `SendCoreAsync`.
- `ILocalizationSource` lives in `Abp.Localization.Sources`.
- `IObjectMapper` default in `EafWorkerBase` is `NullObjectMapper`.
