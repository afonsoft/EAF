# Login Multi-Tenant em Duas Etapas no EAF

Este documento descreve o fluxo de autenticação multi-tenant nativo do EAF, em que um **usuário host** pode pertencer a vários tenants e seleciona o tenant desejado após a autenticação inicial.

## Quando usar

- O projeto habilita `MultiTenancy` e precisa permitir que um mesmo usuário host seja membro de vários tenants.
- O login deve ser feito em duas etapas: credenciais no host, depois escolha do tenant.
- Cada tenant recebe um *shadow user* com as mesmas roles/permissões do host replicadas automaticamente.

## Fluxo

```text
Usuário informa usuário/senha
      |
      v
TokenAuth/GetAvailableTenants (host)
      |
      v
Lista de tenants do usuário
      |
      +-- 1 tenant + autoSelectSingleTenant = true  --> SelectTenant automático
      |
      +-- > 1 tenant  --> Tela de seleção (SelectTenantComponent)
                              |
                              v
                    TokenAuth/SelectTenant
                              |
                              v
                    JWT escopado ao tenant + cookies
```

## Símbolos principais

| Camada | Arquivo | Responsabilidade |
|---|---|---|
| Core | `Eaf.Middleware.Core/MultiTenancy/UserTenantMembership.cs` | Entidade que liga host user, tenant e shadow user |
| Core | `Eaf.Middleware.Core/MultiTenancy/TenantUserManager.cs` | Cria/gerencia memberships e shadow users |
| Core | `Eaf.Middleware.Core/MultiTenancy/TenantRolePermissionReplicationService.cs` | Replica roles/permissões do host para o tenant |
| Web.Core | `Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs` | Endpoints `GetAvailableTenants` e `SelectTenant` |
| Angular | `Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.component.ts` | Inicia o fluxo de duas etapas |
| Angular | `Templates/Angular/Eaf.ProjectName.UI/src/account/login/select-tenant/select-tenant.component.ts` | Tela de seleção de tenant |
| Angular | `Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.service.ts` | Serviço `availableTenants`/`selectTenant`/`loginTenant` |

## Ativação no template Angular

Em `src/shared/AppConsts.ts`:

```typescript
static readonly multiTenancy = {
  twoStepLogin: true,
};

static autoSelectSingleTenant = true; // se houver só um tenant, entra direto
```

Quando `twoStepLogin` for `true`, o componente `LoginComponent` chama `GetAvailableTenants` ao invés de `Authenticate`. Se houver apenas um tenant e `autoSelectSingleTenant` estiver habilitado, o `SelectTenant` é chamado automaticamente.

## Endpoints da API

### `POST api/TokenAuth/GetAvailableTenants`

Autentica o usuário no host e retorna os tenants aos quais ele está associado.

Request:
```json
{
  "userNameOrEmailAddress": "admin",
  "password": "123qwe"
}
```

Response:
```json
[
  {
    "tenantId": 1,
    "tenantName": "Default",
    "tenancyName": "Default",
    "isDefault": true
  }
]
```

### `POST api/TokenAuth/SelectTenant`

Autentica o usuário no host, verifica a membership para o tenant informado e retorna um token JWT escopado ao shadow user dentro do tenant.

Request:
```json
{
  "userNameOrEmailAddress": "admin",
  "password": "123qwe",
  "tenantId": 1
}
```

Response: `AuthenticateResultModel` com `accessToken`, `encryptedAccessToken`, `expireInSeconds` e `userId` do shadow user.

## Comportamento do filtro `MayHaveTenant`

O `TenantUserManager` manipula o filtro explicitamente:

- Para ler o usuário host e suas roles: desabilita `MayHaveTenant`.
- Para criar o shadow user e replicar roles: entra em `SetTenantId(tenantId)` e **reabilita** `MayHaveTenant`, porque `SetTenantId` apenas altera o parâmetro do filtro.

Veja mais detalhes em [eaf-tenant-user-manager.md](eaf-tenant-user-manager.md).

## Migrações

A entidade `UserTenantMembership` gera a tabela `AbpUserTenantMemberships`. A migration `AddUserTenantMembership` foi adicionada ao template `ProjectNameDbContext` e pode ser replicada nos `DbContext`s do projeto final.

## Testes

- `TokenAuthControllerMultiTenantBddTests` cobre os endpoints `GetAvailableTenants` e `SelectTenant` com credenciais corretas e a validação de usuário de tenant.
- Para testar `TenantUserManager` e `TenantRolePermissionReplicationService`, use um `DbContext` que herde `AbpZeroDbContext<Tenant, Role, User, ...>` com as entidades do `Eaf.Middleware.Core`.
