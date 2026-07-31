# EAF Middleware — Paridade de Features com ASP.NET Boilerplate (ABP)

## Resumo
Comparar os módulos do EAF com os do ASP.NET Boilerplate (branch `dev`) e identificar módulos/stratégias que podem ser incorporados ao EAF para ampliar suporte a storage, cache, ORM, validação, jobs e notificações.

## Motivação
- EAF é baseado no ASP.NET Boilerplate, mas possui menos módulos opcionais.
- ABP oferece integrações prontas que reduzem trabalho manual em novos projetos enterprise.
- Manter paridade relevante facilita migrações e reduz customizações.

## Comparativo de Módulos

| Módulo ABP | Módulo EAF equivalente | Status | Ação proposta |
|---|---|---|---|
| `Abp.BlobStoring` / `Azure` / `FileSystem` | Não encontrado | **Ausente** | Adicionar `Eaf.BlobStoring.*` para upload de arquivos, imagens de perfil, anexos de chat |
| `Abp.HtmlSanitizer` | Não encontrado | **Ausente** | Criar `Eaf.HtmlSanitizer` integrado ao chat, emails e conteúdo rico |
| `Abp.MailKit` | Não encontrado (usa `System.Net.Mail`?) | **Ausente/Parcial** | `Eaf.MailKit` para emails com templates e anexos |
| `Abp.FluentValidation` | Não encontrado | **Ausente** | Integrar FluentValidation em DTOs e Application Services |
| `Abp.Dapper` | Não encontrado | **Ausente** | `Eaf.Dapper` para queries complexas e relatórios |
| `Abp.RedisCache` / `ProtoBuf` | `Eaf.SqlServerCache`, `Eaf.SqliteCache` | **Parcial** | Adicionar provedor Redis para cache distribuído |
| `Abp.MongoDB` | Não encontrado | **Ausente** | `Eaf.MongoDB` como alternativa ao EF Core para alguns cenários |
| `Abp.NHibernate` | Não encontrado | **Ausente** | Avaliar se faz sentido no EAF (.NET 10 foca EF Core) |
| `Abp.HangFire` / `Quartz` | `Eaf.Middleware.Worker` | **Parcial** | Adicionar `Eaf.Hangfire` e `Eaf.Quartz` como opções ao worker |
| `Abp.MemoryDb` | Não encontrado | **Ausente** | `Eaf.MemoryDb` para testes e prototipagem |
| `Abp.FluentMigrator` | Não encontrado | **Ausente** | Alternativa ao `dotnet ef migrations` em ambientes legados |
| `Abp.AspNetCore.SignalR` | `Eaf.Middleware.Web.Core` usa SignalR? | **Parcial** | Módulo dedicado `Eaf.SignalR` com hubs e integração |
| `Abp.AspNetCore.OData` | Não encontrado | **Ausente** | `Eaf.OData` para endpoints OData |
| `Abp.AspNetCore.OpenIddict` | `Eaf.Middleware.AzureActiveDirectory`, `Ldap` | **Parcial** | Modernizar autenticação com OpenIddict nativo |

## Proposta por Prioridade

### Alta Prioridade
1. **Eaf.BlobStoring.FileSystem + Azure + OCI** — upload de arquivos é necessário no chat e perfil.
2. **Eaf.RedisCache** — cache distribuído em multi-tenant e containers.
3. **Eaf.MailKit** — envio de emails transacionais e campanhas.
4. **Eaf.HtmlSanitizer** — segurança no chat e conteúdo dinâmico.

### Média Prioridade
5. **Eaf.Dapper** — relatórios e queries de performance.
6. **Eaf.FluentValidation** — validações mais expressivas.
7. **Eaf.SignalR** — módulo isolado para realtime.
8. **Eaf.Quartz** — agendamento alternativo ao Hangfire.

### Baixa Prioridade / Avaliar
9. **Eaf.MongoDB** — se houver necessidade de NoSQL.
10. **Eaf.OData** — se APIs precisarem suporte OData.
11. **Eaf.FluentMigrator** — se houver legado sem EF Core.

## Plano de Migração
1. Criar issues/features para cada módulo prioritário.
2. Copiar/adaptar a estrutura dos módulos ABP (`Abp.*`) para `Eaf.*`.
3. Adicionar testes xUnit para cada módulo novo.
4. Atualizar `Eaf.sln`, `common.props` e templates.
5. Documentar uso no README e AGENTS.md.

## Impacto
- **Alto**: expande capacidade do EAF.
- **Médio**: aumenta superfície de testes e manutenção.
- **Médio**: requer decisões arquiteturais sobre multi-tenancy e cache.

## Riscos
- Sincronizar com upstream ABP pode ser trabalhoso se APIs mudarem.
- Cada módulo precisa de providers específicos (Azure, OCI, Redis, Mongo).
- Aumento de complexidade e tempo de build.

## Referências
- `/home/ubuntu/repos/abp-aspnetboilerplate/src` — lista de módulos ABP.
- `/home/ubuntu/repos/EAF/src` — lista de módulos EAF.
- <https://aspnetboilerplate.com/Pages/Documents/Module-System>.
