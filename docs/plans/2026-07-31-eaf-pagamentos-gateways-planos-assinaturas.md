# Plano de implementação — Pagamentos, Gateways, Planos/Edições e Dashboard

**Data:** 2026-07-31  
**Escopo:** Módulo `Eaf.Middleware.*` (backend) + Template Angular (`Templates/Angular/Eaf.ProjectName.UI`)  
**Objetivo:** Fechar os gaps de assinatura/pagamento do ASP.NET Zero: dashboard financeiro do host, planos com validade mensal/trimestral/anual/bianual/permanente, gateways (Stripe, PayPal, PagSeguro, MercadoPago), tela de configuração de gateways e tela de atribuição de plano/edição aos tenants.

---

## 1. Resultado dos testes iniciais

- `dotnet build Eaf.sln --configuration Release` — **OK** (0 warnings / 0 erros).
- `dotnet test Eaf.sln --configuration Release --no-build` — **OK** (0 falhas).
- `npx tsc -p src/tsconfig.app.json --noEmit && npx tsc -p src/tsconfig.spec.json --noEmit` — **OK**.
- `npx ng build --configuration=production` — **OK**.
- `npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox` — **OK** (254/254, após correção de 4 specs que faltavam `FormsModule`/`ModalModule`).
- `docker compose -f docker-compose.all.yml` — **bloqueado**: o template `Templates/Api` ainda não tem migrations para `Tenant.SubscriptionEndDateUtc`, `EafSubscriptionPayments`, `EafMassNotifications`, `EafUserDelegations` e campos da `SubscribableEdition`. A migration é parte deste plano.

---

## 2. Arquitetura e fluxo de dados

```
Host Admin
  ├─ DashboardAppService.GetHostDashboardAsync()   → métricas de tenants/pagamentos
  ├─ PaymentAppService.GetAllAsync() / CreatePaymentAsync() / ProcessPaymentAsync()
  ├─ PaymentGatewaySettingsAppService.Get/Update...  → settings dos gateways
  ├─ EditionAppService.Get/Create/Update/Delete + Features
  └─ TenantAppService.GetTenantSubscriptionAsync() / AssignEditionToTenantAsync()

Tenant Admin
  └─ TenantServiceProxy / SubscriptionServiceProxy → visualizar plano atual e renovar

Gateways (pluggable)
  IPaymentGateway
  ├─ NullPaymentGateway
  ├─ StripePaymentGateway
  ├─ PayPalPaymentGateway
  ├─ MercadoPagoPaymentGateway
  └─ PagSeguroPaymentGateway
```

---

## 3. Backend — entidades, DTOs e AppServices

### 3.1 Domínio / Core

#### `src/Eaf.Middleware.Core/Editions/PaymentPeriodType.cs`
```csharp
public enum PaymentPeriodType
{
    Daily = 1,
    Weekly = 7,
    Monthly = 30,
    Quarterly = 90,
    Biannual = 180,
    Annual = 365,
    Permanent = 99999
}
```

#### `src/Eaf.Middleware.Core/Editions/SubscribableEdition.cs`
Adicionar:
```csharp
public decimal? QuarterlyPrice { get; set; }
public decimal? BiannualPrice { get; set; }
public decimal? PermanentPrice { get; set; }
public int? DefaultPaymentPeriodType { get; set; }

// atualizar
public bool IsFree => !DailyPrice.HasValue && !WeeklyPrice.HasValue && !MonthlyPrice.HasValue
    && !QuarterlyPrice.HasValue && !BiannualPrice.HasValue && !AnnualPrice.HasValue && !PermanentPrice.HasValue;
```
Atualizar `GetPaymentAmountOrNull(PaymentPeriodType?)` para retornar o preço correspondente e tratar `Permanent`.

### 3.2 DTOs

