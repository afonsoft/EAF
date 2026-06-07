# Enterprise Application Foundation (EAF)

[![GitHub](https://img.shields.io/github/license/afonsoft/eaf)](LICENSE) [![GitHub version](https://badge.fury.io/gh/afonsoft%2Feaf.svg)](https://badge.fury.io/gh/afonsoft%2Feaf) [![Commits History](https://img.shields.io/badge/Commits-History-critical)](https://github.com/afonsoft/EAF/commits/main/) [![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) [![DeepWiki](https://img.shields.io/badge/DeepWiki-afonsoft%2FEAF-blue?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyNCIgaGVpZ2h0PSIyNCIgdmlld0JveD0iMCAwIDI0IDI0IiBmaWxsPSJub25lIiBzdHJva2U9IiNmZmZmZmYiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIiBzdHJva2UtbGluZWpvaW49InJvdW5kIj48cGF0aCBkPSJNNCAxOWguMDEiLz48cGF0aCBkPSJNMjAgMTEuMmMuNy40IDEuMSAxIDEuMSAxLjggMCAuNS0uMyAxLjEtLjcgMS41bC0zLjEgMy4xYy0uNS41LTEgLjctMS42LjdsLS44LS4xLTEuNC0uNS0xLjgtMS4xIi8+PHBhdGggZD0iTTQuMyAxNS4zYy0uNC0uNy0uNS0xLjUtLjMtMi4yLjItLjguNy0xLjQgMS4zLTEuOGwxLjgtMS4yYy43LS40IDEuNS0uNiAyLjItLjQuOC4yIDEuNS43IDEuOSAxLjMiLz48cGF0aCBkPSJNOCA1YzAtLjUuMi0xIC42LTEuNEM5IDMuMiA5LjUgMyAxMCAzaDRjLjUgMCAxIC4yIDEuNC42LjQuNC42LjkuNiAxLjR2M2MwIC41LS4yIDEtLjYgMS40LS40LjQtLjkuNi0xLjQuNmgtNGMtLjUgMC0xLS4yLTEuNC0uNkM4LjIgOSA4IDguNSA4IDgiLz48L3N2Zz4=)](https://deepwiki.com/afonsoft/EAF)

English | **[Português](README_pt.md)**

![Line Coverage](https://img.shields.io/badge/Line%20Coverage-19%25-yellow)
![Branch Coverage](https://img.shields.io/badge/Branch%20Coverage-13.1%25-red)
![Method Coverage](https://img.shields.io/badge/Method%20Coverage-41.7%25-yellow)
![Test Success Rate](https://img.shields.io/badge/Test%20Success%20Rate-100%25-brightgreen)
![Total Tests](https://img.shields.io/badge/Total%20Tests-1289-blue)
![Passing Tests](https://img.shields.io/badge/Passing%20Tests-1289-brightgreen)
![Angular Tests](https://img.shields.io/badge/Angular%20Tests-222%20Passed-brightgreen)
![API Template Tests](https://img.shields.io/badge/API%20Template%20Tests-212%20Total-blue)
![API Template Passing](https://img.shields.io/badge/API%20Template%20Passing-211%20Success-brightgreen)

## Table of Contents

- [About the Project](#-about-the-project)
- [ASP.NET Boilerplate](#-aspnet-boilerplate)
- [Technical Overview](#-technical-overview)
- [Installation and Configuration](#-installation-and-configuration)
- [Running and Testing](#-running-and-testing)
- [Code Coverage](#-code-coverage)
- [NuGet Packages](#-nuget-packages)
- [Contributing](#-contributing)


## About the Project

### What is EAF?

The **EAF (Enterprise Application Foundation)** is an open-source middleware platform that provides a solid foundation for developing modern applications. Based on ASP.NET Boilerplate (ABP), EAF has been optimized to work with the latest versions of ASP.NET Core and Entity Framework Core.

### Key Benefits

- **Integrated Security**: Authentication and authorization with support for Azure Active Directory and LDAP
- **Complete Auditing**: Automatic tracking of all system operations
- **Multi-tenancy**: Native support for multi-tenant applications
- **Observability**: Integration with OpenTelemetry for monitoring and telemetry
- **Distributed Cache**: Support for Redis, SQL Server, and SQLite
- **Secret Management**: Integration with Azure Key Vault and Oracle Cloud Infrastructure
- **Advanced Logging**: Replacement of log4net with Serilog for better performance

### Use Cases

- **Web Applications**: Management systems, CRMs, ERPs, and other applications
- **RESTful APIs**: Development of scalable APIs
- **Microservices**: Foundation for microservice architectures
- **Multi-tenant Applications**: SaaS and shared applications

---

## ASP.NET Boilerplate

### What is ASP.NET Boilerplate?

**ASP.NET Boilerplate (ABP)** is an open-source web application framework that provides a robust infrastructure for developing modern applications. Complete documentation available at: [https://aspnetboilerplate.com/Pages/Documents](https://aspnetboilerplate.com/Pages/Documents)

### EAF: Enhanced Open Source Implementation

The **EAF (Enterprise Application Foundation)** is an open-source implementation based on ASP.NET Boilerplate, designed to offer a more user-friendly interface for API and UI development. EAF complements the base framework with various improvements and additional modules:

### Modules and Improvements

#### Authentication and Authorization
- **External Login**: Support for social login (Google, Facebook, Twitter, Microsoft)
- **Azure Active Directory**: Complete integration
- **LDAP/Active Directory**: Authentication via directories
- **Two-Factor Authentication**: Two-factor authentication
- **Permission Management**: Granular permission and role system

#### Auditing and Logging
- **Automatic Auditing**: Tracking of all CRUD operations
- **Structured Logging**: Integration with Serilog for detailed logs
- **Entity Change Tracking**: Monitoring of entity changes
- **Error Logging**: Capture and analysis of exceptions

#### Real-time Communication
- **Chat System**: Chat system between users
- **SignalR Integration**: WebSockets for bidirectional communication
- **Push Notifications**: Real-time notification system
- **Tenant-to-Host Chat**: Chat between tenants and host
- **Group Chat**: Group chat for collaboration

#### Multi-Tenancy
- **Data Isolation**: Complete data separation by tenant
- **Tenant Management**: Tenant management
- **Tenant Resolution**: Automatic tenant resolution
- **Feature Management**: Enable/disable features by tenant

#### Cache and Performance
- **Distributed Cache**: Support for Redis, SQL Server, SQLite
- **Cache Abstraction**: Unified interface for different backends
- **Cache Manager**: Intelligent cache management
- **Performance Optimization**: Integrated performance optimizations

#### Background Jobs
- **Hangfire Integration**: Background task processing
- **Job Management**: Job scheduling and monitoring
- **Recurring Jobs**: Automated recurring tasks
- **Worker Services**: Scalable background services

#### UI and Frontend
- **Angular Integration**: Complete Angular template
- **UI Components**: Reusable and styled components
- **Client-Side Validation**: Automatic frontend validation
- **Internationalization**: Support for multiple languages

#### Configuration and Settings
- **Setting Management**: Configuration management
- **Feature Flags**: Feature flags
- **Environment Configuration**: Configuration by environment
- **Key Vault Integration**: Secret security

#### Other Features
- **Event Bus**: Domain event system
- **Data Filters**: Automatic data filters (SoftDelete, TenantId)
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management
- **Dependency Injection**: Configured dependency injection
- **Object Mapping**: Integrated AutoMapper
- **API Documentation**: Automatic Swagger/OpenAPI

### Benefits of EAF over Pure ABP

1. **More User-Friendly Interface**: Simplified and intuitive APIs
2. **Ready-to-Use Modules**: Pre-configured components for immediate use
3. **Best Practices**: Applied modern development patterns
4. **Optimized Performance**: Integrated performance optimizations
5. **Portuguese Documentation**: Native support for Portuguese language
6. **BDD Tests**: Tests with Given/When/Then pattern
7. **Observability**: OpenTelemetry for advanced monitoring
8. **Enhanced Security**: Multiple authentication and authorization options

---

## Technical Overview

### Architecture

EAF follows Domain-Driven Design (DDD) principles and implements patterns such as:

- **Repository Pattern**: Data layer abstraction
- **Unit of Work**: Transaction management
- **Dependency Injection**: Inversion of control
- **CQRS**: Separation of commands and queries
- **Event Sourcing**: Domain event tracking

### Supported Technologies

| Technology | Version | Status |
|------------|--------|--------|
| **ASP.NET Core** | 10.0 | Supported |
| **Entity Framework Core** | 10.0 | Supported |
| **Angular** | 19 | Supported |
| **.NET** | 10.0 | Supported |

### Main Components

#### Middleware Core
- **Eaf.Middleware.Core**: Central domain layer with entities, services, configurations, authorization, auditing, and base framework features.
- **Eaf.Middleware.Application**: Application layer with DTOs, application services, validations, and intermediate business logic.
- **Eaf.Middleware.Web.Core**: Web components for ASP.NET Core including startup configuration, middleware, filters, and HTTP integration.

#### Authentication and Authorization
- **Eaf.Middleware.AzureActiveDirectory**: Complete integration with Azure Active Directory for external authentication and user synchronization.
- **Eaf.Middleware.Ldap**: LDAP/Active Directory authentication for integration with existing directories.

#### Cache and Persistence
- **Eaf.SqlServerCache**: Distributed cache implementation using SQL Server as backend for high availability scenarios.
- **Eaf.SqliteCache**: Local cache implementation using SQLite for development and low-scale scenarios.

#### Security
- **Eaf.KeyVault**: Secret management supporting Azure Key Vault and Oracle Cloud Infrastructure (OCI) for secure credential storage.
- **Eaf.KeyVault.AspNetCore**: ASP.NET Core integration for automatic loading of configurations and secrets from Key Vault.

#### Observability
- **Eaf.OpenTelemetry**: Complete OpenTelemetry implementation for distributed telemetry, tracing, and metrics with support for multiple exporters.
- **Eaf.Castle.Serilog**: Logging adapter integrating Castle Windsor with Serilog for structured and configurable logging.

#### Processing
- **Eaf.Middleware.Worker**: Background services (Worker Services) for asynchronous processing, scheduled jobs, and long-running tasks.
- **Eaf.Log4NetServiceBus**: Integration with Azure Service Bus using log4net for message logging and messaging events.

---

## Tech Stack

### Backend (.NET)
- **.NET 10.0**: Main framework
- **ASP.NET Core 10.0**: Web API and MVC
- **Entity Framework Core 10.0**: ORM for data access
- **AutoMapper**: Object mapping
- **Castle Windsor**: Dependency injection
- **Hangfire**: Background task processing
- **SignalR**: Real-time communication
- **Swagger/OpenAPI**: API documentation
- **xUnit**: Testing framework
- **Shouldly**: Fluent assertions
- **NSubstitute**: Mocking framework

### Frontend (Template)
- **Angular 19**: SPA framework
- **Node.js 20.20.0**: JavaScript runtime
- **TypeScript 5.2**: Main language
- **Bootstrap 5**: CSS framework
- **PrimeNG 17**: UI components
- **Chart.js**: Charts and visualizations
- **RxJS 7**: Reactive programming

### Infrastructure
- **SQLite**: Local database
- **SQL Server**: Main database
- **Redis**: Distributed cache
- **Azure Key Vault**: Secret management
- **OpenTelemetry**: Observability
- **Serilog**: Structured logging

---

## Documentation

The detailed technical documentation of the EAF system, covering architecture, modules, development guides, and more, can be found in our documentation portal.

[Access Complete Documentation](./docs/README.md) | [DeepWiki - AI-Powered Docs](https://deepwiki.com/afonsoft/EAF)

---

## Installation and Configuration

### Prerequisites

**Required:**
- .NET 10.0 SDK or higher
- Node.js 20.20.0 (for frontend development)
- Git

**For Frontend Development:**
```bash
npm install -g @angular/cli@19
```

**For Coverage Reports:**
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

### Platform Compatibility

| Platform | Status | Notes |
|------------|--------|-------|
| **Windows** | Full Support | Use PowerShell or Command Prompt |
| **Linux** | Full Support | Bash scripts provided |
| **macOS** | Full Support | Use Terminal with bash |

### Cloning the Repository

```bash
git clone https://github.com/afonsoft/EAF.git
cd EAF
```

### Environment Setup

1. **Restore dependencies:**
```bash
dotnet restore Eaf.sln
```

2. **Build the project:**
```bash
dotnet build Eaf.sln
```

---

## Running and Testing

### Quick Start

**Linux/macOS:**
```bash
# Make the script executable
chmod +x build-and-test.sh

# Run build and tests with coverage
./build-and-test.sh
```

**Windows (PowerShell):**
```powershell
# Run build and tests
dotnet build Eaf.sln
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Generate coverage report (if reportgenerator is installed)
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;TextSummary"
```

### Running Only Tests

**Linux/macOS:**
```bash
# Make the script executable
chmod +x run-tests-with-coverage.sh

# Run all tests with coverage
./run-tests-with-coverage.sh
```

**Windows (PowerShell):**
```powershell
# Run all tests with coverage
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Generate coverage report
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;TextSummary"
```

### Manual Test Execution

```bash
# Run a specific test project with coverage
dotnet test test/Eaf.KeyVault.Tests/Eaf.KeyVault.Tests.csproj --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Run all tests in the solution
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Generate coverage report
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"TestResults/CoverageReport" -reporttypes:"Html;Badges;TextSummary"
```

### Usage Example

Let's examine a simple class to see the benefits of EAF:

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

This example demonstrates several EAF features:

- **Dependency Injection**: EAF uses and provides a conventional DI infrastructure
- **Repository**: EAF can create a default repository for each entity
- **Authorization**: EAF can check permissions declaratively
- **Validation**: EAF automatically checks if input is null
- **Audit Logging**: Information is automatically saved for each request
- **Unit of Work**: Each application service method is a unit of work by default

---

## Code Coverage

### Test Status (June 2026)

| Module | Tests | Line Coverage | Status |
|--------|-------|--------------|--------|
| **Eaf.Castle.Serilog** | 71 | 98.6% | ✅ Excellent |
| **Eaf.SqlServerCache** | 95 | 88.3% | ✅ Excellent |
| **Eaf.KeyVault.AspNetCore** | 10 | 88.8% | ✅ Excellent |
| **Eaf.SqliteCache** | 126 | 82.7% | ✅ Good |
| **Eaf.OpenTelemetry** | 49 | 75.4% | ✅ Good |
| **Eaf.KeyVault** | 222 | 69.4% | ✅ Good |
| **Eaf.Log4NetServiceBus** | 41 | 62.2% | ⚠️ Moderate |
| **Eaf.Middleware.Worker** | 101 | 33.3% | ⚠️ Needs improvement |
| **Eaf.Middleware.Application** | 410 | 23.6% | ⚠️ Needs improvement |
| **Eaf.Middleware.AzureActiveDirectory** | 21 | 7.4% | ❌ Low |
| **Eaf.Middleware.Ldap** | 21 | 6.0% | ❌ Low |
| **Eaf.Middleware.Web.Core** | 122 | 4.9% | ❌ Low |
| **Eaf.Middleware.Core** | — | 0.1% | ❌ Low |
| **TOTAL** | **1289** | **19%** | In progress |

### Coverage Goal
- **Target**: 90% code coverage
- **Current**: 19% line, 13.1% branch, 41.7% method
- **Backend Tests**: 1289 total, 1289 passing, 1 skipped, 0 failures (100% success rate)
- **Angular Tests**: 222 total, 222 passing (100% success rate)
- **API Template Tests**: 212 total, 211 passing, 1 skipped (EF Core context issue fixed)
- **Next steps**: Improve coverage for Middleware.Core, Middleware.Web.Core, Middleware.Ldap, and AzureActiveDirectory modules

### Implemented Improvements
- **KeyVault**: 222 BDD tests implemented (100% success, 69.4% coverage)
- **XML Documentation**: Portuguese summaries added
- **BDD Pattern**: Given/When/Then implemented
- **Castle.Serilog**: 71 BDD tests implemented (100% success, 98.6% coverage)
- **OpenTelemetry**: 49/49 tests passing (100% success, 75.4% coverage)
- **SqlServerCache**: 95/95 tests passing (100% success, 88.3% coverage)
- **SqliteCache**: 126/126 tests passing (100% success, 82.7% coverage)
- **Worker**: 101/101 tests passing (100% success, 33.3% coverage)
- **Middleware.Application**: 410/410 tests passing (100% success, 23.6% coverage)
- **Middleware.Web.Core**: 122/122 tests passing (100% success)
- **Test Expansion (June 2026)**: +12 new test files implemented (PR #63)
  - **Eaf.Middleware.Web.Core**: +10 tests (Swagger filters, TokenAuth, Impersonation models)
  - **Eaf.MiddlewareCore**: +30 tests (Entities, DTOs, Extensions, Cache items)
  - **Eaf.Middleware.Application**: +9 tests (constants, helpers, authorization)
  - **Eaf.Middleware.Ldap**: +4 tests (configuration, authentication)
  - **Eaf.Middleware.AzureActiveDirectory**: +4 tests (configuration, authentication)
  - **Eaf.Middleware.Worker**: +4 tests (folders, emailing, base classes)
  - **Eaf.KeyVault**: +3 tests (managers, interfaces)
  - **Eaf.Castle.Serilog**: +4 tests (module, factory, logger)
  - **Eaf.KeyVault.AspNetCore**: +1 test (extensions)
  - **Eaf.Log4NetServiceBus**: +3 tests (logging components)
  - **Eaf.OpenTelemetry**: +2 tests (module, extensions)
  - **Eaf.SqlServerCache**: +2 tests (helpers, extensions)
  - **Eaf.SqliteCache**: +4 tests (pool, commands, options)
- **Bug Fix (June 2026)**: EafSqliteCache.Set expiration parameters forwarded correctly (PR #64)

### Technical Fixes Implemented

#### SqliteCache - Expiration Parameters Fix (June 2026)
- **Problem**: `EafSqliteCache.Set` ignores `slidingExpireTime` and `absoluteExpireTime` parameters
- **Cause**: Parameters not forwarded to internal `CreateForSet` method
- **Solution**: Forward both parameters to `CreateForSet(cmd, key, value, slidingExpireTime, absoluteExpireTime)`
- **Result**: Cache expiration now works as expected; +14 expiration-specific tests added

#### SqliteCache - Static Initialization Fix
- **Problem**: IndexOutOfRangeException during DbCommandPool initialization
- **Cause**: Incorrect order of static property initialization
- **Solution**: Moved `Count` before `Commands` to ensure correct initialization
- **Result**: +21 tests passing (from 53 to 74), 39% improvement

#### Castle.Serilog - Complete Resolution
- **Problem**: Conflicts between Castle.Core.Logging.ILogger and Serilog.ILogger
- **Solution**: Alias `SerilogILogger` to resolve namespace ambiguity
- **Problematic Mocks**: Replaced with actual Serilog instances
- **Invalid Tests**: Removed tests that depended on unsupported runtime configuration
- **Result**: 44/44 tests passing with BDD pattern in Portuguese

### XML Documentation Status
- **Documented Files**: 6/507 (1.2%)
- **Main Documented Classes**:
  - **SerilogLoggerFactory** - Serilog logger factory
  - **SerilogLogger** - Logger implementation
  - **EafSqliteCache** - SQLite-based cache
  - **MiddlewareAppServiceBase** - Base class for services
  - **AzureActiveDirectoryAuthenticationSource** - Azure AD authentication
- **Next Modules**: Entity Framework, Web API, Authorization

### Coverage by Assembly (June 2026)

| Assembly | Line Coverage | Tests | Status |
|----------|--------------|-------|--------|
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

### Test Project Status

| Project | Status | Tests | Line Coverage | Notes |
|---------|--------|--------|--------------|-------|
| **Eaf.Middleware.Application.Tests** | ✅ Passing | 410 | 23.6% | Largest test suite |
| **Eaf.KeyVault.Tests** | ✅ Passing | 222 | 69.4% | BDD pattern |
| **Eaf.SqliteCache.Tests** | ✅ Passing | 126 | 82.7% | +14 expiration tests |
| **Eaf.Middleware.Web.Core.Tests** | ✅ Passing | 122 | 4.9% | Swagger, TokenAuth, Impersonation |
| **Eaf.Middleware.Worker.Tests** | ✅ Passing | 101 | 33.3% | Lifecycle, Background jobs |
| **Eaf.SqlServerCache.Tests** | ✅ Passing | 95 | 88.3% | Excellent coverage |
| **Eaf.Castle.Serilog.Tests** | ✅ Passing | 71 | 98.6% | BDD in Portuguese |
| **Eaf.OpenTelemetry.Tests** | ✅ Passing | 49 | 75.4% | Good coverage |
| **Eaf.Log4NetServiceBus.Tests** | ✅ Passing | 41 | 62.2% | Good coverage |
| **Eaf.Middleware.AzureActiveDirectory.Tests** | ✅ Passing | 21 | 7.4% | Basic coverage |
| **Eaf.Middleware.Ldap.Tests** | ✅ Passing | 21 | 6.0% | Basic coverage |
| **Eaf.KeyVault.AspNetCore.Tests** | ✅ Passing | 10 | 88.8% | Excellent coverage |

### Angular Template Tests

| Metric | Value |
|--------|-------|
| **Total Tests** | 222 |
| **Passing** | 222 (100%) |
| **Statement Coverage** | 11.68% |
| **Branch Coverage** | 2.56% |
| **Function Coverage** | 9.01% |
| **Line Coverage** | 11.16% |

### API Template Tests

| Metric | Value |
|--------|-------|
| **Total Tests** | 212 |
| **Passing** | 211 |
| **Failing** | 0 |
| **Skipped** | 1 |
| **Fix Applied** | `OnConfiguring` now checks provider via `optionsBuilder.Options.Extensions` instead of `Database.IsSqlServer()` |

**Legend:**
- **Passing**: All tests pass successfully
- **Problems**: Tests run but have failures or warnings
- **Build Errors**: Project fails to compile

### BDD Test Pattern

Tests follow the BDD (Behavior-Driven Development) pattern in Portuguese:

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

## NuGet Packages

| Package | NuGet | Description |
|--------|-------|-------------|
| [Eaf.Middleware.Application](https://www.nuget.org/packages/Eaf.Middleware.Application/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Application.svg)](https://badge.fury.io/nu/Eaf.Middleware.Application) | Application layer |
| [Eaf.Middleware.AzureActiveDirectory](https://www.nuget.org/packages/Eaf.Middleware.AzureActiveDirectory/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.AzureActiveDirectory.svg)](https://badge.fury.io/nu/Eaf.Middleware.AzureActiveDirectory) | Azure AD integration |
| [Eaf.Middleware.Core](https://www.nuget.org/packages/Eaf.Middleware.Core/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Core.svg)](https://badge.fury.io/nu/Eaf.Middleware.Core) | Core features |
| [Eaf.Middleware.Ldap](https://www.nuget.org/packages/Eaf.Middleware.Ldap/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Ldap.svg)](https://badge.fury.io/nu/Eaf.Middleware.Ldap) | LDAP authentication |
| [Eaf.Middleware.Web.Core](https://www.nuget.org/packages/Eaf.Middleware.Web.Core/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Web.Core.svg)](https://badge.fury.io/nu/Eaf.Middleware.Web.Core) | Web components |
| [Eaf.Castle.Serilog](https://www.nuget.org/packages/Eaf.Castle.Serilog/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Castle.Serilog.svg)](https://badge.fury.io/nu/Eaf.Castle.Serilog) | Structured logging |
| [Eaf.KeyVault](https://www.nuget.org/packages/Eaf.KeyVault/) | [![NuGet version](https://badge.fury.io/nu/Eaf.KeyVault.svg)](https://badge.fury.io/nu/Eaf.KeyVault) | Secret management |
| [Eaf.KeyVault.AspNetCore](https://www.nuget.org/packages/Eaf.KeyVault.AspNetCore/) | [![NuGet version](https://badge.fury.io/nu/Eaf.KeyVault.AspNetCore.svg)](https://badge.fury.io/nu/Eaf.KeyVault.AspNetCore) | ASP.NET Core integration |
| [Eaf.OpenTelemetry](https://www.nuget.org/packages/Eaf.OpenTelemetry/) | [![NuGet version](https://badge.fury.io/nu/Eaf.OpenTelemetry.svg)](https://badge.fury.io/nu/Eaf.OpenTelemetry) | Telemetry and observability |
| [Eaf.Log4NetServiceBus](https://www.nuget.org/packages/Eaf.Log4NetServiceBus/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Log4NetServiceBus.svg)](https://badge.fury.io/nu/Eaf.Log4NetServiceBus) | Service bus logging |
| [Eaf.SqlServerCache](https://www.nuget.org/packages/Eaf.SqlServerCache/) | [![NuGet version](https://badge.fury.io/nu/Eaf.SqlServerCache.svg)](https://badge.fury.io/nu/Eaf.SqlServerCache) | SQL Server cache |
| [Eaf.SqliteCache](https://www.nuget.org/packages/Eaf.SqliteCache/) | [![NuGet version](https://badge.fury.io/nu/Eaf.SqliteCache.svg)](https://badge.fury.io/nu/Eaf.SqliteCache) | SQLite cache |
| [Eaf.Middleware.Worker](https://www.nuget.org/packages/Eaf.Middleware.Worker/) | [![NuGet version](https://badge.fury.io/nu/Eaf.Middleware.Worker.svg)](https://badge.fury.io/nu/Eaf.Middleware.Worker) | Background services |

---

## Contributing

### How to Contribute

1. **Fork** the repository
2. **Create** a branch for your feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** your changes (`git commit -m 'Add some AmazingFeature'`)
4. **Push** to the branch (`git push origin feature/AmazingFeature`)
5. **Open** a Pull Request

### Development Standards

- **Tests**: All new features must have unit tests
- **Coverage**: Maintain minimum 90% coverage
- **Documentation**: Add XML documentation to public methods
- **BDD**: Follow Given/When/Then pattern in Portuguese

### Useful Links

- [Complete Documentation](src/README.md)
- [Security Policy](SECURITY.md)
- [Changelog](CHANGELOG.md)
- [Testing Guide](TESTING.md)

---

## Quality and Metrics

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-black.svg)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main)

| Code Smell | Bugs | Tests | Lang | Quality |
|------------|------|-------|------|---------|
| [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=code_smells)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=bugs)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) | ![AppVeyor tests](https://img.shields.io/appveyor/tests/afonsoft/eaf) | ![GitHub top language](https://img.shields.io/github/languages/top/afonsoft/eaf) | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main) |

### Statistics

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

**Developed with ❤️ by the opensource community**

Beyond this simple example, EAF provides a robust infrastructure and development model for [modularity](https://aspnetboilerplate.com/Pages/Documents/Module-System), [multi-tenancy](https://aspnetboilerplate.com/Pages/Documents/Multi-Tenancy), [cache](https://aspnetboilerplate.com/Pages/Documents/Caching), [background jobs](https://aspnetboilerplate.com/Pages/Documents/Background-Jobs-And-Workers), [data filters](https://aspnetboilerplate.com/Pages/Documents/Data-Filters), [setting management](https://aspnetboilerplate.com/Pages/Documents/Setting-Management), [domain events](https://aspnetboilerplate.com/Pages/Documents/EventBus-Domain-Events), unit and integration tests, and much more! You focus on your business code and don't repeat yourself!

---

## Star History

<a href="https://www.star-history.com/?repos=afonsoft%2Feaf&type=date&legend=top-left">
 <picture>
   <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/chart?repos=afonsoft/eaf&type=date&theme=dark&legend=top-left" />
   <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/chart?repos=afonsoft/eaf&type=date&legend=top-left" />
   <img alt="Star History Chart" src="https://api.star-history.com/chart?repos=afonsoft/eaf&type=date&legend=top-left" />
 </picture>
</a>

## StarMapper

<a href="https://starmapper.bruniaux.com/afonsoft/eaf">
  <picture>
    <source media="(prefers-color-scheme: dark)" srcset="https://starmapper.bruniaux.com/api/map-image/afonsoft/eaf?theme=dark" />
    <source media="(prefers-color-scheme: light)" srcset="https://starmapper.bruniaux.com/api/map-image/afonsoft/eaf?theme=light" />
    <img alt="StarMapper" src="https://starmapper.bruniaux.com/api/map-image/afonsoft/eaf" />
  </picture>
</a>