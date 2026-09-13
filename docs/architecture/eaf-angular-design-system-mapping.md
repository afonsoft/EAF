# EAF Angular UI & Design System Mapping

Este documento fornece um mapeamento completo e detalhado da arquitetura de layout, frameworks CSS, bibliotecas de componentes e padrões visuais (botões, selects, combos, dropdowns, datas, tabelas, modais, badges e temas) utilizados no template Angular do **EAF (Enterprise Application Foundation)** (`Templates/Angular/Eaf.ProjectName.UI/`).

Este mapeamento serve como fonte de verdade para a criação e atualização de skills de design e engenharia frontend em sessões futuras.

---

## 1. Stack Tecnológica de UI

| Camada | Tecnologia / Biblioteca | Versão | Propósito Principal |
|---|---|---|---|
| **Framework Base** | Angular | ^20.x | Arquitetura SPA, Componentes, Signals, Roteamento |
| **Component Library** | PrimeNG | ^17.17.0 | Componentes UI modernos (Table, Dialog, Dropdown, Calendar, AutoComplete, etc.) |
| **Legacy UI / Modals** | ngx-bootstrap | ^12.0.0 | Datepicker (`bsDatepicker`), Modals (`BsModalService`), Dropdowns legados |
| **Select / Combobox** | `@ng-select/ng-select` | ^12.0.0 | Combobox avançado, busca em tempo real, multi-select |
| **CSS Utilities / Grid** | Bootstrap / Bootstrap Icons | 5.3 / ^1.10.5 | Sistema de grid, utilitários de espaçamento, flexbox e ícones |
| **Theme / Layout** | Metronic (Legacy Bundle) | v5/v6 bundled | Estrutura de layout desktop (`vendors.bundle.css`, `scripts.bundle.js`, skins de header/aside) |
| **Icons** | PrimeIcons, FontAwesome, FamFamFam | ^2.0 / ^5.13 | Ícones de UI, bandeiras de localização |
| **Date & Time** | Moment.js / Moment-Timezone | 2.30.1 | Manipulação, formatação e timezone de datas |
| **Alerts & Loading** | SweetAlert2, FreezeUI | 7.33 / custom | Alertas modais, confirmações e bloqueio de tela durante requisições |

---

## 2. Configuração de Estilos Globais (`angular.json`)

A ordem de carregamento dos estilos globais em `angular.json` define a precedência e o reset visual:

1. `node_modules/animate.css/animate.min.css` (Animações CSS)
2. `node_modules/famfamfam-flags/dist/sprite/famfamfam-flags.css` (Bandeiras de idiomas)
3. `node_modules/angular-calendar/css/angular-calendar.css` (Calendário)
4. `node_modules/@ng-select/ng-select/themes/default.theme.css` (Tema padrão do ng-select)
5. `node_modules/primeng/resources/themes/lara-light-blue/theme.css` (Tema PrimeNG Lara Light Blue)
6. `node_modules/primeicons/primeicons.css` (Ícones PrimeNG)
7. `node_modules/sweetalert2/dist/sweetalert2.css` (Alertas)
8. `node_modules/cookieconsent/build/cookieconsent.min.css` (Consentimento de cookies)
9. `src/assets/lib/freezeUI/freeze-ui.min.css` (Loading overlay)
10. `src/assets/lib/primeng/...` (Overrides específicos do PrimeNG: file-upload, autocomplete, tree, context-menu)
11. `src/assets/common/fonts/fonts-eaf.css` (Fontes customizadas EAF)
12. `src/assets/lib/ngx-bootstrap/bs-datepicker.css` (DataPicker ngx-bootstrap)
13. `src/assets/lib/metronic/assets/vendors/base/vendors.bundle.css` (Tema Metronic Core CSS)
14. `src/assets/common/styles/styles.css` (Estilos globais customizados da aplicação EAF)

---

## 3. Mapeamento Detalhado de Componentes UI

### 3.1 Botões (`btn` / `p-button`)

- **Classes Bootstrap / Metronic**:
  - Primário: `btn btn-primary`
  - Secundário / Cancelar: `btn btn-secondary`, `btn btn-default`
  - Sucesso: `btn btn-success`
  - Perigo / Exclusão: `btn btn-danger`
  - Informação: `btn btn-info`
  - Aviso: `btn btn-warning`
  - Metálico / Neutro: `btn btn-metal`, `btn-light`
  - Ícone / Ação Limpa (Tabelas): `btn btn-sm btn-clean btn-icon btn-icon-md`
- **Diretivas EAF para Botões**:
  - `[buttonBusy]="saving"` e `[busyText]="l('SavingWithThreeDot')"` (gerenciamento automático de estado de loading em botões de formulário).
- **PrimeNG Buttons**: `<p-button label="..." icon="pi pi-check"></p-button>`

### 3.2 Combos, Selects e Dropdowns

- **Select Nativo (Formulários simples)**:
  - `<select class="form-control" [(ngModel)]="..." name="...">`
  - Usado extensivamente em modais de cadastro e filtros de listagem.
- **Busca Avançada / Select Inteligente (`@ng-select/ng-select`)**:
  - `<ng-select [items]="items" bindLabel="name" bindValue="id" [(ngModel)]="selectedId"></ng-select>`
  - Utilizado para comboboxes com grande volume de dados, busca textual e suporte a múltiplos valores.
