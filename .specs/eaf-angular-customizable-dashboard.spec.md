# EAF — Angular Customizable Dashboard

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Angular Customizable Dashboard |
| Product / System | EAF Angular Template |
| Module / Bounded Context | Dashboard / UI |
| Change type | Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-customizable-dashboard` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

EAF has a `DashboardAppService` and a `main/dashboard` page, but the dashboard is static. ASP.NET Zero provides a customizable dashboard where users can add, remove, and configure widgets and filters at runtime.

### Objective

Implement a customizable dashboard in the Angular template that consumes a server-side dashboard definition and renders widgets and filters based on permissions and tenant settings.

### Expected outcome

- Server-side `DashboardCustomization` domain with `WidgetDefinition`, `FilterDefinition`, `DashboardConfiguration`.
- `DashboardCustomizationAppService` to get user dashboard state and widget data.
- Angular `customizable-dashboard` component, `dashboard-view-configuration.service.ts`, widget/filter registry.
- Example widgets (host stats, tenant stats, recent notifications).

### Out of scope

- Report/dashboard designer drag-and-drop (use simple add/remove/ordering).
- Complex third-party chart libraries beyond Chart.js.

## 2. Agent Role

Senior Angular + .NET/ABP engineer. Follow ASP.NET Zero customizable dashboard docs and adapt to PrimeNG 17.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not remove the existing static dashboard; make customization opt-in.

## 4. Product Context

### Functional context

Users with dashboard permission can customize their main dashboard by selecting widgets and filters. Widgets call dedicated application service endpoints.

### Technical context

- `DashboardAppService` exists in `Eaf.Middleware.Application`.
- Angular `main/dashboard` exists.
- ASP.NET Zero docs define server-side `DashboardConfiguration.cs` and Angular `DashboardCustomizationConsts.ts`.

### Relevant stack

- Angular 20 / TypeScript 5.8 / PrimeNG 17
- C# 14 / .NET 10 / ABP 10.5
- Chart.js

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/src/app/main/dashboard/
src/Eaf.Middleware.Application/Dashboard/
src/Eaf.Middleware.Core/Dashboard/
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Application/Dashboard/DashboardAppService.cs`
- `.specs/eaf-aspnetzero-docs-gap-analysis.spec.md`

## 5. Task Definition

### Main task

Implement customizable dashboard server and Angular UI.

### Subtasks

1. Create `DashboardCustomization` domain and `DashboardConfiguration`.
2. Create `DashboardCustomizationAppService`.
3. Add `WidgetDefinition`, `FilterDefinition`, `WidgetOutputDto`.
4. Build Angular `customizable-dashboard` component.
5. Build `dashboard-view-configuration.service.ts` and widget/filter registry.
6. Add example widgets and filters.
7. Add tests.
8. Add README or docs.

### Do not do

- Do not remove the existing static dashboard.
- Do not implement a full BI designer.

## 6. Functional Requirements

### FR-001: Widget and filter definitions

**Description:** Server defines available widgets and filters with permissions and multi-tenancy side.

**Rules:**

- `WidgetDefinition` has `Id`, `Name`, `Permission`, `ViewName`.
- `FilterDefinition` has `Id`, `Name`, `ComponentName`.
- `DashboardConfiguration` lists default widgets and filters per user/tenant.

**Acceptance criteria:**

- [ ] Server returns list of available widgets/filters.
- [ ] Only permitted widgets are shown.

### FR-002: User dashboard state

**Description:** Store and retrieve user-specific dashboard layout (which widgets, filters, order).

**Rules:**

- Store in `AbpSettings` or a new `DashboardCustomization` table.
- Fallback to default configuration.

**Acceptance criteria:**

- [ ] User changes persist across sessions.
- [ ] Default config used when user has no custom state.

### FR-003: Widget data endpoints

**Description:** Each widget calls a dedicated endpoint to fetch data.

**Rules:**

- Endpoint receives filter values from `DashboardCustomizationAppService.GetWidgetData`.
- Output is a generic `WidgetOutputDto` with JSON payload.

**Acceptance criteria:**

- [ ] Widgets render data from API.
- [ ] Filters update widget data.

### FR-004: Angular widget registry

**Description:** Angular maps widget/filter ids to components via a registry.

**Rules:**

- Lazy-load widget components.
- Use PrimeNG cards/panels.

**Acceptance criteria:**

- [ ] New widget can be added by registering id and component.

## 7. Business Rules

### BR-001: Permissions

Widgets are filtered by user permissions and multi-tenancy side (host vs tenant).

### BR-002: Default configuration

A default dashboard is provided and can be overridden per tenant.

## 8. Domain Modeling

### Bounded Context

Dashboard / Reporting

### Entities

