# Wiki do EAF (Enterprise Application Foundation)

Bem-vindo à wiki do EAF. Esta página serve como portal para toda a documentação do projeto.

## Navegação Rápida

* [Documentação Principal](./README.md) — visão geral e índice central.
* [Módulos](./modules/README.md) — detalhamento de cada módulo de middleware.
* [Guia de Uso dos Módulos](./modules/USAGE.md) — exemplos práticos de como usar cada módulo.
* [Sistema de Módulos](./MODULE_SYSTEM.md) — como criar e usar módulos no ASP.NET Boilerplate.
* [Templates](./templates/README.md) — templates para novos projetos.
* [Desenvolvimento](./development/README.md) — guias, boas práticas e troubleshooting.
* [Backport GameHub — Guia de Uso](./development/gamehub-backport-usage.md) — novas funcionalidades do backport GameHub para EAF.
* [Otimizações de Performance e Memória](../docs/performance-memory-optimizations.md) — melhorias de performance e memória da versão 9.4.1.
* [Migração EAF 9.3.1 → 9.4.1](../.specs/eaf-template-migration-9.4.1.md) — spec para atualizar projetos que usam os templates API e Angular.
* [Implantação](./deployment/README.md) — configuração de ambiente, migrações e produção.

## Módulos de Middleware

| Módulo | Documentação | Uso |
|--------|--------------|-----|
| Eaf.Castle.Serilog | [modules/eaf-castle-serilog.md](./modules/eaf-castle-serilog.md) | [USAGE.md#eafcastleserilog](./modules/USAGE.md#eafcastleserilog) |
| Eaf.KeyVault | [modules/eaf-keyvault.md](./modules/eaf-keyvault.md) | [USAGE.md#eafkeyvault](./modules/USAGE.md#eafkeyvault) |
| Eaf.KeyVault.AspNetCore | [modules/eaf-keyvault-aspnetcore.md](./modules/eaf-keyvault-aspnetcore.md) | [USAGE.md#eafkeyvaultaspnetcore](./modules/USAGE.md#eafkeyvaultaspnetcore) |
| Eaf.Log4NetServiceBus | [modules/eaf-log4netservicebus.md](./modules/eaf-log4netservicebus.md) | [USAGE.md#eaflog4netservicebus](./modules/USAGE.md#eaflog4netservicebus) |
| Eaf.Middleware.Application | [modules/eaf-middleware-application.md](./modules/eaf-middleware-application.md) | [USAGE.md#eafmiddlewareapplication](./modules/USAGE.md#eafmiddlewareapplication) |
| Eaf.Middleware.AzureActiveDirectory | [modules/eaf-middleware-aad.md](./modules/eaf-middleware-aad.md) | [USAGE.md#eafmiddlewareazureactivedirectory](./modules/USAGE.md#eafmiddlewareazureactivedirectory) |
| Eaf.Middleware.Core | [modules/eaf-middleware-core.md](./modules/eaf-middleware-core.md) | [USAGE.md#eafmiddlewarecore](./modules/USAGE.md#eafmiddlewarecore) |
| Eaf.Middleware.Ldap | [modules/eaf-middleware-ldap.md](./modules/eaf-middleware-ldap.md) | [USAGE.md#eafmiddlewareldap](./modules/USAGE.md#eafmiddlewareldap) |
| Eaf.Middleware.Web.Core | [modules/eaf-middleware-web-core.md](./modules/eaf-middleware-web-core.md) | [USAGE.md#eafmiddlewarewebcore](./modules/USAGE.md#eafmiddlewarewebcore) |
| Eaf.Middleware.Worker | [modules/eaf-middleware-worker.md](./modules/eaf-middleware-worker.md) | [USAGE.md#eafmiddlewareworker](./modules/USAGE.md#eafmiddlewareworker) |
| Eaf.OpenTelemetry | [modules/eaf-opentelemetry.md](./modules/eaf-opentelemetry.md) | [USAGE.md#eafopentelemetry](./modules/USAGE.md#eafopentelemetry) |
| Eaf.SqlServerCache | [modules/eaf-sqlservercache.md](./modules/eaf-sqlservercache.md) | [USAGE.md#eafsqlservercache](./modules/USAGE.md#eafsqlservercache) |
| Eaf.SqliteCache | [modules/eaf-sqlitecache.md](./modules/eaf-sqlitecache.md) | [USAGE.md#eafsqlitecache](./modules/USAGE.md#eafsqlitecache) |

## Contribuindo

Para sugerir melhorias na documentação ou na wiki, abra uma issue ou pull request no repositório principal.
