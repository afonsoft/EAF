# EAF — Multi-Tenancy, Login em Duas Etapas e Shadow Users

> **For agentic workers:** REQUIRED SUB- SKILL: Use `superpowers:subagent-driven-development` (recommended) or `superpowers:executing-plans` to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Tornar o EAF capaz de autenticar um usuário host, listar os tenants aos quais ele pertence, permitir a seleção de um tenant e emitir um JWT escopado para o *shadow user* dentro do tenant, replicando roles/permissões do host.

**Architecture:**
- Entidade host `UserTenantMembership` liga *host user* → tenant → *shadow user*.
- `TenantRolePermissionReplicationService` cria roles no tenant e copia apenas permissões válidas para tenant.
- `TenantUserManager` cria/atualiza *shadow users* e replica roles, respeitando o filtro `MayHaveTenant`.
- `TokenAuthController` expõe `GetAvailableTenants` e `SelectTenant` reutilizando os helpers de JWT do EAF.
- Template Angular substitui o fluxo de login por duas etapas (credenciais → seleção de tenant → token).

**Tech Stack:** .NET 10 / C# 14, ABP 10.4, EF Core 10, Angular 18, xUnit / NSubstitute / Shouldly.

---

## Escopo

### Dentro do escopo
- Backend genérico no `Eaf.Middleware.Core` e `Eaf.Middleware.Web.Core`.
- Frontend no template Angular (`Eaf.ProjectName.UI`).
- Testes BDD em português (`Dado_Quando_Entao`).
- Migrations no template API e no `SampleAppDbContext` dos testes.
- Documentação em português.

### Fora do escopo (específico do GameHub)
- `GameplayBridgeService` e `HubAuthService` (SDK de jogos).
- Tenant `Player` e `GameHubConsts.PlayerTenantName`.
- Regra "chat sempre no tenant `Player`".

---

## Símbolos (ordem de fluxo de dados)

### 1. Modelo de dados

#### `UserTenantMembership`

```csharp
// src/Eaf.Middleware.Core/MultiTenancy/UserTenantMembership.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Liga um usuário host (TenantId == null) a um tenant e ao shadow user criado dentro desse tenant.
    /// </summary>
    [Table("AbpUserTenantMemberships")]
    public class UserTenantMembership : CreationAuditedEntity<long>
    {
        [Required]
        public virtual long UserId { get; set; }

        [Required]
        public virtual int TenantId { get; set; }

        [Required]
        public virtual long TenantUserId { get; set; }

        /// <summary>
        /// Indica se este é o tenant padrão para login automático.
        /// </summary>
        public virtual bool IsDefault { get; set; }
    }
}
```

Configuração de índices em `ProjectNameDbContext` e `SampleAppDbContext`:

```csharp
modelBuilder.Entity<UserTenantMembership>(b =>
{
    b.HasIndex(e => new { e.UserId, e.TenantId }).IsUnique();
    b.HasIndex(e => e.TenantUserId);
});
```

---

### 2. Serviços de domínio

#### `ITenantRolePermissionReplicationService`

```csharp
// src/Eaf.Middleware.Core/MultiTenancy/ITenantRolePermissionReplicationService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Replica roles e permissões do host para um tenant.
    /// </summary>
    public interface ITenantRolePermissionReplicationService : IDomainService
    {
        /// <summary>
        /// Cria a role no tenant caso não exista. Se <paramref name="permissionNames"/> for nulo,
        /// copia as permissões da role homônima do host.
        /// </summary>
        Task EnsureRoleInTenantAsync(int tenantId, string roleName, IEnumerable<string> permissionNames = null);

        /// <summary>
        /// Copia permissões concedidas da role do host para a role do tenant.
        /// </summary>
        Task CopyRolePermissionsFromHostAsync(int tenantId, string roleName);
    }
}
```

#### `ITenantUserManager`

```csharp
// src/Eaf.Middleware.Core/MultiTenancy/ITenantUserManager.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Gerencia associações entre usuários host e tenants, mantendo shadow users.
    /// </summary>
    public interface ITenantUserManager : IDomainService
    {
        /// <summary>
        /// Garante que o usuário host possui uma membership no tenant, criando/atualizando o shadow user e replicando roles.
        /// </summary>
        Task<UserTenantMembership> EnsureMembershipAsync(long hostUserId, int tenantId, bool isDefault = false);

        /// <summary>
        /// Remove a membership do usuário host no tenant, incluindo o shadow user.
        /// </summary>
        Task RemoveMembershipAsync(long hostUserId, int tenantId);

        /// <summary>
        /// Define o tenant padrão do usuário host, limpando o flag IsDefault dos demais.
        /// </summary>
        Task SetDefaultAsync(long hostUserId, int tenantId);

        /// <summary>
        /// Retorna o Id do shadow user dentro do tenant, ou null se não houver membership.
        /// </summary>
        Task<long?> GetTenantUserIdAsync(long hostUserId, int tenantId);

        /// <summary>
        /// Lista todas as memberships de um usuário host.
        /// </summary>
        Task<IList<UserTenantMembership>> GetMembershipsAsync(long hostUserId);
    }
}
```

#### `TenantUserManager` (assinatura + dependências)

