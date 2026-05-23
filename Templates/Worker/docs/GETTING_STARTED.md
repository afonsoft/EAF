# Getting Started - EAF Worker Template

## Overview

The EAF (Enterprise Application Framework) Worker Template is a background service application built with .NET 10.0, C# 14, and ASP.NET Core Worker. It provides a solid foundation for building background jobs, scheduled tasks, and long-running services with integration with the EAF framework.

## Prerequisites

Before running the EAF Worker template, ensure you have the following installed:

- **.NET 10.0 SDK**: The latest .NET SDK
- **SQL Server**: SQL Server 2016 or later (or SQL Server Express)
- **Visual Studio 2022** or **VS Code**: For development
- **Git**: For version control

## Installation

### 1. Clone or Copy the Template

Copy the `Templates/Worker` folder to your desired location.

### 2. Restore NuGet Packages

Navigate to the solution directory and restore packages:

```bash
cd Templates/Worker
dotnet restore
```

This will restore all required NuGet packages for all projects in the solution.

### 3. Configure Connection Strings

Edit the connection strings in `appsettings.json` in the `Eaf.ProjectName.Worker` project:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=EafProjectNameWorker;Trusted_Connection=True;MultipleActiveResultSets=true",
    "Hangfire": "Server=localhost;Database=EafProjectNameHangfire;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### 4. Update App Configuration

Edit `appsettings.json` in `Eaf.ProjectName.Worker` to configure application settings:

```json
{
  "App": {
    "ServerRootAddress": "http://localhost:21021/"
  }
}
```

## Database Setup

### 1. Create Database

The template uses Entity Framework Core migrations to create and update the database schema.

### 2. Run Migrations

Navigate to the `Eaf.ProjectName.Worker` project and run migrations:

```bash
cd src/Eaf.ProjectName.Worker
dotnet ef database update
```

This will create the database schema and seed initial data.

### 3. Seed Initial Data

The template includes a seed data system that creates:

- Default admin user (admin/123qwe)
- Default tenant
- Default roles
- Default permissions
- Default languages

## Running the Application

### Development Mode

Using Visual Studio:

1. Open `Eaf.ProjectName.WorkerService.sln`
2. Set `Eaf.ProjectName.Worker` as the startup project
3. Press F5 or click "Start" to run the application

Using Command Line:

```bash
cd src/Eaf.ProjectName.Worker
dotnet run
```

The worker service will start and begin processing background jobs.

### Production Mode

Build the application for production:

```bash
dotnet build --configuration Release
```

Or publish the application:

```bash
dotnet publish --configuration Release --output ./publish
```

Run the published application:

```bash
cd publish
dotnet Eaf.ProjectName.Worker.dll
```

## Docker Deployment

The template includes a Dockerfile for containerized deployment.

### Build Docker Image

```bash
docker build -t eaf-worker .
```

### Run Docker Container

```bash
docker run -d --name eaf-worker -e ConnectionStrings__Default="Server=sqlserver;Database=EafProjectNameWorker;User Id=sa;Password=YourPassword" eaf-worker
```

### Docker Compose

Use the included `docker-compose.yml`:

```bash
docker-compose up
```

## Project Structure

```
src/
├── Eaf.ProjectName.Worker/          # Worker service application
│   ├── Workers/                     # Background Workers
│   ├── Jobs/                        # Hangfire Jobs
│   └── Services/                    # Worker services
├── Eaf.ProjectName.Core/            # Domain layer (Entities, Value Objects)
└── Eaf.ProjectName.EntityFrameworkCore/  # Data access layer (DbContext, Repositories)
```

## Background Workers

### BackgroundService Base

The template uses `BackgroundService` from ASP.NET Core as the base for workers:

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
            using var scope = _serviceProvider.CreateScope();
            var myService = scope.ServiceProvider.GetRequiredService<IMyService>();
            
            await myService.ProcessAsync();
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### Hangfire Jobs

The template includes integration with Hangfire for scheduled jobs:

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
        _logger.LogInformation("Executing scheduled job");
        await _myService.ProcessAsync();
    }
}
```

## Service Configuration

### Windows Service

To run as Windows Service:

```bash
dotnet publish -c Release
sc create EafWorker binPath="C:\Path\To\Eaf.ProjectName.Worker.exe"
sc start EafWorker
```

### Linux Service

To run as systemd service on Linux:

Create file `/etc/systemd/system/eaf-worker.service`:

```ini
[Unit]
Description=EAF Worker Service
After=network.target

[Service]
ExecStart=/usr/bin/dotnet /path/to/Eaf.ProjectName.Worker.dll
Restart=always
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

Enable the service:

```bash
sudo systemctl daemon-reload
sudo systemctl enable eaf-worker
sudo systemctl start eaf-worker
```

## Configuration

### Environment Variables

You can override configuration using environment variables:

```bash
export ConnectionStrings__Default="Server=localhost;Database=EafProjectNameWorker;Trusted_Connection=True"
export App__ServerRootAddress="http://localhost:21021/"
```

### appsettings.json

The main configuration file includes:

- Connection strings
- Application settings
- Hangfire settings
- EAF module settings

## Development Scripts

Available dotnet CLI commands:

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test

# Run application
dotnet run --project src/Eaf.ProjectName.Worker

# Add migration
dotnet ef migrations add AddNewTable --project src/Eaf.ProjectName.EntityFrameworkCore

# Update database
dotnet ef database update --project src/Eaf.ProjectName.EntityFrameworkCore
```

## Monitoring

### Health Checks

The template includes health check endpoints:

- `/health`: Basic health check
- `/health/ready`: Readiness check
- `/health/live`: Liveness check

### Hangfire Dashboard

Access the Hangfire dashboard to monitor background jobs:

- URL: `http://localhost:21021/hangfire`

## Troubleshooting

### Common Issues

**1. Database Connection Error**

- Verify SQL Server is running
- Check connection string in appsettings.json
- Ensure SQL Server allows remote connections
- Verify SQL Server authentication mode

**2. Migration Errors**

- Ensure you're in the correct project directory
- Check that Entity Framework Core tools are installed:
  ```bash
  dotnet tool install --global dotnet-ef
  ```
- Verify the connection string is correct

**3. Worker Not Starting**

- Check the configuration in appsettings.json
- Verify the database connection
- Check the logs for error messages
- Ensure all dependencies are installed

**4. Hangfire Jobs Not Executing**

- Verify the Hangfire connection string
- Check that the Hangfire server is running
- Verify the job registration in Startup.cs
- Check the Hangfire dashboard for job status

**5. Service Installation Issues**

- Ensure you have administrator privileges
- Check the service name doesn't conflict
- Verify the path to the executable is correct
- Check Windows Event Viewer for errors

## Default Credentials

After running the application for the first time, you can log in with:

- **Username**: admin
- **Password**: 123qwe

**Important**: Change the default password immediately after first login.

## Next Steps

After successfully running the application:

1. Read the [Modules Documentation](MODULES.md) for detailed module structure
2. Review the [Implementations Documentation](IMPLEMENTATIONS.md) for implementation patterns
3. Explore the Hangfire dashboard at `http://localhost:21021/hangfire`
4. Review the code in `Eaf.ProjectName.Worker` to understand the worker implementation
5. Review the code in `Eaf.ProjectName.Core` to understand the domain layer
6. Review the code in `Eaf.ProjectName.EntityFrameworkCore` to understand the data access layer

## Additional Resources

- [ASP.NET Boilerplate Documentation](https://aspnetboilerplate.com/Pages/Documents)
- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Hangfire Documentation](https://docs.hangfire.com/)
