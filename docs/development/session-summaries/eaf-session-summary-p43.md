# EAF Session Summary P43 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260712-priority43-coverage-audit`
- **Data:** 2026-07-12
- **PR:** (em aberto)

## Baseline P42
| Métrica | Valor |
|---------|-------|
| Line | 95.5% |
| Branch | 80.9% |
| Method | 98.6% |
| Covered Lines | 13061 / 13670 |
| Covered Branches | 2371 / 2930 |
| Covered Methods | 2071 / 2100 |
| Testes | 4296 total, 4295 passando, 1 ignorado |

## Resultado P43
| Métrica | Valor |
|---------|-------|
| Line | 96.1% |
| Branch | 82.0% |
| Method | 99.1% |
| Covered Lines | 13141 / 13674 |
| Covered Branches | 2403 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4344 total, 4343 passando, 1 ignorado |

## Código de produção alterado
- `src/Eaf.Middleware.Ldap/Ldap/Authentication/LdapAuthenticationSource.cs`
  - `CreateLdapContext` agora retorna `Task<ILdapConnection>` e as chamadas internas (`CreateUserAsync`, `UpdateUserAsync`, `GetUsersAsync`, `TryAuthenticateAsync`) usam `ILdapConnection`/`IDisposable` para permitir mock do `SearchAsync`/`Connected`.
  - Alteração justificada: `Novell.Directory.Ldap.LdapConnection` expõe `SearchAsync` como `virtual`/`IsFinal` (`sealed`), então o `NSubstitute` não consegue configurar `Returns` com `Substitute.For<LdapConnection>()`.

## Testes adicionados/ajustados
- `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs` (ajustado)
  - `Dado_LdapContextComResultado_Quando_CreateUserAsync_Entao_DeveAtualizarUsuario`
  - `Dado_LdapContextComResultado_Quando_UpdateUserAsync_Entao_DeveAtualizarUsuario`
  - `Dado_LdapContextComResultado_Quando_GetUsersAsync_Entao_DeveRetornarUsuarios`
  - `Dado_LdapConectado_Quando_TryAuthenticateAsync_Entao_DeveRetornarTrue`
  - `Dado_LdapConectadoComEmail_Quando_TryAuthenticateAsync_Entao_DeveRemoverDominio`
  - `Dado_LdapContextInvalido_Quando_GetUsersAsync_Entao_DeveLancarExcecao`
  - `Dado_FillUsersLdapNulo_Quando_Executar_Entao_DeveLancarExcecao`
  - `Dado_FillUsersLdapComNextLancandoExcecao_Quando_Executar_Entao_DeveContinuarProcessando`
  - `Dado_FillUsersLdapSemMail_Quando_Executar_Entao_DeveUsarUserPrincipalName`
  - `Dado_CreateLdapContextFalhando_Quando_CreateUserAsync_Entao_DeveLancarExcecao`
  - `Dado_LdapContextInvalido_Quando_UpdateUserAsync_Entao_DeveCapturarExcecaoELogar`
  - `Dado_CreateLdapContextLancandoExcecao_Quando_TryAuthenticateAsync_Entao_DevePropagar`

- `test/Eaf.Middleware.Application.Tests/Auditing/EntityHistoryConfigurationExtensionsBddTests.cs` (novo)
  - `Dado_EntityHistoryHabilitadoSemSeletor_Quando_AddAllAuditedEntities_Entao_DeveAdicionarSeletorDeTodasEntidades`
  - `Dado_EntityHistoryHabilitadoComSeletorExistente_Quando_AddAllAuditedEntities_Entao_DeveRetornarSemAdicionar`
  - `Dado_EntityHistoryDesabilitado_Quando_AddAllAuditedEntities_Entao_DeveRetornarSemAdicionar`

- `test/Eaf.MiddlewareCore.Tests/SampleApp/Core/SampleAppEntitiesBddTests.cs` (novo)
  - `Dado_Advertisement_Quando_Criar_Entao_DevePreencherPropriedades`
  - `Dado_AdvertisementFeedback_Quando_Criar_Entao_DevePreencherPropriedades`
  - `Dado_Country_Quando_Criar_Entao_DevePreencherPropriedades`
  - `Dado_Foo_Quando_Criar_Entao_DevePreencherPropriedades`
  - `Dado_OrderTranslation_Quando_Criar_Entao_DevePreencherPropriedades`
  - `Dado_ProductTranslation_Quando_Criar_Entao_DevePreencherPropriedades`
  - `Dado_User_Quando_Criar_Entao_DevePreencherPropriedades`
  - `Dado_User_Quando_AdicionarRole_Entao_DeveConterRole`

- `test/Eaf.MiddlewareCore.Tests/SampleApp/Core/UserClaimsPrincipalFactoryBddTests.cs` (novo)
  - `Dado_ClaimsPrincipal_Quando_CreateAsync_Entao_DeveRetornarIdentityComClaims`
  - `Dado_UserSemTenant_Quando_CreateAsync_Entao_DeveRetornarClaimsCorretos`

- `test/Eaf.MiddlewareCore.Tests/SampleApp/EafMiddlewareCoreSampleAppModuleBddTests.cs` (novo)
  - `Dado_Modulo_Quando_Initialize_Entao_DeveConfigurarDependencias`
  - `Dado_Modulo_Quando_PostInitialize_Entao_DeveConfigurarServicos`

