# CLAUDE.md — EAF (Enterprise Application Foundation)

## Mission

EAF é uma plataforma middleware enterprise open-source construída sobre ASP.NET Boilerplate (ABP) para .NET 10.0. Fornece módulos reutilizáveis para identidade, cache, observabilidade, background jobs e secret management. Este arquivo é o ponto de entrada do Claude Code: carregado automaticamente antes de cada sessão.

## Tech Stack

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
| Secrets | Azure Key Vault / OCI Vault | — |
| CI/CD | GitHub Actions (`ubuntu-latest`) | — |
| Qualidade | SonarCloud + coverlet | — |
| Licença | GPL-3.0-or-later | — |

## Project Structure

- `src/` — 14 módulos middleware (`Eaf.Middleware.*`, `Eaf.KeyVault`, `Eaf.OpenTelemetry`, etc.)
- `test/` — 14 projetos de teste espelhando `src/`
- `Templates/` — Api, Angular, Worker, Eaf.Gateways.API
- `docs/` — documentação técnica
- `.claude/` — harness nativo do Claude Code (rules, skills, agents, memory, knowledge)

## Harness Structure

| Componente | Local | Carregamento |
|---|---|---|
| Regras globais | `.claude/rules/global-rules.md` | Always-on |
| Regras de domínio | `.claude/rules/csharp-eaf.md`, `.claude/rules/angular-eaf.md`, `.claude/rules/dotnet-project.md` | Pattern-matched por `paths:` |
| Skills | `.claude/skills/{slug}/SKILL.md` | On-demand por relevância |
| Sub-agentes | `.claude/agents/{review,plan,test}.md` | Via descrição ou Task tool |
| Comandos | `.claude/commands/*.md` | Slash commands (quando existirem) |
| Hooks | `.claude/hooks/*.sh` | Eventos registrados em `.claude/settings.json` |
| Memória curto-prazo | `.claude/memory/memory.md` | Sempre |
| Memória longo-prazo | `.claude/memory/{YYYYMMDD}-memory.md` | 3 arquivos mais recentes |
| Conhecimento | `.claude/knowledge/*.md` | On-demand |
| Configuração | `.claude/settings.json` | Always-on (guardrails computacionais) |

## Context Engineering

### Fontes de contexto

| Fonte | Artefato | Notas |
|---|---|---|
| Instruções | `CLAUDE.md`, `.claude/rules/` | Comportamento always-on |
| Estado / memória curto-prazo | `.claude/memory/memory.md` | Estado da sessão atual |
| Memória longo-prazo | `.claude/memory/{YYYYMMDD}-memory.md` | Decisões e aprendizados |
| Conhecimento recuperado | `.claude/knowledge/`, `docs/` | Domínio sob demanda |
| Integrações externas | MCP servers em `.claude/settings.json` | APIs com timeout/rate limits documentados |
| Saídas estruturadas | `.claude/agents/*.md` | Schemas de sub-agentes |

### Estratégias de carregamento

| Tipo | Quando | Exemplos |
|---|---|---|
| Always-on | Toda sessão | `CLAUDE.md`, `global-rules.md`, `memory.md` |
| Pattern-matched | Pelo tipo de arquivo tocado | `rules/csharp-eaf.md` (`paths: ['**/*.cs']`) |
| On-demand | Quando referenciado | `.claude/knowledge/tools-and-integrations.md`, `docs/` |
| Progressive disclosure | Codebases grandes | Mapa de dirs → headers → conteúdo |

### Hierarquia de prioridade

1. `CLAUDE.md` e `global-rules.md` — não-negociáveis.
2. `.claude/memory/memory.md` — estado atual.
3. Regras que casam com arquivos sendo modificados.
4. Skills relevantes ao pedido.
5. Knowledge e `docs/` explicitamente referenciados.
6. Memória longo-prazo — somente os 3 arquivos mais recentes.

### Orçamento de tokens

- Reservar **20% da janela para output**.
- `CLAUDE.md` ≤ 1000 linhas.
- `memory.md` ≤ 100 linhas.
- Tudo além do always-on é on-demand.

### Chunking

- Arquivos > 500 linhas: carregar cabeçalho e índice primeiro; depois apenas as seções necessárias.
- `service-proxies.ts` (16k+ linhas): nunca carregar inteiro — buscar por tipo/método.
- Código gerado (`*.Designer.cs`, `*.g.cs`): ignorar.

