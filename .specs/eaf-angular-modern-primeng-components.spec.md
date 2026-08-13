# EAF Angular — Modernize Components with PrimeNG

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Modernize Components with PrimeNG |
| Product / System | EAF Angular Template |
| Module / Bounded Context | UI Components |
| Change type | Refactor / Frontend |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-primeng` |
| Technical owner | Frontend Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

`package.json` already includes `primeng ^17.17.0`, but many components still use `ngx-bootstrap ^12.0.0` and legacy custom widgets. This leads to inconsistent UX, accessibility gaps, and a larger bundle.

### Objective

Standardize the EAF Angular template on PrimeNG 17+ components and reduce the dependency on `ngx-bootstrap` and jQuery.

### Expected outcome

- All modals, dropdowns, datepickers, tabs, and tooltips use PrimeNG.
- Bundle size is reduced or at least not increased.
- Accessibility improves through PrimeNG built-in ARIA support.

### Out of scope

- Full Metronic 8 migration.
- Rewriting non-admin pages in a single pass.

## 2. Agent Role

Senior Angular engineer. Replace widgets one-to-one, preserve behavior, and add tests.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

The Angular template uses PrimeNG partially (`p-table`, `p-dialog`, `p-fileUpload`) and `ngx-bootstrap` for dropdown, datepicker, modal, tooltip, tabs, and accordion.

### Relevant stack

- Angular 20, TypeScript 5.8, PrimeNG 17.17.0, `ngx-bootstrap` 12

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/src/app
Templates/Angular/Eaf.ProjectName.UI/src/app/app.module.ts
Templates/Angular/Eaf.ProjectName.UI/package.json
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
- `.specs/eaf-angular-dark-mode-theming.spec.md`

## 5. Task Definition

### Main task

Replace `ngx-bootstrap` and legacy widgets with PrimeNG components across the Angular admin template.

### Subtasks

- Inventory all `ngx-bootstrap` usages.
- Replace components in critical admin pages.
- Update CSS for PrimeNG themes.
- Run regression tests.

### Do not do

- Do not modify `service-proxies.ts`.
- Do not remove `ngx-bootstrap` until all usages are replaced.

## 6. Functional Requirements

### FR-001: Replace ngx-bootstrap components

**Description:** Replace `ngx-bootstrap` with PrimeNG equivalents.

| ngx-bootstrap | PrimeNG 17+ |
|---|---|
| Dropdown | `p-dropdown` / `p-splitButton` |
| Datepicker | `p-calendar` |
| Modal | `p-dialog` |
| Tooltip | `p-tooltip` |
| Tabs | `p-tabView` |
| Accordion | `p-accordion` |
| Typeahead | `p-autoComplete` |

**Acceptance criteria:**

- [ ] `ngx-bootstrap` imports removed from `app.module.ts`.
- [ ] No `ngx-bootstrap` usage remains in admin pages.

### FR-002: Standardize forms

**Description:** Use PrimeNG inputs (`p-inputText`, `p-inputNumber`, `p-checkbox`, `p-radioButton`, etc.).

**Acceptance criteria:**

- [ ] Forms use `p-fluid` and responsive grid.
- [ ] Validation messages are associated and accessible.

### FR-003: Tables

**Description:** Replace custom datatables with `p-table`.

**Acceptance criteria:**

- [ ] `p-table` with `responsiveLayout` and lazy loading.
- [ ] `p-paginator` and `p-columnFilter` used where appropriate.

### FR-004: PrimeNG theme

**Description:** Adopt PrimeNG 17 theming system with an EAF theme.

**Acceptance criteria:**

- [ ] `theme-eaf` tokens defined.
- [ ] Dark mode tokens supported.

## 7. Business Rules

### BR-001: One-to-one behavior

Each replacement must preserve existing user-visible behavior and validation.

### BR-002: No bundle regression

Bundle size should not increase; ideally decrease after removing `ngx-bootstrap`.

## 8. Domain Modeling

N/A — frontend only.

## 9. Expected Architecture

Angular feature modules using PrimeNG standalone components where applicable.

## 10. API Contracts

N/A.

## 11. Application Contracts

N/A.

## 12. Persistence and Data

N/A.

## 13. Integrations

N/A.

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| `p-calendar` locale differences | User locale | Use app localization |
| `p-dialog` focus trap | Modal open | Focus trapped and returned on close |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

Import PrimeNG components individually to enable tree-shaking.

### Accessibility

Use PrimeNG `aria-*` attributes and visible focus.

### Maintainability

Document the widget replacement pattern for future admin pages.

## 17. Mandatory Guardrails

Do not remove `ngx-bootstrap` until all usages replaced; do not hand-edit generated files; preserve localization pipe.

## 18. Expected Tests

| Component / Flow | Scenarios |
|---|---|
| Admin users/roles/tenants pages | PrimeNG table, dialog, forms |
| `app.module.ts` | No `ngx-bootstrap` imports |
| Accessibility | Keyboard navigation on new widgets |

## 19. Acceptance Criteria

- [ ] `ngx-bootstrap` removed from `app.module.ts`.
- [ ] Critical admin pages use PrimeNG components.
- [ ] Build and tests pass.
- [ ] No visual regressions in key flows.

## 20. Implementation Plan

1. Inventory `ngx-bootstrap` usages.
2. Replace in Users, Roles, Tenants as pilot.
3. Replace generic components.
4. Migrate CSS to PrimeNG theme.
5. Remove `ngx-bootstrap` package after replacements.
6. Run regression tests.

## 21. Rollback Strategy

- Revert page-specific changes and keep `ngx-bootstrap` if a page fails.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| API differences between widgets | Medium | High | Test each replaced widget |
| Custom CSS relies on Bootstrap classes | Medium | High | Replace with PrimeNG utility classes incrementally |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] `ngx-bootstrap` usages replaced.
- [ ] Tests pass.
- [ ] No bundle regression.

## 24. Key Reminder

> The SPEC is the contract. Replace widgets one-to-one; do not redesign pages.

## Implementation Status (2026-08)

Partial. `p-table`, `p-dialog`, `p-paginator`, `p-fileUpload`, and `p-progressbar` are already used in admin pages. `ngx-bootstrap` (`ModalModule`, `TabsModule`, `BsDropdownModule`, `TooltipModule`, `PopoverModule`) is still imported in `app.module.ts` and widely used.

## References

- <https://primeng.org/installation>
- `Templates/Angular/Eaf.ProjectName.UI/package.json`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/app.module.ts`
