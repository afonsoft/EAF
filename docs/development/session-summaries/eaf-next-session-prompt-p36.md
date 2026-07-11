# Próxima Sessão P36 - EAF Coverage Audit

## Contexto

Sessão anterior: `feature/devin-20260711-priority35-coverage-audit`
Baseline (P35):
- Line: 86.1%
- Branch: 65.4%
- Method: 95.7%
- Covered lines: 11776 / 13672
- Covered branches: 1919 / 2932
- Covered methods: 2010 / 2100

## Objetivo

Continuar o coverage audit adicionando testes BDD em português (xUnit + Shouldly + NSubstitute) para as classes de baixa cobertura restantes, sem regredir o baseline P35.

## Targets prioritários

1. `Eaf.Middleware.Web.Controllers.TokenAuthController` (26.4%)
2. `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
3. `Eaf.KeyVault.OCIKeyVaultManager` (49.3%)
4. `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
5. `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (75.6%)
6. `Eaf.Middleware.Web.Authentication.JwtBearer.MiddlewareJwtSecurityTokenHandler` (94.3%)
7. `Eaf.Middleware.Application.Authorization.Users.Profile.ProfileAppService` (93.2%)
8. `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter` (88.2%)

## Restrições

- Não modificar código de produção, exceto para corrigir bugs bloqueantes.
- Não modificar `.github/workflows/`.
- Não fazer push para `main`, `master` ou `develop`.
- Criar branch `feature/devin-YYYYMMDD-priority36-coverage-audit`.
- Testes em padrão BDD em português: `Dado_..._Quando_..._Entao_...`.
- Código em inglês.
- Manter cobertura >= baseline P35 (Line 86.1%, Branch 65.4%, Method 95.7%).

## Comandos de validação

```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Entregáveis

1. Branch `feature/devin-YYYYMMDD-priority36-coverage-audit`.
2. PR para `main`.
3. `docs/development/session-summaries/eaf-session-summary-p36.md`.
4. `docs/development/session-summaries/eaf-next-session-prompt-p37.md` (se aplicável).
5. Atualização de `.agents/MEMORY.md` com novos números e gotchas.