```csharp
// src/Eaf.Middleware.Core/MultiTenancy/TenantUserManager.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.UI;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Localization;
using Microsoft.EntityFrameworkCore;

namespace Eaf.Middleware.MultiTenancy
{
    public class TenantUserManager : DomainService, ITenantUserManager
    {
        private readonly IRepository<UserTenantMembership, long> _membershipRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly ITenantRolePermissionReplicationService _roleReplicationService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TenantUserManager(
            IRepository<UserTenantMembership, long> membershipRepository,
            IRepository<User, long> userRepository,
            IRepository<Tenant> tenantRepository,
            UserManager userManager,
            RoleManager roleManager,
            ITenantRolePermissionReplicationService roleReplicationService,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _membershipRepository = membershipRepository;
            _userRepository = userRepository;
            _tenantRepository = tenantRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _roleReplicationService = roleReplicationService;
            _unitOfWorkManager = unitOfWorkManager;
            LocalizationSourceName = MiddlewareLocalizationHelper.DefaultSourceName;
        }

        public virtual async Task<UserTenantMembership> EnsureMembershipAsync(long hostUserId, int tenantId, bool isDefault = false)
        {
            // 1. Valida tenant e carrega host user com MayHaveTenant desabilitado.
            // 2. Recusa se o usuário não for host (TenantId != null).
            // 3. Busca/cria membership.
            // 4. Cria ou atualiza shadow user dentro de SetTenantId(tenantId) + EnableFilter(MayHaveTenant).
            // 5. Replica roles do host para o shadow user.
            // 6. Ajusta flag IsDefault.
        }

        public virtual async Task RemoveMembershipAsync(long hostUserId, int tenantId) { /* ... */ }
        public virtual async Task SetDefaultAsync(long hostUserId, int tenantId) { /* ... */ }
        public virtual async Task<long?> GetTenantUserIdAsync(long hostUserId, int tenantId) { /* ... */ }
        public virtual async Task<IList<UserTenantMembership>> GetMembershipsAsync(long hostUserId) { /* ... */ }

        private async Task<User> CreateOrUpdateShadowUserAsync(User hostUser, int tenantId, long? existingShadowUserId)
        {
            // Reabilita MayHaveTenant explicitamente. SetTenantId sozinho não reabilita o filtro.
            using (CurrentUnitOfWork.SetTenantId(tenantId, switchMustHaveTenantEnableDisable: false))
            using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
            {
                // Cria ou atualiza shadow user copiando nome, email, active, password hash e security stamp do host.
            }
        }

        private async Task ReplicateHostRolesAsync(User hostUser, int tenantId, User shadowUser)
        {
            // Lê roles do host com MayHaveTenant desabilitado.
            // Para cada role, chama EnsureRoleInTenantAsync e depois AddToRoleAsync no shadow user.
        }
    }
}
```

#### `TenantRolePermissionReplicationService` (assinatura)

```csharp
// src/Eaf.Middleware.Core/MultiTenancy/TenantRolePermissionReplicationService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Data;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Eaf.Middleware.Authorization.Roles;
using Microsoft.EntityFrameworkCore;

namespace Eaf.Middleware.MultiTenancy
{
    public class TenantRolePermissionReplicationService : DomainService, ITenantRolePermissionReplicationService
    {
        private readonly RoleManager _roleManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public TenantRolePermissionReplicationService(
            RoleManager roleManager,
            IPermissionManager permissionManager,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _roleManager = roleManager;
            _permissionManager = permissionManager;
            _unitOfWorkManager = unitOfWorkManager;
            LocalizationSourceName = MiddlewareLocalizationHelper.DefaultSourceName;
        }

        public virtual async Task EnsureRoleInTenantAsync(int tenantId, string roleName, IEnumerable<string> permissionNames = null)
        {
            // Dentro de SetTenantId(tenantId) + EnableFilter(MayHaveTenant):
            //   - Busca role pelo nome normalizado.
            //   - Se não existir, cria Role(tenantId, roleName, roleName).
            //   - Se permissionNames for null, chama CopyRolePermissionsFromHostAsync.
            //   - Senão, converte nomes em Permission objects e chama _roleManager.SetGrantedPermissionsAsync.
        }

        public virtual async Task CopyRolePermissionsFromHostAsync(int tenantId, string roleName)
        {
            // Com MayHaveTenant desabilitado e filtrando TenantId == null, lê a role do host.
            // Obtém as permissões concedidas e filtra apenas as com MultiTenancySides.Tenant.
            // Chama EnsureRoleInTenantAsync passando os nomes filtrados.
        }
    }
}
```

---

### 3. API Web

#### DTOs

```csharp
// src/Eaf.Middleware.Web.Core/Models/TokenAuth/AvailableTenantsModel.cs
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    public class AvailableTenantsModel
    {
        [Required]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        [DisableAuditing]
        public string CaptchaResponse { get; set; }
    }
}
```

```csharp
// src/Eaf.Middleware.Web.Core/Models/TokenAuth/SelectTenantModel.cs
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    public class SelectTenantModel : AvailableTenantsModel
    {
        [Range(1, int.MaxValue)]
        public int TenantId { get; set; }

        public bool RememberClient { get; set; }
    }
}
```

