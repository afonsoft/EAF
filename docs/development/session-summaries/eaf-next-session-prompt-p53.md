# EAF Next Session Prompt P53 - Coverage Audit

## Goal

Manter ou aumentar a cobertura de código do `afonsoft/EAF`, adicionando testes BDD em português (`Dado/Quando/Então`) sem alterar código de produção, exceto bugs bloqueantes documentados.

## Baseline P52 (após execução)

| Métrica | Valor |
|---------|-------|
| Line coverage | 96.6% (13216 / 13670) |
| Branch coverage | 83.6% (2417 / 2888) |
| Method coverage | 99.3% (2142 / 2156) |
| Testes | 4416 total, 4415 passando, 1 ignorado |
| Build warnings | 159 |

## Classes de baixa cobertura restantes (foco P53)

| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 59.2% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.Controllers.TokenAuthController` | 89.9% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 87.3% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Authorization.Users.UserAppService` | 90.5% | Eaf.Middleware.Application |
| `Eaf.Middleware.Authorization.Users.UserManager` | 90.7% | Eaf.Middleware.Application |
| `Eaf.Middleware.Friendships.FriendshipAppService` | 90.9% | Eaf.Middleware.Application |
| `Eaf.Middleware.Authorization.Permissions.PermissionAppService` | 92.5% | Eaf.Middleware.Application |
| `Eaf.Middleware.MiddlewareCoreModule` | 93.9% | Eaf.Middleware.Core |
| `Eaf.Middleware.AzureActiveDirectory.Authentication.AzureActiveDirectoryAuthenticationSource<T1, T2>` | 91.2% | Eaf.Middleware.AzureActiveDirectory |
| `Eaf.Middleware.Ldap.Configuration.LdapSettings` | 91.8% | Eaf.Middleware.Ldap |
| `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` | 92.8% | Eaf.Log4NetServiceBus |

## Tarefas

1. Adicionar testes BDD para ramos acessíveis das classes listadas acima.
2. Manter ou aumentar as métricas:
   - Line coverage >= 96.6%
   - Branch coverage >= 83.6%
   - Method coverage >= 99.3%
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p53.md`, `docs/development/session-summaries/eaf-next-session-prompt-p54.md` e `.agents/MEMORY.md`.
6. Criar PR para `main`.

## Notas e restrições conhecidas

- `LdapAuthenticationSource` e `MiddlewareWebCoreModule` possuem ramos inalcançáveis no Linux (conexão LDAP real, infra Hangfire/Redis/SQL Server e `??` fallback normalizado).
- `TokenAuthController`, `UserAppService`, `UserManager`, `FriendshipAppService` e `MiddlewareCoreModule` possuem branches de validação, mapeamento DTO e fluxo de negócio acessíveis com mocks.
- `PermissionAppService` 92.5% tem branch `permission.Children == null` inacessível; documentar se não houver outra melhoria.
- `AzureActiveDirectoryAuthenticationSource` possui branches de fallback e parsing de claims acessíveis com mocks.
- `ServiceBusQueueAppender` tem branches de erro/exception e fallback que podem ser cobertos com mocks.
- `LdapSettings` tem branches de validação e parsing de configuração acessíveis.

## Comandos de verificação

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Critérios de aceitação

- `dotnet build Eaf.sln --configuration Release` passa com 0 erros.
- `bash run-tests-with-coverage.sh` passa e a cobertura não regrediu.
- PR aberto para `main` com CI verde.
