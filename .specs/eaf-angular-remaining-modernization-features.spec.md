# EAF Angular — Remaining Modernization Features

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Angular Remaining Modernization Features |
| Product / System | EAF Angular Template |
| Module / Bounded Context | UI Modernization |
| Change type | Refactor / Frontend |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-modernization` |
| Technical owner | Frontend Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

The EAF Angular template is a mix of legacy Metronic, `ngx-bootstrap`, and partial PrimeNG adoption. Several modernization tracks (dark mode, PrimeNG, Bootstrap 5/Metronic 8, mobile-first, PWA) are tracked in separate SPECs but need a single coordination point.

### Objective

Coordinate the remaining Angular modernization tracks and report their implementation status in one place.

### Expected outcome

- Each modernization track has a dedicated SPEC.
- Status is updated monthly.
- Dependencies between tracks are explicit.

### Out of scope

- Implementation details (covered by child SPECs).

## 2. Agent Role

Technical coordinator. Synthesize child SPECs, track status, and update this SPEC.

## 3. Agent Autonomy Level

**0 — Research/Coordination**

## 4. Product Context

Angular template modernization is split across multiple SPECs due to its size.

### Relevant stack

- Angular 20, PrimeNG 17, Bootstrap 4/5, Metronic, `ngx-bootstrap`

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-dark-mode-theming.spec.md`
- `.specs/eaf-angular-modern-primeng-components.spec.md`
- `.specs/eaf-angular-metronic8-bootstrap5-migration.spec.md`
- `.specs/eaf-angular-mobile-responsive-layout.spec.md`
- `.specs/eaf-angular-pwa-offline.spec.md`
- `.specs/eaf-angular-accessibility-a11y.spec.md`

## 5. Task Definition

### Main task

Maintain a coordinated view of Angular template modernization.

### Subtasks

- Track status of each modernization track.
- Identify dependencies and ordering.
- Update this SPEC monthly.

### Do not do

- Do not implement code here; defer to child SPECs.

## 6. Functional Requirements

### FR-001: Status dashboard

**Description:** This SPEC must list each track with status, owner, and blocking dependencies.

**Acceptance criteria:**

- [ ] Table includes all modernization tracks.
- [ ] Status is one of `Not started`, `Partial`, `In progress`, `Completed`.

### FR-002: Dependency mapping

**Description:** Identify which tracks block others.

**Acceptance criteria:**

- [ ] Dark mode depends on PrimeNG theme tokens.
- [ ] Bootstrap 5 migration depends on `ngx-bootstrap` replacement.

## 7. Business Rules

### BR-001: No duplicate implementation

Implementation plans must live in child SPECs, not here.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

N/A.

## 10. API Contracts

N/A.

## 11. Application Contracts

N/A.

## 12. Persistence and Data

N/A.

## 13. Integrations

N/A.

## 14. Edge Cases and Error Scenarios

N/A.

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

- Keep the SPEC concise.
- Update monthly.

## 17. Mandatory Guardrails

Do not expand scope beyond the listed tracks.

## 18. Expected Tests

N/A.

## 19. Acceptance Criteria

- [ ] All Angular modernization tracks listed.
- [ ] Each links to a child SPEC.
- [ ] Status is current.

## 20. Implementation Plan

1. Create child SPECs for each track.
2. Link them here.
3. Update status monthly or after each release.

## 21. Rollback Strategy

N/A.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Tracks drift out of sync | Medium | High | Monthly update and review |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] All child SPECs linked.
- [ ] Status current.

## 24. Key Reminder

> The SPEC is the contract. This is a coordination SPEC, not an implementation SPEC.

## Modernization Tracks

| Track | Status (2026-08) | Child SPEC |
|---|---|---|
| Dark mode / theme system | Not started | `eaf-angular-dark-mode-theming.spec.md` |
| PrimeNG full adoption | Partial | `eaf-angular-modern-primeng-components.spec.md` |
| Bootstrap 5 / Metronic 8 | Not started | `eaf-angular-metronic8-bootstrap5-migration.spec.md` |
| Mobile responsive layout | Partial | `eaf-angular-mobile-responsive-layout.spec.md` |
| PWA / offline / push | Partial | `eaf-angular-pwa-offline.spec.md` |
| Accessibility (WCAG AA) | Not started | `eaf-angular-accessibility-a11y.spec.md` |
| Performance / bundle size | Partial | `eaf-performance-memory-optimization-plan.md` |

## Current State Summary

- `package.json`: Angular ^20.3.26, PrimeNG ^17.17.0, `ngx-bootstrap` ^12.0.0, `@angular/pwa`/`@angular/service-worker`.
- PWA service worker is registered in `app.module.ts` for production.
- `ngx-bootstrap` is still imported in `app.module.ts`.
- No dark mode implementation found.
- No mobile-first off-canvas layout.

## References

- See child SPECs listed above.
