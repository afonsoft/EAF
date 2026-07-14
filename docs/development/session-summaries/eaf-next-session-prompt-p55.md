# EAF Next Session Prompt P55 - Coverage Audit

## Goal

Manter ou aumentar a cobertura de código do `afonsoft/EAF`, adicionando testes BDD em português (`Dado/Quando/Então`) sem alterar código de produção, exceto bugs bloqueantes documentados.

## Baseline P54 (após execução)

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.2% (13235 / 13604) |
| Branch coverage | 85.1% (2440 / 2866) |
| Method coverage | 99.5% (2150 / 2159) |
| Testes | 4467 total, 4466 passando, 1 ignorado |
| Build warnings | 127 |

## Classes de baixa cobertura restantes (foco P55)

| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 59.2% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 87.3% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Web.Controllers.TokenAuthController` | 90.9% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Ldap.Configuration.LdapSettings` | 91.8% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Authorization.Permissions.PermissionAppService` | 92.5% | Eaf.Middleware.Application |
| `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` | 92.8% | Eaf.Log4NetServiceBus |
| `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` | 93.5% | Eaf.Middleware.AzureActiveDirectory |
| `Eaf.Middleware.Web.Controllers.ChatController` | 94.1% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Chat.ChatAppService` | 94.3% | Eaf.Middleware.Application |
| `Eaf.Runtime.Caching.SqlServer.EafSqlServerCache` | 94.8% | Eaf.SqlServerCache |
| `Abp.Runtime.Caching.Sqlite.EafSqliteCache` | 94.9% | Eaf.SqliteCache |
| `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` | 95.2% | Eaf.Middleware.Core |
| `Eaf.Middleware.MultiTenancy.TenantManager` | 96.0% | Eaf.Middleware.Core |
| `Eaf.Middleware.Authorization.Users.UserEmailer` | 96.2% | Eaf.Middleware.Application |
| `Eaf.Middleware.Configuration.EafHostBuilderExtensions` | 96.2% | Eaf.Middleware.Core |
| `Eaf.Middleware.Worker.EafServiceCollectionExtensions` | 96.2% | Eaf.Middleware.Worker |
| `Eaf.Middleware.Localization.LanguageAppService` | 96.3% | Eaf.Middleware.Application |
| `Eaf.Middleware.Core.Authentication.External.RemoteAuthenticationContextExtensions` | 96.4% | Eaf.Middleware.Core |
| `Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host.HostRoleAndUserCreator` | 96.6% | Eaf.EntityFrameworkCore.SampleApp.Tests |
| `Eaf.AspNetCore.Hangfire.Configuration.EafHangfireApplicationBuilderExtensions` | 96.7% | Eaf.Middleware.Hangfire |
| `Eaf.Middleware.MultiTenancy.TenantAppService` | 96.8% | Eaf.Middleware.Application |
| `Eaf.Middleware.Notifications.NotificationAppService` | 96.8% | Eaf.Middleware.Application |
| `Eaf.AspNetCore.Hangfire.EafHangfireAuthorizationFilter` | 96.9% | Eaf.Middleware.Hangfire |
| `Eaf.Middleware.Web.Authentication.DefaultExternalLoginInfoManager` | 97.1% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.MiddlewareAppServiceBase` | 97.3% | Eaf.Middleware.Application |
| `Eaf.Middleware.Configuration.Host.HostSettingsAppService` | 97.4% | Eaf.Middleware.Application |
| `Eaf.AspNetCore.SignalR.Chat.ChatHub` | 97.5% | Eaf.Middleware.Web.Core |
| `Eaf.Controllers.AboutController` | 97.6% | Eaf.Middleware.Web.Core |
| `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` | 97.9% | Eaf.OpenTelemetry |
| `Eaf.Middleware.Web.Controllers.FileController` | 97.9% | Eaf.Middleware.Web.Core |

## Tarefas

1. Adicionar testes BDD para ramos acessíveis das classes listadas acima.
2. Manter ou aumentar as métricas:
   - Line coverage >= 97.2%
   - Branch coverage >= 85.1%
   - Method coverage >= 99.5%
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p55.md`, `docs/development/session-summaries/eaf-next-session-prompt-p56.md` e `.agents/MEMORY.md`.
6. Criar PR para `main`.

## Notas e restrições conhecidas

- `LdapAuthenticationSource` e `MiddlewareWebCoreModule` possuem ramos inalcançáveis no Linux (conexão LDAP real, infra Hangfire/Redis/SQL Server e `??` fallback normalizado).
- `TokenAuthController` ainda tem branches complexos de `ExternalAuthenticate`/`GetExternalUserInfo`/`RegisterExternalUserAsync` que podem ser alcançados com mocks.
- `PermissionAppService` 92.5% tem branch `permission.Children == null` inacessível; documentar se não houver outra melhoria.
- `ServiceBusQueueAppender` `OnClose` catch é inalcançável sem modificar `ServiceBusConnection`/`ClientEntity`.
- `AzureActiveDirectoryAuthenticationSource` tem branches de `CreateGraphServiceClient` e `TryAuthenticateAsync` successo que são inacessíveis porque MSAL builders são `sealed` (não mockáveis com `NSubstitute` sem testes de integração).
- `ChatController` e `ChatAppService` têm branches de validação e listagem acessíveis.
- `EafSqlServerCache` e `EafSqliteCache` têm branches de retry/expiração e fallback de conexão.
- `OpenIdConnectAuthProviderApi` ainda tem branches de `ValidateTokenInternal` e `GetUserInfo` acessíveis.
- `TenantManager`, `TenantAppService`, `LanguageAppService`, `UserEmailer`, `NotificationAppService`, `HostSettingsAppService` e `AboutController` têm branches de validação e fallback.

## Comandos de verificação

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Critérios de aceitação

- `dotnet build Eaf.sln --configuration Release` passa com 0 erros.
- `bash run-tests-with-coverage.sh` passa e a cobertura não regrediu.
- PR aberto para `main` com CI verde.
