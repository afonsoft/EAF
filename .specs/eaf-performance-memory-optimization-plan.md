# EAF — Performance and Memory Optimization Plan

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Performance and Memory Optimization |
| Product / System | EAF Middleware + Angular Template |
| Module / Bounded Context | Cross-cutting |
| Change type | Refactor / Optimization |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-perf-optimization` |
| Technical owner | Core Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

The Angular template ships a large initial bundle with jQuery, `ngx-bootstrap`, legacy Metronic assets, and a giant `service-proxies.ts`. The backend lacks distributed cache and explicit query optimization guidance, which can lead to memory pressure and slow reads under load.

### Objective

Establish measurable performance targets and a concrete roadmap to reduce memory usage, improve API latency, and shrink the Angular bundle.

### Expected outcome

- Angular initial bundle < 1 MB gzipped.
- API read p99 < 200 ms.
- No reduction in test coverage.

### Out of scope

- Complete UI redesign.
- Removing `service-proxies.ts` generation (NSwag) without a replacement.

## 2. Agent Role

Senior performance engineer / .NET + Angular. Measure before optimizing, keep changes safe, and add tests.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

EAF serves multi-tenant enterprise apps. Both backend throughput and frontend load time matter.

### Relevant stack

- .NET 10, EF Core 10, ABP 10.5, Castle Windsor
- Angular 20, PrimeNG 17, `ngx-bootstrap`, jQuery, Metronic
- OpenTelemetry, Serilog, coverlet

### Relevant files or directories

```text
/src/Eaf.Middleware.Application
/src/Eaf.Middleware.Web.Core
/Templates/Angular/Eaf.ProjectName.UI
/Templates/Angular/Eaf.ProjectName.UI/angular.json
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
- `.specs/eaf-backend-modularization.spec.md`

## 5. Task Definition

### Main task

Implement backend and frontend optimizations to meet performance targets.

### Subtasks

- Audit top API queries for N+1 and missing indexes.
- Introduce `Eaf.RedisCache` for distributed caching.
- Reduce Angular bundle size and enable lazy loading.
- Add instrumentation and budgets.

### Do not do

- Do not hand-edit `service-proxies.ts`.
- Do not remove `ngx-bootstrap` without replacements.
- Do not reduce test coverage.

## 6. Functional Requirements

### FR-001: Backend query optimization

**Description:** Remove `ToList()` before filtering, use `AsNoTracking()` for reads, and add composite indexes.

**Acceptance criteria:**

- [ ] Top 10 read endpoints audited.
- [ ] No `.Result` or `.Wait()` in async paths.

### FR-002: Distributed cache

**Description:** `Eaf.RedisCache` must cache user permissions and tenant settings with invalidation on entity changes.

**Acceptance criteria:**

- [ ] `IRedisCacheManager` integrated with ABP `ICacheManager`.
- [ ] Tenant-aware cache keys.

### FR-003: Angular bundle budgets

**Description:** Enforce `angular.json` bundle budgets and split admin modules.

**Acceptance criteria:**

- [ ] `budgets` section configured for `initial` and `anyComponentStyle`.
- [ ] Admin routes lazy-loaded.

### FR-004: Runtime performance

**Description:** Use `OnPush`, debounce filters, and virtual scroll for large tables.

**Acceptance criteria:**

- [ ] Critical components use `OnPush`.
- [ ] Table filters debounced.

## 7. Business Rules

### BR-001: No regressions

Any optimization must not change behavior or reduce test coverage.

### BR-002: Cache invalidation

Cached data must be invalidated when the underlying entity changes via domain events.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

- Backend: ABP layered with domain events for cache invalidation.
- Frontend: Angular lazy-loaded feature modules and `OnPush`.

## 10. API Contracts

N/A.

## 11. Application Contracts

N/A.

## 12. Persistence and Data

### Indexes

| Index | Fields | Reason |
|---|---|---|
| `IX_AbpUsers_TenantId_IsDeleted` | `TenantId`, `IsDeleted` | Tenant-scoped user queries |
| `IX_AbpAuditLogs_TenantId_ExecutionTime` | `TenantId`, `ExecutionTime` | Audit log filtering |

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| Redis | Distributed cache | TCP | 5000ms | Yes |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Redis unavailable | Connection failure | Log warning, fallback to in-memory cache if configured |
| Large table rendered | 10,000 rows | Use virtual scroll or server-side pagination |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

| Metric | Target | Measurement |
|---|---|---|
| Angular initial bundle | < 1 MB gzipped | `ng build --stats-json` |
| First Contentful Paint | < 1.5 s | Lighthouse |
| API p99 reads | < 200 ms | OpenTelemetry |
| Backend memory per request | Flat | `dotnet-counters` |
| Test coverage | ≥ 90% | coverlet |

### Security

Do not cache sensitive tokens; sanitize logs.

### Observability

Add OpenTelemetry metrics for cache hit/miss, API latency, and bundle size.

### Maintainability

Document performance decisions in module READMEs.

## 17. Mandatory Guardrails

Do not reduce test coverage; do not hand-edit generated files; do not introduce dependencies without justification.

## 18. Expected Tests

| Class / Flow | Scenarios |
|---|---|
| `ExampleAppService` | N+1 detection, `AsNoTracking` usage |
| Angular build | Bundle budgets pass |
| `RedisCacheManager` | Get/Set/Invalidate with tenant key |

## 19. Acceptance Criteria

- [ ] Targets defined and measurable.
- [ ] At least one backend and one frontend optimization implemented.
- [ ] Tests pass and coverage does not decrease.

## 20. Implementation Plan

1. Instrument with OpenTelemetry and `dotnet-counters`.
2. Implement `Eaf.RedisCache`.
3. Audit top 10 API endpoints.
4. Add Angular bundle budgets and lazy loading.
5. Apply `OnPush` and debounce patterns.

## 21. Rollback Strategy

- Disable Redis cache provider if errors occur.
- Revert lazy-loaded routes if deep linking breaks.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Lazy loading breaks guards/routing | High | Medium | Add e2e smoke tests |
| Cache stale data | High | Medium | Domain event invalidation |
| Bundle size still too large | Medium | High | Iterate with Lighthouse CI |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Metrics instrumented.
- [ ] Optimizations implemented and tested.
- [ ] Documentation updated.

## 24. Key Reminder

> The SPEC is the contract. Measure before and after each optimization.

## Current Observations

- Angular template contains jQuery, `ngx-bootstrap`, legacy Metronic, and large `service-proxies.ts`.
- Backend has no distributed cache module.
- No `IAsyncEnumerable` usage observed in application services.
- `ChatHub` uses `memoryCache` but no backplane.

## Backend Actions

- Avoid `ToList()` before filtering; project with `Select`.
- Use `AsNoTracking()` for read-only queries.
- Add composite indexes.
- Use `IAsyncEnumerable<T>` for large exports.
- Move heavy work to Hangfire.
- Ensure short-lived `UnitOfWork` and no `.Result`/`.Wait()`.

## Frontend Actions

- Remove jQuery if unused; tree-shake `lodash`.
- Split admin pages with lazy loading.
- Import PrimeNG components individually.
- Use `OnPush`, debounce, virtual scroll.
- Unsubscribe RxJS subscriptions.

## References

- `Templates/Angular/Eaf.ProjectName.UI/angular.json`
- `src/Eaf.Middleware.Web.Core/Chat/ChatHub.cs`
- `.specs/eaf-backend-modularization.spec.md`
