# EAF Angular — Migrate from Legacy Metronic to Metronic 8 + Bootstrap 5

## Summary

Replace the legacy Metronic / Bootstrap 4 layout and components in the Angular template with **Metronic 8 + Bootstrap 5**, using SASS variables, responsive grid, CSS Custom Properties and a modern component set.

## Motivation

- Current template relies on minified `style.bundle.css` and `m-grid` / `m-stack` legacy classes.
- Bootstrap 5 offers native Offcanvas, Accordion, Grid, RTL, dark mode, and CSS variables.
- ASP.NET Zero Angular uses Metronic 8 with Bootstrap 5.
- Reduces custom JS, jQuery and legacy Metronic dependencies.

## Current State

- `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/style.bundle*.css` are minified builds.
- Bootstrap 4-ish classes (`m-grid`, `m-stack`, `m-portlet`, `m-form`) are widely used.
- Layout components: `default-layout`, `theme2-layout`, `theme3-layout`, `theme4-layout`.
- `ngx-bootstrap ^12.0.0` supplies modal/dropdown/datepicker/tabs; Bootstrap JS is present.

## Proposed Changes

### 1. Update Dependencies
- `bootstrap` from v4 to `^5.3.x`.
- `ngx-bootstrap` is not compatible with Bootstrap 5; replace with `ng-bootstrap` or PrimeNG components.
- `@popperjs/core` included automatically with Bootstrap 5.

### 2. Layout Rewrite
- Adopt Bootstrap 5 grid / utilities.
- Replace `m-grid--desktop`, `m-stack--desktop`, `m-aside-left` with:
  ```html
  <div class="d-flex flex-column flex-root">
    <div class="page d-flex flex-row flex-column-fluid">
      <aside id="kt_sidebar" class="sidebar ...">
      <main class="content d-flex flex-column flex-column-fluid">
  ```
- Use `offcanvas` for mobile sidebar.
- Use `container`, `container-fluid`, `row`, `col-*`.

### 3. CSS Custom Properties
- Use Bootstrap 5 CSS variables (`--bs-primary`, `--bs-body-bg`, etc.).
- Create EAF override file (`_variables.scss`) with the brand color `#FF7020`.

### 4. Replace ngx-bootstrap
- Modal → PrimeNG `p-dialog`.
- Tabs → `p-tabView`.
- Dropdown → `p-dropdown`.
- Datepicker → `p-calendar`.
- Tooltip → `p-tooltip`.

### 5. Metronic 8 Assets
- Replace `style.bundle*.css` with Metronic 8 `style.bundle.css` and `scripts.bundle.js`.
- Update `angular.json` assets if necessary.

### 6. Breakpoints and Spacing
- Use Bootstrap 5 classes (`g-3`, `p-3`, `d-none d-lg-block`).
- Remove custom `m--margin-*`, `m--padding-*` classes.

## Implementation Status (2026-08)

Not started. The template still uses legacy Metronic / Bootstrap 4 classes. `ngx-bootstrap ^12.0.0` remains in `package.json`.

## Migration Plan
1. Create a parallel branch with Bootstrap 5 and Metronic 8 assets.
2. Convert `default-layout` first as a pilot.
3. Migrate admin pages incrementally (Users, Roles, Tenants).
4. Replace `ngx-bootstrap` components one by one.
5. Run visual regression with Lighthouse and Percy.
6. Deprecate old CSS themes.

## Impact
- **Very high**: rewrites markup, CSS, layout and components.
- **High**: improves mobile responsiveness, reduces legacy debt.
- **Medium**: impacts all future template projects.

## Risks
- Bootstrap 5 changes many class names and JS APIs.
- `ngx-bootstrap` must be fully replaced before update.
- Many `m-*` utility classes are spread across the app.
- Custom theme files may need regeneration.

## References
- <https://getbootstrap.com/docs/5.3/getting-started/introduction/>
- <https://keenthemes.com/metronic/>
- `Templates/Angular/Eaf.ProjectName.UI/package.json`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout`
