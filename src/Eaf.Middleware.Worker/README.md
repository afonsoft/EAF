# Eaf.Middleware.Worker

## Descrição Técnica

O **Eaf.Middleware.Worker** é um módulo de serviços em background do Enterprise Application Foundation (EAF). Este módulo fornece uma base para criar Worker Services do .NET que executam tarefas assíncronas, jobs agendados e processamento de longa duração, integrando-se perfeitamente com o ecossistema EAF e ABP.

Este módulo utiliza o modelo de hosting do ASP.NET Core para serviços worker, permitindo injeção de dependência, logging estruturado com Serilog, e integração com outros serviços do EAF.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração
- **Abp.AutoMapper**: Mapeamento de objetos
- **Abp.MailKit**: Envio de emails
- **Abp.Zero.Common**: Funcionalidades comuns do ABP Zero

### Dependências Externas
- **Microsoft.Extensions.Hosting**: Hosting do ASP.NET Core para worker services
- **Serilog.AspNetCore**: Logging estruturado
- **Castle.Windsor.MsDependencyInjection**: Integração do Castle Windsor com Microsoft DI
- **Castle.LoggingFacility.MsLogging**: Logging do Castle com Microsoft Extensions Logging

### Principais Componentes

#### EafWorkerBase
Classe base abstrata para worker services, fornecendo:
- Configuração de Serilog
- Injeção de dependência
- Logging estruturado
- Gerenciamento de ciclo de vida

#### Configuration
- Configuração de worker services
- Configuração de logging
- Configuração de email

#### Emailing
- Envio de emails em background
- Templates de email
- Processamento de fila de emails

#### VirtualFileSystem
- Sistema de arquivos virtual
- Gerenciamento de recursos

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Worker --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Worker\Eaf.Middleware.Worker.csproj" />
```

## Exemplo Básico de Uso

### 1. Criando um Worker Service

```csharp
public class MyWorker : EafWorkerBase
{
    private readonly ILogger<MyWorker> _logger;

    public MyWorker(ILogger<MyWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker executando em: {time}", DateTimeOffset.Now);
            
            // Seu código de processamento aqui
            await ProcessTaskAsync();
            
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        _logger.LogInformation("Worker parado");
    }

    private async Task ProcessTaskAsync()
    {
        // Lógica de processamento
        await Task.CompletedTask;
    }
}
```

### 2. Configurando o Worker

No `Program.cs`:

```csharp
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddEafWorker(options =>
        {
            options.ConnectionString = builder.Configuration.GetConnectionString("Default");
        });

        builder.Services.AddHostedService<MyWorker>();

        var host = builder.Build();
        await host.RunAsync();
    }
}
```

### 3. Configurando Serilog

No `Program.cs`:

```csharp
public static void Main(string[] args)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Elasticsearch("http://localhost:9200")
        .CreateLogger();

    try
    {
        Log.Information("Iniciando worker service");
        
        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddSerilog();
        
        // Configurar worker
        builder.Services.AddEafWorker();
        builder.Services.AddHostedService<MyWorker>();

        var host = builder.Build();
        await host.RunAsync();
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Worker service falhou ao iniciar");
    }
    finally
    {
        Log.CloseAndFlush();
    }
}
```

### 4. Usando Injeção de Dependência

```csharp
public class EmailWorker : EafWorkerBase
{
    private readonly IEmailSender _emailSender;
    private readonly IRepository<EmailQueue> _emailQueue;

    public EmailWorker(
        IEmailSender emailSender,
        IRepository<EmailQueue> emailQueue)
    {
        _emailSender = emailSender;
        _emailQueue = emailQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var emails = await _emailQueue.GetAllListAsync();
            
            foreach (var email in emails)
            {
                await _emailSender.SendAsync(email.To, email.Subject, email.Body);
                await _emailQueue.DeleteAsync(email);
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

### 5. Configurando Email

No `appsettings.json`:

```json
{
  "Email": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "SmtpUserName": "user@example.com",
    "SmtpPassword": "password",
    "DefaultFromAddress": "noreply@example.com",
    "DefaultFromDisplayName": "My Application"
  }
}
```

## Estrutura do Módulo

```
Eaf.Middleware.Worker/
├── Configuration/          # Configuração do worker
├── Dependency/             # Configuração de DI
├── Emailing/               # Serviços de email
├── Folders/                # Gerenciamento de pastas
├── Serilog/                # Configuração do Serilog
├── ServiceProviders/       # Provedores de serviço
├── VirtualFileSystem/      # Sistema de arquivos virtual
├── IEafWorkerBase.cs       # Interface base
├── EafWorkerBase.cs        # Implementação base
└── MiddlewareWorkerModule.cs  # Módulo ABP
```

## Configurações Opcionais

### Configuração de Intervalo de Execução
```csharp
public class MyWorker : EafWorkerBase
{
    private readonly IConfiguration _configuration;

    public MyWorker(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = _configuration.GetValue<int>("Worker:IntervalSeconds", 5);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessTaskAsync();
            await Task.Delay(TimeSpan.FromSeconds(interval), stoppingToken);
        }
    }
}
```

### Configuração de Retry Policy
```csharp
protected override async Task ExecuteAsync(CancellationToken stoppingToken)
{
    var retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    while (!stoppingToken.IsCancellationRequested)
    {
        await retryPolicy.ExecuteAsync(async () => await ProcessTaskAsync());
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
}
```

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.Middleware.Worker.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.Middleware.Worker.Tests/Eaf.Middleware.Worker.Tests.csproj
```

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF