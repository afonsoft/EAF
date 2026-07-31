# EAF Angular — Layout Responsivo Mobile

## Resumo
Melhorar a experiência do template Angular do EAF em telas pequenas, substituindo o comportamento desktop-first atual por um layout mobile-first, com navegação adaptativa, menus off-canvas otimizados e componentes administrativos que funcionem bem em smartphones e tablets.

## Motivação
- O template atual (`Templates/Angular/Eaf.ProjectName.UI`) é baseado em Metronic antigo, com estrutura `m-grid--desktop` e poucas regras `@media` próprias.
- Apenas `chat-bar.component.css` possui media query específica (`max-width: 576px`); os demais componentes dependem do bundle Metronic.
- ASP.NET Zero adota **Metronic 8 + Bootstrap 5**, oferecendo grid responsivo nativo, 13+ temas, dark mode e componentes mobile-first.
- Usuários finais acessam cada vez mais dashboards e aprovações pelo celular.

## Estado Atual
- Versão do Angular: `^20.3.26` (já moderna).
- Biblioteca de UI: `primeng ^17.17.0`, `ngx-bootstrap ^12.0.0`, `ngx-scrollbar`.
- CSS/Layout: `style.bundle.css` minificado por tema, classes Metronic (`m-header`, `m-aside-left`, `m-wrapper`), mix de Bootstrap 4/Metronic antigo.
- Sidebar: `m-aside-left` é fixa em desktop; em mobile, o controle de abrir/fechar é limitado.
- Chat: `chatSideRight` já tem largura 100vw em mobile, mas o header, rodapé e lista de amigos não foram pensados para touch.
- Tabelas/admin: `primeng-datatable-container` requer scroll horizontal em telas pequenas.

## Proposta de Mudanças

### 1. Migrar para Bootstrap 5 + Metronic 8 (ou design system próprio)
- Substituir o bundle Metronic antigo por CSS/SASS do **Metronic 8** ou por **Bootstrap 5 + tema customizado EAF**.
- Reaproveitar as variáveis CSS do Bootstrap 5 (`--bs-breakpoint-sm|md|lg|xl`) para responsividade consistente.
- Manter as cores e identidade EAF (`#FF7020`) via CSS variables.

### 2. Refatorar componentes de layout
- `default-layout.component.html`, `theme2/3/4-layout.component.html`:
  - Trocar `m-stack--desktop` e `m-grid--ver-desktop` por grid flex/CSS Grid.
  - Adicionar classes condicionais para off-canvas: `offcanvas offcanvas-start` no sidebar e `offcanvas offcanvas-end` para painéis (chat, notificações).
- `topbar.component.html`:
  - Mover itens de menu para um menu hamburguer em telas < 992px.
  - Agrupar notificações, chat e perfil em uma bottom navigation ou em um top dropdown compacto.
- `side-bar-menu.component.ts/html`:
  - Transformar sidebar em drawer mobile, com gestos de swipe e overlay.
  - Adicionar toggle fixo flutuante para abrir/fechar.

### 3. Responsividade por breakpoints
```css
@media (max-width: 575.98px) { /* portrait phones */ }
@media (min-width: 576px) and (max-width: 991.98px) { /* tablets */ }
@media (min-width: 992px) { /* desktop */ }
```
- Em mobile (< 576px): header fixo no topo, sidebar escondida, conteúdo principal com padding seguro para bottom navigation.
- Em tablet (576-991px): sidebar colapsada em ícones, header expandido.
- Em desktop (>= 992px): sidebar expandida, layout atual.

### 4. Componentes administrativos
- Tabelas: usar `p-table` do PrimeNG com `responsiveLayout="scroll"` ou `stack`.
- Formulários: empilhar campos em mobile (`col-12` por padrão, `col-md-6`/`col-lg-4` em desktop).
- Modais: garantir que `p-dialog` ocupe 100% da viewport em mobile.
- Chat: manter `chatSideRight` 100vw, ajustar header recém-criado (skins) e inputs de mensagem para teclado virtual.

### 5. Navegação touch
- Aumentar áreas de toque para botões (>= 44x44px).
- Adicionar suporte a gestos para sidebar e chat (`hammerjs` já está em `eaf-web-resources`).
- Evitar hover-only (tooltips, dropdowns) em touch.

### 6. Acessibilidade móvel
- Garantir `viewport` meta, `touch-action`, foco visível em campos.
- Testar com leitor de tela e navegação por teclado externo.

## Plano de Migração
1. **Fase 1 — Inventory**: listar todos os componentes de layout e admin que precisam de ajuste.
2. **Fase 2 — Spike**: criar um tema mobile-first alternativo (p.ex. `theme13`) sem afetar os atuais.
3. **Fase 3 — Componentes**: ajustar `layout`, `topbar`, `side-bar-menu`, `chat-bar`, tabelas e modais.
4. **Fase 4 — Testes**: testes em dispositivos reais/emuladores, Cypress/Playwright com viewports mobile.
5. **Fase 5 — Rollout**: tornar o novo layout padrão, deprecar temas antigos gradualmente.

## Impacto
- **Alto**: altera markup e CSS dos componentes de layout.
- **Médio**: possível necessidade de ajustar testes e2e existentes.
- **Baixo**: regras de negócio e APIs não são afetadas.

## Riscos
- Bundle Metronic antigo é grande e minificado; mudanças indevidas podem quebrar CSS global.
- Themes múltiplos (12) aumentam custo de teste.
- `ngx-bootstrap` pode conflitar com Bootstrap 5 JS; avaliar migração ou `ng-bootstrap`.

## Referências
- <https://aspnetzero.com/angular> — Metronic 8, Bootstrap 5, 13+ temas, dark mode.
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout` — componentes de layout atuais.
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/themes` — bundles CSS por tema.