```csharp
// src/Eaf.Middleware.Web.Core/Models/TokenAuth/AvailableTenantResult.cs
namespace Eaf.Middleware.Web.Models.TokenAuth
{
    public class AvailableTenantResult
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenancyName { get; set; }
        public bool IsDefault { get; set; }
    }
}
```

#### `TokenAuthController` novos endpoints e helpers

```csharp
// src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs

[AbpAllowAnonymous]
[HttpPost]
public virtual async Task<ListResultDto<AvailableTenantResult>> GetAvailableTenants([FromBody] AvailableTenantsModel model)
{
    // Valida captcha se habilitado, autentica host user com tenancyName: null,
    // obtém memberships via ITenantUserManager e mapeia para AvailableTenantResult.
}

[AbpAllowAnonymous]
[HttpPost]
public virtual async Task<AuthenticateResultModel> SelectTenant([FromBody] SelectTenantModel model)
{
    // 1. Valida captcha e autentica host user.
    // 2. Verifica membership no tenant via GetTenantUserIdAsync.
    // 3. Dentro de SetTenantId(model.TenantId) + EnableFilter(MayHaveTenant), carrega shadow user.
    // 4. Cria ClaimsIdentity e chama CreateJwtClaims(shadowUser, model.TenantId).
    // 5. Gera access token, refresh token e cookie.
    // 6. Retorna AuthenticateResultModel padrão do EAF.
}

// Helpers a adicionar no controller:
private async Task<IEnumerable<Claim>> CreateJwtClaims(ClaimsIdentity identity, User user, int? tenantId, string externalAuthProviderformation = "")
{
    // Mesma implementação do CreateJwtClaims atual, mas usando tenantId ao invés de AbpSession.TenantId.
}

private async Task<ClaimsIdentity> CreateIdentityForUserAsync(User user)
{
    // Cria ClaimsIdentity com UserId, UserName, Name e role claims do UserManager.
}

private async Task<RefreshTokenInfo> GenerateAndStoreRefreshTokenAsync(User user, int? tenantId = null)
{
    // Sobrecarga que recebe tenantId explicitamente; existentes passam AbpSession.TenantId.
}
```

---

### 4. Frontend Angular

#### Modelos TypeScript

```typescript
// Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.service.ts
export interface AvailableTenantResult {
  tenantId: number;
  tenantName: string;
  tenancyName: string;
  isDefault: boolean;
}

export interface SelectTenantModel {
  userNameOrEmailAddress: string;
  password: string;
  tenantId: number;
  rememberClient: boolean;
  captchaResponse?: string;
}
```

#### `LoginService` métodos novos

```typescript
// Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.service.ts
import { HttpClient } from '@angular/common/http';

constructor(
  injector: Injector,
  private readonly _tokenAuthService: TokenAuthServiceProxy,
  private readonly _httpClient: HttpClient,
  // ... restante das dependências
) { }

availableTenants(model: AuthenticateModel, finallyCallback?: () => void): Observable<AvailableTenantResult[]> {
  return this._httpClient.post<AvailableTenantResult[]>(
    AppConsts.remoteServiceBaseUrl + '/api/TokenAuth/GetAvailableTenants',
    model
  ).pipe(finalize(finallyCallback || (() => {})));
}

selectTenant(model: SelectTenantModel): Observable<AuthenticateResultModel> {
  return this._httpClient.post<AuthenticateResultModel>(
    AppConsts.remoteServiceBaseUrl + '/api/TokenAuth/SelectTenant',
    model
  );
}

loginTenant(result: AuthenticateResultModel, tenantId: number, redirectUrl?: string): void {
  const tokenExpireDate = this.rememberMe
    ? new Date(Date.now() + 10000 * result.expireInSeconds)
    : new Date(Date.now() + 1000 * result.expireInSeconds);

  this._tokenService.setToken(result.accessToken, tokenExpireDate, tenantId);
  this._storageService.setCookieValue(
    AppConsts.authorization.encrptedAuthTokenName,
    result.encryptedAccessToken,
    tokenExpireDate,
    eaf.appPath
  );
  this._storageService.setCookieValue(
    AppConsts.expirationToken.keyName,
    result.expireInSeconds.toString(),
    null,
    eaf.appPath
  );

  if (redirectUrl) {
    setTimeout(() => { location.href = redirectUrl; }, 200);
  } else {
    let initialUrl = UrlHelper.initialUrl;
    if (initialUrl && initialUrl.indexOf('/account') > 0) {
      initialUrl = AppConsts.appBaseUrl;
    }
    setTimeout(() => { location.href = initialUrl || AppConsts.appBaseUrl; }, 200);
  }
}
```

#### `TokenService.setToken` com `tenantId`

```typescript
// Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/auth/token.service.ts
setToken(authToken: string, expireDate?: Date, tenantId?: number): void {
  this.storageService.setCookieValue(eaf.auth.tokenCookieName, authToken, expireDate, eaf.appPath, eaf.domain);
  if (tenantId !== undefined && tenantId !== null) {
    eaf.multiTenancy.setTenantIdCookie(tenantId.toString());
  }
}
```

