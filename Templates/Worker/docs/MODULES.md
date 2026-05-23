# Modules - EAF Worker Template

## Overview

The EAF Worker Template follows the N-Layer Architecture pattern with clear separation of concerns across different layers. Each layer has a specific responsibility and communicates with other layers through well-defined interfaces.

## Project Structure

```
src/
├── Eaf.ProjectName.Worker/          # Worker Service Layer
├── Eaf.ProjectName.Core/            # Domain Layer
└── Eaf.ProjectName.EntityFrameworkCore/  # Data Access Layer
```

## Domain Layer (Eaf.ProjectName.Core)

**Location**: `src/Eaf.ProjectName.Core/`

The Domain Layer contains the core business logic and entities. It has no dependencies on other layers.

### Components

#### Entities

**Location**: `Entities/`

Domain entities that represent business objects:

- `AbpUser`: Extended user entity from ABP
- `AbpRole`: Extended role entity from ABP
- `AbpTenant`: Extended tenant entity from ABP
- Custom entities specific to your application

#### Value Objects

**Location**: `ValueObjects/`

Immutable value objects that represent domain concepts:

- `EmailAddress`: Email address validation
- `PhoneNumber`: Phone number validation
- Custom value objects for your domain

#### Interfaces

**Location**: `Interfaces/`

Repository interfaces for data access:

- `IUserRepository`: User repository interface
- `IRoleRepository`: Role repository interface
- `ITenantRepository`: Tenant repository interface
- Custom repository interfaces

#### Services

**Location**: `DomainServices/`

Domain services that contain business logic not naturally fitting in entities:

- `UserManager`: User management logic
- `RoleManager`: Role management logic
- `TenantManager`: Tenant management logic
- Custom domain services

## Worker Service Layer (Eaf.ProjectName.Worker)

**Location**: `src/Eaf.ProjectName.Worker/`

The Worker Service Layer contains the background workers, jobs, and services for background processing.

### Components

#### Workers

**Location**: `Workers/`

Background workers that inherit from `BackgroundService`:

- `MyWorker`: Sample background worker
- Custom workers for your application

#### Jobs

**Location**: `Jobs/`

Hangfire jobs for scheduled tasks:

- `MyRecurringJob`: Sample recurring job
- Custom jobs for your application

#### Services

**Location**: `Services/`

Application-specific services for worker operations:

- `MyService`: Sample service
- Custom services for your application

#### Program

**Location**: `Program.cs`

Main entry point for the worker service:

- Service configuration
- Dependency injection setup
- Background worker registration

## Data Access Layer (Eaf.ProjectName.EntityFrameworkCore)

**Location**: `src/Eaf.ProjectName.EntityFrameworkCore/`

The Data Access Layer provides implementation of repositories and DbContext using Entity Framework Core.

### Components

#### DbContext

**Location**: `EafProjectNameDbContext.cs`

Main DbContext that configures Entity Framework Core:

- Entity configurations
- Relationship mappings
- Database model builder
- Query filters for multi-tenancy

#### Repositories

**Location**: `Repositories/`

Repository implementations:

- `EfCoreUserRepository`: User repository implementation
- `EfCoreRoleRepository`: Role repository implementation
- `EfCoreTenantRepository`: Tenant repository implementation
- Custom repository implementations

#### Entity Configurations

**Location**: `EntityConfigurations/`

Fluent API entity configurations:

- `UserConfiguration`: User entity configuration
- `RoleConfiguration`: Role entity configuration
- `TenantConfiguration`: Tenant entity configuration
- Custom entity configurations

#### Migrations

**Location**: `Migrations/`

Database migrations created by Entity Framework Core:

- Initial migration with base schema
- Custom migrations for schema changes

## Module Dependencies

### Dependency Rules

The EAF template follows strict dependency rules:

- **Core Layer**: No dependencies on other layers
- **Worker Layer**: Depends only on Core Layer and EntityFrameworkCore Layer
- **EntityFrameworkCore Layer**: Depends on Core Layer

### Dependency Injection

The template uses Castle Windsor for dependency injection:

- Services are registered in modules
- Scoped, transient, and singleton lifetimes
- Interceptor registration for cross-cutting concerns

## Creating New Workers

### Step 1: Define Entity in Core Layer

```csharp
// src/Eaf.ProjectName.Core/Entities/MyEntity.cs
public class MyEntity : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; set; }
    public string Description { get; set; }
}
```

### Step 2: Create Repository Interface in Core Layer

```csharp
// src/Eaf.ProjectName.Core/Interfaces/IMyEntityRepository.cs
public interface IMyEntityRepository : IRepository<MyEntity, Guid>
{
    // Custom methods
}
```

### Step 3: Implement Repository in EntityFrameworkCore Layer

```csharp
// src/Eaf.ProjectName.EntityFrameworkCore/Repositories/MyEntityRepository.cs
public class MyEntityRepository : EfCoreRepositoryBase<EafProjectNameDbContext, MyEntity, Guid>, IMyEntityRepository
{
    public MyEntityRepository(IDbContextProvider<EafProjectNameDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
```

### Step 4: Create Worker Service

```csharp
// src/Eaf.ProjectName.Worker/Workers/MyWorker.cs
public class MyWorker : BackgroundService
{
    private readonly ILogger<MyWorker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public MyWorker(ILogger<MyWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var myService = scope.ServiceProvider.GetRequiredService<IMyService>();
            
            await myService.ProcessAsync();
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### Step 5: Register Worker in Program.cs

```csharp
// src/Eaf.ProjectName.Worker/Program.cs
services.AddHostedService<MyWorker>();
```

### Step 6: Create Migration

```bash
dotnet ef migrations add AddMyEntity --project src/Eaf.ProjectName.EntityFrameworkCore
```

### Step 7: Update Database

```bash
dotnet ef database update --project src/Eaf.ProjectName.EntityFrameworkCore
```

## Creating New Hangfire Jobs

### Step 1: Create Job Class

```csharp
// src/Eaf.ProjectName.Worker/Jobs/MyJob.cs
public class MyJob
{
    private readonly ILogger<MyJob> _logger;
    private readonly IMyService _myService;

    public MyJob(ILogger<MyJob> logger, IMyService myService)
    {
        _logger = logger;
        _myService = myService;
    }

    public async Task ExecuteAsync()
    {
        _logger.LogInformation("Executing job");
        await _myService.ProcessAsync();
    }
}
```

### Step 2: Register Job in Program.cs

```csharp
// src/Eaf.ProjectName.Worker/Program.cs
services.AddHangfire(config =>
{
    config.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire"));
});

services.AddHangfireServer();

// Register job
RecurringJob.AddOrUpdate<MyJob>(
    "my-job",
    job => job.ExecuteAsync(),
    Cron.Hourly);
```

## Module Best Practices

1. **Separation of Concerns**: Keep each layer focused on its responsibility
2. **Dependency Direction**: Dependencies should only point downward
3. **Interface Segregation**: Define specific interfaces for repositories and services
4. **Async**: Use async/await for all database operations
5. **Unit of Work**: Let ABP handle Unit of Work automatically
6. **Multi-tenancy**: Use AbpSession for tenant context
7. **Logging**: Use ILogger for all logging
8. **Cancellation Tokens**: Use cancellation tokens for graceful shutdown
9. **Dependency Injection**: Use constructor injection for dependencies
10. **Configuration**: Use appsettings.json for configuration
