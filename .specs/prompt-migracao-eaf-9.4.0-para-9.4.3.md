# EAF — Migration Prompt 9.4.0 to 9.4.3

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Migration Prompt 9.4.0 to 9.4.3 |
| Product / System | EAF Templates |
| Module / Bounded Context | Migration / Agent Prompt |
| Change type | Migration / Documentation |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `docs/migration-prompt-9.4.3` |
| Technical owner | Core Team |
| Status | Archived |
| Date | 2026-08-13 |
| Target agent | Any |

## 1. Executive Summary

### Problem

This file is a reusable agent prompt for migrating generated EAF projects from 9.4.0 to 9.4.3. It was originally written in Portuguese and should be maintained in en-us for consistency with the rest of `.specs/`.

### Objective

Provide a prompt template that can be copied into an agent to drive the 9.4.0 → 9.4.3 migration, aligned with the generic migration playbook.

### Expected outcome

- Clear instructions for an agent to upgrade a 9.4.0 EAF project to 9.4.3.
- Safe, step-by-step commands and verification checks.

### Out of scope

- Automatic migration tooling.
- Migration to versions other than 9.4.3.

## 2. Agent Role

Migration executor. Follow the playbook, verify each step, and do not proceed without backups.

## 3. Agent Autonomy Level

**1 — Assistive**

## 4. Product Context

Generated EAF API + Angular projects from the 9.4.0 template need version and package upgrades to 9.4.3.

### Relevant stack

- .NET 10, ABP 10.5, Angular 20

### Context files the agent must read before implementation

- `.specs/eaf-template-migration-and-update.spec.md`
- `.specs/eaf-template-migration-9.4.1.md`

## 5. Task Definition

### Main task

Migrate an existing 9.4.0 EAF project to 9.4.3 following safe migration steps.

### Subtasks

- Confirm current versions and backups.
- Update EAF/ABP packages in .NET projects.
- Update Angular dependencies.
- Apply namespace and configuration changes.
- Add and apply EF Core migrations.
- Build and test.

### Do not do

- Do not delete `bin`/`obj` without confirmation.
- Do not run `Update-Database` on production automatically.
- Do not commit secrets.

## 6. Functional Requirements

### FR-001: Backup and version check

**Description:** Before migration, verify the source is committed and record current package versions.

**Acceptance criteria:**

- [ ] `git status` clean or committed branch.
- [ ] Current EAF version recorded.

### FR-002: Package update

**Description:** Update EAF NuGet packages and npm packages to 9.4.3 compatible versions.

**Acceptance criteria:**

- [ ] `common.props` and `Directory.Build.props` updated.
- [ ] `package.json` and `package-lock.json` updated.

### FR-003: Code and configuration migration

**Description:** Apply any 9.4.0 → 9.4.3 breaking changes.

**Acceptance criteria:**

- [ ] EAF-specific namespace/class changes applied.
- [ ] `appsettings.json` updated.

### FR-004: Database migrations

**Description:** Add and apply EF Core migrations.

**Acceptance criteria:**

- [ ] `Add-Migration` executed.
- [ ] Migrations tested on a staging database before production.

### FR-005: Verification

**Description:** Build, test, and smoke-test the migrated project.

**Acceptance criteria:**

- [ ] `dotnet restore`, `dotnet build`, `dotnet test` pass.
- [ ] `npm install`, `ng build`, `ng test` pass.
- [ ] Smoke tests on staging pass.

## 7. Business Rules

### BR-001: Backup before migration

The agent must confirm a backup exists before running database commands.

### BR-002: No automatic production changes

Production database updates must be approved by the user.

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

- Add migration with `Add-Migration EAF943_Initial`.
- Apply to staging with `Update-Database`.

## 13. Integrations

N/A.

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| Restore fails | Conflicting packages | Run `dotnet restore --no-cache`; ask user if unresolved |
| Migration error | Snapshot mismatch | Re-add migration after fixing model |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

- Use en-us for the prompt text and instructions.
- Keep the prompt copy-paste friendly.

## 17. Mandatory Guardrails

Do not execute destructive commands without user confirmation. Do not commit secrets. Do not modify `.github/workflows/` without approval.

## 18. Expected Tests

N/A.

## 19. Acceptance Criteria

- [ ] Prompt is in en-us.
- [ ] It references the generic migration playbook.
- [ ] It includes safety checks and verification steps.

## 20. Implementation Plan

N/A — this is a prompt template.

## 21. Rollback Strategy

- Revert commits.
- Restore database from backup.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Agent over-automates | High | Medium | Use autonomy level 1 and ask for confirmation |
| Breaking changes not covered | Medium | Medium | Stop and ask user when unknown error occurs |

## 23. Definition of Done

- [ ] Prompt reviewed and formatted consistently.
- [ ] Linked from index.

## 24. Key Reminder

> The SPEC is the contract. This is an agent prompt, not a code contract.

## Prompt Template

```text
You are migrating an existing EAF 9.4.0 generated project to EAF 9.4.3.

Before any changes:
1. Confirm the working directory is a git repo with a clean/commit state.
2. Record current EAF, .NET, ABP, Angular, and PrimeNG versions.
3. Confirm a database backup exists (ask user for production).

Migration steps:
1. Update EAF NuGet packages in .csproj and common.props/Directory.Build.props to 9.4.3.
2. Update Angular package.json dependencies to the versions used by EAF 9.4.3.
3. Apply any namespace or class renames documented in EAF release notes.
4. Update appsettings.json if new sections are required.
5. Run `dotnet restore` and `dotnet build`.
6. Add EF Core migrations: `Add-Migration EAF943_Initial`.
7. Apply to staging and test.
8. Run `npm install` and `ng build --configuration=production`.
9. Run `dotnet test` and `ng test --no-watch --browsers=ChromeHeadlessNoSandbox`.
10. Run smoke tests.

Do not:
- Run `Update-Database` on production without user approval.
- Delete bin/obj without explaining why.
- Commit secrets or connection strings.
- Modify .github/workflows without approval.

If any step fails, stop and report the error with the exact command and output.
```

## References

- `.specs/eaf-template-migration-and-update.spec.md`
- `.specs/eaf-template-migration-9.4.1.md`
