
## Session — 2026-11-09 18:20

**Scope**: ASP.NET Zero v15.5 parity plan — full W1–W4 delivery + Phase 5 verification/QA
**Decisions**: ABP 11.x not public (NuGet max 10.5.1) — blocked spec stays Draft; PrimeNG kept (no ng-zorro migration, Angular 21 + PrimeNG 21 upgrade instead); OpenIddict module gated behind EAF_USE_SOURCE_PROJECTS define until first NuGet publish; passkeys via Fido2 lib (not .NET 10 IUserPasskeyStore — too invasive for AbpUserStore); QR login tokens cache-backed; workflow engine standalone module with IWorkflowUserDirectory seam
**Delivered**: PRs #447–#460 merged (specs, API keys, passkeys, session security, mandatory dynamic props, Mapperly spike, workflow engine A/B/C, email templates, QR login, OpenIddict, Angular 21 + PrimeNG 21)
**Remaining**: ABP 11.x upstream watch (blocked); Mapperly rollout spec (~2.5d); Draft/In-review specs for user decision
**Lessons**: dotnet ef fails on repo csprojs (MSB4057) — use /tmp/efhost scratch project; common.props needs EAF.ico+EAF.png+README pack items per new project; Angular 21 NG0100 checkNoChanges broke mid-cycle directive tests; ngx-bootstrap TabsModule alias conflict with PrimeNG TabsModule
