# Próxima Sessão P35 - EAF Coverage Audit

## Contexto

Sessão anterior: `feature/devin-20260711-priority34-coverage-audit`
Baseline (P34):
- Line: 84.5%
- Branch: 64.1%
- Method: 95.2%
- Covered lines: 11563 / 13672
- Covered branches: 1880 / 2932
- Covered methods: 2001 / 2100

## Objetivo

Continuar o coverage audit adicionando testes BDD em português (xUnit + Shouldly + NSubstitute) para as classes de baixa cobertura restantes, sem regredir o baseline P34.

## Targets prioritários

1. `Eaf.Middleware.Web.Controllers.TokenAuthController` (14.0%)
2. `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
3. `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (12.6%)
4. `Eaf.Middleware.Identity.LogInManager`, `SecurityStampValidator`, `SignInManager` (0%)
5. `Eaf.KeyVault.OCIKeyVaultManager` (34.9%)
6. `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
7. `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (60.7%)
8. `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (60.4%)
9. `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (65.6%)
10. `Eaf.Middleware.Application.Authorization.Users.Profile.ProfileAppService` (81.3%)
11. `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter` (88.2%)

## Restrições

- Não modificar código de produção, exceto para corrigir bugs bloqueantes.
- Não modificar `.github/workflows/`.
- Não fazer push para `main`, `master` ou `develop`.
- Criar branch `feature/devin-YYYYMMDD-priority35-coverage-audit`.
- Testes em padrão BDD em português: `Dado_..._Quando_..._Entao_...`.
- Código em inglês.
- Manter cobertura >= baseline P34 (Line 84.5%, Branch 64.1%, Method 95.2%).

## Comandos de validação

```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Entregáveis

1. Branch `feature/devin-YYYYMMDD-priority35-coverage-audit`.
2. PR para `main`.
3. `docs/development/session-summaries/eaf-session-summary-p35.md`.
4. `docs/development/session-summaries/eaf-next-session-prompt-p36.md` (se aplicável).
5. Atualização de `.agents/MEMORY.md` com novos números e gotchas.
