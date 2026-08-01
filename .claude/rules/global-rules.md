---
name: global-rules
description: Always-on guardrails for the EAF repository. Loaded before every session.
---

# EAF Global Rules

## Mission

EAF (Enterprise Application Foundation) is an open-source middleware platform built on ASP.NET Boilerplate (ABP) for .NET 10.0. It provides reusable modules for identity, cache, observability, background jobs, and secret management.

## Tech Stack

| Layer | Technology | Version |
|-------|-----------|--------|
| Runtime | .NET | 10.0 |
| Framework | ASP.NET Boilerplate (ABP) | 10.5.0 |
| ORM | Entity Framework Core | 10.0 |
| DI | Castle Windsor | — |
| Frontend | Angular | 18 (Templates) |
| Tests | xUnit + Shouldly + NSubstitute | — |
| Background | Hangfire | — |
| Realtime | SignalR | — |
| Observability | OpenTelemetry + Serilog | — |
| Secrets | Azure Key Vault / OCI Vault | — |
| CI/CD | GitHub Actions | — |
| Quality | SonarCloud + coverlet | — |
| License | GPL-3.0-or-later | — |

## Project Structure

- `src/` — 14 middleware modules (Eaf.Middleware.*, Eaf.KeyVault, Eaf.OpenTelemetry, etc.)
- `test/` — 14 test projects mirroring `src/`
- `Templates/` — Api, Angular, Worker, Eaf.Gateways.API templates
- `docs/` — technical documentation
- `.claude/` — Claude Code harness (rules, skills, agents, memory, knowledge)

## Hard Rules

1. **Protected branches**: `main` and `develop` — merge only via PR; never push directly.
2. **Tests must pass**: CI fails if any test fails.
3. **Coverage must not decrease** from the baseline.
4. **XML documentation** is required on all public APIs.
5. **Secrets**: never commit `.env`, credentials, tokens, or connection strings.
6. **Immutable files**: do not edit `service-proxies.ts`, `*.Designer.cs`, `*.g.cs`, `*.g.i.cs`.
7. **Workflows**: do not modify `.github/workflows/` without human review.
8. **No `--no-verify` or `--force`** on git operations without approval.
9. **Do not reduce** the number of existing tests.
10. **Generated outputs** (`bin/`, `obj/`, `nupkg/`, `TestResults/`, `sonar/`) are never committed.

## Soft Rules (warning + confirmation)

1. Modifying `Dockerfile` or `docker-compose.yml` → confirm with user.
2. Changing `common.props` → confirm compatibility (affects all projects).
3. Deleting test files → require justification.
4. Changing global NuGet dependencies → check for breaking changes.
5. Modifying legacy CI (`appveyor.yml`) → confirm necessity.
6. Changing DB schema/migrations → verify rollback.
7. Adding a new middleware module → follow ABP module pattern.

## Branching Strategy

- Features: `feature/<description>` or `feature/{AgentLLM}-{YYYYMMDD}-{description}`
- Bug fixes: `bug/<description>` or `hotfix/<description>`
- Releases: `release/<version>`
- Devin: `devin/<timestamp>-<description>`

## Memory Ritual

Before acting:

1. Read `.claude/memory/memory.md`.
2. Read the 3 most recent `.claude/memory/[0-9]*-memory.md` files.
3. Treat long-term memory as a hint; verify facts against the current code before acting.

After every checkpoint commit:

1. Update `.claude/memory/memory.md`.
2. Append durable records to `.claude/memory/{YYYYMMDD}-memory.md`.

## Code Conventions

- Architecture: ABP N-Layer (Domain → Application → Infrastructure → Presentation).
- Use `async/await` for I/O; suffix async methods with `Async`.
- Use constructor injection; never `new` for services.
- Public API XML docs in Portuguese (pt-BR).
- BDD test names: `Dado_Quando_Entao` or `[Fact] // Dado X, Quando Y, Então Z`.
- Minimum coverage target: ≥ 90%.
- `LangVersion` 14.0, `Nullable` disabled for EAF projects.

## Agent Loop

**Plan-and-Execute** (default for multi-file tasks):

1. Load `CLAUDE.md` and `global-rules.md`.
2. Read `.claude/memory/memory.md` and the 3 most recent long-term files.
3. Load pattern-matched skills and rules.
4. Present the Execution Plan and wait for approval.
5. Verify guardrails and permissions.
6. Execute within permissions.
7. Verification loop: `lint → test → CI`.
8. Validate the result.
9. Adjust — at most 2 iterations before escalating.
10. Update memory and commit the checkpoint.

## Response Style

- Portuguese (pt-BR) for documentation, test names, and comments.
- English for code and commits (`feat:`, `fix:`, `test:`, `docs:`).
- Concise, direct, no filler.
- Use `file:line` references when pointing to code.

## References

- `.claude/rules/csharp-eaf.md`
- `.claude/rules/angular-eaf.md`
- `.claude/rules/dotnet-project.md`
- `.claude/skills/`
- `.claude/memory/`
- `docs/`
