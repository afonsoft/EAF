# Plano de Implementação — Telas de Pagamentos

> **Para agentes:** use `superpowers:executing-plans` ou implemente passo a passo.

**Objetivo:** Implementar as telas Angular `account/gateway-selection` e `admin/subscriptions` usando os endpoints de `PaymentServiceProxy` já existentes.

**Abordagem:** Criar componentes e rotas no template Angular, reutilizar `PaymentServiceProxy` e DTOs gerados, adicionar permissão `Pages.Administration.Subscriptions` e listar/manage subscription payments.

**Stack:** Angular 20, PrimeNG 17, TypeScript 5.8, `service-proxies.ts`.

---

## Estrutura de arquivos

- `Templates/Angular/Eaf.ProjectName.UI/src/account/gateway-selection/gateway-selection.component.ts|html|less`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/subscriptions/subscriptions.component.ts|html|less`
- `Templates/Angular/Eaf.ProjectName.UI/src/account/account-routing.module.ts` (rota)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/admin-routing.module.ts` (rota)
- `Templates/Angular/Eaf.ProjectName.UI/src/account/account.module.ts` (declarar)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/admin.module.ts` (declarar)
- `src/Eaf.Middleware.Core/Authorization/MiddlewarePermissions.cs` (adicionar permissão)
- `src/Eaf.Middleware.Core/Authorization/MiddlewareAuthorizationProvider.cs` (registrar permissão)
- `Templates/Angular/Eaf.ProjectName.UI/src/shared/AppEnums.ts` (adicionar enum se necessário)

---

## Tarefas

### Tarefa 1: Adicionar permissão `Pages.Administration.Subscriptions`

**Arquivos:**
- Modificar: `src/Eaf.Middleware.Core/Authorization/MiddlewarePermissions.cs`
- Modificar: `src/Eaf.Middleware.Core/Authorization/MiddlewareAuthorizationProvider.cs`

**Passos:**

- [ ] **1.1 Constante**

```csharp
public const string Pages_Administration_Subscriptions = "Pages.Administration.Subscriptions";
```

- [ ] **1.2 Registro no provider**

```csharp
administration.CreateChildPermission(MiddlewarePermissions.Pages_Administration_Subscriptions, L("Subscriptions"));
```

- [ ] **1.3 Build**

```bash
dotnet build src/Eaf.Middleware.Core/Eaf.Middleware.Core.csproj --configuration Release
```

### Tarefa 2: Criar `account/gateway-selection`

**Arquivos:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/account/gateway-selection/gateway-selection.component.ts`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/account/gateway-selection/gateway-selection.component.html`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/account/gateway-selection/gateway-selection.component.less`

**Passos:**

- [ ] **2.1 Componente**

```typescript
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PaymentServiceProxy, CreateSubscriptionPaymentInput, PaymentGatewayDto } from '@shared/service-proxies/service-proxies';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-gateway-selection',
  standalone: false,
  templateUrl: './gateway-selection.component.html'
})
export class GatewaySelectionComponent extends AppComponentBase implements OnInit {
  paymentId: number | undefined;
  editionId: number | undefined;
  paymentPeriodType: number | undefined;
  gateways: PaymentGatewayDto[] = [];
  selectedGateway: string | undefined;
  loading = false;

  constructor(
    injector: Injector,
    private paymentService: PaymentServiceProxy,
    private route: ActivatedRoute,
    private router: Router
  ) { super(injector); }

  ngOnInit(): void {
    this.paymentId = this.route.snapshot.queryParams.paymentId;
    this.editionId = this.route.snapshot.queryParams.editionId;
    this.paymentPeriodType = this.route.snapshot.queryParams.paymentPeriodType;
    this.loadGateways();
  }

  loadGateways(): void {
    this.paymentService.getGatewayList().subscribe(result => {
      this.gateways = result;
    });
  }

  pay(): void {
    if (!this.selectedGateway || !this.editionId || !this.paymentPeriodType) return;

    const input = new CreateSubscriptionPaymentInput();
    input.editionId = this.editionId;
    input.paymentPeriodType = this.paymentPeriodType;
    input.gateway = this.selectedGateway;
    input.successUrl = window.location.origin + '/account/payment-success';
    input.errorUrl = window.location.origin + '/account/payment-error';

    this.loading = true;
    this.paymentService.createPayment(input).subscribe(result => {
      this.loading = false;
      if (result.gatewayUrl) {
        window.location.href = result.gatewayUrl;
      } else if (result.paymentId) {
        this.router.navigate(['/account/payment-success'], { queryParams: { paymentId: result.paymentId } });
      }
    }, () => this.loading = false);
  }
}
```

- [ ] **2.2 Template**