#### `src/Eaf.Middleware.Application/Editions/Dto/CreateEditionInput.cs`
```csharp
public decimal? QuarterlyPrice { get; set; }
public decimal? BiannualPrice { get; set; }
public decimal? PermanentPrice { get; set; }
public int? DefaultPaymentPeriodType { get; set; }
```

#### `src/Eaf.Middleware.Application/Editions/Dto/EditionDto.cs`
Mesmas propriedades de preço + `DefaultPaymentPeriodType` + `IsFree`.

#### `src/Eaf.Middleware.Application/MultiTenancy/Dto/TenantListDto.cs`
```csharp
public int? EditionId { get; set; }
public string EditionDisplayName { get; set; }
public DateTime? SubscriptionEndDateUtc { get; set; }
```

#### `src/Eaf.Middleware.Application/MultiTenancy/Dto/TenantEditDto.cs`
```csharp
public int? EditionId { get; set; }
public DateTime? SubscriptionEndDateUtc { get; set; }
```

#### `src/Eaf.Middleware.Application/Payments/Dto/TenantSubscriptionDto.cs` (novo)
```csharp
public class TenantSubscriptionDto
{
    public int TenantId { get; set; }
    public string TenantName { get; set; }
    public int? EditionId { get; set; }
    public string EditionDisplayName { get; set; }
    public DateTime? SubscriptionEndDateUtc { get; set; }
    public bool IsExpired { get; set; }
    public int? RemainingDays { get; set; }
}
```

#### `src/Eaf.Middleware.Application/Payments/Dto/AssignEditionToTenantInput.cs` (novo)
```csharp
public class AssignEditionToTenantInput
{
    public int TenantId { get; set; }
    public int EditionId { get; set; }
    public PaymentPeriodType PaymentPeriodType { get; set; }
    public bool StartImmediately { get; set; }
    public decimal? OverrideAmount { get; set; }
}
```

#### `src/Eaf.Middleware.Application/Payments/Dto/PaymentGatewayDto.cs` (novo)
```csharp
public class PaymentGatewayDto
{
    public string Name { get; set; }       // Stripe, PayPal, MercadoPago, PagSeguro, Null
    public string DisplayName { get; set; }
    public bool IsConfigured { get; set; }
    public bool IsDefault { get; set; }
}
```

#### `src/Eaf.Middleware.Application/Payments/Dto/PaymentGatewaySettingsDto.cs` (novo)
```csharp
public class PaymentGatewaySettingsDto
{
    public string DefaultGateway { get; set; }
    public StripeSettingsDto Stripe { get; set; }
    public PayPalSettingsDto PayPal { get; set; }
    public MercadoPagoSettingsDto MercadoPago { get; set; }
    public PagSeguroSettingsDto PagSeguro { get; set; }
}
```

#### `src/Eaf.Middleware.Application/Dashboard/Dto/DashboardTileDto.cs`
```csharp
public string Value { get; set; }   // texto formatado opcional (ex: "R$ 1.234,56")
```

### 3.3 AppServices

#### `src/Eaf.Middleware.Application/Editions/EditionAppService.cs`
Trocar `IRepository<Edition>` por `IRepository<SubscribableEdition, int>`.
```csharp
public async Task<PagedResultDto<EditionDto>> GetEditions(GetEditionsInput input)
public async Task<EditionDto> GetEditionForEdit(EntityDto input)
public async Task CreateEdition(CreateEditionInput input)
public async Task UpdateEdition(UpdateEditionInput input)
public async Task DeleteEdition(EntityDto input)

// novos
public async Task<GetEditionFeaturesEditOutput> GetEditionFeaturesForEditAsync(EntityDto input)
public async Task UpdateEditionFeaturesAsync(UpdateEditionFeaturesInput input)
```

#### `src/Eaf.Middleware.Application/Editions/IEditionAppService.cs`
Adicionar os novos contratos.

