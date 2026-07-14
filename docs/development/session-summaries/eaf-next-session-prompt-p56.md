# EAF Next Session Prompt P56 - Coverage Audit

## Goal

Manter ou aumentar a cobertura de código do `afonsoft/EAF`, adicionando testes BDD em português (`Dado/Quando/Então`) sem alterar código de produção, exceto bugs bloqueantes documentados.

## Baseline P55 (após execução)

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.5% (13263 / 13593) |
| Branch coverage | 85.6% (2454 / 2866) |
| Method coverage | 99.6% (2155 / 2162) |
| Testes | 4492 total, 4491 passando, 1 ignorado |
| Build warnings | 129 |

## Classes de baixa cobertura restantes (foco P56)

| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 59.2% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 87.3% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Web.Controllers.TokenAuthController` | 90.9% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Authorization.Permissions.PermissionAppService` | 92.5% | Eaf.Middleware.Application |
| `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` | 92.8% | Eaf.Log4NetServiceBus |
| `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` | 93.5% | Eaf.Middleware.AzureActiveDirectory |
| `Eaf.Middleware.Chat.ChatAppService` | 94.3% | Eaf.Middleware.Application |
| `Abp.Runtime.Caching.Sqlite.EafSqliteCache` | 94.9% | Eaf.SqliteCache |
| `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` | 95.2% | Eaf.Middleware.Core |
| `Eaf.Middleware.Configuration.EafHostBuilderExtensions` | 96.2% | Eaf.Middleware.Core |
| `Eaf.Middleware.Configuration.EafHostBuilderExtensions` | 96.2% | Eaf.Middleware.Worker |
| `Eaf.AspNetCore.Hangfire.Configuration.EafHangfireApplicationBuilderExtensions` | 96.7% | Eaf.Middleware.Hangfire |
| `Eaf.AspNetCore.Hangfire.EafHangfireAuthorizationFilter` | 97.7% | Eaf.Middleware.Hangfire |
| `Eaf.Middleware.MiddlewareAppServiceBase` | 97.3% | Eaf.Middleware.Application |
| `Eaf.Runtime.Caching.SqlServer.EafSqlServerCache` | 97.4% | Eaf.SqlServerCache |
| `Eaf.AspNetCore.SignalR.Chat.ChatHub` | 97.5% | Eaf.Middleware.Web.Core |
| `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` | 97.2% | Eaf.OpenTelemetry |
| `Eaf.Middleware.Worker.EafServiceCollectionExtensions` | 98.1% | Eaf.Middleware.Worker |
| `Eaf.Middleware.MultiTenancy.TenantAppService` | 98.4% | Eaf.Middleware.Application |
| `Eaf.Middleware.Configuration.Host.HostSettingsAppService` | 99.3% | Eaf.Middleware.Application |

## Tarefas

1. Adicionar testes BDD para ramos acessíveis das classes listadas acima.
2. Manter ou aumentar as métricas:
   - Line coverage >= 97.5%
   - Branch coverage >= 85.6%
   - Method coverage >= 99.6%
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p56.md`, `docs/development/session-summaries/eaf-next-session-prompt-p57.md` e `.agents/MEMORY.md`.
6. Criar PR para `main`.

## Notas e restrições conhecidas

- `LdapAuthenticationSource` e `MiddlewareWebCoreModule` possuem ramos inalcançáveis no Linux (conexão LDAP real, infra Hangfire/Redis/SQL Server e `??` fallback normalizado).
- `TokenAuthController` ainda tem branches complexos de `ExternalAuthenticate`/`GetExternalUserInfo`/`RegisterExternalUserAsync` que podem ser alcançados com mocks.
- `PermissionAppService` 92.5% tem branch `permission.Children == null` inacessível; documentar se não houver outra melhoria.
- `ServiceBusQueueAppender` `OnClose` catch é inalcançável sem modificar `ServiceBusConnection`/`ClientEntity`.
- `AzureActiveDirectoryAuthenticationSource` tem branches de `CreateGraphServiceClient` e `TryAuthenticateAsync` sucesso que são inacessíveis porque MSAL builders são `sealed` (não mockáveis com `NSubstitute` sem testes de integração).
- `ChatAppService` e `EafSqliteCache` têm branches de retry/expiração e fallback de conexão acessíveis.
- `OpenIdConnectAuthProviderApi` ainda tem branches de `ValidateTokenInternal` e `GetUserInfo` acessíveis.
- `EafHostBuilderExtensions` (Core e Worker), `EafHangfireApplicationBuilderExtensions`, `EafOpenTelemetryServiceCollectionExtensions` e `EafServiceCollectionExtensions` (Worker) têm branches de fallback e exception acessíveis.
- `EafHangfireAuthorizationFilter`, `MiddlewareAppServiceBase`, `TenantAppService` e `HostSettingsAppService` têm branches de validação e fallback.

## Comandos de verificação

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Critérios de aceitação

- `dotnet build Eaf.sln --configuration Release` passa com 0 erros.
- `bash run-tests-with-coverage.sh` passa e a cobertura não regrediu.
- PR aberto para `main` com CI verde.
