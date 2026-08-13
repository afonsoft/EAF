# EAF Angular — Accessibility (a11y) and WCAG Compliance

## Summary

Make the EAF Angular admin template conform to **WCAG 2.1 Level AA** by improving keyboard navigation, color contrast, ARIA roles, focus management and screen-reader support across the main components.

## Motivation

- Accessibility is a requirement in enterprise and government projects.
- Modern frameworks (PrimeNG, Angular Material) already ship with accessibility baked in.
- Improves UX for all users, not only those using assistive technologies.

## Current State

- Uses PrimeNG 17, ngx-bootstrap 12 and legacy Metronic markup.
- Tables use `p-table` in some pages but often rely on custom CSS classes.
- No explicit ARIA live regions or skip links.
- Color contrast may fail for light gray text (`m--font-grey`, `text-muted`) and the orange `#FF7020` on white.
- `role="presentation"` and `tabindex` are hardcoded in some places.

## Proposed Changes

### 1. Semantic HTML and ARIA
- Use `<header>`, `<nav>`, `<main>`, `<aside>`, `<footer>` in layout components.
- Replace `div`-based buttons with `<button>` or `p-button`.
- Add `aria-label`, `aria-describedby`, `aria-expanded` and `aria-current` to menus, buttons, tabs and page navigation.
- Add skip link "Skip to main content" visible on focus.

### 2. Color Contrast
- Ensure all text meets 4.5:1 (AA) or 3:1 for large text.
- Update `--primary: #FF7020` usage to provide dark text on orange.
- Avoid color-only indicators; add icons and text labels.

### 3. Keyboard Navigation
- Sidebar menu items must be focusable and operable with Enter/Space/Arrow keys.
- Modals (`p-dialog`) must trap focus and return focus to the trigger on close.
- Escape key should close dropdowns, modals and panels.

### 4. Screen Reader Support
- Add ARIA live regions for chat messages, notifications and toasts.
- Use `aria-live="polite"` for dynamic content updates.
- Label all icon-only buttons (`aria-label` / `title`).

### 5. Forms
- Associate labels and inputs with `for`/`id`.
- Use `aria-invalid` and `aria-errormessage` for validation.
- Error messages should be programmatically associated.

### 6. Tables
- Use `p-table` with `aria-label` or `caption`.
- Ensure headers are real `<th>` with `scope`.
- Add `aria-sort` to sortable columns.

### 7. Focus Visibility
- Provide a visible focus ring (`:focus-visible`) that is not removed by `outline: 0`.

## Implementation Status (2026-08)

Partial. PrimeNG 17 components include built-in ARIA attributes, but the application does not yet have a global a11y audit, skip links, contrast validation, or automated a11y tests.

## Migration Plan
1. Audit top 20 components with Lighthouse, axe-core and manual keyboard test.
2. Fix layout, topbar, sidebar and form components first.
3. Add `axe-core` + `jest-axe` unit tests and Cypress a11y checks.
4. Set target: WCAG 2.1 AA.
5. Document accessibility patterns in the UI contribution guide.

## Impact
- **Medium**: changes templates and CSS.
- **High**: improves compliance and usability.
- **Low/Medium**: may require icon font alternatives for screen readers.

## Risks
- Legacy Metronic CSS may override focus styles; need careful CSS management.
- Some widgets (chat, notifications) are custom and need manual ARIA roles.

## References
- <https://www.w3.org/WAI/WCAG21/quickref/> — WCAG 2.1 quick reference.
- <https://primeng.org/accessibility> — PrimeNG accessibility docs.
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout`