### Compaction ladder

Aplicar em ordem, escalando só quando o anterior não bastar:

1. **Budget reduction** — descartar conteúdo on-demand não mais relevante.
2. **Snip** — cortar saídas verbosas para as linhas de resumo.
3. **Microcompact** — resumir sub-tarefas concluídas em uma linha cada.
4. **Collapse** — substituir um grupo finalizado pela entrada de checkpoint.
5. **Auto-compact** — último recurso; **sempre escrever memória antes**.

### Tiers de memória

| Tier | Persistência | Conteúdo | Onde vive |
|---|---|---|---|
| Procedural | Sempre carregado | Como trabalhar | `CLAUDE.md`, `.claude/rules/` |
| Semântica | On demand | Fatos, padrões | `.claude/knowledge/`, `docs/` |
| Episódica | Cross-session | Experiências, decisões | `.claude/memory/` |

### Governança e controles de risco

| Risco | Controle | Onde vive |
|---|---|---|
| Exposição de dados | Restrições de leitura em secrets e paths sensíveis | `.claude/settings.json` `permissions.deny` |
| Alucinação | Respostas baseadas em fontes do repositório; toda entrada de knowledge cita origem | `.claude/knowledge/`, `docs/`, regra "sem contexto inventado" |
| Contexto obsoleto | Verificar just-in-time a memória contra o código atual; código vence | Memory protocol |
| Decisões não rastreáveis | Log append-only com racional e alternativas descartadas | `.claude/memory/{YYYYMMDD}-memory.md` |
| Ferramentas descontroladas | Permissões classificadas por risco: allow, ask, deny | `.claude/settings.json` |
| Chamadas externas ilimitadas | Headers, timeouts e rate limits documentados | Declaração MCP + `CLAUDE.md`/`knowledge` |

## Memory Protocol

### Leitura

No início de cada sessão:

1. Ler `.claude/memory/memory.md`.
2. Listar `.claude/memory/[0-9]*-memory.md`, ordenar decrescente, ler os 3 primeiros.
3. Tratar memória longo-prazo como **hint, não verdade absoluta**; verificar fatos contra o código antes de agir.

### Escrita

| Gatilho | Escrever em | O |
|---|---|---|
| Sessão inicia | — | Apenas leitura |
| Checkpoint verificado / commit | Ambos | Atualizar `memory.md`; adicionar `## Checkpoints` no arquivo do dia |
| Decisão tomada | Longo-prazo | `## Decisions` com racional e alternativas descartadas |
| Erro corrigido | Longo-prazo | `## Lessons learned` |
| Problema fora de escopo encontrado | Curto-prazo | Adicionar a blockers; não corrigir agora |
| Antes de compaction / reset | Ambos | Promover entradas duráveis, depois resetar `memory.md` |

### Promoção e rotação

- Quando `memory.md` exceder 100 linhas ou um checkpoint for commitado:
  1. Mover entradas duráveis para `.claude/memory/{YYYYMMDD}-memory.md`.
  2. Reescrever `memory.md` do template limpo, mantendo só o estado atual.
- Arquivos longo-prazo são append-only. Corrigir fatos obsoletos com nova entrada prefixada por `SUPERSEDED:`.

### Segurança

- Zero secrets, tokens, passwords, connection strings, private keys.
- Zero PII (nomes de clientes, documentos, cartões, contas).
- Referenciar identificadores, nunca os valores.

## Code Standards

### DO

- Seguir arquitetura em camadas ABP: Domain → Application → Infrastructure → Presentation.
- Usar `async/await` para operações I/O; sufixo `Async` em métodos assíncronos.
- Adicionar documentação XML (`///`) a todas as APIs públicas.
- Usar BDD em português: `Dado_Quando_Entao` ou `[Fact] // Dado X, Quando Y, Então Z`.
- Manter cobertura ≥ 90% e não diminuir o baseline.
- Documentação e nomes de teste em português (pt-BR).
- Declarar dependências de módulos com `[DependsOn]`.
- Injetar dependências via construtor (Castle Windsor).
- Tratar warnings como erros em Release.

### DON'T

