# EAF Modernização — Plano de Implementação por Feature

> **For agentic workers:** REQUIRED SUB-SKILL: Use `superpowers:subagent-driven-development` (recommended) or `superpowers:executing-plans` to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implementar incrementalmente as melhorias documentadas nas 9 specs da pasta `.specs/` do repositório `afonsoft/EAF`, começando pelas mudanças de menor risco e maior ROI e avançando para as mudanças arquiteturais. Cada feature deve ser entregue em PRs isolados e testáveis.

**Architecture:** Manter o template Angular (`Templates/Angular/Eaf.ProjectName.UI`) e o backend (`src/`) compatíveis com projetos gerados a partir do EAF. Aplicar mudanças por trás de feature flags ou temas alternativos sempre que possível, e validar com `npx tsc`, `npx ng build --configuration=production` e `dotnet test`.

**Tech Stack:** Angular 20 + PrimeNG 17 + Metronic legado, .NET 10 + ASP.NET Boilerplate (ABP), Castle Windsor, EF Core, xUnit/Shouldly/NSubstitute.

---

## Ordem de Execução (do mais simples ao mais complexo)

| # | Feature | Risco | Esforço | Faz sentido? | Justificativa |
|---|---------|-------|---------|--------------|---------------|
| 1 | Acessibilidade (a11y) | Baixo | Baixo | Sim | Mudanças de markup/ARIA; não quebra APIs nem lógica de negócio. |
| 2 | PWA/Offline | Baixo | Baixo/Médio | Sim | `@angular/pwa` e `@angular/service-worker` já estão instalados; manifest e SW já existem; faltam detalhes de UX offline. |
| 3 | Mobile Responsive Layout | Médio | Médio | Sim | Melhoria direta de UX; pode ser feita sem migrar Metronic. |
| 4 | Dark Mode e Sistema de Temas | Médio | Médio/Alto | Sim | Alto valor, mas exige alteração no DTO de UI customization e em CSS. |
| 5 | Modernização PrimeNG | Médio | Alto | Parcial | Substituir `ngx-bootstrap` completo é grande; pode ser feito componente a componente. |
| 6 | Metronic 8 + Bootstrap 5 | Alto | Muito alto | Avaliar | Requer licença Metronic 8 e reescrita massiva de markup; faz sentido apenas se houver budget/licença. |
| 7 | Backend Modularization | Médio/Alto | Alto | Sim (priorizar) | `Eaf.BlobStoring` e `Eaf.HtmlSanitizer` são realmente ausentes e necessários. MailKit/Redis já existem. |
| 8 | ASP.NET Zero Feature Adoption | Alto | Muito alto | Parcial | Editions, Payment e OU têm alto valor comercial; devem ser priorizados individualmente. |
| 9 | ABP Feature Parity | Médio/Alto | Alto | Parcial | Vários módulos (Dapper, FluentValidation, MongoDB, OData) são nice-to-have; priorizar os que faltam no dia a dia. |

---

## Considerações por Feature

### 1. Acessibilidade (a11y)

**Faz sentido:** Sim, imediato.
**Escopo:** Melhorar HTML semântico, roles, labels, contraste e foco visível no template Angular.

**Symbols / arquivos a modificar (ordem de controle/data flow):**

1. `src/index.html:1-30`
   - Verificar `<meta name="viewport">` e `<html lang>` dinâmico por cultura.

2. `src/app/shared/layout/themes/default/default-layout.component.html`
   - `Theme2LayoutComponent`, `Theme3LayoutComponent`, `Theme4LayoutComponent` templates
   - Trocar `<div>` raiz por `<header>`, `<nav>`, `<main>`, `<aside>`.
   - Adicionar `aria-label` ao `<aside>` da sidebar e `role="banner"` ao header.

3. `src/app/shared/layout/nav/side-bar-menu.component.ts:30-65`
   ```ts
   export class SideBarMenuComponent extends AppComponentBase implements OnInit, AfterViewInit {
     // Adicionar aria-expanded tracking para itens de menu
     @HostBinding('attr.aria-expanded') expanded = 'false';
   }
   ```
   - Template: `<nav aria-label="Main">`, menu recursivo com `role="tree"`, `aria-expanded`.

