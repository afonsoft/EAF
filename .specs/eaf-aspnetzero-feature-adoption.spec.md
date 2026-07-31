# EAF — Adoção de Features do ASP.NET Zero

## Resumo
Analisar funcionalidades e experiências do ASP.NET Zero (frontend Angular, backend .NET, mobile MAUI, RAD tooling) e definir quais podem ser replicadas no EAF para aproximá-lo de uma solução enterprise pronta para produção.

## Motivação
- ASP.NET Zero é um produto comercial derivado do ABP, com centenas de features prontas.
- EAF é a base open-source da Afonsoft; adotar features consolidadas do Zero reduz gap de mercado.
- Usuário final espera dashboard, notificações, chat, mobile, multi-tenant billing, aprovações, etc.

## Features do ASP.NET Zero Mapeadas

### UI/UX Angular
- **13+ Theme Options + Dark Mode**: Metronic 8, Bootstrap 5, theming dinâmico.
- **Responsive Layout**: mobile-first, bottom navigation, drawer menus.
- **Dashboard Host/Tenant**: widgets de estatísticas, gráficos e atividades recentes.
- **Tour/Onboarding**: guia interativo para novos usuários.
- **RTL e Localization**: suporte nativo a idiomas da direita para esquerda.

### Funcionalidades de Administração
- **Editions & Feature Management**: planos com features e limites.
- **Subscription & Payment Management**: integração com Stripe/PayPal para SaaS.
- **Tenant Registration**: página pública para novos tenants se cadastrarem.
- **Impersonation**: logar como usuário para suporte.
- **Organization Units**: hierarquia de OUs para permissões.
- **Audit Logs**: filtro avançado, exportação e detalhamento.
- **Dynamic Parameters**: configurações por tenant customizáveis.

### Mobile
- **.NET MAUI App**: app nativo iOS/Android consumindo a API.
- **Push Notifications**: notificações via Firebase/APNs.

### RAD / Developer Experience
- **Power Tools / Suite**: geração de CRUD a partir de entidades.
- **Master-Detail Scaffolding**: geração de páginas complexas.
- **Code Generator**: reduce boilerplate.

## Proposta de Adoção no EAF

### Fase 1 — UI/UX (curto prazo)
1. Adotar Metronic 8 + Bootstrap 5 (ver `eaf-angular-metronic8-bootstrap5-migration.spec.md`).
2. Implementar dark mode e theming tokens (ver `eaf-angular-dark-mode-theming.spec.md`).
3. Criar dashboard inicial com widgets reutilizáveis.
4. Melhorar responsividade mobile (ver `eaf-angular-mobile-responsive-layout.spec.md`).

### Fase 2 — Features Enterprise (médio prazo)
1. **Editions**: expandir entidades de Edition com FeatureValues.
2. **Subscription/Payment**: abstrair `Eaf.Payment` com providers Stripe/PayPal.
3. **Organization Units**: já existe no ABP/EAF? Verificar e documentar.
4. **Audit Log UI**: tela de busca avançada com exportação CSV/PDF.
5. **Tenant Self-Registration**: página `/register-tenant`.

### Fase 3 — Mobile e RAD (longo prazo)
1. Criar template .NET MAUI consumindo a API EAF.
2. Implementar push notifications backend + frontend.
3. Criar ferramenta de scaffolding/code generator para EAF.

## Plano de Migração
1. Validar quais features já existem parcialmente no EAF.
2. Criar specs detalhados por feature priorizada.
3. Implementar incrementalmente, mantendo compatibilidade com templates.
4. Atualizar documentação e AGENTS.md.

## Impacto
- **Alto**: muda significativamente o produto EAF.
- **Alto**: atratividade para novos usuários e projetos.
- **Médio**: crescimento de complexidade e testes.

## Riscos
- Reimplementar features do Zero pode violar licença se copiar código; usar como referência de requisitos/UX.
- Módulos de pagamento exigem compliance e segurança (PCI DSS).
- MAUI requer expertise e manutenção cross-platform.

## Referências
- <https://aspnetzero.com/features> — lista de features.
- <https://aspnetzero.com/angular> — Angular UI.
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/src/app` — funcionalidades atuais.
