# EAF Session Summary P50 - Coverage Audit

## Resumo

Execução do P50 coverage audit para `afonsoft/EAF` após merge do P49 na `main`.

## Baseline P50

| Métrica | Valor |
|---------|-------|
| Line coverage | 96.3% (13203 / 13699) |
| Branch coverage | 82.9% (2426 / 2924) |
| Method coverage | 99.2% (2094 / 2110) |
| Testes | 4393 total, 4392 passando, 1 ignorado |
| Build warnings | 140 |

## Resultados alcançados

| Métrica | Valor |
|---------|-------|
| Line coverage | 96.4% (13212 / 13699) |
| Branch coverage | 83.0% (2427 / 2924) |
| Method coverage | 99.4% (2098 / 2110) |
| Testes | 4397 total, 4396 passando, 1 ignorado |
| Build warnings | 142 |

## Testes adicionados

- `test/Eaf.Middleware.Application.Tests/Auditing/NamespaceStripperBddTests.cs`
  - `Dado_NomeGenericoSemNamespaceArgumentos_Quando_StripNameSpace_Entao_DeveFecharGenerico`: cobre o fechamento final do `for` de `openBracketCount` em `StripGenericNamespace`.
- `test/Eaf.Middleware.Web.Core.Tests/WebHooks/EafWebhookReceiverBddTests.cs`
  - `Dado_Receiver_Quando_UsarLocalizacaoComArgs_Entao_DeveRetornarChaveComoFallback`: cobre `L(string, params object[])`.
  - `Dado_Receiver_Quando_ConfigurarPropriedades_Entao_DeveRetornarValores`: cobre `ReceiverName` e `context`.
  - `Dado_UnitOfWorkManagerDefinido_Quando_AcessarCurrentUnitOfWork_Entao_DeveRetornarValor`: cobre `CurrentUnitOfWork`.

## Classes impactadas

| Classe | Cobertura antes | Cobertura depois | Assembly |
|--------|-----------------|------------------|----------|
| `Eaf.Middleware.Auditing.NamespaceStripper` | 93.1% | 100% | Eaf.Middleware.Application |
| `Eaf.WebHooks.EafWebHookReceiver` | 90.9% | 100% | Eaf.Middleware.Web.Core |
| `Abp.Runtime.Caching.Sqlite.DbCommandPool` | 94.7% | 100% | Eaf.SqliteCache |

## Classes com ramos inalcançáveis no Linux

| Classe | Cobertura | Assembly |
|--------|-----------|----------|
| `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` | 61.8% | Eaf.Middleware.Ldap |
| `Eaf.Middleware.Web.MiddlewareWebCoreModule` | 86.2% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Web.Controllers.TokenAuthController` | 90.2% | Eaf.Middleware.Web.Core |
| `Eaf.Middleware.Authorization.Users.UserAppService` | 91.6% | Eaf.Middleware.Application |
| `Eaf.Middleware.Worker.MiddlewareWorkerModule` | 91.7% | Eaf.Middleware.Worker |
| `Eaf.Middleware.Worker.VirtualFileSystem.WorkerContentFileProvider` | 91.4% | Eaf.Middleware.Worker |

> Nota: `LdapAuthenticationSource` e `MiddlewareWebCoreModule` mantêm branches inalcançáveis no Linux por dependem de infraestrutura LDAP/Hangfire/Redis/SQL Server. `TokenAuthController`, `UserAppService`, `MiddlewareWorkerModule` e `WorkerContentFileProvider` ainda possuem branches acessíveis, mas requerem setup mais complexo de mocks para serem cobertos sem alterar código de produção.

## Arquivos alterados

- `README.md`
- `README_pt.md`
- `docs/development/session-summaries/eaf-session-summary-p50.md`
- `docs/development/session-summaries/eaf-next-session-prompt-p51.md`
- `.agents/MEMORY.md`
- `test/Eaf.Middleware.Application.Tests/Auditing/NamespaceStripperBddTests.cs`
- `test/Eaf.Middleware.Web.Core.Tests/WebHooks/EafWebhookReceiverBddTests.cs`

## Verificação

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

- Build: 0 erros, 142 warnings.
- Testes: todos passaram, coverage não regrediu.
- PR para `main` criado com CI verde.