4. `src/app/shared/layout/topbar.component.ts:18-40`
   - Dropdowns com `role="menu"`, `aria-haspopup`, `aria-expanded`.

5. `src/app/shared/layout/chat/chat-bar.component.html:12-19`
   - Já tem `aria-label` e `aria-labelledby`; garantir `role="complementary"` e foco no input ao abrir.

6. `src/app/shared/layout/profile/*.component.html`
   - Modais: adicionar `cdkTrapFocus` ou `p-focusTrap`.

7. `src/app/shared/layout/notifications/header-notifications.component.ts`
   - `aria-live="polite"` para listas de notificações.

8. `src/app/main/main.component.html` e tabelas administrativas
   - `p-table` com `[responsiveLayout]="'scroll'"` e `<caption>`.

9. `src/assets/common/styles/styles.css`
   - Garantir `:focus-visible` outline com contraste.

**Testes:**
- `npx tsc -p src/tsconfig.app.json --noEmit`
- `npx ng build --configuration=production`
- Lighthouse a11y audit >= 90.
- Axe DevTools sem erros críticos.

---

### 2. PWA / Cache / Offline

**Faz sentido:** Sim; a infraestrutura já existe.
**Observação:** `app.module.ts:116` já registra `ServiceWorkerModule`; `ngsw-config.json` e `src/manifest.json` já existem. O trabalho é aprimorar UX offline e cache.

**Symbols / arquivos a modificar (ordem de controle/data flow):**

1. `src/manifest.json`
   - Atualizar `name`, `short_name`, `theme_color`, `background_color` para valores EAF (`#FF7020`/`#37322d`).

2. `ngsw-config.json:30-43`
   - Adicionar `dataGroups` por tenant/API:
     ```json
     {
       "name": "tenant-config",
       "urls": ["/AbpUserConfiguration/GetAll"],
       "cacheConfig": { "maxSize": 20, "maxAge": "1h", "strategy": "performance" }
     }
     ```

3. `src/app/app.component.ts`
   - Injetar `SwUpdate` e exibir toast quando `versionUpdates` disponível.

4. Criar `src/app/shared/utils/network.service.ts`
   ```ts
   @Injectable({ providedIn: 'root' })
   export class NetworkService {
     online$ = fromEvent(window, 'online').pipe(map(() => true));
     offline$ = fromEvent(window, 'offline').pipe(map(() => false));
     isOnline = toSignal(merge(of(navigator.onLine), this.online$, this.offline$));
   }
   ```

5. Criar `src/app/shared/layout/offline-banner.component.{ts,html}`
   - Exibe banner quando `NetworkService.isOnline()` é `false`.

6. `src/app/shared/layout/chat/chat-bar.component.ts:58-120`
   - Integrar fila offline para mensagens usando `localforage`.

7. Criar `src/app/shared/service-worker/update.service.ts`
   - Lógica para `SwUpdate.activateUpdate()` e reload.

**Testes:**
- `npx ng build --configuration=production` gera `ngsw-worker.js`.
- `npx lighthouse --preset=desktop` PWA checks.
- Simular offline em Chrome DevTools > Application > Service Workers.

---

### 3. Mobile Responsive Layout

**Faz sentido:** Sim, sem precisar de Metronic 8.
**Escopo:** Adicionar media queries, aumentar touch targets, melhorar off-canvas e tabelas.

**Symbols / arquivos a modificar (ordem de controle/data flow):**

1. `src/assets/common/styles/styles.css`
   - Adicionar CSS custom properties e breakpoints:
     ```css
     :root {
       --eaf-touch-target: 44px;
     }
     @media (max-width: 991.98px) {
       .m-aside-left { display: none; }
       .m-aside-left--open { display: block; position: fixed; z-index: 1040; }
     }
     ```

