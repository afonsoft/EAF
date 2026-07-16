# EAF Next Session Prompt P66 - Template API Runtime Validation & Swagger

## Contexto

O P65 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13310 / 13589) |
| Branch coverage | 90.5% (2598 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4604 total, 4603 passando, 1 ignorado |
| Build warnings (Eaf.sln) | 0 |
| Template warnings (Api/Worker) | 0 |

Branch ativa: `feature/devin-20260716-priority66-api-runtime` (a criar a partir de `origin/main` no commit do P65).

## Objetivo

Validar o template API em runtime, abrir o Swagger em `localhost` e garantir que a aplicação inicialize corretamente. Foco em:

1. Buildar e executar `Templates/Api/Eaf.ApiWithSrc.sln` (usa os projetos fonte do EAF ao invés dos pacotes NuGet).
2. Identificar e corrigir problemas de inicialização/runtime (connection string, migration, dependências, configuração).
3. Abrir o Swagger (`/swagger`) no navegador e validar que a página carrega e os endpoints principais estão documentados.
4. Se necessário, ajustar `appsettings*.json`, `Startup`/`Program` ou módulos para que o template API suba localmente.
5. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.5%
   - Method coverage >= 99.8%
6. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p66.md`, `docs/development/session-summaries/eaf-next-session-prompt-p67.md` e `.agents/MEMORY.md` com as métricas finais e notas.
7. Criar PR para `main`.

## Tarefas

1. `dotnet build Templates/Api/Eaf.ApiWithSrc.sln --configuration Release` e tratar erros de build.
2. Se houver migrations pendentes, avaliar se é seguro rodar `dotnet ef migrations add` / `dotnet ef database update` ou se deve usar `EnsureCreated`/`InMemory` para teste local.
3. Executar o projeto host (`src/Eaf.ProjectName.Web.Host` ou equivalente) via `dotnet run` e identificar a URL (`http://localhost:<port>`).
4. Abrir o Swagger no Chrome e validar:
   - Página `/swagger` carrega sem erro 500.
   - Endpoint(s) de autenticação/account estão presentes.
   - Pelo menos um endpoint de teste retorna 200/401 (esperado sem autenticação).
5. Verificar se há exceções no console/log e corrigir as de baixo risco (configuração, connection string, etc.).
6. `dotnet build Eaf.sln --configuration Release` e `bash run-tests-with-coverage.sh`; manter cobertura.
7. Build dos templates `Templates/Worker` e `Templates/Angular/Eaf.ProjectName.UI` para garantir que não regressaram.
8. Atualizar documentação e MEMORY.
9. Criar PR para `main`.

## Restrições

- Não modificar `.github/workflows/`.
- Não reduzir cobertura de testes.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`) caso novos testes sejam adicionados.
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.
- Não commitar secrets (connection strings, senhas, tokens).

## Notas P65 (aprendizados)

- `Eaf.sln` continua com 0 warnings; cobertura mantida em 97.9% / 90.5% / 99.8%.
- Warnings de template (`Pomelo` NU1608 e `AutoMapper` NU1903) foram suprimidos de forma documentada nos `common.props` dos templates, pois as versões seguras ainda não estão disponíveis publicamente.
- `Templates/Api`, `Templates/Worker` e `Templates/Angular/Eaf.ProjectName.UI` buildam com 0 erros e 0 warnings.
- `Eaf.ApiWithSrc.sln` ainda não foi executado em runtime; pode haver ajustes de configuração necessários.

## Referências

- `docs/development/session-summaries/eaf-session-summary-p65.md`
- `Templates/Api/Eaf.ApiWithSrc.sln`
- `TestResults/CoverageReport/Summary.txt`
