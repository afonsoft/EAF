# EAF Session Summary P66 - Template API Runtime Validation & Swagger

## Data

2026-07-16

## Branch

`feature/devin-20260716-priority66-api-runtime`

## Objetivo

Validar o template API (`Eaf.ApiWithSrc.sln`) em runtime, abrir o Swagger em `localhost` e tratar os problemas de inicialização encontrados.

## Métricas Finais

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13311 / 13590) |
| Branch coverage | 90.5% (2597 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4605 total, 4605 passando, 0 ignorados |
| Build warnings (Eaf.sln) | 0 |
| Template warnings (Api/Worker) | 0 |

## Destaques

- **Swagger validado**: `http://localhost:5000/swagger` carregou a UI e o JSON `swagger/v1/swagger.json` retornou HTTP 200.
- **Endpoint validado**: `GET /api/services/app/About/GetAbout` retornou HTTP 200 com dados do ambiente.
- **Banco local**: SQL Server 2022 em container Docker (`mcr.microsoft.com/mssql/server:2022-latest`) na porta 1433.
- **Configuração por variáveis de ambiente**: corrigida a precedência em `AppConfigurations` e `EafHostBuilderExtensions` para que variáveis de ambiente sobrescrevam valores de `appsettings.json`/`appsettings.{Environment}.json` (comportamento padrão do .NET).
- **Templates buildam**: `Api`, `Worker` e `Angular` buildam com 0 warnings.

## Ajustes de Código

- `src/Eaf.Middleware.Core/Configuration/AppConfigurations.cs` — move `AddEnvironmentVariables` para depois dos arquivos JSON, garantindo que `ASPNETCORE_`, `EAF_` e variáveis sem prefixo possam substituir configurações do `appsettings`.
- `src/Eaf.Middleware.Core/Configuration/EafHostBuilderExtensions.cs` — mesma correção para o host builder (`UseEafConfiguration`).
- `src/Eaf.Middleware.Core/MiddlewareCoreModule.cs` — reposiciona comentário XML para evitar warning `CS1587`.
- `Templates/Worker/src/Eaf.ProjectName.Core/Application/AppConfigurations.cs` — aplica a mesma correção no template Worker.
- `test/Eaf.MiddlewareCore.Tests/Configuration/AppConfigurationsBddTests.cs` — adiciona teste BDD `Dado_AppsettingsEVariavelDeAmbienteComMesmoNome_Quando_Get_Entao_VariavelDeAmbienteSobrescreveJson`.

## Validação do Swagger

- URL: `http://localhost:5000/swagger`
- Página exibe "ProjectName API" v1 OAS 3.0 com endpoints `About` e `Account`.
- `GET /api/services/app/About/GetAbout` retornou:
  ```json
  {
    "result": {
      "version": "Eaf.Middleware.Core, Version=9.1.0.0, Culture=neutral, PublicKeyToken=null",
      "osVersion": "Unix 5.15.200.0",
      "os": "LINUX",
      "runtimeIdentifier": "linux-x64",
      "frameworkDescription": ".NET 10.0.10"
    }
  }
  ```

## Como Reproduzir Localmente

1. Subir SQL Server em Docker:
   ```bash
   docker run --name eaf-sqlserver -e ACCEPT_EULA=Y -e MSSQL_SA_PASSWORD=EafProjectName123! -e MSSQL_PID=Developer -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
   ```
2. Aplicar migrations:
   ```bash
   cd Templates/Api/src/Eaf.ProjectName.Migrator
   ConnectionStrings__LOCAL="Server=localhost,1433;Database=afonsoft_eaf;user id=sa;Password=EafProjectName123!;TrustServerCertificate=True;Encrypt=false" \
     Database__Provider=SqlServer ASPNETCORE_Docker_Enabled=true dotnet run --configuration Release -- -s
   ```
3. Executar a API:
   ```bash
   cd Templates/Api/src/Eaf.ProjectName.Web.Host
   ConnectionStrings__Default="Server=localhost,1433;Database=afonsoft_eaf;user id=sa;Password=EafProjectName123!;TrustServerCertificate=True;Encrypt=false" \
     Database__Provider=SqlServer Hangfire__IsEnabled=false SqlServerCache__IsEnabled=false \
     ASPNETCORE_ENVIRONMENT=Local ASPNETCORE_URLS=http://localhost:5000 \
     dotnet bin/Release/net10.0/Eaf.ProjectName.Web.Host.dll
   ```
4. Abrir `http://localhost:5000/swagger`.

## Aprendizados / Gotchas

- `AppConfigurations` e `EafHostBuilderExtensions` adicionavam `AddEnvironmentVariables` **antes** dos arquivos `appsettings.json`, fazendo com que o JSON sempre vencesse. Isso impedia ajustar `ConnectionStrings` por variáveis de ambiente, o que é essencial para rodar o template em ambientes locais/Docker sem alterar arquivos versionados.
- SQL Server em Docker precisa de `Encrypt=false` (ou certificado confiável) para conexão local com `Microsoft.Data.SqlClient`.
- OLTp para `https://otlp.nr-data.net` retorna 404/405 em ambiente local, mas não impede a subida da API.
- `Hangfire__IsEnabled=false` e `SqlServerCache__IsEnabled=false` reduzem dependências externas no primeiro teste local.

## Próximos Passos (P67)

Ver `eaf-next-session-prompt-p67.md`.
