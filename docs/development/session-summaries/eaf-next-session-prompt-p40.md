# P40 Coverage Audit — Prompt para Próxima Sessão

Execute o P40 coverage audit para o repositório `afonsoft/EAF` e atualize o README com as novas métricas.

## Contexto
- Repositório: `afonsoft/EAF` (clone local `/home/ubuntu/repos/EAF`)
- Branch atual: `feature/devin-20260711-priority39-coverage-audit` (ou a branch do P40 a partir da `main` atual)
- Baseline P39: Line 90.8%, Branch 72%, Method 96.9% (12419 / 13672 linhas, 2112 / 2932 branches, 2036 / 2100 métodos)
- Testes: 4128 total, 4127 passando, 1 ignorado, 0 falhas
- Stack: xUnit + Shouldly + NSubstitute, BDD em português (`Dado/Quando/Então`)
- Build: `dotnet build Eaf.sln --configuration Release`
- Cobertura: `bash run-tests-with-coverage.sh` (requer `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet`)
- Métricas: `TestResults/CoverageReport/Summary.txt`

## Objetivos
1. Adicionar testes BDD em português para as classes de baixa cobertura restantes, priorizando as com maior impacto e menor percentual atual:
   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (2.8%)
   - `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` (8.7%)
   - `Eaf.Middleware.Localization.CultureHelper` (78.5%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (80.6%)
   - `Eaf.Middleware.Authorization.Impersonation.ImpersonationManager` (81.0%)
   - `Eaf.Middleware.Web.Controllers.TokenAuthController` (81.4%)
   - `Eaf.Middleware.Worker.MiddlewareWorkerModule` (82.1%)
   - `Eaf.Middleware.Web.Configuration.EafHostBuilderExtensions` (83.3%)
   - `Eaf.Middleware.Web.WebContentDirectoryFinder` (83.3%)
   - `Eaf.Middleware.Web.Startup.RedisConfigurer` (84.6%)
   - `Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker` (85.0%)
   - `Eaf.Middleware.DataExporting.Excel.EpPlus.EpPlusExcelExporterBase` (85.1%)
   - `Eaf.Middleware.Friendships.FriendshipManager` (85.9%)
   - `Eaf.Middleware.Web.Controllers.ProfileControllerBase` (86.7%)
   - `Eaf.Middleware.Authorization.Users.UserAppService` (86.9%)
2. Manter ou aumentar a cobertura: Line >= 90.8%, Branch >= 72%, Method >= 96.9%.
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.

## Entregáveis
- Novos/ajustados arquivos de teste BDD em `test/`.
- `docs/development/session-summaries/eaf-session-summary-p40.md`.
- `docs/development/session-summaries/eaf-next-session-prompt-p41.md`.
- `README.md` e `README_pt.md` atualizados com as novas métricas.
- `.agents/MEMORY.md` atualizado com novos gotchas de P40.
- PR para `main` com CI verificado.

## Notas técnicas
- `LdapAuthenticationSource` e `AzureActiveDirectoryAuthenticationSource` dependem de APIs específicas de diretório e plataforma; isolar com `NSubstitute` ou reflection para caminhos de exceção.
- `CultureHelper` usa `CultureInfo`/`DateTimeFormatInfo`; mocks de `CultureInfo` devem usar culturas concretas (`pt-BR`, `en-US`, `fr-FR`) para evitar comparações instáveis.
- `MiddlewareWorkerModule` e `EafHostBuilderExtensions` usam `HostBuilder`/ServiceCollection; registrar dependências mínimas e usar `BuildServiceProvider` para executar lambdas.
- `ExpiredAuditLogDeleterWorker` tem `MaxDeletionCount` privado; usar reflection para reduzir e cobrir `DoWork`.
- `EpPlusExcelExporterBase` gera arquivos `.xlsx` em `MemoryStream`; limpar arquivos temporários após cada teste.
- `FriendshipManager` e `UserAppService` usam repositórios e `UnitOfWorkManager`; mockar repositórios e `IRepository`.
- `ProfileControllerBase` é base de controller; criar controller concreto mínimo para testar `UpdateProfilePicture` e `DeleteProfilePicture`.
- `RedisConfigurer` e `MiddlewareWebCoreModule` usam `IConfiguration` e `IServiceCollection`; usar `ConfigurationBuilder` com `AddInMemoryCollection`.

## Validação
- `dotnet build Eaf.sln --configuration Release` deve passar sem erros.
- `bash run-tests-with-coverage.sh` deve passar sem falhas.
- Cobertura não pode regredir abaixo do baseline P39.
- CI do PR deve passar.
