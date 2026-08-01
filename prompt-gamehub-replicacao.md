# Prompt de replicação: ajustar GameHub para Edition "Free", tenant registration e aprovação

Aplique no repositório `afonsoft/gamehub` os mesmos ajustes implementados no `afonsoft/EAF`, adaptando-os à estrutura do GameHub.

## Contexto

O GameHub usa `Eaf.Middleware` como base (ASP.NET Boilerplate / ABP multi-tenancy). As entidades `Tenant`, `User`, `Role` e `Edition` vêm do EAF. O GameHub possui:

- `Api/src/GameHub.Core/GameHubConsts.cs` (tenant padrão `Player`, multi-tenancy ativado)
- `Api/src/GameHub.Application/Authorization/RegistrationAppService.cs` (registro público)
- `Api/src/GameHub.Application/Authorization/IRegistrationAppService.cs` e `Api/src/GameHub.Application/Authorization/Dto/RegisterInput.cs`
- `Api/src/GameHub.EntityFrameworkCore/Migrations/Seed/Tenants/DefaultTenantBuilder.cs` e `PlayerTenantBuilder.cs` (criação dos tenants iniciais)
- `Api/src/GameHub.EntityFrameworkCore/Migrations/Seed/Tenants/TenantRoleAndUserBuilder.cs` (criação dos perfis Admin/User e usuário admin do tenant)
- `angular/src/app/public/register/register.component.ts` (tela de registro)
- `angular/src/app/core/auth/auth.service.ts` (chamada HTTP de registro)

## Objetivos

1. **Edition "Free" como padrão**: garantir que exista uma edição chamada `"Free"` e que todo novo tenant seja criado nessa edição.
2. **Tela de cadastro com criação/seleção de tenant**: na tela de registro, permitir:
   - Criar um novo tenant (usuário vira admin do tenant)
   - Selecionar um tenant existente para se cadastrar
3. **Aprovação de cadastro em tenants**: todos os usuários cadastrados em um tenant devem ser criados com `IsActive = false`, exceto administradores.
4. **Roles padrão por tenant**: cada tenant deve ter os roles `Admin` e `User`.
5. **Primeiro cadastro como administrador**: quando um novo tenant é criado, o primeiro usuário deve ser o administrador do tenant (ativo, com role `Admin`).

## Alterações sugeridas no backend (.NET / API)

### 1. Edition "Free"

- Verifique se `Eaf.Middleware.Core/Editions/EditionManager` já possui `DefaultEditionName = "Free"` e `GetOrCreateDefaultEditionAsync()` (deve vir com a nova versão do EAF). Se a versão local do EAF ainda estiver antiga, aplique a mesma alteração feita no EAF:
  - Em `EditionManager`, altere `DefaultEditionName` para `"Free"`.
  - Adicione o método `GetOrCreateDefaultEditionAsync()` que cria a edição caso não exista.

- Nos builders `DefaultTenantBuilder` e `PlayerTenantBuilder`, após criar o tenant, atribua `EditionId` usando `EditionManager.GetOrCreateDefaultEditionAsync()` ou, se estiverem usando `DbContext` diretamente, busque/crie a edição no contexto e vincule via `tenant.EditionId`.

- Se existir algum `TenantManager` customizado no GameHub, injete `EditionManager` no construtor e chame `GetOrCreateDefaultEditionAsync()` durante a criação do tenant.

### 2. Roles padrão Admin e User por tenant

- Em `TenantRoleAndUserBuilder.Create()` já existe criação do role `Admin`.
- Adicione a criação do role `User` (com `IsStatic = true`, `IsDefault = true`) caso ainda não exista no tenant:
  ```csharp
  var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
  if (userRole == null)
  {
      _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true });
      _context.SaveChanges();
  }
  ```

### 3. Aprovação de cadastro em tenants

- Em `RegistrationAppService.RegisterAsync`, quando o cadastro for dentro de um tenant (incluindo `Player` ou novo tenant), crie o usuário com `IsActive = false` e `IsEmailConfirmed = false`.
- Atribua o role `User` para cadastros em tenants existentes.
- Mantenha `IsActive = true` e role `Admin` apenas para o primeiro usuário/administrador de um novo tenant.
- Se o usuário selecionar "criar novo tenant" na UI, o backend deve:
  1. Criar o tenant (`Tenant`)
  2. Criar o usuário administrador dentro do tenant (ativo)
  3. Atribuir o role `Admin`

### 4. AccountAppService/RegistrationAppService

- Expanda `RegisterInput` (ou crie um novo DTO) para aceitar:
  - `TenancyName` (para criação de novo tenant)
  - `TenantName` (nome amigável do novo tenant)
  - `TenantId` (para cadastro em tenant existente)
- Adicione validação: se `TenancyName` for preenchido, crie o novo tenant; senão, use `TenantId`.
- Retorne informação indicando se o usuário pode logar imediatamente (`CanLogin`) ou se está aguardando aprovação.

### 5. Localization

- Adicione as chaves necessárias em `Localization/Source/GameHub.xml` e `GameHub-pt-BR.xml`:
  - `CreateNewTenant`
  - `SelectTenant`
  - `TenancyName`
  - `TenantName`
  - `SuccessfullyRegistered`
  - `RegistrationWaitingForApproval`
  - `TenantIsNotActive`
  - `InvalidRegisterRequest`

## Alterações sugeridas no frontend (Angular)

### `angular/src/app/core/auth/auth.service.ts`

- Atualize o `RegisterModel` e o endpoint de registro para enviar `tenancyName`, `tenantName` e `tenantId` quando aplicável.
- Ajuste o tratamento do retorno para distinguir cadastro aprovado vs. aguardando aprovação.

### `angular/src/app/public/register/register.component.ts` e `.html`

- Adicione um checkbox/flag "Criar novo canal/tenant".
- Quando marcado, exiba campos `TenancyName` e `TenantName`.
- Quando desmarcado, exiba um dropdown para selecionar o tenant existente (consumir endpoint que lista tenants ativos, se existir).
- Ao enviar, preencha `RegisterModel` conforme a opção.
- Exiba mensagem:
  - "Cadastro realizado com sucesso" se `CanLogin` for true.
  - "Seu cadastro está aguardando aprovação" se for false.

### Rota e navegação

- Adicione link "Cadastrar" na tela de login (`public/login`) apontando para `/register` se ainda não existir.

## Adaptações específicas do GameHub

- O GameHub já separa `Player` vs `Developer`. Mantenha a lógica de `isDeveloper` existente, mas:
  - Se o usuário escolher "criar novo tenant", trate-o como administrador desse tenant.
  - Se o usuário for `isDeveloper` e não selecionar um tenant, mantenha o fluxo atual de developer no tenant host.
  - Se o usuário for `Player` e selecionar um tenant, cadastre-o como `User` inativo (`IsActive = false`), aguardando aprovação do admin do tenant.

- Use `StaticRoleNames.Tenants.Admin` e `StaticRoleNames.Tenants.User` para os nomes dos roles padrão.

## Validações obrigatórias

- `dotnet build Api/GameHub.sln -c Release`
- `dotnet test Api/GameHub.sln -c Release --no-build`
- `npx ng build --configuration=production` no `angular` (frontend público)
- Se houver `angular-admin/GameHub.UI`, não é necessário alterá-lo para este prompt, a menos que a funcionalidade deva estar disponível também no admin.

## Observações

- Não edite arquivos gerados (`service-proxies.ts` e similares).
- Não commitar secrets.
- Criar branch `feature/gamehub-tenant-registration` e abrir PR para `develop` ou `main` conforme padrão do repositório.
