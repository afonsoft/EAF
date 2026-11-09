# EAF — ABP 11.x Upstream Watch & Fix Porting (W0)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | ABP 11.x upstream tracking + manual port of 11.4 fixes |
| Product / System | EAF Middleware |
| Module / Bounded Context | Cross-cutting / Dependencies |
| Change type | Maintenance / Roadmap |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `chore/eaf-abp-11x-watch` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-11-08 |
| Target agent | Any |
| Wave | W0 (validation gate, runs last) |

## 1. Executive Summary

### Problem

ASP.NET Zero v15.5 claims "ABP 11.4", but public NuGet/GitHub only has `Abp.*` **10.5.1** (verified 2026-11-08; v10.5.1 released 2026-10-13). The 11.x line is currently Volosoft-internal for commercial products. EAF cannot upgrade, but the announced fixes may be worth porting manually.

### Objective

Track upstream ABP releases; meanwhile evaluate and port the security/correctness fixes announced for "11.4" that apply to EAF on 10.5.1.

### Expected outcome

- Documented verdict per announced fix: already-fixed / ported / not-applicable / waiting-upstream.
- PRs for any ported fix with tests.
- Re-check cadence defined.

### Out of scope

- Vendoring/forking `aspnetboilerplate` wholesale (re-evaluate only if upstream goes unmaintained).

## 2–4. Context

Announced ABP 11.4 items relevant to EAF:

- EF Core: tenant-aware query compilation caching (perf).
- `UseAbpQueryCompiler`: fail (not silently drop filters) when tenant/soft-delete filters can't apply.
- `AbpSignInManager` denies sign-in for inactive tenants (incl. bypass flows).
- Webhook subscription tenant isolation fixes.
- OpenIddict stores save entities on create.
- Module Zero passkey store (superseded by `eaf-passkey-login.spec.md` approach).

## 5. Task Definition

- For each item: locate equivalent EAF/Abp 10.5.1 code path; classify status; port if valuable + feasible.
- Add a recurring check (automation or roadmap note) for `Abp.*` ≥ 11.0 on NuGet.

### Do not do

- Do not backport wholesale without a failing/gap test justifying each change.

## 6. Functional Requirements

- [ ] Per-item verdict table committed to this spec or linked doc.
- [ ] Each ported fix has regression test proving the gap existed.

## Pending Questions and Ambiguities

- Where does `AbpSignInManager` behavior live in EAF (custom sign-in manager?) — audit at execution.
