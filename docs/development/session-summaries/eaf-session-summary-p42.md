# EAF Session Summary P42 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260712-priority42-coverage-audit`
- **Data:** 2026-07-12
- **PR:** (em aberto)

## Baseline (após merge da main)
| Métrica | Valor |
|---------|-------|
| Line | 95.1% |
| Branch | 80.1% |
| Method | 98.5% |
| Covered Lines | 13008 / 13670 |
| Covered Branches | 2348 / 2930 |
| Covered Methods | 2069 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 95.5% |
| Branch | 80.9% |
| Method | 98.6% |
| Covered Lines | 13061 / 13670 |
| Covered Branches | 2371 / 2930 |
| Covered Methods | 2071 / 2100 |

## Código de produção alterado
Nenhum. Apenas testes foram adicionados/ajustados nesta sessão.

## Testes adicionados/ajustados

- `test/Eaf.MiddlewareCore.Tests/Hangfire/HangfireBackgroundJobManagerBddTests.cs` (ajustado)
  - `Dado_JobComArgs_Quando_EnqueueAsyncComAbpBackgroundJobEDelay_Entao_DeveAgendarComDelay`
  - `Dado_JobComArgs_Quando_EnqueueAsyncComEafBackgroundJobEDelay_Entao_DeveAgendarComDelay`
  - `Dado_JobComArgs_Quando_EnqueueComAbpBackgroundJobEDelay_Entao_DeveAgendarComDelay`
  - `Dado_JobComArgs_Quando_EnqueueComEafBackgroundJobEDelay_Entao_DeveAgendarComDelay`

- `test/Eaf.MiddlewareCore.Tests/Auditing/hangfire/ExpiredEntityLogDeleterWorkerBddTests.cs` (ajustado)
  - `Dado_LogsExpiradosAcimaMaximo_Quando_DeleteLancarExcecao_Entao_DeveCapturarEContinuar`
  - `Dado_LogsExpiradosAbaixoMaximo_Quando_DeleteLancarExcecao_Entao_DeveCapturarEContinuar`

- `test/Eaf.Middleware.Web.Core.Tests/Configuration/RedisConfigurerBddTests.cs` (ajustado)
  - `Dado_RedisComEnabledTrue_Quando_Configure_Entao_DeveRegistrarOptions`
  - `Dado_RedisComIsRedisEnabledTrue_Quando_Configure_Entao_DeveRegistrarOptions`

- `test/Eaf.Log4NetServiceBus.Tests/Logging/ServiceBusQueueAppenderBddTests.cs` (ajustado)
  - `Dado_CorpoCurto_Quando_AppendBuffer_Entao_DevePublicarMensagem`
  - `Dado_ErroGenericoNoEnvio_Quando_AppendBuffer_Entao_DeveCapturarExcecao`

- `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs` (ajustado)
  - `Dado_UserNameSemDominio_Quando_CreateLdapContext_Entao_DevePrefixarComDominio`
  - `Dado_UserNameComBackslash_Quando_CreateLdapContext_Entao_NaoDevePrefixar`
  - `Dado_DominioComPonto_Quando_CreateLdapContext_Entao_NaoDevePrefixarUserName`
  - `Dado_DominioComDC_Quando_CreateLdapContext_Entao_NaoDevePrefixarUserName`
  - `Dado_ContainerVazioComDominioComPonto_Quando_CreateLdapContext_Entao_DeveTransformarContainer`
  - `Dado_ContainerComDC_Quando_CreateLdapContext_Entao_DeveManterContainer`
  - `Dado_UserNameEPasswordVazios_Quando_CreateLdapContext_Entao_DeveUsarConfiguracoes`
  - `Dado_UserNameEPasswordFornecidos_Quando_CreateLdapContext_Entao_DeveUsarParametros`

- `test/Eaf.Middleware.Application.Tests/Friendships/FriendshipManagerBddTests.cs` (ajustado)
  - `Dado_ChaveLocalizada_Quando_LComArgs_Entao_DeveRetornarTextoFormatado`
  - `Dado_ChaveLocalizada_Quando_LComCultura_Entao_DeveRetornarChave`

