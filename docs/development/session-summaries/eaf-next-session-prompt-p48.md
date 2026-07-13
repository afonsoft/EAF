# EAF Next Session Prompt P48 - Coverage Audit

## Goal
Manter ou aumentar a cobertura de código do `afonsoft/EAF`, focando nas classes ainda com baixa cobertura, adicionando testes BDD em português (`Dado/Quando/Então`) sem alterar código de produção, exceto bugs bloqueantes documentados.

## Baseline P47 (após execução)
| Métrica | Valor |
|---------|-------|
| Line coverage | 96.2% (13160 / 13674) |
| Branch coverage | 82.8% (2427 / 2930) |
| Method coverage | 99.1% (2083 / 2100) |
| Testes | 4388 total, 4387 passando, 1 ignorado |
| Build warnings | 141 |

## Classes de baixa cobertura restantes (foco P48)
| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 61.8% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 86.2% | Eaf.Middleware.Web.Core |

## Tarefas
1. Adicionar testes BDD para ramos ainda não cobertos em `LdapAuthenticationSource` e/ou `MiddlewareWebCoreModule` que sejam acessíveis no ambiente Linux.
2. Manter ou aumentar as métricas:
   - Line coverage >= 96.2%
   - Branch coverage >= 82.8%
   - Method coverage >= 99.1%
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p48.md`, `docs/development/session-summaries/eaf-next-session-prompt-p49.md` e `.agents/MEMORY.md`.
6. Criar PR para `main`.

## Notas e restrições conhecidas
- `LdapAuthenticationSource` possui ramos Windows-only (`CreatePrincipalContext`, `UpdateUserFromPrincipal`, `ValidateCredentials`, `SearchWithLimit`) que não são executáveis no Linux.
- `MiddlewareWebCoreModule` possui `PostInitialize` com `recurringJobs`/`failedJobs` que exigem infraestrutura Hangfire/SQL Server/Redis real para serem cobertos.
- O bloco `catch` de `CompositeFileProvider` em `SetAppFolders` não é facilmente acionável sem alterar produção.
- O ramo `RedisConnectionString` nulo (`?? "localhost"`) em `MiddlewareWebCoreModule.PostInitialize` não é atingível no Linux porque o construtor `RedisStorage` lança exceção antes de `GetConnection`.

## Comandos de verificação
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Critérios de aceitação
- `dotnet build Eaf.sln --configuration Release` passa com 0 erros.
- `bash run-tests-with-coverage.sh` passa e a cobertura não regrediu.
- PR aberto para `main` com CI verde.