#### `SelectTenantComponent`

```typescript
// Templates/Angular/Eaf.ProjectName.UI/src/account/select-tenant/select-tenant.component.ts
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AvailableTenantResult, LoginService } from '../login/login.service';

@Component({
  selector: 'app-select-tenant',
  standalone: false,
  templateUrl: './select-tenant.component.html'
})
export class SelectTenantComponent {
  @Input() tenants: AvailableTenantResult[] = [];
  @Input() credentials: any;
  @Output() tenantSelected = new EventEmitter<AvailableTenantResult>();

  selectedTenant: AvailableTenantResult;

  constructor(public loginService: LoginService) {}

  select(tenant: AvailableTenantResult): void {
    this.selectedTenant = tenant;
    this.tenantSelected.emit(tenant);
  }
}
```

#### `LoginComponent` fluxo de duas etapas

```typescript
// Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.component.ts
showTenantSelection = false;
availableTenants: AvailableTenantResult[] = [];

login(): void {
  this.submitting = true;
  this.loginService.availableTenants(
    this.loginService.authenticateModel,
    () => this.submitting = false
  ).subscribe(tenants => {
    this.submitting = false;
    this.availableTenants = tenants;

    if (tenants.length === 0) {
      this.message.error(this.l('UserHasNoAssociatedTenants'));
      return;
    }

    if (tenants.length === 1 && AppConsts.autoSelectSingleTenant) {
      this.selectTenant(tenants[0]);
    } else {
      this.showTenantSelection = true;
    }
  });
}

selectTenant(tenant: AvailableTenantResult): void {
  this.submitting = true;
  const model: SelectTenantModel = {
    userNameOrEmailAddress: this.loginService.authenticateModel.userNameOrEmailAddress,
    password: this.loginService.authenticateModel.password,
    tenantId: tenant.tenantId,
    rememberClient: this.loginService.rememberMe,
    captchaResponse: this.loginService.authenticateModel.captchaResponse
  };

  this.loginService.selectTenant(model).subscribe(
    result => this.loginService.loginTenant(result, tenant.tenantId),
    () => this.submitting = false
  );
}
```

#### `AppConsts`

```typescript
// Templates/Angular/Eaf.ProjectName.UI/src/shared/AppConsts.ts
export class AppConsts {
  static autoSelectSingleTenant = true; // quando true e houver 1 tenant, faz login direto
  // ... demais propriedades
}
```

---

## Tarefas

### Task 1: Entidade `UserTenantMembership` e configuração EF

**Files:**
- Create: `src/Eaf.Middleware.Core/MultiTenancy/UserTenantMembership.cs`
- Modify: `Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContext.cs`
- Modify: `test/Eaf.MiddlewareCore.SampleApp/EntityFramework/SampleAppDbContext.cs`
- Create: migration `AddUserTenantMembership` no template EF Core

- [ ] **Step 1: Criar entidade `UserTenantMembership`**

```csharp
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace Eaf.Middleware.MultiTenancy
{
    [Table("AbpUserTenantMemberships")]
    public class UserTenantMembership : CreationAuditedEntity<long>
    {
        [Required]
        public virtual long UserId { get; set; }

        [Required]
        public virtual int TenantId { get; set; }

        [Required]
        public virtual long TenantUserId { get; set; }

        public virtual bool IsDefault { get; set; }
    }
}
```

- [ ] **Step 2: Adicionar `DbSet` e índice em `ProjectNameDbContext`**

```csharp
public virtual DbSet<UserTenantMembership> UserTenantMemberships { get; set; }

// Dentro de OnModelCreating:
modelBuilder.Entity<UserTenantMembership>(b =>
{
    b.HasIndex(e => new { e.UserId, e.TenantId }).IsUnique();
    b.HasIndex(e => e.TenantUserId);
});
```

- [ ] **Step 3: Adicionar `DbSet` e índice em `SampleAppDbContext`**

```csharp
public DbSet<UserTenantMembership> UserTenantMemberships { get; set; }

// Dentro de OnModelCreating:
modelBuilder.Entity<UserTenantMembership>(b =>
{
    b.HasIndex(e => new { e.UserId, e.TenantId }).IsUnique();
    b.HasIndex(e => e.TenantUserId);
});
```

- [ ] **Step 4: Gerar migration no template**

Run:
```bash
cd Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore
dotnet ef migrations add AddUserTenantMembership --startup-project ../Eaf.ProjectName.Web.Host/Eaf.ProjectName.Web.Host.csproj
```

Expected: migration criada sem erros.

- [ ] **Step 5: Commit**

```bash
git add src/Eaf.Middleware.Core/MultiTenancy/UserTenantMembership.cs \
        Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContext.cs \
        test/Eaf.MiddlewareCore.SampleApp/EntityFramework/SampleAppDbContext.cs \
        "Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/Migrations/*AddUserTenantMembership*"
git commit -m "feat(multi-tenancy): add UserTenantMembership entity and migrations"
```

---

### Task 2: Serviço de replicação de roles e permissões