2. `src/app/shared/layout/themes/default/default-layout.component.html`
   - Substituir `m-grid--desktop` por flexbox/CSS Grid.
   - Adicionar classes `.layout-mobile`, `.layout-desktop` via `@HostBinding`.

3. `src/app/shared/layout/nav/side-bar-menu.component.ts:30-65`
   - Adicionar `@HostListener('window:resize')` para colapsar sidebar em `< 992px`.
   - Adicionar toggle flutuante.

4. `src/app/shared/layout/topbar.component.ts` e `.html`
   - Menu hamburguer < 992px.
   - Agrupar notificações/chat/perfil em dropdown compacto.

5. `src/app/shared/layout/chat/chat-bar.component.css:80-90`
   - Ampliar media query existente; ajustar touch targets e altura do input.

6. `src/app/main/*/*.component.html` (tabelas e formulários)
   - `p-table` com `responsiveLayout="stack"`.
   - Formulários: `class="col-12 col-md-6 col-lg-4"`.

7. `src/app/shared/layout/titlebar.component.ts:11-25`
   - Garantir que título e botões de ação não quebrem em telas pequenas.

**Testes:**
- Chrome DevTools viewports: 375px, 768px, 1024px, 1920px.
- `npx ng build --configuration=production` sem erros de CSS.
- Testes e2e com Playwright/Cypress em mobile.

---

### 4. Dark Mode e Sistema de Temas

**Faz sentido:** Sim, mas requer backend + frontend.
**Escopo:** Introduzir design tokens CSS e persistência de `themeMode`.

**Symbols / arquivos a modificar (ordem de controle/data flow):**

1. Backend: `src/Eaf.Middleware.Core/Configuration/UiManagement/ThemeSettings.cs` (ou DTO equivalente)
   - Adicionar `ThemeMode`: `Light`, `Dark`, `System`.

2. `Templates/Angular/Eaf.ProjectName.UI/src/shared/service-proxies/service-proxies.ts`
   - `UiCustomizationSettingsDto` / `ThemeSettingsDto` devem conter `themeMode?: string`.
   - **Atenção:** arquivo gerado pelo NSwag; alterar DTO no backend e regenerar.

3. `src/shared/common/ui/app-ui-customization.service.ts`
   - Getter `themeMode()` e método `applyTheme(mode: 'light' | 'dark' | 'system')`.

4. `src/main.ts` / `src/app/app.module.ts`
   - Ler `localStorage.getItem('eaf-theme')` antes do bootstrap e aplicar `data-theme`.

5. `src/assets/common/styles/styles.css`
   - Tokens:
     ```css
     :root {
       --eaf-bg: #ffffff;
       --eaf-surface: #f8f9fa;
       --eaf-text: #212529;
       --eaf-text-muted: #6c757d;
       --eaf-border: #dee2e6;
       --eaf-primary: #FF7020;
     }
     [data-theme="dark"] {
       --eaf-bg: #1e1e2d;
       --eaf-surface: #2b2b40;
       --eaf-text: #f5f5f5;
       --eaf-text-muted: #a1a5b7;
       --eaf-border: #2b2b40;
     }
     ```

6. `src/app/shared/layout/themes/*/*-layout.component.ts`
   - Aplicar `data-theme` no host element.

7. `src/app/shared/layout/topbar.component.html`
   - Adicionar toggle light/dark/system.

8. `src/app/shared/layout/chat/chat-bar.component.css:263-298`
   - Usar `var(--eaf-*)` ao invés de cores hardcoded.

**Testes:**
- `npx tsc`, `npx ng build --configuration=production`.
- Verificar `prefers-color-scheme` quando `themeMode === 'system'`.
- Testes de contraste no Lighthouse.

---

### 5. Modernização PrimeNG

**Faz sentido:** Parcialmente. Substituir `ngx-bootstrap` componente a componente.
**Escopo:** Padronizar dropdowns, datepickers, modais, tabs, tooltips no PrimeNG.

**Symbols / arquivos a modificar (ordem de controle/data flow):**

