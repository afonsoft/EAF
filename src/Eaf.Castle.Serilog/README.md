# Eaf.Castle.Serilog

## Descrição Técnica

O **Eaf.Castle.Serilog** é um módulo de integração do Enterprise Application Foundation (EAF). Este módulo fornece um adaptador de logging que integra Castle Windsor com Serilog, permitindo logging estruturado e configurável em aplicações EAF.

Este módulo substitui o log4net tradicional pelo Serilog, oferecendo uma solução de logging mais moderna, flexível e com melhor performance, mantendo a integração com o Castle Windsor utilizado pelo ABP.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração

### Dependências Externas
- **Castle.Windsor**: Container de injeção de dependência
- **Castle.LoggingFacility**: Facility de logging do Castle
- **Serilog**: Framework de logging estruturado
- **Serilog.AspNetCore**: Integração com ASP.NET Core
- **Serilog.Sinks.Console**: Sink para console
- **Serilog.Sinks.File**: Sink para arquivo
- **Serilog.Sinks.Elasticsearch**: Sink para Elasticsearch (opcional)
- **Serilog.Sinks.Seq**: Sink para Seq (opcional)

### Principais Componentes

#### SerilogLoggerFactory
Factory que cria loggers Serilog integrados com Castle Windsor:
- Criação automática de loggers
- Integração com Castle Windsor LoggingFacility
- Configuração de sinks e enriquecers

#### Serilog Integration
Integração entre Castle Windsor e Serilog:
- Configuração de logging no startup
- Suporte a múltiplos sinks
- Enrichers automáticos (contexto, tenant, usuário)

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Castle.Serilog --version 9.3.1
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Castle.Serilog\Eaf.Castle.Serilog.csproj" />
```

## Exemplo Básico de Uso

### 1. Registrando o Módulo

No seu módulo principal, herde de `CastleSerilogModule`:

```csharp
[DependsOn(
    typeof(CastleSerilogModule),
    typeof(AbpModule)
)]
public class MyApplicationModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Configurando Serilog

No `Program.cs` ou `Startup.cs`:

```csharp
public static void Main(string[] args)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", Warning)
        .MinimumLevel.Override("System", Warning)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "MyApp")
        .WriteTo.Console()
        .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();

    try
    {
        Log.Information("Iniciando aplicação");
        
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSerilog();
        
        // Configurar aplicação EAF
        builder.Services.AddEafCastleSerilog();
        
        var host = builder.Build();
        await host.RunAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Aplicação falhou ao iniciar");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}
```

### 3. Usando Logger em Serviços

```csharp
public class MyService : ApplicationService
{
    private readonly ILogger _logger;

    public MyService(ILogger logger)
    {
        _logger = logger;
    }

    public void DoWork()
    {
        _logger.LogInformation("Iniciando trabalho");
        
        try
        {
            // Lógica de negócio
            _logger.LogInformation("Trabalho concluído com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao executar trabalho");
            throw;
        }
    }
}
```

### 4. Configurando Elasticsearch

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Elasticsearch(
        new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
        {
            IndexFormat = "eaf-logs-{0:yyyy.MM.dd}",
            AutoRegisterTemplate = true,
            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7
        })
    .CreateLogger();
```

### 5. Configurando Seq

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
```

### 6. Enrichers Personalizados

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
    .Enrich.WithProperty("MachineName", Environment.MachineName)
    .Enrich.With(new TenantEnricher())
    .Enrich.With(new UserEnricher())
    .WriteTo.Console()
    .CreateLogger();
```

## Estrutura do Módulo

```
Eaf.Castle.Serilog/
├── Castle/               # Integração com Castle Windsor
│   ├── SerilogLoggerFactory.cs
│   ├── SerilogLogger.cs
│   └── CastleSerilogModule.cs
└── Eaf.Castle.Serilog.csproj  # Projeto
```

## Configurações Opcionais

### Configuração de Nível de Log por Namespace
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", Warning)
    .MinimumLevel.Override("System", Warning)
    .MinimumLevel.Override("Abp", Information)
    .CreateLogger();
```

### Configuração de Output Template
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();
```

### Configuração de Rolling File
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10485760
    )
    .CreateLogger();
```

## Enrichers Personalizados

### Tenant Enricher
```csharp
public class TenantEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var tenantId = AbpSession.TenantId;
        if (tenantId.HasValue)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("TenantId", tenantId.Value));
        }
    }
}
```

### User Enricher
```csharp
public class UserEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var userId = AbpSession.UserId;
        if (userId.HasValue)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("UserId", userId.Value));
        }
    }
}
```

## Testes

Os testes para este módulo devem ser criados seguindo o padrão dos outros módulos do EAF.

**Cobertura Atual:** 10.0% (necessita expansão significativa para atingir meta de 90%)

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF