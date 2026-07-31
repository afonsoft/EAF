# EAF Backend — Modularização e Novos Providers

## Resumo
Modularizar o backend EAF introduzindo módulos opcionais para storage de blobs, cache Redis, envio de emails, realtime e autenticação moderna, reduzindo a necessidade de customizações por projeto.

## Motivação
- Projetos EAF repetem código para upload de arquivos, cache, email e push notifications.
- ASP.NET Boilerplate já oferece módulos equivalentes que podem servir de base.
- Arquitetura de módulos do ABP permite ativar/desativar providers via DI.

## Proposta de Mudanças

### 1. Eaf.BlobStoring
- `IBlobContainer<T>` para armazenamento abstrato.
- Providers:
  - `Eaf.BlobStoring.FileSystem` — desenvolvimento/testes.
  - `Eaf.BlobStoring.Azure` — Azure Blob Storage.
  - `Eaf.BlobStoring.Oci` — Oracle Cloud Object Storage.
- Usos: imagem de perfil, anexos de chat, documentos.

### 2. Eaf.RedisCache
- Provedor de `IDistributedCache` usando StackExchange.Redis.
- Suporte a serialização JSON e fallback para `Eaf.SqlServerCache`.

### 3. Eaf.MailKit
- `IEmailSender` baseado em MailKit/MimeKit.
- Templates de email com Razor ou Scriban.
- Integração com `Eaf.BlobStoring` para anexos.

### 4. Eaf.SignalR
- Módulo dedicado com `HubBase<T>` e integração com `IOnlineClientManager`.
- Hubs: chat, notificações, presença.
- Suporte a backplane Redis para múltiplas instâncias.

### 5. Eaf.OpenIddict
- Módulo de autenticação baseado em OpenIddict como alternativa ao IdentityServer4/JWT manual.
- Suporte a OAuth2/OIDC, clients, scopes, consentimento.

### 6. Eaf.HtmlSanitizer
- Pipeline de sanitização de HTML para chat, email e conteúdo rich-text.
- Integração com `IHtmlSanitizer` do AngleSharp ou HtmlSanitizer.

## Plano de Migração
1. Criar projects `Eaf.*` para cada módulo, seguindo padrão dos existentes.
2. Adicionar interfaces, implementações e testes xUnit.
3. Atualizar `Eaf.Middleware.Web.Core` para registrar providers quando presentes.
4. Documentar configuração em `appsettings.json`.
5. Criar templates de uso nos projetos Api e Angular.

## Impacto
- **Alto**: adiciona novos módulos e dependências.
- **Médio**: aumenta superfície de manutenção.
- **Alto**: reduz customizações em novos projetos.

## Riscos
- Novas dependências NuGet podem conflitar com versões atuais.
- Multi-tenancy exige isolamento de cache, blob e conexões.
- Testes de integração exigem infraestrutura (Redis, container de email).

## Referências
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.BlobStoring*`
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.RedisCache*`
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.MailKit`
- `/home/ubuntu/repos/abp-aspnetboilerplate/src/Abp.AspNetCore.SignalR`
- `/home/ubuntu/repos/EAF/src` — estrutura atual de módulos EAF.