**Files:**
- Create: `src/Eaf.Middleware.Core/MultiTenancy/ITenantRolePermissionReplicationService.cs`
- Create: `src/Eaf.Middleware.Core/MultiTenancy/TenantRolePermissionReplicationService.cs`
- Modify: `src/Eaf.Middleware.Core/Localization/Source/EafCore.xml`
- Modify: `src/Eaf.Middleware.Core/Localization/Source/EafCore-pt-BR.xml`

- [ ] **Step 1: Criar interface**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Eaf.Middleware.MultiTenancy
{
    public interface ITenantRolePermissionReplicationService : IDomainService
    {
        Task EnsureRoleInTenantAsync(int tenantId, string roleName, IEnumerable<string> permissionNames = null);
        Task CopyRolePermissionsFromHostAsync(int tenantId, string roleName);
    }
}
```

- [ ] **Step 2: Implementar `TenantRolePermissionReplicationService`**

Regras:
- Sempre executar dentro de `SetTenantId(tenantId)` + `EnableFilter(AbpDataFilters.MayHaveTenant)` para operações no tenant.
- Para ler a role do host, desabilitar `MayHaveTenant` e filtrar `TenantId == null`.
- Copiar apenas permissões com `MultiTenancySides.HasFlag(MultiTenancySides.Tenant)`.
- Usar `PermissionManager.GetPermissionsFromNamesByValidating` + `RoleManager.SetGrantedPermissionsAsync`.

- [ ] **Step 3: Adicionar chaves de localização**

`EafCore.xml`:
```xml
<text name="TenantDoesNotExist">Tenant does not exist.</text>
<text name="OnlyHostUsersCanBeAssociatedWithMultipleTenants">Only host users can be associated with multiple tenants.</text>
<text name="UserIsNotMemberOfTenant">User is not a member of the tenant.</text>
<text name="UserIsNotAssociatedWithSelectedTenant">User is not associated with the selected tenant.</text>
<text name="UserHasNoAssociatedTenants">User has no associated tenants.</text>
```

`EafCore-pt-BR.xml`:
```xml
<text name="TenantDoesNotExist" value="O tenant não existe." />
<text name="OnlyHostUsersCanBeAssociatedWithMultipleTenants" value="Apenas usuários host podem estar associados a múltiplos tenants." />
<text name="UserIsNotMemberOfTenant" value="O usuário não é membro do tenant." />
<text name="UserIsNotAssociatedWithSelectedTenant" value="O usuário não está associado ao tenant selecionado." />
<text name="UserHasNoAssociatedTenants" value="O usuário não possui tenants associados." />
```

- [ ] **Step 4: Commit**

```bash
git add src/Eaf.Middleware.Core/MultiTenancy/ITenantRolePermissionReplicationService.cs \
        src/Eaf.Middleware.Core/MultiTenancy/TenantRolePermissionReplicationService.cs \
        src/Eaf.Middleware.Core/Localization/Source/EafCore.xml \
        src/Eaf.Middleware.Core/Localization/Source/EafCore-pt-BR.xml
git commit -m "feat(multi-tenancy): add tenant role and permission replication service"
```

---

### Task 3: `TenantUserManager`

**Files:**
- Create: `src/Eaf.Middleware.Core/MultiTenancy/ITenantUserManager.cs`
- Create: `src/Eaf.Middleware.Core/MultiTenancy/TenantUserManager.cs`

- [ ] **Step 1: Criar interface `ITenantUserManager`**

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Services;

namespace Eaf.Middleware.MultiTenancy
{
    public interface ITenantUserManager : IDomainService
    {
        Task<UserTenantMembership> EnsureMembershipAsync(long hostUserId, int tenantId, bool isDefault = false);
        Task RemoveMembershipAsync(long hostUserId, int tenantId);
        Task SetDefaultAsync(long hostUserId, int tenantId);
        Task<long?> GetTenantUserIdAsync(long hostUserId, int tenantId);
        Task<IList<UserTenantMembership>> GetMembershipsAsync(long hostUserId);
    }
}
```

- [ ] **Step 2: Implementar `TenantUserManager`**

Pontos críticos:
- `EnsureMembershipAsync` deve carregar o host user e as memberships com `_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant)`.
- A criação/atualização do shadow user deve ocorrer dentro de:
  ```csharp
  using (CurrentUnitOfWork.SetTenantId(tenantId, switchMustHaveTenantEnableDisable: false))
  using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
  ```
- O shadow user deve copiar `Name`, `Surname`, `EmailAddress`, `IsActive`, `Password`, `SecurityStamp`, `ProfilePictureId` do host.
- As roles do host devem ser lidas **fora** do contexto do tenant (com filtro desabilitado) e depois replicadas dentro do tenant.

- [ ] **Step 3: Commit**

```bash
git add src/Eaf.Middleware.Core/MultiTenancy/ITenantUserManager.cs \
        src/Eaf.Middleware.Core/MultiTenancy/TenantUserManager.cs
git commit -m "feat(multi-tenancy): add TenantUserManager for shadow users"
```

---

### Task 4: Endpoints `GetAvailableTenants` e `SelectTenant`

**Files:**
- Create: `src/Eaf.Middleware.Web.Core/Models/TokenAuth/AvailableTenantsModel.cs`
- Create: `src/Eaf.Middleware.Web.Core/Models/TokenAuth/SelectTenantModel.cs`
- Create: `src/Eaf.Middleware.Web.Core/Models/TokenAuth/AvailableTenantResult.cs`
- Modify: `src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs`