- `test/Eaf.MiddlewareCore.Tests/SampleApp/EntityFramework/EafMiddlewareTemplateDbContextConfigurerBddTests.cs` (novo)
  - `Dado_Configuracao_Quando_Configure_Entao_DeveAplicarConnectionString`
  - `Dado_Configuracao_Quando_Configure_Entao_DeveUsarConnectionStringDaConfiguracao`

- `test/Eaf.MiddlewareCore.Tests/SampleApp/Seed/SampleAppSeedBddTests.cs` (novo)
  - `Dado_TenantDefault_Quando_DefaultTenantBuilder_Entao_DeveCriarTenantAdmin`
  - `Dado_HostDefault_Quando_DefaultSettingsCreator_Entao_DeveCriarSettings`

- `test/Eaf.SqliteCache.Tests/DbCommandPoolBddTests.cs` (ajustado)
  - `Dado_PoolVazio_Quando_Get_Entao_DeveCriarNovoComando`
  - `Dado_PoolComComando_Quando_Get_Entao_DeveReutilizarComando`
  - `Dado_ComandoUsado_Quando_Return_Entao_DeveArmazenarNoPool`

- `test/Eaf.SqliteCache.Tests/EafSqliteCacheTests.cs` (ajustado)
  - `Dado_CacheVazio_Quando_Get_Entao_DeveRetornarNulo`
  - `Dado_CacheComValor_Quando_Get_Entao_DeveRetornarValor`
  - `Dado_Cache_Quando_Set_Entao_DeveArmazenarValor`
  - `Dado_Cache_Quando_Remove_Entao_DeveRemoverValor`
  - `Dado_Cache_Quando_Clear_Entao_DeveLimparTodos`

- `test/Eaf.SqliteCache.Tests/Runtime/Caching/Sqlite/EafSqliteCacheOptionsBddTests.cs` (novo)
  - `Dado_OpcoesPadrao_Quando_Configurar_Entao_DeveTerValoresPadrao`
  - `Dado_OpcoesCustomizadas_Quando_Configurar_Entao_DeveAplicarValores`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs` (ajustado)
  - `Dado_HostEnvironmentComRedisEnabledHabilitado_Quando_Initialize_Entao_DeveRegistrarRedisAssembly`
  - `Dado_IocManagerConfigurado_Quando_Initialize_Entao_DeveRegistrarConventions`
  - `Dado_IocManagerConfigurado_Quando_PostInitialize_Entao_DeveConfigurarPastasEProvedoresExternos`

## READMEs atualizados
- `README.md` e `README_pt.md` atualizados com as novas métricas de testes (Line 96.1%, Branch 82.0%, Method 99.1%, Total 4344, Passing 4343).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P42 (Line 95.5% -> 96.1%, Branch 80.9% -> 82.0%, Method 98.6% -> 99.1%).
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Advertisement` subiu de 50.0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.Core.UserClaimsPrincipalFactory` subiu de 70.0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.Core.Shop.OrderTranslation` subiu de 75.0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.Core.Shop.ProductTranslation` subiu de 75.0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.Core.User` subiu de 80.0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.EafMiddlewareCoreSampleAppModule` subiu de 76.9% para 92.3%.
- `Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host.DefaultSettingsCreator` subiu de 87.5% para 100%.
- `Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Tenants.DefaultTenantBuilder` subiu de 87.5% para 100%.
- `Eaf.Middleware.Auditing.EntityHistoryConfigurationExtensions` subiu de 87.5% para 100%.
- `Abp.Runtime.Caching.Sqlite.DbCommandPool` subiu de 89.4% para 94.7%.
- `Abp.Runtime.Caching.Sqlite.EafSqliteCache` subiu de 88.8% para 95.7%.
- `Eaf.Middleware.Localization.CultureHelper` subiu de 78.5% para 100%.
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Country` subiu de 0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Foo` subiu de 0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.AdvertisementFeedback` subiu de 0% para 100%.
- `Eaf.MiddlewareCore.SampleApp.EntityFramework.EafMiddlewareTemplateDbContextConfigurer` subiu de 0% para 100%.
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` subiu de 50.3% para 58.5% (aumento limitado pelos ramos Windows-only `PrincipalContext`/`UserPrincipal`, não executáveis no Linux).
- `Eaf.Middleware.Web.WebContentDirectoryFinder` manteve 83.3% (branches `directoryInfo.Parent == null` e `coreAssemblyDirectoryPath == null` exigem controle de `Assembly.GetEntryAssembly()`, difícil sem alterar produção).
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` manteve 84.8% (ramo `RedisStorage` com `ConnectionString` padrão `localhost` e `SetAppFolders` com `ContentRootPath` nulo ainda não cobertos).
- Não houve alteração em `.github/workflows/`.
- O único código de produção alterado foi `LdapAuthenticationSource.cs` para desbloquear mocking do `ILdapConnection`.

## Classes com cobertura ainda baixa (foco P44)
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (58.5%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (83.3%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (84.8%)
- `Eaf.MiddlewareCore.SampleApp.EafMiddlewareCoreSampleAppModule` (92.3%)
- `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (90.6%)
- `Eaf.Middleware.Core.Configuration.EafStartupConfigurationExtensions` (92.5%)
