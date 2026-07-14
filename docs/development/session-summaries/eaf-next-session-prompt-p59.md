# EAF Next Session Prompt P59 - Coverage Audit

## Contexto

O P58 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.7% (13284 / 13589) |
| Branch coverage | 89.1% (2556 / 2868) |
| Method coverage | 99.7% (2156 / 2162) |
| Tests | 4555 total, 4554 passando, 1 ignorado |
| Build warnings | 159 |

Branch ativa: `feature/devin-20260714-priority59-coverage-audit` (a criar a partir de `origin/main` no commit do P58).

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
   - `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (95.2%)
   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (59.2%)
   - `Eaf.Middleware.Ldap.Configuration.LdapSettings` (91.8%)
   - `Eaf.AspNetCore.SignalR.Chat.ChatHub` (97.4%)
   - `Eaf.Middleware.Web.Controllers.TokenAuthController` (90.9%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (87.3%)
   - `Eaf.Middleware.Worker.EafServiceCollectionExtensions` (98.1%)
   - `Eaf.Middleware.Worker.EafWorkerBase` (98.6%)
   - `Eaf.Runtime.Caching.SqlServer.EafSqlServerCache` (97.4%)
   - `Eaf.Middleware.Application.AuditLogListExcelExporter`
   - `Eaf.Middleware.Application.ChatMessageListExcelExporter`
   - `Eaf.Middleware.Application.WebLogAppService`
   - `Eaf.Middleware.Application.LanguageAppService`
   - `Eaf.Middleware.Web.Core.AboutController`
   - `Eaf.Middleware.Web.Core.DefaultExternalLoginInfoManager`
   - `Eaf.Middleware.Web.Core.EafStartupConfigurationExtensions`
   - `Eaf.Middleware.Core.EafHangfireAuthorizationFilter`
   - `Eaf.Middleware.Core.ImpersonationManager`

2. Manter ou aumentar as métricas:
   - Line coverage >= 97.7%
   - Branch coverage >= 89.1%
   - Method coverage >= 99.7%

3. Build Release e rodar `run-tests-with-coverage.sh`:

   ```bash
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
   ```

4. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p59.md`, `docs/development/session-summaries/eaf-next-session-prompt-p60.md` e `.agents/MEMORY.md` com as métricas finais e notas.

5. Criar PR para `main`.

## Restrições

- Não modificar código de produção, exceto bugs bloqueantes documentados.
- Não modificar `.github/workflows/`.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`).
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.

## Notas P58 (aprendizados)

- `Serilog.ILogger` não pode ser mockado com `NSubstitute` quando o objetivo é testar `SerilogLogger` — usar um logger Serilog real configurado com `LevelAlias.Off`.
- `ChatMessageManager.Delete(sharedMessageId)` sempre chama `_chatMessageRepository.Delete(...)`, mesmo quando nenhuma mensagem é encontrada; a asserção correta é `Received(1)`.
- `HostSettingsAppService.UpdateAllSettings` só entra em `UpdateLdapSettingsAsync` quando `_ldapModuleConfig.IsEnabled` é `true`.
- `EafWorkerBase.L` retorna a chave bruta quando `args` é vazio (`Array.Empty<object>()`).
- `EafSqliteCache.ObjectToByteArray(null)` retorna um array vazio; `ByteArrayToObject(null/empty)` retorna `default`.
- `EafSqlServerCache.TryGetValue` possui um `catch` que pode ser coberto quando `IDistributedCache.GetAsync` lança exceção.
- `PermissionAppService` mantém o branch `permission.Children == null` inalcançável (usa `ImmutableList`); documentar, não forçar.
- `LdapAuthenticationSource`, `LdapSettings`, `MiddlewareWebCoreModule`, `TokenAuthController`, `EafHangfireApplicationBuilderExtensions`, `EafHangfireAuthorizationFilter`, `ChatHub` e `ServiceBusQueueAppender` possuem ramos dependentes de infraestrutura real (LDAP, Hangfire/Redis/SQL Server, MSAL, SignalR) que são inacessíveis no Linux. Documentar como inalcançáveis quando apropriado.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p58.md`
- `TestResults/CoverageReport/Summary.txt`
- `TestResults/CoverageReport/index.html`
- `/tmp/p59_coverage_gaps.txt`
