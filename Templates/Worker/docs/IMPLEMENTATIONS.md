# Implementations - EAF Worker Template

## Overview

This document describes the key implementation patterns and practices used in the EAF Worker Template.

## Background Worker Implementation

### BackgroundService Base

The template uses `BackgroundService` from ASP.NET Core as the base for all workers:

```csharp
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
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var myService = scope.ServiceProvider.GetRequiredService<IMyService>();
                
                _logger.LogInformation("Processing task");
                await myService.ProcessAsync();
                
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing task");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
```

### Graceful Shutdown

Handle cancellation tokens for graceful shutdown:

```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    while (!stoppingToken.IsCancellationRequested)
    {
        // Process task
        await ProcessTaskAsync(stoppingToken);
    }
}
```

## Hangfire Job Implementation

### Recurring Job

```csharp
public class MyRecurringJob
{
    private readonly ILogger<MyRecurringJob> _logger;
    private readonly IMyService _myService;

    public MyRecurringJob(ILogger<MyRecurringJob> logger, IMyService myService)
    {
        _logger = logger;
        _myService = myService;
    }

    public async Task ExecuteAsync()
    {
        try
        {
            _logger.LogInformation("Executing job");
            await _myService.ProcessAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing job");
            throw;
        }
    }
}
```

### Job Registration

Register jobs in `Program.cs`:

```csharp
services.AddHangfire(config =>
{
    config.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire"));
    config.UseCollectedMetrics();
    config.UseDefaultTypeResolver();
});

services.AddHangfireServer();

// Register recurring job
RecurringJob.AddOrUpdate<MyRecurringJob>(
    "my-recurring-job",
    job => job.ExecuteAsync(),
    Cron.Hourly);

// Register delayed job
BackgroundJob.Schedule(() => Console.WriteLine("Delayed job"), TimeSpan.FromMinutes(5));

// Register fire-and-forget job
BackgroundJob.Enqueue(() => Console.WriteLine("Fire-and-forget job"));
```

## Service Implementation

### Worker Service

```csharp
public class MyService : ITransientDependency
{
    private readonly IRepository<MyEntity, Guid> _myEntityRepository;
    private readonly ILogger<MyService> _logger;

    public MyService(
        IRepository<MyEntity, Guid> myEntityRepository,
        ILogger<MyService> logger)
    {
        _myEntityRepository = myEntityRepository;
        _logger = logger;
    }

    public async Task ProcessAsync()
    {
        var entities = await _myEntityRepository.GetAllListAsync();
        
        foreach (var entity in entities)
        {
            _logger.LogInformation($"Processing entity: {entity.Name}");
            await ProcessEntityAsync(entity);
        }
    }

    private async Task ProcessEntityAsync(MyEntity entity)
    {
        // Process entity
        entity.LastProcessed = DateTime.UtcNow;
        await _myEntityRepository.UpdateAsync(entity);
    }
}
```

## Repository Implementation

### Custom Repository

```csharp
public class MyEntityRepository : EfCoreRepositoryBase<EafProjectNameDbContext, MyEntity, Guid>, IMyEntityRepository
{
    public MyEntityRepository(IDbContextProvider<EafProjectNameDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<MyEntity>> GetPendingEntitiesAsync()
    {
        return await (await GetAllAsync())
            .Where(x => x.Status == EntityStatus.Pending)
            .ToListAsync();
    }

    public async Task<List<MyEntity>> GetEntitiesByDateAsync(DateTime date)
    {
        return await (await GetAllAsync())
            .Where(x => x.CreationTime.Date == date.Date)
            .ToListAsync();
    }
}
```

## Multi-Tenancy Implementation

### Tenant-Aware Queries

Repositories automatically filter by tenant when using `IRepository`:

```csharp
public async Task<List<MyEntity>> GetAllAsync()
{
    // This automatically filters by current tenant
    var entities = await _myEntityRepository.GetAllListAsync();
    return entities;
}
```

### Disable Tenant Filter

To query across all tenants (host level):

```csharp
using (CurrentUnitOfWork.DisableFilter(AbpDataFilters.MayHaveTenant))
{
    var allEntities = await _myEntityRepository.GetAllListAsync();
}
```

### Switch Tenant

```csharp
using (CurrentUnitOfWork.SetTenantId(tenantId))
{
    // Operations in this scope use the specified tenant
    var entities = await _myEntityRepository.GetAllListAsync();
}
```

## Event Bus Implementation

### Define Event

```csharp
public class MyEntityProcessedEvent : EventSource
{
    public Guid EntityId { get; set; }
    public string Name { get; set; }
}
```

### Publish Event

```csharp
public class MyService : ITransientDependency
{
    private readonly IEventBus _eventBus;

    public MyService(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task ProcessEntityAsync(MyEntity entity)
    {
        // Process entity
        entity.LastProcessed = DateTime.UtcNow;
        await _myEntityRepository.UpdateAsync(entity);

        // Publish event
        await _eventBus.PublishAsync(new MyEntityProcessedEvent
        {
            EntityId = entity.Id,
            Name = entity.Name
        });
    }
}
```

### Handle Event

```csharp
public class MyEntityProcessedEventHandler : IEventHandler<MyEntityProcessedEvent>, ITransientDependency
{
    public void HandleEvent(MyEntityProcessedEvent eventData)
    {
        // Handle event
        Logger.Info($"Entity processed: {eventData.Name}");
    }
}
```

