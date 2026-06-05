# EAF Specs — Prompt de Orquestração Multi-Agent

> **Padrão**: Harness Engineering (OpenAI) — Plan-and-Execute com sub-agents paralelos.
> **Objetivo**: Executar as 21 especificações desta pasta usando delegação multi-agent.
> **Referência**: [agents-skills](https://github.com/afonsoft/agents-skills) | [AGENTS.md](/AGENTS.md) | [WORKFLOWS.md](/.agents/WORKFLOWS.md)

---

## Arquitetura de Agentes

```
┌─────────────────────────────────────────────────────┐
│                 ORCHESTRATOR AGENT                   │
│   (Lê prompt.md, delega tarefas, coleta resultados) │
│                                                     │
│   Responsabilidades:                                │
│   • Criar branch develop → feature/eaf-specs-impl   │
│   • Delegar specs para sub-agents por fase          │
│   • Aguardar resultado de cada fase                 │
│   • Executar verification loop global               │
│   • Criar PR final para develop                     │
└────────┬──────────┬──────────┬──────────┬───────────┘
         │          │          │          │
    ┌────▼───┐ ┌───▼────┐ ┌──▼─────┐ ┌──▼─────┐
    │Agent-1 │ │Agent-2 │ │Agent-3 │ │Agent-4 │
    │Backend │ │Multi-DB│ │Angular │ │SOLID   │
    │Perf    │ │Support │ │Perf    │ │Refactor│
    │        │ │        │ │        │ │        │
    │01-05   │ │06-08   │ │09-11   │ │80-86   │
    │+ 12-14 │ │        │ │        │ │        │
    └────────┘ └────────┘ └────────┘ └────────┘
```

---

## Agents e Delegação

### Agent-Orchestrator (Orchestrator)

**Papel**: Coordena a execução de todas as fases. Não implementa código.

**Prompt do Orchestrator**:
```
Você é o orchestrator do projeto EAF. Sua missão é executar as especificações
em .specs/ na ordem correta, delegando para sub-agents especializados.

REGRAS:
1. Leia .specs/00-indice-geral.md para entender o plano completo
2. Crie uma branch feature/eaf-improvements a partir de develop
3. Execute as fases na ordem: 1 → 2 → 3 → 4 → 5
4. Fases 1-3 podem rodar em PARALELO (agents independentes)
5. Fase 4 pode rodar em PARALELO com 1-3
6. Fase 5 (SOLID) deve rodar SEQUENCIAL e APÓS fases 1-4
7. Após cada fase, execute o verification loop:
   dotnet build Eaf.sln --configuration Release
   dotnet test Eaf.sln --collect:"XPlat Code Coverage"
8. Se um sub-agent reportar complexidade > 3 falhas, PARE e registre
9. Ao final, crie PR para develop com resumo de todas as mudanças
10. Idioma: PT-BR para docs/testes, EN para código
```

---

### Agent-1: Backend Performance (Fases 1 + 4)

**Specs atribuídas**: `01`, `02`, `03`, `04`, `05`, `12`, `13`, `14`

**Contexto a carregar**:
- `.specs/01-remover-binaryformatter-sqlservercache.md`
- `.specs/02-remover-binaryformatter-sqlitecache.md`
- `.specs/03-corrigir-sync-over-async-cache.md`
- `.specs/04-batch-delete-auditlog-worker.md`
- `.specs/05-httpclientfactory-auth-providers.md`
- `.specs/12-api-response-compression.md`
- `.specs/13-api-efcore-asnotracking.md`
- `.specs/14-misc-performance-fixes.md`

**Skills a ativar**:
- `eaf-modules` — Padrões de módulos EAF
- `eaf-testing` — Padrões de teste EAF
- `analyzing-dotnet-performance` — Anti-patterns de performance
- `dotnet-best-practices` — Boas práticas .NET

**Prompt do Agent-1**:
```
Você é o sub-agent de performance backend do EAF.
Sua missão é executar as specs 01-05 e 12-14 em ordem.

REGRAS:
1. Leia cada spec COMPLETAMENTE antes de implementar
2. Execute na ordem: 01 → 02 → 03 → 04 → 05 → 12 → 13 → 14
3. Após cada spec:
   - Execute os Comandos de Verificação listados na spec
   - Execute: dotnet build Eaf.sln --configuration Release
   - Execute: dotnet test Eaf.sln --collect:"XPlat Code Coverage"
   - Faça commit com mensagem descritiva (feat:, fix:, perf:)
4. Se falhar 3x na mesma spec, PARE e reporte:
   - Qual spec falhou
   - Qual erro ocorreu
   - O que já foi feito
5. NUNCA reduza cobertura de testes
6. Testes em português: Dado/Quando/Então
7. XML docs em todas as APIs públicas novas
8. NÃO modifique specs das fases 2, 3 ou 5
```

**Ordem de execução**: SEQUENCIAL (01 → 02 dependem do mesmo padrão BinaryFormatter)

**Paralelismo interno**: 01+02 podem rodar em paralelo (módulos diferentes). 03-05, 12-14 são independentes.

---

### Agent-2: Multi-Database Support (Fase 2)

**Specs atribuídas**: `06`, `07`, `08`

**Contexto a carregar**:
- `.specs/06-multi-db-dbcontext-configurer.md`
- `.specs/07-multi-db-packages-e-factory.md`
- `.specs/08-multi-db-dbcontext-e-config.md`

**Skills a ativar**:
- `eaf-api` — Padrões da API EAF
- `efcore-patterns` — Entity Framework Core
- `aspnet-boilerplate-development` — Padrões ABP

**Prompt do Agent-2**:
```
Você é o sub-agent de Multi-Database do EAF.
Sua missão é adicionar suporte a SQL Server, PostgreSQL e MySQL no template API.

REGRAS:
1. Leia cada spec COMPLETAMENTE antes de implementar
2. Execute na ordem ESTRITA: 06 → 07 → 08 (dependências entre specs)
3. Spec 06 implementa o switch — spec 07 adiciona packages — spec 08 corrige DbContext
4. Após cada spec:
   - Execute os Comandos de Verificação listados na spec
   - Execute: dotnet build Eaf.sln --configuration Release
5. IMPORTANTE: Verificar se Pomelo.EntityFrameworkCore.MySql v10.0.0
   está estável — se não, usar v9.0.x
6. Verificar se Npgsql.EntityFrameworkCore.PostgreSQL v10.0.1 funciona
7. Se falhar 3x, PARE e reporte
8. NÃO modifique arquivos fora de Templates/Api/
```

**Ordem de execução**: SEQUENCIAL ESTRITA (06 → 07 → 08)

---

### Agent-3: Angular Performance (Fase 3)

**Specs atribuídas**: `09`, `10`, `11`

**Contexto a carregar**:
- `.specs/09-angular-subscription-cleanup.md`
- `.specs/10-angular-lazy-loading-e-budgets.md`
- `.specs/11-angular-onpush-strategy.md`

**Skills a ativar**:
- `eaf-ui` — Padrões UI EAF
- `angular-development` — Desenvolvimento Angular

**Prompt do Agent-3**:
```
Você é o sub-agent de Angular Performance do EAF.
Sua missão é otimizar o template Angular em Templates/Angular/.

REGRAS:
1. Leia cada spec COMPLETAMENTE antes de implementar
2. Execute na ordem: 09 → 10 → 11
3. Spec 09 (subscriptions) DEVE ser feita antes de 11 (OnPush)
4. Após cada spec:
   - Execute: npx ng build --configuration=production
   - Execute: npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
   - Faça commit
5. Spec 09 tem 117 arquivos — trabalhar em batches de 20
6. Spec 11 — se OnPush quebrar um componente, REVERTER e marcar como SKIP
7. Se falhar 3x, PARE e reporte
8. NÃO modifique arquivos fora de Templates/Angular/
9. NÃO modifique service-proxies.ts (gerado automaticamente)
```

**Ordem de execução**: SEQUENCIAL (09 → 10 → 11; 09 é pré-requisito de 11)

---

### Agent-4: SOLID / Clean Architecture (Fase 5)

**Specs atribuídas**: `80`, `81`, `82`, `83`, `84`, `85`, `86`

**Contexto a carregar**:
- `.specs/80-solid-service-locator-removal.md`
- `.specs/81-solid-srp-webcore-module-extract.md`
- `.specs/82-solid-srp-tokenauth-extract.md`
- `.specs/83-solid-isp-worker-interface.md`
- `.specs/84-solid-keyvault-factory-extract.md`
- `.specs/85-solid-cache-serializer-interface.md`
- `.specs/86-solid-log4net-error-handling.md`

**Skills a ativar**:
- `eaf-modules` — Padrões de módulos EAF
- `dotnet-design-pattern-review` — Design patterns .NET
- `dotnet-best-practices` — Boas práticas .NET
- `eaf-testing` — Padrões de teste EAF

**Prompt do Agent-4**:
```
Você é o sub-agent de SOLID/Clean Architecture do EAF.
Sua missão é refatorar os módulos middleware para seguir SOLID.

⚠️ ATENÇÃO: Esta fase é a mais CRÍTICA — altera a estrutura do projeto.
SOMENTE execute APÓS as fases 1-4 estarem completas e validadas.

REGRAS:
1. Leia cada spec COMPLETAMENTE antes de implementar
2. Execute na ordem ESTRITA: 86 → 83 → 84 → 85 → 80 → 81 → 82
   (das menos para as mais arriscadas)
3. Após CADA spec:
   - Execute: dotnet build Eaf.sln --configuration Release
   - Execute: dotnet test Eaf.sln --collect:"XPlat Code Coverage"
   - Faça commit IMEDIATAMENTE
4. Se spec 82 (TokenAuth) falhar 3x → PARE e reporte
5. Se spec 81 (WebCoreModule) quebrar startup → REVERTER e reportar
6. NUNCA refatorar tudo de uma vez — mudanças INCREMENTAIS
7. Se cobertura diminuir > 1%, PARE e adicione testes
8. Spec 85 depende de specs 01/02 — verificar se foram executadas
9. Testes em português: Dado/Quando/Então
10. XML docs em TODAS as APIs públicas novas/modificadas
```

**Ordem de execução**: SEQUENCIAL ESTRITA (menos arriscada → mais arriscada)

**Ordem recomendada**:
1. `86` (Log4Net — isolado, baixo risco)
2. `83` (ISP Worker — interface isolada)
3. `84` (KeyVault Factory — módulo isolado)
4. `85` (Cache Serializer — depende de 01/02)
5. `80` (Service Locator — afeta múltiplos módulos)
6. `81` (WebCoreModule — módulo central)
7. `82` (TokenAuth — controller crítico, maior risco)

---

## Fluxo de Execução

```
Orchestrator
│
├─ Fase 0: Setup
│  └─ git checkout -b feature/eaf-improvements develop
│
├─ Fase 1-4: Paralelo
│  ├─ Agent-1: 01→02→03→04→05→12→13→14  (Backend Perf)
│  ├─ Agent-2: 06→07→08                   (Multi-DB)
│  └─ Agent-3: 09→10→11                   (Angular Perf)
│
├─ Merge Point: Aguardar todos os agents concluírem
│  └─ Verification Loop Global:
│     dotnet build Eaf.sln --configuration Release
│     dotnet test Eaf.sln --collect:"XPlat Code Coverage"
│
├─ Fase 5: Sequencial
│  └─ Agent-4: 86→83→84→85→80→81→82       (SOLID)
│
├─ Verification Loop Final:
│  ├─ dotnet build Eaf.sln --configuration Release
│  ├─ dotnet test Eaf.sln --collect:"XPlat Code Coverage"
│  └─ cd Templates/Angular/Eaf.ProjectName.UI && npx ng build --configuration=production
│
└─ Entrega:
   └─ git push && criar PR para develop
```

---

## Protocolo de Complexidade

Quando um sub-agent encontrar dificuldade:

### Nível 1 — Retry (até 3x)
```
1. Falha no build/test
2. Analisar erro
3. Corrigir
4. Re-executar verification loop
5. Se sucesso → continuar
```

### Nível 2 — Reportar (após 3 falhas)
```
1. PARAR imediatamente
2. Fazer commit do estado atual (WIP)
3. Criar arquivo de relatório:
   .specs/reports/<spec-number>-complexity-report.md
4. Conteúdo do relatório:
   - Spec: <número>
   - Tentativas: 3
   - Erros encontrados: <lista>
   - Estado atual: <o que foi feito>
   - Sugestão: <como continuar>
5. Notificar Orchestrator
6. Orchestrator decide: continuar com próxima spec OU escalar para humano
```

### Nível 3 — Escalar para Humano
```
1. Orchestrator coleta todos os relatórios de complexidade
2. Cria resumo em .specs/reports/escalation-summary.md
3. Cria PR parcial com o progresso feito
4. Notifica o humano com:
   - O que foi concluído
   - O que falhou
   - Sugestões de como continuar
```

---

## Verificação Global (Orchestrator)

```bash
# 1. Build completo
dotnet build Eaf.sln --configuration Release

# 2. Testes com cobertura
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# 3. Angular build (se fase 3 foi executada)
cd Templates/Angular/Eaf.ProjectName.UI
npx ng build --configuration=production

# 4. Angular testes (se fase 3 foi executada)
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox

# 5. Verificar cobertura mínima
# Cobertura deve ser ≥ 90%

# 6. Verificar XML docs
# Nenhuma API pública sem documentação
```

---

## Formato de Commit

```
<tipo>(<escopo>): <descrição curta>

[corpo opcional com detalhes]

Spec: <número da spec>
```

**Tipos**: `feat:` | `fix:` | `perf:` | `refactor:` | `test:` | `docs:`

**Exemplos**:
```
perf(SqlServerCache): replace BinaryFormatter with System.Text.Json

Remove deprecated BinaryFormatter serialization and replace with
System.Text.Json for .NET 10 compatibility.

Spec: 01

feat(EntityFrameworkCore): add PostgreSQL and MySQL provider support

Implement database provider switch in DbContextConfigurer supporting
SqlServer, PostgreSQL (Npgsql) and MySQL (Pomelo).

Spec: 06

refactor(TokenAuthController): extract IAuthenticationService

Extract authentication logic into dedicated service to comply with SRP.
Reduces controller from 1215 to ~300 lines.

Spec: 82
```

---

## Critérios de Aceite Global

| # | Critério | Verificação |
|---|----------|------------|
| 1 | Build compila sem erros | `dotnet build Eaf.sln --configuration Release` |
| 2 | Todos os testes passam | `dotnet test Eaf.sln` |
| 3 | Cobertura ≥ 90% | coverlet reports |
| 4 | Angular build passa | `npx ng build --configuration=production` |
| 5 | Zero BinaryFormatter | `grep -rn "BinaryFormatter" src/` retorna 0 |
| 6 | Zero `new HttpClient()` | `grep -rn "new HttpClient()" src/` retorna 0 |
| 7 | Multi-DB funcional | Build com Npgsql + Pomelo packages |
| 8 | APIs documentadas | XML docs em todas as APIs públicas |
| 9 | Commits descritivos | Padrão tipo(escopo): descrição |
| 10 | PR criado para develop | Com resumo de todas as mudanças |

---

## Quick Start

```bash
# 1. Clone e setup
git clone https://github.com/afonsoft/EAF.git
cd EAF
git checkout -b feature/eaf-improvements develop

# 2. Leia o índice
cat .specs/00-indice-geral.md

# 3. Leia este prompt
cat .specs/prompt.md

# 4. Execute specs na ordem:
#    - Paralelo: Agent-1 (01-05, 12-14) | Agent-2 (06-08) | Agent-3 (09-11)
#    - Sequencial: Agent-4 (86, 83, 84, 85, 80, 81, 82)

# 5. Verification loop final
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage"
```
