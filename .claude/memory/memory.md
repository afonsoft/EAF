# memory.md — short-term memory

Working state of the current session. Overwritten on every update.
Durable records belong in .claude/memory/{YYYYMMDD}-memory.md.

- Last verified commit: (working tree on branch devin/1786700000-subscription-payments-lifecycle)
- Test baseline: full `dotnet test Eaf.sln` passed except one flaky `EafSqliteCacheTests.RemoveExpired_ShouldOnlyRemoveExpiredItems` timing test, which passed in isolation. Application and Web.Core test suites passed.
- Build: `dotnet build Eaf.sln` 0 warnings, 0 errors.
- Active branch: devin/1786700000-subscription-payments-lifecycle
- Active work item: implement subscription payment lifecycle (PaymentManager, Stripe recurring support, webhook endpoint, renewal worker, DTOs, proration/upgrade/downgrade, tests)
- In progress: commit and open PR to main
- Uncommitted files: src/Eaf.Middleware.* changes, test changes, new PaymentWebhookController, new SubscriptionPayment entity/product and DTOs
- Blockers / out-of-scope findings: template DB migration for SubscriptionPaymentProduct blocked until Eaf.Middleware.Core package is published with new entity
- Next action: stage, commit, push and create PR
- Last updated: 20260813