- Não editar arquivos gerados (`service-proxies.ts`, `*.Designer.cs`, `*.g.cs`, `*.g.i.cs`).
- Não reduzir cobertura de testes.
- Não fazer push direto em `main` ou `develop`.
- Não commitar secrets (`.env`, `appsettings.Production.json`, tokens).
- Não usar `Any`, `getattr`, hard-coding ou `new` para serviços.
- Não modificar `node_modules/`, `bin/`, `obj/`, `nupkg/`, `TestResults/`, `sonar/`.
- Não usar `.Result` ou `.Wait()` em código assíncrono.

## Hard Rules

1. Branches `main` e `develop` são protegidas — merge apenas via PR.
2. Todos os testes devem passar antes do merge.
3. Cobertura de código não pode diminuir.
4. APIs públicas devem ter documentação XML.
5. Nunca commitar secrets, tokens ou connection strings.
6. Não modificar `.github/workflows/` sem revisão humana.
7. Não editar arquivos gerados.
8. Não usar `--no-verify` ou `--force` sem aprovação.
9. Não reduzir o número de testes existentes.

## Soft Rules

1. Modificar `Dockerfile` ou `docker-compose.yml` → confirmar com usuário.
2. Alterar `common.props` → confirmar compatibilidade.
3. Deletar arquivos de teste → exigir justificativa.
4. Alterar dependências NuGet globais → verificar breaking changes.
5. Modificar `appveyor.yml` ou CI legado → confirmar necessidade.
6. Alterar schema/migrations → verificar rollback.
7. Adicionar novo módulo middleware → seguir padrão ABP.

## Agent Loop

**Padrão: Plan-and-Execute** (tarefas multi-arquivo).

1. Receber tarefa.
2. Carregar `CLAUDE.md` e `global-rules.md`.
3. Ler `memory.md` e os 3 arquivos longo-prazo mais recentes.
4. Carregar regras e skills que casam com a tarefa.
5. Apresentar Execution Plan e aguardar aprovação.
6. Verificar guardrails (`settings.json`) e hooks.
7. Executar dentro das permissões.
8. Verification loop: `lint → test → CI`.
9. Ajustar — no máximo 2 iterações antes de escalar.
10. Atualizar memória e commitar o checkpoint.

## Response Style

- Idioma: Português (pt-BR) para documentação, testes e comentários; Inglês para código e commits.
- Formato: conciso, direto, sem preâmbulos.
- Referências: usar `arquivo:linha` ou tags `<ref_file />`/`<ref_snippet />` em mensagens ao usuário.
- Commits: mensagens descritivas em inglês, Conventional Commits (`feat:`, `fix:`, `test:`, `docs:`, `ci:`).

## Workflows

- Feature: branch `feature/<descricao>` ou `feature/{AgentLLM}-{YYYYMMDD}-{descricao}` → implementar → testes → PR para `develop`.
- Bug fix: `bug/<descricao>` ou `hotfix/<descricao>` → reproduzir → corrigir → teste de regressão → PR.
- Release: `release/<versao>` → validar CI → merge para `main` → tag `v<versao>` → `publish-all.yml`.
- Detalhes completos em `.claude/knowledge/workflows.md`.

## Tools & Integrations

- Build: `dotnet restore Eaf.sln`, `dotnet build Eaf.sln --configuration Release`.
- Testes: `dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings` ou `./run-tests-with-coverage.sh`.
- Angular: `nvm use 18 && npm install --legacy-peer-deps && npx ng build --configuration=production && npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox`.
- Qualidade: SonarCloud (`./sonarcloud.sh`), Qodana, Snyk via workflows.
- Publicação: `publish-all.yml` (NuGet + API/UI deploy).
- Detalhes completos em `.claude/knowledge/tools-and-integrations.md`.

## References

- `.claude/rules/global-rules.md` — guardrails always-on
- `.claude/rules/csharp-eaf.md` — padrões C# (pattern-matched)
- `.claude/rules/angular-eaf.md` — padrões Angular (pattern-matched)
- `.claude/rules/dotnet-project.md` — configuração de projetos .NET (pattern-matched)
- `.claude/skills/` — skills on-demand
- `.claude/agents/review.md` — sub-agente de revisão
- `.claude/agents/plan.md` — sub-agente de planejamento
- `.claude/agents/test.md` — sub-agente de testes
- `.claude/knowledge/context-engineering.md` — estratégias de contexto
- `.claude/knowledge/tools-and-integrations.md` — ferramentas e integrações
- `.claude/knowledge/workflows.md` — workflows de automação
- `.claude/memory/memory.md` — estado da sessão
- `.claude/memory/20260731-memory.md` — memória longo-prazo
