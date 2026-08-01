# memory.md — short-term memory

Working state of the current session. Overwritten on every update.
Durable records belong in .claude/memory/{YYYYMMDD}-memory.md.

- Last verified commit: 3339886 (Merge pull request #278 from afonsoft/devin/20260801-eaf-version-9.4.2)
- Test baseline: 4605 total / 4604 passing / 0 skipped; Line 97.9%, Branch 90.5%, Method 99.8%; Build warnings 0 (Eaf.sln) — last known from legacy memory; current CI metrics may differ
- Active branch: feature/claude-20260801-bootstrap-claude-harness
- Active work item: Bootstrap native Claude Code harness
- In progress: nothing — clean boundary; ready to commit
- Uncommitted files: full `.claude/` harness migration
- Blockers / out-of-scope findings:
  - opencode.json left untouched (OpenCode server config; not a Claude harness artifact)
- Next action: commit harness migration and create PR to main
- Last updated: 20260731
