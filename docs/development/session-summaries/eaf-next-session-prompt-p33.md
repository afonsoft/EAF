# Próxima Sessão P33 - EAF Coverage Audit

## Contexto

Sessão anterior: `feature/devin-20260711-priority32-coverage-audit`
PR: https://github.com/afonsoft/EAF/pull/130
Baseline (P32):
- Line: 83.6%
- Branch: 62.8%
- Method: 94.4%
- Covered lines: 11433 / 13661
- Covered branches: 1843 / 2932
- Covered methods: 1984 / 2100

## Objetivo

Continuar o coverage audit adicionando testes BDD em português (xUnit + Shouldly + NSubstitute) para as classes de baixa cobertura restantes, sem regredir o baseline P32.

## Targets prioritários

1. `Eaf.Middleware.Web.Controllers.TokenAuthController` (0%)
2. `Eaf.Middleware.Web.MiddlewareWebCoreModule` (45.5%)
3. `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)
4. `Eaf.Middleware.Identity.LogInManager`, `SecurityStampValidator`, `SignInManager` (0%)
5. `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (17.4%)
6. `Eaf.KeyVault.OCIKeyVaultManager` (19.2%)
7. `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
8. `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (60.4%)
9. `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (60.7%)
10. `Eaf.Middleware.Web.Startup.EafServiceCollectionMiddlewareExtensions` (53.4%)
11. `Eaf.Middleware.Web.Startup.AuthConfigurer` (69.3%)
12. `Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker` (85%)
13. `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%)
14. `Eaf.OpenTelemetry` (78.5%) — `EafOpenTelemetryServiceCollectionExtensions` (68.1%)
15. `Eaf.Middleware.Application.Authorization.Users.Profile.ProfileAppService` (81.3%)
16. `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter` (88.2%)

## Restrições

- Não modificar código de produção, exceto para corrigir bugs bloqueantes.
- Não modificar `.github/workflows/`.
- Não fazer push para `main`, `master` ou `develop`.
- Criar branch `feature/devin-YYYYMMDD-priority33-coverage-audit`.
- Testes em padrão BDD em português: `Dado_..._Quando_..._Entao_...`.
- Código em inglês.
- Manter cobertura >= baseline P32 (Line 83.6%, Branch 62.8%, Method 94.4%).

## Comandos de validação

```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Entregáveis

1. Branch `feature/devin-YYYYMMDD-priority33-coverage-audit`.
2. PR para `main`.
3. `docs/development/session-summaries/eaf-session-summary-p33.md`.
4. `docs/development/session-summaries/eaf-next-session-prompt-p34.md` (se aplicável).
5. Atualização de `.agents/MEMORY.md` com novos números e gotchas.
