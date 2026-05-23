# Claude Code Configuration — EAF

@import AGENTS.md

## Delta Claude

### Carregamento Automático
Claude Code carrega este arquivo automaticamente antes de cada sessão. O `@import` acima carrega o AGENTS.md como SSoT.

### Memory
Claude Code mantém memória automática em `.agents/MEMORY.md`. Este arquivo é escrito pelo agente e persiste aprendizados entre sessões.

### Skills
Skills carregadas de `.agents/skills/` sob demanda. Claude Code detecta automaticamente a skill relevante pelo contexto da tarefa.

### Guardrails
- `.agents/RULES.md` — Hard/Soft rules
- `rules/*.instructions.md` — Rules por domínio (ativadas por glob)
- `.claudeignore` — Arquivos a ignorar

### Referências
- `.agents/CONTEXT.md` — Estratégias de contexto
- `.agents/TOOLS.md` — Ferramentas disponíveis
- `.agents/WORKFLOWS.md` — Workflows de automação
