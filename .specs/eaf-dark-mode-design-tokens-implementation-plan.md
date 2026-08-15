# Plano de Implementação — Dark Mode e Design Tokens

> **Para agentes:** use `superpowers:executing-plans` ou implemente passo a passo.

**Objetivo:** Adicionar suporte a dark mode no template Angular via design tokens CSS, troca dinâmica de tema PrimeNG e persistência da preferência do usuário.

**Abordagem:** Criar arquivo `src/assets/common/styles/design-tokens.css` com variáveis de cor para light/dark, alternar classe `data-theme` no `<html>`, carregar o tema PrimeNG correspondente e salvar a escolha em `localStorage`.

**Stack:** Angular 20, PrimeNG 17, CSS custom properties, `localStorage`.

---

## Estrutura de arquivos

- `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/design-tokens.css`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout/theme.service.ts`
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout/header/header.component.ts` (adicionar toggle)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout/header/header.component.html`
- `Templates/Angular/Eaf.ProjectName.UI/src/index.html` (adicionar `data-theme`)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/app.component.ts` (aplicar tema ao iniciar)
- `Templates/Angular/Eaf.ProjectName.UI/angular.json` (separar styles por tema)
- `Templates/Angular/Eaf.ProjectName.UI/src/app/app.module.ts` (carregar tema PrimeNG dinamicamente)
- `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/styles.css` (usar variáveis)

---

## Tarefas

### Tarefa 1: Criar design tokens

**Arquivo:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/design-tokens.css`

**Passos:**

- [ ] **1.1 Definir variáveis para light e dark**

```css
:root,
[data-theme="light"] {
  --eaf-bg-primary: #ffffff;
  --eaf-bg-secondary: #f8f9fa;
  --eaf-text-primary: #212529;
  --eaf-text-secondary: #6c757d;
  --eaf-border: #dee2e6;
  --eaf-primary: #FF7020;
  --eaf-primary-hover: #e66018;
  --eaf-sidebar-bg: #ffffff;
  --eaf-sidebar-text: #212529;
  --eaf-card-bg: #ffffff;
}

[data-theme="dark"] {
  --eaf-bg-primary: #121212;
  --eaf-bg-secondary: #1e1e1e;
  --eaf-text-primary: #e9ecef;
  --eaf-text-secondary: #adb5bd;
  --eaf-border: #343a40;
  --eaf-primary: #FF7020;
  --eaf-primary-hover: #ff8f4d;
  --eaf-sidebar-bg: #1e1e1e;
  --eaf-sidebar-text: #e9ecef;
  --eaf-card-bg: #1e1e1e;
}
```

- [ ] **1.2 Adicionar ao `angular.json` styles**

```json
{
  "input": "src/assets/common/styles/design-tokens.css",
  "bundleName": "design-tokens",
  "inject": true
}
```

### Tarefa 2: Criar `ThemeService`

**Arquivo:**
- Criar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout/theme.service.ts`

**Passos:**

- [ ] **2.1 Implementar serviço**

```typescript
import { Injectable, Renderer2, RendererFactory2 } from '@angular/core';

export type EafTheme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly key = 'eaf-theme';
  private renderer: Renderer2;

  constructor(rendererFactory: RendererFactory2) {
    this.renderer = rendererFactory.createRenderer(null, null);
  }

  initialize(): void {
    const saved = this.getSavedTheme();
    this.apply(saved);
  }

  toggle(): void {
    const current = this.getSavedTheme();
    const next: EafTheme = current === 'dark' ? 'light' : 'dark';
    this.apply(next);
  }

  private apply(theme: EafTheme): void {
    this.renderer.setAttribute(document.documentElement, 'data-theme', theme);
    localStorage.setItem(this.key, theme);
    this.loadPrimeNgTheme(theme);
  }

  private getSavedTheme(): EafTheme {
    return (localStorage.getItem(this.key) as EafTheme) ?? 'light';
  }

  private loadPrimeNgTheme(theme: EafTheme): void {
    const light = 'node_modules/primeng/resources/themes/lara-light-blue/theme.css';
    const dark = 'node_modules/primeng/resources/themes/lara-dark-blue/theme.css';
    const href = theme === 'dark' ? dark : light;
    const existing = document.getElementById('eaf-primeng-theme') as HTMLLinkElement;

    if (existing) {
      existing.href = href;
    } else {
      const link = document.createElement('link');
      link.id = 'eaf-primeng-theme';
      link.rel = 'stylesheet';
      link.href = href;
      document.head.appendChild(link);
    }
  }
}
```

