# EAF — Email Template Management (W4)

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | Email Template Management (Administration > Email Templates) |
| Product / System | EAF Middleware + Angular Template |
| Module / Bounded Context | `Eaf.MailKit` + admin UI |
| Change type | Feature (gap completion) |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-email-template-management` |
| Technical owner | Core Team |
| Status | Completed|
| Date | 2026-11-08 |
| Target agent | Devin |
| Wave | W4 (ASP.NET Zero v15.5 parity plan) |

## 1. Executive Summary

### Problem

ASP.NET Zero v15.4 added email template management (Administration > Email Templates). EAF sends emails via `Eaf.MailKit` and has scattered template references (~8 files), but no CRUD/manage UI for templates.

### Objective

Inventory existing template handling, then add tenant-level email template management: list, view, edit (subject/body with placeholders), reset to default, localized variants.

### Expected outcome

- Admin page listing system email templates.
- Editable per-tenant overrides with placeholder validation and preview.
- Mail sending resolves tenant override → default.

### Out of scope

- WYSIWYG designer (basic code/preview editor is enough).
- Notification template unification (separate concern).

## 2–4. Context

First step: grep `EmailTemplate`/`IEmailTemplate` in `src/` and `Templates/` to determine what's already implemented vs missing. ABP has `ITemplateDefinitionProvider`/`TextTemplateManagement` patterns — evaluate reuse before building custom.

## 5. Task Definition

- Audit current email template mechanism; document gaps in spec update.
- `EmailTemplateDefinition` (name, display name, default subject/body per culture, placeholders).
- Per-tenant override entity + app service (get/list/update/reset/preview with sample data).
- Angular admin page: list → edit (subject + body, culture tabs), placeholder hints, preview, reset.
- Wire `Eaf.MailKit` send path to resolve overrides.
- Localization en + pt-BR.

### Do not do

- Do not store secrets in templates.
- Do not allow arbitrary code execution in placeholder engine (restrict to named placeholders + basic conditionals).
- Do not break existing sends — default fallback must always resolve.

## 6. Functional Requirements

- [ ] Admin edits subject/body per culture; send uses override.
- [ ] Reset restores defaults.
- [ ] Unknown/malformed placeholders rejected at save with message.
- [ ] Preview renders sample data without sending.

## 12. Persistence and Data

`EafEmailTemplateOverrides` (tenant, name, culture, subject, body). Migration up/down.

## 18. Expected Tests

- Unit: placeholder validation, override resolution order, preview rendering.
- Integration: send path picks override; reset restores default.

## Pending Questions and Ambiguities

- Exact current template mechanism — resolved by the audit step at implementation.
- Placeholder syntax — follow whatever Eaf.MailKit already uses; document it.

## Delivered

Implementado e mergeado em `main` (PR #457).