#### `src/Eaf.Middleware.Application/MultiTenancy/TenantAppService.cs`
```csharp
public async Task<TenantSubscriptionDto> GetTenantSubscriptionAsync(EntityDto<int> input)
public async Task AssignEditionToTenantAsync(AssignEditionToTenantInput input)
public async Task ExtendTenantSubscriptionAsync(AssignEditionToTenantInput input)
```

#### `src/Eaf.Middleware.Application/Payments/PaymentAppService.cs`
Atualizar `CalculateEndDate` para suportar `Quarterly`, `Biannual` e `Permanent`.
```csharp
private static DateTime? CalculateEndDate(DateTime start, PaymentPeriodType period)
{
    return period switch
    {
        PaymentPeriodType.Daily => start.AddDays(1),
        PaymentPeriodType.Weekly => start.AddDays(7),
        PaymentPeriodType.Monthly => start.AddMonths(1),
        PaymentPeriodType.Quarterly => start.AddMonths(3),
        PaymentPeriodType.Biannual => start.AddMonths(6),
        PaymentPeriodType.Annual => start.AddYears(1),
        PaymentPeriodType.Permanent => null,
        _ => start
    };
}
```
Adicionar:
```csharp
public virtual async Task<List<PaymentGatewayDto>> GetGatewayListAsync()
public virtual async Task<PagedResultDto<SubscriptionPaymentDto>> GetPaymentHistoryForTenantAsync(GetSubscriptionPaymentsInput input)
public virtual async Task<PaymentGatewaySettingsDto> GetGatewaySettingsAsync()
public virtual async Task UpdateGatewaySettingsAsync(PaymentGatewaySettingsDto input)
public virtual async Task ProcessPaymentWebhookAsync(string gateway, string payload)
```

#### `src/Eaf.Middleware.Application/Dashboard/DashboardAppService.cs`
Injetar `IRepository<SubscriptionPayment, long>` e `EditionManager`.
Adicionar tiles:
- `totalPayments` / `pendingPayments` / `completedPayments`
- `monthlyRecurringRevenue`
- `tenantsWithActiveSubscription` / `tenantsWithExpiredSubscription`
- No tenant dashboard: `mySubscription` com `EditionDisplayName` e dias restantes.

#### `src/Eaf.Middleware.Application/Payments/PaymentGatewaySettingsAppService.cs` (novo — opcional)
Expõe `GetGatewaySettingsAsync` / `UpdateGatewaySettingsAsync` guardado em `AbpSettings` com prefixo `Eaf.Payment.*`.

### 3.4 Gateways concretos

#### `src/Eaf.Middleware.Application/Payments/Gateways/StripePaymentGateway.cs`
Usa `Stripe.net` (`Stripe.StripeClient`, `PaymentIntentService` ou `SessionService`).
```csharp
public Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
public Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
```

#### `src/Eaf.Middleware.Application/Payments/Gateways/PayPalPaymentGateway.cs`
Usa `PayPalServerSDK` (OrdersController) ou `HttpClient` contra `/v2/checkout/orders`.

#### `src/Eaf.Middleware.Application/Payments/Gateways/MercadoPagoPaymentGateway.cs`
Usa `mercadopago-sdk` (`MercadoPagoConfig`, `PaymentClient` / `PreferenceClient`).

#### `src/Eaf.Middleware.Application/Payments/Gateways/PagSeguroPaymentGateway.cs`
SDK oficial está depreciado → usar `HttpClient` com API v4 (token + email). Criar `HttpClient` nomeado ou `IHttpClientFactory`.

#### `src/Eaf.Middleware.Application/Payments/PaymentGatewayResolver.cs`
Manter a regra atual por prefixo do nome da classe. Para cada gateway adicionado, o resolver encontrará automaticamente via DI (`IEnumerable<IPaymentGateway>`).

### 3.5 Features por edição

