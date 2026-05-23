# EAF Worker Template

## Overview

The EAF (Enterprise Application Framework) Worker Template is a background service application built with .NET 10.0, C# 14, and ASP.NET Core Worker. It provides a solid foundation for building background jobs, scheduled tasks, and long-running services with integration with the EAF framework.

## Technology Stack

- **.NET 10.0**: Modern .NET framework
- **C# 14**: Type-safe programming language
- **ASP.NET Core Worker**: Background service template
- **Hangfire**: Background job scheduling
- **Entity Framework Core 10.0**: ORM for data access
- **ASP.NET Boilerplate 10.4.0**: Framework base

## Features

- **Background Workers**: Long-running background services
- **Hangfire Integration**: Scheduled background jobs
- **Multi-Tenancy**: Full multi-tenancy support
- **Database Access**: Entity Framework Core with migrations
- **Logging**: Structured logging with Serilog
- **Health Checks**: Built-in health check endpoints
- **Docker Support**: Containerized deployment
- **Windows Service**: Run as Windows service
- **Linux Service**: Run as systemd service on Linux

## Database Configuration

### Database Provider

The template supports different database providers through the `Database:Provider` configuration in `appsettings.json`:

```json
{
  "Database": {
    "Provider": "SqlServer"
  }
}
```

**Supported providers:**
- `SqlServer` or `MSSQL` - Microsoft SQL Server (default)
- `MySQL` - MySQL (when stable EF Core 10.0 support is available)
- `PostgreSQL` or `Postgres` - PostgreSQL (when stable EF Core 10.0 support is available)

**Note:** PostgreSQL and MySQL do not yet have stable support for EF Core 10.0. When stable versions are available, support will be added to the template.

### Connection Strings

Configure your database connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=ProjectNameDb;Trusted_Connection=True;"
  }
}
```

## Documentation

For detailed documentation about the template, see the [docs folder](docs/).

### Available Documentation

- **[Getting Started](docs/GETTING_STARTED.md)** - Installation, configuration, and running the application
- **[Modules](docs/MODULES.md)** - Angular module structure and organization
- **[Implementations](docs/IMPLEMENTATIONS.md)** - Implementation patterns and best practices

## Quick Start

### Prerequisites

- Node.js v18 or later
- npm v9 or later
- Angular CLI v19 or later
- Backend API (EAF ASP.NET Core) must be running

### Installation

```bash
# Install dependencies
npm install

# Generate service proxies
npm run nswag

# Start development server
npm start
```

The application will be available at `http://localhost:4200`

### Configuration

Edit environment files in `src/environments/`:

**Development (environment.ts)**:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:21021',
  remoteServiceBaseUrl: 'http://localhost:21021',
  signalrUrl: 'http://localhost:21021/signalr'
};
```

**Production (environment.prod.ts)**:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.com',
  remoteServiceBaseUrl: 'https://your-api-domain.com',
  signalrUrl: 'https://your-api-domain.com/signalr'
};
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

## Development Scripts

```bash
# Start development server
npm start

# Build for production
npm run build

# Run unit tests
npm test

# Lint code
npm run lint

# Format code with Prettier
npm run prettier
npm run prettier-fix

# Generate service proxies
npm run nswag
```

## Docker Deployment

The template includes a Dockerfile for containerized deployment:

```bash
# Build Docker image
docker build -t eaf-worker .

# Run Docker container
docker run -d --name eaf-worker eaf-worker
```

## Service Deployment

### Windows Service

```bash
# Build for release
dotnet publish -c Release

# Install as Windows service
sc create EafWorker binPath="C:\Path\To\Eaf.ProjectName.Worker.exe"
sc start EafWorker
```

### Linux Service (systemd)

```bash
# Create service file
sudo nano /etc/systemd/system/eaf-worker.service

# Enable and start service
sudo systemctl daemon-reload
sudo systemctl enable eaf-worker
sudo systemctl start eaf-worker
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

## License

This template is part of the EAF (Enterprise Application Framework) project and follows the same license as the main project.

## Support

For issues and questions:

- Check the [documentation](docs/) folder
- Review the main EAF documentation

## Contributing

When contributing to this template:

1. Follow the existing code style and patterns
2. Use async/await for all I/O operations
3. Add appropriate unit tests
4. Update documentation for new features