| Entity | Identity | Responsibility |
|---|---|---|
| `DashboardConfiguration` | `int` or setting | Stores widget/filter layout per user/tenant |
| `WidgetDefinition` | value object | Server definition of a widget |
| `FilterDefinition` | value object | Server definition of a filter |

### Domain Events

N/A.

## 9. Expected Architecture

```text
src/Eaf.Middleware.Core/DashboardCustomization/
  Definitions/
    DashboardConfiguration.cs
    WidgetDefinition.cs
    FilterDefinition.cs
src/Eaf.Middleware.Application/DashboardCustomization/
  DashboardCustomizationAppService.cs
  IDashboardCustomizationAppService.cs
  Dto/
    DashboardOutputDto.cs
    WidgetOutputDto.cs
Templates/Angular/Eaf.ProjectName.UI/
  shared/common/customizable-dashboard/
    customizable-dashboard.component.ts
    dashboard-view-configuration.service.ts
    widget-filter-registry.ts
    widgets/
    filters/
```

## 10. API Contracts

### Get dashboard

```http
GET /api/services/app/DashboardCustomization/GetDashboard
```

```json
{
  "widgets": [
    { "id": "HostStats", "name": "Host Stats", "permission": "Pages.Host.Dashboard" }
  ],
  "filters": [
    { "id": "DateRange", "name": "Date Range" }
  ],
  "userConfiguration": { "widgetIds": ["HostStats"], "filterValues": {} }
}
```

### Save user configuration

```http
POST /api/services/app/DashboardCustomization/SaveUserConfiguration
```

## 11. Application Contracts

```csharp
public interface IDashboardCustomizationAppService : IApplicationService
{
    Task<DashboardOutputDto> GetDashboardAsync();
    Task<WidgetOutputDto> GetWidgetDataAsync(string widgetId, Dictionary<string, object> filters);
    Task SaveUserConfigurationAsync(UserDashboardConfigurationDto input);
}
```

## 12. Persistence and Data

### Persisted entities

| Table / Setting | Purpose |
|---|---|
| `Dashboard.UserConfiguration` (setting) | Per-user dashboard state |
| `Dashboard.TenantDefault` (setting) | Per-tenant default config |

### Migration required

No — use `ISettingManager` for state.

### Compatibility

- [ ] Existing dashboard table unaffected.

## 13. Integrations

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `ISettingManager` | Persist config | EF Core | default | no |
| `DashboardAppService` | Host/tenant data | In-process | default | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Widget not registered in Angular | unknown id | Log warning and show placeholder |
| User has no permission | unauthorized widget | Do not include in server response |
| Invalid filter value | wrong date format | Validation error 400 |

## 15. Few-Shot Examples

### Example 1: Add widget

```typescript
widgetRegistry.register('HostStats', HostStatsWidgetComponent);
```

### Example 2: Filter change

```typescript
filterService.publish('dateRange', { start: '2026-01-01', end: '2026-12-31' });
```

## 16. Non-Functional Requirements

### Performance

- Dashboard loads < 500 ms.
- Widget data < 300 ms each.

### Security

- Filter values are validated server-side.
- Widget data endpoints respect permissions.

### Maintainability

- README.md with widget/filter authoring guide.

## 17. Mandatory Guardrails

- Do not remove existing static dashboard.
- Do not expose widget data without permission checks.
- Do not execute filter values as code.

## 18. Expected Tests

### Backend tests

| Class | Scenarios |
|---|---|
| `DashboardConfiguration` | Filter by permission, default config |
| `DashboardCustomizationAppService` | Get/save config, widget data |

### Frontend tests

| Component | Scenarios |
|---|---|
| `CustomizableDashboardComponent` | Render widgets, save layout |
| `DashboardViewConfigurationService` | Register widgets/filters |

## 19. Acceptance Criteria

- [ ] Server returns dashboard definition and widget data.
- [ ] Angular customizable dashboard page works.
- [ ] User can add/remove/ordering widgets.
- [ ] Filters update widgets.
- [ ] Existing tests pass.

## 20. Implementation Plan

1. Design server domain and app service.
2. Implement backend.
3. Build Angular component and registry.
4. Add example widgets and filters.
5. Add tests and docs.
6. Update index.

## 21. Rollback Strategy

- Revert to static dashboard by removing customization component.
- Keep existing dashboard data.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Widget registry coupling | Medium | Medium | Use string ids and lazy loading |
| Stateful dashboard complexity | Medium | Medium | Start with host/tenant defaults |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Backend and Angular implemented.
- [ ] Tests pass.
- [ ] Index updated.

## 24. Key Reminder

> Keep the existing static dashboard. Customization is opt-in and permission-aware. Start with a small set of widgets.
