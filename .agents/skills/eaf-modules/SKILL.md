---
name: eaf-modules
description: Expert guidance for developing, maintaining, and extending EAF (Enterprise Application Foundation) middleware modules. Covers all 13 modules including Core, Application, Web.Core, AzureActiveDirectory, Ldap, SqlServerCache, SqliteCache, KeyVault, KeyVault.AspNetCore, OpenTelemetry, Castle.Serilog, Worker, and Log4NetServiceBus. Use this skill when working with EAF middleware modules, creating new modules, updating module dependencies, configuring module initialization, implementing README.md files, or troubleshooting module-specific issues. Do NOT use for general .NET development, non-EAF ABP projects, or frontend development.
---

# EAF Modules Development Skill

You are an expert in EAF (Enterprise Application Foundation) middleware modules. You develop, maintain, and extend the 13 EAF modules located in `src/` directory. You write functional, maintainable, performant, and scalable code following EAF and .NET best practices.

## Project Context

EAF is an open source middleware platform built on ASP.NET Boilerplate (ABP). The middleware modules provide enterprise-grade functionality for ABP-based applications.

### Technology Stack
- **.NET Version**: 10.0
- **ABP Version**: 10.4.0
- **Database**: SQL Server and SQLite support
- **Architecture**: N-Layer Architecture (Domain, Application, Infrastructure layers)

### EAF Middleware Modules

#### Core Middleware Modules
1. **Eaf.Middleware.Core** - Core middleware abstractions, entities, services, configuration, authorization, auditing
2. **Eaf.Middleware.Application** - Application layer middleware with DTOs, services, validation
3. **Eaf.Middleware.Web.Core** - Web core middleware for ASP.NET Core (startup, middleware, filters, HTTP)

#### Authentication & Authorization Modules
4. **Eaf.Middleware.AzureActiveDirectory** - Azure Active Directory integration for external authentication
5. **Eaf.Middleware.Ldap** - LDAP/Active Directory authentication integration

#### Cache & Persistence Modules
6. **Eaf.SqlServerCache** - SQL Server distributed cache for high availability scenarios
7. **Eaf.SqliteCache** - SQLite local cache for development and low-scale scenarios

#### Security Modules
8. **Eaf.KeyVault** - Secret management supporting Azure Key Vault and Oracle Cloud Infrastructure (OCI)
9. **Eaf.KeyVault.AspNetCore** - ASP.NET Core integration for automatic configuration loading

#### Observability Modules
10. **Eaf.OpenTelemetry** - Complete OpenTelemetry implementation for distributed telemetry, tracing, metrics
11. **Eaf.Castle.Serilog** - Logging adapter integrating Castle Windsor with Serilog

#### Processing Modules
12. **Eaf.Middleware.Worker** - Background services (Worker Services) for async processing, scheduled jobs
13. **Eaf.Log4NetServiceBus** - Azure Service Bus integration using log4net for message logging

## Module Development Patterns

### Module Structure

Each EAF module follows this structure:
```
Eaf.ModuleName/
├── ModuleName/              # Module-specific implementations
├── IModuleName.cs          # Main interface (if applicable)
├── ModuleName.cs           # Main implementation
├── EafModuleNameModule.cs # ABP module definition
└── Eaf.ModuleName.csproj   # Project file with PackageReadmeFile
```

### Module Initialization

```csharp
[DependsOn(
    typeof(AbpKernelModule),
    typeof(AbpAutoMapperModule)
)]
public class EafMyModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.EafMyModule().EnableFeature = true;
        Configuration.Modules.EafMyModule().CacheDuration = TimeSpan.FromMinutes(30);
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }

    public override void PostInitialize()
    {
        // Post-initialization logic
    }
}
```

### README.md Requirement

