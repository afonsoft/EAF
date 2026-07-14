# EAF Next Session Prompt P58 - Coverage Audit

## Contexto

O P57 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.7% (13280 / 13589) |
| Branch coverage | 87.5% (2510 / 2868) |
| Method coverage | 99.6% (2155 / 2162) |
| Tests | 4533 total, 4532 passando, 1 ignorado |
| Build warnings | 154 |

Branch ativa: `feature/devin-20260713-priority58-coverage-audit` (a criar a partir de `origin/main` no commit do P57).

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
   - `Eaf.Middleware.MultiTenancy.TenantAddress` (98%)
   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (59.2%)
   - `Eaf.Middleware.Ldap.Configuration.LdapSettings` (91.8%)
   - `Eaf.AspNetCore.SignalR.Chat.ChatHub` (97.4%)
   - `Eaf.Middleware.Web.Controllers.TokenAuthController` (90.9%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (87.3%)
   - `Eaf.Middleware.Worker.EafServiceCollectionExtensions` (98.1%)
   - `Eaf.Middleware.Worker.EafWorkerBase` (98.6%)
   - `Abp.Runtime.Caching.Sqlite.EafSqliteCache` (94.9%)
   - `Eaf.Runtime.Caching.SqlServer.EafSqlServerCache` (97.4%)

2. Manter ou aumentar as métricas:
   - Line coverage >= 97.7%
   - Branch coverage >= 87.5%
   - Method coverage >= 99.6%

3. Build Release e rodar `run-tests-with-coverage.sh`:

   ```bash
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
   PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
   ```

4. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p58.md`, `docs/development/session-summaries/eaf-next-session-prompt-p59.md` e `.agents/MEMORY.md` com as métricas finais e notas.

5. Criar PR para `main`.

## Restrições

- Não modificar código de produção, exceto bugs bloqueantes documentados.
- Não modificar `.github/workflows/`.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`).
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.

## Notas P57 (aprendizados)

- `NSubstitute` retorna `string.Empty` para membros `string` não configurados; usar `NullLocalizationManager.Instance` ou configurar `GetStringOrNull` para retornar `null` quando o fallback para a chave for esperado.
- `AccountAppService.Impersonate` lança `UserFriendlyException` para tenant inativo; a mensagem deve conter `TenantIdIsNotActive`.
- `RoleAppService.GetRoles` filtra por permissão e requer `Permissions` não nulo em todas as `Role` do `IQueryable`.
- `TenantAppService.GetTenantFeaturesForEdit` filtra features pelo escopo `FeatureScopes.Tenant`; o `ObjectMapper` para `List<FlatFeatureDto>` deve respeitar a contagem de itens filtrados.
- `AzureActiveDirectoryAuthenticationSource.CreateUserAsync`/`UpdateUserAsync` capturam `AbpException` do Graph e criam/atualizam um usuário básico.
- `ChatHub.DeleteMessage` retorna mensagem de não encontrado quando `SharedMessageId` é nulo; `SendMessage` retorna `InternalServerError` quando `UserId`/`GroupId` é zero.
- `EafWorkerBase.L` retorna a chave bruta quando `args` é nulo; `LocalizationSource` é cacheado.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry()` sem opções e sem exporters retorna um `IOpenTelemetryBuilder` configurado.
- `ServiceBusQueueAppender.SendBuffer` ignora envio quando `StorageType` é vazio.
- Classes com `LdapConnection` real, infraestrutura Hangfire/Redis/SQL Server, builders `sealed`, MSAL ou `Hub` SignalR têm ramos inacessíveis no Linux. Documentar como inalcançáveis em vez de alterar código de produção.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p57.md`
- `TestResults/CoverageReport/Summary.txt`
- `TestResults/CoverageReport/index.html`
