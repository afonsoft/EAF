# EAF Angular — Acessibilidade (a11y) e WCAG AA

## Resumo
Aplicar diretrizes de acessibilidade WCAG 2.1 nível AA no template Angular EAF, melhorando navegação por teclado, leitores de tela, contraste de cores e semântica HTML.

## Motivação
- Aplicações enterprise devem ser acessíveis a todos os usuários, incluindo PCDs.
- Melhor acessibilidade melhora SEO, usabilidade mobile e conformidade legal.
- PrimeNG 17 e Angular CDK oferecem recursos de a11y prontos.

## Estado Atual
- Alguns elementos possuem `aria-label` e `aria-labelledby` (ex: header, chat).
- Uso inconsistente de `aria-hidden`, `role` e foco visível.
- Tabelas `p-table` não sempre usam `<caption>`.
- Cores de texto/fundo podem não atingir contraste 4.5:1.
- Navegação por teclado em modais e dropdowns pode perder foco.

## Proposta de Mudanças

### 1. Semântica HTML
- Substituir `<div>` por `<header>`, `<nav>`, `<main>`, `<aside>`, `<footer>` onde apropriado.
- Usar `<button>` para ações, não `<a href="javascript:;">`.
- Tabelas com `<caption>` e `scope` em `<th>`.

### 2. Navegação por Teclado
- Trap focus em modais (`cdkTrapFocus` ou `p-focusTrap`).
- Ordem de tabulação lógica.
- Atalhos de teclado documentados para ações frequentes.

### 3. Leitores de Tela
- Adicionar `aria-live` para notificações e status de carregamento.
- Garantir `aria-expanded` em menus e dropdowns.
- Usar `aria-describedby` para mensagens de erro em formulários.

### 4. Contraste e Cores
- Verificar contraste de todas as combinações de cores do tema.
- Não depender apenas de cor para transmitir informação (ícones + texto).
- Garantir foco visível com outline ou shadow.

### 5. Componentes Específicos
- `chat-bar`: adicionar `role="complementary"`, `aria-label`, foco no input ao abrir.
- `topbar`: menu dropdown com `role="menu"` e `aria-haspopup`.
- `side-bar-menu`: árvore de menu com `role="tree"` e `aria-expanded`.
- `p-table`: ordenação e paginação acessíveis.

## Plano de Migração
1. Executar audit com Lighthouse e axe DevTools.
2. Corrigir HTML semântico e roles.
3. Implementar focus management em modais e drawers.
4. Revisar contraste e cores.
5. Adicionar testes a11y automatizados (axe-core em Cypress/Playwright).

## Impacto
- **Médio**: mudanças de markup e testes.
- **Alto**: inclusão e conformidade.
- **Baixo**: APIs não afetadas.

## Riscos
- Componentes customizados podem exigir ajustes manuais.
- Leitores de tela variam; testes manuais são necessários.

## Referências
- <https://www.w3.org/WAI/WCAG21/quickref/>
- <https://angular.io/guide/accessibility>
- `/home/ubuntu/repos/EAF/Templates/Angular/Eaf.ProjectName.UI/src/app/shared/layout`.
