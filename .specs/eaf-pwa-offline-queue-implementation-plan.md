# Plano de Implementação — PWA Offline Queue

> **Para agentes:** use `superpowers:executing-plans` ou implemente passo a passo.

**Objetivo:** Completar a experiência PWA do template Angular: banner offline, fila de ações, prompt de instalação e push notifications.

**Abordagem:** Criar serviços Angular (`NetworkStatusService`, `OfflineQueueService`, `InstallPromptService`, `PushNotificationService`) que usam `navigator.onLine`, `window.beforeinstallprompt`, IndexedDB/localforage e Service Worker (`SwUpdate`, `SwPush`).

**Stack:** Angular 20, `@angular/service-worker`, `@angular/pwa`, `localforage`, PrimeNG.

---

## Estrutura de arquivos

- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/network-status.service.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/offline-queue.service.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/install-prompt.service.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/push-notification.service.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/components/offline-banner/offline-banner.component.ts|html|less`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/app.component.ts` (registrar listeners)
- `Templates/Angular/Eaf.ProjectName.UI/ngsw-config.json` (ajustar cache de GET/POST)
- `Templates/Angular/Eaf.ProjectName.UI/src/manifest.json` (adicionar `gcm_sender_id`, categories)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/app.module.ts` (registrar SwUpdate/SwPush)
- `Templates/Angular/Eaf.ProjectName.UI/src/environments/environment*.ts` (adicionar VAPID public key)

---

## Tarefas

### Tarefa 1: Ajustar configuração PWA

**Arquivos:**
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/ngsw-config.json`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/manifest.json`

**Passos:**

- [ ] **1.1 Cache de API de leitura (GET)**

```json
{
  "name": "api-read-cache",
  "urls": [
    "/api/AbpUserConfiguration/GetAll",
    "/api/services/app/**/Get*",
    "/api/services/app/**/GetAll*"
  ],
  "cacheConfig": {
    "maxSize": 100,
    "maxAge": "1d",
    "timeout": "30s",
    "strategy": "performance"
  }
}
```

- [ ] **1.2 POST/PUT/DELETE não devem ser cacheados**

Manter `api-write` com `maxAge: "0u"`, `maxSize: 0`, `strategy: "freshness"` (já existe).

- [ ] **1.3 Manifesto**

```json
{
  "background_color": "#fafafa",
  "theme_color": "#FF7020",
  "display": "standalone",
  "start_url": "/",
  "scope": "/",
  "categories": ["business", "productivity"],
  "icons": [...]
}
```

### Tarefa 2: Criar `NetworkStatusService`

**Arquivo:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/network-status.service.ts`

**Passos:**

- [ ] **2.1 Implementar**

```typescript
import { Injectable, OnDestroy } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class NetworkStatusService implements OnDestroy {
  private online = new BehaviorSubject<boolean>(navigator.onLine);

  constructor() {
    window.addEventListener('online', () => this.online.next(true));
    window.addEventListener('offline', () => this.online.next(false));
  }

  get isOnline$(): Observable<boolean> {
    return this.online.asObservable();
  }

  get isOnline(): boolean {
    return this.online.value;
  }

  ngOnDestroy(): void {
    this.online.complete();
  }
}
```

### Tarefa 3: Criar `OfflineQueueService`

**Arquivo:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/offline-queue.service.ts`

**Passos:**

- [ ] **3.1 Implementar fila com localforage**

```typescript
import { Injectable } from '@angular/core';
import { HttpRequest } from '@angular/common/http';
import * as localforage from 'localforage';

export interface QueuedRequest {
  id: string;
  method: string;
  url: string;
  body?: any;
  headers?: Record<string, string>;
  timestamp: number;
}

@Injectable({ providedIn: 'root' })
export class OfflineQueueService {
  private readonly store = localforage.createInstance({ name: 'eaf-offline-queue' });

  async enqueue(request: HttpRequest<any>): Promise<void> {
    const queued: QueuedRequest = {
      id: `${Date.now()}-${Math.random().toString(36).slice(2)}`,
      method: request.method,
      url: request.urlWithParams,
      body: request.body,
      headers: Object.fromEntries(request.headers.keys().map(k => [k, request.headers.get(k)])),
      timestamp: Date.now()
    };
    const items = await this.getAll();
    items.push(queued);
    await this.store.setItem('queue', items);
  }

  async dequeue(id: string): Promise<void> {
    const items = (await this.getAll()).filter(i => i.id !== id);
    await this.store.setItem('queue', items);
  }

  async getAll(): Promise<QueuedRequest[]> {
    return (await this.store.getItem<QueuedRequest[]>('queue')) ?? [];
  }

  async clear(): Promise<void> {
    await this.store.removeItem('queue');
  }
}
```

### Tarefa 4: Criar HTTP interceptor offline

**Arquivo:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/interceptors/offline.interceptor.ts`

**Passos:**

- [ ] **4.1 Interceptar POST/PUT/DELETE quando offline**

