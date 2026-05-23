# Eaf.OpenTelemetry

## Descrição Técnica

O **Eaf.OpenTelemetry** é um módulo de observabilidade do Enterprise Application Foundation (EAF). Este módulo fornece implementação completa de OpenTelemetry para telemetria distribuída, tracing e métricas, permitindo monitoramento profundo de aplicações .NET com suporte a múltiplos exporters.

Este módulo segue os padrões do OpenTelemetry e se integra perfeitamente com ASP.NET Core, Entity Framework Core, Hangfire e HTTP requests.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração
- **Abp.AspNetCore**: Integração com ASP.NET Core

### Dependências Externas
- **OpenTelemetry**: SDK principal do OpenTelemetry
- **OpenTelemetry.Api**: API principal do OpenTelemetry
- **OpenTelemetry.Extensions.Hosting**: Integração com ASP.NET Core Hosting
- **OpenTelemetry.Instrumentation.AspNetCore**: Instrumentação para ASP.NET Core
- **OpenTelemetry.Instrumentation.EntityFrameworkCore**: Instrumentação para Entity Framework Core
- **OpenTelemetry.Instrumentation.Http**: Instrumentação para HTTP
- **OpenTelemetry.Instrumentation.Runtime**: Instrumentação para .NET Runtime
- **OpenTelemetry.Instrumentation.Hangfire**: Instrumentação para Hangfire
- **OpenTelemetry.Exporter.Prometheus.AspNetCore**: Exporter para Prometheus
- **OpenTelemetry.Exporter.OpenTelemetryProtocol**: Exporter para OTLP
- **OpenTelemetry.Exporter.Console**: Exporter para Console

### Principais Componentes

#### Instrumentação
- **ASP.NET Core**: Tracing de requests HTTP
- **Entity Framework Core**: Tracing de queries de banco de dados
- **HTTP Client**: Tracing de requisições HTTP externas
- **Hangfire**: Tracing de jobs em background
- **Runtime**: Métricas do .NET Runtime

#### Exporters
- **OTLP**: Exportação para backends compatíveis com OTLP (Jaeger, Zipkin, etc.)
- **Prometheus**: Exportação de métricas para Prometheus
- **Console**: Exportação para console (debugging)

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.4.0

### Instalação via NuGet
```bash
dotnet add package Eaf.OpenTelemetry --version 10.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.OpenTelemetry\Eaf.OpenTelemetry.csproj" />
```

## Exemplo Básico de Uso

### 1. Registrando o Módulo

No `Program.cs` ou `Startup.cs`:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddEafOpenTelemetry(options =>
        {
            options.ServiceName = "MyApplication";
            options.ServiceVersion = "1.0.0";
        });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseEafOpenTelemetry();
    }
}
```

### 2. Configurando Exporter OTLP

No `appsettings.json`:
```json
{
  "OpenTelemetry": {
    "Exporter": "Otlp",
    "Otlp": {
      "Endpoint": "http://localhost:4317",
      "Headers": {
        "X-My-Header": "value"
      }
    }
  }
}
```

### 3. Configurando Exporter Prometheus

No `appsettings.json`:
```json
{
  "OpenTelemetry": {
    "Exporter": "Prometheus",
    "Prometheus": {
      "Path": "/metrics",
      "ScrapeEndpoint": "/metrics"
    }
  }
}
```

### 4. Configurando Instrumentação

```csharp
services.AddEafOpenTelemetry(options =>
{
    options.ServiceName = "MyApplication";
    options.ServiceVersion = "1.0.0";
    
    // Habilita instrumentação ASP.NET Core
    options.AddAspNetCoreInstrumentation();
    
    // Habilita instrumentação Entity Framework Core
    options.AddEntityFrameworkCoreInstrumentation();
    
    // Habilita instrumentação HTTP
    options.AddHttpClientInstrumentation();
    
    // Habilita instrumentação Hangfire
    options.AddHangfireInstrumentation();
    
    // Habilita métricas do runtime
    options.AddRuntimeInstrumentation();
    
    // Configura exporter
    options.AddOtlpExporter();
});
```

### 5. Usando Tracing Manual

```csharp
public class MyService
{
    private readonly ActivitySource _activitySource;

    public MyService()
    {
        _activitySource = new ActivitySource("MyService");
    }

    public async Task DoWorkAsync()
    {
        using var activity = _activitySource.StartActivity("DoWork");
        activity?.SetTag("operation", "important");
        
        // Seu código aqui
        await Task.Delay(100);
    }
}
```

### 6. Adicionando Métricas Customizadas

```csharp
public class MyService
{
    private readonly Counter<long> _requestCounter;

    public MyService(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("MyService");
        _requestCounter = meter.CreateCounter<long>("requests", "requests");
    }

    public void ProcessRequest()
    {
        _requestCounter.Add(1, new("operation", "process"));
    }
}
```

## Estrutura do Módulo

```
Eaf.OpenTelemetry/
├── AspNetCore/            # Instrumentação ASP.NET Core
├── OTEL_DIAGNOSTICS.json  # Configurações de diagnóstico
└── Properties/            # Propriedades do assembly
```

## Configurações Opcionais

### Sampling
```csharp
services.AddEafOpenTelemetry(options =>
{
    options.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(0.1)));
});
```

### Resource Attributes
```csharp
services.AddEafOpenTelemetry(options =>
{
    options.SetResourceBuilder(ResourceBuilder
        .CreateDefault()
        .AddService("MyService", "1.0.0")
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = "production",
            ["host.name"] = Environment.MachineName
        }));
});
```

### Batch Export
```csharp
services.AddEafOpenTelemetry(options =>
{
    options.AddOtlpExporter(opt =>
    {
        opt.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>
        {
            MaxQueueSize = 2048,
            ScheduledDelayMilliseconds = 5000,
            ExporterTimeoutMilliseconds = 30000,
            MaxExportBatchSize = 512
        };
    });
});
```

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.OpenTelemetry.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.OpenTelemetry.Tests/Eaf.OpenTelemetry.Tests.csproj
```

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF
