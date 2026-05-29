# Context Engineering — EAF

## Estratégias de Carregamento

| Tipo | Quando | Arquivos |
|------|--------|----------|
| **Always-on** | Sempre carregado | `AGENTS.md`, `.agents/RULES.md` |
| **Pattern-matched** | Por tipo de arquivo | `rules/*.instructions.md` (via `applyTo` glob) |
| **On-demand** | Quando solicitado | `.agents/skills/`, `docs/`, `.agents/MEMORY.md` |
| **Progressive disclosure** | Codebases grandes | Mapa de dirs → headers → conteúdo |

## Hierarquia de Prioridade

1. Instruções do usuário (chat/prompt)
2. `AGENTS.md` (SSoT)
3. Arquivo de plataforma (`CLAUDE.md`) — Devin e Windsurf leem `AGENTS.md` diretamente
4. `.agents/RULES.md` (guardrails)
5. `rules/*.instructions.md` (pattern-matched)
6. `.agents/skills/` (on-demand)
7. `.agents/MEMORY.md` (cross-session)

## Token Budget

- Reservar 20% do contexto para output
- `AGENTS.md` ≤ 500 linhas (~2K tokens)
- Skills: carregar apenas as relevantes para a tarefa
- Rules: ativar apenas por glob match

## Chunking

- Arquivos >500 linhas: carregar headers primeiro, depois seções relevantes
- `service-proxies.ts` (16k+ linhas): nunca carregar inteiro — buscar por tipo/método
- Código gerado (`*.Designer.cs`, `*.g.cs`): ignorar

## Context Compaction

Quando o budget é insuficiente:
1. **Snip**: remover seções não relevantes do AGENTS.md
2. **Microcompact**: resumir skills em uma linha cada
3. **Collapse**: manter apenas Hard Rules + Build Commands
4. **Auto-compact**: permitir que o modelo decida o que manter

## Mapa de Diretórios

```
Prioridade alta (sempre indexar):
  src/Eaf.Middleware.Core/
  src/Eaf.Middleware.Application/
  test/

Prioridade média (indexar sob demanda):
  src/Eaf.KeyVault/
  src/Eaf.OpenTelemetry/
  Templates/Api/

Prioridade baixa (ignorar por padrão):
  Templates/Angular/Eaf.ProjectName.UI/node_modules/
  nupkg/
  sonar/
  docs/_site/
```
