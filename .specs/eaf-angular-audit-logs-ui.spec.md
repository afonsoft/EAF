# EAF — Angular Audit Logs and Entity History UI

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Angular Audit Logs and Entity History UI |
| Product / System | EAF Angular Template |
| Module / Bounded Context | Administration / Auditing |
| Change type | Refactor / Feature |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-audit-logs-ui` |
| Technical owner | Core Team |
| Status | Draft |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

`Templates/Angular/Eaf.ProjectName.UI/src/app/admin/audit-logs/` exists but uses legacy `ngx-bootstrap`/`Metronic` patterns. It does not show entity history (property-level changes) inline and lacks modern PrimeNG table, filtering, and export features.

### Objective

Modernize the Audit Logs page using PrimeNG 17 components, add an Entity History tab/modal for property-level changes, and keep the existing `AuditLogAppService` contracts.

### Expected outcome

- PrimeNG `p-table` with server-side sorting, paging, and filtering.
- Filter panel by date range, user, tenant, service name, method, execution duration, and error status.
- Audit log detail modal with request/response payloads.
- Entity History view showing `EntityChangeSet` / `EntityChange` / `EntityPropertyChange` data.
- Excel export via existing `AuditLogListExcelExporter`.

### Out of scope

- Real-time audit log streaming.
- Custom audit retention policies (DB maintenance).

## 2. Agent Role

Senior Angular + .NET engineer. Modernize UI while preserving backend contracts and permissions.

## 3. Agent Autonomy Level

**2 — Reliable**

Restrictions: do not change `AuditLogAppService` DTOs; do not remove existing route/permission.

## 4. Product Context

### Functional context

Host admins use `admin/audit-logs` to inspect user actions, errors, and entity changes. ASP.NET Zero provides a similar page with entity history.

### Technical context

- `AuditLogAppService` has `GetAuditLogs`, `GetAuditLogsToExcel`, and entity history endpoints.
- Existing `audit-logs.component.*` uses legacy table.
- PrimeNG 17 is partially adopted.

### Relevant stack

- Angular 20 / TypeScript 5.8 / PrimeNG 17
- C# 14 / .NET 10 / ABP 10.5

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/src/app/admin/audit-logs/
src/Eaf.Middleware.Application/Auditing/
```

### Context files the agent must read before implementation

- `src/Eaf.Middleware.Application/Auditing/AuditLogAppService.cs`
- `src/Eaf.Middleware.Application/Auditing/Dto/GetAuditLogsInput.cs`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/audit-logs/audit-logs.component.ts`

## 5. Task Definition

### Main task

Modernize the Angular Audit Logs page and add Entity History view.

### Subtasks

1. Refactor `audit-logs.component` to use PrimeNG `p-table` and `p-paginator`.
2. Add filter panel with PrimeNG `p-calendar`, `p-dropdown`, `p-inputText`.
3. Add `audit-log-detail-modal` improvements (syntax-highlighted JSON, copy button).
4. Add `entity-history` component or tab using `EntityChangeSet` data.
5. Add Excel export button.
6. Add component tests.

### Do not do

- Do not change `AuditLogAppService` DTOs.
- Do not remove existing route.
- Do not add backend migrations.

## 6. Functional Requirements

### FR-001: PrimeNG audit log table

**Description:** Replace legacy table with PrimeNG `p-table`.

**Rules:**

- Server-side paging, sorting, filtering.
- Columns: ExecutionTime, UserName, ServiceName, MethodName, ExecutionDuration, ClientIpAddress, BrowserInfo, ErrorState.
- Row click opens detail modal.

**Acceptance criteria:**

- [ ] Table renders with PrimeNG.
- [ ] Sorting and paging call backend.

### FR-002: Filter panel

**Description:** Provide filters for audit logs.

**Rules:**

- Date range, username, service name, method name, min/max duration, has error.
- Filters applied server-side.

**Acceptance criteria:**

- [ ] All filters map to `GetAuditLogsInput`.
- [ ] Reset button clears filters.

### FR-003: Audit log detail

**Description:** Show request/response/parameters and exception in a modal.

**Rules:**

- JSON formatted with line numbers.
- Copy-to-clipboard button.
- Show user and tenant info.

**Acceptance criteria:**

- [ ] Modal displays all `AuditLogListDto` fields.

### FR-004: Entity history

**Description:** Show property-level changes for entities.

**Rules:**

- New tab or modal for Entity History.
- Display `EntityChangeSet` > `EntityChange` > `EntityPropertyChange`.
- Filter by entity type and id.

**Acceptance criteria:**

- [ ] Entity history endpoint consumed.
- [ ] Old and new values visible.

### FR-005: Excel export

**Description:** Export current filtered results to Excel.

**Rules:**

- Use existing `AuditLogAppService.GetAuditLogsToExcel`.
- Download file in browser.

**Acceptance criteria:**

- [ ] Export respects current filters.

## 7. Business Rules

### BR-001: Permission

Only users with `Pages.Administration.AuditLogs` permission can access.

### BR-002: No PII leakage

Do not render raw passwords or tokens in request/response JSON.

## 8. Domain Modeling

N/A — UI SPEC.

## 9. Expected Architecture

```text
Templates/Angular/Eaf.ProjectName.UI/src/app/admin/audit-logs/
  audit-logs.component.ts (refactored)
  audit-logs.component.html
  audit-log-detail-modal.component.ts (refactored)
  entity-history/
    entity-history.component.ts
    entity-history.component.html
    entity-history-modal.component.ts
  services/
    audit-log.service.ts
