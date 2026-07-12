# EAF Session Summary P44 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260712-priority44-coverage-audit`
- **Data:** 2026-07-12
- **PR:** (em aberto)

## Baseline P43
| Métrica | Valor |
|---------|-------|
| Line | 96.1% |
| Branch | 82.0% |
| Method | 99.1% |
| Covered Lines | 13141 / 13674 |
| Covered Branches | 2403 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4344 total, 4343 passando, 1 ignorado |

## Resultado P44
| Métrica | Valor |
|---------|-------|
| Line | 96.1% |
| Branch | 82.0% |
| Method | 99.1% |
| Covered Lines | 13143 / 13674 |
| Covered Branches | 2404 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4356 total, 4355 passando, 1 ignorado |

## Código de produção alterado
- Nenhum. Todos os ajustes foram em arquivos de teste e documentação.

## Testes adicionados/ajustados
- `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs` (ajustado)
  - `Dado_LdapContextSemResultado_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioSemAlterar`
  - `Dado_LdapContextSemResultado_Quando_UpdateUserAsync_Entao_DeveManterUsuarioOriginal`
  - `Dado_LdapContextComErroNoResultado_Quando_GetUsersAsync_Entao_DeveLancarAggregateException`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs` (ajustado)
  - `Dado_HangfireInMemoryHabilitadoAuditingEnabled_Quando_PostInitialize_Entao_DeveRegistrarExpiredWorkers`

- `test/Eaf.Middleware.Web.Core.Tests/Configuration/EafServiceCollectionMiddlewareExtensionsBddTests.cs` (ajustado)
  - `Dado_ServiceCollectionComRedisIsRedisEnabled_Quando_AddEafConfigurer_Entao_DeveRegistrarRedisCache`
  - `Dado_ServiceCollectionComSqlServerIsSqlEnabled_Quando_AddEafConfigurer_Entao_DeveRegistrarSqlServerCache`
  - `Dado_ServiceCollection_Quando_AddEafConfigurerComTudoHabilitado_Entao_DeveConfigurarOptions` (SessionOptions adicionado)

- `test/Eaf.Middleware.Web.Core.Tests/Configuration/EafStartupConfigurationExtensionsBddTests.cs` (ajustado)
  - `Dado_SectionComChavesDuplicadas_Quando_GetChildren_Entao_DeveLancarAbpException`

- `test/Eaf.Middleware.Worker.Tests/Configuration/EafStartupConfigurationExtensionsBddTests.cs` (ajustado)
  - `Dado_SecaoComChaveDuplicada_Quando_SetConfiguration_Entao_DeveLancarAbpException`
  - `Dado_SecaoAninhada_Quando_SetConfiguration_Entao_DeveChamarSetRecursivamente`

- `test/Eaf.MiddlewareCore.Tests/SampleApp/EafMiddlewareCoreSampleAppModuleBddTests.cs` (ajustado)
  - `Dado_ModuloSemDbContextRegistration_Quando_PreInitialize_Entao_NaoDeveRegistrarDbContext`
  - `Dado_Modulo_Quando_Initialize_Entao_DeveRegistrarAssembliesEAutoMapper`

- `test/Eaf.MiddlewareCore.Tests/Net/Web/WebContentDirectoryFinderBddTests.cs` (ajustado)
  - `Dado_DiretorioComWebHostCsproj_Quando_DirectoryContains_Entao_DeveRetornarTrue`

## READMEs atualizados
- `README.md` e `README_pt.md` atualizados com as novas métricas de testes (Total 4356, Passing 4355, Build Warnings 16).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral se manteve em relação ao baseline P43 (Line 96.1%, Branch 82.0%, Method 99.1%).
- `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` subiu de 90.6% para 100%.
- `Eaf.Middleware.Web.Configuration.EafStartupConfigurationExtensions` subiu de 96.2% para 100%.
- `Eaf.Middleware.Configuration.EafStartupConfigurationExtensions` (Worker) manteve 100%.
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` subiu de 58.5% para 60.3% (aumento limitado pelos ramos Windows-only `PrincipalContext`/`UserPrincipal`, não executáveis no Linux).
- `Eaf.Middleware.Web.WebContentDirectoryFinder` manteve 83.3% (branches `directoryInfo.Parent == null` e `coreAssemblyDirectoryPath == null` exigem controle de `Assembly.GetEntryAssembly()`, difícil sem alterar produção).
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` manteve 84.8% (ramo `RedisStorage` com `ConnectionString` padrão `localhost` ainda não coberto; requer Redis disponível).
- `Eaf.MiddlewareCore.SampleApp.EafMiddlewareCoreSampleAppModule` manteve 92.3% (método `PostInitialize` com `SeedHelper.SeedHostDb(IocManager)` requer banco de dados).
- Não houve alteração em `.github/workflows/`.
- Não houve alteração de código de produção.

## Classes com cobertura ainda baixa (foco P45)
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (60.3%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (83.3%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (84.8%)
- `Eaf.MiddlewareCore.SampleApp.EafMiddlewareCoreSampleAppModule` (92.3%)
