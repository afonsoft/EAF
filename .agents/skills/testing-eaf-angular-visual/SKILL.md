---
name: testing-eaf-angular-visual
description: Visual end-to-end verification of the EAF Angular admin UI using Playwright, covering login, dashboard, /account/gateway-selection and /app/admin/subscriptions.
---

# Testing EAF Angular UI visually with Playwright

## Scope

Use this skill after the full Docker stack is healthy or when an Angular dev server is reachable, to verify that the admin UI renders, logs in, and shows the payment/subscription screens without console errors.

## Devin Secrets Needed

None for local stack testing. Default local admin password must be known (see `HostRoleAndUserCreator.cs` / `TenantRoleAndUserBuilder.cs` for the seeded sample password).

## Environment

- Angular UI URL: `http://localhost:4200`
- API URL: `http://localhost:5000`
- Default seeded host admin user: `admin`
- The password is the ABP sample password used by `HostRoleAndUserBuilder` unless it has already been reset.
- Node version: match `Templates/Angular/Eaf.ProjectName.UI/package.json` (currently `>=18 <22`); prefer `nvm use 20`.

## Playwright selectors

- Login username field: `#userNameOrEmailAddress`
- Login password field: `#Password`
- Login submit button: `button[type='submit']`
- Dashboard container: `#TenantDashboard`
- Dashboard data area: `#TenantDashboard .m-content`
- Gateway selection page: `#GatewayName`, `#GatewayEdition`, `#GatewayPaymentPeriod`
- Subscriptions page: `#SubscriptionsFilterText`, `p-table`, `app-empty-state`

## Quick visual check

```bash
python3 .agents/skills/testing-eaf-angular-visual/verify-eaf-angular.py
```

The script above performs the following:

1. Opens `http://localhost:4200/account/login`.
2. Fills `admin` credentials and submits.
3. Waits for `/app/main/dashboard` and checks the `#TenantDashboard` content.
4. Navigates to `/account/gateway-selection` and waits for `#GatewayName`/`#GatewayEdition`.
5. Navigates to `/app/admin/subscriptions` and waits for `#SubscriptionsFilterText`/`p-table`/`app-empty-state`.
6. Captures screenshots under `screenshots/`:
   - `01-login-page.png`
   - `02-dashboard.png`
   - `03-gateway-selection.png`
   - `04-subscriptions.png`
7. Reports any `error`/`severe` console messages, page errors, or 5xx responses.

## Common issues and workarounds

- The login form may show a social/external login view first. If `#Password` is not visible, click the `LoginSistem` or `Back` link to switch to the normal form.
- Localized page titles can differ from the localization key. Match the visible text (e.g. `Selecionar gateway`, `Gateway selection`) or wait for known elements instead of exact strings.
- `app-empty-state` may appear for pages with no data. That is a valid render; assertions should accept either data tables or the empty-state component.
- Run `CHROME_BIN=/home/ubuntu/.local/bin/google-chrome` for Karma/Angular unit tests if the packaged Chromium is not found.
