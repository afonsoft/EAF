# Prompt de Migração — EAF 9.4.0 → 9.4.3

## Objetivo

Aplicar, em um projeto gerado a partir do template EAF 9.4.0 (API .NET 10 + Angular 20), as mudanças da **versão 9.4.3** relacionadas ao fluxo de **cadastro público com seleção/criação de tenant**, **edição `Free` padrão**, **solicitação de ingresso** e **aprovação de membros**.

## Escopo

- **Backend (`Templates/Api`):** `ProjectName.Core`, `ProjectName.Application`, `ProjectName.EntityFrameworkCore`, `ProjectName.Web.Host`, seed/migrations.
- **Frontend (`Templates/Angular/Eaf.ProjectName.UI`):** tela de registro, serviço de solicitações, tela admin de aprovação, navegação.
- **Banco de dados:** migration EF Core para `TenantJoinRequest`.

## Pré-requisitos

- [ ] Branch de migração: `git checkout -b migration/eaf-9.4.3`.
- [ ] .NET 10 SDK e Node.js 18+.
- [ ] Banco SQL Server/PostgreSQL acessível.
- [ ] API compilando e rodando localmente (`dotnet build` / `dotnet run`).
- [ ] Atualizar pacotes/módulos EAF para `9.4.3` (ou `common.props` para projetos que referenciam o fonte).

---

## 1. Backend — Domínio e EF Core

### 1.1 Adicionar entidade `TenantJoinRequest`

No projeto `ProjectName.Core` (ou `Eaf.ProjectName.Core`), crie ou atualize:

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

### 1.2 Atualizar `DbContext`

Em `ProjectNameDbContext.cs`, adicione:

```csharp
public virtual DbSet<TenantJoinRequest> TenantJoinRequests { get; set; }
```

E no `OnModelCreating`:

```csharp
modelBuilder.Entity<TenantJoinRequest>(b =>
{
    b.HasIndex(e => new { e.UserId, e.TenantId });
    b.HasIndex(e => e.Status);
    b.Property(e => e.Status).HasConversion<int>();
});
```

### 1.3 Gerar migration

```bash
dotnet ef migrations add AddTenantJoinRequest \
  --project src/Eaf.ProjectName.EntityFrameworkCore \
  --startup-project src/Eaf.ProjectName.Web.Host
```

Atualize o banco:

```bash
dotnet ef database update \
  --project src/Eaf.ProjectName.EntityFrameworkCore \
  --startup-project src/Eaf.ProjectName.Web.Host
```

### 1.4 Seed da edição `Free` e tenant padrão

Em `Migrations/Seed/Tenants/DefaultTenantBuilder.cs`, garanta que o tenant padrão seja criado:

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

Em `Migrations/Seed/Tenants/TenantRoleAndUserBuilder.cs`, garanta as roles `Admin` e `User` (além do admin user já existente):

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

## 2. Backend — Aplicação

### 2.1 DTOs de registro

Atualize `RegisterInput` para incluir modo de seleção de tenant:

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

Atualize `RegisterOutput`:

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

### 2.2 DTOs de `TenantJoinRequest`

Crie em `Authorization/Accounts/Dto/`:

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

### 2.3 AppService `TenantJoinRequestAppService`

Crie `Authorization/Accounts/TenantJoinRequestAppService.cs` implementando `IApplicationService` (ou uma interface `ITenantJoinRequestAppService`):

- `GetAvailableTenantsAsync()` — `[AbpAllowAnonymous]`; retorna tenants ativos (`t.IsActive`).
- `CreateRequestAsync(CreateTenantJoinRequestInput input)` — `[AbpAuthorize]`; chama `ITenantUserManager.CreatePendingMembershipAsync`.
- `GetMyRequestsAsync()` — `[AbpAuthorize]`; solicitações do usuário logado.
- `GetPendingRequestsForCurrentTenantAsync()` — `[AbpAuthorize(Pages_Administration_Users)]`; pendentes do tenant atual.
- `ApproveAsync(ApproveTenantJoinRequestInput input)` — `[AbpAuthorize(Pages_Administration_Users)]`; aprova (`ITenantUserManager.ApproveMembershipAsync`) ou rejeita.

> Os serviços do EAF 9.4.3 já expõem `ITenantUserManager` com os métodos `CreatePendingMembershipAsync` e `ApproveMembershipAsync`. Se estiver usando os pacotes NuGet `9.4.3`, injete `ITenantUserManager` e utilize-os. Caso contrário, copie a implementação do `TenantUserManager` do EAF.

### 2.4 Ajustar `AccountAppService.Register`

A implementação do `AccountAppService.Register` em 9.4.3 segue o fluxo:

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

