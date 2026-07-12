# EAF Session Summary P45 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260712-priority45-coverage-audit`
- **Data:** 2026-07-12
- **PR:** (em aberto)

## Baseline P44
| Métrica | Valor |
|---------|-------|
| Line | 96.1% |
| Branch | 82.0% |
| Method | 99.1% |
| Covered Lines | 13143 / 13674 |
| Covered Branches | 2404 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4356 total, 4355 passando, 1 ignorado |

## Resultado P45
| Métrica | Valor |
|---------|-------|
| Line | 96.2% |
| Branch | 82.3% |
| Method | 99.1% |
| Covered Lines | 13155 / 13674 |
| Covered Branches | 2412 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4367 total, 4366 passando, 1 ignorado |

## Código de produção alterado
- Nenhum. Todos os ajustes foram em arquivos de teste e documentação.

## Testes adicionados/ajustados
- `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs` (ajustado)
  - `Dado_FillUsersLdapComAtributoMailAusente_Quando_Executar_Entao_DeveRetornarUsuarioComEmailVazio`
  - `Dado_LdapContextSearchNulo_Quando_UpdateUserAsync_Entao_DeveCapturarExcecaoELogar`
  - `Dado_UsuarioSemEmail_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioBase`
  - `Dado_UserNameSemEmail_Quando_GetUsersAsync_Entao_DeveRetornarListaVazia`
  - `Dado_TenantNulo_Quando_TryAuthenticateAsync_Entao_DeveRetornarTrue`
  - `Dado_TenantNulo_Quando_CheckIsEnabled_Entao_DeveUsarTenantIdNulo`

- `test/Eaf.MiddlewareCore.Tests/Net/Web/WebContentDirectoryFinderBddTests.cs` (ajustado)
  - `Dado_AssemblySemLocalizacao_Quando_CalculateContentRootFolder_Entao_DeveLancarExcecaoDeAssembly`
  - `Dado_AssemblySemSolucaoAteRaiz_Quando_CalculateContentRootFolder_Entao_DeveLancarExcecaoDeRaiz`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs` (ajustado)
  - `Dado_VariaveisAmbienteNaoDefinidas_Quando_CriarModulo_Entao_DeveResolverPelaVariavelDotnetEnvironment`
  - `Dado_ContentRootPathNulo_Quando_PostInitialize_Entao_DeveUsarDiretorioAtualEConfigurarPastas`

- `test/Eaf.MiddlewareCore.Tests/SampleApp/EafMiddlewareCoreSampleAppModuleBddTests.cs` (ajustado)
  - `Dado_ModuloComDbContextRegistration_Quando_PreInitialize_Entao_DeveConfigurarDbContextComSqlServer`

## READMEs atualizados
- `README.md` e `README_pt.md` atualizados com as novas métricas de testes (Total 4367, Passing 4366, Build Warnings 121).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral aumentou em relação ao baseline P44: Line 96.1% → 96.2%, Branch 82.0% → 82.3%, Method manteve 99.1%.
- `Eaf.Middleware.Web.WebContentDirectoryFinder` subiu de 83.3% para 100%.
- `Eaf.MiddlewareCore.SampleApp.EafMiddlewareCoreSampleAppModule` subiu de 92.3% para 100%.
- `Eaf.MiddlewareCore.SampleApp.EntityFramework.EafMiddlewareTemplateDbContextConfigurer` chegou a 100%.
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` subiu de 84.8% para 86.2%.
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` subiu de 60.3% para 61.3%.
- Os ramos Windows-only `PrincipalContext`/`UserPrincipal`/`ValidateCredentials` em `LdapAuthenticationSource` continuam inacessíveis no Linux.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração de código de produção.

## Classes com cobertura ainda baixa (foco P46)
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (61.3%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (86.2%)