- **PrimeNG Dropdown / MultiSelect / AutoComplete**:
  - `<p-dropdown [options]="options" [(ngModel)]="selectedValue"></p-dropdown>`
  - `<p-multiSelect [options]="options" [(ngModel)]="selectedValues"></p-multiSelect>`
- **Dropdowns de Ação (ngx-bootstrap)**:
  - `<div class="btn-group dropdown" dropdown>`
  - `<button dropdownToggle class="dropdown-toggle btn btn-sm btn-primary">...</button>`
  - `<ul class="dropdown-menu" *dropdownMenu>...</ul>`

### 3.3 Seletores de Data e Hora (`Date / DateTime Pickers`)

- **ngx-bootstrap Datepicker**:
  - `<input class="form-control" bsDatepicker [bsConfig]="{ containerClass: 'theme-dark-blue' }" [(ngModel)]="selectedDate" />`
  - Suporte a range de datas (`bsDaterangepicker`), formatação customizada e localização via Moment.js.
- **PrimeNG Calendar**:
  - `<p-calendar [(ngModel)]="date" [showTime]="true" dateFormat="dd/mm/yy"></p-calendar>`
  - Usado em relatórios, filtros por período e agendamentos.

### 3.4 Modais e Diálogos (`Modals`)

- **ngx-bootstrap Modals**:
  - Componentes modais herdam de bases ou utilizam `BsModalService` / `BsModalRef`.
  - Estrutura típica em HTML:
    ```html
    <div class="modal-header">
      <h4 class="modal-title"><span>{{ 'Title' | localize }}</span></h4>
      <button type="button" class="close" (click)="close()" [attr.aria-label]="l('Close')">×</button>
    </div>
    <div class="modal-body">...</div>
    <div class="modal-footer">
      <button type="button" class="btn btn-secondary" (click)="close()">{{ 'Cancel' | localize }}</button>
      <button type="button" class="btn btn-primary" [disabled]="saving" (click)="save()">{{ 'Save' | localize }}</button>
    </div>
    ```
- **PrimeNG Dialog**: `<p-dialog [(visible)]="displayModal" [header]="title" [modal]="true" [style]="{width: '50vw'}">`

### 3.5 Tabelas e Grids (`Tables`)

- **PrimeNG Table (`p-table`)**:
  - Suporte nativo a paginação server-side (`[lazy]="true"`), ordenação, filtragem e seleção de linhas.
  - Exemplo padrão:
    ```html
    <p-table #dataTable [value]="records" [lazy]="true" (onLazyLoad)="getRecords($event)" [paginator]="true" [rows]="pagedResultDto.pageSize">
      <ng-template pTemplate="header">
        <tr>
          <th>{{ 'Actions' | localize }}</th>
          <th>{{ 'Name' | localize }}</th>
        </tr>
      </ng-template>
      <ng-template pTemplate="body" let-record="$implicit">
        <tr>
          <td>...</td>
          <td>{{ record.name }}</td>
        </tr>
      </ng-template>
    </p-table>
    ```
- **Tabelas Responsivas Bootstrap**:
  - `<div class="table-responsive"><table class="table table-striped table-bordered">...</table></div>`

### 3.6 Badges, Status e Alertas

- **Badges de Status (`label` / `badge`)**:
  - Classes Metronic / Bootstrap: `<span class="m-badge m-badge--success m-badge--wide">Active</span>` ou `<span class="badge badge-success">Active</span>`.
  - Componente dedicado: `status-badge.component.ts`.
- **Notificações e Alertas**:
  - Toast notifications via `eaf.notify.js` (`abp.notify.success`, `abp.notify.warn`, `abp.notify.error`).
  - Alertas modais via SweetAlert2 (`eaf.sweet-alert.js`).
  - Bloqueio de tela de carregamento (`FreezeUI` / `eaf.freeze-ui.js`).

---

## 4. Temas e Skins de Layout

O EAF suporta múltiplos temas de layout estruturados em `src/app/shared/layout/themes/`:
- **Default Theme** (`default-layout.component.ts`)
- **Theme 2** (`theme2-layout.component.ts`)
- **Theme 3** (`theme3-layout.component.ts`)
- **Theme 4** (`theme4-layout.component.ts`)

### Controle de Aparência
As configurações de tema (`currentTheme.baseSettings`) controlam:
- Header Skin: `dark`, `light`, `color` (mapeados para classes `.header-dark`, `.header-light`, `.header-color`).
- Aside / Menu Skin: `dark`, `light`.
- Subheader style e fixação de menu.

---

## 5. Diretrizes para Futuras Atualizações de Design Skills

Quando uma nova skill de design for criada ou atualizada:
1. **Priorizar PrimeNG + Bootstrap 5**: Evitar introduzir novos componentes baseados em `ngx-bootstrap` ou classes legadas do Metronic (`m-stack`, `m-grid`).
2. **Uso de Signals**: Em componentes novos, preferir Angular Signals e reatividade moderna em vez de variáveis mutáveis tradicionais.
3. **Variáveis CSS Customizadas**: Utilizar variáveis `:root` para cores e espaçamentos, facilitando a implementação de Dark Mode e customização por tenant.
4. **Responsividade Mobile-First**: Garantir que tabelas utilizem `table-responsive` e que comboboxes (`@ng-select` ou PrimeNG) tenham comportamento adaptativo em telas pequenas.
