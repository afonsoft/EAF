# EAF Migration Prompt — 9.4.0 → 9.4.3

## Objective

Apply the changes from EAF **9.4.3** to a project generated from the EAF 9.4.0 template (.NET 10 API + Angular 20). The changes cover the **public registration flow with tenant selection/creation**, the **default `Free` edition**, the **join request** flow and **member approval**.

## Scope

- **Backend (`Templates/Api`):** `ProjectName.Core`, `ProjectName.Application`, `ProjectName.EntityFrameworkCore`, `ProjectName.Web.Host`, seed/migrations.
- **Frontend (`Templates/Angular/Eaf.ProjectName.UI`):** registration screen, request service, admin approval page, navigation.
- **Database:** EF Core migration for `TenantJoinRequest`.

## Prerequisites

- [ ] Migration branch: `git checkout -b migration/eaf-9.4.3`.
- [ ] .NET 10 SDK and Node.js 18+.
- [ ] SQL Server/PostgreSQL accessible.
- [ ] API compiling and running locally (`dotnet build` / `dotnet run`).
- [ ] Update EAF packages/modules to `9.4.3` (or `common.props` for projects referencing the source).

> **Note:** As of the current EAF repository, the features below are already included in the middleware/template. This guide is intended for projects created from EAF 9.4.0 that need to be migrated manually.

---

## 1. Backend — Domain and EF Core

### 1.1 Add the `TenantJoinRequest` entity

In the `ProjectName.Core` project (or `Eaf.ProjectName.Core`), create or update:

```csharp
using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities.Auditing;

namespace Eaf.ProjectName.MultiTenancy
{
    [Table("AbpTenantJoinRequests")]
    public class TenantJoinRequest : CreationAuditedEntity<long>
    {
        [Required]
        public virtual long UserId { get; set; }

        [Required]
        public virtual int TenantId { get; set; }

        [Required]
        public virtual long TenantUserId { get; set; }

        public virtual TenantJoinRequestStatus Status { get; set; }

        [StringLength(512)]
        public virtual string Message { get; set; }

        public virtual long? ApproverUserId { get; set; }
    }

    public enum TenantJoinRequestStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
```

### 1.2 Update `DbContext`

In `ProjectNameDbContext.cs`, add:

```csharp
public virtual DbSet<TenantJoinRequest> TenantJoinRequests { get; set; }
```

And in `OnModelCreating`:

```csharp
modelBuilder.Entity<TenantJoinRequest>(b =>
{
    b.HasIndex(e => new { e.UserId, e.TenantId });
    b.HasIndex(e => e.Status);
    b.Property(e => e.Status).HasConversion<int>();
});
```

### 1.3 Generate migration

```bash
dotnet ef migrations add AddTenantJoinRequest \
  --project src/Eaf.ProjectName.EntityFrameworkCore \
  --startup-project src/Eaf.ProjectName.Web.Host
```

Update the database:

```bash
dotnet ef database update \
  --project src/Eaf.ProjectName.EntityFrameworkCore \
  --startup-project src/Eaf.ProjectName.Web.Host
```

### 1.4 Seed the `Free` edition and default tenant

In `Migrations/Seed/Tenants/DefaultTenantBuilder.cs`, ensure the default tenant is created:

```csharp
var defaultTenant = _context.Tenants.IgnoreQueryFilters()
    .FirstOrDefault(t => t.TenancyName == AbpTenantBase.DefaultTenantName);
if (defaultTenant == null)
{
    defaultTenant = new Tenant(AbpTenantBase.DefaultTenantName, AbpTenantBase.DefaultTenantName);
    _context.Tenants.Add(defaultTenant);
    _context.SaveChanges();
}
```

In `Migrations/Seed/Tenants/TenantRoleAndUserBuilder.cs`, ensure the `Admin` and `User` roles exist (in addition to the admin user already present):

```csharp
var adminRole = _context.Roles.IgnoreQueryFilters()
    .FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
if (adminRole == null)
{
    _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin)
    {
        IsStatic = true,
        IsDefault = true
    });
    _context.SaveChanges();
}

var userRole = _context.Roles.IgnoreQueryFilters()
    .FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
if (userRole == null)
{
    _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User)
    {
        IsStatic = true,
        IsDefault = true
    });
    _context.SaveChanges();
}
```

