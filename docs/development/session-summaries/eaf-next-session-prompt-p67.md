# EAF Next Session Prompt P67 - Worker Template Runtime & Integration

## Contexto

O P66 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13311 / 13590) |
| Branch coverage | 90.5% (2597 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4605 total, 4605 passando, 0 ignorados |
| Build warnings (Eaf.sln) | 0 |
| Template warnings (Api/Worker) | 0 |

Branch ativa: `feature/devin-20260716-priority67-worker-runtime` (a criar a partir de `origin/main` no commit do P66).

## Objetivo

Validar o template Worker em runtime, garantir que ele consiga se conectar a uma fila/job store local e confirmar a integração entre os templates. Foco em:

1. Buildar e executar `Templates/Worker/Eaf.ProjectName.WorkerService.sln` (com os projetos fonte do EAF, se houver um `WithSrc` equivalente; senão, usar a solution principal do Worker).
2. Identificar e corrigir problemas de inicialização/runtime (connection string, configuração, jobs, logging).
3. Garantir que o Worker inicie sem erros críticos em ambiente local/Docker.
4. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.5%
   - Method coverage >= 99.8%
5. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p67.md`, `docs/development/session-summaries/eaf-next-session-prompt-p68.md` e `.agents/MEMORY.md` com as métricas finais e notas.
6. Criar PR para `main`.

## Tarefas

1. `dotnet build Templates/Worker/Eaf.ProjectName.WorkerService.sln --configuration Release` e tratar erros de build.
2. Se necessário, executar o Worker localmente com configuração apontando para o SQL Server Docker já criado no P66 (ou outro banco/job store local).
3. Verificar se o Worker inicia, registra módulos e processa jobs sem erros fatais.
4. `dotnet build Eaf.sln --configuration Release` e `bash run-tests-with-coverage.sh`; manter cobertura.
5. Build dos templates `Templates/Api` e `Templates/Angular/Eaf.ProjectName.UI` para garantir que não regressaram.
6. Atualizar documentação e MEMORY.
7. Criar PR para `main`.

## Restrições

- Não modificar `.github/workflows/`.
- Não reduzir cobertura de testes.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`) caso novos testes sejam adicionados.
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.
- Não commitar secrets (connection strings, senhas, tokens).

## Notas P66 (aprendizados)

- `Eaf.ApiWithSrc.sln` inicia e o Swagger carrega em `http://localhost:5000/swagger`.
- `AppConfigurations` e `EafHostBuilderExtensions` foram corrigidos para que variáveis de ambiente sobrescrevam `appsettings.json`.
- SQL Server 2022 Docker com `Encrypt=false` funciona para testes locais.
- `Hangfire__IsEnabled=false` e `SqlServerCache__IsEnabled=false` ajudam a reduzir dependências em testes locais.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p66.md`
- `Templates/Worker/Eaf.ProjectName.WorkerService.sln`
- `TestResults/CoverageReport/Summary.txt`
