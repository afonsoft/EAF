# RULES.md — EAF Guardrails

## Hard Rules (bloqueio imediato)

| # | Regra | Verificação |
|---|-------|-------------|
| H1 | Branches `main` e `develop` protegidas — merge apenas via PR | CI: branch protection |
| H2 | Todos os testes devem passar antes do merge | CI: `dotnet test` |
| H3 | Cobertura de código não pode diminuir | CI: coverlet + SonarCloud |
| H4 | APIs públicas devem ter documentação XML | CI: `GenerateDocumentationFile=true` |
| H5 | Nunca commitar secrets (`.env`, tokens, connection strings) | `.gitignore` + review |
| H6 | Não modificar `.github/workflows/` sem revisão humana | CODEOWNERS |
| H7 | Não editar arquivos gerados (`service-proxies.ts`, `*.Designer.cs`) | `.aiignore` |
| H8 | Não push direto em `main` ou `develop` | Branch protection rules |
| H9 | Não usar `--no-verify` ou `--force` sem aprovação | Git hooks |
| H10 | Não reduzir número de testes existentes | CI: test count check |

## Soft Rules (warning + confirmação)

| # | Regra | Ação |
|---|-------|------|
| S1 | Modificar `Dockerfile` ou `docker-compose.yml` | Confirmar com usuário |
| S2 | Alterar `common.props` (afeta todos os projetos) | Confirmar compatibilidade |
| S3 | Deletar arquivos de teste | Exigir justificativa |
| S4 | Alterar dependências NuGet globais | Verificar breaking changes |
| S5 | Modificar `appveyor.yml` ou CI legado | Confirmar necessidade |
| S6 | Alterar schema do banco (migrations) | Verificar rollback |
| S7 | Adicionar novo módulo middleware | Seguir padrão de módulos ABP |

## Permissões por Ambiente

| Ambiente | Read | Write | Execute | Deploy |
|----------|------|-------|---------|--------|
| **dev** | Livre | Livre | Sandbox | — |
| **staging** | Livre | Via PR | Sandbox + logs | Via CI |
| **prod** | Livre | Bloqueado | Bloqueado | Via release workflow |

## Tool Permissions

- **Read-only** por padrão: busca, navegação, análise
- **Write** via gates de aprovação: edição de código, criação de arquivos
- **Execute** em sandbox com logging: build, test, lint
- **Deploy**: apenas via workflows CI/CD aprovados

## Arquivos Imutáveis (Do Not Touch)

```
node_modules/
bin/
obj/
nupkg/
TestResults/
sonar/
.git/
.idea/
.vscode/
.vs/
```