---

## 2. Backend — Application

### 2.1 Registration DTOs

Update `RegisterInput` to include tenant selection mode:

```csharp
using Eaf.Middleware.Authorization.Accounts.Dto;

namespace Eaf.ProjectName.Authorization.Accounts.Dto
{
    public class RegisterInput
    {
        [Required]
        public TenantSelectionMode TenantSelectionMode { get; set; }

        [StringLength(AbpTenantBase.MaxTenancyNameLength)]
        [RegularExpression(TenantConsts.TenancyNameRegex)]
        public string TenancyName { get; set; }

        [StringLength(TenantConsts.MaxNameLength)]
        public string TenantName { get; set; }

        public int? ExistingTenantId { get; set; }

        [StringLength(512)]
        public string JoinRequestMessage { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
```

Update `RegisterOutput`:

```csharp
public class RegisterOutput
{
    public bool CanLogin { get; set; }
    public int? TenantId { get; set; }
    public string TenancyName { get; set; }
}
```

`TenantSelectionMode`:

```csharp
public enum TenantSelectionMode
{
    DefaultTenant,
    CreateNew,
    JoinExisting
}
```

### 2.2 `TenantJoinRequest` DTOs

Create in `Authorization/Accounts/Dto/`:

```csharp
public class AvailableTenantDto
{
    public int TenantId { get; set; }
    public string TenantName { get; set; }
    public string TenancyName { get; set; }
    public bool IsDefault { get; set; }
}

public class TenantJoinRequestDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; }
    public long TenantUserId { get; set; }
    public TenantJoinRequestStatus Status { get; set; }
    public string Message { get; set; }
    public long? ApproverUserId { get; set; }
    public string CreationTime { get; set; }
}

public class CreateTenantJoinRequestInput
{
    public int TenantId { get; set; }
    [StringLength(512)]
    public string Message { get; set; }
}

public class ApproveTenantJoinRequestInput
{
    public long RequestId { get; set; }
    public bool IsApproved { get; set; }
}
```

### 2.3 `TenantJoinRequestAppService`

Create `Authorization/Accounts/TenantJoinRequestAppService.cs` implementing `IApplicationService` (or an `ITenantJoinRequestAppService` interface):

- `GetAvailableTenantsAsync()` — `[AbpAllowAnonymous]`; returns active tenants (`t.IsActive`).
- `CreateRequestAsync(CreateTenantJoinRequestInput input)` — `[AbpAuthorize]`; calls `ITenantUserManager.CreatePendingMembershipAsync`.
- `GetMyRequestsAsync()` — `[AbpAuthorize]`; requests for the current user.
- `GetPendingRequestsForCurrentTenantAsync()` — `[AbpAuthorize(Pages_Administration_Users)]`; pending requests for the current tenant.
- `ApproveAsync(ApproveTenantJoinRequestInput input)` — `[AbpAuthorize(Pages_Administration_Users)]`; approves (`ITenantUserManager.ApproveMembershipAsync`) or rejects.

> EAF 9.4.3 already exposes `ITenantUserManager` with `CreatePendingMembershipAsync` and `ApproveMembershipAsync`. If using NuGet packages `9.4.3`, inject `ITenantUserManager` and use them. Otherwise, copy `TenantUserManager` from EAF.

### 2.4 Adjust `AccountAppService.Register`

The 9.4.3 implementation follows this flow:

```csharp
public async Task<RegisterOutput> Register(RegisterInput input)
{
    if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.TenantManagement.AllowSelfRegistration))
        throw new UserFriendlyException(L("SelfRegistrationIsDisabled"));

    var hostUser = await CreateHostUserAsync(input);

    switch (input.TenantSelectionMode)
    {
        case TenantSelectionMode.DefaultTenant:
            return await RegisterDefaultTenantAsync(hostUser);
        case TenantSelectionMode.CreateNew:
            return await RegisterCreateNewAsync(input, hostUser);
        case TenantSelectionMode.JoinExisting:
            return await RegisterJoinExistingAsync(input, hostUser);
        default:
            throw new UserFriendlyException(L("InvalidRegisterRequest"));
    }
}
```