#### `src/Eaf.Middleware.Web.Core/Features/MiddlewareFeatureProvider.cs`
Adicionar features com escopo `Edition`:
```csharp
var planFeatures = context.Create("App.PlanFeatures", defaultValue: "true", displayName: L("PlanFeatures"), scope: FeatureScopes.Edition);
planFeatures.CreateChildFeature("App.PlanFeatures.MaxUserCount", defaultValue: "0", displayName: L("MaximumUserCount"), scope: FeatureScopes.Edition);
planFeatures.CreateChildFeature("App.PlanFeatures.MaxOrganizationUnitCount", defaultValue: "0", displayName: L("MaximumOrganizationUnitCount"), scope: FeatureScopes.Edition);
planFeatures.CreateChildFeature("App.PlanFeatures.ApiCallLimit", defaultValue: "0", displayName: L("ApiCallLimit"), scope: FeatureScopes.Edition);
planFeatures.CreateChildFeature("App.PlanFeatures.StorageLimitGb", defaultValue: "0", displayName: L("StorageLimitGb"), scope: FeatureScopes.Edition);
```

#### `src/Eaf.Middleware.Application/Editions/EditionAppService.cs`
```csharp
public async Task<GetEditionFeaturesEditOutput> GetEditionFeaturesForEditAsync(EntityDto input)
{
    var features = FeatureManager.GetAll().Where(f => f.Scope.HasFlag(FeatureScopes.Edition));
    var featureValues = await EditionManager.GetFeatureValuesAsync(input.Id);
    return new GetEditionFeaturesEditOutput { Features = ObjectMapper.Map<List<FlatFeatureDto>>(features), FeatureValues = ObjectMapper.Map<List<NameValueDto>>(featureValues) };
}

public async Task UpdateEditionFeaturesAsync(UpdateEditionFeaturesInput input)
{
    await EditionManager.SetFeatureValuesAsync(input.Id, input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
}
```

### 3.6 Permissões

#### `src/Eaf.Middleware.Core/Authorization/MiddlewarePermissions.cs`
```csharp
public const string Pages_Administration_Payments_GatewaySettings = "Pages.Administration.Payments.GatewaySettings";
public const string Pages_Administration_Editions_Features = "Pages.Administration.Editions.Features";
public const string Pages_Tenants_Subscription = "Pages.Tenants.Subscription";
```

#### `src/Eaf.Middleware.Core/Authorization/MiddlewareAuthorizationProvider.cs`
Registrar as permissões filhas:
```csharp
payments.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Payments_GatewaySettings, L("PaymentGatewaySettings"));
editions.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Editions_Features, L("EditionFeatures"));
tenants.CreateChildPermission(MiddlewarePermissions.Pages_Tenants_Subscription, L("TenantSubscription"), multiTenancySides: MultiTenancySides.Host);
```

### 3.7 AutoMapper

#### `src/Eaf.Middleware.Application/MiddlewareCustomDtoMapper.cs`
```csharp
configuration.CreateMap<SubscribableEdition, EditionDto>();
configuration.CreateMap<CreateEditionInput, SubscribableEdition>();
configuration.CreateMap<UpdateEditionInput, SubscribableEdition>();
```

### 3.8 Localization

Adicionar em `src/Eaf.Middleware.Core/Localization/Source/EafCore.xml` e `EafCore-pt-BR.xml`:
```xml
<text name="Quarterly">Trimestral</text>
<text name="Biannual">Semestral</text>
<text name="Permanent">Permanente</text>
<text name="QuarterlyPrice">Preço trimestral</text>
<text name="BiannualPrice">Preço semestral</text>
<text name="PermanentPrice">Preço permanente</text>
<text name="DefaultPaymentPeriodType">Período de pagamento padrão</text>
<text name="PaymentGatewaySettings">Configuração de gateways de pagamento</text>
<text name="TenantSubscription">Assinatura do tenant</text>
<text name="AssignEdition">Atribuir edição</text>
<text name="ChangePlan">Alterar plano</text>
<text name="SubscriptionEndDateUtc">Data de término da assinatura</text>
<text name="RemainingDays">Dias restantes</text>
<text name="MonthlyRecurringRevenue">Receita mensal recorrente</text>
<text name="TenantsWithActiveSubscription">Tenants com assinatura ativa</text>
<text name="TenantsWithExpiredSubscription">Tenants com assinatura expirada</text>
<text name="TotalPayments">Total de pagamentos</text>
<text name="PendingPayments">Pagamentos pendentes</text>
<text name="CompletedPayments">Pagamentos concluídos</text>
```