1. `package.json:83,96`
   - Manter `ngx-bootstrap` durante a transição; remover só quando todos os componentes migrarem.

2. `src/app/main/users/create-or-edit-user-modal.component.html`
   - Substituir `bs-modal` por `p-dialog`.

3. `src/app/main/tenants/tenants.component.html` e `src/app/main/roles/roles.component.html`
   - Substituir `bs-dropdown` por `p-dropdown` / `p-splitButton`.
   - `bs-datepicker` por `p-calendar`.

4. `src/app/shared/layout/topbar.component.html`
   - Tooltips `tooltip` por `pTooltip`.

5. `src/app/shared/layout/nav/side-bar-menu.component.html`
   - Tabs/accordion por `p-tabView` / `p-accordion` se houver.

6. `src/app/shared/common/app-component-base.ts`
   - Garantir que localize pipe e notificações continuem funcionando com PrimeNG.

7. `src/assets/common/styles/styles.css`
   - Ajustes de variáveis para PrimeNG theming.

**Testes:**
- `npx eslint` nos arquivos migrados.
- Testes unitários com `TestBed` + `BrowserAnimationsModule`.
- `npx ng build --configuration=production`.

---

### 6. Metronic 8 + Bootstrap 5

**Faz sentido:** Depende de licença Metronic 8. Sem licença, fazer design system próprio.
**Escopo:** Migrar markup e classes legadas para Bootstrap 5 (ou Metronic 8 se licenciado).

**Symbols / arquivos a modificar (ordem de controle/data flow):**

1. `package.json`
   - Adicionar `bootstrap@^5.3.3` e `@popperjs/core`.
   - Remover `ngx-bootstrap` se todos os componentes já migraram para PrimeNG.

2. `src/app/shared/layout/themes/default/default-layout.component.html`
   - Novo layout base:
     ```html
     <div id="kt_app_root" class="d-flex flex-column flex-root">
       <div id="kt_app_page" class="app-page flex-row flex-column-fluid">
         <app-sidebar class="app-sidebar offcanvas offcanvas-start"></app-sidebar>
         <div class="app-wrapper d-flex flex-column flex-row-fluid">
           <app-header class="app-header"></app-header>
           <div class="app-main flex-column flex-row-fluid">
             <router-outlet></router-outlet>
           </div>
         </div>
       </div>
     </div>
     ```

3. `src/app/shared/layout/nav/side-bar-menu.component.ts`
   - Menu recursivo com Bootstrap 5 offcanvas.

4. `src/app/shared/layout/topbar.component.ts` e `.html`
   - Navbar Bootstrap 5 com collapse.

5. `src/app/shared/layout/chat/chat-bar.component.html`
   - Usar `offcanvas offcanvas-end` nativo do Bootstrap 5.

6. `src/assets/common/styles/themes/theme4/style.bundle.css`
   - Avaliar substituição por `bootstrap.min.css` + tema EAF customizado.

7. `angular.json:47-50`
   - Atualizar `styles` e `scripts` arrays.

**Testes:**
- Build completo em todos os temas.
- Testes e2e em todos os breakpoints.
- Verificar que `ngx-bootstrap` foi completamente removido.

---

### 7. Backend Modularization

**Faz sentido:** Sim; priorizar `Eaf.BlobStoring` e `Eaf.HtmlSanitizer` que realmente faltam.
**Observação:** `MailKit` e `Redis` já existem no EAF (`MiddlewareMailKitSmtpBuilder.cs`, `RedisConfigurer.cs`). Não recriar.

**Symbols / arquivos a modificar (ordem de controle/data flow):**

#### 7.1 Eaf.BlobStoring

1. Criar `src/Eaf.BlobStoring/Eaf.BlobStoring.csproj`
2. Criar `src/Eaf.BlobStoring/Containers/IBlobContainer.cs`
   ```csharp
   public interface IBlobContainer<TContainer> : ITransientDependency
   {
       Task SaveAsync(string name, Stream stream, bool overrideExisting = false);
       Task<Stream> GetAsync(string name);
       Task<bool> DeleteAsync(string name);
   }
   ```
