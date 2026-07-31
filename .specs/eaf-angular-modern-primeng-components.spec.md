# EAF Angular — Modernização de Componentes com PrimeNG

## Resumo
Padronizar o template Angular EAF em componentes **PrimeNG 17+**, reduzindo o uso de `ngx-bootstrap`, jQuery e widgets legados, aproveitando novos recursos como Tailwind-aware theming, unstyled mode, inline messages e acessibilidade aprimorada.

## Motivação
- O `package.json` já inclui `primeng ^17.17.0`, mas muitos componentes ainda usam `ngx-bootstrap` (`^12.0.0`) e estilos customizados.
- PrimeNG 17 introduziu nova API de theming, melhor suporte a acessibilidade e componentes unstyled.
- ASP.NET Zero usa PrimeNG como base para a UI Angular.
- Menor dependência de jQuery/Bootstrap JS facilita manutenção e SSR/PWA.

## Estado Atual
- `primeng` presente: `p-fileUpload`, `p-table` (usado em datatables?), `p-dialog` parcial.
- `ngx-bootstrap` usado para: dropdown, datepicker, modal, tooltip, tabs, accordion.
- Formulários: mistura de HTML puro + classes Bootstrap + `m-form` Metronic.
- Datepicker: `ngx-bootstrap` e `bs-datepicker` presentes em assets.
- Modais: Bootstrap modal + `m-modal` + `p-dialog`.

## Proposta de Mudanças

### 1. Substituir ngx-bootstrap por PrimeNG
| ngx-bootstrap | PrimeNG 17+ |
|---|---|
| Dropdown | `p-dropdown` / `p-splitButton` |
| Datepicker | `p-calendar` |
| Modal | `p-dialog` |
| Tooltip | `p-tooltip` |
| Tabs | `p-tabView` |
| Accordion | `p-accordion` |
| Typeahead | `p-autoComplete` |

### 2. Padronizar formulários
- Usar `p-inputText`, `p-inputNumber`, `p-inputTextarea`, `p-checkbox`, `p-radioButton`, `p-toggleButton`.
- Integrar com `ReactiveFormsModule` e validações do Angular.
- Substituir `m-form` / `form-group form-md-line-input` por `p-fluid` + grid responsiva.

### 3. Tabelas
- Substituir `primeng-datatable-container` customizado por `p-table` com `responsiveLayout="stack"` ou `scroll`.
- Adicionar `p-paginator` nativo e lazy loading.
- Usar `p-columnFilter` para filtros inline.

### 4. Temas PrimeNG
- Migrar de `theme.css` legado para novo sistema de theming do PrimeNG 17 (`Aura`, `Lara`, `Material`, `Bootstrap` ou tema customizado EAF).
- Criar `theme-eaf` com tokens de design (cores, radius, spacing, tipografia).

### 5. Acessibilidade
- Usar atributos `aria-*` e roles fornecidos pelos componentes PrimeNG.
- Garantir foco visível e navegação por teclado.

## Plano de Migração
1. Inventariar todos os usos de `ngx-bootstrap` no `src/app`.
2. Criar exemplos de substituição em componentes admin críticos (Users, Roles, Tenants).
3. Migrar componentes genéricos (dropdowns, datepickers, modais).
4. Atualizar CSS para novo theming PrimeNG.
5. Testes de regressão visual e funcional.

## Impacto
- **Médio/Alto**: muitos componentes de UI precisam ser reescritos.
- **Alto**: melhora de acessibilidade e consistência.
- **Baixo/Médio**: pode reduzir tamanho de bundle ao remover `ngx-bootstrap`.

## Riscos
- Mudanças de API entre `ngx-bootstrap` e PrimeNG exigem testes.
- Projetos gerados pelo template EAF podem depender de estilos `m-form`.
- Necessidade de manter compatibilidade com localize pipe e serviços existentes.

## Referências
- <https://primeng.org/installation> — PrimeNG 17 theming e unstyled mode.
- ASP.NET Zero Angular UI usa PrimeNG como base.
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/package.json`.
