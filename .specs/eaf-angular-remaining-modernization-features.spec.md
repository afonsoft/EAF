# EAF Angular - Remaining Modernization Features

## Goal

Consolidar as features de modernização do template Angular EAF que ainda não foram implementadas, organizadas do mais simples ao mais complexo, com análise de viabilidade e escopo.

## Features restantes

### 1. Dark Mode e Sistema de Temas (Feature 4)

**Objetivo:** Adicionar um tema escuro e um sistema de design tokens no Angular, substituindo as cores fixas do Metronic por CSS variables.

**Escopo atual proposto:**
- Criar `theme-variables.scss`/`styles.css` com tokens para cores de superfície, texto, borda, primária, status e destaque.
- Estender `Header/Theme/UiCustomization` DTOs para expor `isDarkMode` e `themeName`.
- Adicionar toggle no header para alternar entre `light` e `dark`.
- Persistir preferência via `localStorage` e `UserPreferences` (backend opcional).
- Ajustar `chat-bar.component.css` e todos os layouts para usar `var(--eaf-surface)` etc.

**Faz sentido?** Sim. É uma demanda comum e melhora acessibilidade. Complexidade média.

---

### 2. Modernização de Componentes PrimeNG (Feature 5)

**Objetivo:** Substituir componentes legados do `ngx-bootstrap` e do Metronic por componentes nativos do PrimeNG 17 (p-table, p-menu, p-dropdown, p-dialog, p-toast, p-inputswitch).

**Escopo atual proposto:**
- Substituir `BsDropdownModule` dos menus por `p-menu`/`p-tieredmenu`.
- Substituir modais `ngx-bootstrap` por `p-dialog`.
- Substituir tabelas manuais por `p-table` com `responsiveLayout="scroll"`.
- Consolidar `p-paginator` e `p-confirmDialog`.
- Adicionar `p-toast` para notificações e remover `notify` customizado.

**Faz sentido?** Sim. O `package.json` já lista `primeng ^17.17.0`. Complexidade média-alta por causa dos estilos do Metronic.

---

### 3. Migração Metronic 8 + Bootstrap 5 (Feature 6)

**Objetivo:** Atualizar o layout do template Angular para Metronic 8 com Bootstrap 5, abandonando classes legadas (`m-grid`, `m-stack`, `m-portlet`) em favor de `row`, `col`, `card`, `navbar`, `offcanvas`.

**Escopo atual proposto:**
- Substituir `style.bundle.css` do Metronic 5/7 por assets do Metronic 8 ou por um design system próprio.
- Refatorar `default-layout`, `theme2-layout`, `theme3-layout`, `theme4-layout` para estrutura Bootstrap 5.
- Criar componentes reutilizáveis: `app-card`, `app-page-header`, `app-offcanvas-menu`.
- Garantir responsividade mobile com `offcanvas` nativo e breakpoints Bootstrap.

**Faz sentido?** Apenas se houver licença do Metronic 8. Sem licença, a recomendação é construir um design system próprio incrementalmente. Complexidade alta.

---

### 4. Backend Modularization (Feature 7)

**Objetivo:** Criar os módulos backend ausentes no EAF e padronizar os já existentes.

**Escopo atual proposto:**
- `Eaf.BlobStoring` — abstração para armazenamento de arquivos (Azure Blob, S3, local).
- `Eaf.HtmlSanitizer` — sanitização de HTML para chat/notificações.
- `Eaf.OpenIddict` — provedor OpenID Connect (já existe `ExternalLoginProviderInfo`, mas sem implementação).
- `Eaf.Dapper` — repositórios Dapper para consultas complexas.
- `Eaf.FluentValidation` — validação fluente nos Application Services.
- Padronizar `MailKit` e `Redis` já existentes, extraindo para módulos bem definidos.

**Faz sentido?** `BlobStoring`, `HtmlSanitizer` e `OpenIddict` têm alto valor. `Dapper` e `FluentValidation` dependem de demandas reais. Complexidade alta.

---

### 5. ABP Feature Parity (Feature 9)

**Objetivo:** Aproximar o EAF das funcionalidades modernas do ABP Framework.

**Escopo atual proposto:**
- `Eaf.BlobStoring` (também listado na modularização).
- Suporte a MongoDB (`Eaf.Middleware.MongoDB`)
- Background jobs com Quartz (`Eaf.Quartz`)
- OData controllers para entidades administrativas
- Feature system aprimorado (Editions/Feature values)
- OpenIddict/OAuth2 servidor

**Faz sentido?** MongoDB e Quartz fazem sentido para projetos grandes. OData e OpenIddict dependem de roadmap. Complexidade muito alta.

## Prioridade recomendada

1. Dark Mode e Design Tokens
2. PrimeNG Modernization
3. Metronic 8 + Bootstrap 5 (ou design system próprio)
4. Backend Modularization (`BlobStoring`, `HtmlSanitizer`, `OpenIddict`)
5. ABP Feature Parity

## Critérios de aceite gerais

- Cada feature deve ter sua própria spec detalhada antes de implementação.
- Build do Angular (`ng build --configuration=production`) sem erros.
- Build do .NET (`dotnet build Eaf.sln`) sem erros.
- Testes unitários/xUnit passando.
- Cobertura mínima 90% para código backend novo.

## Notas

- Features 1, 2, 3 e 8 já implementadas. Esta spec documenta o trabalho restante.
- Recomenda-se aprovar e implementar uma feature por vez, validando CI antes de avançar.
