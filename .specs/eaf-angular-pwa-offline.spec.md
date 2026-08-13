# EAF Angular — PWA, Cache and Offline Experience

## 0. SPEC Metadata

| Field | Value |
|---|---|
| Feature name | PWA, Cache and Offline Experience |
| Product / System | EAF Angular Template |
| Module / Bounded Context | UI / PWA |
| Change type | Feature / Frontend |
| Repository | `github.com/afonsoft/EAF` |
| Suggested branch | `feature/eaf-angular-pwa` |
| Technical owner | Frontend Team |
| Status | In review |
| Date | 2026-08-13 |
| Target agent | Claude Code / Devin |

## 1. Executive Summary

### Problem

The Angular template already includes `@angular/pwa` and `@angular/service-worker` in `package.json`, plus `ngsw-config.json` and `ServiceWorkerModule.register`, but PWA behavior is not fully activated: no offline UX, API queue, push notifications, or install prompt.

### Objective

Finish the PWA experience with offline banner, cached assets and APIs, install prompt, and push notifications.

### Expected outcome

- App is installable.
- Key GET APIs are cached for offline reads.
- User actions can be queued and synced when online.
- Push notifications work for chat and system alerts.

### Out of scope

- Native mobile app.
- Background sync for all endpoints.

## 2. Agent Role

Senior Angular/PWA engineer. Use Angular service worker and web APIs, respect multi-tenancy.

## 3. Agent Autonomy Level

**2 — Reliable**

## 4. Product Context

Angular template uses `@angular/service-worker` and `localforage` but lacks offline UI and push logic.

### Relevant stack

- Angular 20, `@angular/pwa`, `@angular/service-worker`, PrimeNG 17

### Relevant files or directories

```text
Templates/Angular/Eaf.ProjectName.UI/ngsw-config.json
Templates/Angular/Eaf.ProjectName.UI/src/app/app.module.ts
Templates/Angular/Eaf.ProjectName.UI/src/manifest.json
Templates/Angular/Eaf.ProjectName.UI/angular.json
```

### Context files the agent must read before implementation

- `CLAUDE.md`
- `.specs/eaf-angular-mobile-responsive-layout.spec.md`
- `.specs/eaf-backend-modularization.spec.md`

## 5. Task Definition

### Main task

Complete PWA functionality for the EAF Angular template.

### Subtasks

- Validate `ngsw-config.json` and `manifest.json`.
- Implement offline UX and action queue.
- Add push notifications backend endpoint.
- Add install prompt.

### Do not do

- Do not cache POST/PUT/DELETE by default unless explicitly configured.
- Do not expose VAPID private keys.

## 6. Functional Requirements

### FR-001: Service Worker configuration

**Description:** Ensure `ngsw-config.json`, `manifest.webmanifest`, and `angular.json` are correct.

**Acceptance criteria:**

- [ ] `assetGroups` cache CSS, JS, fonts, icons.
- [ ] `dataGroups` cache read APIs with `performance` strategy and write APIs with `freshness`.
- [ ] `manifest` has EAF icons, theme colors, shortcuts.

### FR-002: Offline UX

**Description:** Detect connectivity and show offline banner / pending sync indicator.

**Acceptance criteria:**

- [ ] Use `navigator.onLine` and `online`/`offline` events.
- [ ] Show snackbar/banner when offline.
- [ ] Queue actions (chat, forms) and sync when online.

### FR-003: Push notifications

**Description:** Enable Web Push notifications for chat and alerts.

**Acceptance criteria:**

- [ ] Backend endpoint to register subscriptions and send pushes via VAPID.
- [ ] Angular service requests permission and handles messages.

### FR-004: Installation

**Description:** Support app installation.

**Acceptance criteria:**

- [ ] Handle `beforeinstallprompt` and suggest installation.
- [ ] Provide 192x192 and 512x512 icons.

## 7. Business Rules

### BR-001: Tenant-scoped cache

Cache keys and push topics must include tenant context.

### BR-002: No stale writes

POST/PUT/DELETE endpoints must use `freshness` strategy; do not cache writes as reads.

## 8. Domain Modeling

N/A.

## 9. Expected Architecture

- Angular service worker + `localforage` for offline queue.
- Backend `PushNotificationAppService`.

## 10. API Contracts

```http
POST /api/services/app/PushNotification/RegisterSubscription
POST /api/services/app/PushNotification/Send
```

## 11. Application Contracts

N/A.

## 12. Persistence and Data

- `localforage` for queued actions.
- `PushSubscription` store in backend.

## 13. Integrations

| Service | Purpose | Protocol | Timeout | Retry |
|---|---|---|---|---|
| Web Push Service | Push notifications | HTTPS | 15000ms | Yes |

## 14. Edge Cases and Error Scenarios

| Scenario | Input | Expected behavior |
|---|---|---|
| User denies push permission | Permission denied | Do not ask again in same session |
| Service worker not supported | Older browser | Gracefully degrade |

## 15. Few-Shot Examples

N/A.

## 16. Non-Functional Requirements

### Performance

Cached reads should be fast; avoid stale data by short `maxAge`.

### Security

VAPID keys stored in KeyVault/config, not in source.

### Reliability

Queue should persist across reloads and retry on reconnect.

## 17. Mandatory Guardrails

Do not expose VAPID secrets; do not cache write endpoints as performance; do not push PII.

## 18. Expected Tests

| Component / Flow | Scenarios |
|---|---|
| Service worker | Install, cache, update |
| Offline queue | Queue form, sync on reconnect |
| Push | Subscribe, receive, click action |

## 19. Acceptance Criteria

- [ ] App installs and runs offline for cached data.
- [ ] Offline banner shown and actions queued.
- [ ] Push notifications received.
- [ ] Tests pass.

## 20. Implementation Plan

1. Validate PWA config.
2. Add offline detection and UX.
3. Implement API queue with `localforage`.
4. Add backend push endpoints.
5. Test with Lighthouse and real devices.

## 21. Rollback Strategy

- Disable service worker by setting `enabled: false` in `app.module.ts`.
- Clear `localforage` queues.

## 22. Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|---|---|---:|---|
| Aggressive caching stale data | High | Medium | Fine-grained `dataGroups` |
| Multi-tenant cache leakage | High | Medium | Tenant-scoped URLs and keys |
| HTTPS requirement | Medium | High | Document and enforce in deploy |

## 23. Definition of Done

- [ ] SPEC reviewed.
- [ ] PWA features implemented.
- [ ] Tests updated.
- [ ] Documentation updated.

## 24. Key Reminder

> The SPEC is the contract. Respect tenant boundaries and do not over-cache.

## Implementation Status (2026-08)

Partial. Service Worker configuration (`ngsw-config.json`, `ServiceWorkerModule.register`, `manifest.json`, `angular.json` assets) is in place, but there is no offline UX, API queue, push notification, or install prompt implementation.

## References

- <https://angular.io/guide/service-worker-intro>
- <https://angular.io/guide/service-worker-config>
- `Templates/Angular/Eaf.ProjectName.UI/package.json`