Each module MUST have a README.md file that includes:
- Description of the module
- Relationship to EAF and ABP
- Dependencies (ABP modules and external packages)
- Main components
- Installation guide (NuGet and project reference)
- Basic usage examples
- Module structure
- Optional configurations
- Testing information
- License and support information

### .csproj Configuration

Each module's .csproj MUST include:
```xml
<PropertyGroup>
    <RootNamespace>Eaf</RootNamespace>
    <TargetFrameworks>net10.0</TargetFrameworks>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <AssemblyName>Eaf.ModuleName</AssemblyName>
    <PackageId>Eaf.ModuleName</PackageId>
    <PackageTags>asp.net;asp.net mvc;application framework;web framework;framework;domain driven design;Eaf;Boilerplate</PackageTags>
    <Description>Enterprise Application Foundation - Module Description</Description>
    <PackageReadmeFile>README.md</PackageReadmeFile>
</PropertyGroup>

<ItemGroup>
    <None Include="README.md" Pack="true" PackagePath="\" />
</ItemGroup>
```

## Module-Specific Guidelines

### Eaf.Middleware.Core

**Purpose**: Core middleware with domain entities, services, configuration, authorization, auditing

**Key Components**:
- Authorization: Permission and role management
- Auditing: Automatic operation tracking
- Configuration: Multi-tenant settings system
- Chat: Real-time chat system
- Hangfire: Advanced background job configuration with Serilog and Console
- Editions: Edition management
- Features: Feature management system
- Friendships: User friendship management

**Dependencies**:
- Abp.ZeroCore
- Abp.AutoMapper
- Abp.MailKit
- Abp.HangFire.AspNetCore
- Eaf.Middleware.AzureActiveDirectory
- Eaf.Middleware.Ldap

### Eaf.Middleware.Application

**Purpose**: Application layer with DTOs, application services, validation, business logic

**Key Components**:
- DTOs (Data Transfer Objects)
- Application Services
- Validation
- Business logic intermediation between Core and Web layers

**Dependencies**:
- Eaf.Middleware.Core
- Abp.AutoMapper
- EPPlus
- AutoMapper
- MailKit

### Eaf.Middleware.Web.Core

**Purpose**: Web components for ASP.NET Core including startup, middleware, filters

**Key Components**:
- Startup configuration
- Middleware pipeline
- Filters
- Controllers
- SignalR integration
- Swagger/OpenAPI
- Health checks

**Dependencies**:
- Eaf.Middleware.Application
- Eaf.SqlServerCache
- Abp.AspNetCore
- Swashbuckle
- Serilog sinks

### Eaf.Middleware.AzureActiveDirectory

**Purpose**: Azure Active Directory integration for external authentication

**Key Components**:
- AzureActiveDirectoryAuthenticationSource
- AzureActiveDirectorySettings
- Microsoft Graph API integration
- OpenID Connect and OAuth 2.0 support
- Automatic user synchronization

**Dependencies**:
- Abp
- Abp.Zero.Common
- Microsoft.Identity.Web
- Microsoft.Identity.Web.MicrosoftGraph
- Microsoft.Graph

### Eaf.Middleware.Ldap

**Purpose**: LDAP/Active Directory authentication integration

**Key Components**:
- LdapAuthenticationSource
- LdapSettings
- Support for Active Directory and other LDAP servers
- User attribute mapping

**Dependencies**:
- Abp
- Abp.Zero.Common
- Novell.Directory.Ldap.NET
- System.DirectoryServices
- System.DirectoryServices.AccountManagement
- System.DirectoryServices.Protocols

### Eaf.SqlServerCache

**Purpose**: SQL Server distributed cache for high availability

**Key Components**:
- EafSqlServerCache implementation
- Cache manager integration
- SQL Server backend support

**Dependencies**:
- Abp
- ExtendedXmlSerializer
- Microsoft.Extensions.Caching.SqlServer
- Microsoft.IdentityModel.JsonWebTokens
- System.IdentityModel.Tokens.Jwt

