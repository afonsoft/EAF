# EAF Session Summary P37 - Coverage Audit

## Sessão
- **Branch:** `feature/devin-20260711-priority37-coverage-audit`
- **Data:** 2026-07-11
- **PR:** #137 (https://github.com/afonsoft/EAF/pull/137)

## Baseline
| Métrica | Valor |
|---------|-------|
| Line | 87.6% |
| Branch | 67.2% |
| Method | 96.2% |
| Covered Lines | 11981 / 13672 |
| Covered Branches | 1971 / 2932 |
| Covered Methods | 2022 / 2100 |

## Resultado
| Métrica | Valor |
|---------|-------|
| Line | 88.1% |
| Branch | 68.0% |
| Method | 96.3% |
| Covered Lines | 12051 / 13672 |
| Covered Branches | 1994 / 2932 |
| Covered Methods | 2023 / 2100 |

## Código de produção alterado
Nenhum. Apenas testes foram adicionados/ajustados nesta sessão.

## Testes adicionados/ajustados

- `test/Eaf.KeyVault.Tests/Azure/AzureKeyVaultManagerBddTests.cs` (novo)
  - `Dado_ChaveExistente_Quando_GetValue_Entao_DeveRetornarValor`
  - `Dado_ChaveExistente_Quando_GetValueAsync_Entao_DeveRetornarValor`
  - `Dado_ChaveExistente_Quando_GetKeyValues_Entao_DeveRetornarDicionario`
  - `Dado_ChaveExistente_Quando_GetKeyValuesAsync_Entao_DeveRetornarDicionario`
  - `Dado_ValorValido_Quando_SetValue_Entao_DeveDefinirValor`
  - `Dado_ValorValido_Quando_SetValueAsync_Entao_DeveDefinirValor`
  - `Dado_GetSecretLancandoExcecao_Quando_GetValue_Entao_DeveLancarExcecaoOriginal`
  - `Dado_GetPropertiesOfSecretsLancandoExcecao_Quando_GetKeyValues_Entao_DeveLancarExcecaoOriginal`

- `test/Eaf.Log4NetServiceBus.Tests/Logging/ServiceBusQueueAppenderBddTests.cs`
  - `Dado_ConexaoPreConfiguradaComTimeout_Quando_SendBufferComEventos_Entao_DeveTratarServiceBusTimeoutException`
  - `Dado_AppenderComConexaoAberta_Quando_OnClose_Entao_DeveFecharConexaoSemLancarExcecao`

- `test/Eaf.Middleware.Web.Core.Tests/WebHooks/EafWebhookReceiverBddTests.cs`
  - `Dado_LocalizationSourceNaoDefinido_Quando_Acessar_Entao_DeveLancarExcecao`
  - `Dado_LocalizationManagerComSource_Quando_AcessarLocalizationSource_Entao_DeveRetornarSource`

- `test/Eaf.OpenTelemetry.Tests/EafOpenTelemetryServiceCollectionExtensionsBddTests.cs`
  - `Dado_ServiceCollection_Quando_IniciarHostedServices_Entao_DeveCriarTracerEMeterProviders`

## READMEs atualizados
- `README.md` e `README_pt.md` foram atualizados com as novas métricas de testes (Line 88.1%, Branch 68.0%, Method 96.3%, Total 4063, Passing 4062).

## Comandos executados
```bash
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet dotnet build Eaf.sln --configuration Release
PATH=/home/ubuntu/.dotnet:/home/ubuntu/.dotnet/tools:$PATH DOTNET_ROOT=/home/ubuntu/.dotnet bash run-tests-with-coverage.sh
```

## Observações
- A cobertura geral subiu em relação ao baseline P36 (Line 87.6% -> 88.1%, Branch 67.2% -> 68.0%, Method 96.2% -> 96.3%).
- `AzureKeyVaultManager` subiu de 75.3% para 100%.
- `ServiceBusQueueAppender` subiu de 51.4% para 64.2%.
- `EafWebHookReceiver` subiu de 75.7% para 90.9%.
- `EafOpenTelemetryServiceCollectionExtensions` subiu de 78.7% para 98.1%.
- `HangFireConfigurer` manteve 77.5% porque a execução do lambda `AddHangfire` conflita com `UseConsole` já inicializado no processo de testes.
- `TenantAppService` manteve 79.6% porque `TenantManager.CreateWithAdminUserAsync` é não-virtual e não pode ser mockado diretamente.
- Não houve alteração em `.github/workflows/`.
- Não houve alteração em código de produção.

## Classes com cobertura ainda baixa (foco P38)
- `Eaf.Middleware.Web.Controllers.TokenAuthController` (46.4%)
- `Eaf.Middleware.Core.Authentication.External.OpenIdConnect.OpenIdConnectAuthProviderApi` (47.6%)
- `Eaf.Middleware.Web.MiddlewareWebCoreModule` (69.6%)
- `Eaf.Middleware.Web.WebContentDirectoryFinder` (70.8%)
- `Eaf.Middleware.Web.Startup.HangFireConfigurer` (77.5%)
- `Eaf.Middleware.MultiTenancy.TenantAppService` (79.6%)
