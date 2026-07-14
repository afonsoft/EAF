# EAF Next Session Prompt P60 - Coverage Audit

## Contexto

O P59 foi concluído e mergeado. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.7% (13290 / 13589) |
| Branch coverage | 90.0% (2582 / 2868) |
| Method coverage | 99.8% (2158 / 2162) |
| Tests | 4585 total, 4584 passando, 1 ignorado |
| Build warnings | 161 |

Branch ativa: `feature/devin-20260715-priority60-coverage-audit` (a criar a partir de `origin/main` no commit do P59).

## Objetivo

Continuar o coverage audit adicionando testes BDD (`Dado/Quando/Então` em português) para os ramos acessíveis das classes ainda abaixo de 100%, mantendo ou aumentando as métricas atuais.

## Tarefas

1. Adicionar/ajustar testes BDD para ramos acessíveis das seguintes classes (foco nas de menor cobertura e com maior impacto):

   - `Eaf.Castle.Logging.SerilogIntegration.SerilogLogger` (98.8%)
   - `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (92.8%)
   - `Eaf.Middleware.Authorization.Permissions.PermissionAppService` (92.5%)
   - `Eaf.Middleware.Authorization.Roles.RoleAppService` (98.6%)
   - `Eaf.Middleware.Authorization.Users.UserAppService` (99.4%)
   - `Eaf.Middleware.Chat.ChatMessageManager` (99.1%)
   - `Eaf.Middleware.Configuration.Host.HostSettingsAppService` (99.3%)
   - `Eaf.Middleware.MiddlewareAppServiceBase` (97.3%)
   - `Eaf.Middleware.MultiTenancy.TenantAppService` (98.4%)
   - `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` (93.5%)
   - `Eaf.AspNetCore.Hangfire.Configuration.EafHangfireApplicationBuilderExtensions` (96.7%)
   - `Eaf.AspNetCore.Hangfire.EafHangfireAuthorizationFilter` (97.7%)
   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (59.2%)
   - `Eaf.Middleware.Ldap.Configuration.LdapSettings` (91.8%)
   - `Eaf.AspNetCore.SignalR.Chat.ChatHub` (97.4%)
   - `Eaf.Middleware.Web.Controllers.TokenAuthController` (90.9%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (87.3%)
   - `Eaf.Runtime.Caching.SqlServer.EafSqlServerCache` (97.4%)
   - `Eaf.Runtime.Caching.Sqlite.EafSqliteCache` (96.6%)

2. Manter ou aumentar as métricas:
   - Line coverage >= 97.7%
   - Branch coverage >= 90.0%
   - Method coverage >= 99.8%

3. Build Release e rodar `run-tests-with-coverage.sh`:

   ```bash
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
   ```

4. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p60.md`, `docs/development/session-summaries/eaf-next-session-prompt-p61.md` e `.agents/MEMORY.md` com as métricas finais e notas.

5. Criar PR para `main`.

## Restrições

- Não modificar código de produção, exceto bugs bloqueantes documentados.
- Não modificar `.github/workflows/`.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`).
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.

## Notas P59 (aprendizados)

- `IRepository<UserToken, long>` configurado com `NSubstitute` `Returns` espera `Task<UserToken>`; ao retornar um `EafUserToken`, faça o cast: `Task.FromResult((UserToken)userToken)`.
- O construtor de `BinaryObject` prefixa `FileName` com `{Id}_`; asserções sobre `FileContentResult.FileDownloadName` devem usar `binaryObject.FileName` quando o parâmetro `fileName` for `null`.
- `EafOpenTelemetryOptions` lê `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_PROTOCOL` e `OTEL_SERVICE_NAME` das variáveis de ambiente e armazena em `OtlpVariables`.
- `ImpersonationManager.GetImpersonatedUserAndIdentity` reconstrói o item de cache a partir do repositório `UserToken` quando há cache miss; testar `Value` contendo `"{impersonatorTenantId}-{impersonatorUserId}"` e `Value` nulo.
- `WebLogAppService.GetLatestWebLogs` reconhece prefixos de nível de log `IMF`, `DBG`, `WRN`, `ERR`, `FAT`, `FTL`, além de nomes em maiúsculas e linhas sem prefixo.
- `AuditLogListExcelExporter.ExportToFile` usa ternário `_.Exception.IsNullOrEmpty() ? L("Success") : _.Exception`; para cobrir o ramo `false`, usar um audit log com `Exception` não vazio.
- `LanguageAppService.GetLanguages` retorna `DefaultLanguageName = null` quando nenhum idioma padrão é encontrado.
- `DefaultExternalLoginInfoManager.GetNameAndSurname` usa `nameClaim.Value` diretamente quando `givenName`/`surname` estão vazios e remove espaços no final.
- `EafWebhookReceiver` mantém `LocalizationSource` em cache por `CurrentCulture`/`CurrentUICulture`; mudar `SourceName` invalida o cache.
- `LdapAuthenticationSource`, `MiddlewareWebCoreModule`, `PermissionAppService`, `EafHangfireApplicationBuilderExtensions`, `EafHangfireAuthorizationFilter`, `ChatHub`, `TokenAuthController`, `EafSqlServerCache`, `EafSqliteCache` e `ServiceBusQueueAppender` possuem ramos dependentes de infraestrutura real (LDAP, Hangfire/Redis/SQL Server, SignalR) que são inacessíveis no Linux. Documentar como inalcançáveis quando apropriado.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p59.md`
- `TestResults/CoverageReport/Summary.txt`
- `TestResults/CoverageReport/index.html`
- `/tmp/p60_coverage_gaps.txt`