- [ ] **Step 1: Criar DTOs**

`AvailableTenantsModel`:
```csharp
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    public class AvailableTenantsModel
    {
        [Required]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [DisableAuditing]
        public string Password { get; set; }

        [DisableAuditing]
        public string CaptchaResponse { get; set; }
    }
}
```

`SelectTenantModel`:
```csharp
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    public class SelectTenantModel : AvailableTenantsModel
    {
        [Range(1, int.MaxValue)]
        public int TenantId { get; set; }

        public bool RememberClient { get; set; }
    }
}
```

`AvailableTenantResult`:
```csharp
namespace Eaf.Middleware.Web.Models.TokenAuth
{
    public class AvailableTenantResult
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public string TenancyName { get; set; }
        public bool IsDefault { get; set; }
    }
}
```

- [ ] **Step 2: Adicionar `ITenantUserManager` no construtor do `TokenAuthController`**

```csharp
private readonly ITenantUserManager _tenantUserManager;

public TokenAuthController(
    // ... parâmetros existentes
    ITenantUserManager tenantUserManager)
{
    // ... atribuições existentes
    _tenantUserManager = tenantUserManager;
}
```

- [ ] **Step 3: Implementar `GetAvailableTenants` e `SelectTenant`**

`GetAvailableTenants`:
```csharp
[AbpAllowAnonymous]
[HttpPost]
public virtual async Task<ListResultDto<AvailableTenantResult>> GetAvailableTenants([FromBody] AvailableTenantsModel model)
{
    if (!ModelState.IsValid)
        throw new UserFriendlyException(L("InvalidRequest"));

    if (UseCaptchaOnLogin())
        await ValidateReCaptcha(model.CaptchaResponse);

    var loginResult = await GetLoginResultAsync(
        model.UserNameOrEmailAddress.ToLower().Trim(),
        model.Password,
        tenancyName: null);

    var memberships = await _tenantUserManager.GetMembershipsAsync(loginResult.User.Id);

    var results = memberships.Select(m => new AvailableTenantResult
    {
        TenantId = m.TenantId,
        TenantName = _tenantRepository.Get(m.TenantId).Name,
        TenancyName = _tenantRepository.Get(m.TenantId).TenancyName,
        IsDefault = m.IsDefault
    }).ToList();

    return new ListResultDto<AvailableTenantResult>(results);
}
```

> Nota: mapeamento do tenant deve preferir uma consulta separada (`IRepository<Tenant>`) para evitar dependência de navigation property.

`SelectTenant`:
```csharp
[AbpAllowAnonymous]
[HttpPost]
public virtual async Task<AuthenticateResultModel> SelectTenant([FromBody] SelectTenantModel model)
{
    if (!ModelState.IsValid)
        throw new UserFriendlyException(L("InvalidRequest"));

    if (UseCaptchaOnLogin())
        await ValidateReCaptcha(model.CaptchaResponse);

    var loginResult = await GetLoginResultAsync(
        model.UserNameOrEmailAddress.ToLower().Trim(),
        model.Password,
        tenancyName: null);

    var tenantUserId = await _tenantUserManager.GetTenantUserIdAsync(loginResult.User.Id, model.TenantId);
    if (!tenantUserId.HasValue)
        throw new UserFriendlyException(L("UserIsNotAssociatedWithSelectedTenant"));

    User shadowUser;
    ClaimsIdentity identity;
    using (CurrentUnitOfWork.SetTenantId(model.TenantId, switchMustHaveTenantEnableDisable: false))
    using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
    {
        shadowUser = await _userManager.GetUserByIdAsync(tenantUserId.Value);
        identity = await CreateIdentityForUserAsync(shadowUser);
    }

    var expiration = model.RememberClient
        ? TimeSpan.FromDays(365)
        : TimeSpan.FromSeconds(await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration));

    var accessToken = CreateAccessToken(await CreateJwtClaims(identity, shadowUser, model.TenantId), expiration);
    var refreshToken = await GenerateAndStoreRefreshTokenAsync(shadowUser, model.TenantId);
    AppendRefreshTokenCookie(refreshToken.Token, refreshToken.ExpireDate);

    return new AuthenticateResultModel
    {
        AccessToken = accessToken,
        EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
        ExpireInSeconds = (int)expiration.TotalSeconds,
        UserId = shadowUser.Id
    };
}
```

- [ ] **Step 4: Refatorar helpers de JWT**

Adicionar overload `CreateJwtClaims(ClaimsIdentity identity, User user, int? tenantId, string externalAuthProviderformation = "")` e refatorar o método existente para chamá-lo passando `AbpSession.TenantId`.

Adicionar `CreateIdentityForUserAsync(User user)` e `GenerateAndStoreRefreshTokenAsync(User user, int? tenantId = null)`.

- [ ] **Step 5: Commit**