### 3.9 Migrations no template API

#### `Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContext.cs`
Adicionar `DbSet`s:
```csharp
public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
public virtual DbSet<MassNotification> MassNotifications { get; set; }
public virtual DbSet<UserDelegation> UserDelegations { get; set; }
public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }
```

#### Geração da migration
```bash
cd Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore
dotnet ef migrations add AddSubscriptionPaymentsAndPlanFields --startup-project ../Eaf.ProjectName.Web.Host/Eaf.ProjectName.Web.Host.csproj
```
A migration deve conter:
- colunas `SubscriptionEndDateUtc` em `AbpTenants`;
- campos de preço + `DefaultPaymentPeriodType` em `AbpEditions` (TPH);
- tabelas `EafSubscriptionPayments`, `EafMassNotifications`, `EafUserDelegations`.

---

## 4. Frontend — componentes, rotas e proxies

### 4.1 Service proxies manuais

Não editar `service-proxies.ts` gerado. Criar/atualizar arquivos manuais:

#### `src/shared/service-proxies/edition.service-proxy.ts`
Atualizar interfaces `ICreateEditionInput`, `IUpdateEditionInput`, `IEditionDto` com preços trimestral/semestral/permanente e `defaultPaymentPeriodType`.
Adicionar:
```ts
getEditionFeaturesForEdit(id: number): Observable<IFeatureTreeEditOutput>
updateEditionFeatures(input: IUpdateEditionFeaturesInput): Observable<void>
```

#### `src/shared/service-proxies/payment.service-proxy.ts`
Adicionar:
```ts
getGatewayList(): Observable<IPaymentGatewayDto[]>
getGatewaySettings(): Observable<IPaymentGatewaySettingsDto>
updateGatewaySettings(input: IPaymentGatewaySettingsDto): Observable<void>
getPaymentHistoryForTenant(...): Observable<IPagedResultDtoOfSubscriptionPaymentDto>
```

#### `src/shared/service-proxies/dashboard.service-proxy.ts`
Atualizar `IDashboardTileDto`:
```ts
export interface IDashboardTileDto {
    id: string;
    title: string;
    count: number;
    value: string | undefined;
    style: string;
    icon: string;
}
```

#### `src/shared/service-proxies/tenant-subscription.service-proxy.ts` (novo)
```ts
export interface ITenantSubscriptionDto { ... }
export interface IAssignEditionToTenantInput { ... }
getTenantSubscription(tenantId: number): Observable<ITenantSubscriptionDto>
assignEditionToTenant(input: IAssignEditionToTenantInput): Observable<void>
```

#### `src/shared/service-proxies/service-proxy.module.ts`
Adicionar `TenantSubscriptionServiceProxy` (se criado) e garantir que `PaymentServiceProxy` / `EditionServiceProxy` / `DashboardServiceProxy` estão registrados.

### 4.2 Componentes admin

#### `src/app/admin/payments/payments.component.{ts,html}`
- Tabela com filtros por gateway, status e período.
- Botão "Processar" abre modal com dropdown de gateway configurado (não input livre).
- Badge colorido por status já existente; manter.
- Adicionar coluna `Amount` formatada.

