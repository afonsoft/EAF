# Gap Analysis — 2026-11-09 (post W1–W4 delivery)

## Sources
- AS-IS: `src/`, `test/`, coverage run `TestResults-qa/` (256k lines scanned, 52 modules), `git log` PRs #447–#460
- TO-BE: `.specs/*.spec.md` (57 files), `CLAUDE.md` (coverage no-drop), `docs/architecture/`
- gh: authenticated; 0 open issues

## Verdicts

| Key | Verdict | Evidence |
|-----|---------|----------|
| GAP-tests-openiddict-coverage | CONFIRMADO | `Eaf.OpenIddict` 1.9% line coverage (2 tests, options only); controllers/seed worker/middleware untested. Target: repo coverage no-drop baseline + 90% aspirational. |
| GAP-docs-drawio-arch-outdated | CONFIRMADO | `eaf-system-architecture.drawio` predates API keys/passkeys/OpenIddict/Workflow; only `.mmd` added (headless env cannot render drawio). |
| GAP-operation-packages-not-published | INCONCLUSIVO | `Eaf.OpenIddict`/`Eaf.Workflow`/`Eaf.DynamicEntityProperties` have no public NuGet package; template compiles via `EAF_USE_SOURCE_PROJECTS` gate. Depends on next `publish-all.yml` run. |
| GAP-impl-mapperly-rollout | DUPLICADO | `.specs/eaf-mapperly-migration.spec.md` (Draft) already covers ~2.5d rollout; pilot merged in #453. |
| GAP-external-abp-11x | DUPLICADO | `.specs/eaf-abp-11x-upstream-watch.spec.md`; NuGet still 10.5.1. |
| GAP-tests-webcore-reflection | REJEITADO (fixed) | `TokenAuthControllerP{39,41}BddTests` invoked `CreateJwtClaims` with 3 args after `sessionId` param added — fixed this branch. |
| GAP-specs-backlog | CONFIRMADO (info) | ~30 specs in Draft/In-review unimplemented (module-blob-storage, dapper, quartz, redis-cache, signalr spec, a11y, pwa, dark-mode, metronic8, mobile, payments UI...). They are tracked Drafts — need user triage, not new artifacts. |
