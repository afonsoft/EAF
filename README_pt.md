# Enterprise Application Foundation (EAF)

[![GitHub](https://img.shields.io/github/license/afonsoft/eaf)](LICENSE) [![GitHub version](https://badge.fury.io/gh/afonsoft%2Feaf.svg)](https://badge.fury.io/gh/afonsoft%2Feaf) [![Commits History](https://img.shields.io/badge/Commits-History-critical)](https://github.com/afonsoft/EAF/commits/main/) [![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) [![DeepWiki](https://img.shields.io/badge/DeepWiki-afonsoft%2FEAF-blue?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyNCIgaGVpZ2h0PSIyNCIgdmlld0JveD0iMCAwIDI0IDI0IiBmaWxsPSJub25lIiBzdHJva2U9IiNmZmZmZmYiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIiBzdHJva2UtbGluZWpvaW49InJvdW5kIj48cGF0aCBkPSJNNCAxOWguMDEiLz48cGF0aCBkPSJNMjAgMTEuMmMuNy40IDEuMSAxIDEuMSAxLjggMCAuNS0uMyAxLjEtLjcgMS41bC0zLjEgMy4xYy0uNS41LTEgLjctMS42LjdsLS44LS4xLTEuNC0uNS0xLjgtMS4xIi8+PHBhdGggZD0iTTQuMyAxNS4zYy0uNC0uNy0uNS0xLjUtLjMtMi4yLjItLjguNy0xLjQgMS4zLTEuOGwxLjgtMS4yYy43LS40IDEuNS0uNiAyLjItLjQuOC4yIDEuNS43IDEuOSAxLjMiLz48cGF0aCBkPSJNOCA1YzAtLjUuMi0xIC42LTEuNEM5IDMuMiA5LjUgMyAxMCAzaDRjLjUgMCAxIC4yIDEuNC42LjQuNC42LjkuNiAxLjR2M2MwIC41LS4yIDEtLjYgMS40LS40LjQtLjkuNi0xLjQuNmgtNGMtLjUgMC0xLS4yLTEuNC0uNkM4LjIgOSA4IDguNSA4IDgiLz48L3N2Zz4=)](https://deepwiki.com/afonsoft/EAF)

**[English](README.md)** | Português

![Line Coverage](https://img.shields.io/badge/Line%20Coverage-97.9%25-brightgreen)
![Branch Coverage](https://img.shields.io/badge/Branch%20Coverage-90.5%25-brightgreen)
![Method Coverage](https://img.shields.io/badge/Method%20Coverage-99.8%25-brightgreen)
![Test Success Rate](https://img.shields.io/badge/Test%20Success%20Rate-100%25-brightgreen)
![Total Tests](https://img.shields.io/badge/Total%20Tests-4604-blue)
![Passing Tests](https://img.shields.io/badge/Passing%20Tests-4603-brightgreen)
![Build Warnings](https://img.shields.io/badge/Build%20Warnings-0-brightgreen)
![Angular Tests](https://img.shields.io/badge/Angular%20Tests-222%20Passed-brightgreen)
![API Template Tests](https://img.shields.io/badge/API%20Template%20Tests-212%20Total-blue)
![API Template Passing](https://img.shields.io/badge/API%20Template%20Passing-211%20Success-brightgreen)

## Índice

- [Sobre o Projeto](#-sobre-o-projeto)
- [ASP.NET Boilerplate](#-aspnet-boilerplate)
- [Visão Técnica](#-visão-técnica)
- [Instalação e Configuração](#-instalação-e-configuração)
- [Execução e Testes](#-execução-e-testes)
- [Cobertura de Código](#-cobertura-de-código)
- [Pacotes NuGet](#-pacotes-nuget)
- [Contribuição](#-contribuição)


## Sobre o Projeto

### O que é o EAF?

O **EAF (Enterprise Application Foundation)** é uma plataforma de middleware open source que fornece uma base sólida para o desenvolvimento de aplicações modernas. Baseado no ASP.NET Boilerplate (ABP), o EAF foi otimizado para trabalhar com as versões mais recentes do ASP.NET Core e Entity Framework Core.

### Principais Benefícios

- **Segurança Integrada**: Autenticação e autorização com suporte a Azure Active Directory e LDAP
- **Auditoria Completa**: Rastreamento automático de todas as operações do sistema
- **Multi-tenancy**: Suporte nativo para aplicações multi-inquilino
- **Observabilidade**: Integração com OpenTelemetry para monitoramento e telemetria
- **Cache Distribuído**: Suporte para Redis, SQL Server e SQLite
- **Gerenciamento de Segredos**: Integração com Azure Key Vault e Oracle Cloud Infrastructure
- **Logging Avançado**: Substituição do log4net por Serilog para melhor performance

### Casos de Uso

- **Aplicações Web**: Sistemas de gestão, CRMs, ERPs e outras aplicações
- **APIs RESTful**: Desenvolvimento de APIs escaláveis
- **Microserviços**: Base para arquiteturas de microserviços
- **Aplicações Multi-tenant**: SaaS e aplicações compartilhadas

---

## ASP.NET Boilerplate

### O que é ASP.NET Boilerplate?

O **ASP.NET Boilerplate (ABP)** é um framework de aplicação web de código aberto que fornece uma infraestrutura robusta para o desenvolvimento de aplicações modernas. Documentação completa disponível em: [https://aspnetboilerplate.com/Pages/Documents](https://aspnetboilerplate.com/Pages/Documents)

### EAF: Implementação Open Source Aprimorada

O **EAF (Enterprise Application Foundation)** é uma implementação open source baseada no ASP.NET Boilerplate, projetada para oferecer uma interface mais amigável para desenvolvimento de APIs e UIs. O EAF complementa o framework base com diversas melhorias e módulos adicionais:

### Módulos e Melhorias

#### Autenticação e Autorização
- **Login Externo**: Suporte a login social (Google, Facebook, Twitter, Microsoft)
- **Azure Active Directory**: Integração completa
- **LDAP/Active Directory**: Autenticação via diretórios
- **Two-Factor Authentication**: Autenticação de dois fatores
- **Gerenciamento de Permissões**: Sistema granular de permissões e roles

#### Auditoria e Logging
- **Auditoria Automática**: Rastreamento de todas as operações CRUD
- **Logging Estruturado**: Integração com Serilog para logs detalhados
- **Entity Change Tracking**: Monitoramento de alterações em entidades
- **Log de Erros**: Captura e análise de exceções

#### Comunicação em Tempo Real
- **Chat System**: Sistema de chat entre usuários
- **SignalR Integration**: WebSockets para comunicação bidirecional
- **Notificações Push**: Sistema de notificações em tempo real
- **Tenant-to-Host Chat**: Chat entre inquilinos e host
- **Group Chat**: Chat em grupo para colaboração

#### Multi-Tenancy
- **Isolamento de Dados**: Separação completa de dados por tenant
- **Tenant Management**: Gerenciamento de inquilinos
- **Tenant Resolution**: Resolução automática de tenant
- **Feature Management**: Habilitação/desabilitação de features por tenant

#### Cache e Performance
- **Cache Distribuído**: Suporte a Redis, SQL Server, SQLite
- **Cache Abstraction**: Interface unificada para diferentes backends
- **Cache Manager**: Gerenciamento inteligente de cache
- **Performance Optimization**: Otimizações de performance integradas

#### Background Jobs
- **Hangfire Integration**: Processamento de tarefas em background
- **Job Management**: Agendamento e monitoramento de jobs
- **Recurring Jobs**: Tarefas recorrentes automatizadas
- **Worker Services**: Serviços de background escaláveis

#### UI e Frontend
- **Angular Integration**: Template Angular completo
- **Componentes UI**: Componentes reutilizáveis e estilizados
- **Validação Client-Side**: Validação automática no frontend
- **Internacionalização**: Suporte a múltiplos idiomas

#### Configuração e Settings
- **Setting Management**: Gerenciamento de configurações
- **Feature Flags**: Flags de funcionalidades
- **Environment Configuration**: Configuração por ambiente
- **Key Vault Integration**: Segurança de segredos

#### Outros Recursos
- **Event Bus**: Sistema de eventos de domínio
- **Data Filters**: Filtros de dados automáticos (SoftDelete, TenantId)
- **Repository Pattern**: Abstração de acesso a dados
- **Unit of Work**: Gerenciamento de transações
- **Dependency Injection**: Injeção de dependências configurada
- **Object Mapping**: AutoMapper integrado
- **API Documentation**: Swagger/OpenAPI automático

### Benefícios do EAF sobre ABP Puro

1. **Interface Mais Amigável**: APIs simplificadas e intuitivas
2. **Módulos Prontos**: Componentes pré-configurados para uso imediato
3. **Melhores Práticas**: Padrões de desenvolvimento modernos aplicados
4. **Performance Otimizada**: Otimizações de performance integradas
5. **Documentação em Português**: Suporte nativo para língua portuguesa
6. **Testes BDD**: Testes com padrão Dado/Quando/Então
7. **Observabilidade**: OpenTelemetry para monitoramento avançado
8. **Segurança Aprimorada**: Múltiplas opções de autenticação e autorização

---

## Visão Técnica

### Arquitetura

O EAF segue os princípios do Domain-Driven Design (DDD) e implementa padrões como:

- **Repository Pattern**: Abstração da camada de dados
- **Unit of Work**: Gerenciamento de transações
- **Dependency Injection**: Inversão de controle
- **CQRS**: Separação de comandos e consultas
- **Event Sourcing**: Rastreamento de eventos de domínio

### Tecnologias Suportadas

| Tecnologia | Versão | Status |
|------------|--------|--------|
| **ASP.NET Core** | 10.0 | Suportado |
| **Entity Framework Core** | 10.0 | Suportado |
| **Angular** | 20 | Suportado |
| **.NET** | 10.0 | Suportado |

### Componentes Principais

#### Middleware Core
- **Eaf.Middleware.Core**: Camada de domínio central com entidades, serviços, configurações, autorização, auditoria e funcionalidades base do framework.
- **Eaf.Middleware.Application**: Camada de aplicação com DTOs, serviços de aplicação, validações e lógica de negócio intermediária.
- **Eaf.Middleware.Web.Core**: Componentes web para ASP.NET Core incluindo configuração de startup, middleware, filtros e integração HTTP.

#### Autenticação e Autorização
- **Eaf.Middleware.AzureActiveDirectory**: Integração completa com Azure Active Directory para autenticação externa e sincronização de usuários.
- **Eaf.Middleware.Ldap**: Autenticação via LDAP/Active Directory para integração com diretórios existentes.

#### Cache e Persistência
- **Eaf.SqlServerCache**: Implementação de cache distribuído usando SQL Server como backend para cenários de alta disponibilidade.
- **Eaf.SqliteCache**: Implementação de cache local usando SQLite para cenários de desenvolvimento e baixa escala.

#### Segurança
- **Eaf.KeyVault**: Gerenciamento de segredos suportando Azure Key Vault e Oracle Cloud Infrastructure (OCI) para armazenamento seguro de credenciais.
- **Eaf.KeyVault.AspNetCore**: Integração ASP.NET Core para carregamento automático de configurações e segredos do Key Vault.

#### Observabilidade
- **Eaf.OpenTelemetry**: Implementação completa de OpenTelemetry para telemetria distribuída, tracing e métricas com suporte a múltiplos exporters.
- **Eaf.Castle.Serilog**: Adaptador de logging integrando Castle Windsor com Serilog para logging estruturado e configurável.

#### Processamento
- **Eaf.Middleware.Worker**: Serviços de background (Worker Services) para processamento assíncrono, jobs agendados e tarefas de longa duração.
- **Eaf.Log4NetServiceBus**: Integração com Azure Service Bus usando log4net para logging de mensagens e eventos de mensageria.

---

## Stack Tecnológico

### Backend (.NET)
- **.NET 10.0**: Framework principal
- **ASP.NET Core 10.0**: Web API e MVC
- **Entity Framework Core 10.0**: ORM para acesso a dados
- **AutoMapper**: Mapeamento de objetos
- **Castle Windsor**: Injeção de dependência
- **Hangfire**: Processamento de tarefas em background
- **SignalR**: Comunicação em tempo real
- **Swagger/OpenAPI**: Documentação de API
- **xUnit**: Framework de testes
- **Shouldly**: Assertions fluentes
- **NSubstitute**: Mocking framework

### Frontend (Template)
- **Angular 20**: Framework SPA
- **Node.js 20.20.2**: Runtime JavaScript
- **TypeScript 5.8.3**: Linguagem principal
- **ngx-bootstrap 12.0.0** / **Bootstrap 5**: Framework CSS
- **PrimeNG 17**: Componentes UI
- **Chart.js 4.4.7**: Gráficos e visualizações
- **RxJS 7.8.0**: Programação reativa

### Infraestrutura
- **SQLite**: Banco de dados local
- **SQL Server**: Banco de dados principal
- **Redis**: Cache distribuído
- **Azure Key Vault**: Gerenciamento de segredos
- **OpenTelemetry**: Observabilidade
- **Serilog**: Logging estruturado

---

## Documentação

A documentação técnica detalhada do sistema EAF, cobrindo arquitetura, módulos, guias de desenvolvimento e mais, pode ser encontrada em nosso portal de documentação.

[Acesse a Documentação Completa](./docs/README.md) | [DeepWiki - Docs com IA](https://deepwiki.com/afonsoft/EAF)

---

## Instalação e Configuração

### Pré-requisitos

**Obrigatórios:**
- .NET 10.0 SDK ou superior
- Node.js 20.20.2 (para desenvolvimento frontend)
- Git

**Para Desenvolvimento Frontend:**
```bash
npm install -g @angular/cli@20.3.32
```

**Para Relatórios de Cobertura:**
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Compatibilidade de Plataforma

| Plataforma | Status | Notas |
|------------|--------|-------|
| **Windows** | Suporte Completo | Use PowerShell ou Command Prompt |
| **Linux** | Suporte Completo | Scripts bash fornecidos |
| **macOS** | Suporte Completo | Use Terminal com bash |

### Clonando o Repositório

```bash
git clone https://github.com/afonsoft/EAF.git
cd EAF
```

### Configuração do Ambiente

1. **Restaurar dependências:**
```bash
dotnet restore Eaf.sln
```

2. **Compilar o projeto:**
```bash
dotnet build Eaf.sln
```

---

## Execução e Testes

### Início Rápido

**Linux/macOS:**
```bash
# Tornar o script executável
chmod +x build-and-test.sh

# Executar build e testes com cobertura
./build-and-test.sh
```

**Windows (PowerShell):**
```powershell
# Executar build e testes
dotnet build Eaf.sln
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Gerar relatório de cobertura (se reportgenerator estiver instalado)
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;TextSummary"
```

### Executando Apenas os Testes

**Linux/macOS:**
```bash
# Tornar o script executável
chmod +x run-tests-with-coverage.sh

# Executar todos os testes com cobertura
./run-tests-with-coverage.sh
```

**Windows (PowerShell):**
```powershell
# Executar todos os testes com cobertura
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Gerar relatório de cobertura
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;TextSummary"
```

### Execução Manual de Testes

```bash
# Executar um projeto de teste específico com cobertura
dotnet test test/Eaf.KeyVault.Tests/Eaf.KeyVault.Tests.csproj --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Executar todos os testes na solução
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Gerar relatório de cobertura
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;Badges;TextSummary"
```

### Exemplo de Uso

Vamos investigar uma classe simples para ver os benefícios do EAF:

```csharp
public class TaskAppService : ApplicationService, ITaskAppService
{
    private readonly IRepository<Task> _taskRepository;

    public TaskAppService(IRepository<Task> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [AbpAuthorize(MyPermissions.UpdateTasks)]
    public async Task UpdateTask(UpdateTaskInput input)
    {
        Logger.Info("Updating a task for input: " + input);

        var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
        if (task == null)
        {
            throw new UserFriendlyException(L("CouldNotFindTheTaskMessage"));
        }

        ObjectMapper.MapTo(input, task);
    }
}
```

Este exemplo demonstra vários recursos do EAF:

- **Injeção de Dependência**: O EAF usa e fornece uma infraestrutura de DI convencional
- **Repository**: O EAF pode criar um repositório padrão para cada entidade
- **Autorização**: O EAF pode verificar permissões declarativamente
- **Validação**: O EAF verifica automaticamente se a entrada é nula
- **Log de Auditoria**: Informações são salvas automaticamente para cada solicitação
- **Unidade de Trabalho**: Cada método de serviço de aplicação é uma unidade de trabalho por padrão

---

## Cobertura de Código

### Status dos Testes (Julho 2026)

| Módulo | Testes | Cobertura de Linha | Status |
|--------|--------|-------------------|--------|
| **Eaf.Castle.Serilog** | 73 | 100% | ✅ Excelente |
| **Eaf.SqlServerCache** | 100 | 100% | ✅ Excelente |
| **Eaf.KeyVault.AspNetCore** | 10 | 100% | ✅ Excelente |
| **Eaf.Middleware.Application** | 1507 | 99.9% | ✅ Excelente |
| **Eaf.Middleware.Core** | 1266 | 99.9% | ✅ Excelente |
| **Eaf.Middleware.Web.Core** | 769 | 96.9% | ✅ Excelente |
| **Eaf.Middleware.Worker** | 191 | 100% | ✅ Excelente |
| **Eaf.SqliteCache** | 162 | 98% | ✅ Excelente |
| **Eaf.OpenTelemetry** | 67 | 100% | ✅ Excelente |
| **Eaf.KeyVault** | 243 | 100% | ✅ Excelente |
| **Eaf.Log4NetServiceBus** | 52 | 96.0% | ✅ Boa |
| **Eaf.Middleware.AzureActiveDirectory** | 66 | 95.3% | ✅ Excelente |
| **Eaf.Middleware.Ldap** | 99 | 66.3% | ⚠️ Precisa melhorar |
| **TOTAL** | **4605** | **97.9%** | ✅ Em progresso |

### Meta de Cobertura
- **Objetivo**: 90% de cobertura de código
- **Atual**: 97.9% linha, 90.5% branch, 99.8% método; build `Eaf.sln` com 0 warnings
- **Testes Backend**: 4605 total, 4604 passando, 1 ignorado, 0 falhas (100% sucesso)
- **Testes Angular**: 222 total, 222 passando (100% sucesso)
- **Testes API Template**: 212 total, 211 passando, 1 ignorado
- **Build dos templates**: 0 warnings (`Api`, `Worker` e `Angular`)
- **Template API runtime**: `Eaf.ApiWithSrc.sln` iniciou e o Swagger carregou em `http://localhost:5000/swagger`; precedência das variáveis de ambiente corrigida para sobrescrever `appsettings.json`.
- **Próximos passos**: P67 — validar o template Worker em runtime e garantir a integração entre templates.

### Melhorias Implementadas
- **Expansão de Testes (Julho 2026)**: P58 coverage audit — testes BDD para SerilogLogger, TenantAddress, EafWorkerBase, UserAppService, ChatMessageManager, HostSettingsAppService, EafSqliteCache e EafSqlServerCache
  - **Eaf.Castle.Serilog**: +teste BDD para `SerilogLogger` com logger desabilitado (invoca todos os métodos de log sem chamar o sink)
  - **Eaf.MiddlewareCore**: +teste BDD para `TenantAddress.Tenant` (setter do navigation property)
  - **Eaf.Middleware.Worker**: +testes BDD para `EafWorkerBase.L` com `args` vazio nas duas sobrecargas
  - **Eaf.Middleware.Application**: +testes BDD para `UserAppService` (Azure AD com campos preenchidos e username sem domínio; LDAP com `TenantId`) e `ChatMessageManager` (delete sem mensagens, send com tenants nulos, atualização parcial de amizade)
  - **Eaf.Middleware.Application**: +testes BDD para `HostSettingsAppService.UpdateAllSettings` (Azure AD ClientId vazio, Google Analytics vazio, LDAP com valores, LogDeleter/LoginImpersonator preenchidos)
  - **Eaf.SqliteCache**: +testes BDD para expiração absoluta padrão e serialização com nulo/vazio
  - **Eaf.SqlServerCache**: +testes BDD para `TryGetValue` quando cache distribuído lança ou retorna bytes vazios e `ByteArrayToObject` com nulo/vazio
- **Expansão de Testes (Julho 2026)**: P57 coverage audit — testes BDD para AccountAppService, RoleAppService, TenantAppService, AzureActiveDirectoryAuthenticationSource, ProfileControllerBase, EafHangfireApplicationBuilderExtensions, ChatHub, EafWorkerBase, EafOpenTelemetryServiceCollectionExtensions e ServiceBusQueueAppender
  - **Eaf.Middleware.Application**: +testes BDD para AccountAppService (impersonate com tenant inativo), RoleAppService (filtro por permissão e delete sem usuários) e TenantAppService (features com escopo Tenant e mapper de FlatFeatureDto)
  - **Eaf.Middleware.AzureActiveDirectory**: +testes BDD para AzureActiveDirectoryAuthenticationSource (create/update com AbpException do Graph)
  - **Eaf.Middleware.Web.Core**: +testes BDD para ProfileControllerBase (ModelState inválido), EafHangfireApplicationBuilderExtensions (UseEafHangfire sem options) e ChatHub (DeleteMessage com SharedMessageId nulo, SendMessage com UserId/GroupId zero)
  - **Eaf.Middleware.Worker**: +testes BDD para EafWorkerBase (L com args nulo, LocalizationManager nulo, cache de LocalizationSource)
  - **Eaf.OpenTelemetry**: +testes BDD para EafOpenTelemetryServiceCollectionExtensions (AddEafOpenTelemetry sem opções e sem exporters)
  - **Eaf.Log4NetServiceBus**: +testes BDD para ServiceBusQueueAppender (SendBuffer com StorageType vazio)
- **KeyVault**: 241 testes BDD implementados (100% sucesso, 95.4% cobertura)
- **Documentação XML**: Summaries em português adicionados
- **Padrão BDD**: Dado/Quando/Então implementado
- **Castle.Serilog**: 72 testes BDD implementados (100% sucesso, 98.9% cobertura)
- **OpenTelemetry**: 64/64 testes passando (100% sucesso, 99.5% cobertura)
- **SqlServerCache**: 99/99 testes passando (100% sucesso, 98.1% cobertura)
- **SqliteCache**: 161/161 testes passando (100% sucesso, 98.0% cobertura)
- **Worker**: 186/186 testes passando (100% sucesso, 99.5% cobertura)
- **Middleware.Application**: 1496/1496 testes passando (100% sucesso, 99.7% cobertura)
- **Middleware.Web.Core**: 757/757 testes passando (100% sucesso, 96.9% cobertura)
- **Middleware.Core**: 1250/1250 testes passando (100% sucesso, 99.7% cobertura)
- **Expansão de Testes (Julho 2026)**: P56 coverage audit — testes BDD para HostSettingsAppService, ChatAppService, EafHostBuilderExtensions (Core/Worker), EafServiceCollectionExtensions (Worker), AzureActiveDirectoryAuthenticationSource e EafHangfireApplicationBuilderExtensions
  - **Eaf.Middleware.Application**: +testes BDD para HostSettingsAppService (UpdateAllSettings com sub-DTOs nulos, timezone, LogDeleter, LoginImpersonator, Google vazio, Azure AD/LDAP habilitados com valores em branco, external-login providers com JSON/mapeamentos de claims) e ChatAppService (lado das mensagens de grupo, marcação de mensagens não lidas com tenants distintos)
  - **Eaf.Middleware.AzureActiveDirectory**: +testes BDD para AzureActiveDirectoryAuthenticationSource (e-mail sem `@` em `Mail`/`UserPrincipalName` para `GetUserAsync`/`GetUsersAsync`/`UpdateUserAsync`)
  - **Eaf.MiddlewareCore**: +testes BDD para EafHostBuilderExtensions (action nula com prefixo, prefixo vazio) e ChatAppService (lado das mensagens de grupo via `ChatMessageManager`)
  - **Eaf.Middleware.Worker**: +testes BDD para EafHostBuilderExtensions (action nula com prefixo, prefixo vazio) e EafServiceCollectionExtensions (`AddEaf` sem `optionsAction`)
- **Expansão de Testes (Julho 2026)**: P55 coverage audit — testes BDD para UserEmailer, TenantManager, TenantAppService, LanguageAppService, NotificationAppService, HostSettingsAppService, ChatAppService, ChatController, ChatHub, FileController, DefaultExternalLoginInfoManager, RemoteAuthenticationContextExtensions, EafHostBuilderExtensions, EafServiceCollectionExtensions (Worker), EafOpenTelemetryServiceCollectionExtensions, EafHangfireAuthorizationFilter e HostRoleAndUserCreator
  - **Eaf.Middleware.Core**: +testes BDD para UserEmailer (sobrecargas `L` com args e cultura), TenantManager (validador de senha customizado), RemoteAuthenticationContextExtensions (mapeamentos vazios), EafHostBuilderExtensions (prefixo de environment variables), EafHangfireAuthorizationFilter (JWT com tenant sem `Sub`) e HostRoleAndUserCreator (seed idempotente)
  - **Eaf.Middleware.Application**: +testes BDD para LanguageAppService (idiomas vazios, SkipCount, idioma duplicado), TenantAppService (features de tenant), NotificationAppService (notificação de outro usuário) e HostSettingsAppService (erro no login impersonator e timezone)
  - **Eaf.Middleware.Web.Core**: +testes BDD para FileController (ModelState inválido), ChatController (ModelState inválido), ChatHub (dispose duplicado), DefaultExternalLoginInfoManager (name claim vazio) e AboutController (módulos registrados)
  - **Eaf.Middleware.Worker**: +testes BDD para EafHostBuilderExtensions (UseAbpConfiguration com prefixo) e EafServiceCollectionExtensions (CastleLoggerFactory registrado)
  - **Eaf.OpenTelemetry**: +teste BDD para EafOpenTelemetryServiceCollectionExtensions (variável OTLP inválida com `=`)
  - **Eaf.SqlServerCache**: +teste BDD para EafSqlServerCache (TryGetValue com cache existente)
- **Expansão de Testes (Julho 2026)**: P53 coverage audit — testes BDD para UserManager, UserAppService, TokenAuthController, FriendshipAppService, AzureActiveDirectoryAuthenticationSource, LdapAuthenticationSource e ServiceBusQueueAppender
  - **Eaf.Middleware.Application**: +testes BDD para UserManager (renomeação admin, username duplicado, permissões e roles), UserAppService (Azure AD novo, LDAP vazio/existente) e FriendshipAppService (bloqueio com clientes online, tenant inexistente)
  - **Eaf.Middleware.Web.Core**: +testes BDD para TokenAuthController (modelo inválido em Authenticate e ExternalAuthenticate)
  - **Eaf.Middleware.AzureActiveDirectory**: +teste BDD para AzureActiveDirectoryAuthenticationSource (tratamento de AbpException no Graph)
  - **Eaf.Middleware.Ldap**: +testes BDD para LdapAuthenticationSource (CreateUser, UpdateUser e GetUsers com entrada LDAP)
  - **Eaf.Log4NetServiceBus**: +testes BDD para ServiceBusQueueAppender (erro/exception/fallback)
  - **Eaf.MiddlewareCore.SampleApp**: +teste BDD para MiddlewareCoreModule (cache de amigos com expiração)
- **Expansão de Testes (Julho 2026)**: P52 coverage audit — testes BDD para MiddlewareControllerBase, EafKeyVaultConfigurationProvider, DefaultLanguagesCreator, TenantRoleAndUserBuilder, ProfileAppService, ChatAppService e ChatMessageManager
  - **Eaf.Middleware.Web.Core**: +testes BDD para MiddlewareControllerBase (`L` com cultura)
  - **Eaf.Hosting**: +teste BDD para EafKeyVaultConfigurationProvider (provider desconhecido)
  - **Eaf.MiddlewareCore.SampleApp**: +testes BDD para DefaultLanguagesCreator e TenantRoleAndUserBuilder (seed idempotente)
  - **Eaf.Middleware.Application**: +testes BDD para ProfileAppService (foto definida, foto nula, imagem acima de 5MB), ChatAppService e ChatMessageManager
- **Expansão de Testes (Julho 2026)**: P51 coverage audit — testes BDD para WorkerContentFileProvider, MiddlewareWorkerModule e AuthZeroAuthProviderApi
  - **Eaf.Middleware.Worker**: +testes BDD para WorkerContentFileProvider (arquivos/diretórios existentes) e MiddlewareWorkerModule (ServiceReplaceActions de email)
  - **Eaf.Middleware.Core**: +testes BDD para AuthZeroAuthProviderApi (Endpoint vazio, foto com base64)
- **Expansão de Testes (Julho 2026)**: P50 coverage audit — testes BDD para NamespaceStripper e EafWebHookReceiver
  - **Eaf.Middleware.Application**: +teste BDD para NamespaceStripper (genérico sem namespace nos argumentos)
  - **Eaf.Middleware.Web.Core**: +testes BDD para EafWebHookReceiver (L com args, propriedades, CurrentUnitOfWork)
- **Expansão de Testes (Julho 2026)**: P49 coverage audit — testes BDD para PasswordComplexitySetting e AbpLoginResultTypeHelper, mantendo cobertura após ajustes do Sonar
  - **Eaf.Middleware.Core**: +testes BDD para PasswordComplexitySetting (GetHashCode, Equals com outro tipo)
  - **Eaf.Middleware.Application**: +testes BDD para AbpLoginResultTypeHelper (SanitizeForLog, L com cultura)
- **Expansão de Testes (Julho 2026)**: P39 coverage audit — cobertura de linha subiu para 90.8% e branch para 72%
  - **Eaf.Middleware.Web.Core**: +testes BDD para TokenAuthController (LogOut, TwoFactor, RegisterExternalUser, TeamsAuthenticate, helpers privados)
  - **Eaf.Middleware.Web.Core**: +testes BDD para MiddlewareWebCoreModule (Hangfire Redis/SqlServer e ExpiredAuditLogDeleterWorker)
  - **Eaf.MiddlewareCore**: +testes BDD para OpenIdConnectAuthProviderApi.GetUserInfo (JWT mockado com RSA e OIDC discovery)
  - **Eaf.MiddlewareCore**: +testes BDD para WebContentDirectoryFinder.CalculateContentRootFolder
- **Expansão de Testes (Junho 2026)**: +12 novos arquivos de testes implementados (PR #63)
  - **Eaf.Middleware.Web.Core**: +10 testes (Swagger filters, TokenAuth, Impersonation models)
  - **Eaf.MiddlewareCore**: +30 testes (Entities, DTOs, Extensions, Cache items)
  - **Eaf.Middleware.Application**: +9 testes (constants, helpers, authorization)
  - **Eaf.Middleware.Ldap**: +4 testes (configuration, authentication)
  - **Eaf.Middleware.AzureActiveDirectory**: +4 testes (configuration, authentication)
  - **Eaf.Middleware.Worker**: +4 testes (folders, emailing, base classes)
  - **Eaf.KeyVault**: +3 testes (managers, interfaces)
  - **Eaf.Castle.Serilog**: +4 testes (module, factory, logger)
  - **Eaf.KeyVault.AspNetCore**: +1 teste (extensions)
  - **Eaf.Log4NetServiceBus**: +3 testes (logging components)
  - **Eaf.OpenTelemetry**: +2 testes (module, extensions)
  - **Eaf.SqlServerCache**: +2 testes (helpers, extensions)
  - **Eaf.SqliteCache**: +4 testes (pool, commands, options)
- **Correção de Bug (Junho 2026)**: Parâmetros de expiração do EafSqliteCache.Set corrigidos (PR #64)

### Correções Técnicas Implementadas

#### SqliteCache - Correção de Parâmetros de Expiração (Junho 2026)
- **Problema**: `EafSqliteCache.Set` ignora parâmetros `slidingExpireTime` e `absoluteExpireTime`
- **Causa**: Parâmetros não eram repassados ao método interno `CreateForSet`
- **Solução**: Repassar ambos parâmetros: `CreateForSet(cmd, key, value, slidingExpireTime, absoluteExpireTime)`
- **Resultado**: Expiração do cache funciona corretamente; +14 testes de expiração adicionados

#### SqliteCache - Correção de Inicialização Estática
- **Problema**: IndexOutOfRangeException na inicialização do DbCommandPool
- **Causa**: Ordem incorreta de inicialização das propriedades estáticas
- **Solução**: Movido `Count` antes de `Commands` para garantir inicialização correta
- **Resultado**: +21 testes passando (de 53 para 74), melhoria de 39%

#### Castle.Serilog - Resolução Completa
- **Problema**: Conflitos entre Castle.Core.Logging.ILogger e Serilog.ILogger
- **Solução**: Alias `SerilogILogger` para resolver ambiguidade de namespace
- **Mocks Problemáticos**: Substituídos por instâncias reais do Serilog
- **Testes Inválidos**: Removidos testes que dependiam de configuração runtime não suportada
- **Resultado**: 44/44 testes passando com padrão BDD em português

### Status da Documentação XML
- **Arquivos Documentados**: 6/507 (1.2%)
- **Classes Principais Documentadas**:
  - **SerilogLoggerFactory** - Fábrica de loggers Serilog
  - **SerilogLogger** - Implementação do logger
  - **EafSqliteCache** - Cache baseado em SQLite
  - **MiddlewareAppServiceBase** - Classe base para serviços
  - **AzureActiveDirectoryAuthenticationSource** - Autenticação Azure AD
- **Próximos Módulos**: Entity Framework, Web API, Authorization

### Cobertura por Assembly (Julho 2026)

| Assembly | Cobertura de Linha | Testes | Status |
|----------|-------------------|--------|--------|
| **Eaf.Castle.Serilog** | 98.9% | 72 | ✅ |
| **Eaf.SqlServerCache** | 98.1% | 99 | ✅ |
| **Eaf.KeyVault.AspNetCore** | 100% | 10 | ✅ |
| **Eaf.Middleware.Application** | 99.7% | 1496 | ✅ |
| **Eaf.Middleware.Core** | 99.7% | 1250 | ✅ |
| **Eaf.Middleware.Web.Core** | 96.9% | 757 | ✅ |
| **Eaf.Middleware.Worker** | 99.5% | 186 | ✅ |
| **Eaf.SqliteCache** | 98.0% | 161 | ✅ |
| **Eaf.OpenTelemetry** | 99.5% | 64 | ✅ |
| **Eaf.KeyVault** | 100% | 243 | ✅ |
| **Eaf.Log4NetServiceBus** | 96.0% | 51 | ✅ |
| **Eaf.Middleware.AzureActiveDirectory** | 95.3% | 64 | ✅ |
| **Eaf.Middleware.Ldap** | 66.3% | 99 | ⚠️ |

### Status dos Projetos de Teste

| Projeto | Status | Testes | Cobertura de Linha | Notas |
|---------|--------|--------|-------------------|-------|
| **Eaf.Castle.Serilog.Tests** | ✅ Passando | 72 | 98.9% | BDD em português |
| **Eaf.KeyVault.Tests** | ✅ Passando | 242 | 100% | Padrão BDD |
| **Eaf.KeyVault.AspNetCore.Tests** | ✅ Passando | 10 | 100% | Excelente cobertura |
| **Eaf.Log4NetServiceBus.Tests** | ✅ Passando | 52 | 96.0% | Boa cobertura |
| **Eaf.Middleware.Application.Tests** | ✅ Passando | 1496 | 99.7% | Maior suíte de testes |
| **Eaf.Middleware.AzureActiveDirectory.Tests** | ✅ Passando | 66 | 95.3% | Cobertura básica |
| **Eaf.Middleware.Ldap.Tests** | ✅ Passando | 99 | 66.3% | Cobertura básica |
| **Eaf.Middleware.Worker.Tests** | ✅ Passando | 186 | 99.5% | Lifecycle, Background jobs |
| **Eaf.Middleware.Web.Core.Tests** | ✅ Passando | 757 | 96.9% | Swagger, TokenAuth, Impersonation |
| **Eaf.MiddlewareCore.Tests** | ✅ Passando | 1250 | 99.7% | Core, SampleApp, DDD |
| **Eaf.OpenTelemetry.Tests** | ✅ Passando | 64 | 99.5% | Boa cobertura |
| **Eaf.SqliteCache.Tests** | ✅ Passando | 161 | 98.0% | +14 testes de expiração |
| **Eaf.SqlServerCache.Tests** | ✅ Passando | 99 | 98.1% | Excelente cobertura |

### Testes Template Angular

| Métrica | Valor |
|---------|-------|
| **Total de Testes** | 222 |
| **Passando** | 222 (100%) |
| **Cobertura de Statements** | 11.68% |
| **Cobertura de Branches** | 2.56% |
| **Cobertura de Functions** | 9.01% |
| **Cobertura de Linhas** | 11.16% |

### Testes Template API

| Métrica | Valor |
|---------|-------|
| **Total de Testes** | 212 |
| **Passando** | 211 |
| **Falhando** | 0 |
| **Ignorados** | 1 |
| **Correção Aplicada** | `OnConfiguring` agora verifica provider via `optionsBuilder.Options.Extensions` ao invés de `Database.IsSqlServer()` |

**Legenda:**
- **Passando**: Todos os testes passam com sucesso
- **Problemas**: Testes executam mas têm falhas ou avisos
- **Erros de Build**: Projeto falha ao compilar

### Padrão de Testes BDD

Os testes seguem o padrão BDD (Behavior-Driven Development) em português:

```csharp
[Fact]
public void Dado_ParametroValido_Quando_ChamarMetodo_Entao_DeveRetornarSucesso()
{
    // Dado (Given)
    var parametro = "valor_valido";
    
    // Quando (When)
    var resultado = _service.ProcessarParametro(parametro);
    
    // Então (Then)
    resultado.ShouldNotBeNull();
    resultado.Sucesso.ShouldBeTrue();
}
```

---

## Pacotes NuGet

| Pacote | NuGet | Descrição |
|--------|-------|-----------|
| [Eaf.Middleware.Application](https://www.nuget.org/packages/Eaf.Middleware.Application/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Application.svg)](https://badge.fury.io/nu/Eaf.Middleware.Application) | Camada de aplicação |
| [Eaf.Middleware.AzureActiveDirectory](https://www.nuget.org/packages/Eaf.Middleware.AzureActiveDirectory/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.AzureActiveDirectory.svg)](https://badge.fury.io/nu/Eaf.Middleware.AzureActiveDirectory) | Integração Azure AD |
| [Eaf.Middleware.Core](https://www.nuget.org/packages/Eaf.Middleware.Core/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Core.svg)](https://badge.fury.io/nu/Eaf.Middleware.Core) | Funcionalidades core |
| [Eaf.Middleware.Ldap](https://www.nuget.org/packages/Eaf.Middleware.Ldap/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Ldap.svg)](https://badge.fury.io/nu/Eaf.Middleware.Ldap) | Autenticação LDAP |
| [Eaf.Middleware.Web.Core](https://www.nuget.org/packages/Eaf.Middleware.Web.Core/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Web.Core.svg)](https://badge.fury.io/nu/Eaf.Middleware.Web.Core) | Componentes web |
| [Eaf.Castle.Serilog](https://www.nuget.org/packages/Eaf.Castle.Serilog/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Castle.Serilog.svg)](https://badge.fury.io/nu/Eaf.Castle.Serilog) | Logging estruturado |
| [Eaf.KeyVault](https://www.nuget.org/packages/Eaf.KeyVault/) | [![NuGet version](https://badge.fury.io/nu/Eaf.KeyVault.svg)](https://badge.fury.io/nu/Eaf.KeyVault) | Gerenciamento de segredos |
| [Eaf.KeyVault.AspNetCore](https://www.nuget.org/packages/Eaf.KeyVault.AspNetCore/) | [![NuGet version](https://badge.fury.io/nu/Eaf.KeyVault.AspNetCore.svg)](https://badge.fury.io/nu/Eaf.KeyVault.AspNetCore) | Integração ASP.NET Core |
| [Eaf.OpenTelemetry](https://www.nuget.org/packages/Eaf.OpenTelemetry/) | [![NuGet version](https://badge.fury.io/nu/Eaf.OpenTelemetry.svg)](https://badge.fury.io/nu/Eaf.OpenTelemetry) | Telemetria e observabilidade |
| [Eaf.Log4NetServiceBus](https://www.nuget.org/packages/Eaf.Log4NetServiceBus/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Log4NetServiceBus.svg)](https://badge.fury.io/nu/Eaf.Log4NetServiceBus) | Service bus logging |
| [Eaf.SqlServerCache](https://www.nuget.org/packages/Eaf.SqlServerCache/) | [![NuGet version](https://badge.fury.io/nu/Eaf.SqlServerCache.svg)](https://badge.fury.io/nu/Eaf.SqlServerCache) | Cache SQL Server |
| [Eaf.SqliteCache](https://www.nuget.org/packages/Eaf.SqliteCache/) | [![NuGet version](https://badge.fury.io/nu/Eaf.SqliteCache.svg)](https://badge.fury.io/nu/Eaf.SqliteCache) | Cache SQLite |
| [Eaf.Middleware.Worker](https://www.nuget.org/packages/Eaf.Middleware.Worker/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Worker.svg)](https://badge.fury.io/nu/Eaf.Middleware.Worker) | Background services |

---

## Contribuição

### Como Contribuir

1. **Fork** o repositório
2. **Crie** uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. **Push** para a branch (`git push origin feature/AmazingFeature`)
5. **Abra** um Pull Request

### Padrões de Desenvolvimento

- **Testes**: Todos os novos recursos devem ter testes unitários
- **Cobertura**: Manter cobertura mínima de 90%
- **Documentação**: Adicionar XML documentation em métodos públicos
- **BDD**: Seguir padrão Dado/Quando/Então em português

### Links Úteis

- [Documentação Completa](src/README.md)
- [Política de Segurança](SECURITY.md)
- [Changelog](CHANGELOG.md)
- [Guia de Testes](TESTING.md)

---

## Qualidade e Métricas

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-black.svg)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2)

| Code Smell | Bugs | Tests | Lang | Quality |
|------------|------|-------|------|---------|
| [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=code_smells)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=bugs)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | ![AppVeyor tests](https://img.shields.io/appveyor/tests/afonsoft/eaf) | ![GitHub top language](https://img.shields.io/github/languages/top/afonsoft/eaf) | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) |

### Estatísticas

| Lines of Code | Duplicated Lines | Coverage | Maintainability |
|---------------|------------------|----------|-----------------|
| [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=ncloc)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=duplicated_lines_density)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=coverage)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=sqale_rating)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) |

| Reliability | Security | Technical Debt | Vulnerabilities |
|-------------|----------|----------------|-----------------|
| [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=reliability_rating)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=security_rating)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=sqale_index)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=vulnerabilities)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) |

### Downloads

![GitHub all releases](https://img.shields.io/github/downloads/afonsoft/eaf/total)

### Issues

![GitHub issues](https://img.shields.io/github/issues-raw/afonsoft/eaf)

---

**Desenvolvido com ❤️ pela comunidade opensource**

Além deste exemplo simples, o EAF fornece uma infraestrutura robusta e modelo de desenvolvimento para [modularidade](https://aspnetboilerplate.com/Pages/Documents/Module-System), [multi-tenancy](https://aspnetboilerplate.com/Pages/Documents/Multi-Tenancy), [cache](https://aspnetboilerplate.com/Pages/Documents/Caching), [jobs em background](https://aspnetboilerplate.com/Pages/Documents/Background-Jobs-And-Workers), [filtros de dados](https://aspnetboilerplate.com/Pages/Documents/Data-Filters), [gerenciamento de configurações](https://aspnetboilerplate.com/Pages/Documents/Setting-Management), [eventos de domínio](https://aspnetboilerplate.com/Pages/Documents/EventBus-Domain-Events), testes unitários e de integração, e muito mais! Você foca no seu código de negócio e não se repete!

---

## Star History

[![Star History Chart](https://api.star-history.com/chart?repos=afonsoft/eaf&type=date&legend=top-left&sealed_token=LuAl7DTwrVSZjyWlqewFeoezq4tojGQ6ESqMVSmAJErLd2FM9PStjfERSyqaN3tSXTNTVQ02MXxKOq5_hG9N_W8hyMGZqr2uFrlblerV0uAcAHU1LRvzog)](https://www.star-history.com/?repos=afonsoft%2Feaf&type=date&legend=top-left)

## StarMapper

[![StarMapper](https://img.shields.io/badge/StarMapper-afonsoft%2Feaf-blue)](https://starmapper.bruniaux.com/afonsoft/eaf)

> O StarMapper também requer um token do GitHub para obter os dados de geolocalização das estrelas. O mapa ao vivo não está disponível até que o repositório seja escaneado.
