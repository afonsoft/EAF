# Plano de implementação: ASP.NET Zero features 1, 2, 5, 6, 7 no EAF

> Itens escolhidos da especificação `eaf-aspnetzero-functional-gap.spec.md`:
> 1. Subscription & Payment System  
> 2. Organization Units  
> 5. User Delegation  
> 6. Mass Notifications  
> 7. Host/Tenant Dashboard  

## Ordem de execução (do mais simples ao mais complexo)

1. **Host/Tenant Dashboard** — apenas agregação de estatísticas; não altera schema de autenticação.
2. **Organization Units** — entidades do ABP Zero já existem (`OrganizationUnit`, `UserOrganizationUnit`, `OrganizationUnitRole`); exige apenas AppService + UI.
3. **Mass Notifications** — estende a infraestrutura existente (`INotificationPublisher`) com uma nova entidade/background job.
4. **User Delegation** — estende o impersonation existente com validade temporal.
5. **Subscription & Payment System** — maior escopo: novas entidades (`SubscriptionPayment`, `SubscriptionPaymentProduct`), gateways PayPal/Stripe, workers de expiração e UI de assinatura.

## Backend

### 1. Dashboard (`Eaf.Middleware.Application.Dashboard`)

- `IDashboardAppService`
  - `Task<DashboardOutput> GetHostDashboardAsync()`
  - `Task<DashboardOutput> GetTenantDashboardAsync()`
- `DashboardAppService : MiddlewareAppServiceBase, IDashboardAppService`
  - Contadores: tenants, users, editions, subscriptions (quando houver).
- DTOs: `DashboardOutput`, `DashboardTileDto`.
- Permissões: reutilizar `MiddlewarePermissions.Pages_Dashboard`.

### 2. Organization Units (`Eaf.Middleware.Application.OrganizationUnits`)

- `IOrganizationUnitAppService`
  - `Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnits()`
  - `Task<OrganizationUnitDto> CreateAsync(CreateOrganizationUnitInput input)`
  - `Task<OrganizationUnitDto> UpdateAsync(UpdateOrganizationUnitInput input)`
  - `Task MoveAsync(MoveOrganizationUnitInput input)`
  - `Task DeleteAsync(EntityDto<long> input)`
  - `Task<PagedResultDto<OrganizationUnitUserListDto>> GetOrganizationUnitUsersAsync(GetOrganizationUnitUsersInput input)`
  - `Task AddUserToOrganizationUnit(UserToOrganizationUnitInput input)`
  - `Task RemoveUserFromOrganizationUnit(UserToOrganizationUnitInput input)`
  - `Task<PagedResultDto<OrganizationUnitRoleListDto>> GetOrganizationUnitRolesAsync(...)`
  - `Task AddRoleToOrganizationUnit(RoleToOrganizationUnitInput input)`
  - `Task RemoveRoleFromOrganizationUnit(RoleToOrganizationUnitInput input)`
- `OrganizationUnitAppService : MiddlewareAppServiceBase, IOrganizationUnitAppService`
- Usa `IRepository<OrganizationUnit, long>`, `IRepository<UserOrganizationUnit, long>`, `IRepository<OrganizationUnitRole, long>`, `UserManager`, `RoleManager`.
- Permissões: `Pages.Administration.OrganizationUnits` (+ Create, Edit, Delete, ManageMembers, ManageRoles).

### 3. Mass Notifications (`Eaf.Middleware.Application.MassNotifications`)

- Entidade `MassNotification` (AggregateRoot<long>)
- DTOs: `CreateMassNotificationInput`, `MassNotificationDto`, `GetMassNotificationsInput`.
- `IMassNotificationAppService`
  - `Task<PagedResultDto<MassNotificationDto>> GetAllAsync(...)`
  - `Task CreateAsync(CreateMassNotificationInput input)`
  - `Task DeleteAsync(EntityDto<long> input)`
- `MassNotificationManager` (domain service) filtra destinatários por user/role/OU/tenant e publica `INotificationPublisher`.
- Background job `MassNotificationJob` dispara notificações assíncronas.
- Permissões: `Pages.Administration.MassNotifications`.

### 4. User Delegation (`Eaf.Middleware.Application.Authorization.Users`)

- Entidade `UserDelegation` (AggregateRoot<long>)
  - `SourceUserId`, `TargetUserId`, `TenantId`, `StartTime`, `EndTime`, `IsDeleted`.
