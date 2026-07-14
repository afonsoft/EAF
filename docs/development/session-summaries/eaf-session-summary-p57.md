# EAF Session Summary P57 - Coverage Audit

## Data

14 de julho de 2026

## Branch

`feature/devin-20260713-priority57-coverage-audit`

## Baseline P56

| Metric | Value |
|--------|-------|
| Line coverage | 97.6% (13273 / 13589) |
| Branch coverage | 87.2% (2502 / 2868) |
| Method coverage | 99.6% (2155 / 2162) |
| Tests | 4516 total, 4515 passing, 1 skipped |
| Build warnings | 154 |

## Final P57

| Metric | Value |
|--------|-------|
| Line coverage | 97.7% (13280 / 13589) |
| Branch coverage | 87.5% (2510 / 2868) |
| Method coverage | 99.6% (2155 / 2162) |
| Tests | 4533 total, 4532 passing, 1 skipped |
| Build warnings | 154 |

## Changes

- Adicionados testes BDD para ramos acessíveis das classes de baixa cobertura do P57.
- Cobertura de ramo (branch) subiu de 87.2% para 87.5% e de linha de 97.6% para 97.7%.
- `Eaf.Middleware.Application`, `Eaf.Middleware.Core` e `Eaf.OpenTelemetry` chegaram a 100% de cobertura de linha.
- Nenhum código de produção foi alterado.
- Nenhum arquivo `.github/workflows/` foi modificado.

## New / updated tests

| Class | Assembly | Test file | Focus |
|-------|----------|-----------|-------|
| `AccountAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Authorization/Accounts/AccountAppServiceBddTests.cs` | `Impersonate` com tenant inativo; `TenantIdIsNotActive` |
| `RoleAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Authorization/Roles/RoleAppServiceBddTests.cs` | Filtro por permissão em `GetRoles`; `DeleteRole` sem usuários |
| `TenantAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/MultiTenancy/TenantAppServiceBddTests.cs` | `GetTenantFeaturesForEdit` com escopo `FeatureScopes.Tenant`; mapper de `FlatFeatureDto` |
| `AzureActiveDirectoryAuthenticationSource<T1, T2>` | Eaf.Middleware.AzureActiveDirectory | `test/Eaf.Middleware.AzureActiveDirectory.Tests/AzureActiveDirectory/Authentication/AzureActiveDirectoryAuthenticationSourceBddTests.cs` | `CreateUserAsync`/`UpdateUserAsync` com `AbpException` no Graph |
| `ProfileControllerBase` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Controllers/ProfileControllerBaseBddTests.cs` | `GetProfilePictureByUser` com `ModelState` inválido |
| `EafHangfireApplicationBuilderExtensions` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Hangfire/EafHangfireApplicationBuilderExtensionsBddTests.cs` | `UseEafHangfire()` sem `optionsAction` |
| `ChatHub` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/SignalR/Chat/ChatHubBddTests.cs` | `DeleteMessage` com `SharedMessageId` nulo; `SendMessage` com `UserId`/`GroupId` zero |
| `EafWorkerBase` | Eaf.Middleware.Worker | `test/Eaf.Middleware.Worker.Tests/Worker/EafWorkerBaseBddTests.cs` | `L` com `args` nulo; `LocalizationManager` nulo; cache de `LocalizationSource` |
| `EafOpenTelemetryServiceCollectionExtensions` | Eaf.OpenTelemetry | `test/Eaf.OpenTelemetry.Tests/EafOpenTelemetryServiceCollectionExtensionsBddTests.cs` | `AddEafOpenTelemetry()` sem opções e sem exporters |
| `ServiceBusQueueAppender` | Eaf.Log4NetServiceBus | `test/Eaf.Log4NetServiceBus.Tests/Logging/ServiceBusQueueAppenderBddTests.cs` | `SendBuffer` com `StorageType` vazio |

## Notes

- `LdapAuthenticationSource` (59.2%), `MiddlewareWebCoreModule` (87.3%), `TokenAuthController` (90.9%), `PermissionAppService` (92.5%), `ServiceBusQueueAppender` (92.8%), `AzureActiveDirectoryAuthenticationSource` (93.5%), `OpenIdConnectAuthProviderApi` (95.2%), `EafHangfireApplicationBuilderExtensions` (96.7%), `EafHangfireAuthorizationFilter` (97.7%), `MiddlewareAppServiceBase` (97.3%), `EafSqlServerCache` (97.4%), `ChatHub` (97.4%), `EafOpenTelemetryServiceCollectionExtensions` (97.2% → 100%), `EafServiceCollectionExtensions` (98.1%), `TenantAppService` (98.4%), `HostSettingsAppService` (99.3%) e `ChatMessageManager` (99.1%) têm ramos inacessíveis no Linux ou dependentes de infraestrutura real (LDAP, Redis, Hangfire/SignalR, MSAL) e não devem ser alterados sem revisão.
- Build Release: 0 erros, 154 warnings. Todos os testes passam sem regressão de cobertura.

## Verification

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

Build: 0 errors, 154 warnings. All tests pass with no coverage regression.
