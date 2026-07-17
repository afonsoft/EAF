# EAF Next Session Prompt P68 - Docker Compose & End-to-End Integration

## Contexto

O P67 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13311 / 13590) |
| Branch coverage | 90.5% (2597 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4605 total, 4604 passando, 1 ignorado |
| Build warnings (Eaf.sln) | 0 |
| Template warnings (Api/Worker/Angular) | 0 |

Branch ativa: `feature/devin-20260717-priority68-docker-integration` (a criar a partir de `origin/main` no commit do P67).

## Objetivo

Criar/validar um cenário de execução end-to-end com os templates API, Worker e Angular orquestrados via Docker Compose, garantindo que a stack completa suba sem erros e que os serviços se comuniquem corretamente.

1. Criar ou revisar `docker-compose.yml` na raiz (ou em `Templates/`) que suba:
   - SQL Server 2022 (Docker).
   - API (`Templates/Api/src/Eaf.ProjectName.Web.Host`).
   - Worker (`Templates/Worker/src/Eaf.ProjectName.WorkerService`).
   - Angular (`Templates/Angular/Eaf.ProjectName.UI`) servido por nginx.
2. Validar que a API, o Worker e o Angular inicializam com as variáveis de ambiente corretas.
3. Garantir que o Worker consiga enfileirar/processar jobs (ou ao menos inicializar sem erros fatais quando Hangfire estiver habilitado).
4. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.5%
   - Method coverage >= 99.8%
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p68.md`, `docs/development/session-summaries/eaf-next-session-prompt-p69.md` e `.agents/MEMORY.md` com as métricas finais e notas.
6. Criar PR para `main`.

## Tarefas

1. `dotnet build Templates/Api/Eaf.ApiWithSrc.sln --configuration Release` e `dotnet build Templates/Worker/Eaf.ProjectName.WorkerService.sln --configuration Release` (regressão).
2. Criar/revisar `docker-compose.yml` com serviços `eaf-sqlserver`, `eaf-api`, `eaf-worker` e `eaf-angular`.
3. Garantir que a API execute migrations na inicialização (ou via `Eaf.ProjectName.Migrator` como serviço `eaf-migrator` com `depends_on` no SQL Server).
4. Executar `docker compose up --build` e verificar logs de cada serviço.
5. Validar que `http://localhost:5000/swagger` (API) e `http://localhost:4200` (Angular) respondem.
6. `dotnet build Eaf.sln --configuration Release` e `bash run-tests-with-coverage.sh`; manter cobertura.
7. Atualizar documentação e MEMORY.
8. Criar PR para `main`.

## Restrições

- Não modificar `.github/workflows/`.
- Não reduzir cobertura de testes.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`) caso novos testes sejam adicionados.
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.
- Não commitar secrets (connection strings, senhas, tokens).

## Notas P67 (aprendizados)

- Worker template `ProjectNameCoreModule` precisa depender de `MiddlewareCoreModule` para inicializar `AbpZeroEntityTypes`.
- `Hangfire__IsEnabled=false` e `SqlServerCache__IsEnabled=false` simplificam testes locais.
- SQL Server Docker com `Encrypt=false` funciona para testes locais.
- `Eaf.sln` builda com 0 warnings; comentários XML devem preceder atributos `[DependsOn]`.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p67.md`
- `Templates/Api/Eaf.ApiWithSrc.sln`
- `Templates/Worker/Eaf.ProjectName.WorkerService.sln`
- `Templates/Angular/Eaf.ProjectName.UI/angular.json`
- `TestResults/CoverageReport/Summary.txt`
