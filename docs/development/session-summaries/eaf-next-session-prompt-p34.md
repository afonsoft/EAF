# Próxima Sessão P34 - EAF Coverage Audit

## Contexto

Sessão anterior: `feature/devin-20260711-priority33-coverage-audit`
Baseline (P33):
- Line: 83.8%
- Branch: 63.3%
- Method: 94.6%
- Covered lines: 11450 / 13661
- Covered branches: 1857 / 2932
- Covered methods: 1987 / 2100

## Objetivo

Continuar o coverage audit adicionando testes BDD em português (xUnit + Shouldly + NSubstitute) para as classes de baixa cobertura restantes, sem regredir o baseline P33.

## Targets prioritários

1. `Eaf.Middleware.Web.Controllers.TokenAuthController` (11.9%)
2. `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (25.3%)
3. `Eaf.KeyVault.OCIKeyVaultManager` (34.9%)
4. `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
5. `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (60.7%)
6. `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (60.4%)
7. `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (65.6%)
8. `Eaf.Middleware.Configuration.EafHostBuilderExtensions` (66.6%)
9. `Abp.Dependency.EafCastleWindsorHostBuilderExtensions` (66.6%)
10. `Eaf.WebHooks.EafWebHookReceiver` (66.6%)
11. `Abp.Runtime.Caching.Sqlite.DbCommandPool` (69.7%)
12. `Eaf.Middleware.Configuration.UiCustomizationSettingsAppService` (69.3%)
13. `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
14. `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Blog` (53.5%)
15. `Eaf.MiddlewareCore.SampleApp.Core.EntityHistory.Post` (56.2%)

## Restrições

- Não modificar código de produção, exceto para corrigir bugs bloqueantes.
- Não modificar `.github/workflows/`.
- Não fazer push para `main`, `master` ou `develop`.
- Criar branch `feature/devin-YYYYMMDD-priority34-coverage-audit`.
- Testes em padrão BDD em português: `Dado_..._Quando_..._Entao_...`.
- Código em inglês.
- Manter cobertura >= baseline P33 (Line 83.8%, Branch 63.3%, Method 94.6%).

## Comandos de validação

```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Entregáveis

1. Branch `feature/devin-YYYYMMDD-priority34-coverage-audit`.
2. PR para `main`.
3. `docs/development/session-summaries/eaf-session-summary-p34.md`.
4. `docs/development/session-summaries/eaf-next-session-prompt-p35.md` (se aplicável).
5. Atualização de `.agents/MEMORY.md` com novos números e gotchas.
