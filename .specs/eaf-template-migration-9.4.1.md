# EAF — Template Migration Guide 9.4.0 to 9.4.1

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Template Migration Guide 9.4.0 to 9.4.1 |
| Product / System | EAF Templates |
| Module / Bounded Context | Migration / Upgrade |
| Change type | Migration / Documentation |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/template-migration-9.4.1` |
| Technical owner | Core Team |
| Status | Archived |
| Date | 2026-08-13 |
| Target agent | Any |

## 1. Executive Summary

### Problem

Existing projects generated from the EAF 9.4.0 template need a step-by-step guide to migrate to 9.4.1.

### Objective

Provide a concise migration checklist covering package updates, code changes, and manual merge steps.

### Expected outcome

- Teams can migrate a 9.4.0 project to 9.4.1 without missing breaking changes.
- The guide references EAF-specific files and namespaces.

### Out of scope

- Automatic migration tooling.
- Migration to versions different from 9.4.1.

## 2. Agent Role

Migration guide author. Reference actual source diff if possible.

## 3. Agent Autonomy Level

**0 — Documentation**

## 4. Product Context

EAF templates are periodically updated. This guide covers the 9.4.0 → 9.4.1 delta.

### Relevant stack

- .NET 10, ABP 10.5, Angular 20

### Relevant files or directories

```text
Templates/Api
Templates/Angular/Eaf.ProjectName.UI
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-template-migration-and-update.spec.md`

## 5. Task Definition

### Main task

Write a guide that walks a project from 9.4.0 to 9.4.1.

### Subtasks

- List package/dependency changes.
- List namespace, class, and configuration changes.
- Provide merge and verification steps.

### Do not do

- Do not provide steps that break an existing 9.4.0 project without explanation.
- Do not include secrets or hardcoded keys.

## 6. Functional Requirements

### FR-001: Dependency changes

**Description:** List NuGet and npm package version changes.

**Acceptance criteria:**

- [ ] All changed packages listed.
- [ ] Breaking changes highlighted.

### FR-002: Code changes

**Description:** List EAF-specific namespace, class, and configuration changes.

**Acceptance criteria:**

- [ ] All changed EAF namespaces listed.
- [ ] Migration snippets provided.

### FR-003: Verification

**Description:** Provide a verification checklist.

**Acceptance criteria:**

- [ ] Build, tests, and Angular build steps included.

## 7. Business Rules

### BR-001: Backup first

The guide must instruct users to commit and backup before migrating.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

N/A.

## 10. API Contracts

N/A.

## 11. Application Contracts

N/A.

## 12. Persistence and Data

### EF Core migrations

- Add new migrations if new entities are introduced.
- Apply migrations after package updates.

## 13. Integrations

N/A.

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Package restore errors | Outdated lock file | Delete `bin`/`obj` and run `dotnet restore` |
| Build errors | Namespace changes | Follow migration snippets |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

- Keep the guide concise and copy-paste friendly.
- Language: en-us.

## 17. Mandatory Guardrails

Do not include secrets; do not recommend destructive commands without backup warning.

## 18. Expected Tests

N/A.

## 19. Acceptance Criteria

- [ ] Guide is complete for 9.4.0 → 9.4.1.
- [ ] Build and test verification steps listed.
- [ ] No secrets.

## 20. Implementation Plan

N/A.

## 21. Rollback Strategy

- Restore from backup if build or tests fail.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Guide becomes stale | Medium | High | Archive after next major release |
| Manual merge errors | Medium | Medium | Provide clear diff and snippets |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Guide tested against actual 9.4.0 → 9.4.1 diff if available.

## 24. Key Reminder

> The SPEC is the contract. This is a migration guide, not an implementation contract.

## Migration Steps

1. Backup the 9.4.0 solution and database.
2. Update EAF NuGet packages from 9.4.0 to 9.4.1.
3. Update npm packages in the Angular project if the package.json changed.
4. Apply namespace and class renames per EAF 9.4.1 release notes.
5. Add/update EF Core migrations.
6. Update `appsettings.json` if new configuration sections are required.
7. Build the .NET solution and the Angular project.
8. Run unit and integration tests.
9. Run UI smoke tests.

## References

- `.specs/eaf-template-migration-and-update.spec.md`