```html
<div class="gateway-selection">
  <h2>{{ 'SelectPaymentGateway' | localize }}</h2>

  <div class="gateway-list">
    <div
      class="gateway-card"
      *ngFor="let gateway of gateways"
      [class.selected]="selectedGateway === gateway.gatewayType"
      (click)="selectedGateway = gateway.gatewayType"
    >
      <i class="pi pi-credit-card"></i>
      <span>{{ gateway.gatewayType }}</span>
    </div>
  </div>

  <button pButton type="button" [disabled]="!selectedGateway || loading" (click)="pay()" label="{{ 'Pay' | localize }}"></button>
</div>
```

- [ ] **2.3 Rota**

Em `src/account/account-routing.module.ts`:

```typescript
import { GatewaySelectionComponent } from './gateway-selection/gateway-selection.component';

{ path: 'gateway-selection', component: GatewaySelectionComponent, canActivate: [AccountRouteGuard] }
```

- [ ] **2.4 Declarar no módulo**

Em `src/account/account.module.ts`:

```typescript
import { GatewaySelectionComponent } from './gateway-selection/gateway-selection.component';

declarations: [..., GatewaySelectionComponent]
```

### Tarefa 3: Criar `admin/subscriptions`

**Arquivos:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/subscriptions/subscriptions.component.ts`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/subscriptions/subscriptions.component.html`
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/admin/subscriptions/subscriptions.component.less`

**Passos:**

- [ ] **3.1 Componente**

```typescript
import { Component, Injector, OnInit, ViewChild } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { PaymentServiceProxy, SubscriptionPaymentDto } from '@shared/service-proxies/service-proxies';
import { Table } from 'primeng/table';

@Component({
  selector: 'app-subscriptions',
  standalone: false,
  templateUrl: './subscriptions.component.html'
})
export class SubscriptionsComponent extends AppComponentBase implements OnInit {
  @ViewChild('dt') dataTable: Table;
  payments: SubscriptionPaymentDto[] = [];
  totalCount = 0;
  filter = '';
  loading = false;

  constructor(
    injector: Injector,
    private paymentService: PaymentServiceProxy
  ) { super(injector); }

  ngOnInit(): void {
    this.load();
  }

  load(event?: any): void {
    this.loading = true;
    const skipCount = event ? event.first : 0;
    const maxResultCount = event ? event.rows : 10;
    this.paymentService.getAll(this.filter, event?.sortField, skipCount, maxResultCount).subscribe(result => {
      this.payments = result.items;
      this.totalCount = result.totalCount;
      this.loading = false;
    });
  }
}
```

- [ ] **3.2 Template**

```html
<div class="subscriptions-page">
  <h2>{{ 'Subscriptions' | localize }}</h2>

  <p-table #dt [value]="payments" [lazy]="true" [paginator]="true" [rows]="10" [totalRecords]="totalCount" (onLazyLoad)="load($event)">
    <ng-template pTemplate="header">
      <tr>
        <th>{{ 'Id' | localize }}</th>
        <th>{{ 'Description' | localize }}</th>
        <th>{{ 'Amount' | localize }}</th>
        <th>{{ 'Status' | localize }}</th>
        <th>{{ 'CreationTime' | localize }}</th>
      </tr>
    </ng-template>
    <ng-template pTemplate="body" let-payment>
      <tr>
        <td>{{ payment.id }}</td>
        <td>{{ payment.description }}</td>
        <td>{{ payment.amount | currency }}</td>
        <td>{{ payment.status }}</td>
        <td>{{ payment.creationTime | date:'short' }}</td>
      </tr>
    </ng-template>
  </p-table>
</div>
```

- [ ] **3.3 Rota**

Em `src/app/admin/admin-routing.module.ts`:

```typescript
import { SubscriptionsComponent } from './subscriptions/subscriptions.component';

{ path: 'subscriptions', component: SubscriptionsComponent, data: { permission: 'Pages.Administration.Subscriptions' } }
```

- [ ] **3.4 Declarar no módulo**

Em `src/app/admin/admin.module.ts`:

```typescript
import { SubscriptionsComponent } from './subscriptions/subscriptions.component';

declarations: [..., SubscriptionsComponent]
```

### Tarefa 4: Adicionar menu

**Arquivo:**
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout/nav/app-navigation.service.ts` ou equivalente

**Passos:**

- [ ] **4.1 Item de menu**

```typescript
new AppMenuItem('Subscriptions', 'Pages.Administration.Subscriptions', 'flaticon-list', '/app/admin/subscriptions')
```

### Tarefa 5: Verificação

- [ ] **5.1 Build**

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npx ng build --configuration=production
```

- [ ] **5.2 Testes unitários**

```bash
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

- [ ] **5.3 Build backend**

```bash
dotnet build Eaf.sln --configuration Release
```

### Tarefa 6: Commit e PR

Branch: `feature/eaf-payment-screens`.

```bash
git checkout -b feature/eaf-payment-screens
...
git commit -m "feat(angular): add gateway selection and admin subscriptions screens"
```

---

## Cobertura da spec

| Spec item | Tarefa |
|---|---|
| Angular `account/gateway-selection` | 2 |
| Angular `admin/subscriptions` | 3, 4 |
| Permissão de administração | 1 |
