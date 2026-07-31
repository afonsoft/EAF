# EAF Angular — Dark Mode e Sistema de Temas

## Resumo
Adicionar suporte nativo a **dark mode** e um sistema de temas consistente baseado em CSS Custom Properties, permitindo que o EAF ofereça a mesma flexibilidade de personalização encontrada no ASP.NET Zero (13+ temas, incluindo modo escuro).

## Motivação
- ASP.NET Zero anuncia "13+ Theme Options with Dark Mode".
- O EAF tem 12 temas (`theme2` a `theme12`), mas todos parecem ser variações de skins claras (`header-dark`, `header-color`, `header-light`); não há dark mode global.
- Dark mode reduz fadiga visual e é esperado em aplicações enterprise modernas.
- Facilita integração com Metronic 8 / PrimeNG 17 theming.

## Estado Atual
- Cores hardcoded em `styles.css`, `customize.css` e `chat-bar.component.css`.
- Classes `header-{{ skin }}` e `m-aside-left--skin-{{ skin }}` apenas alteram header/sidebar, não o tema geral.
- Variáveis CSS limitadas: `--primary: #FF7020` definida no `style.bundle.css`.
- `AppUiCustomizationService` expõe `baseSettings` com `header.headerSkin`, `menu.asideSkin`, mas não `themeMode`.

## Proposta de Mudanças

### 1. Design Tokens CSS
Criar variáveis semânticas:
```css
:root {
  --eaf-bg: #ffffff;
  --eaf-surface: #f8f9fa;
  --eaf-text: #212529;
  --eaf-text-muted: #6c757d;
  --eaf-border: #dee2e6;
  --eaf-primary: #FF7020;
  --eaf-header-bg: #37322d; /* variável por skin */
}

[data-theme="dark"] {
  --eaf-bg: #1e1e2d;
  --eaf-surface: #2b2b40;
  --eaf-text: #f5f5f5;
  --eaf-text-muted: #a1a5b7;
  --eaf-border: #2b2b40;
  --eaf-primary: #ff8f4f;
}
```

### 2. Persistência do Modo
- Adicionar `themeMode` (`light` | `dark` | `system`) em `UiCustomizationSettingsDto`.
- Salvar preferência em `localStorage` e aplicar no `app.module.ts` / `main.ts` antes do bootstrap para evitar flash.
- Respeitar `prefers-color-scheme: dark` quando `themeMode === 'system'`.

### 3. Adaptar Componentes
- Substituir fundos e textos hardcoded por variáveis (`var(--eaf-bg)`, `var(--eaf-text)`).
- Criar tema dark para componentes PrimeNG (`p-table`, `p-dialog`, `p-calendar`).
- Ajustar `chat-bar`, tabelas, modais, formulários e dashboards.

### 4. Toggle de Tema
- Adicionar toggle no header (perfil ou configurações) para alternar light/dark.
- Atualizar classe `data-theme` no `<html>` ou `<body>`.

### 5. Consolidar Temas
- Reduzir 12 temas CSS minificados para um sistema baseado em tokens + overrides por skin.
- Manter skins `dark`, `light`, `color` para header/sidebar.

## Plano de Migração
1. Definir design tokens e adicionar `themeMode` no DTO de personalização.
2. Criar CSS base com variáveis e classes `data-theme`.
3. Refatorar componentes críticos para usar variáveis.
4. Implementar toggle e persistência.
5. Testar em todos os 12 temas + dark mode.

## Impacto
- **Alto**: altera grande parte do CSS e markup.
- **Médio**: backend (`UiCustomizationSettingsDto`) precisa incluir novo campo.
- **Alto**: melhora UX e moderniza a aparência.

## Riscos
- Temas antigos podem quebrar se variáveis não forem aplicadas corretamente.
- `service-proxies.ts` é gerado pelo NSwag; alterações no DTO exigem regeneração.
- Cores de gráficos (`chart.js`) precisam ser dinâmicas.

## Referências
- <https://primeng.org/theming> — unstyled + styled theming.
- <https://getbootstrap.com/docs/5.3/customize/color-modes/> — dark mode Bootstrap 5.
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/src/shared/common/ui/app-ui-customization.service.ts`.
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/src/shared/service-proxies/service-proxies.ts` — `UiCustomizationSettingsDto`.
