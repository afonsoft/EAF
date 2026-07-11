# Próxima Sessão P37 - EAF Coverage Audit

## Contexto

Sessão anterior: `feature/devin-20260711-priority36-coverage-audit`
Baseline (P36):
- Line: 87.6%
- Branch: 67.2%
- Method: 96.2%
- Covered lines: 11981 / 13672
- Covered branches: 1971 / 2932
- Covered methods: 2022 / 2100

## Objetivo

Continuar o coverage audit adicionando testes BDD em português (xUnit + Shouldly + NSubstitute) para as classes de baixa cobertura restantes, sem regredir o baseline P36.

## Targets prioritários

1. `Eaf.Middleware.Web.Controllers.TokenAuthController` (46.4%)
2. `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (47.6%)
3. `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
4. `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
5. `Eaf.Middleware.Web.WebContentDirectoryFinder` (70.8%)
6. `Eaf.KeyVault.AzureKeyVaultManager` (75.3%)
7. `Eaf.WebHooks.EafWebHookReceiver` (75.7%)
8. `Eaf.Middleware.Web.Startup.HangFireConfigurer` (77.5%)
9. `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (78.7%)
10. `Eaf.Middleware.MultiTenancy.TenantAppService` (79.6%)

## Restrições

- Não modificar código de produção, exceto para corrigir bugs bloqueantes.
- Não modificar `.github/workflows/`.
- Não fazer push para `main`, `master` ou `develop`.
- Criar branch `feature/devin-YYYYMMDD-priority37-coverage-audit`.
- Testes em padrão BDD em português: `Dado_..._Quando_..._Entao_...`.
- Código em inglês.
- Manter cobertura >= baseline P36 (Line 87.6%, Branch 67.2%, Method 96.2%).

## Comandos de validação

```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Entregáveis

1. Branch `feature/devin-YYYYMMDD-priority37-coverage-audit`.
2. PR para `main`.
3. `docs/development/session-summaries/eaf-session-summary-p37.md`.
4. `docs/development/session-summaries/eaf-next-session-prompt-p38.md` (se aplicável).
5. Atualização de `.agents/MEMORY.md` com novos números e gotchas.