## Caching Implementation

### Cache Worker Service

```csharp
public class MyService : ITransientDependency
{
    private readonly ICacheManager _cacheManager;

    public MyService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task<MyEntity> GetEntityAsync(Guid id)
    {
        var cacheKey = $"MyEntity_{id}";
        var cachedEntity = await _cacheManager.GetCache("MyEntityCache")
            .GetAsync(cacheKey, async () =>
            {
                var entity = await _myEntityRepository.GetAsync(id);
                return entity;
            });

        return cachedEntity;
    }
}
```

## Unit of Work Implementation

### Automatic Unit of Work

ABP automatically manages Unit of Work for worker methods:

```csharp
public class MyWorker : BackgroundService
{
    // Unit of Work is automatically started and committed
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var myService = scope.ServiceProvider.GetRequiredService<IMyService>();
        
        await myService.ProcessAsync();
        // Unit of Work is committed here
    }
}
```

### Manual Unit of Work

```csharp
public class MyService
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<MyEntity, Guid> _myEntityRepository;

    public MyService(
        IUnitOfWorkManager unitOfWorkManager,
        IRepository<MyEntity, Guid> myEntityRepository)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _myEntityRepository = myEntityRepository;
    }

    public async Task ProcessEntityAsync()
    {
        using (var uow = _unitOfWorkManager.Begin())
        {
            var entity = new MyEntity { Name = "Test" };
            await _myEntityRepository.InsertAsync(entity);
            await uow.CompleteAsync();
        }
    }
}
```

## Logging Implementation

### Structured Logging

```csharp
public class MyWorker : BackgroundService
{
    private readonly ILogger<MyWorker> _logger;

    public MyWorker(ILogger<MyWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker started at: {Time}", DateTime.UtcNow);
        
        try
        {
            // Process task
            _logger.LogInformation("Task completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing task");
        }
    }
}
```

## Configuration Implementation

### Options Pattern

```csharp
public class MyServiceOptions
{
    public string ApiUrl { get; set; }
    public int IntervalMinutes { get; set; }
}

public class MyService : ITransientDependency
{
    private readonly MyServiceOptions _options;

    public MyService(IOptions<MyServiceOptions> options)
    {
        _options = options.Value;
    }

    public async Task ProcessAsync()
    {
        var interval = TimeSpan.FromMinutes(_options.IntervalMinutes);
        await Task.Delay(interval);
    }
}
```

### Register Options

```csharp
// Program.cs
services.Configure<MyServiceOptions>(Configuration.GetSection("MyService"));
```

## Health Check Implementation

### Health Check Service

```csharp
public class MyHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check service health
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(ex.Message);
        }
    }
}
```

### Register Health Checks

```csharp
// Program.cs
services.AddHealthChecks()
    .AddCheck<MyHealthCheck>("my-service");
```

## Best Practices

### Background Workers

1. **Use Cancellation Tokens**: Always accept and respect cancellation tokens
2. **Error Handling**: Wrap operations in try-catch blocks
3. **Logging**: Log all important events and errors
4. **Graceful Shutdown**: Handle shutdown gracefully
5. **Resource Cleanup**: Use `using` statements for resource cleanup
6. **Dependency Injection**: Use constructor injection for dependencies
7. **Scope**: Create service scopes for each iteration
8. **Delay**: Use appropriate delays between iterations
9. **Idempotency**: Make operations idempotent where possible
10. **Monitoring**: Add health checks for monitoring

### Hangfire Jobs

1. **Error Handling**: Handle errors and log appropriately
2. **Retry Logic**: Implement retry logic for transient failures
3. **Timeout**: Set appropriate timeouts for long-running operations
4. **Idempotency**: Make jobs idempotent to handle retries
5. **Logging**: Log job start, completion, and failures
6. **Monitoring**: Use Hangfire dashboard for monitoring
7. **Scheduling**: Use appropriate cron expressions for scheduling
8. **Queue**: Use queues for high-priority jobs
9. **Batch Processing**: Process items in batches for efficiency
10. **Cleanup**: Clean up completed jobs periodically

### Services

1. **Dependency Injection**: Use constructor injection
2. **Async**: Use async/await for all I/O operations
3. **Caching**: Use caching for frequently accessed data
4. **Logging**: Log important operations
5. **Error Handling**: Handle errors gracefully
6. **Validation**: Validate inputs
7. **Configuration**: Use options pattern for configuration
8. **Testing**: Write unit tests for services
9. **Documentation**: Document public methods
10. **Single Responsibility**: Keep services focused on single responsibility

### Repositories

1. **Use IRepository**: Use the generic repository for simple operations
2. **Custom Repositories**: Create custom repositories for complex queries
3. **Async**: Use async methods for all database operations
4. **Unit of Work**: Let ABP manage Unit of Work automatically
5. **Multi-tenancy**: Repositories automatically filter by tenant
6. **Specs**: Use specifications for complex queries
7. **Tracking**: Be aware of change tracking
8. **Performance**: Use AsNoTracking for read-only queries
9. **Batch Operations**: Use batch operations for bulk inserts/updates
10. **Transactions**: Use transactions when needed