### Tarefa 3: Aplicar tema ao iniciar

**Arquivo:**
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/app.component.ts`

**Passos:**

- [ ] **3.1 Injetar `ThemeService` no `AppComponent`**

```typescript
import { Component, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeService } from '@shared/layout/theme.service';

@Component({
  selector: 'app-root',
  template: '<router-outlet></router-outlet>'
})
export class AppComponent extends AppComponentBase implements OnInit {
  constructor(
    injector: Injector,
    private themeService: ThemeService
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.themeService.initialize();
  }
}
```

### Tarefa 4: Adicionar toggle no header

**Arquivo:**
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout/header/header.component.ts`
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout/header/header.component.html`

**Passos:**

- [ ] **4.1 Método no componente**

```typescript
import { Component, Injector } from '@angular/core';
import { AppComponentBase } from '@shared/common/app-component-base';
import { ThemeService } from '../theme.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html'
})
export class HeaderComponent extends AppComponentBase {
  constructor(
    injector: Injector,
    public themeService: ThemeService
  ) { super(injector); }

  toggleTheme(): void {
    this.themeService.toggle();
  }
}
```

- [ ] **4.2 Botão no template**

```html
<button type="button" class="btn btn-icon" (click)="toggleTheme()">
  <i class="pi pi-sun" *ngIf="(themeService.theme$ | async) === 'light'"></i>
  <i class="pi pi-moon" *ngIf="(themeService.theme$ | async) === 'dark'"></i>
</button>
```

Se `themeService` não expõe `theme$`, ajustar o serviço para expor um `BehaviorSubject`.

### Tarefa 5: Refatorar estilos existentes

**Arquivo:**
- Modificar: `Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/styles.css`

**Passos:**

- [ ] **5.1 Substituir cores fixas por variáveis nos seletores principais**

```css
body {
  background-color: var(--eaf-bg-primary);
  color: var(--eaf-text-primary);
}

.page-sidebar {
  background-color: var(--eaf-sidebar-bg);
  color: var(--eaf-sidebar-text);
}

.card {
  background-color: var(--eaf-card-bg);
  border-color: var(--eaf-border);
}
```

Realizar busca por `#fff`, `#ffffff`, `#000`, `background`, `color` e substituir progressivamente pelos tokens onde apropriado. Fazer a troca apenas nos estilos customizados do EAF, não nos bundles de terceiros.

### Tarefa 6: Testar build e regressão visual

- [ ] **6.1 Build**

```bash
cd Templates/Angular/Eaf.ProjectName.UI
npx ng build --configuration=production
```

- [ ] **6.2 Testes unitários**

```bash
npx ng test --no-watch --browsers=ChromeHeadlessNoSandbox
```

- [ ] **6.3 Commit e PR**

Branch: `feature/eaf-dark-mode-design-tokens`.

```bash
git checkout -b feature/eaf-dark-mode-design-tokens
...
git commit -m "feat(angular): add dark mode and design tokens"
```

---

## Cobertura da spec

| Spec item | Tarefa |
|---|---|
| Design tokens centralizados | 1 |
| Troca de tema light/dark | 2, 3 |
| Persistência da preferência | 2 |
| Atualização visual das telas | 5 |
