# Infraestrutura de Agentes — EAF

## Diagrama de Arquivos

```
AGENTS.md                 ← SSoT (≤500 linhas) — lido por Devin e Windsurf
CLAUDE.md                 ← Delta para Claude Code (@import AGENTS.md)
.claudeignore             ← Ignore Claude Code
.devinignore              ← Ignore Devin
.windsurfignore           ← Ignore Windsurf
.agents/
├── CONTEXT.md            ← Estratégias de carregamento de contexto
├── RULES.md              ← Guardrails (hard/soft rules)
├── MEMORY.md             ← Estado cross-session (auto-maintained)
├── TOOLS.md              ← Ferramentas, CI/CD, MCP
├── WORKFLOWS.md          ← Workflows de automação
├── README.md             ← Este arquivo
└── skills/               ← Skills on-demand (SKILL.md format)
    ├── eaf-api/
    ├── eaf-modules/
    ├── eaf-testing/
    ├── eaf-ui/
    ├── eaf-cicd/
    ├── eaf-code-quality/
    ├── analyzing-dotnet-performance/
    ├── angular-development/
    ├── aspnet-boilerplate-development/
    ├── aspnet-boilerplate-modules/
    ├── dotnet-backend-patterns/
    ├── dotnet-best-practices/
    ├── dotnet-design-pattern-review/
    ├── abp-multi-tenancy/
    ├── abp-microservice/
    ├── security-jwt/
    ├── sql-code-review/
    ├── sql-optimization/
    ├── dotnet-github-actions/
    ├── systematic-debugging/
    ├── test-driven-development/
    ├── verification-before-completion/
    ├── design-patterns/
    ├── writing-plans/
    └── using-git-worktrees/
rules/
├── csharp-eaf.instructions.md        ← Rules C#/EAF (applyTo: **/*.cs)
├── angular-eaf.instructions.md       ← Rules Angular (applyTo: **/*.ts)
└── dotnet-project.instructions.md    ← Rules .csproj (applyTo: **/*.csproj)
```

## Como Skills São Carregadas

Skills usam a **descrição tripartite**:

1. **What**: o que a skill faz
2. **When**: gatilhos e contextos de ativação
3. **Do NOT**: quando NÃO usar

O agente lê o `name` e `description` do frontmatter YAML para decidir se ativa a skill.

## Como Adicionar Nova Skill

1. Criar diretório em `.agents/skills/<nome-kebab-case>/`
2. Criar `SKILL.md` com frontmatter YAML obrigatório:
   ```yaml
   ---
   name: nome-da-skill
   description: >
     What: o que faz.
     When: quando ativar.
     Do NOT: quando NÃO usar.
   ---
   ```
3. Adicionar conteúdo: Contexto, Atuação, Restrições, Exemplos
4. Opcionalmente criar `references/`, `templates/`, `scripts/`
5. Atualizar o `AGENTS.md` na seção de referências

## Compatibilidade por Plataforma

| Feature | Claude Code | Devin | Windsurf |
|---------|-------------|-------|----------|
| AGENTS.md (SSoT) | Sim | Sim | Sim |
| Platform file | CLAUDE.md | AGENTS.md | AGENTS.md |
| Skills (.agents/skills/) | Sim | Sim | Sim |
| Rules (rules/) | Sim | Sim | Sim |
| Ignore file | .claudeignore | .devinignore | .windsurfignore |
| Memory | .agents/MEMORY.md | Knowledge | — |

## Referências

- [AGENTS.md Spec](https://agents.md/)
- [Agent Skills Spec](https://agentskills.io/specification)
- [OpenAI Harness Engineering](https://openai.com/index/harness-engineering/)
- [awesome-ai-conventions](https://github.com/GuilhermeAlbert/awesome-ai-conventions)
