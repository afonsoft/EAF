# Eaf.Middleware.Web.Core

## Descrição Técnica

O **Eaf.Middleware.Web.Core** é o módulo web do Enterprise Application Foundation (EAF). Este módulo fornece componentes web para ASP.NET Core incluindo configuração de startup, middleware, filtros, controllers, SignalR, Swagger, health checks e integração HTTP.

Este módulo serve como a camada de apresentação web, integrando todos os componentes do EAF com ASP.NET Core e fornecendo funcionalidades prontas para uso em aplicações web.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp.AspNetCore**: Framework base para ASP.NET Core
- **Abp.AspNetCore.SignalR**: Integração com SignalR para comunicação em tempo real
- **Abp.HangFire.AspNetCore**: Processamento de jobs em background
- **Abp.AspNetCore.PerRequestRedisCache**: Cache por requisição com Redis
- **Abp.Zero.Common**: Funcionalidades comuns do ABP Zero

### Dependências do EAF
- **Eaf.Middleware.Application**: Camada de aplicação
- **Eaf.SqlServerCache**: Cache distribuído com SQL Server

### Dependências Externas
- **Serilog.Sinks.Elasticsearch**: Exportação de logs para Elasticsearch
- **Serilog.Sinks.Seq**: Exportação de logs para Seq
- **Swashbuckle.AspNetCore**: Documentação de API com Swagger/OpenAPI
- **Hangfire.SqlServer**: Storage do Hangfire com SQL Server
- **Hangfire.AspNetCore**: Integração do Hangfire com ASP.NET Core
- **Microsoft.Extensions.Caching.StackExchangeRedis**: Cache com Redis

### Principais Componentes

#### Authentication
- Configuração de autenticação JWT Bearer
- Autenticação Microsoft Account
- Gerenciamento de tokens

#### Configuration
- Configuração de startup do ASP.NET Core
- Configuração de middleware
- Configuração de serviços

#### Controllers
- Controllers base para APIs REST
- Filtros de autorização
- Tratamento de erros

#### SignalR
- Hubs para comunicação em tempo real
- Integração com chat
- Notificações em tempo real

#### Swagger
- Configuração de documentação de API
- Integração com OpenAPI
- UI do Swagger

#### Health Checks
- Health checks para monitoramento
- Integração com ASP.NET Core Health Checks

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Eaf.Middleware.Application 10.4.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Web.Core --version 10.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Web.Core\Eaf.Middleware.Web.Core.csproj" />
```

## Exemplo Básico de Uso

### 1. Registrando o Módulo

No `Startup.cs` ou `Program.cs`:

```csharp
[DependsOn(
    typeof(MiddlewareWebCoreModule),
    typeof(MiddlewareApplicationModule)
)]
public class MyWebModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Configurando o Startup

```csharp
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEafWebCore(options =>
        {
            options.ConnectionString = Configuration.GetConnectionString("Default");
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseEafWebCore();
    }
}
```

### 3. Criando um Controller

```csharp
public class TaskController : EafControllerBase
{
    private readonly ITaskAppService _taskAppService;

    public TaskController(ITaskAppService taskAppService)
    {
        _taskAppService = taskAppService;
    }

    [HttpGet]
    public async Task<PagedResultDto<TaskDto>> GetAll(GetAllTasksInput input)
    {
        return await _taskAppService.GetAll(input);
    }

    [HttpPost]
    public async Task<TaskDto> Create(CreateTaskInput input)
    {
        return await _taskAppService.Create(input);
    }

    [HttpPut]
    public async Task<TaskDto> Update(UpdateTaskInput input)
    {
        return await _taskAppService.Update(input);
    }

    [HttpDelete]
    public async Task Delete(EntityDto input)
    {
        await _taskAppService.Delete(input);
    }
}
```

### 4. Configurando Swagger

No `Startup.cs`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "My API",
            Version = "v1",
            Description = "API documentation"
        });
    });
}

public void Configure(IApplicationBuilder app)
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}
```

### 5. Configurando SignalR

```csharp
public class MyHub : AbpHub
{
    public async Task SendMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}

// No Startup.cs
public void Configure(IApplicationBuilder app)
{
    app.UseSignalR(routes =>
    {
        routes.MapHub<MyHub>("/myHub");
    });
}
```

### 6. Configurando Health Checks

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHealthChecks()
        .AddCheck("database", new SqlConnectionHealthCheck(Configuration.GetConnectionString("Default")))
        .AddCheck("redis", new RedisHealthCheck(Configuration.GetConnectionString("Redis")));
}

public void Configure(IApplicationBuilder app)
{
    app.UseHealthChecks("/health");
    app.UseHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Name == "database"
    });
}
```

## Estrutura do Módulo

```
Eaf.Middleware.Web.Core/
├── Authentication/         # Configurações de autenticação
├── Configuration/          # Configuração de startup e middleware
├── Controllers/            # Controllers base
├── Features/               # Features específicas
├── HealthChecks/           # Health checks
├── Helpers/                # Helpers utilitários
├── Models/                 # Models DTOs
├── Notifications/          # Notificações
├── Security/               # Configurações de segurança
├── Serilog/                # Configuração do Serilog
├── Session/                # Gerenciamento de sessão
├── SignalR/                # Hubs SignalR
├── Swagger/                # Configuração Swagger
├── UiCustomization/        # Customização de UI
├── Url/                    # Helpers de URL
└── WebHooks/               # WebHooks
```

## Configurações Opcionais

### Configuração de Serilog
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSerilog(options =>
    {
        options.WriteTo.Elasticsearch(Configuration.GetConnectionString("Elasticsearch"));
        options.WriteTo.Seq(Configuration.GetConnectionString("Seq"));
    });
}
```

### Configuração de Redis Cache
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "MyApp"
  }
}
```

### Configuração de Hangfire

O Hangfire seleciona automaticamente o tipo de armazenamento com base no provider de banco de dados e configurações de Redis:

- **SQL Server** (`Database:Provider` = `SqlServer`/`MSSQL`): Usa SQL Server storage
- **Não SQL Server + Redis habilitado** (`RedisCache:IsEnabled` = `true`): Usa Redis storage via `Hangfire.Redis.StackExchange`
- **Não SQL Server + Redis desabilitado**: Usa armazenamento em memória

```csharp
// A configuração é feita automaticamente pelo HangFireConfigurer.Configure()
// O tipo de storage é resolvido por HangFireConfigurer.ResolveStorageType()
public void ConfigureServices(IServiceCollection services)
{
    HangFireConfigurer.Configure(services, Configuration);
}
```

```json
{
  "Hangfire": {
    "IsEnabled": "true",
    "IsInMemoryDatabase": "false"
  },
  "RedisCache": {
    "IsEnabled": "true",
    "ConnectionString": "localhost:6379",
    "DatabaseId": 0
  }
}
```

## Testes

Os testes para este módulo devem ser criados seguindo o padrão dos outros módulos do EAF.

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF
