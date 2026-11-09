# EAF — AutoMapper → Mapperly Migration: Spike Report (W2.2)

Status: spike complete — **go**. Date: 2026-11-09. Pilot module: `Eaf.DynamicEntityProperties`.

## What was proven

- `Riok.Mapperly 4.3.1` (stable) works on `net10.0`; source generator emits a partial class at build time, zero runtime reflection.
- `IObjectMapper` coexistence: `EafMapperlyObjectMapper` is registered `IsDefault` in `EafDynamicEntityPropertiesModule.Initialize()` (runs after `AbpAutoMapperModule` in `[DependsOn]` order), so all `ObjectMapper.Map<>` calls in the module route through generated code for owned pairs and delegate everything else to the still-registered AutoMapper `IMapper`. No DI conflicts — both frameworks live side by side.
- All 18 module tests pass unchanged through the adapter (includes the mandatory-property app-service tests added in W2.1).
- One RMG020 diagnostic surfaced at build time (`DynamicPropertyValue.DynamicProperty` nav has no DTO target) — exactly the "silent unmapped member" class of bug Mapperly is meant to catch. Acceptable here (AutoMapper also skipped it); enables treat-diagnostics-as-errors for the rollout.

## Migration pattern (repeat per module)

1. `PackageReference Riok.Mapperly 4.3.1` in the module csproj (keep `Abp.AutoMapper` during transition).
2. `[Mapper] public partial class <Module>Mapperly` — one `partial` method per entity→DTO pair; `[MapProperty]` for renamed members; list methods for each `Map<List<TDto>>` call site.
3. Extend the adapter's `Dispatch` table with the module's pairs (or a per-module adapter registered `IsDefault` in that module's `Initialize`).
4. Delete the module's `[AutoMapFrom]` attributes / `IMapperConfigurationExpression` registrations **last**, only after the whole repo is on the adapter (attributes are inert while Abp.AutoMapper stays).

## Inventory & effort estimate

| Area | Mapping sites | Est. effort |
|---|---|---|
| `Eaf.DynamicEntityProperties` (pilot) | 4 pairs + 1 custom map | done |
| `Eaf.Middleware.Core` (Chat, Friendships, cache items) | ~4 `[AutoMap]` DTOs | 0.5 day |
| `Eaf.Middleware.Application` (Roles, Users/Profile, WebHooks + `MiddlewareCustomDtoMapper`) | ~4 `[AutoMap]` + 1 custom profile | 1 day |
| `Eaf.Middleware.Web.Core` (TokenAuth models, SignalR chat, webhook receiver) | ~3 sites, mostly `IObjectMapper` passthrough | 0.5 day |
| `Eaf.Middleware.Worker` | base-class usages only, no DTO maps | trivial (adapter fallback) |
| `Templates/Api` (`CreateOrEditAirplaneDto`, `ProjectNameCustomDtoMapper`, module registration) | 1 `[AutoMap]` + 1 profile + tests | 0.5 day |
| `Templates/Worker` | csproj ref only | trivial |

**Total remaining: ~2.5 days** across 5 areas. Template NuGet-path builds (`Eaf.ProjectName.sln` on 9.4.6 packages) keep working because `Abp.AutoMapper` stays referenced; only after the last module ships do we drop the package and remove `AbpAutoMapperModule` from `[DependsOn]` chains.

## Risks

- `Map<TSource,TDest>(src,dest)` (object-update overload) and `ProjectTo<T>(IQueryable)` remain on AutoMapper during transition — modules using them can't be migrated until Mapperly `[Mapper]` update methods/`IQueryable` projections are written (none found in inventory).
- `IsDefault` override order depends on `Initialize()` ordering — verify per module that its `AbpModule` runs after `AbpAutoMapperModule` (guaranteed via `[DependsOn]`).

## Decision

**Go** — proceed to full rollout as a follow-up spec (`eaf-mapperly-rollout`, W4 or earlier).
