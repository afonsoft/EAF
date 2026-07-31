---
name: testing-eaf-angular-runtime
license: GPL-3.0-or-later
description: 'Stand up a runnable EAF Angular admin UI with a lightweight mock backend to test panels like chat, UI customization, and SignalR-dependent features without the full .NET/SQL stack.'
---

# Testing EAF Angular Runtime with a Mock Backend

Use this when you need to manually verify the `Templates/Angular/Eaf.ProjectName.UI` admin UI in a browser without starting the .NET API or SQL Server.

## Prerequisites

- Node `v20.20.x` via nvm (`nvm use 20`).
- `npm install --legacy-peer-deps` in `Templates/Angular/Eaf.ProjectName.UI`.
- The postinstall script should patch `devtools-ignore-plugin.js` for the `.map` BOM issue. If `ng serve`/`ng build` still fails on a `.map` file, strip the BOM from the offending `src/assets/.../*.map`.

## Devin Secrets Needed

None for this harness.

## Steps

1. **Pick or create a Node/Express mock API** on `localhost:3000` that serves:
   - `POST /api/TokenAuth/Authenticate`
   - `GET /AbpUserConfiguration/GetAll` (with `auth.grantedPermissions`, `features['App.ChatFeature']`, `session.user`, `user`, `localization.values`, `setting.values`)
   - `GET /api/services/app/Session/GetCurrentLoginInformations` (with `theme.baseSettings.header.headerSkin`)
   - `GET /api/services/app/Profile/GetProfilePicture`
   - `GET /api/services/app/Chat/GetUserChatFriendsWithSettings`
   - `GET /api/services/app/Chat/GetUserChatMessages`
   - `POST /api/services/app/Chat/MarkAllUnreadMessagesOfUserAsRead`
   - `GET /api/services/app/UiCustomizationSettings/GetUiManagementSettings`
   - `POST /api/services/app/UiCustomizationSettings/UpdateDefaultUiManagementSettings`
   - CORS headers must include `Access-Control-Allow-Headers` for `Cache-Control`, `Pragma`, `Expires`, `X-Correlation-ID`, `.AspNetCore.Culture`, `Abp.Localization.CultureName`, `Accept-Language`, `Abp-TenantId`, `X-Requested-With`.

2. **Point the UI at the mock API:**
   - `src/assets/appconfig.json` and `src/assets/appconfig.Local.json` are both used by the production build; set `remoteServiceBaseUrl` to `http://localhost:3000`.

3. **Disable the service worker for the runtime harness** so stale cached assets do not loop reloads:
   - Temporarily set `production: false` in `src/environments/environment.build.ts`.
   - Revert before final static `npx ng build --configuration=production` and `npx tsc`.

4. **Patch SignalR for chat-only tests** because the mock does not expose `/signalr-chat`:
   - Temporarily patch `src/app/shared/layout/chat/chat-signalr.service.ts` to set `isChatConnected = true` and trigger `app.chat.connected` plus echo `sendMessage`.
   - Revert before final checks.

5. **Serve the production `dist/` with SPA fallback:**
   - `npx ng build --configuration=production` (or use a harness build with SW disabled).
   - `npx serve -s dist -l 4200` provides fallback to `index.html` for deep routes.

6. **Open a fresh Chrome profile** and navigate to `http://localhost:4200/app/admin/ui-customization`.
   - The chat panel is conditionally rendered by `app.component.html` once `chatConnected` is true.
   - The chat header is in `#chatSideRight .bs-canvas-header` and updates via `chat-bar.component.{ts,html,css}`.

7. **Verify theme changes** by updating `headerSkin` through UI customization or by calling the mock update endpoint and reloading.

## Common Issues

- **CORS preflight fails** because the mock does not list all request headers; add `Cache-Control`, `Pragma`, `Expires`, and `X-Correlation-ID` to `Access-Control-Allow-Headers`.
- **Service worker serves stale assets and reload loops** when testing rapid builds; disable it for harness builds.
- **`appconfig.Local.json` overrides `appconfig.json`** in production builds; update both.
- **`topbar.component.ts` `showChat()` only adds `mr-0`** and may not visibly reopen the panel in the current Metronic layout; the close/pin/send behavior is more reliable through direct `chat-bar` controls.
- **Legibility classes may not match expectations**: `text-light`/`text-dark` on the chat header title and pin icon are overridden by theme/metronic CSS, so computed colors may differ from the class names.

## Cleanup

- Revert `chat-signalr.service.ts`, `environment.build.ts`, `appconfig*.json`.
- Delete any temporary `proxy.conf.json` and stop the mock server.
- Run `npx tsc -p src/tsconfig.app.json --noEmit` and `npx ng build --configuration=production` on the reverted source.