3. Criar `src/Eaf.BlobStoring/FileSystem/FileSystemBlobContainer.cs`
4. Criar `src/Eaf.BlobStoring.Azure/AzureBlobContainer.cs`
5. Criar `src/Eaf.BlobStoring.Oci/OciObjectStorageBlobContainer.cs`
6. `src/Eaf.Middleware.Web.Core/MiddlewareWebCoreModule.cs`
   - Registrar `IBlobContainer<>` por configuração.
7. `src/Eaf.Middleware.Application/Chat/ChatMessageManager.cs`
   - Usar `IBlobContainer<ChatBlobContainer>` para anexos.

#### 7.2 Eaf.HtmlSanitizer

1. Criar `src/Eaf.HtmlSanitizer/Eaf.HtmlSanitizer.csproj`
2. Criar `src/Eaf.HtmlSanitizer/IHtmlSanitizer.cs` e `HtmlSanitizer.cs` (wrapper do HtmlSanitizer package).
3. `src/Eaf.Middleware.Application/Chat/ChatMessageManager.cs`
   - Sanitizar HTML antes de salvar.
4. `src/Eaf.Middleware.Application/Emailing/*`
   - Sanitizar templates HTML.

#### 7.3 Eaf.SignalR (refatorar módulo existente)

1. `src/Eaf.Middleware.Web.Core/SignalR/Chat/ChatHub.cs`
   - Já existe; extrair `Eaf.SignalR` se necessário para outros hubs.
2. Criar `src/Eaf.SignalR/HubBase.cs` genérico.
3. Criar `src/Eaf.SignalR/Notifications/NotificationHub.cs`.
4. Adicionar backplane Redis para multi-instância.

#### 7.4 Eaf.OpenIddict

1. Criar `src/Eaf.OpenIddict/Eaf.OpenIddict.csproj`
2. Criar `src/Eaf.OpenIddict/EafOpenIddictModule.cs`
3. `src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs`
   - Adicionar opção de usar OpenIddict para tokens.

**Testes:**
- `dotnet build Eaf.sln`
- `dotnet test Eaf.sln --collect:"XPlat Code Coverage"`
- Testes de integração para Blob (filesystem) e HtmlSanitizer.

---

### 8. ASP.NET Zero Feature Adoption

**Faz sentido:** Parcial; priorizar Editions, Organization Units e Tenant Self-Registration.
**Escopo:** Adicionar funcionalidades enterprise sem copiar código do Zero (usar como referência de requisitos).

**Symbols / arquivos a modificar (ordem de controle/data flow):**

#### 8.1 Editions & Feature Management

1. `src/Eaf.Middleware.Core/Authorization/Edition.cs` (verificar se existe)
   - Adicionar `FeatureValues` JSON.
2. `src/Eaf.Middleware.Application/Editions/EditionAppService.cs`
   - CRUD de editions e feature values.
3. Angular: `src/app/main/editions/*`
   - Criar lista e modal de editions.

#### 8.2 Organization Units

1. `src/Eaf.Middleware.Core/Authorization/OrganizationUnit.cs`
   - Entidade com `ParentId`, `Code`.
2. `src/Eaf.Middleware.Application/OrganizationUnits/OrganizationUnitAppService.cs`
   - CRUD e movimentação na árvore.
3. Angular: `src/app/main/organization-units/*`
   - Árvore e atribuição de usuários.

#### 8.3 Tenant Self-Registration

1. `src/Eaf.Middleware.Web.Core/Controllers/AccountController.cs`
   - Adicionar endpoint `RegisterTenant`.
2. `src/Eaf.Middleware.Application/Account/AccountAppService.cs`
   - Criar tenant e usuário admin.
3. Angular: `src/app/account/register-tenant/*`
   - Página pública de registro.

#### 8.4 Audit Log UI Avançado

