# EAF Session Summary P52 - Coverage Audit

## Data

13 de julho de 2026

## Branch

`feature/devin-20260713-priority52-coverage-audit`

## Baseline P51

| Metric | Value |
|--------|-------|
| Line coverage | 96.4% (13184 / 13670) |
| Branch coverage | 83.0% (2399 / 2888) |
| Method coverage | 99.3% (2141 / 2156) |
| Tests | 4401 total, 4400 passing, 1 skipped |
| Build warnings | 159 |

## Final P52

| Metric | Value |
|--------|-------|
| Line coverage | 96.6% (13216 / 13670) |
| Branch coverage | 83.6% (2417 / 2888) |
| Method coverage | 99.3% (2142 / 2156) |
| Tests | 4416 total, 4415 passing, 1 skipped |
| Build warnings | 159 |

## Changes

- Adicionados testes BDD para ramos acessíveis das classes de baixa cobertura do P52.
- Código de produção não foi alterado.
- Nenhum arquivo `.github/workflows/` foi modificado.

## New / updated tests

| Class | Assembly | Test file | Focus |
|-------|----------|-----------|-------|
| `MiddlewareControllerBase` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Controllers/MiddlewareControllerBaseBddTests.cs` | `L(string, CultureInfo)` helper |
| `EafKeyVaultConfigurationProvider` | Eaf.Hosting | `test/Eaf.KeyVault.Tests/EafKeyVaultConfigurationProviderTests.cs` | unknown provider fallback to `NullKeyVaultManager` |
| `DefaultLanguagesCreator` | Eaf.MiddlewareCore.SampleApp | `test/Eaf.MiddlewareCore.Tests/SampleApp/Seed/SampleAppSeedBddTests.cs` | duplicate/no-duplicate language seed branches |
| `TenantRoleAndUserBuilder` | Eaf.MiddlewareCore.SampleApp | `test/Eaf.MiddlewareCore.Tests/SampleApp/Seed/SampleAppSeedBddTests.cs` | tenant 1 and tenant 2 branches, duplicate calls |
| `ProfileAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Authorization/Users/Profile/ProfileAppServiceBddTests.cs` | `GetProfilePicture` with picture, `GetProfilePictureByUser`/`GetFriendProfilePicture` with null picture, `UpdateProfilePicture` >5MB error |
| `ChatAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Chat/ChatAppServiceBddTests.cs` | `GetUserChatMessages` sets `TargetUserName`, `MarkAllUnreadMessagesOfUserAsRead` no-messages branch |
| `ChatMessageManager` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Chat/ChatMessageManagerBddTests.cs` | receiver not found, online client branch, `HandleSenderUserInfoChangeAsync` no-op branches |

## Notes

- `ProfileAppService`, `MiddlewareControllerBase`, `EafKeyVaultConfigurationProvider`, `DefaultLanguagesCreator` e `TenantRoleAndUserBuilder` atingiram 100% de cobertura de linha.
- `ChatAppService` subiu de 91.1% para 92.5%; `ChatMessageManager` subiu de 92.4% para 95.5%.
- `PermissionAppService` permaneceu em 92.5% porque o branch `permission.Children == null` no `Permission.GetAllPermissions` é inacessível (`Permission.Children` utiliza `ImmutableList` e nunca retorna `null` sem lançar exceção).
- `LdapAuthenticationSource` (59.2%), `TokenAuthController` (89.9%), `UserAppService` (90.5%), `UserManager` (90.7%), `FriendshipAppService` (90.9%), `MiddlewareWebCoreModule` (87.3%), `MiddlewareCoreModule` (93.9%), `AzureActiveDirectoryAuthenticationSource` (91.2%), `LdapSettings` (91.8%) e `ServiceBusQueueAppender` (92.8%) mantiveram cobertura estável; ramos Linux-inacessíveis continuam documentados.

## Verification

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

Build: 0 errors, 159 warnings. All tests pass with no coverage regression.
