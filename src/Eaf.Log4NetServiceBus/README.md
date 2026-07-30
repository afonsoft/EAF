# Eaf.Log4NetServiceBus

## Descrição Técnica

O **Eaf.Log4NetServiceBus** é um módulo de integração do Enterprise Application Foundation (EAF). Este módulo fornece integração com Azure Service Bus usando log4net para logging de mensagens e eventos de mensageria.

Este módulo permite que aplicações EAF enviem e recebam mensagens através do Azure Service Bus, com logging estruturado automático de todas as operações de mensageria, facilitando o monitoramento e troubleshooting de sistemas distribuídos.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração

### Dependências Externas
- **Azure.Messaging.ServiceBus**: SDK do Azure Service Bus
- **log4net**: Framework de logging
- **log4net.Appender.AzureServiceBus**: Appender do log4net para Azure Service Bus

### Principais Componentes

#### ServiceBus Logger
Logger para Azure Service Bus:
- Envio de mensagens para filas e tópicos
- Recebimento de mensagens
- Logging automático de operações
- Tratamento de erros e retries

#### Log4Net Appender
Appender do log4net para Service Bus:
- Configuração de appenders
- Formatação de mensagens de log
- Envio de logs para Service Bus

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Azure Service Bus Namespace configurado

### Instalação via NuGet
```bash
dotnet add package Eaf.Log4NetServiceBus --version 9.4.1
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Log4NetServiceBus\Eaf.Log4NetServiceBus.csproj" />
```

## Exemplo Básico de Uso

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

## Estrutura do Módulo

```
Eaf.Log4NetServiceBus/
├── Logging/              # Implementações de logging
│   ├── ServiceBusLogger.cs
│   └── Log4NetServiceBusModule.cs
└── Eaf.Log4NetServiceBus.csproj  # Projeto
```

## Configurações Opcionais

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

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.Log4NetServiceBus.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.Log4NetServiceBus.Tests/Eaf.Log4NetServiceBus.Tests.csproj
```

**Cobertura Atual:** 85.0% (boa cobertura, mas pode ser expandida para atingir meta de 90%)

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF