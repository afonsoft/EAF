# TenantUserManager e Shadow Users no EAF

Este documento detalha o serviço de domínio `TenantUserManager`, responsável por vincular usuários host a tenants, criar *shadow users* e replicar permissões.

## `UserTenantMembership`

A entidade `UserTenantMembership` é host-level (sem `TenantId` próprio, mas com coluna `TenantId` referenciando o tenant associado):

```csharp
[Table("AbpUserTenantMemberships")]
public class UserTenantMembership : CreationAuditedEntity<long>
{
    public virtual long UserId { get; set; }       // host user id
    public virtual int TenantId { get; set; }      // tenant selecionado
    public virtual long TenantUserId { get; set; }   // shadow user id dentro do tenant
    public virtual bool IsDefault { get; set; }    // login automático
}
```

A tabela possui índice único em `(UserId, TenantId)` e índice em `TenantUserId`.

## `ITenantUserManager`

```csharp
public interface ITenantUserManager : IDomainService
{
    Task<UserTenantMembership> EnsureMembershipAsync(long hostUserId, int tenantId, bool isDefault = false);
    Task RemoveMembershipAsync(long hostUserId, int tenantId);
    Task SetDefaultAsync(long hostUserId, int tenantId);
    Task<long?> GetTenantUserIdAsync(long hostUserId, int tenantId);
    Task<IReadOnlyList<UserTenantMembership>> GetMembershipsAsync(long hostUserId);
}
```

### `EnsureMembershipAsync`

Regras:

1. Apenas usuários host (`TenantId == null`) podem ter memberships.
2. Obtém o usuário host e suas roles com o filtro `MayHaveTenant` desabilitado.
3. Entra no escopo do tenant (`SetTenantId(tenantId)` + `EnableFilter(MayHaveTenant)`) para procurar/criar o shadow user.
4. Para cada role do host, replica as permissões no tenant e adiciona a role ao shadow user.
5. Insere a `UserTenantMembership` no escopo host (filtro `MayHaveTenant` desabilitado).

### `RemoveMembershipAsync`

Remove a associação host ↔ tenant, sem deletar o shadow user.

### `SetDefaultAsync`

Define o tenant padrão para o host user e limpa o flag `IsDefault` dos demais.

## Controle do filtro `MayHaveTenant`

No EAF/ABP, `SetTenantId` altera apenas o parâmetro do filtro. Se o filtro foi desabilitado anteriormente, ele **não é reabilitado automaticamente**. Por isso o `TenantUserManager` chama explicitamente:

```csharp
using (CurrentUnitOfWork.SetTenantId(null, switchMustHaveTenantEnableDisable: false))
using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
{
    // ler host user e roles
}

using (CurrentUnitOfWork.SetTenantId(tenantId, switchMustHaveTenantEnableDisable: false))
using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
{
    // criar shadow user e replicar roles no tenant
}
```

## `TenantRolePermissionReplicationService`

Serviço de domínio auxiliar que garante que uma role exista no tenant e copia as permissões da role host para ela.

```csharp
public interface ITenantRolePermissionReplicationService : IDomainService
{
    Task EnsureRoleInTenantAsync(int tenantId, string roleName, IEnumerable<string> permissionNames);
    Task CopyRolePermissionsFromHostAsync(int tenantId, string roleName);
}
```

`CopyRolePermissionsFromHostAsync` executa:

1. Busca a role host com `MayHaveTenant` desabilitado.
2. Obtém as permissões concedidas à role host.
3. Chama `EnsureRoleInTenantAsync(tenantId, roleName, hostPermissions)`.

`EnsureRoleInTenantAsync` executa dentro de `SetTenantId(tenantId)` + `EnableFilter(MayHaveTenant)`:

1. Verifica se o tenant existe.
2. Procura a role no tenant pelo nome.
3. Se não existir, cria.
4. Concede as permissões informadas.

## Uso em um Controller

```csharp
using (var tenantUserManager = _iocManager.ResolveAsDisposable<ITenantUserManager>())
{
    var membership = await tenantUserManager.Object.EnsureMembershipAsync(hostUserId, tenantId);
    // membership.TenantUserId contém o shadow user id
}
```

## Considerações

- O shadow user é criado com uma senha aleatória (`User.CreateRandomPassword()`) e `IsActive = true`.
- O `UserName` do shadow user é o mesmo do host user. Se houver conflito de username no tenant, o `TenantUserManager` encontrará o usuário existente e o usará como shadow user.
- As permissões são replicadas quando a membership é criada; alterações posteriores nas roles host não são sincronizadas automaticamente.
