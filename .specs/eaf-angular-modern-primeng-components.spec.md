# EAF Angular — Modernize Components with PrimeNG

## Summary

Standardize the EAF Angular template on **PrimeNG 17+** components, reducing the use of `ngx-bootstrap`, jQuery and legacy widgets, and leveraging new features such as Tailwind-aware theming, unstyled mode, inline messages and improved accessibility.

## Motivation

- `package.json` already includes `primeng ^17.17.0`, but many components still use `ngx-bootstrap` (`^12.0.0`) and custom styles.
- PrimeNG 17 introduces a new theming API, better accessibility and unstyled components.
- ASP.NET Zero Angular uses PrimeNG as the base UI library.
- Less dependency on jQuery/Bootstrap JS simplifies maintenance and SSR/PWA.

## Current State

- `primeng` is present: `p-fileUpload`, `p-table` (used in datatables), `p-dialog` partially.
- `ngx-bootstrap` is used for: dropdown, datepicker, modal, tooltip, tabs, accordion.
- Forms: mix of plain HTML + Bootstrap classes + `m-form` Metronic.
- Datepicker: `ngx-bootstrap` and `bs-datepicker` assets are present.
- Modals: Bootstrap modal + `m-modal` + `p-dialog`.

## Proposed Changes

### 1. Replace ngx-bootstrap with PrimeNG
| ngx-bootstrap | PrimeNG 17+ |
|---------------|-------------|
| Dropdown | `p-dropdown` / `p-splitButton` |
| Datepicker | `p-calendar` |
| Modal | `p-dialog` |
| Tooltip | `p-tooltip` |
| Tabs | `p-tabView` |
| Accordion | `p-accordion` |
| Typeahead | `p-autoComplete` |

### 2. Standardize Forms
- Use `p-inputText`, `p-inputNumber`, `p-inputTextarea`, `p-checkbox`, `p-radioButton`, `p-toggleButton`.
- Integrate with `ReactiveFormsModule` and Angular validations.
- Replace `m-form` / `form-group form-md-line-input` with `p-fluid` + responsive grid.

### 3. Tables
- Replace custom `primeng-datatable-container` with `p-table` using `responsiveLayout="stack"` or `scroll`.
- Add native `p-paginator` and lazy loading.
- Use `p-columnFilter` for inline filters.

### 4. PrimeNG Themes
- Migrate from legacy `theme.css` to the new PrimeNG 17 theming system (`Aura`, `Lara`, `Material`, `Bootstrap` or a custom EAF theme).
- Create `theme-eaf` with design tokens (colors, radius, spacing, typography).

### 5. Accessibility
- Use `aria-*` attributes and roles provided by PrimeNG components.
- Ensure visible focus and keyboard navigation.

## Implementation Status (2026-08)

Partial. `p-table`, `p-dialog`, `p-paginator`, `p-fileUpload` and `p-progressbar` are already used in admin pages. `ngx-bootstrap` (`ModalModule`, `TabsModule`, `BsDropdownModule`, `TooltipModule`, `PopoverModule`) is still imported in `app.module.ts` and widely used across the application.

## Migration Plan
1. Inventory all `ngx-bootstrap` usages in `src/app`.
2. Create replacement examples in critical admin components (Users, Roles, Tenants).
3. Migrate generic components (dropdowns, datepickers, modals).
4. Update CSS for the new PrimeNG theming system.
5. Run visual and functional regression tests.

## Impact
- **Medium/High**: many UI components must be rewritten.
- **High**: improves accessibility and consistency.
- **Low/Medium**: may reduce bundle size by removing `ngx-bootstrap`.

## Risks
- API differences between `ngx-bootstrap` and PrimeNG require testing.
- Projects generated from the EAF template may depend on `m-form` styles.
- Need to maintain compatibility with `localize` pipe and existing services.

## References
- <https://primeng.org/installation> — PrimeNG 17 theming and unstyled mode.
- ASP.NET Zero Angular UI uses PrimeNG as base.
- `Templates/Angular/Eaf.ProjectName.UI/package.json`
