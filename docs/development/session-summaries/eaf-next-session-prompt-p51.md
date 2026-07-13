# EAF Next Session Prompt P51 - Coverage Audit

## Goal

Manter ou aumentar a cobertura de código do `afonsoft/EAF`, adicionando testes BDD em português (`Dado/Quando/Então`) sem alterar código de produção, exceto bugs bloqueantes documentados.

## Baseline P50 (após execução)

| Métrica | Valor |
|---------|-------|
| Line coverage | 96.4% (13212 / 13699) |
| Branch coverage | 83.0% (2427 / 2924) |
| Method coverage | 99.4% (2098 / 2110) |
| Testes | 4397 total, 4396 passando, 1 ignorado |
| Build warnings | 142 |

## Classes de baixa cobertura restantes (foco P51)

| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 61.8% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 86.2% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Web.Controllers.TokenAuthController` | 90.2% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Authorization.Users.UserAppService` | 91.6% | Eaf.Middleware.Application |
| `Eaf.Middleware.Worker.MiddlewareWorkerModule` | 91.7% | Eaf.Middleware.Worker |
| `Eaf.Middleware.Worker.VirtualFileSystem.WorkerContentFileProvider` | 91.4% | Eaf.Middleware.Worker |
| `Eaf.Middleware.Core.Authentication.External.AuthZero.AuthZeroAuthProviderApi` | 92.3% | Eaf.Middleware.Core |
| `Eaf.Middleware.MiddlewareCoreModule` | 93.9% | Eaf.Middleware.Core |
| `Eaf.Middleware.Authorization.Users.Profile.ProfileAppService` | 93.2% | Eaf.Middleware.Application |
| `Eaf.Middleware.Authorization.Permissions.PermissionAppService` | 92.5% | Eaf.Middleware.Application |
| `Eaf.Middleware.Chat.ChatMessageManager` | 92.4% | Eaf.Middleware.Application |
| `Eaf.Middleware.Friendships.FriendshipAppService` | 90.9% | Eaf.Middleware.Application |
| `Eaf.Middleware.Ldap.Configuration.LdapSettings` | 91.8% | Eaf.Middleware.Ldap |
| `Eaf.Hosting.Configuration.EafKeyVaultConfigurationProvider` | 93.7% | Eaf.Hosting |
| `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` | 92.8% | Eaf.Log4NetServiceBus |

## Tarefas

1. Adicionar testes BDD para ramos acessíveis das classes listadas acima.
2. Manter ou aumentar as métricas:
   - Line coverage >= 96.4%
   - Branch coverage >= 83.0%
   - Method coverage >= 99.4%
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p51.md`, `docs/development/session-summaries/eaf-next-session-prompt-p52.md` e `.agents/MEMORY.md`.
6. Criar PR para `main`.

## Notas e restrições conhecidas

- `LdapAuthenticationSource` e `MiddlewareWebCoreModule` possuem ramos que continuam inalcançáveis no Linux (conexão LDAP real, infra Hangfire/Redis/SQL Server, e `??` fallback no `.ctor` normalizado por `AppConfigurations.Get`).
- `TokenAuthController` e `UserAppService` possuem branches de validação, mapeamento DTO e fluxo de login/2FA que podem ser testados com mocks.
- `MiddlewareWorkerModule` e `WorkerContentFileProvider` possuem branches de inicialização de folders/provedores físicos que podem ser acionados com mocks de `IHostEnvironment` e `IFileProvider`.
- `AuthZeroAuthProviderApi`, `FriendshipAppService`, `ChatMessageManager`, `PermissionAppService`, `ProfileAppService` e `MiddlewareCoreModule` possuem branches de validação e mapeamento DTO acessíveis sem infraestrutura externa.
- `ServiceBusQueueAppender` e `EafKeyVaultConfigurationProvider` têm branches de erro/exception e fallback que podem ser cobertos com mocks.

## Comandos de verificação

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Critérios de aceitação

- `dotnet build Eaf.sln --configuration Release` passa com 0 erros.
- `bash run-tests-with-coverage.sh` passa e a cobertura não regrediu.
- PR aberto para `main` com CI verde.
