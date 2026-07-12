# P41 Coverage Audit — Prompt para Próxima Sessão

Execute o P41 coverage audit para o repositório `afonsoft/EAF` e atualize o README com as novas métricas.

## Contexto
- Repositório: `afonsoft/EAF` (clone local `/home/ubuntu/repos/EAF`)
- Branch atual: `feature/devin-20260712-priority40-coverage-audit` (ou a branch do P41 a partir da `main` atual)
- Baseline P40: Line 93.2%, Branch 77%, Method 98.1% (12753 / 13672 linhas, 2260 / 2932 branches, 2062 / 2100 métodos)
- Testes: 4189 total, 4188 passando, 1 ignorado, 0 falhas
- Stack: xUnit + Shouldly + NSubstitute, BDD em português (`Dado/Quando/Então`)
- Build: `dotnet build Eaf.sln --configuration Release`
- Cobertura: `bash run-tests-with-coverage.sh` (requer `PATH=/home/ubuntu/.dotnet:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet`)
- Métricas: `TestResults/CoverageReport/Summary.txt`

## Objetivos
1. Adicionar testes BDD em português para as classes de baixa cobertura restantes, priorizando as com maior impacto e menor percentual atual:
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
   - `Eaf.Middleware.Localization.CultureHelper` (78.5%)
2. Manter ou aumentar a cobertura: Line >= 93.2%, Branch >= 77%, Method >= 98.1%.
3. Não modificar código de produção, salvo bugs bloqueantes documentados.
4. Não modificar `.github/workflows/`.

## Entregáveis
- Novos/ajustados arquivos de teste BDD em `test/`.
- `docs/development/session-summaries/eaf-session-summary-p41.md`.
- `docs/development/session-summaries/eaf-next-session-prompt-p42.md`.
- `README.md` e `README_pt.md` atualizados com as novas métricas.
- `.agents/MEMORY.md` atualizado com novos gotchas de P41.
- PR para `main` com CI verificado.

## Notas técnicas
- `MiddlewareWebCoreModule` e `MiddlewareWorkerModule` usam `HostBuilder`/ServiceCollection e `DependsOn`; registrar dependências mínimas e usar `BuildServiceProvider` para executar inicializadores.
- `ImpersonationManager` e `UserAppService` dependem de repositórios e `IUnitOfWorkManager`; mockar repositórios e `IRepository`.
- `TokenAuthController` e `ProfileControllerBase` são controllers base; criar controller concreto mínimo para testar helpers, `TwoFactorAuthenticate`, `UpdateProfilePicture` e `DeleteProfilePicture`.
- `SignalRChatCommunicator` e `ChatControllerBase` usam `IOnlineClientManager` e `IChatCommunicator`; mockar conexões e clients.
- `AzureActiveDirectorySettings` é POCO de configuração; validar parsing via `IConfiguration` com `AddInMemoryCollection`.
- `OCIKeyVaultManager` faz chamadas HTTP/REST; isolar com `HttpMessageHandler` mockado ou `NSubstitute` para `IKeyVaultManager`.
- `EpPlusExcelExporterBase` gera arquivos `.xlsx` em `MemoryStream`; limpar arquivos temporários após cada teste.
- `ExpiredAuditLogDeleterWorker` tem `MaxDeletionCount` privado; usar reflection para reduzir e cobrir `DoWork`.
- `UserFriendsCache` e `CultureHelper` usam `CultureInfo`/`DateTimeFormatInfo`; usar culturas concretas (`pt-BR`, `en-US`, `fr-FR`, `ar-SA`) para evitar comparações instáveis.
- `FileController` e `ChatControllerBase` retornam `IActionResult`; mockar `IFileStorageManager` e `IChatManager`.
- `AppConfigurations` e `SerilogEafHostBuilderExtensions` usam `IConfiguration`/`IHostBuilder`; usar `ConfigurationBuilder` e `HostBuilder` reais.
- `SwaggerEnumParameterFilter` e `SwaggerOperationFilter` processam `OpenApiParameter`/`OpenApiOperation`; criar `SchemaRepository` e `ApiDescription` mínimas.

## Validação
- `dotnet build Eaf.sln --configuration Release` deve passar sem erros.
- `bash run-tests-with-coverage.sh` deve passar sem falhas.
- Cobertura não pode regredir abaixo do baseline P40.
- CI do PR deve passar.
