# EAF — Workflow & Approval Engine (W3)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Workflow and Approval Builder |
| Product / System | EAF Middleware + Api/Angular Templates |
| Module / Bounded Context | New module `Eaf.Workflow` + template UI |
| Change type | Feature (multi-phase) |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-workflow-engine` |
| Technical owner | Core Team |
| Status | Completed|
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W3 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

ASP.NET Zero v15.5's headline feature is a built-in workflow and approval builder: tenant admins design request types (form + flow) visually; employees submit from a catalog; approvers decide from an inbox. EAF has no workflow capability.

### Objective

Implement an open-source workflow/approval engine as `Eaf.Workflow` module, in phases — engine + inbox first, visual designer after. Inspired by Zero's feature set, implemented from scratch.

### Expected outcome

- Phase A: workflow definition model (form schema + step graph), versioned publish, runtime instances, full audit timeline.
- Phase B: Approval Inbox, delegations, assignment rules (user/role/OU/manager), quorum (any/all/quorum), send-back-for-changes.
- Phase C: Angular visual designer + starter templates (Leave request, Expense claim, Purchase request).
- Phase D (optional, separate specs): scheduled/public triggers, webhooks, email/Teams actions, BPMN 2.0/Elsa import.

### Out of scope

- Copying Zero code/UI.
- Generic BPMN engine in Phase A–C (own step-graph model).
- AI-assisted authoring.

## 2. Agent Role

Senior .NET/ABP architect. This is the largest spec of the program — keep phases independently shippable (PR per phase minimum).

## 3. Agent Autonomy Level

**3 — Resilient** (may propose design refinements; still PR-gated).

## 4. Product Context

New top-level module `src/Eaf.Workflow` + `test/Eaf.Workflow.Tests` + template wiring. Follows existing module conventions (`Eaf.*Module` classes, `[DependsOn]`, EF Core, localization en + pt-BR, ≥90% coverage target).

### Context files

- `CLAUDE.md`
- `src/Eaf.Webhooks/` (closest structural model for a new module)
- `.specs/eaf-specs-index-and-roadmap-2026.md`
- Hangfire usage in `src/` (timers/reminders will need background jobs)

## 5. Task Definition

### Main task — Phase A (engine core)

- Domain: `WorkflowDefinition` (name, description, category, status draft/published/archived, version), `FormDefinition` (typed fields: text, number, money, date, selection, user, role, organization-unit, attachment), `FlowDefinition` (ordered step graph: start → approval/condition/parallel/timer/notification/subprocess → end), `WorkflowInstance` (requester, form values, current step, status), `WorkflowTask` (assignee resolution, decision), `WorkflowTimelineEntry` (every submission/decision/transition).
- Versioning: publishing bumps version; running instances keep the version they started on.
- Validation: publish-time validator naming the exact broken step; simulation endpoint showing the path a request would take + resolved assignees.
- App services: definitions CRUD + publish, catalog (start request), my requests, instance detail with timeline.

### Phase B (approvals)

- Approval Inbox app service + Angular page: pending tasks, approve/reject/send-back with comments.
- Assignment resolution: specific users, roles, organization units, requester's manager; quorum semantics (one/all/quorum N).
- Workflow delegations: hand tasks to a colleague for a period (integrate with existing user-delegations concept).
- SLA: due dates, reminders (Hangfire), escalations, business calendar.

### Phase C (designer)

- Angular visual form builder + flow designer (shared component pattern), template gallery seeded per tenant.
- Admin pages: Workflow Instances, Operations, Audit, basic process analytics.

### Do not do

- Do not let instance edits mutate published definitions (versioned).
- Do not execute external side effects in Phase A–C (webhooks/actions are Phase D).
- Do not bypass ABP permissions — every admin page permission-gated.

## 6. Functional Requirements (Phase A core)

- [ ] Tenant admin creates/edits/publishes a workflow definition.
- [ ] Validation rejects publish with exact-step diagnostics.
- [ ] Requester starts from catalog, fills form, submits → instance created.
- [ ] Simulation endpoint returns step path + resolved assignees without persisting.
- [ ] Timeline records every event.
- [ ] Old instances unaffected by republish.

## 7. Business Rules

- BR-001: Definitions are immutable once published; edits create a new version.
- BR-002: Instance runs against its starting version.
- BR-003: Send-back returns instance to requester preserving values and timeline.
- BR-004: Quorum resolution deterministic and audited.
- BR-005: Feature opt-in via host settings (mirroring Zero's Settings > Workflows gate).

## 8. Domain Modeling

| Aggregate | Responsibility | Invariants |
|---|---|---|
| `WorkflowDefinition` | versioned form+flow spec | published ⇒ immutable |
| `WorkflowInstance` | one running request | bound to start version |
| `WorkflowTask` | one approval unit | resolved assignee set at activation |

## 12. Persistence and Data

Tables `EafWorkflow*` (definitions, versions, fields, steps, instances, tasks, timeline, delegations). Migrations up/down. Tenant-scoped.

## 16. Non-Functional Requirements

- Engine transitions idempotent (safe under retry/concurrency).
- Background reminders via Hangfire with tenant-aware filtering.
- Observability: OpenTelemetry spans around step transitions.

## 18. Expected Tests

- Unit: validator, assignee resolution (all/quorum), version pinning, send-back semantics.
- Integration: catalog → submit → approve → complete round-trip; republish isolation; delegation rerouting; SLA reminder firing (Hangfire test host).

## Pending Questions and Ambiguities

- Step-graph serialization: JSON column vs normalized step tables — recommend JSON per version (immutable blobs) + normalized current-instance state.
- Manager resolution source (existing user.Manager? org chart?) — verify what EAF already has before Phase B.
- Phase D integrations deliberately deferred.

## Delivered

Implementado e mergeado em `main` (PRs #454, #455, #456).
