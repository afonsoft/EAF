# EAF — AutoMapper → Mapperly Migration Spike (W2)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | AutoMapper → Mapperly migration (spike + rollout) |
| Product / System | EAF Middleware + Templates |
| Module / Bounded Context | Cross-cutting / Infrastructure |
| Change type | Refactor / Migration |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-mapperly-spike` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W2 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

EAF is pinned to `Abp.AutoMapper 10.5.0` — binary-incompatible with AutoMapper ≥ 15 and dependent on runtime reflection. ASP.NET Zero migrated to Mapperly (source-generated mappers: faster, AOT-friendly, compile-time checked).

### Objective

Spike: prove Mapperly can replace AutoMapper in one bounded module end-to-end, then define the migration pattern for the remaining modules/templates.

### Expected outcome

- One pilot module migrated with tests green.
- Documented migration pattern + go/no-go report for full rollout.
- Decision recorded (ADR/spec update) — full rollout is a follow-up spec if approved.

### Out of scope

- Migrating every module in this spec (spike only).
- Keeping both mappers long-term (transitional only).

## 2–4. Role / Context

`grep -r "AutoMapper" src Templates` to inventory profiles/usages. `Abp.AutoMapper` provides `AutoMap` attributes + `IObjectMapper`; Mapperly uses `[Mapper]` partial classes — the shim between `IObjectMapper` and Mapperly is the design core.

### Context files

- `CLAUDE.md`, `common.props`
- All `*AutoMapperProfile.cs` / `[AutoMap]` usages in `src/` and `Templates/`

## 5. Task Definition

- Inventory all AutoMapper usage (profiles, attributes, `IObjectMapper` calls).
- Design `IObjectMapper` adapter delegating to generated Mapperly mappers; fallback for unmapped pairs.
- Migrate ONE module (suggest smallest: `Eaf.HtmlSanitizer` or a template submodule) including tests.
- Write migration-pattern doc + effort estimate per remaining module.

### Do not do

- Do not change public DTO shapes.
- Do not remove `Abp.AutoMapper` package repo-wide in the spike (pilot module only).

## 6. Functional Requirements

- [ ] Pilot module maps identically (existing tests pass unchanged).
- [ ] Generated mapper output reviewed for correctness (no silent unmapped members).
- [ ] Migration doc lists every module's profiles and estimated effort.

## 16. Non-Functional Requirements

- Source-gen must not break `dotnet build` determinism; warning-level mismatches surfaced as errors (Mapperly diagnostics enabled).

## 18. Expected Tests

- Existing mapping tests green; add one generated-code sanity test if feasible.

## Pending Questions and Ambiguities

- Mapperly package/version compatible with net10.0 — verify at spike.
- Coexistence: `Abp.AutoMapper` stays for untouched modules during transition — confirm no DI conflicts when both registered.
