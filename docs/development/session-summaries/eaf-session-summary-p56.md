# EAF Session Summary P56 - Coverage Audit

## Data

14 de julho de 2026

## Branch

`feature/devin-20260713-priority56-coverage-audit`

## Baseline P55

| Metric | Value |
|--------|-------|
| Line coverage | 97.5% (13263 / 13593) |
| Branch coverage | 85.6% (2454 / 2866) |
| Method coverage | 99.6% (2155 / 2162) |
| Tests | 4492 total, 4491 passing, 1 skipped |
| Build warnings | 129 |

## Final P56

| Metric | Value |
|--------|-------|
| Line coverage | 97.6% (13273 / 13589) |
| Branch coverage | 87.2% (2502 / 2868) |
| Method coverage | 99.6% (2155 / 2162) |
| Tests | 4516 total, 4515 passing, 1 skipped |
| Build warnings | 154 |

## Changes

- Adicionados testes BDD para ramos acessíveis das classes de baixa cobertura do P56.
- Cobertura de ramo (branch) subiu de 85.6% para 87.2%.
- `ChatAppService` e `EafHostBuilderExtensions` (Core/Worker) atingiram 100% de cobertura de linha.
- Código de produção não foi alterado (salvo ajustes em testes para remover warnings).
- Nenhum arquivo `.github/workflows/` foi modificado.

## New / updated tests

| Class | Assembly | Test file | Focus |
|-------|----------|-----------|-------|
| `HostSettingsAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Configuration/Host/HostSettingsAppServiceBddTests.cs` | `UpdateAllSettings` com sub-DTOs nulos, timezone, LogDeleter, LoginImpersonator, Google vazio, Azure AD/LDAP habilitados com valores em branco, external-login providers com JSON e claims mapping |
| `ChatAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Chat/ChatAppServiceBddTests.cs` | Lado das mensagens de grupo (`Side`), marcação de mensagens não lidas com `TargetTenantId` distintos |
| `AzureActiveDirectoryAuthenticationSource<T1, T2>` | Eaf.Middleware.AzureActiveDirectory | `test/Eaf.Middleware.AzureActiveDirectory.Tests/AzureActiveDirectory/Authentication/AzureActiveDirectoryAuthenticationSourceBddTests.cs` | `Mail`/`UserPrincipalName` sem `@` para `GetUserAsync`/`GetUsersAsync`/`UpdateUserAsync` |
| `EafHostBuilderExtensions` (Core) | Eaf.MiddlewareCore | `test/Eaf.MiddlewareCore.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs` | `UseEafConfiguration` com action nula + prefixo, e prefixo vazio |
| `EafHostBuilderExtensions` (Worker) | Eaf.Middleware.Worker | `test/Eaf.Middleware.Worker.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs` | `UseAbpConfiguration` com action nula + prefixo, e prefixo vazio |
| `EafServiceCollectionExtensions` (Worker) | Eaf.Middleware.Worker | `test/Eaf.Middleware.Worker.Tests/ServiceProviders/EafServiceCollectionExtensionsBddTests.cs` | `AddEaf<TStartupModule>()` sem `optionsAction` |

## Notes

- `LdapAuthenticationSource` (59.2%), `MiddlewareWebCoreModule` (87.3%), `TokenAuthController` (90.9%), `PermissionAppService` (92.5%), `ServiceBusQueueAppender` (92.8%), `AzureActiveDirectoryAuthenticationSource` (93.5%), `EafSqliteCache` (94.9%), `OpenIdConnectAuthProviderApi` (95.2%), `EafHangfireApplicationBuilderExtensions` (96.7%), `EafHangfireAuthorizationFilter` (97.7%), `MiddlewareAppServiceBase` (97.3%), `EafSqlServerCache` (97.4%), `ChatHub` (97.4%), `EafOpenTelemetryServiceCollectionExtensions` (97.2%), `EafServiceCollectionExtensions` (98.1%), `TenantAppService` (98.4%) e `HostSettingsAppService` (99.3%) mantêm ramos inacessíveis no Linux ou com builders/`sealed` não mockáveis com `NSubstitute`.
- `EafHostBuilderExtensions` (Core e Worker) subiram para 100% de cobertura de linha com os novos testes de prefixo/action nula.
- `ChatAppService` atingiu 100% de cobertura de linha com os novos testes de mensagens de grupo.
- Build Release: 0 erros, 154 warnings. Todos os testes passam sem regressão de cobertura.

## Verification

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

Build: 0 errors, 154 warnings. All tests pass with no coverage regression.
