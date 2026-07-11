# Próxima Sessão P39 - EAF Coverage Audit

## Contexto

Sessão anterior: `feature/devin-20260711-priority38-coverage-audit`
Baseline (P38):
- Line: 90.4%
- Branch: 71.2%
- Method: 96.9%
- Covered lines: 12364 / 13672
- Covered branches: 2088 / 2932
- Covered methods: 2036 / 2100

## Objetivo

Continuar o coverage audit adicionando testes BDD em português (xUnit + Shouldly + NSubstitute) para as classes de baixa cobertura restantes, sem regredir o baseline P38.

## Targets prioritários

1. `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (66.6%)
2. `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
3. `Eaf.Middleware.Web.WebContentDirectoryFinder` (70.8%)
4. `Eaf.Middleware.Web.Controllers.TokenAuthController` (80.7%)

## Restrições

- Não modificar código de produção, exceto para corrigir bugs bloqueantes.
- Não modificar `.github/workflows/`.
- Não fazer push para `main`, `master` ou `develop`.
- Criar branch `feature/devin-YYYYMMDD-priority39-coverage-audit`.
- Testes em padrão BDD em português: `Dado_..._Quando_..._Entao_...`.
- Código em inglês.
- Manter cobertura >= baseline P38 (Line 90.4%, Branch 71.2%, Method 96.9%).

## Comandos de validação

```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Entregáveis

1. Branch `feature/devin-YYYYMMDD-priority39-coverage-audit`.
2. PR para `main`.
3. `docs/development/session-summaries/eaf-session-summary-p39.md`.
4. `docs/development/session-summaries/eaf-next-session-prompt-p40.md` (se aplicável).
5. Atualização de `.agents/MEMORY.md` com novos números e gotchas.
