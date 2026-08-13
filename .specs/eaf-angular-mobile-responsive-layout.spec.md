# EAF Angular — Mobile Responsive Layout

## Summary

Improve the EAF Angular template experience on small screens by replacing the current desktop-first behavior with a mobile-first layout, adaptive navigation, optimized off-canvas menus and admin components that work well on smartphones and tablets.

## Motivation

- The current template (`Templates/Angular/Eaf.ProjectName.UI`) is based on legacy Metronic, with `m-grid--desktop` structure and few custom `@media` rules.
- Only `chat-bar.component.css` has a specific mobile media query (`max-width: 576px`); other components rely on the Metronic bundle.
- ASP.NET Zero uses **Metronic 8 + Bootstrap 5**, offering native responsive grid, 13+ themes, dark mode and mobile-first components.
- End users increasingly access dashboards and approvals from mobile devices.

## Current State

- Angular: `^20.3.26`.
- UI libraries: `primeng ^17.17.0`, `ngx-bootstrap ^12.0.0`, `ngx-scrollbar`.
- CSS/Layout: per-theme minified `style.bundle.css`, Metronic classes (`m-header`, `m-aside-left`, `m-wrapper`), mix of Bootstrap 4 / legacy Metronic.
- Sidebar: `m-aside-left` is fixed on desktop; mobile open/close control is limited.
- Chat: `chatSideRight` already has `100vw` on mobile, but header, footer and friend list are not touch-optimized.
- Tables/admin: `primeng-datatable-container` requires horizontal scroll on small screens.

## Proposed Changes

### 1. Adopt Bootstrap 5 + Metronic 8 (or custom EAF design system)
- Replace legacy Metronic bundle with **Metronic 8 CSS/SASS** or **Bootstrap 5 + custom EAF theme**.
- Reuse Bootstrap 5 variables (`--bs-breakpoint-sm|md|lg|xl`) for consistent responsiveness.
- Keep EAF identity (`#FF7020`) through CSS variables.

### 2. Refactor Layout Components
- `default-layout.component.html`, `theme2/3/4-layout.component.html`:
  - Replace `m-stack--desktop` and `m-grid--ver-desktop` with Flex / CSS Grid.
  - Add conditional off-canvas classes: `offcanvas offcanvas-start` for sidebar and `offcanvas offcanvas-end` for panels (chat, notifications).
- `topbar.component.html`:
  - Move menu items into a hamburger menu on screens < 992px.
  - Group notifications, chat and profile into a bottom navigation or compact top dropdown.
- `side-bar-menu.component.ts/html`:
  - Turn sidebar into a mobile drawer with swipe gestures and overlay.
  - Add floating toggle to open/close.

### 3. Breakpoints
```css
@media (max-width: 575.98px) { /* portrait phones */ }
@media (min-width: 576px) and (max-width: 991.98px) { /* tablets */ }
@media (min-width: 992px) { /* desktop */ }
```
- Mobile (< 576px): fixed top header, hidden sidebar, main content with safe padding for bottom navigation.
- Tablet (576–991px): collapsed icon sidebar, expanded header.
- Desktop (>= 992px): expanded sidebar, current layout.

### 4. Admin Components
- Tables: use `p-table` with `responsiveLayout="scroll"` or `stack`.
- Forms: stack fields on mobile (`col-12` by default, `col-md-6` / `col-lg-4` on desktop).
- Modals: ensure `p-dialog` uses 100% viewport on mobile.
- Chat: keep `chatSideRight` 100vw, adjust newly created header skins and message inputs for virtual keyboard.

### 5. Touch Navigation
- Increase touch targets (>= 44x44px).
- Add gesture support for sidebar and chat (`hammerjs` is already in `eaf-web-resources`).
- Avoid hover-only tooltips/dropdowns on touch.

### 6. Mobile Accessibility
- Ensure `viewport` meta, `touch-action`, visible focus on fields.
- Test with screen readers and external keyboard navigation.

## Implementation Status (2026-08)

Partial. Some responsive CSS exists in `styles.css`, but no mobile-first off-canvas layout, bottom navigation or comprehensive breakpoint system. `m-stack`, `m-grid` and `m-aside-left` classes still dominate the layout.

## Migration Plan
1. **Phase 1 — Inventory**: list all layout and admin components that need adjustment.
2. **Phase 2 — Spike**: create an alternative mobile-first theme (e.g. `theme13`) without affecting current ones.
3. **Phase 3 — Components**: adjust `layout`, `topbar`, `side-bar-menu`, `chat-bar`, tables and modals.
4. **Phase 4 — Tests**: test on real devices/emulators, Cypress/Playwright with mobile viewports.
5. **Phase 5 — Rollout**: make the new layout default and gradually deprecate old themes.

## Impact
- **High**: changes layout markup and CSS.
- **Medium**: may require e2e test adjustments.
- **Low**: business rules and APIs are not affected.

## Risks
- Legacy Metronic bundle is large and minified; incorrect changes can break global CSS.
- Multiple themes (12) increase test cost.
- `ngx-bootstrap` may conflict with Bootstrap 5 JS; evaluate migration or `ng-bootstrap`.

## References
- <https://aspnetzero.com/angular> — Metronic 8, Bootstrap 5, 13+ themes, dark mode.
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout` — current layout components.
- `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/themes` — per-theme CSS bundles.
