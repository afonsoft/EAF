# EAF — Template Migration and Update

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Template Migration and Update |
| Product / System | EAF Templates |
| Module / Bounded Context | Migration / Upgrade |
| Change type | Migration / Documentation |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/template-migration-update` |
| Technical owner | Core Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Any |

## 1. Executive Summary

### Problem

EAF templates are updated frequently with new EAF package versions, ABP upgrades, and configuration changes. Generated projects need a maintained migration/update playbook to stay current.

### Objective

Create a reusable template migration/update guide and keep it current for the latest EAF, .NET, and Angular versions.

### Expected outcome

- A general migration playbook applicable to any EAF template version.
- Links to specific migration guides such as 9.4.0 → 9.4.1.
- Clear rollback and verification steps.

### Out of scope

- Automatic patch tooling.
- Upgrade of custom code not generated from the template.

## 2. Agent Role

Migration playbook author. Focus on generic steps and clear, safe commands.

## 3. Agent Autonomy Level

**0 — Documentation**

## 4. Product Context

EAF provides API + Angular + Worker templates. The playbook must cover all three and reference current `Directory.Build.props`/`common.props` versions.

### Relevant stack

- .NET 10, ABP 10.5, Angular 20

### Relevant files or directories

```text
Templates/Api
Templates/Angular/Eaf.ProjectName.UI
Templates/Worker
common.props
Directory.Build.props
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-template-migration-9.4.1.md`

## 5. Task Definition

### Main task

Write and maintain a generic EAF template migration/update guide.

### Subtasks

- Define pre-migration checklist.
- Document package, config, and code migration steps.
- Document verification and rollback.
- Link to version-specific guides.

### Do not do

- Do not recommend `git push --force` or destructive steps without backup warning.
- Do not include secrets or hardcoded keys.

## 6. Functional Requirements

### FR-001: Pre-migration checklist

**Description:** Steps to prepare before any template update.

**Acceptance criteria:**

- [ ] Backup source and database.
- [ ] Record current EAF, .NET, ABP, Angular, and PrimeNG versions.
- [ ] Review breaking changes in release notes.

### FR-002: Package and dependency migration

**Description:** Update NuGet and npm packages.

**Acceptance criteria:**

- [ ] Update `common.props` and `Directory.Build.props`.
- [ ] Update `package.json` and run `npm install`.

### FR-003: Code and configuration migration

**Description:** Update namespaces, services, and `appsettings.json`.

**Acceptance criteria:**

- [ ] List common breaking changes by version.
- [ ] Provide snippets for migration helpers.

### FR-004: Verification

**Description:** Steps to verify the migration.

**Acceptance criteria:**

- [ ] `dotnet restore`, `dotnet build`, `dotnet test`.
- [ ] `npm install`, `ng build --configuration=production`, `ng test`.
- [ ] Apply and smoke-test EF Core migrations.

### FR-005: Rollback

**Description:** Document rollback strategy.

**Acceptance criteria:**

- [ ] Restore database and source control steps.
- [ ] Identify commits to revert.

## 7. Business Rules

### BR-001: Backup before migration

Every migration must start with source and database backups.

### BR-002: No in-place production migration

Production migrations must be tested in a non-production environment first.

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

- Always add a new migration after package/entity changes.
- Test migrations on a copy of production data before applying to production.

## 13. Integrations

N/A.

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| `dotnet restore` fails | Conflicting packages | Delete `bin`/`obj`, run `dotnet restore --no-cache` |
| Angular build fails | New major Angular | Follow `ng update` migration |
| Database migration fails | Snapshot mismatch | Use `Update-Database` verbose, fix, re-add migration |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

- Keep the playbook concise and version-agnostic.
- Language: en-us.

## 17. Mandatory Guardrails

Do not include secrets; do not recommend unsafe production commands; do not advise manual `bin`/`obj` deletion without caution.

## 18. Expected Tests

N/A.

## 19. Acceptance Criteria

- [ ] Playbook covers pre-migration, migration, verification, rollback.
- [ ] Links to version-specific guides (e.g. 9.4.0 → 9.4.1).
- [ ] No secrets or unsafe commands.

## 20. Implementation Plan

1. Define generic migration flow.
2. Document pre-migration and backup.
3. Document package and code migration.
4. Document verification and rollback.
5. Link specific migration guides.

## 21. Rollback Strategy

- Revert the source branch.
- Restore database from backup.
- Revert or downgrade NuGet/npm packages.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Breaking changes not covered | High | Medium | Pin EAF version matrix in guide |
| Production migration without testing | High | Medium | Require staging verification |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] Playbook validated against latest template structure.
- [ ] Linked from `.specs/eaf-specs-index-and-roadmap-2026.md`.

## 24. Key Reminder

> The SPEC is the contract. The playbook must be safe, generic, and link to specific guides.

## Generic Migration Flow

1. Backup source and database.
2. Record current versions.
3. Read EAF release notes and breaking changes.
4. Update `common.props`/`Directory.Build.props` and NuGet packages.
5. Update `package.json` and npm packages.
6. Apply namespace, class, and configuration migrations.
7. Add and apply EF Core migrations.
8. Build backend and frontend.
9. Run tests and smoke tests.
10. Deploy to staging, then production.

## Version-Specific Guides

- `.specs/eaf-template-migration-9.4.1.md` — 9.4.0 → 9.4.1

## References

- `CLAUDE.md`
- `.specs/eaf-template-migration-9.4.1.md`