```

## 10. API Contracts

Existing endpoints from `AuditLogAppService`:

```http
POST /api/services/app/AuditLog/GetAuditLogs
POST /api/services/app/AuditLog/GetAuditLogsToExcel
GET/POST /api/services/app/AuditLog/GetEntityChanges  (if exists, else add)
```

## 11. Application Contracts

No new application contracts unless entity history endpoint is missing.

## 12. Persistence and Data

No migration needed.

## 13. Integrations

### Internal services

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| `AuditLogAppService` | Get audit logs and entity changes | HTTP | default | no |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Large JSON payload | > 1 MB | Truncate display or offer download |
| Entity history disabled | `EntityHistory` not configured | Show informative message |
| No audit logs | empty result | Show empty state |

## 15. Few-Shot Examples

### Example 1: Filter by date and error

```typescript
this.input = {
  startDate: '2026-08-01',
  endDate: '2026-08-13',
  hasException: true
};
```

### Example 2: Entity history modal

```html
<app-entity-history [entityTypeName]="'User'" [entityId]="42"></app-entity-history>
```

## 16. Non-Functional Requirements

### Performance

- Table loads < 500 ms for 50 rows.
- JSON formatting on demand (not at table render).

### Security

- Sanitize rendered HTML.
- Do not show secrets.

### Accessibility

- WCAG 2.1 AA target (labels, focus states, keyboard navigation).

## 17. Mandatory Guardrails

- Do not change backend DTOs.
- Do not remove existing route/permissions.
- Do not expose secrets in UI.

## 18. Expected Tests

### Frontend tests

| Component | Scenarios |
|---|---|
| `AuditLogsComponent` | Load data, filter, sort, export |
| `AuditLogDetailModalComponent` | Render JSON, copy |
| `EntityHistoryComponent` | Load entity changes |

## 19. Acceptance Criteria

- [ ] Audit logs page uses PrimeNG table.
- [ ] Filters work server-side.
- [ ] Detail modal shows formatted JSON.
- [ ] Entity history view works.
- [ ] Export downloads Excel.
- [ ] Tests pass.

## 20. Implementation Plan

1. Refactor `audit-logs.component` to PrimeNG.
2. Add filter panel.
3. Improve detail modal.
4. Build entity history component.
5. Add export.
6. Add tests and update index.

## 21. Rollback Strategy

- Revert to previous component version from git.
- Keep backend unchanged.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---|---|
| Large JSON payloads slow UI | Medium | Medium | Lazy-load modal and truncate display |
| Entity history endpoint missing | Medium | Low | Add endpoint without changing schema |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] UI refactored and entity history added.
- [ ] Tests pass.
- [ ] Index updated.

## 24. Key Reminder

> Modernize the UI only. Preserve backend contracts and permissions. Use PrimeNG 17 patterns.