1. `src/Eaf.Middleware.Application/AuditLogs/AuditLogAppService.cs`
   - Adicionar filtros e exportação CSV/PDF.
2. Angular: `src/app/main/audit-logs/*`
   - Filtros avançados, exportação.

**Testes:**
- Testes BDD para cada funcionalidade.
- `dotnet test`.

---

### 9. ABP Feature Parity

**Faz sentido:** Parcial; avaliar demanda real.
**Prioridade sugerida:**
1. **Eaf.BlobStoring** (já coberto no item 7).
2. **Eaf.HtmlSanitizer** (já coberto no item 7).
3. **Eaf.Dapper** — queries complexas e relatórios.
4. **Eaf.FluentValidation** — integração opcional com DTOs.
5. **Eaf.MongoDB** — apenas se houver caso de uso NoSQL.
6. **Eaf.OData** — apenas se APIs precisarem de query OData.
7. **Eaf.Quartz** — alternativa ao Hangfire.

**Symbols / arquivos a modificar:**

- `Eaf.Dapper`: `src/Eaf.Dapper/Eaf.Dapper.csproj`, `DapperRepositoryBase.cs`, `IDapperRepository.cs`.
- `Eaf.FluentValidation`: `src/Eaf.FluentValidation/Eaf.FluentValidation.csproj`, `FluentValidationMethodParameterValidator.cs`.
- `Eaf.Quartz`: `src/Eaf.Quartz/Eaf.Quartz.csproj`, `QuartzJobManager.cs`, `IQuartzJob`.

**Testes:**
- Unit tests e integration tests para cada módulo novo.

---

## Cronograma Sugerido (Sprints de 2 semanas)

| Sprint | Features | Entregáveis |
|--------|----------|-------------|
| 1 | 1 (a11y) + 2 (PWA UX offline) | PR com markup ARIA, banner offline, cache config |
| 2 | 3 (Mobile) | PR com breakpoints, off-canvas, tabelas responsivas |
| 3-4 | 4 (Dark Mode) | PR com design tokens + backend DTO + toggle |
| 5-7 | 5 (PrimeNG) | PRs por componente (modal, dropdown, datepicker, tabs) |
| 8-12 | 6 (Metronic 8/Bootstrap 5) | PR grande de migração de layout e temas |
| 13-15 | 7 (Backend) | PRs `Eaf.BlobStoring`, `Eaf.HtmlSanitizer`, `Eaf.SignalR` |
| 16-20 | 8 (Zero features) | Editions, OU, Tenant Self-Reg, Audit Log UI |
| 21+ | 9 (ABP parity) | Dapper, FluentValidation, Quartz, etc. |

---

## Riscos Transversais

1. **service-proxies.ts é gerado:** alterações em DTOs do backend exigem regeneração via NSwag.
2. **Temas antigos:** 12 temas minificados aumentam custo de teste; recomenda-se criar `theme13` e depois deprecar.
3. **ngx-bootstrap vs Bootstrap 5:** versão atual pode não suportar Bootstrap 5; migrar para PrimeNG primeiro.
4. **Multi-tenancy:** cache, blob e push notifications devem isolar dados por tenant.
5. **Licença Metronic 8:** sem licença, optar por Bootstrap 5 + design system próprio.
6. **Testes:** aumento de cobertura exigido pela AGENTS.md (>= 90%); todo backend novo precisa de xUnit.

---

## Referências

- `.specs/eaf-angular-accessibility-a11y.spec.md`
- `.specs/eaf-angular-pwa-offline.spec.md`
- `.specs/eaf-angular-mobile-responsive-layout.spec.md`
- `.specs/eaf-angular-dark-mode-theming.spec.md`
- `.specs/eaf-angular-modern-primeng-components.spec.md`
- `.specs/eaf-angular-metronic8-bootstrap5-migration.spec.md`
- `.specs/eaf-backend-modularization.spec.md`
- `.specs/eaf-aspnetzero-feature-adoption.spec.md`
- `.specs/eaf-abp-feature-parity.spec.md`