- **DefaultTenant**: retorna `CanLogin = true`.
- **CreateNew**: valida `AllowTenantCreation`; cria tenant vinculado à edição `Free`; cria shadow user admin com roles `Admin` e `User`; retorna `CanLogin = true`.
- **JoinExisting**: valida `AllowJoinRequests`; cria shadow user inativo e `TenantJoinRequest` pendente; retorna `CanLogin = false`.

### 2.5 Configurações (`AppSettings`)

As novas settings existem no EAF 9.4.3 e podem ser controladas por host e tenant:

```csharp
AppSettings.TenantManagement.AllowSelfRegistration  // default: true
AppSettings.TenantManagement.AllowTenantCreation  // default: true
AppSettings.TenantManagement.AllowJoinRequests    // default: true
```

No `appsettings*.json` do projeto gerado, adicione (opcional) para sobrescrever defaults:

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

### 2.6 Login e seleção de tenant

No `TokenAuthController`:

- `GetAvailableTenants` e `SelectTenant` devem permitir apenas usuários host (`loginResult.User.TenantId` nulo).
- Em `SelectTenant`, após carregar o shadow user via `membership.TenantUserId`, valide `shadowUser.IsActive`. Se falso, lance `UserFriendlyException(L("TenantUserIsNotActive"))`.

### 2.7 Localização

Adicione as chaves nos XMLs de localização (`Localization/Source/EafCore.xml` e `EafCore-pt-BR.xml`):

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

### 3.1 Model e serviço de registro

Atualize `src/account/register/register.model.ts`:

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

### 3.2 Tela de registro (`register.component.html`)

Adicione a seção de seleção de tenant (exemplo, multi-tenancy habilitado):

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

<!-- Campos condicionais CreateNew -->
<div *ngIf="model.isCreatingTenant" class="form-group m-form__group md-form">
  <input [(ngModel)]="model.tenancyName" name="tenancyName" required class="form-control m-input" type="text" />
  <label>{{ 'TenancyName' | localize }}</label>
</div>
<div *ngIf="model.isCreatingTenant" class="form-group m-form__group md-form">
  <input [(ngModel)]="model.tenantName" name="tenantName" class="form-control m-input" type="text" />
  <label>{{ 'TenantName' | localize }}</label>
</div>

<!-- Campos condicionais JoinExisting -->
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

### 3.4 Serviço `TenantJoinRequestService`

Crie `src/shared/service-proxies/tenant-join-request.service.ts`:

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

Registre o serviço em `src/shared/service-proxies/service-proxy.module.ts`.

### 3.5 Tela admin de aprovação

Crie `src/app/admin/tenant-join-requests/`:

- `tenant-join-requests.component.ts`: lista pendentes via `getPendingRequestsForCurrentTenant()` e chama `approve({requestId, isApproved: true/false})`.
- `tenant-join-requests.component.html`: tabela com nome do usuário/tenant, mensagem e botões Aprovar/Rejeitar.
- Adicione rota em `admin-routing.module.ts`:
  ```typescript
  { path: 'tenant-join-requests', component: TenantJoinRequestsComponent }
  ```
- Adicione o item de menu em `app-navigation.service.ts`:
  ```typescript
  new AppMenuItem('TenantJoinRequests', 'Pages.Administration.Users', 'flaticon-user-add', '/app/admin/tenant-join-requests'),
  ```

### 3.6 Atualizar `service-proxies`

Se o projeto usa `nswag` para gerar proxies automaticamente, suba a API e execute:

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npm install
npm run service-update
```

> O script `service-update` lê `http://localhost:8001/swagger/v1/swagger.json` (conforme `service.config.nswag`). Certifique-se de que a API está rodando na porta `8001` com o Swagger exposto.

Caso não use NSwag, mantenha os serviços manuais criados acima.

---

## 4. Validação

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

### 4.3 Docker (opcional, mas recomendado)

```bash
export MSSQL_SA_PASSWORD="Your_password123"
bash scripts/validate-docker-compose.sh
```

---

## 5. Checklist de migração

- [ ] Entidade `TenantJoinRequest` criada no `Core`.
- [ ] `DbSet<TenantJoinRequest>` e configuração de índices no `DbContext`.
- [ ] Migration `AddTenantJoinRequest` gerada e banco atualizado.
- [ ] Seed de edição `Free` e roles `Admin`/`User` no tenant.
- [ ] `RegisterInput`/`RegisterOutput` com `TenantSelectionMode`.
- [ ] `AccountAppService.Register` implementando três modos.
- [ ] `TenantJoinRequestAppService` e DTOs criados.
- [ ] `TokenAuthController.SelectTenant` validando `shadowUser.IsActive`.
- [ ] Localizações adicionadas/verificadas.
- [ ] Tela de registro Angular com seleção de tenant.
- [ ] Serviço `TenantJoinRequestService` criado e registrado.
- [ ] Tela admin de aprovação criada e rota/menu adicionados.
- [ ] `service-proxies` atualizados via NSwag.
- [ ] Build, testes e Docker Compose validados.
