# EAF Session Summary P34 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260711-priority34-coverage-audit`
- **Data:** 2026-07-11
- **PR:** (a ser criado)

## Baseline
| Métrica | Valor |
|---------|-------|
| Line | 83.8% |
| Branch | 63.3% |
| Method | 94.6% |
| Covered Lines | 11450 / 13661 |
| Covered Branches | 1857 / 2932 |
| Covered Methods | 1987 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 84.5% |
| Branch | 64.1% |
| Method | 95.2% |
| Covered Lines | 11563 / 13672 |
| Covered Branches | 1880 / 2932 |
| Covered Methods | 2001 / 2100 |

## Classes adicionadas/alteradas

### Código de produção alterado (correções bloqueantes)
- `src/Eaf.Middleware.Application/Configuration/UiCustomizationSettingsAppService.cs`
  - Construtor alterado de `SettingManager` concreto para `ISettingManager` para permitir mocking com `NSubstitute`.
- `src/Eaf.SqliteCache/Runtime/Caching/Sqlite/EafSqliteCache.cs`
  - Adicionado `try/catch` em torno de `db.Open()` para tratar `SqliteException` em banco de cache inválido.
  - Chama `SqliteConnection.ClearAllPools()` após deletar arquivo de cache corrompido.
- `src/Eaf.SqliteCache/Runtime/Caching/Sqlite/EafSqliteCacheOptions.cs`
  - Desabilitado `Pooling` no connection string do SQLite cache para evitar handles obsoletos.

### Testes adicionados
- `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs`
  - `LogOut` com `IAbpSession.UserId` nulo.
- `test/Eaf.Middleware.Web.Core.Tests/WebHooks/EafWebhookReceiverBddTests.cs`
  - Cobertura dos overloads de `L` com cultura.
- `test/Eaf.Middleware.Worker.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs`
  - `UseEafConfiguration` com action e prefixo.
- `test/Eaf.MiddlewareCore.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs`
  - `UseEafConfiguration` com prefixo e com action + prefixo usando `HostBuilder` real.
- `test/Eaf.MiddlewareCore.Tests/Authorization/AuthorizationExtensionsBddTests.cs`
  - Isolamento de `IocManager.Instance` via reflection para `GetExternalTokenInformation`.
- `test/Eaf.MiddlewareCore.Tests/Authorization/External/Providers/OpenIdConnectAuthProviderApiBddTests.cs`
  - `ValidateTokenInternal` com `ConfigurationManager` mockado, token sem `aud`, e `ValidateIssuer` inválido.
- `test/Eaf.Middleware.Application.Tests/Configuration/UiCustomizationSettingsAppServiceBddTests.cs`
  - `GetUiManagementSettings` com `BaseSettings` nulo.
- `test/Eaf.Middleware.Ldap.Tests/LdapSettingsTests.cs`
  - `GetIsEnabled` com `ISettingManager` non-generic corrigido.
- `test/Eaf.Log4NetServiceBus.Tests/Logging/ServiceBusQueueAppenderBddTests.cs`
  - `OnClose` com conexão aberta.
- `test/Eaf.SqliteCache.Tests/DbCommandPoolBddTests.cs` (novo)
  - `Use` e `UseAsync` para `Operation.Get`, múltiplas operações, conexões exauridas e `Dispose`.
- `test/Eaf.MiddlewareCore.Tests/SampleApp/Core/EntityHistory/BlogBddTests.cs` (novo)
  - Construtor, validações, `ChangeUrl`, coleções `Posts` e `Promotions`.
- `test/Eaf.MiddlewareCore.Tests/SampleApp/Core/EntityHistory/PostBddTests.cs` (novo)
  - Construtor, propriedades, coleções.
- `test/Eaf.Middleware.Worker.Tests/Dependency/EafCastleWindsorHostBuilderExtensionsBddTests.cs`
  - `HostBuilder.Build()` com `UseCastleWindsor`.

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P33.
- Código de produção foi modificado apenas para corrigir bugs bloqueantes (SettingManager/ABP `IsFinal`, SQLite cache handles).
- Não houve alteração em `.github/workflows/`.
- Alguns targets de baixa cobertura (TokenAuthController, MiddlewareWebCoreModule, OCIKeyVaultManager, ServiceBusQueueAppender) permanecem para continuidade na P35.
