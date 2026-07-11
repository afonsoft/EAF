# EAF Session Summary P36 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260711-priority36-coverage-audit`
- **Data:** 2026-07-11
- **PR:** #136 (https://github.com/afonsoft/EAF/pull/136)

## Baseline
| Métrica | Valor |
|---------|-------|
| Line | 86.1% |
| Branch | 65.4% |
| Method | 95.7% |
| Covered Lines | 11776 / 13672 |
| Covered Branches | 1919 / 2932 |
| Covered Methods | 2010 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 87.6% |
| Branch | 67.2% |
| Method | 96.2% |
| Covered Lines | 11981 / 13672 |
| Covered Branches | 1971 / 2932 |
| Covered Methods | 2022 / 2100 |

## Código de produção alterado
Nenhum. Apenas testes foram adicionados/ajustados nesta sessão.

## Testes adicionados/ajustados

- `test/Eaf.KeyVault.Tests/KeyVault/OCI/OCIKeyVaultManagerBddTests.cs`
  - `Dado_ClienteComAutenticacaoExplicita_Quando_GetKeyValues_Entao_DeveRetornarDicionarioVazioSemLancarExcecao`
  - `Dado_ClienteComAutenticacaoExplicita_Quando_GetValue_Entao_DeveLancarExcecaoQuandoServicoFalhar`

- `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs`
  - `Dado_UsuarioDeveAlterarSenha_Quando_Authenticate_Entao_DeveRetornarPasswordResetCode`
  - `Dado_CredenciaisInvalidas_Quando_Authenticate_Entao_DeveLancarUserFriendlyException`
  - `Dado_ExternalLoginValido_Quando_ExternalAuthenticate_Entao_DeveRetornarAccessToken`
  - `Dado_ImpersonationTokenValido_Quando_ImpersonatedAuthenticate_Entao_DeveRetornarAccessToken`
  - `Dado_SingleSignInHabilitado_Quando_Authenticate_Entao_DeveRetornarAccessTokenComReturnUrlModificado`
  - `Dado_LoginUnicoPorUsuario_Quando_Authenticate_Entao_DeveAtualizarSecurityStamp`
  - `Dado_UsuarioAutenticado_Quando_LogOut_Entao_DeveAtualizarSecurityStampELimparCache`
  - `Dado_CacheItemExistente_Quando_SendTwoFactorAuthCode_Entao_DeveEnviarCodigoPorEmail`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs`
  - `Dado_IocManagerConfigurado_Quando_PostInitialize_Entao_DeveConfigurarPastasEProvedoresExternos`
  - `Dado_HangfireHabilitado_Quando_PostInitialize_Entao_DeveConfigurarStorageELimparJobs`

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P35 (Line 86.1% -> 87.6%, Branch 65.4% -> 67.2%, Method 95.7% -> 96.2%).
- `TokenAuthController` subiu de 26.4% para 46.4%, mas ainda tem caminhos como `TwoFactorAuthenticate`, `SendEmailActivationLink`, `Register`, `IsTwoFactorAuthRequiredAsync` e `GetTenancyNameOrNull` que podem ser cobertos na P37.
- `MiddlewareWebCoreModule` manteve 69.6%: o branch `Hangfire` foi coberto, mas `SetAppFolders` ainda tem exceção não coberta e `PostInitialize` possui caminhos `SqlServer`/`Redis` de `HangfireStorageType` não exercitados.
- `EafOpenTelemetryServiceCollectionExtensions` subiu de 75.6% para 78.7%.
- `ServiceBusQueueAppender` manteve 51.4% e continua como foco para P37.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração em código de produção.

## Classes com cobertura ainda baixa (foco P37)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (46.4%)
- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (47.6%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (70.8%)
- `Eaf.KeyVault.AzureKeyVaultManager` (75.3%)
- `Eaf.WebHooks.EafWebHookReceiver` (75.7%)
- `Eaf.Middleware.Web.Startup.HangFireConfigurer` (77.5%)
- `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (78.7%)
- `Eaf.Middleware.MultiTenancy.TenantAppService` (79.6%)
