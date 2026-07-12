# P42 Coverage Audit — Prompt para Próxima Sessão

Execute o P42 coverage audit para o repositório `afonsoft/EAF` e atualize o README com as novas métricas.

## Contexto
- Repositório: `afonsoft/EAF` (clone local `/home/ubuntu/repos/EAF`)
- Branch atual: `feature/devin-20260712-priority41-coverage-audit` (ou a branch do P42 a partir da `main` atual)
- Baseline P41: Line 95%, Branch 80.1%, Method 98.5% (12996 / 13670 linhas, 2348 / 2930 branches, 2069 / 2100 métodos)
- Testes: 4266 total, 4265 passando, 1 ignorado, 0 falhas
- Stack: xUnit + Shouldly + NSubstitute, BDD em português (`Dado/Quando/Então`)
- Build: `dotnet build Eaf.sln --configuration Release`
- Cobertura: `bash run-tests-with-coverage.sh` (requer `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet`)
- Métricas: `TestResults/CoverageReport/Summary.txt`

## Objetivos
1. Adicionar testes BDD em português para as classes de baixa cobertura restantes, priorizando as com maior impacto e menor percentual atual:
   - `Eaf.Middleware.Ldap` (58.7%)
   - `Eaf.Log4NetServiceBus` (80.3%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (84.8%)
   - `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%)
   - `Eaf.Middleware.Web.Startup.RedisConfigurer` (84.6%)
   - `Eaf.Middleware.Worker.MiddlewareWorkerModule` (89%)
   - `Eaf.Hangfire.HangfireBackgroundJobManager` (85%)
   - `Eaf.Middleware.Web.WebContentDirectoryFinder` (83.3%)
   - `Eaf.Middleware.Friendships.FriendshipManager` (89.4%)
2. Manter ou aumentar a cobertura: Line >= 95%, Branch >= 80.1%, Method >= 98.5%.
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.

## Entregáveis
- Novos/ajustados arquivos de teste BDD em `test/`.
- `docs/development/session-summaries/eaf-session-summary-p42.md`.
- `docs/development/session-summaries/eaf-next-session-prompt-p43.md`.
- `README.md` e `README_pt.md` atualizados com as novas métricas.
- `.agents/MEMORY.md` atualizado com novos gotchas de P42.
- PR para `main` com CI verificado.

## Notas técnicas
- `Eaf.Middleware.Ldap` e `Eaf.Log4NetServiceBus` têm cobertura baixa; isolar com `TestableLdapConnection`/`TestableSmtpClient` ou handlers mockados.
- `MiddlewareWebCoreModule` e `MiddlewareWorkerModule` usam `HostBuilder`/ServiceCollection e `DependsOn`; registrar dependências mínimas e usar `BuildServiceProvider` para executar inicializadores.
- `ExpiredEntityLogDeleterWorker` é similar ao `ExpiredAuditLogDeleterWorker`, mas usa `IEntityHistoryConfiguration`, `ISettingManager` e `IRepository<EntityChange, long>`.
- `RedisConfigurer` usa `bool.Parse` e `IConfigurationRoot.GetValue<int>`; usar `ConfigurationBuilder` com `Dictionary<string, string?>`.
- `HangfireBackgroundJobManager` depende de `IBackgroundJobManager` do ABP; mockar `IRecurringJobManager` e `JobStorage`.
- `WebContentDirectoryFinder` é `public static class` no namespace `Eaf.Middleware.Web`; `CalculateContentRootFolder` procura por `src/Eaf.Middleware.Web.Host`.
- `FriendshipManager` usa `IRepository<Friendship>` e `IUnitOfWorkManager`; mockar repositórios e retornar `Task.FromResult<Friendship>(null!)` para branches inexistentes.

## Validação
- `dotnet build Eaf.sln --configuration Release` deve passar sem erros.
- `bash run-tests-with-coverage.sh` deve passar sem falhas.
- Cobertura não pode regredir abaixo do baseline P41.
- CI do PR deve passar.
