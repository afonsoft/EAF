# EAF — Índice Geral de Especificações

> Cada arquivo nesta pasta é uma tarefa independente para execução por sub-agent.
> SOLID/Clean Architecture está no final (80-99) pois pode alterar a estrutura do projeto.
> **Para instruções de orquestração multi-agent, veja [`prompt.md`](prompt.md)**.

## Fase 0 — Pré-requisitos

| # | Arquivo | Complexidade | Descrição | Status |
|---|---------|-------------|-----------|--------|
| 15 | `15-consolidar-dependency-updates.md` | BAIXA | Consolidar 5 PRs Dependabot: EFCore, Sqlite, TestHost, EPPlus | ✅ CONCLUÍDA |

## Fase 1 — Performance dos Módulos `src/` (Crítica)

| # | Arquivo | Complexidade | Descrição |
|---|---------|-------------|-----------|
| 01 | `01-remover-binaryformatter-sqlservercache.md` | ALTA | Remover BinaryFormatter do SqlServerCache (não compila no .NET 10) |
| 02 | `02-remover-binaryformatter-sqlitecache.md` | ALTA | Remover BinaryFormatter do SqliteCache (não compila no .NET 10) |
| 03 | `03-corrigir-sync-over-async-cache.md` | MÉDIA | Corrigir sync-over-async no EafSqlServerCache |
| 04 | `04-batch-delete-auditlog-worker.md` | MÉDIA | Converter loop de delete individual para batch delete |
| 05 | `05-httpclientfactory-auth-providers.md` | MÉDIA | Substituir `new HttpClient()` por IHttpClientFactory em 3 providers |

## Fase 2 — Suporte Multi-Database (Alta Prioridade)

| # | Arquivo | Complexidade | Descrição |
|---|---------|-------------|-----------|
| 06 | `06-multi-db-dbcontext-configurer.md` | MÉDIA | Implementar switch de provider no DbContextConfigurer |
| 07 | `07-multi-db-packages-e-factory.md` | BAIXA | Adicionar NuGet packages e atualizar DbContextFactory |
| 08 | `08-multi-db-dbcontext-e-config.md` | MÉDIA | Corrigir DbContext (warnings, Migrate) e appsettings |

## Fase 3 — Performance Angular Template

| # | Arquivo | Complexidade | Descrição |
|---|---------|-------------|-----------|
| 09 | `09-angular-subscription-cleanup.md` | ALTA | Corrigir 117 subscribe() sem cleanup (memory leaks) |
| 10 | `10-angular-lazy-loading-e-budgets.md` | BAIXA | Corrigir preload e adicionar bundle budgets |
| 11 | `11-angular-onpush-strategy.md` | MÉDIA | Aplicar OnPush em componentes stateless |

## Fase 4 — Performance API Template

| # | Arquivo | Complexidade | Descrição |
|---|---------|-------------|-----------|
| 12 | `12-api-response-compression.md` | BAIXA | Adicionar Brotli/Gzip response compression |
| 13 | `13-api-efcore-asnotracking.md` | BAIXA | Aplicar AsNoTracking em queries read-only |
| 14 | `14-misc-performance-fixes.md` | BAIXA | Correções menores: Task.CompletedTask, [Obsolete], await fix |

## Fase 5 — SOLID / Clean Architecture (Final — Alto Impacto)

| # | Arquivo | Complexidade | Descrição |
|---|---------|-------------|-----------|
| 80 | `80-solid-service-locator-removal.md` | ALTA | Remover Service Locator (IocManager.Instance) de 5+ classes |
| 81 | `81-solid-srp-webcore-module-extract.md` | ALTA | Extrair responsabilidades do MiddlewareWebCoreModule |
| 82 | `82-solid-srp-tokenauth-extract.md` | MUITO ALTA | Extrair TokenAuthController em services menores |
| 83 | `83-solid-isp-worker-interface.md` | MÉDIA | Segregar IEafWorkerBase em interfaces menores |
| 84 | `84-solid-keyvault-factory-extract.md` | MÉDIA | Extrair factory do KeyVaultSecretManager |
| 85 | `85-solid-cache-serializer-interface.md` | MÉDIA | Extrair ICacheSerializer para Open/Closed |
| 86 | `86-solid-log4net-error-handling.md` | BAIXA | Corrigir fire-and-forget e async void no ServiceBusQueueAppender |

## Regras para Sub-Agents

1. **Ordem de execução**: Seguir a numeração (01 → 86). Nunca executar fase 5 antes das fases 1-4.
2. **Testes obrigatórios**: Cada tarefa inclui cenários de teste. O sub-agent DEVE executar os testes.
3. **Complexidade alta**: Se a tarefa for MUITO ALTA, o sub-agent deve:
   - Executar testes a cada mudança incremental
   - Se falhar 3x, reportar complexidade e voltar ao início
4. **Build obrigatório**: Após cada tarefa, executar `dotnet build Eaf.sln --configuration Release`
5. **Testes obrigatórios**: `dotnet test Eaf.sln --collect:"XPlat Code Coverage"`
6. **Não reduzir cobertura**: Cobertura mínima ≥ 90%
7. **Idioma**: Testes em português (pt-BR), BDD: Dado/Quando/Então
8. **XML docs**: Todas as APIs públicas devem ter documentação XML
