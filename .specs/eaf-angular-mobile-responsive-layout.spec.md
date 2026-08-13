# EAF Angular — Mobile Responsive Layout

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Mobile Responsive Layout |
| Product / System | EAF Angular Template |
| Module / Bounded Context | UI / Layout |
| Change type | Feature / Refactor |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-mobile` |
| Technical owner | Frontend Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

The Angular template is desktop-first. Layout relies on legacy Metronic `m-grid--desktop` and few custom media queries, so admin workflows are hard to use on phones and tablets.

### Objective

Deliver a mobile-first responsive layout with adaptive navigation, off-canvas sidebar, optimized admin components, and touch-friendly targets.

### Expected outcome

- Admin pages are usable on screens down to 320px.
- Sidebar and panels behave as off-canvas drawers on mobile.
- Tables and forms adapt to small screens without horizontal overflow.

### Out of scope

- Full Metronic 8 migration.
- Native mobile app.

## 2. Agent Role

Senior frontend engineer with mobile UX focus. Use Bootstrap 5 / native CSS, test on real viewports.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

Template uses Angular 20, PrimeNG 17, legacy Metronic CSS. Only `chat-bar.component.css` has a mobile media query.

### Relevant stack

- Angular 20, PrimeNG 17, Bootstrap 4/5, `ngx-bootstrap`

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout
Templates/Angular/Eaf.ProjectName.UI/src/app/admin
Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-metronic8-bootstrap5-migration.spec.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`

## 5. Task Definition

### Main task

Implement mobile-first responsive layout for the EAF Angular admin template.

### Subtasks

- Define breakpoints and layout behavior.
- Refactor layout components for off-canvas sidebar and panels.
- Optimize admin tables, forms, and modals for mobile.
- Add touch targets and gestures.

### Do not do

- Do not break desktop layout.
- Do not add native app features.

## 6. Functional Requirements

### FR-001: Breakpoints

**Description:** Adopt standard breakpoints and layout behavior.

| Range | Behavior |
|---|---|
| < 576px | Mobile: fixed header, hidden sidebar, off-canvas, bottom nav padding |
| 576–991px | Tablet: collapsed icon sidebar |
| ≥ 992px | Desktop: expanded sidebar |

**Acceptance criteria:**

- [ ] CSS uses standard `@media` breakpoints.
- [ ] Layout tested at 320px, 768px, 1920px.

### FR-002: Layout components

**Description:** Refactor `default-layout`, `topbar`, `side-bar-menu`, `chat-bar`.

**Acceptance criteria:**

- [ ] Sidebar is off-canvas on mobile with overlay and close on outside click.
- [ ] Topbar has hamburger menu and compact profile/notifications dropdown.
- [ ] Chat panel is full-width on mobile.

### FR-003: Admin components

**Description:** Tables and forms adapt to mobile.

**Acceptance criteria:**

- [ ] `p-table` uses `responsiveLayout="scroll"` or `stack`.
- [ ] Forms stack fields on mobile.
- [ ] `p-dialog` uses full viewport on mobile.

### FR-004: Touch navigation

**Description:** Touch targets and gestures.

**Acceptance criteria:**

- [ ] Touch targets ≥ 44x44px.
- [ ] Hover-only menus avoided or duplicated with click handlers.

## 7. Business Rules

### BR-001: Desktop parity

Desktop layout and features must remain unchanged.

### BR-002: No horizontal overflow

Admin pages must not require horizontal scrolling on phones except for intentionally scrollable tables.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

Mobile-first CSS, off-canvas components, responsive PrimeNG tables.

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
| Virtual keyboard opens on input | Mobile focus | Keep input and buttons visible |
| Orientation change | Portrait ↔ landscape | Re-layout without breaking state |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

Lazy-load mobile-specific components if beneficial.

### Accessibility

Mobile menus must be operable with screen readers and external keyboards.

## 17. Mandatory Guardrails

Do not break desktop; do not use fixed widths that break small screens; do not add native-only gestures.

## 18. Expected Tests

| Component / Flow | Scenarios |
|---|---|
| Layout | Mobile off-canvas open/close |
| Users page | Table scroll and form stacking |
| e2e | Login and tenant creation on mobile viewport |

## 19. Acceptance Criteria

- [ ] Core admin flows usable on 320px screen.
- [ ] No horizontal overflow on pages.
- [ ] Desktop unchanged.
- [ ] Tests pass.

## 20. Implementation Plan

1. Inventory layout and admin components.
2. Add off-canvas CSS and behavior.
3. Refactor tables and forms.
4. Add mobile e2e tests.
5. Document mobile-first patterns.

## 21. Rollback Strategy

- Revert CSS if desktop layout breaks.
- Keep old layout behind feature flag until verified.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Metronic bundle conflicts | High | High | Scope CSS carefully, use `!important` only when needed |
| Testing on all devices | Medium | High | Use BrowserStack or emulators |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Mobile layout implemented.
- [ ] Tests updated.
- [ ] Documentation updated.

## 24. Key Reminder

> The SPEC is the contract. Mobile-first does not mean desktop-last.

## Implementation Status (2026-08)

Partial. Some responsive CSS exists in `styles.css`, but no mobile-first off-canvas layout, bottom navigation, or comprehensive breakpoint system. `m-stack`, `m-grid`, and `m-aside-left` still dominate the layout.

## References
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout`
- `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/themes`