#### `src/app/admin/payments/payment-gateway-settings.component.{ts,html}` (novo)
- Tabs por gateway: Stripe, PayPal, MercadoPago, PagSeguro.
- Campos de chave/segredo/token/ativo/padrão.
- Salvar via `PaymentAppService.UpdateGatewaySettingsAsync`.
- Rota `/app/admin/payment-gateway-settings`.

#### `src/app/admin/editions/editions.component.{ts,html}`
- Adicionar colunas trimestral/semestral/permanente.
- Ações: Create, Edit, Delete, Features.
- Botão `+ Criar` e botão de editar abrem modal `create-or-edit-edition-modal`.

#### `src/app/admin/editions/create-or-edit-edition-modal.component.{ts,html}` (novo)
- Form com nome, preços diário/semanal/mensal/trimestral/semestral/anual/permanente, trial, carência, edição de expiração, período padrão.
- Checkbox `IsFree` desabilita campos de preço.

#### `src/app/admin/editions/edition-features-modal.component.{ts,html}` (novo)
- Reutilizar `<feature-tree>` com `FeatureTreeEditModel` carregado de `EditionAppService.GetEditionFeaturesForEditAsync`.
- Salvar via `UpdateEditionFeaturesAsync`.

#### `src/app/admin/tenants/tenants.component.{ts,html}`
- Adicionar colunas `EditionDisplayName` e `SubscriptionEndDateUtc`.
- Adicionar ação no dropdown "Atribuir edição / Alterar plano" que abre `tenant-subscription-modal`.
- Ação usa `TenantSubscriptionServiceProxy.assignEditionToTenant`.

#### `src/app/admin/tenants/tenant-subscription-modal.component.{ts,html}` (novo)
- Dropdown de edições + período (mensal/trimestral/anual/bianual/permanente).
- Preview do valor baseado no `GetPaymentAmount`.
- Checkbox "Iniciar imediatamente" / campos de início/fim.

### 4.3 Componente tenant

#### `src/app/main/subscription/subscription.component.{ts,html}` (novo)
- Exibe plano atual, dias restantes, botão "Renovar/Upgrade".
- Lista histórico de pagamentos do tenant.
- Rota `/app/main/subscription`.
- Adicionar no `main-routing.module.ts` e `main.module.ts`.

### 4.4 Dashboard

#### `src/app/main/dashboard/dashboard.component.html`
- Usar `tile.value` quando preenchido, senão `tile.count`.
- Títulos formatados para métricas financeiras.

#### `src/app/main/dashboard/dashboard.component.ts`
- Sem mudanças estruturais; continua chamando `DashboardServiceProxy`.

### 4.5 Reorganização de menus

#### `src/app/shared/layout/nav/app-navigation.service.ts`
```ts
getMenu(): AppMenu {
  return new AppMenu('MainMenu', 'MainMenu', [
    new AppMenuItem('Dashboard', 'Pages.Dashboard', 'flaticon-line-graph', '/app/main/dashboard'),
    new AppMenuItem('Tenants', 'Pages.Tenants', 'flaticon-squares-4', '/app/admin/tenants'), // host
    new AppMenuItem('MySubscription', 'Pages.Dashboard', 'flaticon-coins', '/app/main/subscription', undefined, undefined, undefined, undefined, () => this._appSessionService.tenantId != null),
  ]);
}

getAdminMenu(): AppMenu {
  return new AppMenu('AdminMenu', 'AdminMenu', [
    // Usuários/Perfis
    new AppMenuItem('Users', 'Pages.Administration.Users', ...),
    new AppMenuItem('Roles', 'Pages.Administration.Roles', ...),
    new AppMenuItem('OrganizationUnits', 'Pages.Administration.OrganizationUnits', ...),
    new AppMenuItem('UserDelegation', 'Pages.Administration.Users.Delegation', ...),
    // Comunicação
    new AppMenuItem('MassNotifications', 'Pages.Administration.MassNotifications', ...),
    // Assinaturas/Planos
    new AppMenuItem('Editions', 'Pages.Administration.Editions', ...),
    new AppMenuItem('Payments', 'Pages.Administration.Payments', ...),
    new AppMenuItem('PaymentGatewaySettings', 'Pages.Administration.Payments.GatewaySettings', ...),
    // Sistema
    new AppMenuItem('Languages', 'Pages.Administration.Languages', ...),
    new AppMenuItem('AuditLogs', 'Pages.Administration.AuditLogs', ...),
    new AppMenuItem('VisualSettings', 'Pages.Administration.UiCustomization', ...),
    new AppMenuItem('Maintenance', 'Pages.Administration.Maintenance', ...),
    new AppMenuItem('Settings', 'Pages.Administration.Settings', ...),
  ]);
}
```

