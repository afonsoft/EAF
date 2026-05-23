# Gemini CLI Configuration — EAF

> Referência principal: [AGENTS.md](AGENTS.md)

## Delta Gemini

### Contexto
Gemini CLI carrega este arquivo automaticamente. O AGENTS.md contém todas as convenções do projeto. Este arquivo contém apenas delta específico para Gemini.

### Build
```bash
dotnet restore Eaf.sln
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Convenções
- Documentação em português (pt-BR)
- Testes BDD: Dado/Quando/Então
- XML docs obrigatórios em APIs públicas
- Cobertura mínima: 90%

### Skills
Disponíveis em `.agents/skills/` — carregar conforme contexto da tarefa.

### Referências
- `.agents/RULES.md` — guardrails
- `.agents/TOOLS.md` — ferramentas disponíveis
- `.agents/WORKFLOWS.md` — automação
