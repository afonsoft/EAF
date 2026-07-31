# Documentação do Sistema EAF

Bem-vindo à documentação oficial do Sistema EAF. Este documento serve como um guia central para entender a arquitetura, módulos, desenvolvimento, API e implantação do sistema.

O EAF é uma plataforma de middleware open source construída sobre o [ASP.NET Boilerplate](https://aspnetboilerplate.com/), um framework robusto para a criação de aplicações web modernas. Esta documentação cobre tanto as funcionalidades herdadas do ASP.NET Boilerplate quanto as customizações e extensões específicas do EAF.

## Navegação

Para facilitar a consulta, a documentação está organizada nas seguintes seções principais:

*   **[Arquitetura](./architecture/README.md)**: Visão geral da arquitetura do sistema, incluindo a base do ASP.NET Boilerplate, extensões do EAF, Domain-Driven Design (DDD) e Injeção de Dependência.
*   **[Módulos](./modules/README.md)**: Detalhamento dos módulos de middleware do EAF (14 módulos) e referência aos módulos base do ASP.NET Boilerplate. Inclui o [Guia de Uso dos Módulos EAF](./modules/USAGE.md) com exemplos práticos para cada módulo de middleware.
*   **[Sistema de Módulos](./MODULE_SYSTEM.md)**: Documentação completa do sistema de módulos do ASP.NET Boilerplate, incluindo como usar módulos e criar novos módulos.
*   **[Templates](./templates/README.md)**: Documentação dos templates fornecidos pelo EAF para iniciar novos projetos.
    *   **[Angular UI Template](../../Templates/Angular/Eaf.ProjectName.UI/README.md)**: Documentação completa do template Angular UI com guias de instalação, módulos, componentes e implementações.
    *   **[ASP.NET Core API Template](../../Templates/Api/README.md)**: Documentação completa do template API com guias de instalação, módulos e implementações.
    *   **[Worker Template](../../Templates/Worker/README.md)**: Documentação completa do template Worker para serviços de background com guias de instalação, módulos e implementações.
*   **[Desenvolvimento](./development/README.md)**: Informações para desenvolvedores, incluindo como começar, melhores práticas, configurações avançadas, otimização de performance e troubleshooting.
*   **[Migração](./migration/README.md)**: Guias de migração entre versões do EAF e frameworks. Inclui o [spec de migração 9.3.1 → 9.4.1](../.specs/eaf-template-migration-9.4.1.md) para projetos baseados nos templates API e Angular.
*   **[Otimizações de Performance e Memória](../docs/performance-memory-optimizations.md)**: Detalhamento das otimizações de performance e memória introduzidas na versão 9.4.1.
*   **[API](./api/README.md)**: Documentação da API do sistema, cobrindo Controllers, Application Services, Domain Services, Repositórios e Entidades.
*   **[Implantação](./deployment/README.md)**: Guias sobre configuração de ambiente, migrações de banco de dados e considerações para produção.
*   **[Multi-Tenancy Avançado](./eaf-multi-tenant-login.md)**: Fluxo de login em duas etapas para usuários host, `TenantUserManager`, *shadow users* e replicação de roles/permissões. Veja também [TenantUserManager e Shadow Users](./eaf-tenant-user-manager.md) e [Testes Reais](./eaf-multi-tenant-login-real-tests.md).
*   **[Integração com Consumidores Realtime e Sociais](./integration/gamehub-consumer-contracts.md)**: Contratos versionados para chat contextual, notificações, amizade/block/mute, rate limit, auditoria de moderação e SignalR.

## Changelog

*   **[Changelog v2.0](./development/CHANGELOG-v2.md)**: Melhorias de performance backend, suporte multi-database, otimizações Angular e refatoração SOLID (21 especificações implementadas — PR #61).
*   **[CHANGELOG.md](../../CHANGELOG.md)**: Histórico completo de mudanças, incluindo a versão 9.4.2.

## Contribuindo

Se você deseja contribuir para a documentação, por favor, siga as diretrizes de contribuição do projeto (se houver) e submeta suas alterações através de Pull Requests.

## Recursos Adicionais

*   [Documentação oficial ASP.NET Boilerplate](https://aspnetboilerplate.com/Pages/Documents)
*   [Repositório ASP.NET Boilerplate no GitHub](https://github.com/aspnetboilerplate/aspnetboilerplate)
*   [Repositório do Sistema EAF](https://github.com/afonsoft/EAF) (link para o repositório principal, se diferente)

---

*Esta documentação está em constante desenvolvimento. Última atualização: 2026-07-28.*