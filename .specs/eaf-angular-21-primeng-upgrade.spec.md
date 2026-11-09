# EAF — Angular 21 + PrimeNG Upgrade (W4)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Angular 21 + PrimeNG major upgrade (no ng-zorro) |
| Product / System | EAF Angular Template |
| Module / Bounded Context | `Templates/Angular/Eaf.ProjectName.UI` |
| Change type | Migration |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular21-primeng` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W4 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

ASP.NET Zero runs Angular 21 (v15.2) and swapped PrimeNG for ng-zorro (v15.5). **Decision (user, 2026-11-08): EAF stays on PrimeNG — no ng-zorro.** EAF needs Angular 20 → 21 and PrimeNG 17 → current major (19/20) to stay supported.

### Objective

Upgrade the Angular template to Angular 21 and latest compatible PrimeNG, adapting to PrimeNG's new theming system (`@primeuix/themes`, design tokens) introduced in v18+.

### Expected outcome

- `npx ng build --configuration=production` and `npx ng test` green on Node 20/22.
- `PrimengTableHelper` and all admin pages work unchanged functionally.
- Theme migrated to `@primeuix/themes` preset (Aura or Lara) preserving current look.

### Out of scope

- ng-zorro adoption.
- Visual redesign / Metronic.
- Backend changes.

## 2–4. Context

`Templates/Angular/Eaf.ProjectName.UI/package.json`: Angular `^20.3`, primeng `^17.17`, ngx-bootstrap `^12`. PrimeNG ≥18 breaking: theming moved from CSS theme files to `@primeuix/themes` presets + `providePrimeNG`. Check blueprint notes: Node 20, `--legacy-peer-deps`, ChromeHeadlessNoSandbox Karma.

### Context files

- `CLAUDE.md`, blueprint knowledge (Angular workspace)
- `package.json`, `angular.json`, theme/style assets
- `src/app/shared/` helpers (`PrimengTableHelper`)
- `.specs/eaf-angular-modern-primeng-components.spec.md`, `.specs/eaf-angular-dark-mode-theming.spec.md`

## 5. Task Definition

- `ng update` path Angular 20→21 (cli, core, cdk if used).
- PrimeNG 17→19/20: replace theme CSS imports with `@primeuix/themes` preset config; map old theme choice; verify dark-mode spec compatibility.
- Fix deprecated/removed APIs (`p-table` paginator APIs, `TableLazyLoadEvent`, styleClass→style, etc. per migration guide).
- Keep `PrimengTableHelper` API surface stable (or adapt call-sites).
- ngx-bootstrap compatibility check.
- Run unit tests (179 spec files), prod build, lint, eslint.
- Update blueprint knowledge notes (node version, commands) if changed.

### Do not do

- Do not redesign pages.
- Do not remove tests to make suite pass — fix them.
- Do not bump Node in CI without checking blueprint/workflows alignment.

## 6. Functional Requirements

- [ ] Build + all unit tests green.
- [ ] No PrimeNG deprecation warnings left that block future minor upgrades.
- [ ] Theme visually equivalent (screenshot spot-check key admin pages).

## 16. Non-Functional Requirements

- Bundle size regression ≤ 10% vs baseline.
- Keep `--legacy-peer-deps` workflow documented if still needed.

## 18. Expected Tests

- Existing 179 spec files pass; add coverage only if helper APIs changed.

## Pending Questions and Ambiguities

- Exact PrimeNG target major (19 vs 20/21 if released) — pick latest stable compatible with Angular 21 at implementation time.
- Karma vs Web Test Runner — Angular 21 defaults may warrant test-runner migration; decide at implementation (minimal disruption preferred).
