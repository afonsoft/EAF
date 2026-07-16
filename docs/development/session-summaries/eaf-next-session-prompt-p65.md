# EAF Next Session Prompt P65 - Template Dependency Warnings & Sonar Debt

## Contexto

O P64 foi concluído. As métricas atuais são:

| Métrica | Valor |
|---------|-------|
| Line coverage | 97.9% (13310 / 13589) |
| Branch coverage | 90.5% (2597 / 2868) |
| Method coverage | 99.8% (2159 / 2162) |
| Tests | 4604 total, 4603 passando, 1 ignorado |
| Build warnings (Eaf.sln) | 0 |

Branch ativa: `feature/devin-20260715-priority65-template-deps` (a criar a partir de `origin/main` no commit do P64).

## Objetivo

Tratar os warnings restantes nos templates e débito técnico pendentes do SonarCloud, sem diminuir a cobertura de testes. Foco em:

1. Avaliar e aplicar atualizações seguras de dependências nos templates (`Pomelo.EntityFrameworkCore.MySql`, `AutoMapper`) quando versões compatíveis estiverem disponíveis.
2. Reduzir warnings de build nos templates `Templates/Api` e `Templates/Worker` (Pomelo `NU1608`, AutoMapper `NU1903`).
3. Revisar o quality gate do SonarCloud no PR #199/P64 e tratar issues classificadas como `Bug` ou `Vulnerability` de baixo risco, se houver.
4. Manter ou aumentar as métricas:
   - Line coverage >= 97.9%
   - Branch coverage >= 90.5%
   - Method coverage >= 99.8%
5. Garantir que `Templates/Api`, `Templates/Worker` e `Templates/Angular/Eaf.ProjectName.UI` continuem buildando.

## Tarefas

1. Verificar disponibilidade de versões compatíveis de `Pomelo.EntityFrameworkCore.MySql` para EF Core 10 e de `AutoMapper` sem a vulnerabilidade `GHSA-rvv3-g6hj-g44x` que seja compatível com `Abp.AutoMapper 10.4.0`.
2. Se houver versão segura, atualizar `Templates/Api` e/ou `Templates/Worker` (csproj/Directory.Build.props) e reexecutar:
   - `dotnet build Templates/Api/Eaf.ProjectName.sln --configuration Release`
   - `dotnet build Templates/Worker/Eaf.ProjectName.WorkerService.sln --configuration Release`
3. Se ainda não houver versão segura, documentar a dependência bloqueadora e adicionar/suprimir warnings de forma consistente com `common.props` (sem esconder vulnerabilidades sem justificativa).
4. Verificar quality gate do SonarCloud após o merge do P64 e tratar issues classificadas como `Bug`/`Vulnerability` de baixo risco.
5. `dotnet build Eaf.sln --configuration Release` e `bash run-tests-with-coverage.sh`; manter cobertura.
6. Build do Angular:
   - `cd Templates/Angular/Eaf.ProjectName.UI && source /home/ubuntu/.nvm/nvm.sh && nvm use 20 && npm install --legacy-peer-deps && npx ng build --configuration=production`
7. Atualizar `README.md`, `README_pt.md`, `docs/development/session-summaries/eaf-session-summary-p65.md`, `docs/development/session-summaries/eaf-next-session-prompt-p66.md` e `.agents/MEMORY.md` com as métricas finais e notas.
8. Criar PR para `main`.

## Restrições

- Não modificar `.github/workflows/`.
- Não reduzir cobertura de testes.
- Não suprimir warnings de vulnerabilidade sem documentar o motivo e a dependência bloqueadora.
- Nomes de testes BDD em português (`Dado_..._Quando_..._Entao_...`) caso novos testes sejam adicionados.
- Mensagens de commit em inglês com prefixos `feat:/fix:/test:/docs:`.

## Notas P64 (aprendizados)

- `Eaf.sln` atingiu 0 warnings com `<Nullable>annotations</Nullable>` nos testes, remoção de pacotes transitivos desnecessários, ajuste de `new`/`SYSLIB0050`/`CA1416` e remoção de `ServicePointManager` no template Worker.
- SonarCloud quality gate do PR #198 passou com 0 new issues.
- Templates `Api` e `Worker` ainda emitem warnings por dependências externas:
  - `Pomelo.EntityFrameworkCore.MySql` 9.0.0 `NU1608` (EF Core 10 não suportado na versão estável atual).
  - `AutoMapper` 14.0.0 `NU1903` (não há patch 14.x; >= 15 binário-incompatível com `Abp.AutoMapper 10.4.0`).

## Referências

- `docs/development/session-summaries/eaf-session-summary-p64.md`
- `TestResults/CoverageReport/Summary.txt`
- `TestResults/CoverageReport/index.html`