### Eaf.SqliteCache

**Purpose**: SQLite local cache for development and low-scale scenarios

**Key Components**:
- EafSqliteCache implementation
- DbCommandPool for connection pooling
- Cache manager integration
- SQLite backend support

**Dependencies**:
- Abp
- ExtendedXmlSerializer
- SQLitePCLRaw.bundle_green
- Microsoft.Data.Sqlite.Core
- Microsoft.Data.Sqlite

### Eaf.KeyVault

**Purpose**: Secret management supporting Azure Key Vault and OCI

**Key Components**:
- IKeyVaultSecretManager interface
- KeyVaultSecretManager implementation
- Azure Key Vault support
- Oracle Cloud Infrastructure (OCI) Vault support
- Local cache for performance
- Automatic retries on failures

**Dependencies**:
- Abp
- Microsoft.Extensions.Hosting.Abstractions
- Azure.Identity
- Azure.Security.KeyVault.Secrets
- Azure.Extensions.AspNetCore.Configuration.Secrets
- OCI.DotNetSDK.Identity
- OCI.DotNetSDK.Secrets
- Microsoft.IdentityModel.JsonWebTokens
- System.IdentityModel.Tokens.Jwt

### Eaf.KeyVault.AspNetCore

**Purpose**: ASP.NET Core integration for KeyVault

**Key Components**:
- Configuration builder integration
- Automatic secret loading on startup
- DI registration

**Dependencies**:
- Eaf.KeyVault
- Abp
- Microsoft.Extensions.Hosting.Abstractions

### Eaf.OpenTelemetry

**Purpose**: Complete OpenTelemetry implementation for observability

**Key Components**:
- ASP.NET Core instrumentation
- Entity Framework Core instrumentation
- HTTP client instrumentation
- Hangfire instrumentation
- Runtime metrics
- Multiple exporters (OTLP, Prometheus, Console)

**Dependencies**:
- Abp
- Abp.AspNetCore
- OpenTelemetry
- OpenTelemetry.Api
- OpenTelemetry.Extensions.Hosting
- OpenTelemetry.Instrumentation.AspNetCore
- OpenTelemetry.Instrumentation.EntityFrameworkCore
- OpenTelemetry.Instrumentation.Http
- OpenTelemetry.Instrumentation.Runtime
- OpenTelemetry.Instrumentation.Hangfire
- OpenTelemetry.Exporter.Prometheus.AspNetCore
- OpenTelemetry.Exporter.OpenTelemetryProtocol
- OpenTelemetry.Exporter.Console

### Eaf.Castle.Serilog

**Purpose**: Logging adapter integrating Castle Windsor with Serilog

**Key Components**:
- SerilogLoggerFactory
- SerilogLogger implementation
- Castle Windsor integration
- Structured logging support

**Dependencies**:
- Abp
- Castle.Core
- Castle.Windsor
- Castle.LoggingFacility
- Serilog
- Serilog.Sinks.Console
- Serilog.Sinks.File

### Eaf.Middleware.Worker

**Purpose**: Background services (Worker Services) for async processing

**Key Components**:
- Worker service base classes
- Background job management
- Email integration
- Folder operations

**Dependencies**:
- Abp
- Abp.AutoMapper
- Abp.MailKit
- Abp.Zero.Common
- Serilog.Sinks.Console
- Serilog.Sinks.File
- MimeKit
- Castle.LoggingFacility

### Eaf.Log4NetServiceBus

**Purpose**: Azure Service Bus integration using log4net

**Key Components**:
- Service Bus logging integration
- Message logging
- Event logging

**Dependencies**:
- Abp
- Castle.Core
- log4net
- Microsoft.Azure.ServiceBus
- Newtonsoft.Json
- System.IdentityModel.Tokens.Jwt

## Common Patterns Across Modules

### Dependency Injection

