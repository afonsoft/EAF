# EAF Session Summary P33 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260711-priority33-coverage-audit`
- **Data:** 2026-07-11
- **PR:** (a ser criado)

## Baseline
| Métrica | Valor |
|---------|-------|
| Line | 83.6% |
| Branch | 62.8% |
| Method | 94.4% |
| Covered Lines | 11429 / 13661 |
| Covered Branches | 1844 / 2932 |
| Covered Methods | 1984 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 83.8% |
| Branch | 63.3% |
| Method | 94.6% |
| Covered Lines | 11450 / 13661 |
| Covered Branches | 1857 / 2932 |
| Covered Methods | 1987 / 2100 |

## Classes adicionadas/alteradas

### Testes adicionados
- `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs`
  - `GetAuthenticationProviders` com usuário não encontrado.
  - `GetDefaultEnabledProvider` com LDAP e Microsoft habilitados.
- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs`
  - Verificação de herança e constructor com `IHostEnvironment`.
  - `Initialize` com `IocManager` e `Configuration` via reflection.
- `test/Eaf.Middleware.Web.Core.Tests/Serilog/SerilogEafHostBuilderExtensionsBddTests.cs`
  - Níveis `Warning` e `Debug` para cobrir branches de override.
- `test/Eaf.Middleware.Worker.Tests/Serilog/SerilogEafHostBuilderExtensionsBddTests.cs`
  - Níveis `Warning` e `Debug` para cobrir branches de override.
- `test/Eaf.Middleware.Application.Tests/Configuration/UiCustomizationSettingsAppServiceBddTests.cs`
  - `UseSystemDefaultSettings` com e sem tenant.
- `test/Eaf.Middleware.Ldap.Tests/LdapSettingsTests.cs`
  - Cobertura para `GetContextType`, `GetIsEnabled`, `GetUserName`, `GetPassword`, `GetContainer`, `GetDomain` com/sem tenant.
- `test/Eaf.MiddlewareCore.Tests/Authorization/External/Providers/OpenIdConnectAuthProviderApiBddTests.cs`
  - Testes de token inválido e JWT mal-formado.
- `test/Eaf.OpenTelemetry.Tests/EafOpenTelemetryServiceCollectionExtensionsBddTests.cs`
  - `OtlpVariables` vazias, `ConsoleExporter` true e defaults.
- `test/Eaf.KeyVault.Tests/KeyVault/OCI/OCIKeyVaultManagerBddTests.cs`
  - `Base64Decode` com string Base64 e não-Base64.
- `test/Eaf.SqliteCache.Tests/EafSqliteCacheTests.cs`
  - `Dispose` idempotente.

### Projetos alterados
- `src/Eaf.SqliteCache/Eaf.SqliteCache.csproj`
  - Adicionado `InternalsVisibleTo` para `Eaf.SqliteCache.Tests` (usado para tentativa de teste de `DbCommandPool`; a alteração permanece para uso futuro).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P32.
- Nenhum código de produção foi modificado (exceto `InternalsVisibleTo` no `csproj` do SqliteCache).
- Não houve alteração em `.github/workflows/`.
- O teste `EafSqliteCacheTests.Constructor_WithExistingFileButInvalidSchema_ShouldRecreateDatabase` continua falhando isoladamente (preexisting), mas o script de cobertura completa passou.
