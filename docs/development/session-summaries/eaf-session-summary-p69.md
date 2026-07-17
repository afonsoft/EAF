# EAF Session Summary P69 - Docker Compose Hardening & Variable-Driven Stacks

## Data

2026-07-17

## Branch

`feature/devin-20260718-priority69-compose-hardening`

## Objetivo

Endurecer a orquestração Docker Compose criada no P68, separar a stack completa de uma versão mínima (API + Angular) e garantir que tudo seja configurável por variáveis de ambiente.

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

- **Split do `docker-compose.yml`**:
  - `docker-compose.all.yml` — stack completo (SQL Server, Migrator, API, Worker, Angular) com volumes e healthchecks.
  - `docker-compose.yml` — API + Angular apenas, sem infraestrutura, totalmente parametrizado por variáveis de ambiente (`.env`).
- **Hardening**:
  - Volumes nomeados para persistência do SQL Server (`mssql-data`) e logs da API/Worker (`eaf-api-logs`, `eaf-worker-logs`).
  - Healthchecks para `eaf-api` (`/health`), `eaf-worker` (`pgrep -x dotnet`) e `eaf-angular` (`curl http://localhost/`).
  - `restart: unless-stopped` nos serviços long-running.
- **Segurança de configuração**:
  - Criado `.env.example` com variáveis documentadas.
  - `.env` adicionado ao `.gitignore`; nenhuma senha real é commitada.
  - `docker-compose.all.yml` e `docker-compose.yml` usam `${VAR}`/`${VAR:?required}`, sem secrets hard-coded.
- **Validação automatizada**:
  - Criado `scripts/validate-docker-compose.sh` que sobe a stack, aguarda endpoints, verifica o migrator e analisa logs do Worker.
- **Ambos os compose foram validados**:
  - `docker-compose.all.yml`: `http://localhost:5000/swagger/v1/swagger.json` (200) e `http://localhost:4200/` (200).
  - `docker-compose.yml` (com infra do `docker-compose.all.yml` como `eaf-sqlserver`/`eaf-migrator`): mesmos endpoints respondendo.

## Ajustes de Código

- `docker-compose.yml` -> `docker-compose.all.yml` (renomeado) com adição de volumes, healthchecks e restart.
- `docker-compose.yml` (novo) contendo apenas `eaf-api` e `eaf-angular`, com variáveis de ambiente.
- `Templates/Api/Dockerfile` — adicionados `ca-certificates` e `curl` para o healthcheck interno da API.
- `Templates/Worker/Dockerfile` — adicionado `procps` para o healthcheck do Worker (`pgrep`).
- `.env.example` — exemplo de todas as variáveis necessárias.
- `.gitignore` — `.env` ignorado.
- `scripts/validate-docker-compose.sh` — script de validação end-to-end.

## Como Reproduzir

### Stack completo

```bash
cd /path/to/EAF
cp .env.example .env
# ajuste .env se necessário
docker compose -f docker-compose.all.yml up --build -d
```

### Stack mínimo (com SQL Server do compose completo)

```bash
docker compose -f docker-compose.all.yml up -d eaf-sqlserver eaf-migrator
docker compose -f docker-compose.yml up --build -d
```

### Validação automatizada

```bash
bash scripts/validate-docker-compose.sh
```

## Próximos Passos (P70)

Ver `eaf-next-session-prompt-p70.md`.
