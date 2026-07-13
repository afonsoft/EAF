# EAF Next Session Prompt P52 - Coverage Audit

## Goal

Manter ou aumentar a cobertura de código do `afonsoft/EAF`, adicionando testes BDD em português (`Dado/Quando/Então`) sem alterar código de produção, exceto bugs bloqueantes documentados.

## Baseline P51 (após execução)

| Métrica | Valor |
|---------|-------|
| Line coverage | 96.4% (13184 / 13670) |
| Branch coverage | 83.0% (2399 / 2888) |
| Method coverage | 99.3% (2141 / 2156) |
| Testes | 4401 total, 4400 passando, 1 ignorado |
| Build warnings | 159 |

## Classes de baixa cobertura restantes (foco P52)

| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 59.2% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.Controllers.MiddlewareControllerBase` | 90.0% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Web.Controllers.TokenAuthController` | 89.9% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 87.3% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Authorization.Users.UserAppService` | 90.5% | Eaf.Middleware.Application |
| `Eaf.Middleware.Authorization.Users.UserManager` | 90.7% | Eaf.Middleware.Application |
| `Eaf.Middleware.Chat.ChatAppService` | 91.1% | Eaf.Middleware.Application |
| `Eaf.Middleware.Friendships.FriendshipAppService` | 90.9% | Eaf.Middleware.Application |
| `Eaf.Middleware.Chat.ChatMessageManager` | 92.4% | Eaf.Middleware.Application |
| `Eaf.Middleware.Authorization.Permissions.PermissionAppService` | 92.5% | Eaf.Middleware.Application |
| `Eaf.Middleware.Authorization.Users.Profile.ProfileAppService` | 93.2% | Eaf.Middleware.Application |
| `Eaf.Middleware.MiddlewareCoreModule` | 93.9% | Eaf.Middleware.Core |
| `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` | 91.2% | Eaf.Middleware.AzureActiveDirectory |
| `Eaf.Middleware.Ldap.Configuration.LdapSettings` | 91.8% | Eaf.Middleware.Ldap |
| `Eaf.Hosting.Configuration.EafKeyVaultConfigurationProvider` | 93.7% | Eaf.Hosting |
| `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` | 92.8% | Eaf.Log4NetServiceBus |
| `Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Host.DefaultLanguagesCreator` | 92.8% | Eaf.MiddlewareCore.SampleApp |
| `Eaf.MiddlewareCore.SampleApp.EntityFramework.Seed.Tenants.TenantRoleAndUserBuilder` | 93.4% | Eaf.MiddlewareCore.SampleApp |

## Tarefas

1. Adicionar testes BDD para ramos acessíveis das classes listadas acima.
2. Manter ou aumentar as métricas:
   - Line coverage >= 96.4%
   - Branch coverage >= 83.0%
   - Method coverage >= 99.3%
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p52.md`, `docs/development/session-summaries/eaf-next-session-prompt-p53.md` e `.agents/MEMORY.md`.
6. Criar PR para `main`.

## Notas e restrições conhecidas

- `LdapAuthenticationSource` e `MiddlewareWebCoreModule` possuem ramos que continuam inalcançáveis no Linux (conexão LDAP real, infra Hangfire/Redis/SQL Server e `??` fallback normalizado).
- `TokenAuthController`, `UserAppService`, `UserManager`, `ChatAppService`, `FriendshipAppService`, `ChatMessageManager`, `PermissionAppService`, `ProfileAppService` e `MiddlewareCoreModule` possuem branches de validação, mapeamento DTO e fluxo de negócio acessíveis com mocks.
- `MiddlewareControllerBase` possui branches de helpers/exception acessíveis sem infraestrutura externa.
- `AzureActiveDirectoryAuthenticationSource` possui branches de fallback e parsing de claims acessíveis com mocks.
- `ServiceBusQueueAppender` e `EafKeyVaultConfigurationProvider` têm branches de erro/exception e fallback que podem ser cobertos com mocks.
- `DefaultLanguagesCreator` e `TenantRoleAndUserBuilder` são seed helpers executados durante a inicialização do SampleApp.

## Comandos de verificação

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Critérios de aceitação

- `dotnet build Eaf.sln --configuration Release` passa com 0 erros.
- `bash run-tests-with-coverage.sh` passa e a cobertura não regrediu.
- PR aberto para `main` com CI verde.
