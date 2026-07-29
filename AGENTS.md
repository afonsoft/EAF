# AGENTS.md — EAF (Enterprise Application Foundation)

## Missão

EAF é uma plataforma middleware enterprise open-source construída sobre ASP.NET Boilerplate (ABP) para .NET 10.0. Fornece módulos reutilizáveis para identidade, cache, observabilidade, background jobs e secret management. Qualquer agente LLM que trabalhe neste repositório deve seguir as convenções aqui documentadas.

## Stack Tecnológica

| Camada | Tecnologia | Versão |
|--------|-----------|--------|
| Runtime | .NET | 10.0 |
| Framework | ASP.NET Boilerplate (ABP) | 10.5.0 |
| ORM | Entity Framework Core | 10.0 |
| DI | Castle Windsor | — |
| Frontend | Angular | 18 (Templates) |
| Testes | xUnit + Shouldly + NSubstitute | — |
| Background | Hangfire | — |
| Realtime | SignalR | — |
| Observabilidade | OpenTelemetry + Serilog | — |
| Secrets | Azure Key Vault | — |
| CI/CD | GitHub Actions | — |
| Qualidade | SonarCloud + coverlet | — |
| Licença | GPL-3.0-or-later | — |

## Estrutura do Projeto

```
src/                          # 14 módulos middleware
├── Eaf.Middleware.Core/      # Núcleo do framework (entidades, repos, serviços)
├── Eaf.Middleware.Application/ # Camada de aplicação (DTOs, AppServices)
├── Eaf.Middleware.Web.Core/  # Camada web (controllers, filtros)
├── Eaf.Middleware.AzureActiveDirectory/
├── Eaf.Middleware.Ldap/
├── Eaf.Middleware.Worker/
├── Eaf.KeyVault/             # Secret management
├── Eaf.KeyVault.AspNetCore/
├── Eaf.OpenTelemetry/        # Observabilidade distribuída
├── Eaf.Castle.Serilog/       # Logging estruturado
├── Eaf.SqlServerCache/       # Cache SQL Server
├── Eaf.SqliteCache/          # Cache SQLite
└── Eaf.Log4NetServiceBus/
test/                         # 14 projetos de teste (espelham src/)
Templates/                    # Templates de projeto
├── Api/                      # Template .NET 10 Web API
├── Angular/                  # Template Angular 18 (Metronic)
├── Worker/                   # Template Background Worker
└── Eaf.Gateways.API/        # Template API Gateway
docs/                         # Documentação técnica
.agents/                      # Infraestrutura de agentes
├── skills/                   # 25 SKILL.md files
├── CONTEXT.md                # Estratégias de carregamento de contexto
├── RULES.md                  # Guardrails (hard/soft rules)
├── MEMORY.md                 # Estado cross-session
├── TOOLS.md                  # Ferramentas e MCP
├── WORKFLOWS.md              # Automação e CI/CD
└── README.md                 # Documentação do harness
```

## Caminhos por Plataforma

| Plataforma | Config Principal | Skills | Rules |
|-----------|-----------------|--------|-------|
| Base (todas) | `AGENTS.md` | `.agents/skills/` | `.agents/RULES.md`, `rules/` |
| Claude Code | `CLAUDE.md` | auto-loaded | `.agents/RULES.md` |
| Devin | `AGENTS.md` | `.agents/skills/` | `.agents/RULES.md` |
| Windsurf | `.windsurfignore` | `.agents/skills/` | `rules/*.instructions.md` |

## Comandos de Build

```bash
# Restaurar dependências
dotnet restore Eaf.sln

# Build completo
dotnet build Eaf.sln --configuration Release

# Testes com cobertura
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Script de testes (Linux/macOS)
./run-tests-with-coverage.sh

# Angular (Templates)
cd Templates/Angular/Eaf.ProjectName.UI && nvm use 18 && npm install --legacy-peer-deps
npx ng build --configuration=production
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

## Padrões de Código

### DO (Faça)
- Seguir arquitetura em camadas ABP: Domain → Application → Infrastructure → Presentation
- Usar `async/await` para operações I/O
- Adicionar documentação XML a todas as APIs públicas
- Usar BDD em português: `Dado/Quando/Então` (Given/When/Then)
- Manter cobertura ≥ 90%
- Documentação e nomes de teste em português (pt-BR)
- Usar `DependsOn` para declarar dependências entre módulos
- Injetar dependências via construtor (Castle Windsor)

### DON'T (Não Faça)
- Não editar arquivos gerados (`service-proxies.ts`, `*.Designer.cs`)
- Não reduzir cobertura de testes
- Não push direto em `main` ou `develop`
- Não commitar secrets (`.env`, `appsettings.Production.json`)
- Não usar `Any`, `getattr`, hard-coding
- Não modificar `node_modules/`, `bin/`, `obj/`, `nupkg/`, `TestResults/`, `sonar/`

## Hard Rules

1. **Branches protegidas**: `main` e `develop` — merge apenas via PR
2. **Testes obrigatórios**: CI falha se qualquer teste quebrar
3. **Cobertura mínima**: não pode diminuir em relação ao baseline
4. **XML docs**: APIs públicas sem documentação são bloqueadas
5. **Secrets**: nunca commitar `.env`, credentials ou tokens
6. **Workflows imutáveis**: não modificar `.github/workflows/` sem revisão

## Soft Rules

1. Modificar `Dockerfile` → requer confirmação
2. Alterar `common.props` → requer confirmação (afeta todos os projetos)
3. Deletar arquivos de teste → requer justificativa
4. Alterar dependências NuGet globais → confirmar compatibilidade

## Agent Loop

> Padrão: **Plan-and-Execute** (tarefas multi-arquivo)

```
1. Receber tarefa
2. Carregar AGENTS.md + RULES.md (always-on)
3. Carregar skills e rules pattern-matched
4. Apresentar Execution Plan
5. Verificar guardrails
6. Executar (sandbox + permissions)
7. Verification loop: lint → test → CI
8. Validar resultado
9. Ajustar (máx. 2 iterações antes de escalar)
10. Atualizar MEMORY.md
```

## Response Style

- Idioma: Português (pt-BR) para docs/testes; inglês para código
- Formato: conciso, direto, sem preâmbulos
- Referências: usar `arquivo:linha` para navegação
- Commits: mensagens descritivas em inglês (`feat:`, `fix:`, `test:`, `docs:`)

## Referências

- `.agents/CONTEXT.md` — Estratégias de context engineering
- `.agents/RULES.md` — Guardrails detalhados
- `.agents/TOOLS.md` — Ferramentas e MCP
- `.agents/WORKFLOWS.md` — Automação CI/CD
- `.agents/MEMORY.md` — Estado cross-session
- `.agents/skills/` — Skills on-demand (25 disponíveis)
- `rules/` — Rules por domínio com `applyTo`
- `docs/` — Documentação técnica
