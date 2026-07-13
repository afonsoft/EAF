# EAF Session Summary P47 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260713-priority47-coverage-audit`
- **Data:** 2026-07-13
- **PR:** (em aberto)

## Baseline P46
| Métrica | Valor |
|---------|-------|
| Line | 96.2% |
| Branch | 82.6% |
| Method | 99.1% |
| Covered Lines | 13167 / 13674 |
| Covered Branches | 2423 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4377 total, 4376 passando, 1 ignorado |

## Resultado P47
| Métrica | Valor |
|---------|-------|
| Line | 96.2% |
| Branch | 82.8% |
| Method | 99.1% |
| Covered Lines | 13160 / 13674 |
| Covered Branches | 2427 / 2930 |
| Covered Methods | 2083 / 2100 |
| Testes | 4388 total, 4387 passando, 1 ignorado |
| Build Warnings | 141 |

## Código de produção alterado
- Nenhum. Todos os ajustes foram em arquivos de teste e documentação.

## Testes adicionados/ajustados
- `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs`
  - `Dado_PlataformaNaoWindows_Quando_SearchWithLimit_Entao_DeveLancarExcecaoDePlataforma`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs`
  - `Dado_HangfireRedisComDatabaseIdInvalido_Quando_PostInitialize_Entao_DeveConfigurarRedisStorageComDatabaseIdZero`
  - `Dado_HangfireNaoConfiguradoComAuditingHabilitado_Quando_PostInitialize_Entao_DeveRegistrarExpiredAuditLogDeleterWorker`
  - `Dado_BackgroundJobsHabilitadoHangfireNaoConfigurado_Quando_PreInitialize_Entao_NaoDeveUsarHangfire`

- `test/Eaf.Middleware.Web.Core.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs`
  - `Dado_IHostBuilder_Quando_UseAbpConfigurationComPrefixoENulo_Entao_DeveAdicionarEnvironmentVariablesComPrefixo`
  - `Dado_IHostBuilder_Quando_UseAbpConfigurationComPrefixoVazio_Entao_DeveConfigurarSemPrefixo`
  - `Dado_IWebHostBuilder_Quando_UseAbpConfigurationComPrefixoENulo_Entao_DeveAdicionarEnvironmentVariablesComPrefixo`
  - `Dado_IWebHostBuilder_Quando_UseAbpConfigurationComPrefixoVazio_Entao_DeveConfigurarSemPrefixo`

- `test/Eaf.Middleware.Web.Core.Tests/Configuration/EafWebHostBuilderExtensionsBddTests.cs`
  - `Dado_WebHostBuilder_Quando_UseEafConfigurationComPrefixoENulo_Entao_DeveAdicionarEnvironmentVariablesComPrefixo`
  - `Dado_WebHostBuilder_Quando_UseEafConfigurationComPrefixoVazio_Entao_DeveConfigurarSemPrefixo`

- `test/Eaf.Middleware.Worker.Tests/ServiceProviders/EafServiceCollectionExtensionsBddTests.cs`
  - `Dado_ColecoesDeServicos_Quando_AdicionarEafSemRetornarServiceProviderSemRemoveConventionalInterceptors_Entao_DeveManterInterceptors`

## READMEs atualizados
- `README.md` e `README_pt.md` atualizados com as novas métricas (Total 4388, Passing 4387, Branch 82.8%, Cobertura Eaf.Middleware.Ldap 68.1%, Eaf.Middleware.Web.Core 96.1%).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral aumentou em relação ao baseline P46: Line 96.2% (mantido), Branch 82.6% → 82.8%, Method 99.1% (mantido).
- `Eaf.Middleware.Ldap` subiu de 67.7% para 68.1%.
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` subiu de 61.3% para 61.8%.
- `Eaf.Middleware.Web.Core` manteve 96.1%.
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` manteve 86.2% (branch 70%).
- `Eaf.Middleware.Configuration.EafHostBuilderExtensions` subiu de 83.3% para 96.2%.
- `Eaf.Middleware.Web.Startup.EafWebHostBuilderExtensions` manteve 100%.
- `Eaf.Middleware.Worker.EafServiceCollectionExtensions` subiu de 92.3% para 96.2%.
- O ramo `RedisConnectionString` nulo em `MiddlewareWebCoreModule.PostInitialize` não é coberto porque o construtor `RedisStorage` lança exceção no ambiente Linux sem Redis; o teste para esse ramo foi removido.
- Os loops `recurringJobs`/`failedJobs` de `PostInitialize` do `MiddlewareWebCoreModule` ainda não são cobertos com dados, pois `JobStorage.Current` é sempre recriado durante `PostInitialize` e não pode ser pré-populado com jobs no ambiente de teste.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração de código de produção.

## Classes com cobertura ainda baixa (foco P48)
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (61.8%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (86.2%)
