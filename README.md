# Enterprise Application Foundation (EAF)

[![GitHub](https://img.shields.io/github/license/afonsoft/eaf)](LICENSE) [![GitHub version](https://badge.fury.io/gh/afonsoft%2Feaf.svg)](https://badge.fury.io/gh/afonsoft%2Feaf) [![Commits History](https://img.shields.io/badge/Commits-History-critical)](https://github.com/afonsoft/EAF/commits/main/) [![SonarCloud](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) [![DeepWiki](https://img.shields.io/badge/DeepWiki-afonsoft%2FEAF-blue?logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyNCIgaGVpZ2h0PSIyNCIgdmlld0JveD0iMCAwIDI0IDI0IiBmaWxsPSJub25lIiBzdHJva2U9IiNmZmZmZmYiIHN0cm9rZS13aWR0aD0iMiIgc3Ryb2tlLWxpbmVjYXA9InJvdW5kIiBzdHJva2UtbGluZWpvaW49InJvdW5kIj48cGF0aCBkPSJNNCAxOWguMDEiLz48cGF0aCBkPSJNMjAgMTEuMmMuNy40IDEuMSAxIDEuMSAxLjggMCAuNS0uMyAxLjEtLjcgMS41bC0zLjEgMy4xYy0uNS41LTEgLjctMS42LjdsLS44LS4xLTEuNC0uNS0xLjgtMS4xIi8+PHBhdGggZD0iTTQuMyAxNS4zYy0uNC0uNy0uNS0xLjUtLjMtMi4yLjItLjguNy0xLjQgMS4zLTEuOGwxLjgtMS4yYy43LS40IDEuNS0uNiAyLjItLjQuOC4yIDEuNS43IDEuOSAxLjMiLz48cGF0aCBkPSJNOCA1YzAtLjUuMi0xIC42LTEuNEM5IDMuMiA5LjUgMyAxMCAzaDRjLjUgMCAxIC4yIDEuNC42LjQuNC42LjkuNiAxLjR2M2MwIC41LS4yIDEtLjYgMS40LS40LjQtLjkuNi0xLjQuNmgtNGMtLjUgMC0xLS4yLTEuNC0uNkM4LjIgOSA4IDguNSA4IDgiLz48L3N2Zz4=)](https://deepwiki.com/afonsoft/EAF)

English | **[Português](README_pt.md)**

![Line Coverage](https://img.shields.io/badge/Line%20Coverage-97.9%25-brightgreen)
![Branch Coverage](https://img.shields.io/badge/Branch%20Coverage-90.5%25-brightgreen)
![Method Coverage](https://img.shields.io/badge/Method%20Coverage-99.8%25-brightgreen)
![Test Success Rate](https://img.shields.io/badge/Test%20Success%20Rate-100%25-brightgreen)
![Total Tests](https://img.shields.io/badge/Total%20Tests-4604-blue)
![Passing Tests](https://img.shields.io/badge/Passing%20Tests-4603-brightgreen)
![Build Warnings](https://img.shields.io/badge/Build%20Warnings-162-yellow)
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
| **Angular** | 20 | Supported |
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
- **Angular 20**: SPA framework
- **Node.js 20.20.2**: JavaScript runtime
- **TypeScript 5.8.3**: Main language
- **ngx-bootstrap 12.0.0** / **Bootstrap 5**: CSS framework
- **PrimeNG 17**: UI components
- **Chart.js 4.4.7**: Charts and visualizations
- **RxJS 7.8.0**: Reactive programming

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
- Node.js 20.20.2 (for frontend development)
- Git

**For Frontend Development:**
```bash
npm install -g @angular/cli@20.3.32
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

### Latest Test Results (2026-07-15)

| Metric | Value |
|---|---|
| **Line coverage** | 97.9% |
| **Branch coverage** | 90.5% |
| **Method coverage** | 99.8% |
| **Total tests** | 4604 |
| **Passed** | 4603 |
| **Skipped** | 1 |
| **Failed** | 0 |

### Coverage by Module

| Module | Tests | Line Coverage | Status |
|---|---|---|---|
| **Eaf.Castle.Serilog** | 73 | 100% | ✅ Excellent |
| **Eaf.SqlServerCache** | 100 | 100% | ✅ Excellent |
| **Eaf.KeyVault.AspNetCore** | 10 | 100% | ✅ Excellent |
| **Eaf.Middleware.Application** | 1507 | 99.9% | ✅ Excellent |
| **Eaf.Middleware.Core** | 1265 | 99.9% | ✅ Excellent |
| **Eaf.Middleware.Web.Core** | 769 | 96.9% | ✅ Excellent |
| **Eaf.Middleware.Worker** | 191 | 100% | ✅ Excellent |
| **Eaf.SqliteCache** | 162 | 98% | ✅ Excellent |
| **Eaf.OpenTelemetry** | 67 | 100% | ✅ Excellent |
| **Eaf.KeyVault** | 243 | 100% | ✅ Excellent |
| **Eaf.Log4NetServiceBus** | 52 | 96.0% | ✅ Good |
| **Eaf.Middleware.AzureActiveDirectory** | 66 | 95.3% | ✅ Excellent |
| **Eaf.Middleware.Ldap** | 99 | 66.3% | ⚠️ Needs improvement |

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

[![SonarCloud](https://sonarcloud.io/images/project_badges/sonarcloud-black.svg)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2)

| Code Smell | Bugs | Tests | Lang | Quality |
|------------|------|-------|------|---------|
| [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=code_smells)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=bugs)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) | ![AppVeyor tests](https://img.shields.io/appveyor/tests/afonsoft/eaf) | ![GitHub top language](https://img.shields.io/github/languages/top/afonsoft/eaf) | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=afonsoft_EAF2&metric=alert_status)](https://sonarcloud.io/project/overview?id=afonsoft_EAF2) |

### Statistics

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

**Developed with ❤️ by the opensource community**

Beyond this simple example, EAF provides a robust infrastructure and development model for [modularity](https://aspnetboilerplate.com/Pages/Documents/Module-System), [multi-tenancy](https://aspnetboilerplate.com/Pages/Documents/Multi-Tenancy), [cache](https://aspnetboilerplate.com/Pages/Documents/Caching), [background jobs](https://aspnetboilerplate.com/Pages/Documents/Background-Jobs-And-Workers), [data filters](https://aspnetboilerplate.com/Pages/Documents/Data-Filters), [setting management](https://aspnetboilerplate.com/Pages/Documents/Setting-Management), [domain events](https://aspnetboilerplate.com/Pages/Documents/EventBus-Domain-Events), unit and integration tests, and much more! You focus on your business code and don't repeat yourself!

---

## Star History

[![Star History Chart](https://api.star-history.com/chart?repos=afonsoft/eaf&type=date&legend=top-left&sealed_token=LuAl7DTwrVSZjyWlqewFeoezq4tojGQ6ESqMVSmAJErLd2FM9PStjfERSyqaN3tSXTNTVQ02MXxKOq5_hG9N_W8hyMGZqr2uFrlblerV0uAcAHU1LRvzog)](https://www.star-history.com/?repos=afonsoft%2Feaf&type=date&legend=top-left)

## StarMapper

[![StarMapper](https://img.shields.io/badge/StarMapper-afonsoft%2Feaf-blue)](https://starmapper.bruniaux.com/afonsoft/eaf)

> StarMapper also requires a GitHub token to fetch star geolocation data. The live map image is not available until the repository is scanned.