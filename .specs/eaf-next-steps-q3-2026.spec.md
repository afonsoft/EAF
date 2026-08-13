# EAF — Next Steps Roadmap Q3 2026

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Next Steps Roadmap Q3 2026 |
| Product / System | EAF |
| Module / Bounded Context | Cross-cutting / Program Management |
| Change type | Roadmap |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/roadmap-q3-2026` |
| Technical owner | Core Team |
| Status | Approved |
| Date | 2026-08-13 |
| Target agent | Any |

## 1. Executive Summary

### Problem

Multiple `.specs/` files describe individual modernization areas without a consolidated quarterly roadmap tying backend, frontend, quality, and documentation priorities together.

### Objective

Provide a single, time-bound roadmap for Q3 2026 that guides implementation and tracks progress against ABP/ASP.NET Zero parity.

### Expected outcome

- Clear themes and priorities for Q3 2026.
- Milestones and success criteria.
- Each roadmap item links to a per-feature SPEC.

### Out of scope

- Detailed implementation designs (covered by per-module SPECs).
- Long-term planning beyond Q3 2026.

## 2. Agent Role

Technical program owner. Synthesizes other SPECs into an actionable plan without inventing new features.

## 3. Agent Autonomy Level

**0 — Research/Roadmap**

## 4. Product Context

EAF is an ABP-based middleware and template project. The roadmap helps the core team and community focus on the highest-value gaps.

### Relevant stack

- .NET 10, ABP 10.5, EF Core 10
- Angular 20, PrimeNG 17
- Redis, Hangfire, OpenTelemetry

### Relevant files or directories

```text
/.specs
/docs
/src
/Templates
```

### Context files the agent must read before implementation

- `.specs/eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`

## 5. Task Definition

### Main task

Define and maintain the Q3 2026 EAF roadmap.

### Subtasks

- Group pending work into themes.
- Assign priorities and owners.
- Set milestones and success criteria.

### Do not do

- Do not invent new features not already documented in other SPECs.
- Do not assign unrealistic dates.

## 6. Functional Requirements

### FR-001: Roadmap themes

**Description:** The roadmap must cover backend modules, subscription/payment lifecycle, Angular modernization, quality/performance, and documentation.

**Acceptance criteria:**

- [ ] Every active `.specs/` file is referenced by a roadmap theme.
- [ ] Each theme has a priority and target milestone.

### FR-002: Milestones

**Description:** Milestones must be defined for end of July, August, and September 2026.

**Acceptance criteria:**

- [ ] Each milestone lists deliverables.
- [ ] Deliverables are testable.

## 7. Business Rules

### BR-001: Evidence-based priority

Priorities must be based on the comparison matrix and gap analysis, not opinion.

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

- Keep the SPEC updated after each monthly review.
- Language: en-us.

## 17. Mandatory Guardrails

Do not expand scope beyond Q3 2026 without a new roadmap SPEC.

## 18. Expected Tests

N/A.

## 19. Acceptance Criteria

- [ ] Roadmap covers all open gaps.
- [ ] Each item links to a SPEC.
- [ ] Success criteria are measurable.

## 20. Implementation Plan

1. Review comparison and gap SPECs.
2. Group work into themes.
3. Define milestones and success criteria.
4. Publish in `.specs/eaf-next-steps-q3-2026.spec.md`.

## 21. Rollback Strategy

N/A — roadmap can be revised by a new PR.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Roadmap becomes outdated | Medium | High | Update monthly |
| Over-committing to Q3 | High | Medium | Keep stretch goals explicit |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Linked from index.
- [ ] Milestones agreed with team.

## 24. Key Reminder

> The SPEC is the contract. The roadmap is a plan, not a guarantee.

## Themes and Priorities

| Theme | Priority | Key deliverables |
|---|---|---|
| Backend foundation modules | P1 | `Eaf.RedisCache`, `Eaf.MailKit`, `Eaf.BlobStoring`, `Eaf.SignalR` |
| Subscription and payment lifecycle | P2 | `SubscriptionPayment` entities, `SubscriptionAppService`, invoice worker |
| Angular template modernization | P3 | PrimeNG completion, Bootstrap 5/Metronic 8 spike, dark mode |
| Quality and performance | P4 | Instrumentation, bundle budgets, lazy loading, a11y tests |
| Documentation and DevEx | P5 | Spec index, migration guides, agent skills |

## Milestones

- **End of July 2026**: `Eaf.RedisCache` PoC + Angular dark mode spike.
- **End of August 2026**: `Eaf.MailKit` and `Eaf.BlobStoring` modules; PrimeNG migration 50% complete.
- **End of September 2026**: `Eaf.SignalR` module; Bootstrap 5/Metronic 8 layout spike; PWA offline MVP.

## Success Criteria

- P1 modules compile and have ≥ 90% coverage.
- Angular Lighthouse mobile score ≥ 80.
- No increase in CI build/test time beyond 20%.
- No reduction in overall code coverage.

## References

- `.specs/eaf-abp-vs-aspnetzero-vs-eaf-comparison.spec.md`
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