- `IUserDelegationAppService`
  - `Task<ListResultDto<UserDelegationDto>> GetMyDelegations()`
  - `Task<ListResultDto<UserDelegationDto>> GetDelegatedUsers()`
  - `Task CreateAsync(CreateUserDelegationInput input)`
  - `Task CancelAsync(EntityDto<long> input)`
- `IUserDelegationManager` valida período e resolve usuário delegado no login.
- Alterar `ImpersonationManager`/`TokenAuthController` para respeitar `EndTime` e gravar `ImpersonatorUserId` nos audit logs.
- Permissões: `Pages.Administration.Users.Delegation`.

### 5. Subscription & Payment (`Eaf.Middleware.Application.Payments`)

- Extender `SubscribableEdition` já existente.
- Novas entidades:
  - `SubscriptionPayment` (AggregateRoot<long>)
    - `TenantId`, `EditionId`, `Gateway`, `Status`, `Amount`, `PaymentPeriodType`, `Description`, `SuccessUrl`, `ErrorUrl`, `ExternalPaymentId`, `InvoiceNo`.
  - `SubscriptionPaymentProduct` (value object ou entidade filha)
    - `Description`, `Amount`, `Count`, `TotalPrice`.
- Gateway abstrato: `IPaymentGateway` com `CreatePaymentAsync`, `ExecutePaymentAsync`, `CancelPaymentAsync`, `GetPaymentStatusAsync`.
- Provedores: `PayPalPaymentGateway`, `StripePaymentGateway` (placeholder, implementação mínima).
- `IPaymentAppService`
  - `Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentInput input)`
  - `Task ExecutePaymentAsync(ExecutePaymentInput input)`
  - `Task<PagedResultDto<SubscriptionPaymentDto>> GetPaymentHistoryAsync(...)`
- `ISubscriptionAppService`
  - `Task<SubscribeOutput> SubscribeAsync(SubscribeInput input)`
  - `Task ExtendTrialAsync(...)`
- Workers:
  - `SubscriptionExpirationCheckWorker` — verifica assinaturas expiradas diariamente.
  - `SubscriptionExpireEmailNotifierWorker` — notifica tenants próximos ao vencimento.
- Permissões: `Pages.Administration.TenantSubscriptions` (host), `Pages.Payment.Buy` (tenant).

## Frontend (Angular)

Cada feature terá:

- Rota em `admin-routing.module.ts` (ou `main-routing.module.ts` para dashboard/assinatura).
- Componente standalone desativado (`standalone: false`) seguindo padrão existente.
- Service proxy manual em `@shared/service-proxies/` (o EAF não regenera `service-proxies.ts` automaticamente a cada mudança; o build do Angular é feito com os arquivos manuais).
- Templates HTML com PrimeNG `p-table`, `p-tree` (OUs), `p-dialog`.
- Testes `.spec.ts` usando os mocks existentes (`mock-services.ts`).

### Componentes a criar

- `src/app/main/dashboard/dashboard.component.*`
- `src/app/admin/organization-units/organization-units.component.*`
- `src/app/admin/mass-notifications/mass-notifications.component.*`
- `src/app/admin/user-delegations/user-delegations.component.*`
- `src/app/admin/subscriptions/subscriptions.component.*`
- `src/app/account/delegated-login/...` (se necessário para fluxo de delegação).

## Migrations e DbContext

- Features 2 (OUs) não precisam de migrations novas (tabelas do ABP Zero já existem).
- Features 1, 3, 4, 5 precisam de novas entidades e, consequentemente, migrations nos templates `Api` e `Worker`.
- Serão adicionados `DbSet<T>` em `ProjectNameDbContext` dos templates e geradas migrations via `dotnet ef migrations add`.

## Testes

- xUnit BDD para cada `AppService` (`*BddTests.cs`).
- Testes Angular `.spec.ts` para componentes com serviços mockados.
- Build: `dotnet build Eaf.sln`, `npx ng build --configuration=production`.
- SonarCloud: monitorar e corrigir novas issues.

## Critérios de aceite

- Todas as APIs publicam XML docs.
- 90% de cobertura nos arquivos novos.
- Nenhum push direto em `main`/`develop`.
- PR separado por feature ou um PR unificado com commits bem separados.
