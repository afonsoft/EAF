# EAF Angular — Dark Mode and Theme System

## Summary

Add native dark-mode support and a consistent theme system based on CSS Custom Properties, so EAF can offer the same customization flexibility found in ASP.NET Zero (13+ themes including dark mode).

## Motivation

- ASP.NET Zero advertises "13+ Theme Options with Dark Mode".
- EAF has 12 themes (`theme2` to `theme12`), but they are light skin variations (`header-dark`, `header-color`, `header-light`); no global dark mode exists.
- Dark mode reduces eye strain and is expected in modern enterprise apps.
- Eases integration with Metronic 8 / PrimeNG 17 theming.

## Current State

- Hardcoded colors in `styles.css`, `customize.css` and `chat-bar.component.css`.
- `header-{{ skin }}` and `m-aside-left--skin-{{ skin }}` only change header/sidebar, not the overall theme.
- Limited CSS variables: `--primary: #FF7020` is defined in `style.bundle.css`.
- `AppUiCustomizationService` exposes `baseSettings` with `header.headerSkin` and `menu.asideSkin`, but no `themeMode`.

## Proposed Changes

### 1. CSS Design Tokens
Create semantic variables:
```css
:root {
  --eaf-bg: #ffffff;
  --eaf-surface: #f8f9fa;
  --eaf-text: #212529;
  --eaf-text-muted: #6c757d;
  --eaf-border: #dee2e6;
  --eaf-primary: #FF7020;
  --eaf-header-bg: #37322d; /* variable per skin */
}

[data-theme="dark"] {
  --eaf-bg: #1e1e2d;
  --eaf-surface: #2b2b40;
  --eaf-text: #f5f5f5;
  --eaf-text-muted: #a1a5b7;
  --eaf-border: #2b2b40;
  --eaf-primary: #ff8f4f;
}
```

### 2. Mode Persistence
- Add `themeMode` (`light` | `dark` | `system`) to `UiCustomizationSettingsDto`.
- Save preference to `localStorage` and apply in `app.module.ts` / `main.ts` before bootstrap to avoid flash.
- Respect `prefers-color-scheme: dark` when `themeMode === 'system'`.

### 3. Adapt Components
- Replace hardcoded backgrounds and text with variables (`var(--eaf-bg)`, `var(--eaf-text)`).
- Create dark theme for PrimeNG components (`p-table`, `p-dialog`, `p-calendar`).
- Adjust `chat-bar`, tables, modals, forms and dashboards.

### 4. Theme Toggle
- Add a toggle in the header (profile or settings) to switch light/dark.
- Update `data-theme` class on `<html>` or `<body>`.

### 5. Consolidate Themes
- Reduce 12 minified CSS theme bundles to a token-based system plus skin overrides.
- Keep `dark`, `light`, `color` skins for header/sidebar.

## Implementation Status (2026-08)

Not started. No `themeMode`, `isDarkMode`, `data-theme` or `prefers-color-scheme` references found in `Templates/Angular/Eaf.ProjectName.UI/src`.

## Migration Plan
1. Define design tokens and add `themeMode` to the customization DTO.
2. Create base CSS with variables and `data-theme` classes.
3. Refactor critical components to use variables.
4. Implement toggle and persistence.
5. Test across all 12 themes + dark mode.

## Impact
- **High**: changes a large part of CSS and markup.
- **Medium**: backend (`UiCustomizationSettingsDto`) needs a new field.
- **High**: improves UX and modernizes appearance.

## Risks
- Old themes may break if variables are not applied correctly.
- `service-proxies.ts` is NSwag-generated; DTO changes require regeneration.
- Chart colors (`chart.js`) must become dynamic.

## References
- <https://primeng.org/theming> — unstyled + styled theming.
- <https://getbootstrap.com/docs/5.3/customize/color-modes/> — Bootstrap 5 dark mode.
- `Templates/Angular/Eaf.ProjectName.UI/src/shared/common/ui/app-ui-customization.service.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/shared/service-proxies/service-proxies.ts` — `UiCustomizationSettingsDto`
