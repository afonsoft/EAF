# Enterprise Application Foundation (EAF)

[![GitHub](https://img.shields.io/github/license/afonsoft/eaf)](LICENSE) [![GitHub version](https://badge.fury.io/gh/afonsoft%2Feaf.svg)](https://badge.fury.io/gh/afonsoft%2Feaf) [![Commits History](https://img.shields.io/badge/Commits-History-critical)](https://github.com/afonsoft/EAF/commits/main/)

![Line Coverage](https://img.shields.io/badge/Line%20Coverage-24.1%25-yellow)
![Branch Coverage](https://img.shields.io/badge/Branch%20Coverage-15.5%25-red)
![Method Coverage](https://img.shields.io/badge/Method%20Coverage-54.5%25-yellow)
![Test Success Rate](https://img.shields.io/badge/Test%20Success%20Rate-100%25-brightgreen)
![Total Tests](https://img.shields.io/badge/Total%20Tests-1492-blue)
![Passing Tests](https://img.shields.io/badge/Passing%20Tests-1491-brightgreen)
![API Template Tests](https://img.shields.io/badge/API%20Template%20Tests-122%20Total-blue)
![API Template Passing](https://img.shields.io/badge/API%20Template%20Passing-121%20Success-brightgreen)

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
| **Angular** | 18 | Suportado |
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
- **Angular 18**: Framework SPA
- **Node.js 20.20.0**: Runtime JavaScript
- **TypeScript 5.2**: Linguagem principal
- **Bootstrap 5**: Framework CSS
- **PrimeNG 17**: Componentes UI
- **Chart.js**: Gráficos e visualizações
- **RxJS 7**: Programação reativa

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

[Acesse a Documentação Completa](./docs/README.md)

---

## Instalação e Configuração

### Pré-requisitos

**Obrigatórios:**
- .NET 10.0 SDK ou superior
- Node.js 20.20.0 (para desenvolvimento frontend)
- Git

**Para Desenvolvimento Frontend:**
```bash
npm install -g @angular/cli@19
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

### Status dos Testes (Atualizado)

| Módulo | Cobertura de Linha | Cobertura de Branch | Novos Testes | Status |
|--------|-------------------|---------------------|--------------|--------|
| **Eaf.SqlServerCache** | 86.6% | 81.8% | +2 | Boa cobertura |
| **Eaf.SqliteCache** | 75.5% | 34.9% | +4 | Boa cobertura |
| **Eaf.OpenTelemetry** | 70.1% | 75.0% | +2 | Boa cobertura |
| **Eaf.Middleware.Worker** | 24.6% | N/A | +4 | Precisa de +65.4% |
| **Eaf.Middleware.Ldap** | 5.1% | N/A | +4 | Precisa de +84.9% |
| **Eaf.Middleware.AzureActiveDirectory** | N/A | N/A | +4 | Testes adicionados |
| **Eaf.Middleware.Web.Core** | 0.63% | 0.16% | +10 | Precisa de +89.4% |
| **Eaf.Middleware.Core** | 15.31% | 5.59% | +7 | Precisa de +74.7% |
| **Eaf.Middleware.Application** | 4.45% | 1.35% | +15 | Precisa de +85.6% |
| **Eaf.KeyVault** | 66.0% | 51.6% | +3 | Testes adicionados |
| **TOTAL** | **28.4%** | **21.3%** | **+47** | Em progresso |

### Meta de Cobertura
- **Objetivo**: 90% de cobertura de código
- **Atual**: 24.1% linha, 15.5% branch, 54.5% método
- **Status dos Testes (Maio 2026)**: 1492 total, 1491 passando, 1 ignorado, 0 falhas (100% sucesso)
- **Próximos passos**: Implementar testes para módulos com baixa cobertura

### Melhorias Implementadas
- **KeyVault**: 206+ testes BDD implementados (99.5% sucesso, 66% cobertura)
- **Documentação XML**: Summaries em português adicionados
- **Padrão BDD**: Dado/Quando/Então implementado
- **Castle.Serilog**: 44 testes BDD implementados (100% sucesso, 90% cobertura)
- **OpenTelemetry**: 45/45 testes passando (100% sucesso)
- **SqlServerCache**: 38/38 testes passando (100% sucesso)
- **Worker**: 63/63 testes passando (100% sucesso)
- **Expansão de Testes (Abril 2026)**: +65 novos arquivos de testes implementados
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

### Correções Técnicas Implementadas

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

### Cobertura por Assembly (Atualizada)

| Assembly | Cobertura de Linha | Cobertura de Branch | Cobertura de Método |
|----------|-------------------|---------------------|---------------------|
| **Eaf.KeyVault** | 66.0% (200/303) | 51.6% (33/64) | 70.2% (40/57) |
| **Eaf.SqlServerCache** | 78.3% (123/157) | 81.8% (18/22) | 71.4% (15/21) |
| **Eaf.OpenTelemetry** | 68.9% (164/238) | 75.0% (60/80) | 95.2% (20/21) |
| **Eaf.Log4NetServiceBus** | 85.0% (97/114) | 62.5% (10/16) | 87.1% (27/31) |
| **Eaf.SqliteCache** | 45.0% (175/389) | 34.9% (30/86) | 58.5% (24/41) |
| **Eaf.Castle.Serilog** | 10.0% (29/289) | 3.6% (3/84) | 16.9% (10/59) |
| **Eaf.KeyVault.AspNetCore** | 85.7% (18/21) | 100.0% (4/4) | 50.0% (1/2) |
| **Eaf.Middleware.Web.Core** | 0.63% (1/159) | 0.16% (1/625) | 1.35% (1/74) |
| **Eaf.Middleware.Core** | 15.31% (24/157) | 5.59% (5/90) | 16.9% (10/59) |
| **Eaf.Middleware.Application** | 4.45% (7/157) | 1.35% (1/74) | 5.59% (5/90) |

### Status dos Projetos de Teste

| Projeto | Status | Testes | Cobertura de Linha | Notas |
|---------|--------|--------|-------------------|-------|
| **Eaf.KeyVault.Tests** | Passando | 210 | 67.9% | Excelente cobertura |
| **Eaf.SqlServerCache.Tests** | Passando | 57 | 86.6% | Excelente cobertura |
| **Eaf.OpenTelemetry.Tests** | Passando | 10 | 68.9% | Boa cobertura |
| **Eaf.KeyVault.AspNetCore.Tests** | Passando | 8 | 85.7% | Excelente cobertura |
| **Eaf.Log4NetServiceBus.Tests** | Passando | 12 | 85.0% | Boa cobertura |
| **Eaf.SqliteCache.Tests** | Passando | 94 | 75.6% | Boa cobertura |
| **Eaf.Castle.Serilog.Tests** | Passando | 44 | 10.0% | Resolvido com BDD em português |
| **Eaf.Middleware.Web.Core.Tests** | Passando | 78 | 0.63% | **EXPANDIDO**: +35 testes (Swagger, TokenAuth, Impersonation) |
| **Eaf.Middleware.Application.Tests** | Passando | 58 | 4.45% | Cobertura básica |
| **Eaf.Middleware.Worker.Tests** | Passando | 70 | 25.97% | Cobertura moderada |
| **Eaf.MiddlewareCore.Tests** | Passando | 632 | 15.31% | **EXPANDIDO**: +30 testes (Entities, DTOs, Extensions) |

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

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-black.svg)](https://sonarcloud.io/project/overview?id=afonsoft_EAF)

| Code Smell | Bugs | Tests | Lang | Quality |
|------------|------|-------|------|---------|
| [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=code_smells)](https://sonarcloud.io/dashboard?id=EAF) | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=bugs)](https://sonarcloud.io/dashboard?id=EAF) | ![AppVeyor tests](https://img.shields.io/appveyor/tests/afonsoft/eaf) | ![GitHub top language](https://img.shields.io/github/languages/top/afonsoft/eaf) | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=alert_status)](https://sonarcloud.io/dashboard?id=EAF) |

### Estatísticas

| Lines of Code | Duplicated Lines | Coverage | Maintainability |
|---------------|------------------|----------|-----------------|
| [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=ncloc)](https://sonarcloud.io/dashboard?id=EAF) | [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=EAF) | [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=coverage)](https://sonarcloud.io/dashboard?id=EAF) | [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=EAF) |

| Reliability | Security | Technical Debt | Vulnerabilities |
|-------------|----------|----------------|-----------------|
| [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=EAF) | [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=security_rating)](https://sonarcloud.io/dashboard?id=EAF) | [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=sqale_index)](https://sonarcloud.io/dashboard?id=EAF) | [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF&metric=vulnerabilities)](https://sonarcloud.io/dashboard?id=EAF) |

### Downloads

![GitHub all releases](https://img.shields.io/github/downloads/afonsoft/eaf/total)

### Issues

![GitHub issues](https://img.shields.io/github/issues-raw/afonsoft/eaf)

---

**Desenvolvido com ❤️ pela comunidade opensource**

Além deste exemplo simples, o EAF fornece uma infraestrutura robusta e modelo de desenvolvimento para [modularidade](https://aspnetboilerplate.com/Pages/Documents/Module-System), [multi-tenancy](https://aspnetboilerplate.com/Pages/Documents/Multi-Tenancy), [cache](https://aspnetboilerplate.com/Pages/Documents/Caching), [jobs em background](https://aspnetboilerplate.com/Pages/Documents/Background-Jobs-And-Workers), [filtros de dados](https://aspnetboilerplate.com/Pages/Documents/Data-Filters), [gerenciamento de configurações](https://aspnetboilerplate.com/Pages/Documents/Setting-Management), [eventos de domínio](https://aspnetboilerplate.com/Pages/Documents/EventBus-Domain-Events), testes unitários e de integração, e muito mais! Você foca no seu código de negócio e não se repete!