- `test/Eaf.Middleware.Worker.Tests/Middleware/MiddlewareWorkerModuleIntegrationTests.cs` (ajustado)
  - `Dado_AspNetCoreEnvironment_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente`
  - `Dado_CacheConfigurado_Quando_PreInitialize_Entao_DeveAplicarCacheConfigurator`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs` (ajustado)
  - `Dado_HangfireHabilitado_Quando_PreInitialize_Entao_DeveUsarHangfire`
  - `Dado_RedisDesabilitado_Quando_Initialize_Entao_NaoDeveRegistrarRedisAssembly`

## READMEs atualizados
- `README.md` e `README_pt.md` atualizados com as novas métricas de testes (Line 95.5%, Branch 80.9%, Method 98.6%, Total 4296, Passing 4295).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P42 (Line 95.1% -> 95.5%, Branch 80.1% -> 80.9%, Method 98.5% -> 98.6%).
- `Eaf.Hangfire.HangfireBackgroundJobManager` subiu de 85% para 100%.
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` subiu de 87.6% para 100%.
- `Eaf.Middleware.Web.Startup.RedisConfigurer` subiu de 84.6% para 100%.
- `Eaf.Middleware.Worker.MiddlewareWorkerModule` subiu de 89% para 91.7%.
- `Eaf.Middleware.Friendships.FriendshipManager` subiu de 89.4% para 100%.
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` subiu de 80.3% para 92.8%.
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` subiu de 50.1% para 50.3% (métodos de `LdapConnection` em `Novell.Directory.Ldap.NETStandard` 4.0.0 são `virtual` mas `IsFinal`, então não podem ser substituídos pelo `Castle DynamicProxy` usado pelo NSubstitute).
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` manteve 84.8% (fallbacks de variáveis de ambiente no construtor são normalizados por `AppConfigurations.Get`, e branch `RedisStorage` com `ConnectionString` padrão exigiria conexão real com Redis).
- `Eaf.Middleware.Web.WebContentDirectoryFinder` manteve 83.3% (branches com `directoryInfo.Parent == null` e `coreAssemblyDirectoryPath == null` são difíceis de exercer sem modificar produção).
- Não houve alteração em `.github/workflows/`.
- Não houve alteração em código de produção.
- `Novell.Directory.Ldap.NETStandard` 4.0.0 expõe `LdapConnection` `SearchAsync`/`ConnectAsync`/`BindAsync` como `IsFinal`/`IsVirtual`, impedindo mock com NSubstitute. `ILdapSearchResults` é uma interface e pode ser substituída.
- `MiddlewareWorkerModule.PreInitialize` `Configuration.ReplaceService` adiciona ações a `ServiceReplaceActions`; chamar todas as ações manualmente pode causar `Castle.MicroKernel.ComponentRegistrationException` por registro duplicado.

## Classes com cobertura ainda baixa (foco P43)
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (50.3%)
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Advertisement` (50.0%)
- `Eaf.MiddlewareCore.SampleApp.Core.UserClaimsPrincipalFactory` (70.0%)
- `Eaf.MiddlewareCore.SampleApp.Core.Shop.OrderTranslation` (75.0%)
- `Eaf.MiddlewareCore.SampleApp.Core.Shop.ProductTranslation` (75.0%)
- `Eaf.MiddlewareCore.SampleApp.Core.User` (80.0%)
- `Eaf.MiddlewareCore.SampleApp.EafMiddlewareCoreSampleAppModule` (76.9%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (83.3%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (84.8%)
- `Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host.DefaultSettingsCreator` (87.5%)
- `Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Tenants.DefaultTenantBuilder` (87.5%)
- `Eaf.Middleware.Auditing.EntityHistoryConfigurationExtensions` (87.5%)
- `Abp.Runtime.Caching.Sqlite.DbCommandPool` (89.4%)
- `Abp.Runtime.Caching.Sqlite.EafSqliteCache` (88.8%)
- `Eaf.Middleware.Localization.CultureHelper` (78.5%)
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Country` (0%)
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Foo` (0%)
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.AdvertisementFeedback` (0%)