Nota: a `getAdminMenu()` é renderizada pelo `adm-bar` (dropdown direito do header); a `getMenu()` é a sidebar/esquerda. Assim, configurações administrativas ficam no dropdown direito e dashboards/métricas/funcionalidades do tenant na esquerda.

### 4.6 Responsivo

- Tabelas admin (`payments`, `editions`, `tenants`): manter `p-table [scrollable]` com `ScrollWidth="100%"` + scroll horizontal em mobile.
- Modais: adicionar `modal-dialog-scrollable` e `modal-lg` para não quebrar em telas pequenas.
- Botões de ação em tabelas: `btn-group` com `dropdown` e `btn-sm`; evitar empilhamento forçando `min-width: 44px`.
- CSS em `styles.css`/`customize.css`: adicionar `@media (max-width: 768px)` para títulos de portlet e inputs empilhados.

---

## 5. Testes a adicionar

### 5.1 Backend (xUnit / Shouldly / NSubstitute)

#### `test/Eaf.Middleware.Application.Tests/Payments/PaymentAppServiceBddTests.cs`
- `DadoUmPlanoTrimestral_QuandoCriarPagamento_EntaoValorDeveSerTrimestral`
- `DadoUmPlanoPermanente_QuandoProcessar_EntaoDataFimDeveSerNula`
- `DadoGatewayDefault_QuandoResolver_EntaoRetornaGatewayCorreto`

#### `test/Eaf.Middleware.Application.Tests/Editions/EditionAppServiceBddTests.cs`
- `DadoUmaEdicaoComPrecoPermanente_QuandoCriar_EntaoDevePersistir`
- `DadoUmaEdicaoPaga_QuandoGetPaymentAmount_EntaoRetornaPrecoDoPeriodo`
- `DadoUmaEdicao_QuandoAtualizarFeatures_EntaoFeaturesSaoPersistidas`

#### `test/Eaf.Middleware.Application.Tests/MultiTenancy/TenantSubscriptionBddTests.cs`
- `DadoUmTenant_QuandoAtribuirEdicao_EntaoSubscriptionEndDateDeveSerAtualizado`
- `DadoUmaAssinaturaExpirada_QuandoVerificar_EntaoIsExpiredVerdadeiro`

#### `test/Eaf.Middleware.Application.Tests/Dashboard/DashboardAppServiceBddTests.cs`
- `DadoHost_QuandoGetHostDashboardAsync_EntaoRetornaMetricasDePagamento`

### 5.2 Frontend

- `payments.component.spec.ts` (já existe): adicionar teste de gateway dropdown.
- `payment-gateway-settings.component.spec.ts` (novo): testa salvar configuração.
- `editions.component.spec.ts` (já existe): teste de modal de create/update.
- `edition-features-modal.component.spec.ts` (novo).
- `tenant-subscription-modal.component.spec.ts` (novo).
- `subscription.component.spec.ts` (novo).
- `app-navigation.service.spec.ts`: ajustar expectativas de itens do menu.

### 5.3 Docker / integração

