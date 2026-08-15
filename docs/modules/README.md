# Módulos do EAF (Enterprise Application Foundation)

Esta seção detalha os módulos de middleware do EAF. O EAF é uma plataforma de middleware open source construída sobre o ASP.NET Boilerplate, fornecendo módulos reutilizáveis para aplicações empresariais, além de contratos compartilhados para integração com consumidores realtime e sociais.

## Módulos de Middleware

Para exemplos práticos de uso de cada módulo, consulte o [Guia de Uso dos Módulos EAF](./USAGE.md).

* [Eaf.Castle.Serilog](./eaf-castle-serilog.md) - Integração do Serilog com Castle Windsor para logging estruturado
* [Eaf.KeyVault](./eaf-keyvault.md) - Integração com Azure Key Vault para gerenciamento de segredos
* [Eaf.KeyVault.AspNetCore](./eaf-keyvault-aspnetcore.md) - Integração do Azure Key Vault com ASP.NET Core
* [Eaf.Log4NetServiceBus](./eaf-log4netservicebus.md) - Appender Log4Net para enviar logs ao Azure Service Bus
* [Eaf.Middleware.Application](./eaf-middleware-application.md) - Camada de aplicação do middleware EAF
* [Eaf.Middleware.AzureActiveDirectory](./eaf-middleware-aad.md) - Integração com Azure Active Directory para autenticação
* [Eaf.Middleware.Core](./eaf-middleware-core.md) - Módulo core do middleware EAF
* [Eaf.Middleware.Ldap](./eaf-middleware-ldap.md) - Integração com LDAP/Active Directory para autenticação
* [Eaf.Middleware.Web.Core](./eaf-middleware-web-core.md) - Módulo web core do middleware EAF
* [Eaf.Middleware.Worker](./eaf-middleware-worker.md) - Módulo worker para background jobs
* [Eaf.OpenTelemetry](./eaf-opentelemetry.md) - Integração com OpenTelemetry para observabilidade
* [Eaf.SqlServerCache](./eaf-sqlservercache.md) - Cache distribuído usando SQL Server
* [Eaf.SqliteCache](./eaf-sqlitecache.md) - Cache distribuído usando SQLite
* [Eaf.BlobStoring](./USAGE.md#eafblobstoring) - Armazenamento de BLOBs com providers FileSystem, Azure Blob Storage e AWS S3
* [Eaf.FluentValidation](./USAGE.md#eaffluentvalidation) - Integração do FluentValidation ao pipeline de validação do ABP
* [Eaf.HtmlSanitizer](./USAGE.md#eafhtmlsanitizer) - Sanitização de HTML removendo scripts e URIs inseguras
* [Eaf.MailKit](./USAGE.md#eafmailkit) - Envio de e-mails baseado em MailKit com retry e templates
* [Eaf.RedisCache](./USAGE.md#eafrediscache) - Cache distribuído usando Redis
* [Eaf.SignalR](./USAGE.md#eafsignalr) - Comunicação em tempo real com SignalR
* [Eaf.Webhooks](./USAGE.md#eafwebhooks) - Envio de webhooks HTTP com assinatura HMAC
* [Contratos para consumidores realtime e sociais](../integration/gamehub-consumer-contracts.md) - Contratos versionados para chat contextual, notificações, social, rate limit, auditoria de moderação e SignalR

## Módulos do ASP.NET Boilerplate (Base)

O EAF é construído sobre o ASP.NET Boilerplate. Para documentação detalhada dos módulos base do ABP (Autenticação, Autorização, Auditoria, Multi-tenancy, Background Jobs, Notificações), consulte:

* [Documentação Oficial do ASP.NET Boilerplate](https://aspnetboilerplate.com/Pages/Documents)
* [Módulos do ABP](./abp-modules.md) - Visão geral dos módulos do ABP usados pelo EAF