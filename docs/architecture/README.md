# Arquitetura do Sistema EAF

Esta seção detalha a arquitetura do Sistema EAF. Compreender a arquitetura é fundamental para desenvolver, manter e estender o sistema de forma eficaz.

Cobrimos os seguintes tópicos:

*   **[Visão Geral (Overview)](./overview.md)**: Uma introdução de alto nível à arquitetura do EAF, seus principais componentes e como eles interagem.
*   **[Base ASP.NET Boilerplate](./aspnet-boilerplate-base.md)**: Detalhes sobre como o EAF utiliza o ASP.NET Boilerplate como seu framework fundamental, incluindo os módulos e conceitos principais que são aproveitados.
*   **[Extensões EAF](./eaf-extensions.md)**: Documentação das customizações, extensões e funcionalidades específicas que foram construídas sobre ou ao lado do ASP.NET Boilerplate para atender aos requisitos do EAF.
*   **[Domain-Driven Design (DDD)](./domain-driven-design.md)**: Explicação de como os princípios do Domain-Driven Design são aplicados no EAF, incluindo a estrutura de camadas (Domínio, Aplicação, Infraestrutura, Apresentação), Entidades, Agregados, Serviços de Domínio e Repositórios.
*   **[Injeção de Dependência](./dependency-injection.md)**: Como a Injeção de Dependência (DI) é utilizada em todo o sistema, as principais bibliotecas de DI e padrões de registro.

Navegue pelos links acima para explorar cada aspecto da arquitetura do EAF.
*   **[AD-0001 — ASP.NET Zero v15.5 Parity](./AD-0001-zero-parity-modules.md)**: Decisões dos módulos de paridade (API keys, passkeys, sessions, OpenIddict, workflow engine, Angular 21 + PrimeNG 21).
*   **[Diagrama: extensões de auth & workflow](./eaf-auth-workflow-extensions.mmd)**: Mermaid — fluxo entre UI, endpoints de auth, OpenIddict e engine de workflow.

## Índice de ADRs

| AD | Título | Status |
|----|--------|--------|
| AD-0001 | ASP.NET Zero v15.5 parity — new auth & workflow modules | Accepted |