- **DefaultTenant**: returns `CanLogin = true`.
- **CreateNew**: validates `AllowTenantCreation`; creates tenant linked to the `Free` edition; creates shadow admin user with `Admin` and `User` roles; returns `CanLogin = true`.
- **JoinExisting**: validates `AllowJoinRequests`; creates an inactive shadow user and a pending `TenantJoinRequest`; returns `CanLogin = false`.

### 2.5 Settings (`AppSettings`)

The new settings exist in EAF 9.4.3 and can be controlled by host and tenant:

```csharp
AppSettings.TenantManagement.AllowSelfRegistration  // default: true
AppSettings.TenantManagement.AllowTenantCreation    // default: true
AppSettings.TenantManagement.AllowJoinRequests      // default: true
```

In the generated project's `appsettings*.json`, add (optional) to override defaults:

```json
{
  "App": {
    "TenantManagement": {
      "AllowSelfRegistration": true,
      "AllowTenantCreation": true,
      "AllowJoinRequests": true
    }
  }
}
```

### 2.6 Login and tenant selection

In `TokenAuthController`:

- `GetAvailableTenants` and `SelectTenant` should allow only host users (`loginResult.User.TenantId` null).
- In `SelectTenant`, after loading the shadow user via `membership.TenantUserId`, validate `shadowUser.IsActive`. If false, throw `UserFriendlyException(L("TenantUserIsNotActive"))`.

### 2.7 Localization

Add the keys to the localization XMLs (`Localization/Source/EafCore.xml` and `EafCore-pt-BR.xml`):

```xml
<text name="TenantRequired">Tenant is required.</text>
<text name="TenantNotFound">Tenant not found.</text>
<text name="OnlyHostUsersCanHaveTenantMemberships">Only host users can have tenant memberships.</text>
<text name="UserAlreadyActiveInTenant">User is already active in this tenant.</text>
<text name="RequestAlreadyProcessed">This request has already been processed.</text>
<text name="FailedToCreateShadowUser">Failed to create the tenant user.</text>
<text name="TenantUserIsNotActive">User is not active in this tenant. Please wait for approval.</text>
<text name="InvalidTenancyName">Invalid tenancy name.</text>
<text name="SelfRegistrationIsDisabled">Self registration is disabled.</text>
<text name="TenantCreationIsDisabled">Tenant creation is disabled.</text>
<text name="JoinRequestsAreDisabled">Join requests are disabled.</text>
<text name="CreateNewTenant">Create new tenant</text>
<text name="SelectTenant">Select tenant</text>
<text name="TenantName">Tenant name</text>
<text name="TenancyName">Tenancy name</text>
<text name="JoinCompany">Join existing tenant</text>
<text name="RegistrationWaitingForApproval">Your registration is waiting for approval.</text>
<text name="SuccessfullyRegistered">Successfully registered.</text>
```

---

## 3. Frontend — Angular

### 3.1 Registration model and service

Update `src/account/register/register.model.ts`:

```typescript
export enum TenantSelectionMode {
  DefaultTenant = 'DefaultTenant',
  CreateNew = 'CreateNew',
  JoinExisting = 'JoinExisting',
}

export class RegisterModel {
  tenantSelectionMode: TenantSelectionMode = TenantSelectionMode.DefaultTenant;
  tenancyName: string;
  tenantName: string;
  existingTenantId: number | undefined;
  joinRequestMessage: string;
  name: string;
  surname: string;
  userName: string;
  emailAddress: string;
  password: string;

  get isDefaultTenant(): boolean {
    return this.tenantSelectionMode === TenantSelectionMode.DefaultTenant;
  }
  get isCreatingTenant(): boolean {
    return this.tenantSelectionMode === TenantSelectionMode.CreateNew;
  }
  get isJoiningTenant(): boolean {
    return this.tenantSelectionMode === TenantSelectionMode.JoinExisting;
  }
}

export class RegisterResult {
  canLogin: boolean;
  tenantId: number | undefined;
  tenancyName: string;
}
```

### 3.2 Registration screen (`register.component.html`)

Add the tenant selection section (when multi-tenancy is enabled):

