# Enterprise Application Foundation (EAF)

[![GitHub](https://img.shields.io/github/license/afonsoft/eaf)](LICENSE) [![GitHub version](https://badge.fury.io/gh/afonsoft%2Feaf.svg)](https://badge.fury.io/gh/afonsoft%2Feaf) [![Commits History](https://img.shields.io/badge/Commits-History-critical)](https://github.com/afonsoft/EAF/commits/main/) [![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) [![DeepWiki](https://img.shields.io/badge/DeepWiki-afonsoft%2FEAF-blue?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyNCIgaGVpZ2h0PSIyNCIgdmlld0JveD0iMCAwIDI0IDI0IiBmaWxsPSJub25lIiBzdHJva2U9IiNmZmZmZmYiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIiBzdHJva2UtbGluZWpvaW49InJvdW5kIj48cGF0aCBkPSJNNCAxOWguMDEiLz48cGF0aCBkPSJNMjAgMTEuMmMuNy40IDEuMSAxIDEuMSAxLjggMCAuNS0uMyAxLjEtLjcgMS41bC0zLjEgMy4xYy0uNS41LTEgLjctMS42LjdsLS44LS4xLTEuNC0uNS0xLjgtMS4xIi8+PHBhdGggZD0iTTQuMyAxNS4zYy0uNC0uNy0uNS0xLjUtLjMtMi4yLjItLjguNy0xLjQgMS4zLTEuOGwxLjgtMS4yYy43LS40IDEuNS0uNiAyLjItLjQuOC4yIDEuNS43IDEuOSAxLjMiLz48cGF0aCBkPSJNOCA1YzAtLjUuMi0xIC42LTEuNEM5IDMuMiA5LjUgMyAxMCAzaDRjLjUgMCAxIC4yIDEuNC42LjQuNC42LjkuNiAxLjR2M2MwIC41LS4yIDEtLjYgMS40LS40LjQtLjkuNi0xLjQuNmgtNGMtLjUgMC0xLS4yLTEuNC0uNkM4LjIgOSA4IDguNSA4IDgiLz48L3N2Zz4=)](https://deepwiki.com/afonsoft/EAF)

**[English](README.md)** | Português

![Line Coverage](https://img.shields.io/badge/Line%20Coverage-88.1%25-brightgreen)
![Branch Coverage](https://img.shields.io/badge/Branch%20Coverage-68.0%25-yellow)
![Method Coverage](https://img.shields.io/badge/Method%20Coverage-96.3%25-brightgreen)
![Test Success Rate](https://img.shields.io/badge/Test%20Success%20Rate-100%25-brightgreen)
![Total Tests](https://img.shields.io/badge/Total%20Tests-4063-blue)
![Passing Tests](https://img.shields.io/badge/Passing%20Tests-4062-brightgreen)
![Build Warnings](https://img.shields.io/badge/Build%20Warnings-68-yellow)
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
| **Eaf.Castle.Serilog** | 71 | 98.9% | ✅ Excelente |
| **Eaf.SqlServerCache** | 95 | 98.1% | ✅ Excelente |
| **Eaf.KeyVault.AspNetCore** | 10 | 100% | ✅ Excelente |
| **Eaf.Middleware.Application** | 1399 | 95.7% | ✅ Excelente |
| **Eaf.Middleware.Core** | 1185 | 94.8% | ✅ Excelente |
| **Eaf.Middleware.Worker** | 154 | 92.5% | ✅ Excelente |
| **Eaf.SqliteCache** | 134 | 91.6% | ✅ Excelente |
| **Eaf.OpenTelemetry** | 61 | 98.7% | ✅ Excelente |
| **Eaf.KeyVault** | 241 | 95.4% | ✅ Excelente |
| **Eaf.Log4NetServiceBus** | 49 | 80.3% | ✅ Boa |
| **Eaf.Middleware.Web.Core** | 607 | 81.1% | ✅ Boa |
| **Eaf.Middleware.AzureActiveDirectory** | 23 | 32.2% | ⚠️ Precisa melhorar |
| **Eaf.Middleware.Ldap** | 34 | 20.1% | ❌ Baixa |
| **TOTAL** | **4063** | **88.1%** | ✅ Em progresso |

### Meta de Cobertura
- **Objetivo**: 90% de cobertura de código
- **Atual**: 88.1% linha, 68.0% branch, 96.3% método
- **Testes Backend**: 4063 total, 4062 passando, 1 ignorado, 0 falhas (100% sucesso)
- **Testes Angular**: 222 total, 222 passando (100% sucesso)
- **Testes API Template**: 212 total, 211 passando, 1 ignorado
- **Próximos passos**: Melhorar cobertura dos módulos Middleware.Ldap, AzureActiveDirectory e cobertura de branch geral

### Melhorias Implementadas
- **KeyVault**: 241 testes BDD implementados (100% sucesso, 95.4% cobertura)
- **Documentação XML**: Summaries em português adicionados
- **Padrão BDD**: Dado/Quando/Então implementado
- **Castle.Serilog**: 71 testes BDD implementados (100% sucesso, 98.9% cobertura)
- **OpenTelemetry**: 61/61 testes passando (100% sucesso, 98.7% cobertura)
- **SqlServerCache**: 95/95 testes passando (100% sucesso, 98.1% cobertura)
- **SqliteCache**: 134/134 testes passando (100% sucesso, 91.6% cobertura)
- **Worker**: 154/154 testes passando (100% sucesso, 92.5% cobertura)
- **Middleware.Application**: 1399/1399 testes passando (100% sucesso, 95.7% cobertura)
- **Middleware.Web.Core**: 607/607 testes passando (100% sucesso, 81.1% cobertura)
- **Expansão de Testes (Julho 2026)**: +4 arquivos de testes implementados (P37 coverage audit)
  - **Eaf.KeyVault**: +testes BDD para AzureKeyVaultManager (GetValue, GetKeyValues, SetValue, async)
  - **Eaf.Log4NetServiceBus**: +testes BDD para ServiceBusQueueAppender (SendBuffer, OnClose)
  - **Eaf.Middleware.Web.Core**: +testes BDD para EafWebHookReceiver (LocalizationSource)
  - **Eaf.OpenTelemetry**: +testes BDD para EafOpenTelemetryServiceCollectionExtensions (hosted services)
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

### Cobertura por Assembly (Junho 2026)

| Assembly | Cobertura de Linha | Testes | Status |
|----------|-------------------|--------|--------|
| **Eaf.Castle.Serilog** | 98.6% | 71 | ✅ |
| **Eaf.SqlServerCache** | 88.3% | 95 | ✅ |
| **Eaf.KeyVault.AspNetCore** | 88.8% | 10 | ✅ |
| **Eaf.SqliteCache** | 82.7% | 126 | ✅ |
| **Eaf.OpenTelemetry** | 75.4% | 49 | ✅ |
| **Eaf.KeyVault** | 69.4% | 222 | ✅ |
| **Eaf.Log4NetServiceBus** | 62.2% | 41 | ⚠️ |
| **Eaf.Middleware.Worker** | 33.3% | 101 | ⚠️ |
| **Eaf.Middleware.Application** | 23.6% | 410 | ⚠️ |
| **Eaf.Middleware.AzureActiveDirectory** | 7.4% | 21 | ❌ |
| **Eaf.Middleware.Ldap** | 6.0% | 21 | ❌ |
| **Eaf.Middleware.Web.Core** | 4.9% | 122 | ❌ |
| **Eaf.Middleware.Core** | 0.1% | — | ❌ |

### Status dos Projetos de Teste

| Projeto | Status | Testes | Cobertura de Linha | Notas |
|---------|--------|--------|-------------------|-------|
| **Eaf.Middleware.Application.Tests** | ✅ Passando | 410 | 23.6% | Maior suíte de testes |
| **Eaf.KeyVault.Tests** | ✅ Passando | 222 | 69.4% | Padrão BDD |
| **Eaf.SqliteCache.Tests** | ✅ Passando | 126 | 82.7% | +14 testes de expiração |
| **Eaf.Middleware.Web.Core.Tests** | ✅ Passando | 122 | 4.9% | Swagger, TokenAuth, Impersonation |
| **Eaf.Middleware.Worker.Tests** | ✅ Passando | 101 | 33.3% | Lifecycle, Background jobs |
| **Eaf.SqlServerCache.Tests** | ✅ Passando | 95 | 88.3% | Excelente cobertura |
| **Eaf.Castle.Serilog.Tests** | ✅ Passando | 71 | 98.6% | BDD em português |
| **Eaf.OpenTelemetry.Tests** | ✅ Passando | 49 | 75.4% | Boa cobertura |
| **Eaf.Log4NetServiceBus.Tests** | ✅ Passando | 41 | 62.2% | Boa cobertura |
| **Eaf.Middleware.AzureActiveDirectory.Tests** | ✅ Passando | 21 | 7.4% | Cobertura básica |
| **Eaf.Middleware.Ldap.Tests** | ✅ Passando | 21 | 6.0% | Cobertura básica |
| **Eaf.KeyVault.AspNetCore.Tests** | ✅ Passando | 10 | 88.8% | Excelente cobertura |

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

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-black.svg)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main)

| Code Smell | Bugs | Tests | Lang | Quality |
|------------|------|-------|------|---------|
| [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=code_smells)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=bugs)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | ![AppVeyor tests](https://img.shields.io/appveyor/tests/afonsoft/eaf) | ![GitHub top language](https://img.shields.io/github/languages/top/afonsoft/eaf) | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) |

### Estatísticas

| Lines of Code | Duplicated Lines | Coverage | Maintainability |
|---------------|------------------|----------|-----------------|
| [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=ncloc)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=duplicated_lines_density)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=coverage)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=sqale_rating)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) |

| Reliability | Security | Technical Debt | Vulnerabilities |
|-------------|----------|----------------|-----------------|
| [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=reliability_rating)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=security_rating)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=sqale_index)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=vulnerabilities)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) |

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
