# P46 Coverage Audit — Prompt para Próxima Sessão

Execute o P46 coverage audit para o repositório `afonsoft/EAF` e continue melhorando a cobertura das classes que ainda estão abaixo de 90%.

## Contexto
- Repositório: `afonsoft/EAF` (clone local `/home/ubuntu/repos/EAF`)
- Branch atual: `feature/devin-20260715-priority46-coverage-audit` (a partir da `main` atual)
- Baseline P45: Line 96.2%, Branch 82.3%, Method 99.1% (13155 / 13674 linhas, 2412 / 2930 branches, 2082 / 2100 métodos)
- Testes: 4367 total, 4366 passando, 1 ignorado, 0 falhas
- Stack: xUnit + Shouldly + NSubstitute, BDD em português (`Dado/Quando/Então`)
- Build: `dotnet build Eaf.sln --configuration Release`
- Cobertura: `bash run-tests-with-coverage.sh` (requer `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet`)
- Métricas: `TestResults/CoverageReport/Summary.txt`

## Objetivos
1. Adicionar testes BDD em português para as classes de baixa cobertura restantes, priorizando as com maior impacto e menor percentual atual:
   - `Eaf.Middleware.Ldap.Authentication.LdapAuthenticationSource<T1, T2>` (61.3%)
   - `Eaf.Middleware.Web.MiddlewareWebCoreModule` (86.2%)
2. Manter ou aumentar a cobertura: Line >= 96.2%, Branch >= 82.3%, Method >= 99.1%.
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.

## Entregáveis
- Novos/ajustados arquivos de teste BDD em `test/`.
- `docs/development/session-summaries/eaf-session-summary-p46.md`.
- `docs/development/session-summaries/eaf-next-session-prompt-p47.md`.
- `README.md` e `README_pt.md` atualizados com as novas métricas.
- `.agents/MEMORY.md` atualizado com novos gotchas de P46.
- PR para `main` com CI verificado.

## Notas técnicas
- `LdapAuthenticationSource` continua limitado pelos ramos Windows-only (`PrincipalContext`, `UserPrincipal`, `ValidateCredentials`, `SearchWithLimit`), que não executam no Linux. Foque nos ramos restantes acessíveis sem `System.DirectoryServices.AccountManagement`: exceções de `CreateLdapContext`, `GetUsersAsync` com resultados vazios/inválidos, `TryAuthenticateAsync` com `Connected` false, e `UpdateUserAsync` com atributos ausentes.
- `MiddlewareWebCoreModule` ainda tem branches não cobertos nos loops de `PostInitialize` que removem jobs recorrentes/falhos do Hangfire (requer `JobStorage` configurado) e na configuração de `RedisStorage` quando `Database:Provider` não é `SqlServer`. Testar `SetAppFolders` com `ContentRootPath` nulo e `WebRootFileProvider` que cause `CompositeFileProvider` a lançar exceção (caso seja possível sem alterar produção).
- `EafMiddlewareCoreSampleAppModule` e `WebContentDirectoryFinder` atingiram 100% no P45; não precisam de mais testes salvo regressão.

## Validação
- `dotnet build Eaf.sln --configuration Release` deve passar sem erros.
- `bash run-tests-with-coverage.sh` deve passar sem falhas.
- Cobertura não pode regredir abaixo do baseline P45.
- CI do PR deve passar.
