# EAF Session Summary P32 - Coverage Audit

## Data

2026-07-11

## Branch

`feature/devin-20260711-priority32-coverage-audit`

## PR

https://github.com/afonsoft/EAF/pull/130

## Baseline (P31)

- Line: 83.4%
- Branch: 62.6%
- Method: 94.3%
- Covered lines: 11395 / 13661
- Covered branches: 1837 / 2932
- Covered methods: 1982 / 2100

## Resultado (P32)

- Line: 83.6%
- Branch: 62.8%
- Method: 94.4%
- Covered lines: 11433 / 13661
- Covered branches: 1843 / 2932
- Covered methods: 1984 / 2100

## Alterações

- Adicionados testes BDD em português (padrão Dado/Quando/Então) para cobrir caminhos de baixa cobertura em:
  - `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter`
  - `Eaf.Middleware.Web.Controllers.AntiForgeryController`
  - `Eaf.Middleware.Web.Controllers.ProfileControllerBase`
  - `Eaf.Middleware.Web.Startup.HangFireConfigurer`
  - `Eaf.Middleware.Web.Startup.RedisConfigurer`
  - `Eaf.MiddlewareCore.Authorization.AuthorizationExtensions`
  - `Eaf.OpenTelemetry.EafOpenTelemetryServiceCollectionExtensions`
  - `Eaf.SqliteCache.EafSqliteCache`

## Validação

- `dotnet build Eaf.sln --configuration Release` passou.
- `bash run-tests-with-coverage.sh` passou com todos os testes verdes.
- Cobertura não regrediu em relação ao baseline P31.

## Aprendizados

- `ICache.GetOrDefault` requer chave explícita no formato `userId@tenantId` para `AuthorizationExtensions.GetExternalTokenInformation`.
- `SettingManager.GetSettingValueForTenantAsync`/`GetSettingValueForApplicationAsync` são virtuais e retornam `Task<string>`; usar `Task.FromResult(value)`.
- `TenantManager.CreateWithAdminUserAsync` não é virtual; não pode ser mockado com `NSubstitute.Returns`.
- `EafSqliteCache` reutiliza arquivo existente e recria quando schema é inválido.
- `EafOpenTelemetryServiceCollectionExtensions.AddEafOpenTelemetry` muta variáveis `OTEL_*`.
