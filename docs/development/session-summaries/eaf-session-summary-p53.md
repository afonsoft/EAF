# EAF Session Summary P53 - Coverage Audit

## Data

13 de julho de 2026

## Branch

`feature/devin-20260713-priority53-coverage-audit`

## Baseline P52

| Metric | Value |
|--------|-------|
| Line coverage | 96.6% (13216 / 13670) |
| Branch coverage | 83.6% (2417 / 2888) |
| Method coverage | 99.3% (2142 / 2156) |
| Tests | 4416 total, 4415 passing, 1 skipped |
| Build warnings | 159 |

## Final P53

| Metric | Value |
|--------|-------|
| Line coverage | 97.1% (13274 / 13670) |
| Branch coverage | 84.2% (2433 / 2888) |
| Method coverage | 99.4% (2144 / 2156) |
| Tests | 4433 total, 4432 passing, 1 skipped |
| Build warnings | 163 |

## Changes

- Adicionados testes BDD para ramos acessíveis das classes de baixa cobertura do P53.
- `AzureActiveDirectoryAuthenticationSource.GetUserAsync` e `GetUsersAsync` tornados `virtual` para permitir mock do `AppAzureActiveDirectoryAuthenticationSource` com NSubstitute.
- Testes de integração do `MiddlewareCoreModule` ajustados para usar um `IocManager` isolado por teste, evitando conflito de componente `UnitOfWorkDefaultOptions`.
- Código de produção não foi alterado além do ajuste bloqueante documentado acima.
- Nenhum arquivo `.github/workflows/` foi modificado.

## New / updated tests

| Class | Assembly | Test file | Focus |
|-------|----------|-----------|-------|
| `UserManager` | Eaf.Middleware.Core | `test/Eaf.Middleware.Application.Tests/Authorization/Users/UserManagerBddTests.cs` | `UpdateWithValidateAsync` duplicado e renomeação do admin; `SetGrantedPermissionsAsync`; `SetRolesAsync` |
| `UserAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Authorization/Users/UserAppServiceBddTests.cs` | `CreateUsersByActiveDirectory` novo usuário; `CreateUsersByLdap` ignorando vazio e existente |
| `FriendshipAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Friendships/FriendshipAppServiceBddTests.cs` | `BlockUser` notificando clientes online; `CreateFriendshipRequestByUserName` tenant inexistente |
| `TokenAuthController` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs` | `Authenticate` e `ExternalAuthenticate` com `ModelState` inválido |
| `AzureActiveDirectoryAuthenticationSource` | Eaf.Middleware.AzureActiveDirectory | `test/Eaf.Middleware.AzureActiveDirectory.Tests/AzureActiveDirectory/Authentication/AzureActiveDirectoryAuthenticationSourceBddTests.cs` | `GetUsersAsync` lançando `AbpException` |
| `LdapAuthenticationSource` | Eaf.Middleware.Ldap | `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs` | `CreateUserAsync`, `UpdateUserAsync` e `GetUsersAsync` com entrada LDAP |
| `ServiceBusQueueAppender` | Eaf.Log4NetServiceBus | `test/Eaf.Log4NetServiceBus.Tests/Logging/ServiceBusQueueAppenderBddTests.cs` | ramos de erro/exception e fallback |
| `MiddlewareCoreModule` | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/Middleware/MiddlewareCoreModuleIntegrationTests.cs` | cache de amigos com expiração de 30 minutos |

## Notes

- `ServiceBusQueueAppender` manteve a cobertura existente; os ramos de `OnClose` e `CloseAsync` continuam inacessíveis no Linux sem conexão real com Azure Service Bus.
- `PermissionAppService` permaneceu em 92.5% porque o branch `permission.Children == null` no `Permission.GetAllPermissions` é inacessível (`Permission.Children` utiliza `ImmutableList` e nunca retorna `null` sem lançar exceção).
- `LdapAuthenticationSource` (66.3%), `MiddlewareWebCoreModule` (87.3%), `TokenAuthController` (89.9% - campos `OrderBy`/`Select` inlining limitam coverlet), `UserAppService` (90.5% - idem), `UserManager` (90.7% - idem), `FriendshipAppService` (90.9% - idem), `MiddlewareCoreModule` (93.9%), `AzureActiveDirectoryAuthenticationSource` (94.8%), `LdapSettings` (91.8%) e `ServiceBusQueueAppender` (92.8%) tiveram cobertura estável ou leve aumento; ramos Linux-inacessíveis continuam documentados.

## Verification

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

Build: 0 errors, 163 warnings. All tests pass with no coverage regression.
