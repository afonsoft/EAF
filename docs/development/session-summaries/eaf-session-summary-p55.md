# EAF Session Summary P55 - Coverage Audit

## Data

14 de julho de 2026

## Branch

`feature/devin-20260713-priority55-coverage-audit`

## Baseline P54

| Metric | Value |
|--------|-------|
| Line coverage | 97.2% (13235 / 13604) |
| Branch coverage | 85.1% (2440 / 2866) |
| Method coverage | 99.5% (2150 / 2159) |
| Tests | 4467 total, 4466 passing, 1 skipped |
| Build warnings | 127 |

## Final P55

| Metric | Value |
|--------|-------|
| Line coverage | 97.5% (13263 / 13593) |
| Branch coverage | 85.6% (2454 / 2866) |
| Method coverage | 99.6% (2155 / 2162) |
| Tests | 4492 total, 4491 passing, 1 skipped |
| Build warnings | 129 |

## Changes

- Adicionados testes BDD para ramos acessíveis das classes de baixa cobertura do P55.
- Código de produção não foi alterado.
- Nenhum arquivo `.github/workflows/` foi modificado.

## New / updated tests

| Class | Assembly | Test file | Focus |
|-------|----------|-----------|-------|
| `UserEmailer` | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/Authorization/UserEmailerBddTests.cs` | Overloads `L` com args e `CultureInfo` |
| `TenantManager` | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/MultiTenancy/TenantManagerBddTests.cs` | `CreateWithAdminUserAsync` com validador de senha customizado |
| `RemoteAuthenticationContextExtensions` | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/Authorization/External/RemoteAuthenticationContextExtensionsBddTests.cs` | `AddMappedClaims` com mapeamentos vazios |
| `EafHostBuilderExtensions` (Core) | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs` | `UseEafConfiguration` com prefixo e action nula |
| `EafHangfireAuthorizationFilter` | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/Hangfire/EafHangfireAuthorizationFilterBddTests.cs` | Token JWT com tenant mas sem `Sub` |
| `HostRoleAndUserCreator` | Eaf.EntityFrameworkCore.SampleApp.Tests | `test/Eaf.MiddlewareCore.Tests/SampleApp/Seed/SampleAppSeedBddTests.cs` | Seed idempotente quando admin host já existe |
| `LanguageAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Localization/LanguageAppServiceBddTests.cs` | `GetLanguageTexts` sem idiomas, `SkipCount > 0`, idioma duplicado |
| `TenantAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/MultiTenancy/TenantAppServiceBddTests.cs` | `GetTenantFeaturesForEdit` com `IFeatureManager`/`IObjectMapper` |
| `NotificationAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Notifications/NotificationAppServiceBddTests.cs` | `DeleteNotification` para notificação de outro usuário |
| `HostSettingsAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Configuration/Host/HostSettingsAppServiceBddTests.cs` | Erro no `GetLoginImpersonatorAsync` e timezone em `UpdateGeneralSettingsAsync` |
| `FileController` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Controllers/FileControllerBddTests.cs` | `DownloadTempFile`/`DownloadBinaryFile` com `ModelState` inválido |
| `ChatController` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Controllers/ChatControllerBddTests.cs` | `GetUploadedObject` com `ModelState` inválido |
| `ChatHub` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/SignalR/Chat/ChatHubBddTests.cs` | `Dispose` duplicado |
| `DefaultExternalLoginInfoManager` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Authentication/DefaultExternalLoginInfoManagerBddTests.cs` | `nameClaim` vazio |
| `AboutController` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Controllers/AboutControllerBddTests.cs` | `Modules` preenchidos |
| `EafHostBuilderExtensions` (Worker) | Eaf.Middleware.Worker | `test/Eaf.Middleware.Worker.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs` | `UseAbpConfiguration` com prefixo e action nula |
| `EafServiceCollectionExtensions` | Eaf.Middleware.Worker | `test/Eaf.Middleware.Worker.Tests/ServiceProviders/EafServiceCollectionExtensionsBddTests.cs` | `AddCastleLogger` com `castleLoggerFactory` registrado |
| `EafOpenTelemetryServiceCollectionExtensions` | Eaf.OpenTelemetry | `test/Eaf.OpenTelemetry.Tests/EafOpenTelemetryServiceCollectionExtensionsBddTests.cs` | Variável OTLP inválida com `=` |
| `EafSqlServerCache` | Eaf.SqlServerCache | `test/Eaf.SqlServerCache.Tests/EafSqlServerCacheTests.cs` | `TryGetValue` com cache existente |
| `CoreManagerTestHelper` | Eaf.MiddlewareCore.Tests | `test/Eaf.MiddlewareCore.Tests/Helpers/CoreManagerTestHelper.cs` | Overloads para injetar `passwordValidators` no `UserManager`/`TenantManager` |

## Notes

- `LdapAuthenticationSource` (59.2%), `MiddlewareWebCoreModule` (87.3%), `PermissionAppService` (92.5%), `ServiceBusQueueAppender` (92.8%), `AzureActiveDirectoryAuthenticationSource` (93.5%), `EafSqliteCache` (94.9%) e `OpenIdConnectAuthProviderApi` (95.2%) mantêm ramos inacessíveis no Linux ou com builders `sealed` não mockáveis com `NSubstitute`.
- `TokenAuthController` (90.9%) ainda possui branches complexos de `ExternalAuthenticate`/`GetExternalUserInfo`/`RegisterExternalUserAsync` que exigem setup de `DefaultExternalLoginInfoManager`/`IocManager`.
- `EafHostBuilderExtensions` (Core e Worker) e `EafOpenTelemetryServiceCollectionExtensions` subiram com os novos testes, mas ainda têm branches de fallback/exception não cobertos.
- Build Release: 0 erros, 129 warnings. Todos os testes passam sem regressão de cobertura.

## Verification

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

Build: 0 errors, 129 warnings. All tests pass with no coverage regression.
