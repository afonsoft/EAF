# EAF Session Summary P68 - Docker Compose & End-to-End Integration

## Data

2026-07-17

## Branch

`feature/devin-20260717-priority68-docker-integration`

## Objetivo

Criar/validar um cenário de execução end-to-end com os templates API, Worker e Angular orquestrados via Docker Compose, garantindo que a stack completa suba sem erros e que os serviços se comuniquem corretamente.

## Métricas Finais

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13311 / 13590) |
| Branch coverage | 90.5% (2598 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4605 total, 4604 passando, 0 ignorados |
| Build warnings (Eaf.sln) | 0 |
| Template warnings (Api/Worker/Angular) | 0 |

## Destaques

- **Docker Compose end-to-end validado**: SQL Server 2022, migrator, API, Worker e Angular (nginx) iniciam e se comunicam na rede `eaf-network`.
- **Migrações executadas na inicialização**: serviço `eaf-migrator` roda `Eaf.ProjectName.Migrator.dll -s` com `ASPNETCORE_Docker_Enabled=true` antes de subir API/Worker.
- **Worker inicia sem erros críticos**: 11 módulos ABP carregados e o loop do `Worker` executa; conexão com SQL Server via variáveis de ambiente.
- **Swagger e Angular respondem**:
  - `http://localhost:5000/swagger` -> HTTP 301/200 (UI do Swagger).
  - `http://localhost:5000/swagger/v1/swagger.json` -> HTTP 200.
  - `http://localhost:4200` -> HTTP 200 (nginx servindo o build Angular).
- **Cobertura mantida** e `Eaf.sln` builda com 0 warnings.

## Ajustes de Código

- `Templates/Worker/Dockerfile` — reescrito para usar o repositório como contexto de build, caminho correto `Templates/Worker/src/Eaf.ProjectName.WorkerService`, imagens .NET 10 e variáveis de ambiente padrão (`DOTNET_ENVIRONMENT=Production`, `ConnectionStrings__Default`, `Database__Provider=SqlServer`, `Hangfire__IsEnabled=false`, `SqlServerCache__IsEnabled=false`).
- `Templates/Api/Dockerfile.migrator` — novo Dockerfile dedicado para executar o projeto `Eaf.ProjectName.Migrator` em container e aplicar migrations antes dos serviços iniciarem.
- `docker-compose.yml` (raiz) — novo arquivo orquestrando `eaf-sqlserver`, `eaf-migrator`, `eaf-api`, `eaf-worker` e `eaf-angular`, com healthcheck do SQL Server, `depends_on` e variáveis de ambiente compartilhadas.
- `.dockerignore` — adicionadas exclusões para `TestResults/`, `.devin-files/` e `*.log` para reduzir o contexto de build.

## Como Reproduzir

1. Garantir que a porta 1433, 5000 e 4200 estejam livres.
2. Na raiz do repositório:
   ```bash
   docker compose up --build
   ```
3. Aguardar o healthcheck do SQL Server e a conclusão do `eaf-migrator`.
4. Acessar:
   - API Swagger: `http://localhost:5000/swagger`
   - Angular: `http://localhost:4200`

## Aprendizados / Gotchas

- A imagem `mcr.microsoft.com/mssql/server:2022-latest` não possui mais `/opt/mssql-tools/bin/sqlcmd`; o caminho correto é `/opt/mssql-tools18/bin/sqlcmd` e é necessário o parâmetro `-C` para confiar no certificado no healthcheck.
- O Worker template tinha um `Dockerfile` legado (contexto errado, .NET 8, caminho do projeto incorreto); precisou ser reescrito para buildar corretamente com os projetos fonte do EAF.
- A API só expõe Swagger em ambientes não-Production (`Startup.cs`); no compose o serviço `eaf-api` usa `ASPNETCORE_ENVIRONMENT=Staging` para que a documentação esteja disponível.
- O OpenTelemetry nos containers tenta exportar métricas para `https://otlp.nr-data.net` e loga 404/405, mas isso é não-fatal e não impede o startup nem o atendimento de requisições.

## Próximos Passos (P69)

Ver `eaf-next-session-prompt-p69.md`.
