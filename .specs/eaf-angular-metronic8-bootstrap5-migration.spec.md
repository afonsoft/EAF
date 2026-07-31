# EAF Angular — Migração Metronic 8 + Bootstrap 5

## Resumo
Migrar o template Angular EAF do Metronic antigo (Bootstrap 4 / bundle minificado) para **Metronic 8 baseado em Bootstrap 5**, aproximando-o da stack usada pelo ASP.NET Zero e garantindo responsividade nativa, dark mode e manutenibilidade.

## Motivação
- O bundle Metronic atual em `src/assets/lib/metronic` não tem versionamento explícito e usa Font Awesome 5.2 / Bootstrap 4.
- Metronic 8 oferece: Bootstrap 5, SASS, design tokens, componentes Angular modernos, 13+ layouts, dark mode nativo.
- ASP.NET Zero já usa essa combinação, reduzindo gap de UX entre as plataformas.
- Bootstrap 5 remove dependência do jQuery para componentes JS, alinhando-se a apps Angular modernos.

## Estado Atual
- `vendors.bundle.css` contém Font Awesome + Line Awesome + perfect-scrollbar.
- `style.bundle.css` em cada tema (`theme2` a `theme12`) é um arquivo CSS minificado enorme (~1,4 MB por tema).
- Não há fonte SASS/SCSS do tema; customizações são feitas em `customize.css` e `styles.css`.
- `ngx-bootstrap ^12.0.0` é usado para componentes como datepicker, dropdown, modal; pode conflitar com Bootstrap 5.

## Proposta de Mudanças

### 1. Adotar Bootstrap 5 como base
- Incluir `@popperjs/core` e `bootstrap` no `package.json`.
- Substituir `ngx-bootstrap` por componentes do PrimeNG já presentes (`p-calendar`, `p-dialog`, `p-dropdown`) ou por `ng-bootstrap` caso seja necessário manter APIs similares.
- Atualizar classes de grid: `col-*` ainda existem, mas `form-group`, `form-row`, `media`, `jumbotron` foram removidos no Bootstrap 5.

### 2. Introduzir Metronic 8 via assets ou pacote
- Adquirir/atualizar licença Metronic 8 e copiar os assets (`style.bundle.css`, `scripts.bundle.js`) para `src/assets/metronic8`.
- Manter fallback: criar `theme13` como tema Metronic 8, sem remover os antigos na primeira fase.
- Substituir as classes legadas `m-stack`, `m-grid__item`, `m-header--skin-*` pelas classes Metronic 8 (`header`, `aside`, `wrapper`, `content`).

### 3. CSS Custom Properties
- Substituir cores hardcoded (`#FF7020`, `#37322d`, `#efefef`) por variáveis CSS (`--bs-primary`, `--eaf-header-dark`, `--eaf-header-light`).
- Facilitar temas dinâmicos e dark mode.

### 4. Ajustar componentes de layout
- `default-layout.component.html`: usar layout container Metronic 8 (`#kt_app_root`, `#kt_app_header`, `#kt_app_sidebar`, `#kt_app_content`).
- `topbar.component.html`: migrar para `kt-menu` e componentes de toolbar Metronic 8.
- `side-bar-menu.component`: usar `kt-app-sidebar` com menu recursivo e suporte a collapse.
- `chat-bar.component`: manter off-canvas do Bootstrap 5 (`offcanvas offcanvas-end`) em vez de classes customizadas.

### 5. Iconografia
- Migrar de `flaticon` / `line-awesome` antigos para **Font Awesome 6 Free** ou **Bootstrap Icons**.
- Criar mapeamento de ícones legados para novos.

### 6. Scripts e jQuery
- Remover `jquery` e `mdbootstrap` do bundle crítico, mantendo apenas onde for estritamente necessário (legacy plugins).
- Substituir inicializações jQuery por diretivas Angular ou CDK.

## Plano de Migração
1. **Auditoria**: listar todos os usos de classes Metronic antigas e `ngx-bootstrap`.
2. **Bootstrap 5**: atualizar markup básico, variáveis e grid.
3. **Metronic 8 Theme**: criar `theme13` com assets Metronic 8 e apontar um app demo para ele.
4. **Componentes**: migrar layout, topbar, sidebar, chat, footer, modais.
5. **Testes**: `ng build --configuration=production`, testes e2e em todos os breakpoints.
6. **Deprecação**: remover temas antigos após ciclos de validação.

## Impacto
- **Alto**: substituição de dependências e markup de layout.
- **Alto**: possível quebra de temas customizados dos usuários do EAF.
- **Médio**: documentação de upgrade para projetos gerados a partir do template.

## Riscos
- Licença Metronic 8 e compatibilidade com PrimeNG 17.
- `ngx-bootstrap` pode não ter suporte a Bootstrap 5 na versão atual; requer migração.
- Bundle de assets muito grande; precisa de lazy loading por tema.

## Referências
- <https://keenthemes.com/metronic/tailwind/react/> / <https://keenthemes.com/metronic/tailwind/angular/> — Metronic 8 Angular.
- <https://getbootstrap.com/docs/5.0/migration/> — guia de migração Bootstrap 4 → 5.
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/src/assets/common/styles/themes/theme4/style.bundle.css`.
