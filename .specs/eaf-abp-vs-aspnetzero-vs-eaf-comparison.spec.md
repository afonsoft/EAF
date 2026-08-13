# EAF — ABP Boilerplate vs ASP.NET Zero vs EAF

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | ABP vs ASP.NET Zero vs EAF Comparison |
| Product / System | EAF |
| Module / Bounded Context | Cross-cutting / Roadmap |
| Change type | Roadmap / Analysis |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/spec-comparison` |
| Technical owner | Core Team |
| Status | Approved |
| Date | 2026-08-13 |
| Target agent | Any |

## 1. Executive Summary

### Problem

There is no single source of truth comparing EAF to the upstream ABP Framework and the commercial ASP.NET Zero product. This makes prioritization and feature planning difficult.

### Objective

Produce and maintain a side-by-side feature comparison that identifies what EAF already covers, what is missing, and what should be built next.

### Expected outcome

A comparison SPEC that drives the Q3 2026 roadmap and is referenced by all new module SPECs.

### Out of scope

- Detailed implementation designs (covered by per-module SPECs).
- Frontend visual parity with Metronic 8 (no license-free reuse).

## 2. Agent Role

Analyst / architect. Read-only research; do not invent features. Cite actual source files or public documentation.

## 3. Agent Autonomy Level

**0 — Experimental/Research**

## 4. Product Context

EAF is an open-source middleware platform on ABP .NET 10. The comparison informs module and template investments.

### Relevant stack

- ABP 10.5, ASP.NET Zero (commercial), .NET 10, Angular 20.

### Context files the agent must read before implementation

- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-angular-remaining-modernization-features.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`

## 5. Task Definition

### Main task

Maintain a living comparison matrix of ABP, ASP.NET Zero, and EAF features.

### Subtasks

- Verify EAF implementation status from source.
- Document ASP.NET Zero public capabilities.
- Identify and prioritize gaps.

### Do not do

- Do not copy ASP.NET Zero code or assets.
- Do not claim a feature is implemented without source evidence.

## 6. Functional Requirements

### FR-001: Comparison matrix

**Description:** The matrix must cover framework, identity, multi-tenancy, infrastructure, and UI areas.

**Acceptance criteria:**

- [ ] Each row cites EAF source or a missing module SPEC.
- [ ] Gap level is marked Low, Medium, or High.

## 7. Business Rules

### BR-001: Evidence-based status

Every EAF status must be traceable to a source file, service, or UI component.

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

- Keep the SPEC updated after each major release.
- Language: en-us.

## 17. Mandatory Guardrails

Do not invent ABP/Zero features; use only public docs and EAF source evidence.

## 18. Expected Tests

N/A.

## 19. Acceptance Criteria

- [ ] Matrix is complete and evidence-based.
- [ ] Key findings and next steps are documented.
- [ ] References link to ABP, Zero, and EAF docs.

## 20. Implementation Plan

1. Gather EAF evidence from `src/` and `Templates/`.
2. Map against ABP and Zero feature lists.
3. Update `.specs/eaf-next-steps-q3-2026.spec.md` with priorities.

## 21. Rollback Strategy

N/A — documentation.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Feature claims become stale | Medium | High | Update per release |
| Confusing ABP open source with Zero commercial | Medium | Medium | Separate columns clearly |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Evidence cross-checked against `src/`.
- [ ] Roadmap references this comparison.

## 24. Key Reminder

> The SPEC is the contract. Do not expand the scope with personal opinions.

## Comparison Matrix

| Area | ABP Framework | ASP.NET Zero | EAF (2026-08) | Gap Level |
|---|---|---|---|---|
| Runtime | .NET 6+ | .NET 6+ | .NET 10 | None |
| Architecture | N-Layer, DDD | N-Layer, DDD | N-Layer, DDD (ABP 10.5) | None |
| DI | Castle / MS | Castle / MS | Castle Windsor | None |
| ORM | EF Core / NH | EF Core | EF Core 10 | None |
| Multi-tenancy | Core | Core + dashboard | Shared/host + join requests | Low |
| Users/Roles/Permissions | Full | Full + OU + delegation | Full + OU + delegation | None |
| Organization Units | Entities | Tree UI + members | **Implemented** | None |
| User Delegation | Impersonation | Time-bounded | **Implemented** | None |
| Mass Notifications | Basic | Admin mass-send | **Implemented** | None |
| Tenant Join Requests | No | Yes | **Implemented** | None |
| Dashboard | Empty | Host/tenant widgets | **Implemented** | None |
| Payment/Subscription | No | Stripe/PayPal + lifecycle | Gateways only; no subscription lifecycle | High |
| SMS | No | Yes (Twilio) | Not implemented | Medium |
| MailKit | Basic | Rich templates | Not implemented | Medium |
| Blob Storage | No | Azure/AWS/FileSystem | Not implemented | Medium |
| Redis Cache | No | Distributed | Not implemented | Medium |
| SignalR Module | No | Real-time | `ChatHub` only; no module | Medium |
| Push Notifications | No | Yes | Not implemented | Medium |
| Passwordless Login | No | Email/SMS | Not implemented | Medium |
| QR Login | No | Mobile app | Not implemented | Low |
| Social Account Linking | No | Profile link/unlink | Not implemented | Low |
| Setup Page | No | Web setup | Not implemented | Low |
| Audit Logs UI | Basic | Advanced | Basic UI exists | Low |
| Rate Limiting | No | Core + UI | Core exists; UI missing | Low |
| Background Jobs | Hangfire/Quartz | Hangfire | Hangfire | None |
| OpenTelemetry | Optional | Optional | **Implemented** | None |
| Key Vault | No | Azure | **Implemented** | None |
| Serilog | Optional | Optional | **Implemented** | None |
| Angular UI | Plain Bootstrap | Metronic 8 + Bootstrap 5 | Legacy Metronic + PrimeNG + ngx-bootstrap | High |
| Dark Mode | No | 13+ themes | Not implemented | High |
| PWA | No | MAUI + PWA | SW configured; no offline/push | Medium |
| Mobile Responsive | Bootstrap | Mobile-first | Desktop-first | Medium |
| Accessibility | Basic | Better | Partial | Medium |
| Localization | XML/JSON | XML/JSON | XML/JSON | None |
| Swagger | Swashbuckle | Swashbuckle | Swashbuckle | None |

## Key Findings

1. EAF is closer to ABP than to ASP.NET Zero in breadth.
2. EAF closed several Zero gaps: Organization Units, Mass Notifications, User Delegation, Tenant Join Requests, Dashboard, Payment Gateway, Key Vault, OpenTelemetry.
3. Largest gaps: subscription lifecycle, Redis, Blob, MailKit, SignalR module, SMS, Push, and modern Angular UI (dark mode, Bootstrap 5, PWA offline).

## Recommended Next Steps

1. Implement `Eaf.RedisCache`.
2. Implement `Eaf.MailKit` and `Eaf.BlobStoring`.
3. Implement `Eaf.SignalR` module and `Eaf.Sms`.
4. Build subscription lifecycle on existing payment gateways.
5. Modernize Angular template.

## References

- ABP Framework: <https://abp.io/>
- ASP.NET Zero: <https://aspnetzero.com/>
- `.specs/eaf-aspnetzero-functional-gap.spec.md`
- `.specs/eaf-next-steps-q3-2026.spec.md`
