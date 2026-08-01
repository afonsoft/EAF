# memory.md — short-term memory

Working state of the current session. Overwritten on every update.
Durable records belong in .claude/memory/{YYYYMMDD}-memory.md.

- Last verified commit: fc9d5f1 (chore(harness): migrate legacy agent harness to native .claude/ structure)
- Test baseline: 4605 total / 4604 passing / 0 skipped; Line 97.9%, Branch 90.5%, Method 99.8%; Build warnings 0 (Eaf.sln) — last known from legacy memory; no code changed by this harness migration
- Active branch: feature/claude-20260801-bootstrap-claude-harness
- Active work item: Bootstrap native Claude Code harness
- In progress: nothing — migration committed; PR #279 open
- Uncommitted files: none
- Blockers / out-of-scope findings:
  - opencode.json left untouched (OpenCode server config; not a Claude harness artifact)
- Next action: monitor PR #279 CI; on merge, re-run harness validation and update memory
- Last updated: 20260731