```typescript
import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { NetworkStatusService } from '../services/network-status.service';
import { OfflineQueueService } from '../services/offline-queue.service';

@Injectable()
export class OfflineInterceptor implements HttpInterceptor {
  constructor(
    private network: NetworkStatusService,
    private queue: OfflineQueueService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.isWrite(req) && !this.network.isOnline) {
      this.queue.enqueue(req);
      return of(new HttpEvent({ type: 0 } as any));
    }
    return next.handle(req);
  }

  private isWrite(req: HttpRequest<any>): boolean {
    return ['POST', 'PUT', 'PATCH', 'DELETE'].includes(req.method);
  }
}
```

**Nota:** tipo `HttpEvent` deve ser ajustado para retornar um evento de sucesso simulado (`HttpResponse`) ao invés de evento inválido.

### Tarefa 5: Criar `InstallPromptService`

**Arquivo:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/install-prompt.service.ts`

**Passos:**

- [ ] **5.1 Capturar evento `beforeinstallprompt`**

```typescript
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class InstallPromptService {
  private deferredPrompt: any;

  constructor() {
    window.addEventListener('beforeinstallprompt', (e: Event) => {
      e.preventDefault();
      this.deferredPrompt = e;
    });
  }

  canPrompt(): boolean {
    return !!this.deferredPrompt;
  }

  async prompt(): Promise<void> {
    if (!this.deferredPrompt) return;
    this.deferredPrompt.prompt();
    await this.deferredPrompt.userChoice;
    this.deferredPrompt = null;
  }
}
```

### Tarefa 6: Criar `PushNotificationService`

**Arquivo:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/services/push-notification.service.ts`

**Passos:**

- [ ] **6.1 Usar `SwPush` para inscrição**

```typescript
import { Injectable } from '@angular/core';
import { SwPush } from '@angular/service-worker';
import { HttpClient } from '@angular/common/http';
import { environment } from '@env/environment';

@Injectable({ providedIn: 'root' })
export class PushNotificationService {
  constructor(private swPush: SwPush, private http: HttpClient) {}

  isEnabled(): boolean {
    return this.swPush.isEnabled;
  }

  async requestSubscription(): Promise<void> {
    if (!this.swPush.isEnabled) return;
    const sub = await this.swPush.requestSubscription({
      serverPublicKey: environment.vapidPublicKey
    });
    await this.http.post('/api/services/app/PushSubscription/Register', sub.toJSON()).toPromise();
  }
}
```

**Dependência:** implementar endpoint backend `PushSubscriptionAppService` ou reaproveitar `Eaf.Notifications.Push`.

### Tarefa 7: Criar banner offline

**Arquivos:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/components/offline-banner/offline-banner.component.ts|html|less`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/app.component.html` (incluir banner)

**Passos:**

- [ ] **7.1 Componente**

```typescript
import { Component } from '@angular/core';
import { NetworkStatusService } from '@shared/services/network-status.service';

@Component({
  selector: 'app-offline-banner',
  templateUrl: './offline-banner.component.html',
  styleUrls: ['./offline-banner.component.less']
})
export class OfflineBannerComponent {
  constructor(public network: NetworkStatusService) {}
}
```

```html
<div *ngIf="!(network.isOnline$ | async)" class="offline-banner">
  {{ 'OfflineMode' | localize }}
</div>
```

- [ ] **7.2 Estilo**

```less
.offline-banner {
  background: #dc3545;
  color: #fff;
  text-align: center;
  padding: 8px;
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 9999;
}
```

### Tarefa 8: Integrar no app

**Arquivo:**
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/app.module.ts`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/app.component.html`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/environments/environment.ts`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/environments/environment.prod.ts`

**Passos:**

- [ ] **8.1 Registrar interceptor e ServiceWorkerModule**

```typescript
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { OfflineInterceptor } from '@shared/interceptors/offline.interceptor';

providers: [
  { provide: HTTP_INTERCEPTORS, useClass: OfflineInterceptor, multi: true }
]
```

`ServiceWorkerModule.register` já deve estar presente.

- [ ] **8.2 Adicionar VAPID key nos ambientes**

```typescript
export const environment = {
  production: true,
  vapidPublicKey: '...' // inserir chave pública VAPID
};
```

### Tarefa 9: Testes

- [ ] **9.1 Build**

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npx ng build --configuration=production
```

- [ ] **9.2 Testes unitários**

```bash
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

- [ ] **9.3 Verificar Lighthouse PWA**

```bash
npx lighthouse http://localhost:8000 --preset=desktop --only-categories=pwa
```

### Tarefa 10: Commit e PR

Branch: `feature/eaf-angular-pwa-offline-queue`.

```bash
git checkout -b feature/eaf-angular-pwa-offline-queue
...
git commit -m "feat(angular): add PWA offline queue, install prompt and push notifications"
```

---

## Cobertura da spec

| Spec item | Tarefa |
|---|---|
| Cache de assets e APIs | 1 |
| Fila de ações offline | 2, 3, 4 |
| Banner offline | 7 |
| Prompt de instalação | 5 |
| Push notifications | 6 |
