# EAF Angular — Migrate to Metronic 8 + Bootstrap 5

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Metronic 8 and Bootstrap 5 Migration |
| Product / System | EAF Angular Template |
| Module / Bounded Context | UI / Layout |
| Change type | Refactor / Migration |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-metronic8-bs5` |
| Technical owner | Frontend Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

The Angular template relies on legacy Metronic / Bootstrap 4 minified bundles and `m-grid` / `m-stack` classes. This blocks modern responsive features, dark mode, and CSS Custom Properties.

### Objective

Replace the legacy Metronic / Bootstrap 4 layout with Metronic 8 + Bootstrap 5, using SASS variables, responsive grid, and CSS Custom Properties.

### Expected outcome

- Modern responsive grid and off-canvas sidebar.
- CSS variables for theming.
- Reduced dependency on legacy `m-*` utility classes.

### Out of scope

- Rewriting business logic or APIs.
- Copying Metronic 8 commercial assets without a license.

## 2. Agent Role

Senior Angular/frontend engineer. Migrate incrementally, preserve routes and behavior, and add visual regression tests.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

The template uses minified `style.bundle.css`, `m-grid`, `m-stack`, `m-portlet`, and `m-form` classes. `ngx-bootstrap` supplies Bootstrap 4-based widgets.

### Relevant stack

- Angular 20, Bootstrap 4/5, `ngx-bootstrap` 12, jQuery, Metronic
- PrimeNG 17

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles
Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout
Templates/Angular/Eaf.ProjectName.UI/package.json
Templates/Angular/Eaf.ProjectName.UI/angular.json
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
- `.specs/eaf-angular-dark-mode-theming.spec.md`
- `.specs/eaf-angular-modern-primeng-components.spec.md`

## 5. Task Definition

### Main task

Migrate the Angular template layout and components from legacy Metronic / Bootstrap 4 to Metronic 8 / Bootstrap 5.

### Subtasks

- Update dependencies to Bootstrap 5.
- Replace `ngx-bootstrap` with `ng-bootstrap` or PrimeNG.
- Refactor layout components.
- Migrate CSS to SASS variables.
- Run visual regression.

### Do not do

- Do not remove `ngx-bootstrap` until replacements are in place.
- Do not break existing routes.

## 6. Functional Requirements

### FR-001: Bootstrap 5 dependencies

**Description:** Update `bootstrap` to `^5.3.x` and remove `ngx-bootstrap`.

**Acceptance criteria:**

- [ ] `package.json` updated.
- [ ] `ngx-bootstrap` replaced by `ng-bootstrap` or PrimeNG components.

### FR-002: Layout rewrite

**Description:** Replace `m-grid--desktop`, `m-stack--desktop`, `m-aside-left` with Bootstrap 5 / Metronic 8 layout.

**Acceptance criteria:**

- [ ] `default-layout` uses Flex / CSS Grid / Bootstrap utilities.
- [ ] Mobile sidebar uses `offcanvas`.

### FR-003: CSS variables

**Description:** Use Bootstrap 5 variables and EAF override file.

**Acceptance criteria:**

- [ ] `_variables.scss` with EAF brand color `#FF7020`.
- [ ] No hardcoded colors in global styles.

### FR-004: Component migration

**Description:** Replace `m-*` utility classes with Bootstrap 5 classes.

**Acceptance criteria:**

- [ ] Remove custom `m--margin-*`, `m--padding-*` classes.
- [ ] Use `g-3`, `p-3`, `d-none d-lg-block`, etc.

## 7. Business Rules

### BR-001: No visual regression

The migrated layout must look and behave equivalently on desktop.

### BR-002: Mobile-first

New layout must be mobile-first and responsive.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

Bootstrap 5 + Metronic 8 layout components; CSS Custom Properties; Angular feature modules.

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
| Old class name remains | `m-grid--desktop` | Fail build or lint if not in allowlist |
| Bootstrap 5 JS API change | Collapse, dropdown | Update component code |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

Reduce bundle size by removing legacy Metronic CSS/JS if possible.

### Accessibility

Use Bootstrap 5 a11y patterns and native HTML landmarks.

### Maintainability

Document class migration guide in `docs/`.

## 17. Mandatory Guardrails

Do not copy Metronic 8 commercial assets without license; do not break routes; do not remove `ngx-bootstrap` prematurely.

## 18. Expected Tests

| Component / Flow | Scenarios |
|---|---|
| Layout | Desktop/mobile render |
| Sidebar | Open/close on mobile and desktop |
| Admin pages | No visual regressions |

## 19. Acceptance Criteria

- [ ] Bootstrap 5 and Metronic 8 assets in place.
- [ ] Layout works on mobile, tablet, desktop.
- [ ] Existing admin pages render correctly.
- [ ] Tests pass.

## 20. Implementation Plan

1. Create parallel branch with Bootstrap 5 and Metronic 8 assets.
2. Convert `default-layout` as pilot.
3. Migrate admin pages incrementally.
4. Replace `ngx-bootstrap` components one by one.
5. Run visual regression with Lighthouse and Percy.
6. Deprecate old CSS themes.

## 21. Rollback Strategy

- Keep legacy theme available until new layout is fully verified.
- Feature-flag new layout.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Bootstrap 5 class changes break UI | High | High | Incremental migration + visual regression |
| `ngx-bootstrap` must be fully replaced first | High | Medium | Replace before updating Bootstrap |
| Metronic 8 license | High | Low | Use Bootstrap 5 + custom EAF theme if no license |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Layout migrated and tested.
- [ ] Old CSS themes deprecated.
- [ ] Documentation updated.

## 24. Key Reminder

> The SPEC is the contract. Migrate layout first; do not rewrite business logic.

## Implementation Status (2026-08)

Not started. The template still uses legacy Metronic / Bootstrap 4 classes. `ngx-bootstrap ^12.0.0` remains in `package.json`.

## References

- <https://getbootstrap.com/docs/5.3/getting-started/introduction/>
- <https://keenthemes.com/metronic/>
- `Templates/Angular/Eaf.ProjectName.UI/package.json`