```html
<div *ngIf="multiTenancy.isEnabled" class="form-group m-form__group">
  <label>{{ 'TenantSelection' | localize }}</label>
  <div class="m-radio-list">
    <label class="m-radio m-radio--primary">
      <input type="radio" name="tenantSelectionMode"
        [value]="tenantSelectionMode.DefaultTenant"
        [(ngModel)]="model.tenantSelectionMode" />
      {{ 'DefaultTenant' | localize }}
      <span></span>
    </label>
    <label class="m-radio m-radio--primary">
      <input type="radio" name="tenantSelectionMode"
        [value]="tenantSelectionMode.CreateNew"
        [(ngModel)]="model.tenantSelectionMode" />
      {{ 'CreateNewTenant' | localize }}
      <span></span>
    </label>
    <label class="m-radio m-radio--primary">
      <input type="radio" name="tenantSelectionMode"
        [value]="tenantSelectionMode.JoinExisting"
        [(ngModel)]="model.tenantSelectionMode" />
      {{ 'JoinCompany' | localize }}
      <span></span>
    </label>
  </div>
</div>

<!-- Conditional fields for CreateNew -->
<div *ngIf="model.isCreatingTenant" class="form-group m-form__group md-form">
  <input [(ngModel)]="model.tenancyName" name="tenancyName" required class="form-control m-input" type="text" />
  <label>{{ 'TenancyName' | localize }}</label>
</div>
<div *ngIf="model.isCreatingTenant" class="form-group m-form__group md-form">
  <input [(ngModel)]="model.tenantName" name="tenantName" class="form-control m-input" type="text" />
  <label>{{ 'TenantName' | localize }}</label>
</div>

<!-- Conditional fields for JoinExisting -->
<div *ngIf="model.isJoiningTenant" class="form-group m-form__group">
  <label>{{ 'Tenant' | localize }}</label>
  <select [(ngModel)]="model.existingTenantId" name="existingTenantId" class="form-control m-input" [required]="model.isJoiningTenant">
    <option [ngValue]="undefined">{{ 'SelectTenant' | localize }}</option>
    <option *ngFor="let tenant of tenants" [ngValue]="tenant.tenantId">{{ tenant.tenantName }}</option>
  </select>
</div>
<div *ngIf="model.isJoiningTenant" class="form-group m-form__group md-form">
  <textarea [(ngModel)]="model.joinRequestMessage" name="joinRequestMessage" class="form-control m-input" rows="3"></textarea>
  <label>{{ 'Message' | localize }}</label>
</div>
```

### 3.3 `register.component.ts`

```typescript
import { TenantJoinRequestService, AvailableTenantDto } from '@shared/service-proxies/tenant-join-request.service';

export class RegisterComponent extends AppComponentBase implements OnInit {
  model = new RegisterModel();
  tenants: AvailableTenantDto[] = [];
  tenantSelectionMode = TenantSelectionMode;

  constructor(
    injector: Injector,
    private readonly _registerService: RegisterService,
    private readonly _tenantJoinRequestService: TenantJoinRequestService,
    private readonly _router: Router,
  ) { super(injector); }

  ngOnInit(): void {
    this.clearSession();
    if (this.multiTenancy.isEnabled) {
      this._tenantJoinRequestService.getAvailableTenants().subscribe(result => {
        this.tenants = result;
      });
    }
  }

  register(): void {
    this.submitting = true;
    this._registerService.register(this.model).subscribe({
      next: (result: RegisterResult) => {
        this.submitting = false;
        if (result.canLogin) {
          this.message.success(this.l('SuccessfullyRegistered'));
          this._router.navigate(['/account/login']);
        } else {
          this.message.info(this.l('RegistrationWaitingForApproval'));
        }
      },
      error: () => { this.submitting = false; }
    });
  }
}
```

### 3.4 `TenantJoinRequestService`

Create `src/shared/service-proxies/tenant-join-request.service.ts`:

