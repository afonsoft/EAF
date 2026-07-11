# EAF Session Summary P35 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260711-priority35-coverage-audit`
- **Data:** 2026-07-11
- **PR:** (a ser criado)

## Baseline
| Métrica | Valor |
|---------|-------|
| Line | 84.5% |
| Branch | 64.1% |
| Method | 95.2% |
| Covered Lines | 11563 / 13672 |
| Covered Branches | 1880 / 2932 |
| Covered Methods | 2001 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 86.1% |
| Branch | 65.4% |
| Method | 95.7% |
| Covered Lines | 11776 / 13672 |
| Covered Branches | 1919 / 2932 |
| Covered Methods | 2010 / 2100 |

## Código de produção alterado
Nenhum. Apenas testes foram adicionados/ajustados nesta sessão.

## Testes adicionados/ajustados

- `test/Eaf.KeyVault.Tests/KeyVault/OCI/OCIKeyVaultManagerBddTests.cs`
  - `Dado_OptionsComAutenticacaoExplicita_Quando_Construir_Entao_DeveCriarCliente`
  - `Dado_OptionsSemConfiguracao_Quando_Construir_Entao_DeveLogarErroELancarExcecao`
  - `Dado_StringBase64_Quando_Base64Decode_Entao_DeveRetornarStringOriginal`
  - `Dado_StringNaoBase64_Quando_Base64Decode_Entao_DeveRetornarEntradaOriginal`

- `test/Eaf.Middleware.Application.Tests/Authorization/Users/Profile/ProfileAppServiceBddTests.cs`
  - `Dado_UsuarioSemFoto_Quando_GetProfilePictureById_Entao_DeveRetornarVazio`
  - `Dado_UsuarioInexistente_Quando_GetProfilePictureByUser_Entao_DeveRetornarVazio`
  - `Dado_UsuarioInexistente_Quando_GetFriendProfilePicture_Entao_DeveRetornarVazio`
  - `Dado_UsuarioLogadoSemSuporteATimezone_Quando_GetCurrentUserProfileForEdit_Entao_DeveRetornarTimezoneVazio`
  - `Dado_UsuarioLogadoComTimezone_Quando_GetCurrentUserProfileForEdit_Entao_DeveRetornarTimezone`
  - `Dado_UsuarioLogadoComTimezoneIgualPadrao_Quando_GetCurrentUserProfileForEdit_Entao_DeveRetornarTimezoneVazio`
  - `Dado_PerfilValidoComTimezone_Quando_UpdateCurrentUserProfile_Entao_DeveAtualizarTimezone`
  - `Dado_UsuarioLogadoSemTimezone_Quando_UpdateCurrentUserProfile_Entao_DeveAtualizarParaPadrao`

- `test/Eaf.Middleware.Web.Core.Tests/Authentication/JwtBearer/MiddlewareJwtSecurityTokenHandlerBddTests.cs`
  - Construtor `IDisposable` para isolar `IocManager.Instance` via reflection.
  - `Dado_TokenJaNoCache_Quando_ValidateToken_Entao_DeveRetornarPrincipalSemConsultarUsuario`
  - `Dado_TokenComSecurityStampDiferenteELoginUnico_Quando_ValidateToken_Entao_DeveLancarSecurityTokenException`
  - `Dado_TokenSemSecurityStampNoUsuario_Quando_ValidateToken_Entao_DeveLancarSecurityTokenException`

- `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs`
  - `Dado_CredenciaisValidas_Quando_Authenticate_Entao_DeveRetornarAccessToken`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs`
  - `Dado_IocManagerConfigurado_Quando_Initialize_Entao_DeveRegistrarConventions`
  - `Dado_HostEnvironmentComRedisHabilitado_Quando_Initialize_Entao_DeveRegistrarRedisAssembly`
  - `Dado_HostEnvironmentComRedisEnabledHabilitado_Quando_Initialize_Entao_DeveRegistrarRedisAssembly`

- `test/Eaf.Middleware.Web.Core.Tests/Serilog/SerilogEafHostBuilderExtensionsBddTests.cs`
  - `Dado_HostBuilderComConfigSeq_Quando_UsarEafSerilogSemNivelEBuild_Entao_DeveCriarHost`
  - `Dado_HostBuilderComConfigureLoggerCustomizado_Quando_UsarEafSerilog_Entao_DeveRetornarMesmoBuilder`
  - Outros testes para cobrir os overloads de `UseEafSerilog` com `LogEventLevel`.

- `test/Eaf.Middleware.Web.Core.Tests/Swagger/SwaggerOperationFilterBddTests.cs`
  - Reescrita com `ApiDescription` e `OperationFilterContext` reais.
  - `Dado_OperacaoComParametrosNulos_Quando_AplicarFiltro_Entao_DeveRetornarSemErro`
  - `Dado_OperacaoComParametroEnum_Quando_AplicarFiltro_Entao_DeveSubstituirSchema`
  - `Dado_OperacaoComParametroNaoEnum_Quando_AplicarFiltro_Entao_DeveManterSchema`

- `test/Eaf.Middleware.Worker.Tests/Serilog/SerilogEafHostBuilderExtensionsBddTests.cs`
  - `Dado_HostBuilderComConfigSeq_Quando_UsarEafSerilogSemNivelEBuild_Entao_DeveCriarHost`
  - `Dado_HostBuilderComConfigureLoggerCustomizado_Quando_UsarEafSerilog_Entao_DeveRetornarMesmoBuilder`
  - Outros testes para cobrir os overloads de `UseEafSerilog` com `LogEventLevel`.

- `test/Eaf.OpenTelemetry.Tests/EafOpenTelemetryServiceCollectionExtensionsBddTests.cs`
  - `Dado_ServiceCollection_Quando_BuildarEObterLoggerFactory_Entao_DeveCriarFactory`
  - Ajustes para cobrir configuração de `OtlpVariables` e `ConsoleExporter`.

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P34 (Line 84.5% -> 86.1%, Branch 64.1% -> 65.4%, Method 95.2% -> 95.7%).
- `TokenAuthController` subiu de 14.0% para 26.4%, mas ainda é o target de menor cobertura.
- `LogInManager`, `SecurityStampValidator` e `SignInManager` estão 100%.
- `MiddlewareWebCoreModule` e `ServiceBusQueueAppender` não aumentaram e continuam como foco para P36.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração em código de produção.

## Classes com cobertura ainda baixa (foco P36)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (26.4%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
- `Eaf.KeyVault.OCIKeyVaultManager` (35.3%)
- `Eaf.Log4NetServiceBus.Logging.ServiceBusQueueAppender` (51.4%)
- `Eaf.AspNetCore.Configuration.EafOpenTelemetryServiceCollectionExtensions` (75.6%)
