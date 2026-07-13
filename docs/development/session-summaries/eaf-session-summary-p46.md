# EAF Session Summary P46 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260713-priority46-coverage-audit`
- **Data:** 2026-07-13
- **PR:** (em aberto)

## Baseline P45
| Métrica | Valor |
|---------|-------|
| Line | 96.2% |
| Branch | 82.3% |
| Method | 99.1% |
| Covered Lines | 13155 / 13674 |
| Covered Branches | 2412 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4367 total, 4366 passando, 1 ignorado |

## Resultado P46
| Métrica | Valor |
|---------|-------|
| Line | 96.2% |
| Branch | 82.6% |
| Method | 99.1% |
| Covered Lines | 13167 / 13674 |
| Covered Branches | 2423 / 2930 |
| Covered Methods | 2082 / 2100 |
| Testes | 4377 total, 4376 passando, 1 ignorado |

## Código de produção alterado
- Nenhum. Todos os ajustes foram em arquivos de teste e documentação.
- Correção do link do SonarCloud em `README.md` e `README_pt.md` (de `summary/overall` para `project/overview`).

## Testes adicionados/ajustados
- `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs` (ajustado)
  - `Dado_TenantNulo_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioBase`
  - `Dado_TenantNuloEUsuarioComEmail_Quando_CreateUserAsync_Entao_DeveRetornarUsuarioBase`
  - `Dado_TenantNulo_Quando_UpdateUserAsync_Entao_DeveManterUsuarioOriginal`
  - `Dado_TenantNaoNulo_UserNameEPasswordNulos_Quando_CreateLdapContext_Entao_DeveUsarConfiguracoes`
  - `Dado_TenantNaoNulo_UserNameFornecido_Quando_CreateLdapContext_Entao_DevePrefixarUserName`
  - `Dado_TenantNaoNulo_DominioComDC_Quando_CreateLdapContext_Entao_DeveNaoPrefixarUserName`
  - `Dado_TenantNaoNulo_DominioComPonto_Quando_CreateLdapContext_Entao_DeveNaoPrefixarUserName`
  - `Dado_TenantNaoNulo_ContainerComDC_Quando_CreateLdapContext_Entao_DeveManterContainer`
  - `Dado_TenantNaoNulo_ContainerVazioComDominioComPonto_Quando_CreateLdapContext_Entao_DeveTransformarContainer`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs` (ajustado)
  - `Dado_HangfireInMemoryDefaultComMySqlERedisDesabilitado_Quando_PostInitialize_Entao_DeveConfigurarInMemoryStorage`

## READMEs atualizados
- `README.md` e `README_pt.md` atualizados com as novas métricas (Total 4377, Passing 4376, Branch 82.6%, Cobertura Ldap 67.7%, Web.Core 96.1%) e link correto do SonarCloud.

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral aumentou em relação ao baseline P45: Line 96.2% (mantido), Branch 82.3% → 82.6%, Method 99.1% (mantido).
- `Eaf.Middleware.Ldap` subiu de 65.4% para 67.7%.
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` manteve 86.2%.
- `Eaf.Middleware.Web.Core` subiu de 95.8% para 96.1%.
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` manteve 61.3%.
- `HangFireConfigurer` manteve 100%.
- O `HangFireConfigurer.ResolveStorageType` agora cobre o ramo `InMemory` padrão quando `Database:Provider` não é `SqlServer` e `RedisCache` está desabilitado.
- Os ramos Windows-only `PrincipalContext`/`UserPrincipal`/`ValidateCredentials` em `LdapAuthenticationSource` continuam inacessíveis no Linux.
- Os loops `recurringJobs`/`failedJobs` de `PostInitialize` do `MiddlewareWebCoreModule` ainda não são cobertos com dados, pois `JobStorage.Current` é sempre recriado durante `PostInitialize` e não pode ser pré-populado com jobs no ambiente de teste.
- O bloco `catch` de `CompositeFileProvider` em `SetAppFolders` continua inatingível porque `CompositeFileProvider` não lança exceção com `IFileProvider` nulo.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração de código de produção.

## Classes com cobertura ainda baixa (foco P47)
- `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (67.7%)