```bash
git add src/Eaf.Middleware.Web.Core/Models/TokenAuth/AvailableTenantsModel.cs \
        src/Eaf.Middleware.Web.Core/Models/TokenAuth/SelectTenantModel.cs \
        src/Eaf.Middleware.Web.Core/Models/TokenAuth/AvailableTenantResult.cs \
        src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs
git commit -m "feat(multi-tenancy): add GetAvailableTenants and SelectTenant endpoints"
```

---

### Task 5: Template Angular — login em duas etapas

**Files:**
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/shared/AppConsts.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/auth/token.service.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.service.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.component.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.component.html`
- Create: `Templates/Angular/Eaf.ProjectName.UI/src/account/select-tenant/select-tenant.component.ts`
- Create: `Templates/Angular/Eaf.ProjectName.UI/src/account/select-tenant/select-tenant.component.html`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/account/account.module.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/account/account-routing.module.ts` (se necessário)

- [ ] **Step 1: Adicionar `autoSelectSingleTenant` em `AppConsts`**

```typescript
export class AppConsts {
  static autoSelectSingleTenant = true;
  // ...
}
```

- [ ] **Step 2: Atualizar `TokenService.setToken`**

```typescript
setToken(authToken: string, expireDate?: Date, tenantId?: number): void {
  this.storageService.setCookieValue(eaf.auth.tokenCookieName, authToken, expireDate, eaf.appPath, eaf.domain);
  if (tenantId !== undefined && tenantId !== null) {
    eaf.multiTenancy.setTenantIdCookie(tenantId.toString());
  }
}
```

- [ ] **Step 3: Adicionar métodos em `LoginService`**

```typescript
availableTenants(model: AuthenticateModel, finallyCallback?: () => void): Observable<AvailableTenantResult[]> {
  return this._httpClient.post<AvailableTenantResult[]>(
    AppConsts.remoteServiceBaseUrl + '/api/TokenAuth/GetAvailableTenants',
    model
  ).pipe(finalize(finallyCallback || (() => {})));
}

selectTenant(model: SelectTenantModel): Observable<AuthenticateResultModel> {
  return this._httpClient.post<AuthenticateResultModel>(
    AppConsts.remoteServiceBaseUrl + '/api/TokenAuth/SelectTenant',
    model
  );
}

loginTenant(result: AuthenticateResultModel, tenantId: number, redirectUrl?: string): void {
  const tokenExpireDate = this.rememberMe
    ? new Date(Date.now() + 10000 * result.expireInSeconds)
    : new Date(Date.now() + 1000 * result.expireInSeconds);

  this._tokenService.setToken(result.accessToken, tokenExpireDate, tenantId);
  this._storageService.setCookieValue(AppConsts.authorization.encrptedAuthTokenName, result.encryptedAccessToken, tokenExpireDate, eaf.appPath);
  this._storageService.setCookieValue(AppConsts.expirationToken.keyName, result.expireInSeconds.toString(), null, eaf.appPath);

  if (redirectUrl) {
    setTimeout(() => { location.href = redirectUrl; }, 200);
  } else {
    let initialUrl = UrlHelper.initialUrl;
    if (initialUrl && initialUrl.indexOf('/account') > 0) {
      initialUrl = AppConsts.appBaseUrl;
    }
    setTimeout(() => { location.href = initialUrl || AppConsts.appBaseUrl; }, 200);
  }
}
```

- [ ] **Step 4: Criar `SelectTenantComponent`**

`select-tenant.component.ts` e `select-tenant.component.html` conforme assinaturas na seção "Símbolos".

- [ ] **Step 5: Alterar `LoginComponent` e `login.component.html`**

- Remover o dropdown de seleção de tenant antes do login.
- Implementar `login()` chamando `availableTenants`.
- Implementar `selectTenant(tenant)` chamando `selectTenant` e `loginTenant`.
- Renderizar `<app-select-tenant *ngIf="showTenantSelection" ...>`.

- [ ] **Step 6: Registrar componente e módulo**

Adicionar `SelectTenantComponent` em `AccountModule` `declarations`.

- [ ] **Step 7: Commit**

```bash
git add Templates/Angular/Eaf.ProjectName.UI/src/shared/AppConsts.ts \
        Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/auth/token.service.ts \
        Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.service.ts \
        Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.component.ts \
        Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.component.html \
        Templates/Angular/Eaf.ProjectName.UI/src/account/select-tenant/ \
        Templates/Angular/Eaf.ProjectName.UI/src/account/account.module.ts \
        Templates/Angular/Eaf.ProjectName.UI/src/account/account-routing.module.ts
git commit -m "feat(angular): two-step login with tenant selection"
```

---

### Task 6: Testes

**Files:**
- Create: `test/Eaf.MiddlewareCore.Tests/MultiTenancy/TenantRolePermissionReplicationServiceBddTests.cs`
- Create: `test/Eaf.MiddlewareCore.Tests/MultiTenancy/TenantUserManagerBddTests.cs`
- Modify: `test/Eaf.MiddlewareCore.Tests/Helpers/CoreManagerTestHelper.cs` (adicionar `CreateTenantUserManager`)
- Create/Modify: `test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs` (novos testes)

- [ ] **Step 1: Testes de `TenantRolePermissionReplicationService`**

