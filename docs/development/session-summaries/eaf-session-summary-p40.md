# EAF Session Summary P40 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260712-priority40-coverage-audit`
- **Data:** 2026-07-12
- **PR:** #156 (https://github.com/afonsoft/EAF/pull/156)

## Baseline
| Métrica | Valor |
|---------|-------|
| Line | 90.8% |
| Branch | 72% |
| Method | 96.9% |
| Covered Lines | 12419 / 13672 |
| Covered Branches | 2112 / 2932 |
| Covered Methods | 2036 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 93.1% |
| Branch | 76.9% |
| Method | 98.1% |
| Covered Lines | 12740 / 13670 |
| Covered Branches | 2255 / 2930 |
| Covered Methods | 2062 / 2100 |

## Código de produção alterado
Nenhum. Apenas testes foram adicionados/ajustados nesta sessão.

## Testes adicionados/ajustados

- `test/Eaf.Middleware.Ldap.Tests/Ldap/Authentication/LdapAuthenticationSourceBddTests.cs` (novo)
  - 63 testes BDD cobrindo `TryAuthenticateAsync`, `CreateUserAsync`, `CreateUserAsync` com exceções, `UpdateUserAsync`, `GetUserById` e `Name`.

- `test/Eaf.Middleware.AzureActiveDirectory.Tests/AzureActiveDirectory/Authentication/AzureActiveDirectoryAuthenticationSourceBddTests.cs` (novo)
  - `Dado_ConfiguracaoValida_Quando_Criar_Entao_DeveTerNomeActiveDirectory`
  - `Dado_DominioIncorreto_Quando_TryAuthenticateAsync_Entao_DeveLancarMsalException`
  - `Dado_ErroMsal_Quando_TryAuthenticateAsync_Entao_DeveLancarMsalException`
  - `Dado_UsuarioDoGraphSemEmail_Quando_CreateUserAsync_Entao_DeveRetornarUsuario`
  - `Dado_ErroMsalNoCreateUserAsync_Entao_DeveLancarMsalException`
  - `Dado_ErroMsalNoUpdateUserAsync_Entao_DeveLancarMsalException`

- `test/Eaf.MiddlewareCore.Tests/Localization/CultureHelperBddTests.cs` (novo)
  - `Dado_CulturaValida_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCultura`
  - `Dado_CulturaInvalida_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCulturaAtual`
  - `Dado_CulturaNaoPresenteNaLista_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCulturaAtual`
  - `Dado_CulturaComCasingDiferente_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCultura`
  - `Dado_AllCultures_Quando_Acessar_Entao_DeveConterCulturas`
  - `Dado_IsRtl_Quando_Acessar_Entao_DeveRetornarBooleano`
  - `Dado_UsingLunarCalendar_Quando_Acessar_Entao_DeveRetornarBooleano`

- `test/Eaf.MiddlewareCore.Tests/Net/Web/WebContentDirectoryFinderBddTests.cs` (novo)
  - `Dado_ProjetoWebHostInexistente_Quando_CalculateContentRootFolder_Entao_DeveLancarExcecao`
  - `Dado_ProjetoWebHostExistente_Quando_CalculateContentRootFolder_Entao_DeveRetornarPasta`
  - `Dado_DiretorioSemArquivosEsperados_Quando_DirectoryContains_Entao_DeveRetornarFalse`

- `test/Eaf.Middleware.Web.Core.Tests/Configuration/EafHostBuilderExtensionsBddTests.cs` (novo)
  - `Dado_IHostBuilder_Quando_UseAbpConfigurationSemParametros_Entao_DeveConfigurarAppConfiguration`
  - `Dado_IHostBuilder_Quando_UseAbpConfigurationComPrefixo_Entao_DeveConfigurarAppConfiguration`
  - `Dado_IWebHostBuilder_Quando_UseAbpConfigurationSemParametros_Entao_DeveConfigurarAppConfiguration`
  - `Dado_IWebHostBuilder_Quando_UseAbpConfigurationComPrefixo_Entao_DeveConfigurarAppConfiguration`
  - `Dado_HostBuilderReal_Quando_UsarAbpConfigurationComPrefixo_Entao_DeveCriarHost`
  - `Dado_HostBuilderReal_Quando_UsarAbpConfigurationSemParametros_Entao_DeveCriarHost`
  - `Dado_WebHostBuilderReal_Quando_UsarAbpConfigurationComPrefixo_Entao_DeveCriarWebHost`
  - `Dado_WebHostBuilderReal_Quando_UsarAbpConfigurationSemParametros_Entao_DeveCriarWebHost`

