# Revisão — Processos, Gerenciamento de Memória e Performance

Data: 2026-11-09 · Branch: main (pós-merge até #470) · Método: varredura estática direcionada (triage → evidência → veredito)

## Resumo executivo

EAF está estruturalmente saudável: caches dominados por TTL, poucos sync-over-async, sem concatenação de strings em loops, sem Span/Memory necessários. Os achados concentram-se em **5 pontos de memória** (dicionários não limitados e singletons segurando repositórios) e **4 de performance** (sync-over-async no hot path JWT, queries sem tracking-control e paginação), além de **3 gaps de processo**.

## Memória

| # | Severidade | Local | Achado | Recomendação |
|---|-----------|-------|--------|--------------|
| M1 | **Alta** | `src/Eaf.RateLimiting/RateLimiting/InMemoryRateLimitingStore.cs:21` | `_buckets` (ConcurrentDictionary) nunca remove buckets expirados — vazamento proporcional a IPs/chaves únicas | Sweep periódico no `AddOrUpdate` (a cada N hits remover `ResetsAt < now`) ou `MemoryCache` com `AbsoluteExpirationRelativeToNow` |
| M2 | Média | `src/Eaf.Middleware.Core/Friendships/Cache/UserFriendsCache.cs:19,48-50` | `ISingletonDependency` injetando 3 `IRepository<>` — repositórios são scoped por UoW | Trocar para `ITransientDependency` ou resolver via `IIocResolver` dentro do método sob `IUnitOfWork` |
| M3 | Média | `SubscriptionExpirationCheckWorker`, `ExpiredAuditLogDeleterWorker`, `WorkflowSlaWorker` | Workers singletons injetam `IRepository<>` no ctor | Convenção ABP: resolver dentro de `DoWork` sob o UoW aberto (funciona hoje via proxy do Windsor, mas retém referências longas) |
| M4 | Baixa | `src/Eaf.Middleware.Core/Friendships/Cache/UserFriendsCache.cs:298` | `cache.Set` sem TTL explícito (depende do default do cache nomeado) | Definir `AbsoluteExpireTime` no `FriendCacheItem` (ex.: 30 min) |
| M5 | Baixa | `AppConfigurations.ConfigurationCache` (Core + Worker) | `ConcurrentDictionary` estático — limitado pelo nº de tenants/arquivos, mas sem expiração | Aceitável; documentar bound ou adicionar TTL |

Já corretos: `SessionRegistry` (TTL em sessions/index/revoked-30d), `QrLoginManager` (absoluteExpireTime), `TempFileCacheManager` (5 min), `RateLimitManager._windowLocks` (TryRemove de locks ociosos).

## Performance

| # | Severidade | Local | Achado | Recomendação |
|---|-----------|-------|--------|--------------|
| P1 | **Alta** | `MiddlewareJwtSecurityTokenHandler.cs:69,104,154,157` | 4× `AsyncHelper.RunSync` por requisição autenticada (session revoke, user lookup, touch) — bloqueia thread pool no hot path JWT | Refatorar o validador para async (`ISecurityTokenValidator`/`OnTokenValidated` async callback) ou fazer as chamadas de cache síncronas |
| P2 | Média | `src/Eaf.Middleware.Application` | 59 ocorrências de `GetAllListAsync`/`ToList` — risco de result sets sem paginação | Auditoria dos endpoints de listagem: garantir `PagedResultDto`/`MaxResultCount` em todos |
| P3 | Média | EF Core read paths | `AsNoTracking` em apenas 12 pontos — consultas read-only rastreiam entidades (alocação + snapshot) | Aplicar `AsNoTracking()` nas queries de listagem/lookup (repositórios ABP têm `GetAll().AsNoTracking()`) |
| P4 | Baixa | `ServiceBusQueueAppender.cs:80,99` | `Task.Run(...).GetAwaiter().GetResult()` no caminho de logging | `AsyncHelper.RunSync` direto ou fila bounded com flush assíncrono |
| P5 | Baixa | `SubscriptionExpirationCheckWorker.ApplyPendingDowngradesAsync` | `FirstOrDefaultAsync` por pagamento (N+1) | Único `GetAllListAsync` de tenants afetados ou join — volume é baixo, prioridade baixa |

Ok: `Task.Run` paralelo limitado no LDAP (3 buscas concorrentes), workers com `Timer.RunOnStart` configurado, sem `+=` em strings dentro de loops.

## Processos

| # | Gap | Recomendação |
|---|-----|--------------|
| G1 | `dotnet ef` quebra nos csprojs do repo (MSB4057) — migrations escritas à mão (propensa a erro) | Criar `tools/Eaf.Migrator` ou ajustar props para o differ funcionar; ou documentar o fluxo manual como padrão |
| G2 | Cobertura medida 32,5% vs regra CLAUDE.md "≥90%" | Ajustar a regra ao baseline real (ex.: "não diminuir; +X% por módulo novo") ou investir em ramp-up |
| G3 | Specs `.specs/` divergem do código (vários Draft já implementados) | Check no CI/orchestrator: spec `Completed` exige evidência; ou workflow de reconciliação periódica |
| G4 | `Eaf.ProjectName.sln` só compila módulos novos após publish no NuGet | Já mitigado com `EAF_USE_SOURCE_PROJECTS`; manter CI rodando o caminho ApiWithSrc |
| G5 | Dependabot ativo; sem `dotnet format`/editorconfig gate | Adicionar step de format-check no `ci-build-test.yml` |

## Priorização sugerida

1. **M1 + P1** — fix de 1 dia cada, ganho direto em produção (leak + thread pool).
2. **P3** — sweep mecânico `AsNoTracking` nas listagens (baixo risco).
3. **M2/M3** — re-padrão dos singletons com repositórios.
4. **G1/G3** — processo.
