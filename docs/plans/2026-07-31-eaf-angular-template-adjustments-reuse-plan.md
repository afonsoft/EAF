# Reusable plan: applying EAF Angular template adjustments to another repository

## 1. Goal

Reproduzir, em outro repositório EAF/ABP Angular, os mesmos ajustes de Templates aplicados nesta sessão: melhorias no Payments, testes unitários, responsividade mobile e tela de Dashboard.

## 2. Assumptions

- O repositório alvo usa a mesma arquitetura EAF/ABP com Angular + PrimeNG + ngx-bootstrap + Metronic.
- Os service proxies (`payment.service-proxy.ts`, `organization-unit.service-proxy.ts`, `user-delegation.service-proxy.ts`, `mass-notification.service-proxy.ts`, `dashboard.service-proxy.ts`) já existem ou serão gerados a partir do backend.
- O arquivo `test-helpers/mock-services.ts` segue o padrão do EAF.

## 3. Steps

### 3.1 Payments component

1. Open `src/app/admin/payments/payments.component.ts`.
2. Add helper methods:
   - `getEditionDisplayName(editionId: number): string`
   - `getStatusClass(status: string): string`
   - `getStatusLabel(status: string): string`
3. Open `src/app/admin/payments/payments.component.html`.
4. Replace the `editionId` column display with `getEditionDisplayName(record.editionId)`.
5. Replace raw `record.status` with `<span [ngClass]="getStatusClass(record.status)">{{ getStatusLabel(record.status) }}</span>`.
6. Add localization key `Gateway` to the source XML files.

### 3.2 Organization Units component

1. Open `src/app/admin/organization-units/organization-units.component.ts`.
2. Add member and role management state and methods.
3. Open `src/app/admin/organization-units/organization-units.component.html`.
4. Add action buttons for `ManageMembers` and `ManageRoles` permissions.
5. Add `membersModal` and `rolesModal` ngx-bootstrap modals with user/role dropdowns and lists.
6. Add localization keys: `ManageMembers`, `ManageRoles`, `Add`, `OrganizationUnitUserRemoveWarningMessage`, `OrganizationUnitRoleRemoveWarningMessage`.

### 3.3 User Delegations component

1. Open `src/app/admin/user-delegations/user-delegations.component.ts`.
2. Inject `UserServiceProxy` and load users into `allUsers`.
3. Replace the numeric `targetUserId` input with a `<select>` bound to `allUsers`.
4. Add validation: `targetUserId` required, start time before end time.
5. Add localization keys: `UserDelegationCancelWarningMessage`, `StartTimeMustBeLessThanEndTime`.

### 3.4 Dashboard component

1. Open `src/app/main/dashboard/dashboard.component.ts`.
2. Ensure it calls `DashboardServiceProxy.getHostDashboard()` or `getTenantDashboard()` based on `appSession.tenantId`.
3. Open `src/app/main/dashboard/dashboard.component.html`.
4. Add an `<app-empty-state>` when no tiles are returned.
5. Ensure cards use responsive Bootstrap grid classes (`col-xl-3 col-lg-4 col-md-6`).

### 3.5 Angular unit tests

1. Add mocks to `src/test-helpers/mock-services.ts` for every service proxy used by the new components.
2. Create `*.component.spec.ts` files beside each new admin component.
3. Provide the framework mocks (`LocalizationService`, `PermissionCheckerService`, `MessageService`, `NotifyService`, `AppSessionService`, etc.) and the service mocks.
4. Add at least one `should create` test plus one behavior test per component.

### 3.6 Mobile responsive CSS

1. Open `src/assets/common/styles/styles.css`.
2. Add or update the `@media (max-width: 768px)` block:
   - Set `min-width` on `.p-datatable table` to avoid horizontal clipping on mobile.
   - Set `min-width`/`min-height: 44px` for `.btn-sm`.
3. Add `@media (max-width: 576px)` block:
   - Constrain `.modal-dialog` to `calc(100vw - 1rem)`.
   - Stack `.form-row .col-md-*` columns to 100%.

### 3.7 Localization

1. Identify all new keys introduced in step 3.1–3.4.
2. Add them to the English source file (e.g. `src/Eaf.Middleware.Core/Localization/Source/EafCore.xml`).
3. Add the same keys to the pt-BR source file (e.g. `EafCore-pt-BR.xml`).
4. Avoid duplicates: grep for the key before adding.

### 3.8 Validation

Run, from the Angular folder:

```bash
npx tsc -p src/tsconfig.app.json --noEmit
npx tsc -p src/tsconfig.spec.json --noEmit
npx ng build --configuration=production
```

Run, from the .NET solution root:

```bash
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --configuration Release --no-build
```

## 4. Notes

- Do not edit `service-proxies.ts` files manually; regenerate them from the backend when DTOs change.
- Prefer adding keys to localization XML over hard-coding labels.
- Keep modals without `role="dialog"`/`role="document"` to avoid Sonar warnings.
