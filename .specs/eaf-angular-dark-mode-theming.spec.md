# EAF Angular — Dark Mode and Theme System

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Dark Mode and Theme System |
| Product / System | EAF Angular Template |
| Module / Bounded Context | UI / Theming |
| Change type | Feature / Frontend |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-dark-mode` |
| Technical owner | Frontend Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF Angular has 12 light skin themes (`theme2` to `theme12`) that only change header/sidebar colors. There is no global dark mode, which is an expected enterprise feature and a key gap versus ASP.NET Zero's 13+ themes including dark mode.

### Objective

Introduce a global dark mode and a CSS Custom Property-based theme system that can be persisted per user and applied without a page flash.

### Expected outcome

- `themeMode` (`light` | `dark` | `system`) persisted and respected.
- All core UI components render correctly in dark mode.
- PrimeNG components use the same design tokens.

### Out of scope

- Full Metronic 8 migration.
- Redesign of the 12 existing skins (keep them as light variants).

## 2. Agent Role

Senior Angular/UX engineer. Prefer CSS variables, avoid hardcoded colors, preserve existing behavior.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

The Angular template (`Templates/Angular/Eaf.ProjectName.UI`) uses legacy Metronic CSS and PrimeNG 17. Hardcoded colors are present in `styles.css`, `customize.css`, and component styles.

### Relevant stack

- Angular 20, TypeScript 5.8, PrimeNG 17.17.0
- `ngx-bootstrap`, jQuery, Metronic legacy

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles
Templates/Angular/Eaf.ProjectName.UI/src/app/shared/common/ui/app-ui-customization.service.ts
Templates/Angular/Eaf.ProjectName.UI/src/shared/service-proxies/service-proxies.ts
Templates/Angular/Eaf.ProjectName.UI/src/app/app.module.ts
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
- `.specs/eaf-angular-modern-primeng-components.spec.md`

## 5. Task Definition

### Main task

Add a global dark mode/theme system based on CSS Custom Properties.

### Subtasks

- Define design tokens.
- Add `themeMode` to backend DTOs.
- Apply `data-theme` attribute to `<html>` before bootstrap.
- Update critical components and PrimeNG theme to use tokens.
- Add a user-accessible toggle.

### Do not do

- Do not hand-edit `service-proxies.ts`.
- Do not remove the 12 existing skins (keep as legacy light variants).

## 6. Functional Requirements

### FR-001: Design tokens

**Description:** Replace hardcoded colors with semantic CSS variables (`--eaf-bg`, `--eaf-text`, `--eaf-primary`, etc.).

**Acceptance criteria:**

- [ ] Base variables exist for background, surface, text, border, primary.
- [ ] Dark variant overrides all base variables.

### FR-002: Mode persistence

**Description:** Add `themeMode` to `UiCustomizationSettingsDto`, save to `localStorage`, and apply before Angular bootstrap.

**Acceptance criteria:**

- [ ] `themeMode` added to backend DTO and settings.
- [ ] Theme applied in `main.ts` or `app.module.ts` before first paint to avoid flash.
- [ ] `prefers-color-scheme` respected when `themeMode === 'system'`.

### FR-003: Component adaptation

**Description:** Critical components (tables, modals, chat, forms, dashboard) must use tokens.

**Acceptance criteria:**

- [ ] `styles.css`, `customize.css`, `chat-bar.component.css` updated.
- [ ] PrimeNG tables and dialogs render correctly in dark mode.

### FR-004: Theme toggle

**Description:** Add a toggle in the header or user profile menu.

**Acceptance criteria:**

- [ ] Toggle updates `data-theme` and persists choice.
- [ ] Toggle is keyboard and screen-reader accessible.

## 7. Business Rules

### BR-001: Backward compatibility

Existing 12 light skins continue to work unchanged when dark mode is disabled.

### BR-002: System preference

When `themeMode` is `system`, the app follows `prefers-color-scheme` and updates when the OS changes.

## 8. Domain Modeling

N/A — frontend only.

## 9. Expected Architecture

- CSS Custom Properties in global styles.
- Settings persisted via ABP `ISettingManager` and `localStorage`.
- Theme class applied to `<html>` / `<body>`.

## 10. API Contracts

N/A — no new endpoints.

## 11. Application Contracts

N/A.

## 12. Persistence and Data

### Settings

- `themeMode` string added to `UiCustomizationSettingsDto`.
- NSwag regeneration required to update `service-proxies.ts`.

## 13. Integrations

N/A.

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Invalid themeMode | Unknown value | Fallback to `light` |
| User switches OS mode while app open | system + prefers-color-scheme change | Update live if listener is implemented |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

No FOIT/FOUT. Theme applied before first paint.

### Security

Do not expose user preference as sensitive data.

### Accessibility

Toggle must have visible focus and ARIA label.

### Maintainability

Document token naming convention and how to add new skins.

## 17. Mandatory Guardrails

Do not hand-edit generated files; preserve existing skins; do not add dependencies without justification.

## 18. Expected Tests

| Component / Flow | Scenarios |
|---|---|
| `app-ui-customization.service.ts` | Load/save `themeMode` |
| `AppComponent` | Apply `data-theme` on init |
| Dark mode e2e | Toggle, system preference, persistence |

## 19. Acceptance Criteria

- [ ] Dark mode renders correctly on key pages.
- [ ] `themeMode` persists across reloads.
- [ ] Existing light skins still work.
- [ ] Tests updated and passing.

## 20. Implementation Plan

1. Define CSS design tokens and `data-theme` selectors.
2. Add `themeMode` to backend DTO and regenerate proxies.
3. Apply theme before bootstrap.
4. Refactor critical components to use tokens.
5. Add header toggle.
6. Test across skins.

## 21. Rollback Strategy

- Revert `data-theme` attribute and CSS variables.
- Keep new DTO field optional to avoid breaking clients.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Legacy Metronic CSS overrides tokens | High | High | Test and scope token usage incrementally |
| Generated service-proxies.ts out of sync | Medium | Low | Regenerate via NSwag |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Tokens defined and applied.
- [ ] Persistence works.
- [ ] Tests pass.

## 24. Key Reminder

> The SPEC is the contract. Do not redesign the entire UI; focus on token-based dark mode.

## Current State

- Hardcoded colors in `styles.css`, `customize.css`, `chat-bar.component.css`.
- `AppUiCustomizationService` has `header.headerSkin` and `menu.asideSkin` but no `themeMode`.
- No `themeMode`, `isDarkMode`, `data-theme`, or `prefers-color-scheme` references found in `src/`.

## Proposed Design Tokens

```css
:root {
  --eaf-bg: #ffffff;
  --eaf-surface: #f8f9fa;
  --eaf-text: #212529;
  --eaf-text-muted: #6c757d;
  --eaf-border: #dee2e6;
  --eaf-primary: #FF7020;
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

## References
- <https://primeng.org/theming>
- <https://getbootstrap.com/docs/5.3/customize/color-modes/>
- `Templates/Angular/Eaf.ProjectName.UI/src/shared/service-proxies/service-proxies.ts`