- `test/Eaf.Middleware.Web.Core.Tests/Configuration/RedisConfigurerBddTests.cs` (novo)
  - `Dado_RedisDesabilitado_Quando_Configure_Entao_NaoDeveRegistrarRedisCache`
  - `Dado_RedisHabilitadoComConnectionString_Quando_Configure_Entao_DeveRegistrarRedisCache`
  - `Dado_RedisHabilitadoViaIsEnabled_Quando_Configure_Entao_DeveRegistrarRedisCache`
  - `Dado_RedisHabilitadoSemConnectionString_Quando_Configure_Entao_NaoDeveRegistrarRedisCache`

- `test/Eaf.Middleware.Application.Tests/Friendships/FriendshipManagerBddTests.cs` (novo)
  - `Dado_UsuariosValidos_Quando_CreateFriendshipAsync_Entao_DeveCriarAmizade`
  - `Dado_SolicitacaoPendente_Quando_AcceptFriendshipRequestAsync_Entao_DeveAceitar`
  - `Dado_UsuarioJaBloqueado_Quando_BanFriendAsync_Entao_DeveLancarUserFriendlyException`
  - `Dado_AmizadeExistente_Quando_UpdateFriendshipAsync_Entao_DeveAtualizar`
  - `Dado_AmizadeInexistente_Quando_GetFriendshipOrNullAsync_Entao_DeveRetornarNull`

## READMEs atualizados
- `README.md` e `README_pt.md` foram atualizados com as novas métricas de testes (Line 93.1%, Branch 76.9%, Method 98.1%, Total 4189, Passing 4188).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P39 (Line 90.8% -> 93.1%, Branch 72% -> 76.9%, Method 96.9% -> 98.1%).
- `LdapAuthenticationSource<T1, T2>` subiu de 2.8% para 50.1%.
- `AzureActiveDirectoryAuthenticationSource<T1, T2>` subiu de 8.7% para 90.6%.
- `WebContentDirectoryFinder` manteve 83.3% (caminho de sucesso e `DirectoryContains` cobertos).
- `EafHostBuilderExtensions` (Web) subiu de 83.3% para 96.2%.
- `RedisConfigurer` manteve 84.6%.
- `FriendshipManager` subiu de 85.9% para 89.4%.
- `CultureHelper` manteve 78.5% (métodos `IsRtl` e `UsingLunarCalendar` ainda com caminhos não cobertos).
- `MiddlewareWebCoreModule`, `ImpersonationManager`, `TokenAuthController`, `MiddlewareWorkerModule`, `ExpiredAuditLogDeleterWorker`, `EpPlusExcelExporterBase`, `ProfileControllerBase` e `UserAppService` continuam com cobertura inferior a 90% e são candidatos para P41.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração em código de produção.
- `LdapAuthenticationSource.SourceName` é `"LDAP"` (uppercase).
- `AzureActiveDirectoryAuthenticationSource.SourceName` é `"ActiveDirectory"`.
- `WebContentDirectoryFinder` é `public static class` no namespace `Eaf.Middleware.Web` (`src/Eaf.Middleware.Core/Net/Web/WebContentFolderHelper.cs`).
- `SimpleStringCipher` está no namespace `Abp.Runtime.Security`.

## Classes com cobertura ainda baixa (foco P41)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (80.6%)
- `Eaf.Middleware.Authorization.Impersonation.ImpersonationManager` (81.0%)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (81.4%)
- `Eaf.AspNetCore.SignalR.Chat.SignalRChatCommunicator` (81.5%)
- `Eaf.Middleware.Worker.MiddlewareWorkerModule` (82.1%)
- `Eaf.Middleware.AzureActiveDirectory.Configuration.AzureActiveDirectorySettings` (82.3%)
- `Eaf.KeyVault.OCIKeyVaultManager` (84.3%)
- `Eaf.Middleware.DataExporting.Excel.EpPlus.EpPlusExcelExporterBase` (85.1%)
- `Eaf.Middleware.Web.Controllers.ProfileControllerBase` (86.7%)
- `Eaf.Middleware.Authorization.Users.UserAppService` (86.9%)
- `Eaf.Middleware.Friendships.Cache.UserFriendsCache` (87.2%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%)
- `Eaf.Middleware.Web.Auditing.hangfire.ExpiredEntityLogDeleterWorker` (87.6%)
- `Eaf.Middleware.Web.Controllers.ChatControllerBase` (88.2%)
- `Eaf.Middleware.Web.Swagger.SwaggerEnumParameterFilter` (88.2%)
- `Eaf.Middleware.Web.Swagger.SwaggerOperationFilter` (88.2%)
- `Eaf.Middleware.Configuration.AppConfigurations` (88.4%)
- `Eaf.Middleware.Web.Controllers.FileController` (89.1%)
- `Eaf.Middleware.Serilog.SerilogEafHostBuilderExtensions` (89.2%)
- `Eaf.Middleware.MiddlewareAppServiceBase` (89.4%)
- `Eaf.Middleware.Web.Serilog.SerilogEafHostBuilderExtensions` (89.5%)
- `Eaf.Middleware.Chat.ChatFeatureChecker` (90.2%)
