# TOOLS.md — EAF Ferramentas e MCP

## Ferramentas de Build

| Ferramenta | Comando | Categoria |
|-----------|---------|-----------|
| dotnet restore | `dotnet restore Eaf.sln` | Read-only |
| dotnet build | `dotnet build Eaf.sln --configuration Release` | Execute |
| dotnet test | `dotnet test Eaf.sln --collect:"XPlat Code Coverage"` | Execute |
| dotnet pack | `dotnet pack --configuration Release` | Execute |
| npm install | `npm install --legacy-peer-deps` | Execute |
| ng build | `npx ng build --configuration=production` | Execute |
| ng test | `npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox` | Execute |

## Ferramentas de Qualidade

| Ferramenta | Comando | Categoria |
|-----------|---------|-----------|
| ESLint | `npx eslint src --debug` (Angular) | Execute |
| SonarCloud | `./sonarcloud.sh` | Execute |
| coverlet | via `--collect:"XPlat Code Coverage"` | Execute |
| Code Analysis | habilitado via `.editorconfig` e analyzers | Automático |

## Ferramentas de CI/CD

| Workflow | Trigger | Descrição |
|---------|---------|-----------|
| `ci-build-test.yml` | push/PR em develop, feature/*, bug/* | Build + testes + cobertura |
| `publish-all.yml` | tag release | Publicação NuGet |
| `code-quality.yml` | PR | Análise SonarCloud/Qodana |
| `security-scan.yml` | schedule/PR | Scan de segurança |
| `release.yml` | manual | Release workflow |
| `auto-pr-from-main.yml` | push em main | Auto-PR para develop |

## Ferramentas de Desenvolvimento

| Ferramenta | Uso | Categoria |
|-----------|-----|-----------|
| `eaf-cli` | Gerenciamento de builds UI e NSwag | Execute |
| `nvm` | Gerenciamento de versões Node.js | Execute |
| `run-tests-with-coverage.sh` | Script de testes com cobertura | Execute |
| `build-and-test.sh` | Build e testes completos | Execute |

## APIs Externas

| API | Uso | Rate Limit | Headers |
|-----|-----|-----------|---------|
| NuGet.org | Publicação de pacotes | — | API Key via CI secrets |
| SonarCloud | Análise de qualidade | — | Token via CI secrets |
| GitHub API | CI/CD, PRs, Issues | 5000/hora | GITHUB_TOKEN |

## MCP Servers

| Servidor | Uso |
|---------|-----|
| deepwiki | Documentação de repositórios GitHub |

## Princípios de Design de Tools

- Nomeadas pelo que fazem, não como fazem
- Schemas mínimos, erros em JSON
- Operações idempotentes
- Read-only por padrão; write via gates de aprovação
- Execute em sandbox com logging
