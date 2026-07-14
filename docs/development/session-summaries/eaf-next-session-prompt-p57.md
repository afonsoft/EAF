# EAF Next Session Prompt P57 - Coverage Audit

## Contexto

O P56 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.6% (13273 / 13589) |
| Branch coverage | 87.2% (2502 / 2868) |
| Method coverage | 99.6% (2155 / 2162) |
| Tests | 4516 total, 4515 passando, 1 ignorado |
| Build warnings | 154 |

Branch ativa: `feature/devin-20260713-priority57-coverage-audit` (a criar a partir de `origin/main` no commit do P56).

## Objetivo

Continuar o coverage audit adicionando testes BDD (`Dado/Quando/Então` em português) para os ramos acessíveis das classes listadas abaixo, mantendo ou aumentando as métricas atuais.

## Tarefas

1. Adicionar/ajustar testes BDD para ramos acessíveis das seguintes classes (foco nas de menor cobertura e com maior impacto):

   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (59.2%)
   - `Eaf.Middleware.Ldap.Configuration.LdapSettings` (91.8%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (87.3%)
   - `Eaf.Middleware.Web.Controllers.TokenAuthController` (90.9%)
   - `Eaf.Middleware.Authorization.Permissions.PermissionAppService` (92.5%)
   - `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (92.8%)
   - `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` (93.5%)
   - `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (95.2%)
   - `Eaf.AspNetCore.Hangfire.Configuration.EafHangfireApplicationBuilderExtensions` (96.7%)
   - `Eaf.AspNetCore.Hangfire.EafHangfireAuthorizationFilter` (97.7%)
   - `Eaf.Middleware.MiddlewareAppServiceBase` (97.3%)
   - `Eaf.Runtime.Caching.SqlServer.EafSqlServerCache` (97.4%)
   - `Eaf.AspNetCore.SignalR.Chat.ChatHub` (97.4%)
   - `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (97.2%)
   - `Eaf.Middleware.Worker.EafServiceCollectionExtensions` (98.1%)
   - `Eaf.Middleware.Worker.EafWorkerBase` (98.6%)
   - `Eaf.Middleware.MultiTenancy.TenantAppService` (98.4%)
   - `Eaf.Middleware.Configuration.Host.HostSettingsAppService` (99.3%)
   - `Eaf.Middleware.Chat.ChatMessageManager` (99.1%)
   - `Eaf.Middleware.Authorization.Accounts.AccountAppService` (98.2%)
   - `Eaf.Middleware.Authorization.Roles.RoleAppService` (98.6%)
   - `Eaf.Middleware.Web.Controllers.ProfileControllerBase` (98.2%)
   - `Eaf.Middleware.MultiTenancy.TenantAddress` (98%)
   - `Eaf.Castle.Logging.SerilogIntegration.SerilogLogger` (98.8%)

2. Manter ou aumentar as métricas:
   - Line coverage >= 97.6%
   - Branch coverage >= 87.2%
   - Method coverage >= 99.6%

3. Build Release e rodar `run-tests-with-coverage.sh`:

   ```bash
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
   ```

4. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p57.md`, `docs/development/session-summaries/eaf-next-session-prompt-p58.md` e `.agents/MEMORY.md` com as métricas finais e notas.

5. Criar PR para `main`.

## Restrições

- Não modificar código de produção, exceto bugs bloqueantes documentados.
- Não modificar `.github/workflows/`.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`).
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.

## Notas P56 (aprendizados)

- `HostSettingsAppService` possui muitos ramos em `GetAllSettings`/`UpdateAllSettings` relacionados a `ExternalLoginProviderSettings`, `TimeZoneId`, `LogDeleter`, `LoginImpersonator` e sub-DTOs nulos. JSON válido para todos os providers e claims mapping cobre a maioria dos ramos restantes.
- `ChatAppService` cobre `Side` das mensagens de grupo e `MarkAllUnreadMessagesOfUserAsRead` com tenants distintos.
- `EafHostBuilderExtensions` (Core e Worker) cobre `configureLogger` nulo e `prefix` vazio usando `HostBuilder` real em diretório temporário.
- `EafServiceCollectionExtensions.AddEaf<TStartupModule>()` sem `optionsAction` cobre o ramo `optionsAction == null`.
- `AzureActiveDirectoryAuthenticationSource` normaliza e-mails sem `@` a partir de `UserPrincipalName`/`UserName`.
- Classes com `LdapConnection` real, Hangfire/Redis/SQL Server, builders `sealed`, MSAL ou SignalR `Hub` têm ramos inacessíveis no ambiente Linux sem alterar código de produção. Documentar como inalcançáveis.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p56.md`
- `TestResults/CoverageReport/Summary.txt`
- `TestResults/CoverageReport/index.html`
