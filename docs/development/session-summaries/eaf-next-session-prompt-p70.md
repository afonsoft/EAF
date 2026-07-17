# EAF Next Session Prompt P70 - CI/CD para Docker Compose

## Contexto

O P69 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13311 / 13590) |
| Branch coverage | 90.5% (2598 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4605 total, 4604 passando, 0 ignorados |
| Build warnings (Eaf.sln) | 0 |
| Template warnings (Api/Worker/Angular) | 0 |

Branch ativa: `feature/devin-20260719-priority70-compose-cicd` (a criar a partir de `origin/main` no commit do P69).

## Objetivo

Automatizar a validação do Docker Compose no CI do repositório, garantindo que cada PR que afete os arquivos `docker-compose*.yml`, `Dockerfile*` ou `scripts/validate-docker-compose.sh` execute a stack e valide os endpoints.

1. Criar um novo workflow `.github/workflows/docker-compose-validation.yml` que:
   - Dispare em `pull_request` e `workflow_dispatch` quando houver alterações nos caminhos Docker/Compose.
   - Faça checkout do repositório.
   - Execute `dotnet build Eaf.sln --configuration Release` (ou reaproveite o build existente).
   - Execute `bash scripts/validate-docker-compose.sh` com `COMPOSE_FILE=docker-compose.all.yml`.
   - Em caso de falha, faça upload dos logs dos containers como artifacts.
2. Otimizar o tempo de execução da validação (cache de imagens Docker, cache NuGet, etc.) se necessário.
3. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.5%
   - Method coverage >= 99.8%
4. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p70.md`, `docs/development/session-summaries/eaf-next-session-prompt-p71.md` e `.agents/MEMORY.md` com as métricas finais e notas.
5. Criar PR para `main`.

## Tarefas

1. `dotnet build Eaf.sln --configuration Release` e `bash run-tests-with-coverage.sh` (baseline).
2. Criar `.github/workflows/docker-compose-validation.yml` com os passos descritos no objetivo.
   - Usar `ubuntu-latest` e habilitar Docker Buildx.
   - Configurar timeout adequado (ex: 20 minutos).
   - Permitir execução manual (`workflow_dispatch`) e em PRs que toquem nos paths relevantes.
3. Testar o workflow localmente se possível (`act`) ou validar a sintaxe YAML com `python -c 'import yaml'` / `yamllint`.
4. Executar `bash scripts/validate-docker-compose.sh` para garantir que a stack ainda sobe e os endpoints respondem.
5. `dotnet build Eaf.sln --configuration Release` e `bash run-tests-with-coverage.sh`; manter cobertura.
6. Atualizar documentação e MEMORY.
7. Criar PR para `main`.

## Restrições

- Não modificar workflows existentes em `.github/workflows/`; apenas adicionar o novo workflow `docker-compose-validation.yml`.
- Não reduzir cobertura de testes.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`) caso novos testes sejam adicionados.
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.
- Não commitar secrets (`.env`, connection strings, senhas, tokens).

## Notas P69 (aprendizados)

- `docker-compose.all.yml` contém a stack completa (SQL Server, Migrator, API, Worker, Angular) com healthchecks e volumes nomeados.
- `docker-compose.yml` é a versão mínima (API + Angular) sem infraestrutura, totalmente parametrizada por variáveis de ambiente.
- `scripts/validate-docker-compose.sh` sobe a stack, aguarda endpoints e verifica logs do Worker.
- Healthchecks: API (`curl http://localhost:8001/health`), Worker (`pgrep -x dotnet`), Angular (`curl http://localhost/`), SQL Server (`sqlcmd`).

## Referências

- `docs/development/session-summaries/eaf-session-summary-p69.md`
- `docker-compose.all.yml`
- `docker-compose.yml`
- `scripts/validate-docker-compose.sh`
- `TestResults/CoverageReport/Summary.txt`
