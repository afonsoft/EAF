# Infraestrutura de Agentes — EAF

## Diagrama de Arquivos

```
AGENTS.md                 ← SSoT (≤500 linhas) — router de contexto
CLAUDE.md                 ← Delta para Claude Code (@import AGENTS.md)
DEVIN.md                  ← Delta para Devin
GEMINI.md                 ← Delta para Gemini CLI
.aiignore                 ← Ignore JetBrains AI
.claudeignore             ← Ignore Claude Code
.cursorignore             ← Ignore Cursor
.devinignore              ← Ignore Devin
.geminiignore             ← Ignore Gemini CLI
.windsurfignore           ← Ignore Windsurf
.agents/
├── CONTEXT.md            ← Estratégias de carregamento de contexto
├── RULES.md              ← Guardrails (hard/soft rules)
├── MEMORY.md             ← Estado cross-session (auto-maintained)
├── TOOLS.md              ← Ferramentas, CI/CD, MCP
├── WORKFLOWS.md          ← Workflows de automação
├── README.md             ← Este arquivo
├── .aiignore             ← Ignore base para AI
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
    └── dotnet-design-pattern-review/
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

| Feature | Claude | Devin | Gemini | Cursor | Windsurf | JetBrains |
|---------|--------|-------|--------|--------|----------|-----------|
| AGENTS.md | Sim | Sim | Sim | Sim | Sim | Sim |
| Platform file | CLAUDE.md | DEVIN.md | GEMINI.md | — | — | — |
| Skills (.agents/skills/) | Sim | Sim | Sim | Sim | Sim | Sim |
| Rules (rules/) | Sim | Sim | Sim | Sim | Sim | Sim |
| Ignore file | .claudeignore | .devinignore | .geminiignore | .cursorignore | .windsurfignore | .aiignore |
| Memory | .agents/MEMORY.md | Knowledge | — | — | — | — |

## Referências

- [AGENTS.md Spec](https://agents.md/)
- [Agent Skills Spec](https://agentskills.io/specification)
- [OpenAI Harness Engineering](https://openai.com/index/harness-engineering/)
- [awesome-ai-conventions](https://github.com/GuilhermeAlbert/awesome-ai-conventions)
