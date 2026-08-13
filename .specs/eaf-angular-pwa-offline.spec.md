# EAF Angular — PWA, Cache and Offline Experience

## Summary

Turn the EAF Angular template into a **Progressive Web App (PWA)** with Service Worker (`@angular/service-worker`), asset and API caching, install support and push notifications, using the `@angular/pwa` and `@angular/service-worker` packages already present in `package.json`.

## Motivation

- `@angular/pwa` and `@angular/service-worker` are already in `package.json` dependencies, but PWA features are not fully activated.
- Mobile users expect to install the app and access data offline.
- ASP.NET Zero offers a .NET MAUI mobile app; a PWA can cover lighter scenarios without app-store publishing.

## Current State

- `package.json` has `@angular/pwa` and `@angular/service-worker`.
- `ngsw-config.json` exists with `assetGroups` and `dataGroups`.
- `manifest.json` is referenced in `angular.json` and `index.html`.
- Service worker is registered in `app.module.ts` for production.
- No offline banner, action queue, push notifications or install prompt logic found in `src/app`.

## Proposed Changes

### 1. Configure Angular PWA
- Run `ng add @angular/pwa` (or keep manual `ngsw-config.json` / `manifest.webmanifest`).
- Register `ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production })`.
- Add `manifest.webmanifest` in `src/assets` with EAF icons, colors and shortcuts.

### 2. Data Cache
- Define `assetGroups` for CSS, JS, fonts and images.
- Define `dataGroups` for API calls with `performance` (dashboard) and `freshness` (edit data) strategies.
- Integrate with `localforage` (already present) for user data, settings and preferences.

### 3. Offline UX
- Detect connectivity (`navigator.onLine` + `online`/`offline` events).
- Show offline banner / snackbar and pending sync indicator.
- Queue user actions (e.g. chat messages) offline and sync when online.

### 4. Push Notifications
- Configure `PushSubscription` in the service worker.
- Backend: endpoint to register subscriptions and send pushes via VAPID.
- Use push for chat, system alerts and approvals.

### 5. Installation and Icons
- Add `beforeinstallprompt` to suggest installation.
- Create 192x192 and 512x512 icons and splash screens.
- Configure `display: standalone`, `theme_color`, `background_color`.

### 6. Background Sync (optional)
- Use `BackgroundSync` to send chat messages and forms offline.

## Implementation Status (2026-08)

Partial. Service Worker configuration (`ngsw-config.json`, `ServiceWorkerModule.register`, `manifest.json`, `angular.json` assets) is in place, but there is no offline UX, API queue, push notification or install prompt implementation.

## Migration Plan
1. Verify current PWA configuration in the template.
2. Create/validate `ngsw-config.json` and `manifest.webmanifest`.
3. Adjust `app.module.ts` to register the service worker.
4. Implement API/UI cache and offline handling.
5. Add backend + frontend push notifications.
6. Test with Lighthouse and real devices.

## Impact
- **Medium**: changes build and assets, adds configuration.
- **High**: significantly improves mobile UX.
- **Medium**: backend must expose push endpoints.

## Risks
- Aggressive caching can cause stale data; requires per-endpoint strategies.
- Multi-tenancy with Service Worker requires tenant-scoped cache care.
- Push notifications require HTTPS and VAPID keys.

## References
- <https://angular.io/guide/service-worker-intro>
- <https://angular.io/guide/service-worker-config>
- `Templates/Angular/Eaf.ProjectName.UI/package.json` — `@angular/pwa`, `@angular/service-worker`
