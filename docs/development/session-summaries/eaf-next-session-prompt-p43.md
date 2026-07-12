# P43 Coverage Audit — Prompt para Próxima Sessão

Execute o P43 coverage audit para o repositório `afonsoft/EAF` e atualize o README com as novas métricas.

## Contexto
- Repositório: `afonsoft/EAF` (clone local `/home/ubuntu/repos/EAF`)
- Branch atual: `feature/devin-20260712-priority42-coverage-audit` (ou a branch do P43 a partir da `main` atual)
- Baseline P42: Line 95.5%, Branch 80.9%, Method 98.6% (13061 / 13670 linhas, 2371 / 2930 branches, 2071 / 2100 métodos)
- Testes: 4296 total, 4295 passando, 1 ignorado, 0 falhas
- Stack: xUnit + Shouldly + NSubstitute, BDD em português (`Dado/Quando/Então`)
- Build: `dotnet build Eaf.sln --configuration Release`
- Cobertura: `bash run-tests-with-coverage.sh` (requer `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet`)
- Métricas: `TestResults/CoverageReport/Summary.txt`

## Objetivos
1. Adicionar testes BDD em português para as classes de baixa cobertura restantes, priorizando as com maior impacto e menor percentual atual:
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
2. Manter ou aumentar a cobertura: Line >= 95.5%, Branch >= 80.9%, Method >= 98.6%.
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.

## Entregáveis
- Novos/ajustados arquivos de teste BDD em `test/`.
- `docs/development/session-summaries/eaf-session-summary-p43.md`.
- `docs/development/session-summaries/eaf-next-session-prompt-p44.md`.
- `README.md` e `README_pt.md` atualizados com as novas métricas.
- `.agents/MEMORY.md` atualizado com novos gotchas de P43.
- PR para `main` com CI verificado.

## Notas técnicas
- `LdapAuthenticationSource` continua limitado: `LdapConnection` é `IsFinal`/`IsVirtual`, então foque em `CreateLdapContext` e `TryAuthenticateAsync` exceções; `ILdapSearchResults` pode ser substituído.
- `EafMiddlewareCoreSampleAppModule` e classes de `Seed` do `SampleApp` dependem de `DbContext` e `SampleAppDbContext`; usar `DbContext` em memória ou `UseInMemoryDatabase` para cobrir branches sem conexão real.
- `WebContentDirectoryFinder.CalculateContentRootFolder` procura `src/Eaf.Middleware.Web.Host`; testar a exceção `DirectoryNotFoundException` e a branch `coreAssemblyDirectoryPath == null` com `Assembly.GetEntryAssembly()` nulo.
- `MiddlewareWebCoreModule` constructor faz fallback de variáveis de ambiente, mas `AppConfigurations.Get` normaliza `ASPNETCORE_ENVIRONMENT`/`EAF_ENVIRONMENT`/`ASPNET_ENV`/`DOTNET_ENVIRONMENT`; testar `SetAppFolders` com `ContentRootPath` nulo e `PostInitialize` com `RedisStorage` `ConnectionString` padrão `localhost` (com timeout baixo).
- `CultureHelper` usa `CultureInfo.CurrentUICulture` para `IsRtl` e `UsingLunarCalendar`; alternar cultura com `ar-SA`/`zh-CN` e restaurar.
- `EntityHistoryConfigurationExtensions` são `SetExpiredHistoryEntityWoker` e `SetExpiredAuditWoker`; invocar em `IEntityHistoryConfiguration`/`IAuditingConfiguration` substituídos.
- `EafSqliteCache` e `DbCommandPool` usam `Microsoft.Data.Sqlite`; desabilitar pooling (`SqliteConnectionStringBuilder.Pooling = false`) e limpar pools após testes de exclusão.

## Validação
- `dotnet build Eaf.sln --configuration Release` deve passar sem erros.
- `bash run-tests-with-coverage.sh` deve passar sem falhas.
- Cobertura não pode regredir abaixo do baseline P42.
- CI do PR deve passar.
