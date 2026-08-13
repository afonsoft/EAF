# EAF Angular — Accessibility (a11y) and WCAG Compliance

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Angular Accessibility and WCAG Compliance |
| Product / System | EAF Angular Template |
| Module / Bounded Context | UI / Accessibility |
| Change type | Refactor / Frontend |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-a11y` |
| Technical owner | Frontend Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

The Angular admin template lacks a systematic accessibility audit and does not explicitly target WCAG 2.1 Level AA. This can block enterprise and government use cases.

### Objective

Make the EAF Angular admin template conform to WCAG 2.1 Level AA by improving keyboard navigation, color contrast, ARIA roles, focus management, and screen-reader support.

### Expected outcome

- Lighthouse accessibility score ≥ 90 on core pages.
- Keyboard-only navigation works for all admin flows.
- Color contrast meets 4.5:1 for normal text and 3:1 for large text.

### Out of scope

- Full UX redesign.
- Accessibility of non-admin pages in the first pass.

## 2. Agent Role

Senior frontend engineer with accessibility expertise. Make incremental, evidence-based fixes and add automated checks.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

The template uses PrimeNG 17, `ngx-bootstrap` 12, and legacy Metronic markup. Some components already have ARIA attributes from PrimeNG, but the app lacks global a11y audit and tests.

### Relevant stack

- Angular 20, PrimeNG 17, `ngx-bootstrap`, TypeScript 5.8

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout
Templates/Angular/Eaf.ProjectName.UI/src/app/admin
Templates/Angular/Eaf.ProjectName.UI/src/styles.css
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-dark-mode-theming.spec.md`
- `.specs/eaf-angular-modern-primeng-components.spec.md`

## 5. Task Definition

### Main task

Audit and improve accessibility across the main admin components and layout.

### Subtasks

- Run Lighthouse and axe-core on top pages.
- Add semantic HTML and ARIA roles.
- Fix color contrast.
- Improve keyboard navigation.
- Add automated a11y tests.

### Do not do

- Do not remove visual design.
- Do not modify `service-proxies.ts`.

## 6. Functional Requirements

### FR-001: Semantic HTML and ARIA

**Description:** Use `<header>`, `<nav>`, `<main>`, `<aside>`, `<footer>`, and proper ARIA attributes.

**Acceptance criteria:**

- [ ] Layout components use semantic tags.
- [ ] Buttons are `<button>` or `p-button`, not `div`.
- [ ] Menus, tabs, and page nav have `aria-label`, `aria-expanded`, `aria-current`.
- [ ] Skip link "Skip to main content" added and visible on focus.

### FR-002: Color contrast

**Description:** Ensure text meets WCAG AA contrast ratios.

**Acceptance criteria:**

- [ ] All text ≥ 4.5:1 (AA) or 3:1 for large text.
- [ ] Orange `#FF7020` on white has dark text or sufficient contrast.

### FR-003: Keyboard navigation

**Description:** All interactive elements operable with keyboard.

**Acceptance criteria:**

- [ ] Sidebar menu items focusable and operable with Enter/Space/Arrows.
- [ ] `p-dialog` traps focus and returns focus on close.
- [ ] Escape closes dropdowns, modals, and panels.

### FR-004: Screen reader support

**Description:** Dynamic content announced to screen readers.

**Acceptance criteria:**

- [ ] ARIA live regions for chat, notifications, toasts.
- [ ] Icon-only buttons have `aria-label` / `title`.

### FR-005: Forms and tables

**Description:** Labels, validation, and tables are programmatically associated.

**Acceptance criteria:**

- [ ] Labels `for` match input `id`.
- [ ] `aria-invalid` and error messages associated.
- [ ] Tables have `aria-label` or `<caption>` and `<th scope>`.

### FR-006: Focus visibility

**Description:** Provide visible `:focus-visible` ring.

**Acceptance criteria:**

- [ ] No `outline: 0` without replacement.
- [ ] Focus ring visible on keyboard navigation.

## 7. Business Rules

### BR-001: No color-only indicators

Status and actions must include text or icon labels, not color alone.

### BR-002: Keyboard equivalence

Every mouse-operable feature must be keyboard-operable.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

Angular components with semantic markup and ARIA attributes; CSS focus management.

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
| Focus lost inside modal | Tab past last focusable | Return to first element (loop) |
| Color-only status indicator | Red text alone | Add icon or text label |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

A11y checks should not block build if they are warnings; errors should fail CI.

### Security

No PII exposed in ARIA labels or live regions.

### Maintainability

Document a11y patterns in the frontend contribution guide.

## 17. Mandatory Guardrails

Do not remove focus outlines without replacement; do not rely on color alone; do not modify generated files.

## 18. Expected Tests

| Test type | Scenarios |
|---|---|
| axe-core unit | Top 20 components |
| Cypress/Playwright a11y | Login, users, roles, tenants |
| Manual keyboard | Full admin flow |

## 19. Acceptance Criteria

- [ ] Lighthouse a11y score ≥ 90 on core pages.
- [ ] No critical axe violations.
- [ ] Keyboard flow verified.

## 20. Implementation Plan

1. Audit top 20 components with Lighthouse and axe-core.
2. Fix layout, topbar, sidebar, and form components.
3. Add `axe-core` + `jest-axe` unit tests and Cypress a11y checks.
4. Set target WCAG 2.1 AA.
5. Document patterns.

## 21. Rollback Strategy

- Revert CSS or markup changes if they break existing themes.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Legacy Metronic CSS overrides focus | High | High | Test and override carefully |
| Custom widgets lack ARIA | Medium | High | Add manual roles incrementally |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Audit completed.
- [ ] Critical issues fixed.
- [ ] Automated a11y tests added.

## 24. Key Reminder

> The SPEC is the contract. Aim for WCAG 2.1 AA; do not redesign the UI.

## Implementation Status (2026-08)

Partial. PrimeNG 17 components include built-in ARIA attributes, but the application does not yet have a global a11y audit, skip links, contrast validation, or automated a11y tests.

## References

- <https://www.w3.org/WAI/WCAG21/quickref/>
- <https://primeng.org/accessibility>
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout`
