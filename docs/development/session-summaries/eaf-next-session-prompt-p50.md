# EAF Next Session Prompt P50 - Coverage Audit

## Goal
Manter ou aumentar a cobertura de código do `afonsoft/EAF`, adicionando testes BDD em português (`Dado/Quando/Então`) sem alterar código de produção, exceto bugs bloqueantes documentados.

## Baseline P49 (após execução)
| Métrica | Valor |
|---------|-------|
| Line coverage | 96.3% (13181 / 13680) |
| Branch coverage | 82.9% (2431 / 2930) |
| Method coverage | 99.2% (2085 / 2101) |
| Testes | 4393 total, 4392 passando, 1 ignorado |
| Build warnings | 140 |

## Classes de baixa cobertura restantes (foco P50)
| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 61.8% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 86.2% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Auditing.NamespaceStripper` | 93.1% | Eaf.Middleware.Application |
| `Eaf.Middleware.Authorization.Users.UserAppService` | 91.6% | Eaf.Middleware.Application |
| `Eaf.Middleware.Web.Controllers.TokenAuthController` | 90.1% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Worker.MiddlewareWorkerModule` | 91.7% | Eaf.Middleware.Worker |
| `Eaf.WebHooks.EafWebHookReceiver` | 90.9% | Eaf.Middleware.Web.Core |
| `Abp.Runtime.Caching.Sqlite.DbCommandPool` | 94.7% | Eaf.SqliteCache |

## Tarefas
1. Adicionar testes BDD para ramos acessíveis das classes listadas acima.
2. Manter ou aumentar as métricas:
   - Line coverage >= 96.3%
   - Branch coverage >= 82.9%
   - Method coverage >= 99.2%
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p50.md`, `docs/development/session-summaries/eaf-next-session-prompt-p51.md` e `.agents/MEMORY.md`.
6. Criar PR para `main`.

## Notas e restrições conhecidas
- `LdapAuthenticationSource` e `MiddlewareWebCoreModule` possuem ramos que continuam inalcançáveis no Linux (conexão LDAP real, infra Hangfire/Redis/SQL Server, e `??` fallback no `.ctor` normalizado por `AppConfigurations.Get`).
- `NamespaceStripper` possui ramos de `StripGenericNamespace` com múltiplos tipos genéricos que podem ser cobertos sem infraestrutura externa.
- `UserAppService` e `TokenAuthController` possuem métodos com branches de validação e mapeamento DTO que podem ser testados com mocks.
- `MiddlewareWorkerModule` e `EafWebHookReceiver` possuem branches de inicialização e notificações que podem ser acionados com `IocManager` e `IWebHookPublisher` substituíveis.
- `DbCommandPool` possui branches de `TryTake`/`Add` e `Dispose` que podem ser cobertos com testes de concorrência e limite de pool.

## Comandos de verificação
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Critérios de aceitação
- `dotnet build Eaf.sln --configuration Release` passa com 0 erros.
- `bash run-tests-with-coverage.sh` passa e a cobertura não regrediu.
- PR aberto para `main` com CI verde.
