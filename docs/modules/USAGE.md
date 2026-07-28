# Guia de Uso dos Módulos EAF

Este documento centraliza exemplos práticos de como usar cada módulo de middleware do EAF. Para detalhes completos, consulte o `README.md` dentro da pasta `src/<modulo>`.

## Índice

- [Eaf.Castle.Serilog](#eafcastleserilog)
- [Eaf.KeyVault](#eafkeyvault)
- [Eaf.KeyVault.AspNetCore](#eafkeyvaultaspnetcore)
- [Eaf.Log4NetServiceBus](#eaflog4netservicebus)
- [Eaf.Middleware.Application](#eafmiddlewareapplication)
- [Eaf.Middleware.AzureActiveDirectory](#eafmiddlewareazureactivedirectory)
- [Eaf.Middleware.Core](#eafmiddlewarecore)
- [Eaf.Middleware.Ldap](#eafmiddlewareldap)
- [Eaf.Middleware.Web.Core](#eafmiddlewarewebcore)
- [Eaf.Middleware.Worker](#eafmiddlewareworker)
- [Eaf.OpenTelemetry](#eafopentelemetry)
- [Eaf.SqlServerCache](#eafsqlservercache)
- [Eaf.SqliteCache](#eafsqlitecache)

## <a id="eafcastleserilog"></a>Eaf.Castle.Serilog

**Propósito resumido:** O **Eaf.Castle.Serilog** é um módulo de integração do Enterprise Application Foundation (EAF). Este módulo fornece um adaptador de logging que integra Castle Windsor com Serilog, permitindo logging estruturado e configurável em aplicações EAF.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Castle.Serilog --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Castle.Serilog\Eaf.Castle.Serilog.csproj" />
```

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

> Documentação completa: [`src/Eaf.Castle.Serilog/README.md`](../../src/Eaf.Castle.Serilog/README.md) e [`docs/modules/eaf-castle-serilog.md`](./eaf-castle-serilog.md)

## <a id="eafkeyvault"></a>Eaf.KeyVault

**Propósito resumido:** O **Eaf.KeyVault** é um módulo de gerenciamento de segredos do Enterprise Application Foundation (EAF). Este módulo fornece integração com Azure Key Vault e Oracle Cloud Infrastructure (OCI) para armazenamento seguro de credenciais, chaves de API, strings de conexão e outros segredos sensíveis.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Azure Key Vault ou OCI Vault configurado
- Credenciais de acesso (Azure AD ou OCI)

### Instalação via NuGet
```bash
dotnet add package Eaf.KeyVault --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.KeyVault\Eaf.KeyVault.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `EafKeyVaultModule`:

```csharp
[DependsOn(
    typeof(EafKeyVaultModule),
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

### 2. Configurando Azure Key Vault

No `appsettings.json`:
```json
{
  "KeyVault": {
    "Provider": "Azure",
    "Azure": {
      "VaultName": "my-vault-name",
      "TenantId": "your-tenant-id",
      "ClientId": "your-client-id",
      "ClientSecret": "your-client-secret"
    }
  }
}
```

### 3. Configurando OCI Vault

No `appsettings.json`:
```json
{
  "KeyVault": {
    "Provider": "OCI",
    "OCI": {
      "VaultId": "ocid1.vault.oc1...",
      "Region": "us-ashburn-1",
      "TenancyId": "ocid1.tenancy.oc1...",
      "UserId": "ocid1.user.oc1...",
      "Fingerprint": "your-fingerprint",
      "PrivateKeyFilePath": "path/to/private_key.pem",
      "PrivateKeyPassphrase": "your-passphrase"
    }
  }
}
```

### 4. Usando o KeyVaultSecretManager

```csharp
public class MyService : ApplicationService
{
    private readonly IKeyVaultSecretManager _keyVaultManager;

    public MyService(IKeyVaultSecretManager keyVaultManager)
    {
        _keyVaultManager = keyVaultManager;
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var secret = await _keyVaultManager.GetSecretAsync(secretName);
        return secret.Value;
    }

    public async Task SetSecretAsync(string secretName, string secretValue)
    {
        await _keyVaultManager.SetSecretAsync(secretName, secretValue);
    }
}
```

### 5. Integrando com Configuration do ASP.NET Core

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
        // Adiciona KeyVault como fonte de configuração
        var keyVaultManager = services.BuildServiceProvider()
            .GetRequiredService<IKeyVaultSecretManager>();

        ConfigurationBuilder builder = new ConfigurationBuilder();
        builder.AddAzureKeyVault(keyVaultManager);
    }
}
```

### Cache de Segredos
```csharp
public override void PreInitialize()
{
    Configuration.Modules.EafKeyVault().EnableCache = true;
    Configuration.Modules.EafKeyVault().CacheDuration = TimeSpan.FromMinutes(30);
}
```

### Retries Automáticos
```csharp
public override void PreInitialize()
{
    Configuration.Modules.EafKeyVault().EnableRetries = true;
    Configuration.Modules.EafKeyVault().MaxRetries = 3;
    Configuration.Modules.EafKeyVault().RetryDelay = TimeSpan.FromSeconds(2);
}
```

> Documentação completa: [`src/Eaf.KeyVault/README.md`](../../src/Eaf.KeyVault/README.md) e [`docs/modules/eaf-keyvault.md`](./eaf-keyvault.md)

## <a id="eafkeyvaultaspnetcore"></a>Eaf.KeyVault.AspNetCore

**Propósito resumido:** O **Eaf.KeyVault.AspNetCore** é um módulo de integração ASP.NET Core para o Eaf.KeyVault. Este módulo fornece carregamento automático de configurações e segredos do Azure Key Vault ou Oracle Cloud Infrastructure (OCI) Vault diretamente no sistema de configuração do ASP.NET Core.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Eaf.KeyVault 9.4.0
- Azure Key Vault ou OCI Vault configurado

### Instalação via NuGet
```bash
dotnet add package Eaf.KeyVault.AspNetCore --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.KeyVault.AspNetCore\Eaf.KeyVault.AspNetCore.csproj" />
```

### 1. Registrando o Módulo

No `Startup.cs` ou `Program.cs`:

```csharp
[DependsOn(
    typeof(EafKeyVaultAspNetCoreModule),
    typeof(EafKeyVaultModule)
)]
public class MyWebModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Configurando no Startup

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
        services.AddEafKeyVaultAspNetCore(options =>
        {
            options.Provider = "Azure";
            options.Azure.VaultName = "my-vault-name";
            options.Azure.TenantId = "your-tenant-id";
            options.Azure.ClientId = "your-client-id";
            options.Azure.ClientSecret = "your-client-secret";
        });
    }
}
```

### 3. Usando Configurações do Key Vault

```csharp
public class MyService : ApplicationService
{
    private readonly IConfiguration _configuration;

    public MyService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetSecret()
    {
        return _configuration["MySecret"];
    }

    public string GetConnectionString()
    {
        return _configuration["ConnectionStrings:Default"];
    }
}
```

### 4. Carregando Configuration do Key Vault

```csharp
public class Startup
{
    public IConfigurationRoot Configuration { get; }

    public Startup(IHostingEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEafKeyVault(); // Adiciona Key Vault como fonte de configuração

        Configuration = builder.Build();
    }
}
```

### 5. Mapeando Segredos para Configurações

```json
{
  "KeyVault": {
    "SecretMappings": {
      "MyApp--ConnectionString": "ConnectionStrings:Default",
      "MyApp--ApiKey": "ApiSettings:ApiKey",
      "MyApp--SmtpPassword": "Smtp:Password"
    }
  }
}
```

### 6. Recarregamento de Configurações

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEafKeyVaultAspNetCore(options =>
    {
        options.Provider = "Azure";
        options.Azure.VaultName = "my-vault-name";
        options.Azure.TenantId = "your-tenant-id";
        options.Azure.ClientId = "your-client-id";
        options.Azure.ClientSecret = "your-client-secret";
        options.ReloadOnChange = true;
        options.ReloadInterval = TimeSpan.FromMinutes(5);
    });
}
```

### Configuração de Prefixo de Segredos
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEafKeyVaultAspNetCore(options =>
    {
        options.SecretPrefix = "MyApp--";
    });
}
```

### Configuração de Exclusão de Segredos
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEafKeyVaultAspNetCore(options =>
    {
        options.ExcludedSecrets = new[] { "TestSecret", "DevSecret" };
    });
}
```

### Configuração de OCI Vault
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddEafKeyVaultAspNetCore(options =>
    {
        options.Provider = "OCI";
        options.OCI.VaultId = "ocid1.vault.oc1...";
        options.OCI.Region = "us-ashburn-1";
        options.OCI.TenancyId = "ocid1.tenancy.oc1...";
        options.OCI.UserId = "ocid1.user.oc1...";
        options.OCI.Fingerprint = "your-fingerprint";
        options.OCI.PrivateKeyFilePath = "path/to/private_key.pem";
        options.OCI.PrivateKeyPassphrase = "your-passphrase";
    });
}
```

> Documentação completa: [`src/Eaf.KeyVault.AspNetCore/README.md`](../../src/Eaf.KeyVault.AspNetCore/README.md) e [`docs/modules/eaf-keyvault-aspnetcore.md`](./eaf-keyvault-aspnetcore.md)

## <a id="eaflog4netservicebus"></a>Eaf.Log4NetServiceBus

**Propósito resumido:** O **Eaf.Log4NetServiceBus** é um módulo de integração do Enterprise Application Foundation (EAF). Este módulo fornece integração com Azure Service Bus usando log4net para logging de mensagens e eventos de mensageria.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Azure Service Bus Namespace configurado

### Instalação via NuGet
```bash
dotnet add package Eaf.Log4NetServiceBus --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Log4NetServiceBus\Eaf.Log4NetServiceBus.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `Log4NetServiceBusModule`:

```csharp
[DependsOn(
    typeof(Log4NetServiceBusModule),
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

### 2. Configurando Azure Service Bus

No `appsettings.json`:

```json
{
  "AzureServiceBus": {
    "ConnectionString": "Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key",
    "QueueName": "my-queue",
    "TopicName": "my-topic",
    "SubscriptionName": "my-subscription"
  }
}
```

### 3. Configurando Log4Net

No `log4net.config`:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="ServiceBusAppender" type="log4net.Appender.AzureServiceBus.AzureServiceBusAppender">
    <connectionString value="Endpoint=sb://your-namespace.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key" />
    <queueName value="log-queue" />
    <layout type="log4net.Layout.PatternLayout">
      <conversionPattern value="%date [%thread] %-5level %logger - %message%newline%exception" />
    </layout>
  </appender>

  <root>
    <level value="INFO" />
    <appender-ref ref="ServiceBusAppender" />
  </root>
</log4net>
```

### 4. Enviando Mensagens

```csharp
public class MessageService : ApplicationService
{
    private readonly IServiceBusSender _serviceBusSender;

    public MessageService(IServiceBusSender serviceBusSender)
    {
        _serviceBusSender = serviceBusSender;
    }

    public async Task SendMessageAsync(string message)
    {
        await _serviceBusSender.SendQueueAsync("my-queue", message);
    }

    public async Task PublishMessageAsync(string message)
    {
        await _serviceBusSender.SendTopicAsync("my-topic", message);
    }
}
```

### 5. Recebendo Mensagens

```csharp
public class MessageReceiver : ApplicationService
{
    private readonly IServiceBusReceiver _serviceBusReceiver;

    public MessageReceiver(IServiceBusReceiver serviceBusReceiver)
    {
        _serviceBusReceiver = serviceBusReceiver;
    }

    public async Task StartReceivingAsync()
    {
        await _serviceBusReceiver.ReceiveQueueAsync("my-queue", async (message, cancellationToken) =>
        {
            Logger.Information($"Mensagem recebida: {message}");
            // Processar mensagem
            await Task.CompletedTask;
        });
    }
}
```

### 6. Logging de Operações

```csharp
public class MyService : ApplicationService
{
    private readonly ILogger _logger;

    public MyService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task ProcessMessageAsync(string message)
    {
        _logger.Info($"Processando mensagem: {message}");
        
        try
        {
            // Lógica de processamento
            _logger.Info("Mensagem processada com sucesso");
        }
        catch (Exception ex)
        {
            _logger.Error($"Erro ao processar mensagem: {ex.Message}", ex);
            throw;
        }
    }
}
```

### Configuração de Retry
```json
{
  "AzureServiceBus": {
    "ConnectionString": "...",
    "MaxRetryCount": 3,
    "RetryDelaySeconds": 5
  }
}
```

### Configuração de Batch
```json
{
  "AzureServiceBus": {
    "ConnectionString": "...",
    "BatchSize": 10,
    "MaxWaitTimeSeconds": 30
  }
}
```

### Configuração de Dead Letter
```json
{
  "AzureServiceBus": {
    "ConnectionString": "...",
    "EnableDeadLetterQueue": true,
    "DeadLetterQueueName": "dead-letter-queue"
  }
}
```

> Documentação completa: [`src/Eaf.Log4NetServiceBus/README.md`](../../src/Eaf.Log4NetServiceBus/README.md) e [`docs/modules/eaf-log4netservicebus.md`](./eaf-log4netservicebus.md)

## <a id="eafmiddlewareapplication"></a>Eaf.Middleware.Application

**Propósito resumido:** O **Eaf.Middleware.Application** é a camada de aplicação do Enterprise Application Foundation (EAF). Este módulo fornece DTOs (Data Transfer Objects), serviços de aplicação, validações e lógica de negócio intermediária, servindo como ponte entre a camada de domínio (Core) e a camada de apresentação (Web).

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Eaf.Middleware.Core 9.4.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Application --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Application\Eaf.Middleware.Application.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `MiddlewareApplicationModule`:

```csharp
[DependsOn(
    typeof(MiddlewareCoreModule),
    typeof(MiddlewareApplicationModule),
    typeof(AbpAutoMapperModule)
)]
public class MyWebModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Criando um Serviço de Aplicação

```csharp
public class TaskAppService : MiddlewareAppServiceBase, ITaskAppService
{
    private readonly IRepository<Task> _taskRepository;

    public TaskAppService(IRepository<Task> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [AbpAuthorize(MyPermissions.UpdateTasks)]
    public async Task UpdateTask(UpdateTaskInput input)
    {
        Logger.Info($"Updating task {input.TaskId}");

        var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
        if (task == null)
        {
            throw new UserFriendlyException(L("TaskNotFound"));
        }

        ObjectMapper.Map(input, task);
    }

    public async Task<TaskDto> GetTask(GetTaskInput input)
    {
        var task = await _taskRepository.FirstOrDefaultAsync(input.TaskId);
        return ObjectMapper.Map<TaskDto>(task);
    }
}
```

### 3. Definindo DTOs

```csharp
public class UpdateTaskInput
{
    [Required]
    public int TaskId { get; set; }

    [Required]
    [StringLength(Task.MaxTitleLength)]
    public string Title { get; set; }

    [StringLength(Task.MaxDescriptionLength)]
    public string Description { get; set; }
}

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationTime { get; set; }
}
```

### 4. Configurando Mapeamento de DTOs

No arquivo `MiddlewareCustomDtoMapper.cs`:

```csharp
public class MiddlewareCustomDtoMapper : Profile
{
    public MiddlewareCustomDtoMapper()
    {
        CreateMap<Task, TaskDto>();
        CreateMap<CreateTaskInput, Task>();
        CreateMap<UpdateTaskInput, Task>();
    }
}
```

### 5. Usando Serviços de Notificação

```csharp
public class NotificationAppService : MiddlewareAppServiceBase, INotificationAppService
{
    private readonly INotificationPublisher _notificationPublisher;

    public NotificationAppService(INotificationPublisher notificationPublisher)
    {
        _notificationPublisher = notificationPublisher;
    }

    public async Task SendNotificationAsync(SendNotificationInput input)
    {
        await _notificationPublisher.PublishAsync(
            "MyNotification",
            new NotificationData(input.Message),
            userIds: new[] { input.TargetUserId }
        );
    }
}
```

### 6. Exportando Dados para Excel

```csharp
public class DataExportAppService : MiddlewareAppServiceBase, IDataExportAppService
{
    public async Task<FileDto> ExportTasksToExcel(GetTasksInput input)
    {
        var tasks = await _taskRepository.GetAllListAsync();

        var excelFile = new ExcelFileCreator();
        var file = excelFile.CreateExcelFile(tasks);

        return file;
    }
}
```

### Configuração de AutoMapper
```csharp
public override void Initialize()
{
    var thisAssembly = Assembly.GetExecutingAssembly();

    Configuration.Modules.AbpAutoMapper().Configurators.Add(
        cfg => cfg.AddMaps(thisAssembly)
    );
}
```

### Substituindo Serviços
```csharp
public override void PreInitialize()
{
    Configuration.ReplaceService<INotificationPublisher, MyNotificationPublisher>(
        DependencyLifeStyle.Transient
    );
}
```

> Documentação completa: [`src/Eaf.Middleware.Application/README.md`](../../src/Eaf.Middleware.Application/README.md) e [`docs/modules/eaf-middleware-application.md`](./eaf-middleware-application.md)

## <a id="eafmiddlewareazureactivedirectory"></a>Eaf.Middleware.AzureActiveDirectory

**Propósito resumido:** O **Eaf.Middleware.AzureActiveDirectory** é um módulo de autenticação Azure Active Directory do Enterprise Application Foundation (EAF). Este módulo fornece integração completa com Azure AD para autenticação externa e sincronização de usuários, permitindo que usuários autentiquem usando suas credenciais Microsoft 365.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Azure AD Tenant configurado
- App Registration no Azure AD

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.AzureActiveDirectory --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.AzureActiveDirectory\Eaf.Middleware.AzureActiveDirectory.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `MiddlewareAzureActiveDirectoryModule`:

```csharp
[DependsOn(
    typeof(MiddlewareAzureActiveDirectoryModule),
    typeof(AbpZeroCommonModule)
)]
public class MyAuthenticationModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Configurando Azure AD

No `appsettings.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "CallbackPath": "/signin-oidc",
    "Domain": "your-domain.onmicrosoft.com"
  }
}
```

### 3. Configurando Microsoft Graph

```json
{
  "AzureAd": {
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "GraphScopes": "User.Read Group.Read.All"
  }
}
```

### 4. Usando Autenticação Azure AD

```csharp
public class AzureAdAuthenticationAppService : ApplicationService
{
    private readonly AzureActiveDirectoryAuthenticationSource _azureAdAuthSource;

    public AzureAdAuthenticationAppService(AzureActiveDirectoryAuthenticationSource azureAdAuthSource)
    {
        _azureAdAuthSource = azureAdAuthSource;
    }

    public async Task<bool> AuthenticateAsync(string token)
    {
        try
        {
            var result = await _azureAdAuthSource.AuthenticateAsync(token);
            return result != null;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Azure AD authentication failed");
            return false;
        }
    }
}
```

### 5. Sincronizando Usuários do Azure AD

```csharp
public class AzureAdSyncService : ApplicationService
{
    private readonly AzureActiveDirectoryAuthenticationSource _azureAdAuthSource;

    public AzureAdSyncService(AzureActiveDirectoryAuthenticationSource azureAdAuthSource)
    {
        _azureAdAuthSource = azureAdAuthSource;
    }

    public async Task SyncUserAsync(string objectId)
    {
        var user = await _azureAdAuthSource.CreateOrUpdateUserAsync(
            new ExternalAuthUserInfo
            {
                ProviderName = "AzureActiveDirectory",
                ProviderKey = objectId,
                Name = "user@domain.com"
            }
        );
    }
}
```

### 6. Usando Microsoft Graph API

```csharp
public class AzureAdGraphService : ApplicationService
{
    private readonly GraphServiceClient _graphClient;

    public AzureAdGraphService(GraphServiceClient graphClient)
    {
        _graphClient = graphClient;
    }

    public async Task<User> GetUserAsync(string userId)
    {
        return await _graphClient.Users[userId].Request().GetAsync();
    }

    public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
    {
        var groups = await _graphClient.Users[userId].MemberOf.Request().GetAsync();
        return groups.OfType<Group>();
    }
}
```

### Configuração de Claims Personalizados
```csharp
public override void PreInitialize()
{
    Configuration.Modules.EafAzureAd().ClaimMappings = new Dictionary<string, string>
    {
        ["given_name"] = "FirstName",
        ["family_name"] = "LastName",
        ["job_title"] = "JobTitle"
    };
}
```

### Configuração de Sincronização Automática
```json
{
  "AzureAd": {
    "AutoSyncUsers": true,
    "SyncGroups": true,
    "SyncIntervalMinutes": 60
  }
}
```

### Configuração de Multi-Tenant
```json
{
  "AzureAd": {
    "IsMultiTenant": true,
    "DefaultTenantId": "default-tenant-id"
  }
}
```

> Documentação completa: [`src/Eaf.Middleware.AzureActiveDirectory/README.md`](../../src/Eaf.Middleware.AzureActiveDirectory/README.md) e [`docs/modules/eaf-middleware-aad.md`](./eaf-middleware-aad.md)

## <a id="eafmiddlewarecore"></a>Eaf.Middleware.Core

**Propósito resumido:** O **Eaf.Middleware.Core** é a camada de domínio central do Enterprise Application Foundation (EAF). Este módulo fornece as entidades, serviços, configurações, autorização, auditoria e funcionalidades base do framework, servindo como fundação para todos os outros módulos do EAF.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Core --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Core\Eaf.Middleware.Core.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `MiddlewareCoreModule`:

```csharp
[DependsOn(
    typeof(MiddlewareCoreModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpHangfireAspNetCoreModule)
)]
public class MyApplicationModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Usando Serviços de Domínio

```csharp
public class MyService : ApplicationService
{
    private readonly ISettingManager _settingManager;
    private readonly IPermissionManager _permissionManager;

    public MyService(
        ISettingManager settingManager,
        IPermissionManager permissionManager)
    {
        _settingManager = settingManager;
        _permissionManager = permissionManager;
    }

    public async Task<string> GetSettingValueAsync(string settingName)
    {
        return await _settingManager.GetSettingValueAsync(settingName);
    }

    public async Task<bool> CheckPermissionAsync(string permissionName)
    {
        return await _permissionManager.IsGrantedAsync(permissionName);
    }
}
```

### 3. Usando Configurações de Chat

```csharp
public class ChatService : ApplicationService, IChatService
{
    private readonly IChatMessageManager _chatMessageManager;

    public ChatService(IChatMessageManager chatMessageManager)
    {
        _chatMessageManager = chatMessageManager;
    }

    public async Task SendMessageAsync(SendChatMessageInput input)
    {
        await _chatMessageManager.SendMessageAsync(
            Session.UserId,
            input.TenantId,
            input.TargetUserId,
            input.Message
        );
    }
}
```

### 4. Configurando Hangfire com Console

O módulo já inclui configuração aprimorada para Hangfire:

```csharp
public override void PreInitialize()
{
    Configuration.BackgroundJobs.UseHangfire(configuration =>
    {
        configuration.UseSqlServerStorage("Default");
        configuration.UseConsole(); // Logging aprimorado
        configuration.UseHeartbeat(); // Monitoramento
    });
}
```

### Configuração de Serilog
```csharp
public override void PreInitialize()
{
    Configuration.BackgroundJobs.UseHangfire(configuration =>
    {
        configuration.UseSerilogLogProvider(); // Integração com Serilog
    });
}
```

### Configuração de Email
```csharp
public override void PreInitialize()
{
    Configuration.ReplaceService<IEmailSender, CustomEmailSender>(DependencyLifeStyle.Transient);
}
```

> Documentação completa: [`src/Eaf.Middleware.Core/README.md`](../../src/Eaf.Middleware.Core/README.md) e [`docs/modules/eaf-middleware-core.md`](./eaf-middleware-core.md)

## <a id="eafmiddlewareldap"></a>Eaf.Middleware.Ldap

**Propósito resumido:** O **Eaf.Middleware.Ldap** é um módulo de autenticação LDAP/Active Directory. Este módulo fornece integração completa com diretórios LDAP para autenticação externa, permitindo que usuários autentiquem usando suas credenciais existentes.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Servidor LDAP ou Active Directory configurado

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Ldap --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Ldap\Eaf.Middleware.Ldap.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `MiddlewareLdapModule`:

```csharp
[DependsOn(
    typeof(MiddlewareLdapModule),
    typeof(AbpZeroCommonModule)
)]
public class MyAuthenticationModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
    }
}
```

### 2. Configurando LDAP

No `appsettings.json`:

```json
{
  "Ldap": {
    "Server": "ldap.example.com",
    "Port": 389,
    "UseSsl": false,
    "Domain": "example.com",
    "BaseDn": "DC=example,DC=com",
    "UserDn": "CN=Users,DC=example,DC=com",
    "UsernameAttribute": "sAMAccountName",
    "EmailAttribute": "mail",
    "FirstNameAttribute": "givenName",
    "LastNameAttribute": "sn"
  }
}
```

### 3. Configurando para Active Directory

```json
{
  "Ldap": {
    "Server": "ad.example.com",
    "Port": 636,
    "UseSsl": true,
    "Domain": "example.com",
    "BaseDn": "DC=example,DC=com",
    "UserDn": "CN=Users,DC=example,DC=com",
    "UsernameAttribute": "sAMAccountName",
    "EmailAttribute": "mail",
    "FirstNameAttribute": "givenName",
    "LastNameAttribute": "sn"
  }
}
```

### 4. Usando Autenticação LDAP

```csharp
public class LdapAuthenticationAppService : ApplicationService
{
    private readonly LdapAuthenticationSource _ldapAuthSource;

    public LdapAuthenticationAppService(LdapAuthenticationSource ldapAuthSource)
    {
        _ldapAuthSource = ldapAuthSource;
    }

    public async Task<bool> AuthenticateAsync(string username, string password)
    {
        try
        {
            var result = await _ldapAuthSource.AuthenticateAsync(username, password);
            return result != null;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "LDAP authentication failed");
            return false;
        }
    }
}
```

### 5. Sincronizando Usuários do LDAP

```csharp
public class LdapSyncService : ApplicationService
{
    private readonly LdapAuthenticationSource _ldapAuthSource;

    public LdapSyncService(LdapAuthenticationSource ldapAuthSource)
    {
        _ldapAuthSource = ldapAuthSource;
    }

    public async Task SyncUserAsync(string username)
    {
        var user = await _ldapAuthSource.CreateOrUpdateUserAsync(
            new ExternalAuthUserInfo
            {
                ProviderName = "LDAP",
                ProviderKey = username,
                Name = username
            }
        );
    }
}
```

### Configuração de Timeout
```json
{
  "Ldap": {
    "Server": "ldap.example.com",
    "Port": 389,
    "ConnectionTimeout": 30,
    "SearchTimeout": 60
  }
}
```

### Configuração de Atributos Personalizados
```json
{
  "Ldap": {
    "CustomAttributes": {
      "Department": "department",
      "Title": "title",
      "Phone": "telephoneNumber"
    }
  }
}
```

### Filtro de Usuários
```csharp
public override void PreInitialize()
{
    Configuration.Modules.EafLdap().UserFilter = "(objectClass=user)";
    Configuration.Modules.EafLdap().Enabled = true;
}
```

> Documentação completa: [`src/Eaf.Middleware.Ldap/README.md`](../../src/Eaf.Middleware.Ldap/README.md) e [`docs/modules/eaf-middleware-ldap.md`](./eaf-middleware-ldap.md)

## <a id="eafmiddlewarewebcore"></a>Eaf.Middleware.Web.Core

**Propósito resumido:** O **Eaf.Middleware.Web.Core** é o módulo web do Enterprise Application Foundation (EAF). Este módulo fornece componentes web para ASP.NET Core incluindo configuração de startup, middleware, filtros, controllers, SignalR, Swagger, health checks e integração HTTP.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Eaf.Middleware.Application 9.4.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Web.Core --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Web.Core\Eaf.Middleware.Web.Core.csproj" />
```

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

> Documentação completa: [`src/Eaf.Middleware.Web.Core/README.md`](../../src/Eaf.Middleware.Web.Core/README.md) e [`docs/modules/eaf-middleware-web-core.md`](./eaf-middleware-web-core.md)

## <a id="eafmiddlewareworker"></a>Eaf.Middleware.Worker

**Propósito resumido:** O **Eaf.Middleware.Worker** é um módulo de serviços em background do Enterprise Application Foundation (EAF). Este módulo fornece uma base para criar Worker Services do .NET que executam tarefas assíncronas, jobs agendados e processamento de longa duração, integrando-se perfeitamente com o ecossistema EAF e ABP.

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

> Documentação completa: [`src/Eaf.Middleware.Worker/README.md`](../../src/Eaf.Middleware.Worker/README.md) e [`docs/modules/eaf-middleware-worker.md`](./eaf-middleware-worker.md)

## <a id="eafopentelemetry"></a>Eaf.OpenTelemetry

**Propósito resumido:** O **Eaf.OpenTelemetry** é um módulo de observabilidade do Enterprise Application Foundation (EAF). Este módulo fornece implementação completa de OpenTelemetry para telemetria distribuída, tracing e métricas, permitindo monitoramento profundo de aplicações .NET com suporte a múltiplos exporters.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0

### Instalação via NuGet
```bash
dotnet add package Eaf.OpenTelemetry --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.OpenTelemetry\Eaf.OpenTelemetry.csproj" />
```

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

> Documentação completa: [`src/Eaf.OpenTelemetry/README.md`](../../src/Eaf.OpenTelemetry/README.md) e [`docs/modules/eaf-opentelemetry.md`](./eaf-opentelemetry.md)

## <a id="eafsqlservercache"></a>Eaf.SqlServerCache

**Propósito resumido:** O **Eaf.SqlServerCache** é um módulo de cache distribuído do Enterprise Application Foundation (EAF). Este módulo fornece implementação de cache distribuído usando SQL Server como backend, ideal para cenários de alta disponibilidade e multi-instances.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- SQL Server 2012 ou superior

### Instalação via NuGet
```bash
dotnet add package Eaf.SqlServerCache --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.SqlServerCache\Eaf.SqlServerCache.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `SqlServerCacheModule`:

```csharp
[DependsOn(
    typeof(SqlServerCacheModule),
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

### 2. Configurando o Cache

No `appsettings.json`:

```json
{
  "SqlServerCache": {
    "ConnectionString": "Server=localhost;Database=MyAppCache;Trusted_Connection=True;",
    "SchemaName": "dbo",
    "TableName": "Cache",
    "DefaultSlidingExpirationMinutes": 30,
    "EnableCompression": true
  }
}
```

### 3. Criando a Tabela de Cache

Execute o seguinte script SQL para criar a tabela de cache:

```sql
CREATE TABLE dbo.Cache (
    Id nvarchar(449) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
    Value varbinary(MAX) NOT NULL,
    ExpiresAtTime datetimeoffset NOT NULL,
    SlidingExpirationInSeconds bigint NULL,
    CONSTRAINT PK_Cache PRIMARY KEY CLUSTERED (Id)
);

CREATE NONCLUSTERED INDEX Index_ExpiresAtTime ON dbo.Cache(ExpiresAtTime);
```

### 4. Usando o Cache

```csharp
public class MyService : ApplicationService
{
    private readonly ICacheManager _cacheManager;

    public MyService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task<string> GetDataAsync(string key)
    {
        var cache = _cacheManager.GetCache("MyCache");
        return await cache.GetAsync<string>(key, async () =>
        {
            // Lógica para buscar dados quando não está em cache
            return await FetchFromDatabaseAsync(key);
        });
    }

    public async Task SetDataAsync(string key, string value)
    {
        var cache = _cacheManager.GetCache("MyCache");
        await cache.SetAsync(key, value, TimeSpan.FromMinutes(30));
    }
}
```

### 5. Configurando Expiração

```csharp
public class MyService : ApplicationService
{
    private readonly ICacheManager _cacheManager;

    public MyService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task CacheDataAsync(string key, string value)
    {
        var cache = _cacheManager.GetCache("MyCache");
        
        // Expiração absoluta
        await cache.SetAsync(key, value, TimeSpan.FromHours(1));
        
        // Expiração deslizante
        await cache.SetAsync(key, value, TimeSpan.FromMinutes(30), slidingExpiration: true);
    }
}
```

### 6. Configurando Multi-Tenancy

```csharp
public class MyService : ApplicationService
{
    private readonly ICacheManager _cacheManager;

    public MyService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task<string> GetTenantDataAsync(string key)
    {
        var cache = _cacheManager.GetCache("TenantCache");
        return await cache.GetAsync<string>(key, async () =>
        {
            // O cache automaticamente inclui o tenant ID na chave
            return await FetchTenantDataAsync(key);
        });
    }
}
```

### Configuração de Schema Personalizado
```csharp
public override void PreInitialize()
{
    Configuration.Modules.SqlServerCache().SchemaName = "cache";
    Configuration.Modules.SqlServerCache().TableName = "ApplicationCache";
}
```

### Configuração de Expiração Global
```csharp
public override void PreInitialize()
{
    Configuration.Modules.SqlServerCache().DefaultSlidingExpiration = TimeSpan.FromMinutes(30);
    Configuration.Modules.SqlServerCache().DefaultAbsoluteExpiration = TimeSpan.FromHours(1);
}
```

### Configuração de Compressão
```json
{
  "SqlServerCache": {
    "EnableCompression": true,
    "CompressionThreshold": 1024
  }
}
```

> Documentação completa: [`src/Eaf.SqlServerCache/README.md`](../../src/Eaf.SqlServerCache/README.md) e [`docs/modules/eaf-sqlservercache.md`](./eaf-sqlservercache.md)

## <a id="eafsqlitecache"></a>Eaf.SqliteCache

**Propósito resumido:** O **Eaf.SqliteCache** é um módulo de cache local do Enterprise Application Foundation (EAF). Este módulo fornece implementação de cache local usando SQLite como backend, ideal para cenários de desenvolvimento, testes e aplicações de baixa escala.

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0

### Instalação via NuGet
```bash
dotnet add package Eaf.SqliteCache --version 9.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.SqliteCache\Eaf.SqliteCache.csproj" />
```

### 1. Registrando o Módulo

No seu módulo principal, herde de `SqliteCacheModule`:

```csharp
[DependsOn(
    typeof(SqliteCacheModule),
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

### 2. Configurando o Cache

No `appsettings.json`:

```json
{
  "SqliteCache": {
    "ConnectionString": "Data Source=cache.db",
    "DefaultSlidingExpirationMinutes": 30,
    "EnableCompression": false
  }
}
```

### 3. Usando o Cache

```csharp
public class MyService : ApplicationService
{
    private readonly ICacheManager _cacheManager;

    public MyService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task<string> GetDataAsync(string key)
    {
        var cache = _cacheManager.GetCache("MyCache");
        return await cache.GetAsync<string>(key, async () =>
        {
            // Lógica para buscar dados quando não está em cache
            return await FetchFromDatabaseAsync(key);
        });
    }

    public async Task SetDataAsync(string key, string value)
    {
        var cache = _cacheManager.GetCache("MyCache");
        await cache.SetAsync(key, value, TimeSpan.FromMinutes(30));
    }
}
```

### 4. Configurando Expiração

```csharp
public class MyService : ApplicationService
{
    private readonly ICacheManager _cacheManager;

    public MyService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task CacheDataAsync(string key, string value)
    {
        var cache = _cacheManager.GetCache("MyCache");
        
        // Expiração absoluta
        await cache.SetAsync(key, value, TimeSpan.FromHours(1));
        
        // Expiração deslizante
        await cache.SetAsync(key, value, TimeSpan.FromMinutes(30), slidingExpiration: true);
    }
}
```

### 5. Cache em Ambiente de Desenvolvimento

```csharp
public override void PreInitialize()
{
    if (IsDevelopmentEnvironment())
    {
        Configuration.Modules.SqliteCache().ConnectionString = "Data Source=dev_cache.db";
    }
}
```

### 6. Cache para Testes

```csharp
public class MyTestService : ApplicationService
{
    private readonly ICacheManager _cacheManager;

    public MyTestService(ICacheManager cacheManager)
    {
        _cacheManager = cacheManager;
    }

    public async Task<string> GetTestDataAsync(string key)
    {
        var cache = _cacheManager.GetCache("TestCache");
        return await cache.GetAsync<string>(key, async () =>
        {
            return await GenerateTestDataAsync(key);
        });
    }
}
```

### Configuração de Caminho Personalizado
```csharp
public override void PreInitialize()
{
    Configuration.Modules.SqliteCache().ConnectionString = "Data Source=/path/to/custom/cache.db";
}
```

### Configuração de Expiração Global
```csharp
public override void PreInitialize()
{
    Configuration.Modules.SqliteCache().DefaultSlidingExpiration = TimeSpan.FromMinutes(30);
    Configuration.Modules.SqliteCache().DefaultAbsoluteExpiration = TimeSpan.FromHours(1);
}
```

### Configuração de Compressão
```json
{
  "SqliteCache": {
    "EnableCompression": true,
    "CompressionThreshold": 1024
  }
}
```

> Documentação completa: [`src/Eaf.SqliteCache/README.md`](../../src/Eaf.SqliteCache/README.md) e [`docs/modules/eaf-sqlitecache.md`](./eaf-sqlitecache.md)
