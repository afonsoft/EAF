# P44 Coverage Audit — Prompt para Próxima Sessão

Execute o P44 coverage audit para o repositório `afonsoft/EAF` e continue melhorando a cobertura das classes que ainda estão abaixo de 90%.

## Contexto
- Repositório: `afonsoft/EAF` (clone local `/home/ubuntu/repos/EAF`)
- Branch atual: `feature/devin-20260712-priority44-coverage-audit` (a partir da `main` atual)
- Baseline P43: Line 96.1%, Branch 82.0%, Method 99.1% (13141 / 13674 linhas, 2403 / 2930 branches, 2082 / 2100 métodos)
- Testes: 4344 total, 4343 passando, 1 ignorado, 0 falhas
- Stack: xUnit + Shouldly + NSubstitute, BDD em português (`Dado/Quando/Então`)
- Build: `dotnet build Eaf.sln --configuration Release`
- Cobertura: `bash run-tests-with-coverage.sh` (requer `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet`)
- Métricas: `TestResults/CoverageReport/Summary.txt`

## Objetivos
1. Adicionar testes BDD em português para as classes de baixa cobertura restantes, priorizando as com maior impacto e menor percentual atual:
   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (58.5%)
   - `Eaf.Middleware.Web.WebContentDirectoryFinder` (83.3%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (84.8%)
   - `Eaf.MiddlewareCore.SampleApp.EafMiddlewareCoreSampleAppModule` (92.3%)
   - `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (90.6%)
   - `Eaf.Middleware.Core.Configuration.EafStartupConfigurationExtensions` (92.5%)
2. Manter ou aumentar a cobertura: Line >= 96.1%, Branch >= 82.0%, Method >= 99.1%.
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.

## Entregáveis
- Novos/ajustados arquivos de teste BDD em `test/`.
- `docs/development/session-summaries/eaf-session-summary-p44.md`.
- `docs/development/session-summaries/eaf-next-session-prompt-p45.md`.
- `README.md` e `README_pt.md` atualizados com as novas métricas.
- `.agents/MEMORY.md` atualizado com novos gotchas de P44.
- PR para `main` com CI verificado.

## Notas técnicas
- `LdapAuthenticationSource` continua limitado: `LdapConnection` é `IsFinal`/`IsVirtual`, então foque em `TryAuthenticateAsync` com `ILdapConnection` substituído e exceções de `CreateLdapContext` (já coberto em parte). O ramo Windows `PrincipalContext`/`UserPrincipal` não executa no Linux.
- `WebContentDirectoryFinder.CalculateContentRootFolder` procura `src/Eaf.Middleware.Web.Host`; testar `DirectoryNotFoundException` e a branch `coreAssemblyDirectoryPath == null` com `Assembly.GetEntryAssembly()` nulo (requer isolamento de `Assembly` via reflection).
- `MiddlewareWebCoreModule` `SetAppFolders` pode receber `ContentRootPath` nulo (testar `ArgumentException`/`DirectoryNotFoundException` no catch). `PostInitialize` com `Hangfire.IsEnabled` true e `Database:Provider` != `SqlServer` pode usar `Redis`/`InMemory`; para `InMemory`, `JobStorage.Current = new InMemoryStorage()` e o `try/catch` de remoção de jobs executa sem conexão externa.
- `EafMiddlewareCoreSampleAppModule` `Initialize`/`PostInitialize` podem ser exercidos com `EafMiddlewareTestBase` e `UsingDbContext`.
- `EafServiceCollectionMiddlewareExtensions` e `EafStartupConfigurationExtensions` são extensões; chamar em `IServiceCollection`/`IAbpStartupConfiguration` substituídos para cobrir lambdas.

## Validação
- `dotnet build Eaf.sln --configuration Release` deve passar sem erros.
- `bash run-tests-with-coverage.sh` deve passar sem falhas.
- Cobertura não pode regredir abaixo do baseline P43.
- CI do PR deve passar.
