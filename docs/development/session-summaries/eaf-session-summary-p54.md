# EAF Session Summary P54 - Coverage Audit

## Data

14 de julho de 2026

## Branch

`feature/devin-20260713-priority54-coverage-audit`

## Baseline P53

| Metric | Value |
|--------|-------|
| Line coverage | 97.1% (13274 / 13670) |
| Branch coverage | 84.2% (2433 / 2888) |
| Method coverage | 99.4% (2144 / 2156) |
| Tests | 4433 total, 4432 passing, 1 skipped |
| Build warnings | 163 |

## Final P54

| Metric | Value |
|--------|-------|
| Line coverage | 97.2% (13235 / 13604) |
| Branch coverage | 85.1% (2440 / 2866) |
| Method coverage | 99.5% (2150 / 2159) |
| Tests | 4467 total, 4466 passing, 1 skipped |
| Build warnings | 127 |

## Changes

- Adicionados testes BDD para ramos acessíveis das classes de baixa cobertura do P54.
- `ChatMessageManager` ganhou override de `L(string name, CultureInfo culture, params object[] args)` para usar o `MiddlewareLocalizationHelper` e o fallback de sources; sem isso o overload com `args` e cultura usava o comportamento base do `ApplicationService` e retornava string vazia.
- `EmailRealTimeNotifierBddTests` corrigido para usar `new LocalizableString(...)` no `LocalizableMessageNotificationData`.
- `ServiceBusQueueAppenderBddTests` teve o teste inválido de `OnClose` removido porque `ServiceBusConnection.IsClosedOrClosing` e `CloseAsync` são não-virtuais; não é possível forçar `CloseAsync` a lançar com `NSubstitute`.
- Código de produção não foi alterado além do ajuste bloqueante documentado acima.
- Nenhum arquivo `.github/workflows/` foi modificado.

## New / updated tests

| Class | Assembly | Test file | Focus |
|-------|----------|-----------|-------|
| `ChatAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Chat/ChatAppServiceBddTests.cs` | `MarkUserMessagesAsReadAsync` sem `reverseMessages`, usuário offline/amigo online; `MarkGroupMessagesAsReadAsync` sem mensagens; `GetUserChatFriendsWithSettingsAsync` tenant nulo e amigos nulos |
| `ChatMessageManager` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Chat/ChatMessageManagerBddTests.cs` | `SendMessageAsync` com `receiver.TenantId` nulo; `HandleSenderUserInfoChangeAsync` com `friendship` nula; overloads `L` com `args` e `CultureInfo` |
| `MiddlewareLocalizationHelper` | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/Localization/MiddlewareLocalizationHelperBddTests.cs` | `args` vazio, `source` que lança, fallback de sources |
| `EmailRealTimeNotifier` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Notifications/EmailRealTimeNotifierBddTests.cs` | `LocalizableMessageNotificationData` e `UseOnlyIfRequestedAsTarget` |
| `WebLogAppService` | Eaf.Middleware.Application | `test/Eaf.Middleware.Application.Tests/Logging/WebLogAppServiceBddTests.cs` | Limite de 100 linhas em `GetLatestWebLogs` |
| `OpenIdConnectAuthProviderApi` | Eaf.Middleware.Core | `test/Eaf.MiddlewareCore.Tests/Authorization/External/Providers/OpenIdConnectAuthProviderApiBddTests.cs` | `Surname` vazio quando `name` tem uma palavra |
| `AzureActiveDirectoryAuthenticationSource` | Eaf.Middleware.AzureActiveDirectory | `test/Eaf.Middleware.AzureActiveDirectory.Tests/AzureActiveDirectory/Authentication/AzureActiveDirectoryAuthenticationSourceBddTests.cs` | `mail`/`UserPrincipalName` sem `@` |
| `ServiceBusQueueAppender` | Eaf.Log4NetServiceBus | `test/Eaf.Log4NetServiceBus.Tests/Logging/ServiceBusQueueAppenderBddTests.cs` | Teste inválido de `OnClose` removido; `AppendBuffer`/`SendBuffer` existentes mantidos |
| `TokenAuthController` | Eaf.Middleware.Web.Core | `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs` | `GetDefaultEnabledProvider` para cada provedor; `GetExternalAuthenticationProviders` com tenant; `SecurityStamp` vazio; two-factor; `ExternalAuthenticate` provider-key inválido/senha inválida; `TeamsAuthenticate` desabilitado/não configurado; `SendTwoFactorAuthCode` modelo inválido/provedor não e-mail; `GetAuthenticationProviders` com `AuthenticationSource` nulo |

## Notes

- `ServiceBusQueueAppender` `OnClose` catch branch continua inalcançável no Linux porque `ServiceBusConnection` não torna `IsClosedOrClosing`/`CloseAsync` virtualizáveis; `NSubstitute` não consegue forçar a exceção.
- `PermissionAppService` permaneceu em 92.5% porque o branch `permission.Children == null` é inacessível (`Permission.Children` usa `ImmutableList` e nunca retorna `null`).
- `LdapAuthenticationSource` e `MiddlewareWebCoreModule` mantêm ramos inacessíveis no Linux (conexão LDAP real, infra Hangfire/Redis/SQL Server e `??` fallback normalizado).
- `TokenAuthController` subiu acima de 90% com os novos testes, embora `GetExternalUserInfo` e `RegisterExternalUserAsync` ainda tenham ramos complexos (`IocManager`/real `DefaultExternalLoginInfoManager`).

## Verification

```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

Build: 0 errors, 127 warnings. All tests pass with no coverage regression.
