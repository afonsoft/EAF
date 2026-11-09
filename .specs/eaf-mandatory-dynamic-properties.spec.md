# EAF — Mandatory Dynamic Properties (W2)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Mandatory Dynamic Entity Properties |
| Product / System | EAF Middleware |
| Module / Bounded Context | `Eaf.DynamicEntityProperties` |
| Change type | Feature (extension) |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-mandatory-dynamic-properties` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W2 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

ASP.NET Zero v15.5 lets dynamic properties be marked **Mandatory**: saving entity values without a mandatory value is rejected and mandatory values cannot be deleted. EAF's `Eaf.DynamicEntityProperties` module lacks the mandatory flag.

### Objective

Add `IsMandatory` to dynamic property definitions; enforce on value save and delete.

### Expected outcome

- Definition gains `IsMandatory` (default false, backward compatible).
- Save-values rejects missing mandatory values; delete-value of mandatory rejected.
- Angular management UI exposes the flag.

### Out of scope

- Mandatory validation inside EF entity materialization (validated at app-service boundary).

## 2–4. Role / Context

Extend `src/Eaf.DynamicEntityProperties/` and its Angular admin pages (`dynamic-properties` under admin). Read `CLAUDE.md`, module sources, and existing tests first.

## 5. Task Definition

- Add `IsMandatory` property to the property definition entity + EF migration.
- Validation in the value save path: all mandatory properties of the entity must have values.
- Delete path: reject deletion of values for mandatory properties.
- Angular: checkbox in definition create/edit; badge in list; error message on failed save.
- Localization keys en + pt-BR.

### Do not do

- Do not retroactively invalidate existing data (mandatory applies to new writes only).
- Do not change the public value DTOs' shape — add field, don't reshape.

## 6. Functional Requirements

### FR-001: Definition flag

- [ ] `IsMandatory` persisted, editable, default false.

### FR-002: Save enforcement

- [ ] Saving entity values without all mandatory values → `UserFriendlyException` naming the missing property.
- [ ] Entities without mandatory properties unaffected.

### FR-003: Delete enforcement

- [ ] Deleting a value whose property is mandatory → rejected.
- [ ] Clearing a mandatory value via empty input → rejected.

## 7. Business Rules

- BR-001: Making a property mandatory does not require backfilling existing entities; enforcement applies on next write.
- BR-002: Demoting mandatory → allowed; values remain.

## 8–12. Domain / Persistence

`IsMandatory bool NOT NULL DEFAULT 0` migration on the definitions table. Index unchanged.

## 18. Expected Tests

- Unit: save missing mandatory → throws; delete mandatory → throws; non-mandatory unchanged.
- Integration: full save flow via app service.

## Pending Questions and Ambiguities

- Should mandatory apply per-entity-instance or globally for the entity type? Default: global per definition (matches Zero semantics).