Após gerar a migration do template `Templates/Api`:
```bash
export MSSQL_SA_PASSWORD='YourPassword123!'
docker compose -f docker-compose.all.yml down -v
docker compose -f docker-compose.all.yml up -d --build
```
Verificar:
- `eaf-migrator` completa sem `Invalid column name`.
- `eaf-api` responde `http://localhost:5000/health`.
- `eaf-angular` responde `http://localhost:4200`.

---

## 6. Sequência de implementação

1. **Backend — planos e validade**
   - `PaymentPeriodType`, `SubscribableEdition` preços, DTOs, `EditionAppService`, AutoMapper, permissões, localização.
2. **Backend — dashboard e estatísticas**
   - `DashboardTileDto.Value`, `DashboardAppService` com métricas financeiras.
3. **Backend — features por edição**
   - `MiddlewareFeatureProvider`, métodos `Get/UpdateEditionFeatures`.
4. **Backend — assinatura do tenant**
   - `TenantListDto`, `TenantEditDto`, `TenantAppService` métodos.
5. **Backend — gateway de pagamento**
   - `PaymentGatewaySettingsDto`, `PaymentAppService` configurações, `StripePaymentGateway`, `PayPalPaymentGateway`, `MercadoPagoPaymentGateway`, `PagSeguroPaymentGateway`.
6. **Frontend — service proxies**
   - Atualizar `edition.service-proxy.ts`, `payment.service-proxy.ts`, `dashboard.service-proxy.ts`; criar `tenant-subscription.service-proxy.ts`.
7. **Frontend — telas admin**
   - `payment-gateway-settings.component`, `create-or-edit-edition-modal`, `edition-features-modal`, `tenant-subscription-modal`, tabelas.
8. **Frontend — tela tenant e menus**
   - `subscription.component`, ajuste de `app-navigation.service.ts`.
9. **Migração do template API**
   - Atualizar `ProjectNameDbContext` e gerar migration.
10. **Testes**
    - BDD backend, specs Angular, build, Docker.

---

## 7. Decisões e riscos

- **Gateways brasileiros:** PagSeguro não tem SDK .NET atualizado. A implementação usará `HttpClient` contra a API v4. MercadoPago usa o SDK oficial `mercadopago-sdk`.
- **Chaves de gateway:** serão armazenadas como `AbpSettings` no escopo da aplicação (host). O ABP não criptografa settings por padrão; se necessário, usar `SettingManager` + `ISettingEncryptionService` (não implementado no EAF). Neste plano, assumimos `IsVisibleToClients = false` para chaves secretas.
- **service-proxies.ts gerado:** não será editado. Novos endpoints serão consumidos por proxies manuais ou por extensões dos proxies existentes (evita conflito com NSwag).
- **Migrations:** `ProjectNameDbContext` do template API precisa refletir as novas entidades. A migration será gerada a partir do template `Web.Host` (não do middleware core).
- **Responsivo:** não altera estrutura de dados; aplica CSS utilitário e testa breakpoints mobile.

---

## 8. Critérios de aceite

- Host dashboard exibe tiles de pagamentos (`Total`, `Pendentes`, `Concluídos`, `MRR`, `Ativos`, `Expirados`).
- Tela `Edições` permite criar/editar planos com preços diário, semanal, mensal, trimestral, semestral, anual e permanente.
- Tela `Edições` permite configurar features por pacote usando a árvore de features.
- Tela `Tenants` permite atribuir/alterar plano e período; o tenant recebe `SubscriptionEndDateUtc` correto.
- Tela `Configuração de Gateways` permite escolher gateway padrão e inserir chaves/token.
- Tela `Pagamentos` lista pagamentos e permite processar manualmente via gateway configurado.
- Usuário tenant visualiza seu plano e pode solicitar renovar/upgrade.
- Docker full-stack (`docker-compose.all.yml`) sobe sem erros de coluna ausente.
- Cobertura de testes não diminui (≥ 90% nas áreas alteradas).
