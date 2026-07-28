# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/lang/pt-BR/).

## [Unreleased]

### Added

*   feat(cors): `AddEafCors` centralizado em `Eaf.Middleware.Web.Core` com reflexão de origem real, suporte a wildcards de subdomínio e todos os headers enviados pelo `EafHttpInterceptor`
*   feat(error): Middleware e filtro de public errors (`EafPublicErrorMiddleware`, `EafExceptionFilter`) mapeando `UserFriendlyException` e outras exceções para `PublicErrorContract` com status apropriado
*   feat(auth): Parsing JWT no frontend via `TokenService.getPayload`, `getUserId`, `getTenantId`, `getUserName`, `getRoles` e `isInRole`; claim `tenantid` adicionada ao token no backend
*   feat(signalr): Modernização do SignalR para `@microsoft/signalr` com `HubConnectionBuilder`, `accessTokenFactory` e `withAutomaticReconnect`; suporte a `access_token` na query string do JWT
*   feat(login): Fallback de login para usuários host sem tenants vinculados no two-step login e botão "Login como Host" no `SelectTenantComponent`
*   feat(ui): Componentes reutilizáveis `app-status-badge` e `app-empty-state` e melhorias de responsividade mobile (`100dvh`, touch targets, drawers)
*   feat(multi-tenancy): Fluxo de login em duas etapas para usuários host, com `UserTenantMembership`, `TenantUserManager`, *shadow users* e replicação automática de roles/permissões; endpoints `GetAvailableTenants` e `SelectTenant` e componente Angular `SelectTenantComponent` (PR #250)
*   feat: Campos contextuais em `ChatMessage` (`ConversationId`, `GameId`, `MatchId`, `ContextType`) e contratos compartilhados para consumidores realtime e sociais (chat, notificações, social, rate limit, auditoria de moderação) com abstrações `IRateLimitManager` e `IModerationAuditWriter` (PRs #247 e #249)
*   docs: Guias de login multi-tenant, `TenantUserManager`/shadow users, testes reais e integração com consumidores realtime e sociais (`docs/eaf-multi-tenant-login.md`, `docs/eaf-tenant-user-manager.md`, `docs/eaf-multi-tenant-login-real-tests.md`, `docs/integration/gamehub-consumer-contracts.md`) (PRs #249, #250 e #251)
*   test: Testes BDD multi-tenant para `TokenAuthController` (`TokenAuthControllerMultiTenantBddTests`) e plano de testes reais com Docker Compose (PRs #250 e #251)
*   test: Implementação massiva de testes BDD em português (Dado/Quando/Então)
    - 2384 testes totais (era 1289), 100% passando
    - Cobertura de DTOs, Models, Entities, Domain Logic
    - Testes para Authorization, Chat, Friendships, MultiTenancy, Editions, Notifications
    - Testes para Configuration, Sessions, WebHooks, External Auth Providers
    - Testes para Security, Profile, Localization, Timing, UI Customization
*   docs: Documentação XML `/// <summary>` adicionada a 48 tipos públicos sem documentação
*   feat: add repo summary to .openhands/microagents/repo.md

### Fixed

*   fix(multi-tenancy): Geração de senha Identity-compliant para *shadow users* e execução de testes reais de login multi-tenant (PR #251)
*   fix: Resolver todos os 70 build warnings da solução (70 → 0)
    - fix(NU5118): Corrigir README duplicado em nupkg
    - fix(CA1416): Adicionar guards `[SupportedOSPlatform]` para LDAP
    - fix(CS8600/CS8602/CS8604): Corrigir nullable reference warnings
    - fix(CA2254): Corrigir interpolação de strings em mensagens de log
    - fix(NU1504): Remover PackageReference duplicados
    - fix(CS0169): Remover campos não utilizados
    - fix(CS1587): Corrigir comentários XML posicionados incorretamente
    - fix(CS1572/CS1573): Corrigir nomes de parâmetros em documentação XML
*   fix: Habilitar Eaf.MiddlewareCore.Tests em build Release (não era compilado)
*   fix(test): Corrigir teste PathNavigatesAboveRoot para compatibilidade Linux
*   Fix issue #478: Build All and Test workflow failing due to Coverlet error
*   Fix issue #475: Corrigir testes unitários com falhas e atualizar README
*   Fix: Resolve Coverlet path errors in CI workflow
*   Fix: Address some SqliteCache test failures

### Changed

*   chore: Bump dos templates e módulos para a versão 9.3.1 (PRs #248 e #249)
*   docs: README e documentação atualizados com as novas implementações de login multi-tenant, contratos realtime e sociais, rate limit e moderação
*   docs: Atualizar README badges com métricas atuais (Line 36.6%, Tests 2384, 0 Warnings)
*   refactor: Atualizar xunit.runner.visualstudio para 3.1.4 em todos os projetos de teste
*   refactor: Padronizar PackageReference em Directory.Build.props
*   Update ABP to Version="10.2.0"
*   Update and rename ci.yml to coverage-reports.yml

### Removed

*   Remove Edition from EAF

## [10.0.0] - 2025-02-14

### BREAKING CHANGES


# [9.0.2](https://github.com/afonsoft/EAF/releases/tag/9.0.2)
> 05/31/2025 02:55:20 UTC
##### ``9.0.2``
Update ABP to 10.2.0
# [9.0.1](https://github.com/afonsoft/EAF/releases/tag/9.0.1)
> 04/16/2025 03:02:27 UTC
##### ``9.0.1``
9.0.1
# [9.0.0](https://github.com/afonsoft/EAF/releases/tag/9.0.0)
> 03/05/2025 19:25:01 UTC
##### ``9.0.0``
Removendo o EAF old e usando o ABP como base para o Middle.
# [6.1.9](https://github.com/afonsoft/EAF/releases/tag/6.1.9)
> 01/25/2024 20:15:38 UTC
##### ``6.1.9``
6.1.9
# [6.1.8](https://github.com/afonsoft/EAF/releases/tag/6.1.8)
> 01/11/2024 15:01:57 UTC
##### ``6.1.8``
6.1.8
# [6.1.7](https://github.com/afonsoft/EAF/releases/tag/6.1.7)
> 12/19/2023 13:13:36 UTC
##### ``6.1.7``
Corre&#231;&#245;es do assembly names duplicados e atualiza&#231;&#245;es dos pacotes nugets
# [6.1.6](https://github.com/afonsoft/EAF/releases/tag/6.1.6)
> 12/18/2023 20:13:18 UTC
##### ``6.1.6``
6.1.6
Corre&#231;&#245;es do EF Plus e de refer&#234;ncias do nuget
# [6.1.5](https://github.com/afonsoft/EAF/releases/tag/6.1.5)
> 12/18/2023 13:51:23 UTC
##### ``6.1.5``
6.1.5
Implementa&#231;&#227;o do MySqlServer
Update do Template NET8
Incluido o EafTenantAddress
# [6.1.4](https://github.com/afonsoft/EAF/releases/tag/6.1.4)
> 12/15/2023 12:34:23 UTC
##### ``6.1.4``
6.1.4
# [6.1.3](https://github.com/afonsoft/EAF/releases/tag/6.1.3)
> 11/16/2023 18:05:20 UTC
##### ``6.1.3``
6.1.3

NET7 and Suport a NET8
# [6.1.2](https://github.com/afonsoft/EAF/releases/tag/6.1.2)
> 09/27/2023 18:42:50 UTC
##### ``6.1.2``
6.1.2
# [6.1.1](https://github.com/afonsoft/EAF/releases/tag/6.1.1)
> 09/19/2023 13:23:04 UTC
##### ``6.1.1``
6.1.1
# [6.1.0](https://github.com/afonsoft/EAF/releases/tag/6.1.0)
> 08/08/2023 18:06:27 UTC
##### ``6.1.0``
6.1.0
# [6.0.14](https://github.com/afonsoft/EAF/releases/tag/6.0.14)
> 07/31/2023 14:42:00 UTC
##### ``6.0.14``
6.0.14
# [6.0.13](https://github.com/afonsoft/EAF/releases/tag/6.0.13)
> 07/28/2023 21:27:02 UTC
##### ``6.0.13``
6.0.13
# [6.0.11](https://github.com/afonsoft/EAF/releases/tag/6.0.11)
> 06/19/2023 12:14:04 UTC
##### ``6.0.11``
6.0.11
# [6.0.10](https://github.com/afonsoft/EAF/releases/tag/6.0.10)
> 06/15/2023 13:25:47 UTC
##### ``6.0.10``
6.0.10
# [6.0.9](https://github.com/afonsoft/EAF/releases/tag/6.0.9)
> 05/11/2023 20:51:19 UTC
##### ``6.0.9``
6.0.9
# [6.0.8](https://github.com/afonsoft/EAF/releases/tag/6.0.8)
> 04/20/2023 18:54:22 UTC
##### ``6.0.8``
6.0.8
# [6.0.7](https://github.com/afonsoft/EAF/releases/tag/6.0.7)
> 03/23/2023 19:10:13 UTC
##### ``6.0.7``
6.0.7
# [6.0.6](https://github.com/afonsoft/EAF/releases/tag/6.0.6)
> 03/15/2023 17:37:16 UTC
##### ``6.0.6``
Fix AddStackExchangeRedisCache
# [v6.0.5](https://github.com/afonsoft/EAF/releases/tag/6.0.5)
> 02/08/2023 12:48:47 UTC
##### ``6.0.5``
v6.0.5
# [TAG 6.0.4](https://github.com/afonsoft/EAF/releases/tag/6.0.4)
> 01/31/2023 19:16:18 UTC
##### ``6.0.4``
TAG 6.0.4
# [V6.0.3](https://github.com/afonsoft/EAF/releases/tag/6.0.3)
> 12/14/2022 19:20:21 UTC
##### ``6.0.3``
V6.0.3
# [V6.0.2](https://github.com/afonsoft/EAF/releases/tag/6.0.2.1)
> 11/17/2022 12:19:52 UTC
##### ``6.0.2.1``
V6.0.2
Angular 8
# [V6.0.2](https://github.com/afonsoft/EAF/releases/tag/6.0.2)
> 11/16/2022 20:23:52 UTC
##### ``6.0.2``
V6.0.2
Angular 8
# [V6.0.1](https://github.com/afonsoft/EAF/releases/tag/6.0.1)
> 11/10/2022 15:09:03 UTC
##### ``6.0.1``
V6.0.1
# [v6.0.0](https://github.com/afonsoft/EAF/releases/tag/6.0.0)
> 11/09/2022 16:05:26 UTC
##### ``6.0.0``
v6.0.0
# [First Release 5.0.0](https://github.com/afonsoft/EAF/releases/tag/5.0.0)
> 11/04/2021 16:38:40 UTC
##### ``5.0.0``
First Release 5.0.0
# [release candidate 4](https://github.com/afonsoft/EAF/releases/tag/5.0.0-rc.4)
> 10/22/2021 16:28:26 UTC
##### ``5.0.0-rc.4``
release candidate 4