```csharp
[Fact]
public async Task Dado_RoleHostComPermissoesTenant_Quando_EnsureRoleInTenant_Entao_CopiaPermissoes()
{
    // Dado
    var (roleManager, permissionManager, service) = CriarService();

    // Quando
    await service.EnsureRoleInTenantAsync(1, "Admin");

    // Então
    await roleManager.Received(1).SetGrantedPermissionsAsync(Arg.Any<Role>(), Arg.Any<IEnumerable<Permission>>());
}
```

- [ ] **Step 2: Testes de `TenantUserManager`**

```csharp
[Fact]
public async Task Dado_UsuarioHost_Quando_EnsureMembership_Entao_CriaShadowUser()
{
    // Dado
    var manager = CriarTenantUserManager();
    var hostUser = new User { Id = 1, TenantId = null, UserName = "admin" };

    // Quando
    var membership = await manager.EnsureMembershipAsync(hostUser.Id, 1);

    // Então
    membership.ShouldNotBeNull();
    membership.TenantUserId.ShouldBeGreaterThan(0);
}

[Fact]
public async Task Dado_UsuarioTenant_Quando_EnsureMembership_Entao_LancaExcecao()
{
    // Dado
    var manager = CriarTenantUserManager();
    var tenantUser = new User { Id = 2, TenantId = 1, UserName = "tenantuser" };

    // Quando / Então
    await Should.ThrowAsync<UserFriendlyException>(async () =>
        await manager.EnsureMembershipAsync(tenantUser.Id, 1));
}
```

- [ ] **Step 3: Testes de `TokenAuthController`**

```csharp
[Fact]
public async Task Dado_CredenciaisValidasSemMembership_Quando_SelectTenant_Entao_LancaExcecao()
{
    // Dado
    var controller = CriarController(...);
    var model = new SelectTenantModel { UserNameOrEmailAddress = "admin", Password = "123qwe", TenantId = 2 };

    // Quando / Então
    await Should.ThrowAsync<UserFriendlyException>(async () =>
        await controller.SelectTenant(model));
}
```

- [ ] **Step 4: Commit**

```bash
git add test/Eaf.MiddlewareCore.Tests/MultiTenancy/TenantRolePermissionReplicationServiceBddTests.cs \
        test/Eaf.MiddlewareCore.Tests/MultiTenancy/TenantUserManagerBddTests.cs \
        test/Eaf.MiddlewareCore.Tests/Helpers/CoreManagerTestHelper.cs \
        test/Eaf.Middleware.Web.Core.Tests/Controllers/TokenAuthControllerBddTests.cs
git commit -m "test(multi-tenancy): add TenantUserManager, role replication and auth controller tests"
```

---

### Task 7: Documentação e verificação final

**Files:**
- Create: `docs/eaf-multi-tenant-login.md`
- Create: `docs/eaf-tenant-user-manager.md`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/README.md` (novas telas)

- [ ] **Step 1: Escrever `docs/eaf-multi-tenant-login.md`**

Conteúdo mínimo:
- Fluxo host → `GetAvailableTenants` → `SelectTenant` → JWT escopado.
- Diagrama em Mermaid ou texto.
- Configuração de `AppConsts.autoSelectSingleTenant`.

- [ ] **Step 2: Escrever `docs/eaf-tenant-user-manager.md`**

Conteúdo mínimo:
- Explicação sobre `MayHaveTenant`, `SetTenantId` e `EnableFilter`.
- Exemplo de uso de `ITenantUserManager`.
- Aviso: `SetTenantId` não reabilita `MayHaveTenant` se estiver desabilitado.

- [ ] **Step 3: Verificar build e testes**

Run:
```bash
dotnet restore Eaf.sln
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

Expected: build e testes passam.

Run Angular:
```bash
cd Templates/Angular/Eaf.ProjectName.UI
nvm use 18
npm install --legacy-peer-deps
npx ng build --configuration=production
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

Expected: build e testes passam.

- [ ] **Step 4: Commit final**

```bash
git add docs/eaf-multi-tenant-login.md docs/eaf-tenant-user-manager.md \
        Templates/Angular/Eaf.ProjectName.UI/README.md
git commit -m "docs(multi-tenancy): add two-step login and tenant user manager docs"
```

---

## Coverage check

- `UserTenantMembership`: testes via `TenantUserManager` e `SampleAppDbContext`.
- `TenantRolePermissionReplicationService`: testes de cópia de permissões.
- `TenantUserManager`: testes de criação, validação de host, roles, default, remoção.
- `TokenAuthController`: testes de `GetAvailableTenants` e `SelectTenant` (sucesso e falhas).
- Angular: specs para `SelectTenantComponent` e `LoginService`.

## Notas de implementação

- Não modificar `service-proxies.ts` (gerado). Usar `HttpClient` direto para os novos endpoints.
- O `Abp.TenantId` header já é adicionado pelo `EafHttpInterceptor` a partir do cookie; nenhuma alteração no interceptor é necessária.
- SDK/Bridge (`GameplayBridgeService`, `HubAuthService`) e o tenant `Player` são específicos do GameHub e permanecem fora do escopo.
- As migrations devem ser geradas no template API e a configuração do `SampleAppDbContext` deve ser atualizada para testes (não é necessário migration nos testes, pois usam `EnsureCreated`).