```typescript
import { Injectable, Injector } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppConsts } from '@shared/AppConsts';
import { AppComponentBase } from '@shared/common/app-component-base';

export class AvailableTenantDto {
  tenantId: number;
  tenantName: string | undefined;
  tenancyName: string | undefined;
  isDefault: boolean;
}

export class TenantJoinRequestDto { /* ... */ }

export enum TenantJoinRequestStatus { Pending = 0, Approved = 1, Rejected = 2 }

export class CreateTenantJoinRequestInput { tenantId: number; message: string | undefined; }
export class ApproveTenantJoinRequestInput { requestId: number; isApproved: boolean; }

@Injectable()
export class TenantJoinRequestService extends AppComponentBase {
  private readonly _baseUrl = `${AppConsts.remoteServiceBaseUrl}/api/services/app/TenantJoinRequest`;

  constructor(injector: Injector, private readonly _httpClient: HttpClient) { super(injector); }

  getAvailableTenants(): Observable<AvailableTenantDto[]> {
    return this._httpClient.get<AvailableTenantDto[]>(`${this._baseUrl}/GetAvailableTenants`);
  }

  getMyRequests(): Observable<TenantJoinRequestDto[]> {
    return this._httpClient.get<TenantJoinRequestDto[]>(`${this._baseUrl}/GetMyRequests`);
  }

  getPendingRequestsForCurrentTenant(): Observable<TenantJoinRequestDto[]> {
    return this._httpClient.get<TenantJoinRequestDto[]>(`${this._baseUrl}/GetPendingRequestsForCurrentTenant`);
  }

  createRequest(input: CreateTenantJoinRequestInput): Observable<TenantJoinRequestDto> {
    return this._httpClient.post<TenantJoinRequestDto>(`${this._baseUrl}/CreateRequest`, input);
  }

  approve(input: ApproveTenantJoinRequestInput): Observable<void> {
    return this._httpClient.post<void>(`${this._baseUrl}/Approve`, input);
  }
}
```

Register the service in `src/shared/service-proxies/service-proxy.module.ts`.

### 3.5 Admin approval page

Create `src/app/admin/tenant-join-requests/`:

- `tenant-join-requests.component.ts`: list pending requests via `getPendingRequestsForCurrentTenant()` and call `approve({requestId, isApproved: true/false})`.
- `tenant-join-requests.component.html`: table with user/tenant name, message and Approve/Reject buttons.
- Add the route in `admin-routing.module.ts`:
  ```typescript
  { path: 'tenant-join-requests', component: TenantJoinRequestsComponent }
  ```
- Add the menu item in `app-navigation.service.ts`:
  ```typescript
  new AppMenuItem('TenantJoinRequests', 'Pages.Administration.Users', 'flaticon-user-add', '/app/admin/tenant-join-requests'),
  ```

### 3.6 Update `service-proxies`

If the project uses `nswag` to generate proxies automatically, start the API and run:

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npm install
npm run service-update
```

> The `service-update` script reads `http://localhost:8001/swagger/v1/swagger.json` (as configured in `service.config.nswag`). Make sure the API is running on port `8001` with Swagger exposed.

If NSwag is not used, keep the manual services created above.

---

## 4. Validation

### 4.1 Backend

```bash
dotnet build Eaf.ProjectName.sln -c Release
dotnet test Eaf.ProjectName.sln -c Release
```

### 4.2 Frontend

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npm install --legacy-peer-deps
npx ng build --configuration=production
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

### 4.3 Docker (optional, recommended)

```bash
export MSSQL_SA_PASSWORD="Your_password123"
bash scripts/validate-docker-compose.sh
```

---

## 5. Migration Checklist

- [ ] `TenantJoinRequest` entity created in `Core`.
- [ ] `DbSet<TenantJoinRequest>` and index configuration in `DbContext`.
- [ ] Migration `AddTenantJoinRequest` generated and database updated.
- [ ] `Free` edition seed and `Admin`/`User` tenant roles.
- [ ] `RegisterInput`/`RegisterOutput` with `TenantSelectionMode`.
- [ ] `AccountAppService.Register` implementing the three modes.
- [ ] `TenantJoinRequestAppService` and DTOs created.
- [ ] `TokenAuthController.SelectTenant` validates `shadowUser.IsActive`.
- [ ] Localizations added/verified.
- [ ] Angular registration screen with tenant selection.
- [ ] `TenantJoinRequestService` created and registered.
- [ ] Admin approval page created and route/menu added.
- [ ] `service-proxies` updated via NSwag.
- [ ] Build, tests and Docker Compose validated.