```csharp
// Transient (created each time)
public class MyService : ITransientDependency
{
    private readonly IRepository<User, Guid> _userRepository;
    
    public MyService(IRepository<User, Guid> userRepository)
    {
        _userRepository = userRepository;
    }
}

// Singleton (single instance)
public class MySingletonService : ISingletonDependency
{
    // ...
}

// Scoped (per request)
public class MyScopedService : IScopedDependency
{
    // ...
}
```

### Configuration Management

```csharp
// Module configuration
public override void PreInitialize()
{
    Configuration.Modules.EafMyModule().EnableFeature = true;
    Configuration.Modules.EafMyModule().CacheDuration = TimeSpan.FromMinutes(30);
    Configuration.Modules.EafMyModule().MaxRetries = 3;
}

// Accessing settings
public class MyService : ITransientDependency
{
    private readonly ISettingManager _settingManager;
    
    public async Task<string> GetSettingAsync()
    {
        return await _settingManager.GetSettingValueAsync("Eaf.MyModule.MySetting");
    }
}
```

### Logging

```csharp
public class MyService : ITransientDependency
{
    private readonly ILogger _logger;
    
    public MyService(ILogger logger)
    {
        _logger = logger;
    }
    
    public void DoSomething()
    {
        _logger.Info("Starting operation...");
        try
        {
            // Operation
            _logger.Info("Operation completed successfully");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Operation failed");
            throw;
        }
    }
}
```

### Caching

```csharp
public class MyService : ITransientDependency
{
    private readonly ICacheManager _cacheManager;
    
    public async Task<User> GetUserAsync(Guid id)
    {
        var cache = _cacheManager.GetCache<User>("Users");
        return await cache.GetAsync(id.ToString(), () => GetUserFromDb(id));
    }
}
```

## Testing

### Unit Tests

```csharp
public class EafMyModule_Tests : AbpIntegratedTestBase
{
    private readonly IMyService _myService;
    
    public EafMyModule_Tests()
    {
        _myService = Resolve<IMyService>();
    }
    
    [Fact]
    public async Task Should_Do_Something()
    {
        // Arrange
        var input = new MyInput { Value = "test" };
        
        // Act
        var result = await _myService.DoSomethingAsync(input);
        
        // Assert
        result.ShouldNotBeNull();
        result.Value.ShouldBe("expected");
    }
}
```

### BDD Pattern

Follow the BDD pattern in Portuguese for test naming:
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
    resultado.Sucesso.ShouldBe(true);
}
```

## Best Practices

### Code Quality
- Add XML documentation to all public methods and classes
- Follow C# naming conventions (PascalCase for classes, camelCase for parameters)
- Keep methods small and focused
- Use async/await for I/O operations
- Dispose of IDisposable objects properly

### Performance
- Use caching for frequently accessed data
- Optimize database queries with projections
- Use IQueryable for database-side operations
- Consider background jobs for long-running tasks

### Security
- Never hardcode secrets or connection strings
- Use ISettingManager for configuration
- Validate all user inputs
- Use authorization attributes appropriately
- Log security-relevant events

### Documentation
- Each module MUST have a README.md
- Document complex business logic
- Provide usage examples
- Keep documentation up-to-date with code changes

## Common Issues and Solutions

### Module Dependencies
- Always check that required modules are listed in [DependsOn]
- Use interface-based dependencies to reduce coupling
- Consider circular dependencies when designing modules

### Configuration Issues
- Provide sensible defaults for all configuration options
- Document all configuration options in README.md
- Validate configuration values on module initialization

### Performance Issues
- Profile before optimizing
- Use caching strategically
- Consider async operations for I/O-bound work
- Monitor memory usage in long-running services

## When in Doubt

- Follow ABP conventions over custom patterns
- Check existing EAF modules for patterns
- Maintain consistency across modules
- Test thoroughly before committing
- Update README.md when adding features
- Ensure PackageReadmeFile is configured in .csproj
