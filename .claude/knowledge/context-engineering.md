# Context Engineering — EAF

## Estratégias de Carregamento

| Tipo | Quando | Arquivos |
|------|--------|----------|
| **Always-on** | Sempre carregado | `CLAUDE.md`, `.claude/rules/global-rules.md` |
| **Pattern-matched** | Por tipo de arquivo | `.claude/rules/*.md` (via `paths:` glob) |
| **On-demand** | Quando solicitado | `.claude/skills/`, `docs/`, `.claude/memory/memory.md` |
| **Progressive disclosure** | Codebases grandes | Mapa de dirs → headers → conteúdo |

## Hierarquia de Prioridade

1. Instruções do usuário (chat/prompt)
2. `CLAUDE.md` (SSoT)
3. Arquivo de plataforma (`CLAUDE.md`) — Devin e Windsurf leem `CLAUDE.md` diretamente
4. `.claude/rules/global-rules.md` (guardrails)
5. `.claude/rules/*.md` (pattern-matched)
6. `.claude/skills/` (on-demand)
7. `.claude/memory/memory.md` (cross-session)

## Token Budget

- Reservar 20% do contexto para output
- `CLAUDE.md` ≤ 500 linhas (~2K tokens)
- Skills: carregar apenas as relevantes para a tarefa
- Rules: ativar apenas por glob match

## Chunking

- Arquivos >500 linhas: carregar headers primeiro, depois seções relevantes
- `service-proxies.ts` (16k+ linhas): nunca carregar inteiro — buscar por tipo/método
- Código gerado (`*.Designer.cs`, `*.g.cs`): ignorar

## Context Compaction

Quando o budget é insuficiente:
1. **Snip**: remover seções não relevantes do CLAUDE.md
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
