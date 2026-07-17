# EAF Next Session Prompt P69 - Docker Compose Hardening & Integration Validation

## Contexto

O P68 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13311 / 13590) |
| Branch coverage | 90.5% (2598 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4605 total, 4604 passando, 0 ignorados |
| Build warnings (Eaf.sln) | 0 |
| Template warnings (Api/Worker/Angular) | 0 |

Branch ativa: `feature/devin-20260718-priority69-compose-hardening` (a criar a partir de `origin/main` no commit do P68).

## Objetivo

Endurecer o cenário Docker Compose criado no P68 para desenvolvimento local e prepará-lo para ambientes mais próximos de produção, sem alterar a cobertura de testes.

1. Adicionar persistência via volumes nomeados (SQL Server data, logs da API/Worker).
2. Gerenciar secrets/configuração local com `.env.example` (sem commitar `.env` real).
3. Adicionar healthchecks para `eaf-api` e `eaf-worker` além do SQL Server.
4. Criar script de validação (`scripts/validate-docker-compose.sh`) que sobe a stack, aguarda a saúde dos serviços, verifica endpoints e derruba tudo ao final.
5. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.5%
   - Method coverage >= 99.8%
6. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p69.md`, `docs/development/session-summaries/eaf-next-session-prompt-p70.md` e `.agents/MEMORY.md` com as métricas finais e notas.
7. Criar PR para `main`.

## Tarefas

1. `dotnet build Eaf.sln --configuration Release` e `bash run-tests-with-coverage.sh` (baseline).
2. Adicionar volumes nomeados no `docker-compose.yml` para `eaf-sqlserver` (`mssql-data`) e pastas de logs para `eaf-api`/`eaf-worker`.
3. Criar `.env.example` na raiz com as variáveis necessárias (`MSSQL_SA_PASSWORD`, `ConnectionStrings__Default`, `App__CorsOrigins`, etc.). Garantir que `.env` esteja no `.gitignore`.
4. Atualizar `docker-compose.yml` para ler `MSSQL_SA_PASSWORD` e outras variáveis sensíveis a partir de `.env` (sem hard-coded secrets no arquivo).
5. Adicionar healthcheck no `eaf-api` (ex: `http://localhost:8001/swagger/v1/swagger.json` ou `http://localhost:8001/` retornando 200/301) e no `eaf-worker` (ex: processo `dotnet` ativo ou log contendo `Worker running at`).
6. Criar `scripts/validate-docker-compose.sh` que:
   - Execute `docker compose up --build -d`.
   - Aguarde todos os serviços estarem `healthy`/`running` (com timeout).
   - Verifique `http://localhost:5000/swagger/v1/swagger.json` (HTTP 200).
   - Verifique `http://localhost:4200` (HTTP 200).
   - Verifique logs do `eaf-worker` sem palavras `Fatal`/`Exception` críticas.
   - Use `trap` para executar `docker compose down --volumes` ao sair, mesmo em caso de erro.
7. Executar o script de validação e corrigir problemas encontrados.
8. Atualizar documentação e MEMORY.
9. Criar PR para `main`.

## Restrições

- Não modificar `.github/workflows/`.
- Não reduzir cobertura de testes.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`) caso novos testes sejam adicionados.
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.
- Não commitar secrets (`.env`, connection strings, senhas, tokens).

## Notas P68 (aprendizados)

- O `docker-compose.yml` end-to-end sobe SQL Server, migrator, API, Worker e Angular com healthcheck do SQL Server e `depends_on`/`service_completed_successfully`.
- Healthcheck do SQL Server 2022 requer `/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P ... -Q 'SELECT 1' -C`.
- O `Templates/Worker/Dockerfile` precisou ser reescrito para buildar do repositório root com .NET 10.
- A API só expõe Swagger em ambientes não-Production (`ASPNETCORE_ENVIRONMENT=Staging` no compose).
- Logs de 404/405 do OpenTelemetry para `https://otlp.nr-data.net` são não-fatais.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p68.md`
- `docker-compose.yml`
- `Templates/Api/Dockerfile.migrator`
- `TestResults/CoverageReport/Summary.txt`
