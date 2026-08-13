# EAF Angular — Remaining Modernization Features

## Goal

Consolidate the remaining Angular EAF template modernization features, ordered from simplest to most complex, with feasibility and scope analysis.

## Features Status

### 1. Dark Mode and Design System

**Goal:** Add a dark theme and CSS design-token system to Angular, replacing Metronic hardcoded colors with CSS variables.

**Current proposed scope:**
- Create `theme-variables.scss` / `styles.css` with tokens for surface, text, border, primary, status and accent colors.
- Extend `Header/Theme/UiCustomization` DTOs to expose `isDarkMode` and `themeName`.
- Add a header toggle to switch between `light` and `dark`.
- Persist preference via `localStorage` and optionally `UserPreferences` backend.
- Adjust `chat-bar.component.css` and all layouts to use `var(--eaf-surface)` etc.

**Implementation status (2026-08):** Not started. No `themeMode`, `isDarkMode`, `data-theme` or `prefers-color-scheme` references found in `Templates/Angular/Eaf.ProjectName.UI/src`.

**Makes sense?** Yes. Common demand; improves accessibility. Medium complexity.

---

### 2. PrimeNG Component Modernization

**Goal:** Replace legacy `ngx-bootstrap` and Metronic widgets with native PrimeNG 17 components (`p-table`, `p-menu`, `p-dropdown`, `p-dialog`, `p-toast`, `p-inputswitch`).

**Current proposed scope:**
- Replace `BsDropdownModule` menus with `p-menu` / `p-tieredmenu`.
- Replace `ngx-bootstrap` modals with `p-dialog`.
- Replace manual tables with `p-table` using `responsiveLayout="scroll"`.
- Consolidate `p-paginator` and `p-confirmDialog`.
- Add `p-toast` for notifications and remove custom `notify`.

**Implementation status (2026-08):** Partial. `p-table`, `p-dialog`, `p-paginator` and `p-fileUpload` are already used in admin pages; `ngx-bootstrap` (`ModalModule`, `TabsModule`, `BsDropdownModule`, `TooltipModule`, `PopoverModule`) is still imported in `app.module.ts` and used across many components.

**Makes sense?** Yes. `package.json` already lists `primeng ^17.17.0`. Medium-high complexity because of Metronic styles.

---

### 3. Metronic 8 + Bootstrap 5 Migration

**Goal:** Update the Angular template layout to Metronic 8 with Bootstrap 5, abandoning legacy classes (`m-grid`, `m-stack`, `m-portlet`) in favor of `row`, `col`, `card`, `navbar`, `offcanvas`.

**Current proposed scope:**
- Replace Metronic 5/7 `style.bundle.css` with Metronic 8 assets or a custom design system.
- Refactor `default-layout`, `theme2-layout`, `theme3-layout`, `theme4-layout` to Bootstrap 5 structure.
- Create reusable components: `app-card`, `app-page-header`, `app-offcanvas-menu`.
- Ensure mobile responsiveness with native `offcanvas` and Bootstrap breakpoints.

**Implementation status (2026-08):** Not started. Layouts still use `m-stack`, `m-grid`, `m-aside-left` and `style.bundle.css` per theme.

**Makes sense?** Only if a Metronic 8 license exists. Without a license, build an incremental custom design system. High complexity.

---

### 4. Backend Modularization

**Goal:** Create the missing backend EAF modules and standardize existing ones.

**Current proposed scope:**
- `Eaf.BlobStoring` — file-storage abstraction (Azure Blob, S3, local).
- `Eaf.HtmlSanitizer` — HTML sanitization for chat/notifications.
- `Eaf.OpenIddict` — OpenID Connect provider ( `ExternalLoginProviderInfo` exists but has no implementation).
- `Eaf.Dapper` — Dapper repositories for complex queries.
- `Eaf.FluentValidation` — fluent validation in Application Services.
- Standardize existing `MailKit` and `Redis` into well-defined modules.

**Implementation status (2026-08):** Not started in `src/`. Payment gateway abstraction was implemented; backend modularization items are still pending.

**Makes sense?** `BlobStoring`, `HtmlSanitizer` and `OpenIddict` have high value. `Dapper` and `FluentValidation` depend on real demand. High complexity.

---

### 5. ABP Feature Parity

**Goal:** Bring EAF closer to modern ABP Framework features.

**Current proposed scope:**
- `Eaf.BlobStoring` (also listed under modularization).
- MongoDB support (`Eaf.Middleware.MongoDB`).
- Background jobs with Quartz (`Eaf.Quartz`).
- OData controllers for admin entities.
- Enhanced feature system (Editions/Feature values).
- OpenIddict/OAuth2 server.

**Implementation status (2026-08):** Not started. Edition CRUD exists but feature-value/pricing integration is missing.

**Makes sense?** MongoDB and Quartz make sense for large projects. OData and OpenIddict depend on roadmap. Very high complexity.

---

### 6. PWA and Offline

**Goal:** Complete the Progressive Web App setup.

**Current proposed scope:**
- Verify `ngsw-config.json` and `manifest.json` are production-ready.
- Add offline banner and action queue.
- Implement push notifications backend and frontend.
- Add install prompt handling.

**Implementation status (2026-08):** Partial. `ngsw-config.json`, `ServiceWorkerModule.register('ngsw-worker.js')`, `manifest.json` and `angular.json` PWA assets are present, but no offline queue, push notifications or install prompt logic was found in `src/app`.

**Makes sense?** Yes. Improves mobile UX. Medium complexity.

---

## Recommended Priority

1. Dark Mode and Design Tokens
2. PrimeNG Modernization
3. Metronic 8 + Bootstrap 5 (or custom design system)
4. Backend Modularization (`BlobStoring`, `HtmlSanitizer`, `OpenIddict`)
5. ABP Feature Parity
6. PWA Completion

## General Acceptance Criteria

- Each feature must have its own detailed spec before implementation.
- Angular build (`ng build --configuration=production`) without errors.
- .NET build (`dotnet build Eaf.sln`) without errors.
- Unit/xUnit tests passing.
- Minimum 90% coverage for new backend code.

## Notes

- Features 1, 2, 3 and 8 are still open. This spec documents the remaining work.
- Implement one feature at a time, validating CI before moving on.
