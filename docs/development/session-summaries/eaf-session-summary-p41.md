# EAF Session Summary P41 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260712-priority41-coverage-audit`
- **Data:** 2026-07-12
- **PR:** (em aberto)

## Baseline (após merge da main)
| Métrica | Valor |
|---------|-------|
| Line | 93.2% |
| Branch | 77.2% |
| Method | 98.1% |
| Covered Lines | 12740 / 13670 |
| Covered Branches | 2262 / 2930 |
| Covered Methods | 2062 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 95% |
| Branch | 80.1% |
| Method | 98.5% |
| Covered Lines | 12996 / 13670 |
| Covered Branches | 2348 / 2930 |
| Covered Methods | 2069 / 2100 |

## Código de produção alterado
Nenhum. Apenas testes foram adicionados/ajustados nesta sessão.

## Testes adicionados/ajustados

- `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerP41BddTests.cs` (novo)
  - `Dado_CaptchaAtivo_Quando_Authenticate_Entao_DeveLancarUserFriendlyException`  
  - `Dado_AuthZeroComProviderValido_Quando_ExternalAuthenticate_Entao_DeveRetornarToken`
  - `Dado_AuthZeroComProviderInvalido_Quando_ExternalAuthenticate_Entao_DeveLancarUserFriendlyException`
  - `Dado_AuthZeroComResultadoInvalido_Quando_ExternalAuthenticate_Entao_DeveLancarUserFriendlyException`
  - `Dado_AuthZeroDesabilitado_Quando_IsSchemeEnabled_Entao_DeveRetornarFalse`
  - `Dado_AuthZeroHabilitadoSemTenant_Quando_IsSchemeEnabled_Entao_DeveRetornarFalseParaTenant`
  - `Dado_DoisFatoresDesabilitado_Quando_IsTwoFactorAuthRequiredAsync_Entao_DeveRetornarFalse`
  - `Dado_UsuarioSemDoisFatores_Quando_IsTwoFactorAuthRequiredAsync_Entao_DeveRetornarFalse`
  - `Dado_UsuarioComDoisFatoresSemProvedor_Quando_IsTwoFactorAuthRequiredAsync_Entao_DeveRetornarFalse`
  - `Dado_UsuarioAutenticado_Quando_LogOutComExcessaoEmTresBlocos_Entao_DeveCapturarSemLancar`
  - `Dado_ErroAoSalvarClaims_Quando_CreateJwtClaims_Entao_DeveRetornarClaimsAlternativos`
  - `Dado_ErroNoUpdateExternalUserAsync_Quando_AtualizarExterno_Entao_DeveRetornarNull`
  - `Dado_ErroNaPictureUpdateExternalUserAsync_Quando_AtualizarExterno_Entao_DeveRetornarNull`
  - `Dado_UsuarioExternoExistente_Quando_RegisterExternalUserAsync_Entao_DeveAtualizarUsuario`

- `test/Eaf.Middleware.Web.Core.Tests/MiddlewareWebCoreModuleBddTests.cs` (ajustado)
  - `Dado_HangfireRedisComDatabaseId_Quando_PostInitialize_Entao_DeveConfigurarRedisStorageComDatabaseId`
  - `Dado_HangfireSqlServerComConnectionString_Quando_PostInitialize_Entao_DeveConfigurarSqlServerStorage`
  - `Dado_HostEnvironmentComContentRootInvalido_Quando_PostInitialize_Entao_DeveCapturarExcecoesSemLancar`
  - `Dado_HostEnvironmentComContentRootInvalido_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente`

- `test/Eaf.MiddlewareCore.Tests/Auditing/hangfire/ExpiredAuditLogDeleterWorkerBddTests.cs` (ajustado)
  - `Dado_AuditLogsExpiradosExcedendoLimite_Quando_DeleteLancarExcecao_Entao_DeveCapturarEContinuar`
  - `Dado_AuditLogsExpiradosDentroLimite_Quando_DeleteLancarExcecao_Entao_DeveCapturarEContinuar`

- `test/Eaf.Middleware.Application.Tests/Authorization/Users/UserAppServiceBddTests.cs` (ajustado)
  - `Dado_ExcecoesNaNotificacaoEWebhook_Quando_NotificationNewUser_Entao_DeveCapturarSemLancar`

## READMEs atualizados
- `README.md` e `README_pt.md` atualizados com as novas métricas de testes (Line 95%, Branch 80.1%, Method 98.5%, Total 4266, Passing 4265).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P41 (Line 93.2% -> 95%, Branch 77.2% -> 80.1%, Method 98.1% -> 98.5%).
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` subiu de 80.6% para 84.8%.
- `Eaf.Middleware.Web.Controllers.TokenAuthController` subiu de 81.4% para 90.1%.
- `Eaf.Middleware.Authorization.Users.UserAppService` subiu de 86.9% para 91.6%.
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredAuditLogDeleterWorker` subiu de 87.6% para 100%.
- `Eaf.AspNetCore.SignalR.Chat.SignalRChatCommunicator`, `Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettings`, `Eaf.KeyVault.OCIKeyVaultManager`, `Eaf.Middleware.DataExporting.Excel.EpPlus.EpPlusExcelExporterBase`, `Eaf.Middleware.Web.Controllers.ProfileControllerBase`, `Eaf.Middleware.Friendships.Cache.UserFriendsCache`, `Eaf.Middleware.Web.Controllers.ChatControllerBase`, `Eaf.Middleware.Web.Swagger.SwaggerEnumParameterFilter`, `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter`, `Eaf.Middleware.Configuration.AppConfigurations`, `Eaf.Middleware.Web.Controllers.FileController`, `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions`, `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions`, `Eaf.Middleware.Chat.ChatFeatureChecker` e `Eaf.Middleware.Localization.CultureHelper` atingiram 100%.
- `Eaf.Middleware.Authorization.Impersonation.ImpersonationManager` já estava 100% no baseline P41.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração em código de produção.
- `UserManager.UpdateWithValidateAsync` não é virtual e não pode ser stubado com `NSubstitute`; a solução foi usar `UserManager` real com `UserStore` substituído.
- `ILookupNormalizer` no .NET 10 expõe `NormalizeName` e `NormalizeEmail`, não `Normalize`.
- `AbpUserStore.FindByNameOrEmailAsync` tem overloads `(string)` e `(int?, string)`.
- `User.Identity.GetUserIdentifierOrNull()` usa claims `AbpClaimTypes.UserId` e `AbpClaimTypes.TenantId`.

## Classes com cobertura ainda baixa (foco P42)
- `Eaf.Middleware.Ldap` (58.7%)
- `Eaf.Log4NetServiceBus` (80.3%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (84.8%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%)
- `Eaf.Middleware.Web.Startup.RedisConfigurer` (84.6%)
- `Eaf.Middleware.Worker.MiddlewareWorkerModule` (89%)
- `Eaf.Hangfire.HangfireBackgroundJobManager` (85%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (83.3%)
- `Eaf.Middleware.Friendships.FriendshipManager` (89.4%)
