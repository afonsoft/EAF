# 86 — Corrigir Error Handling no ServiceBusQueueAppender (SRP + Correctness)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 5 — SOLID / Clean Architecture |
| **Princípio** | SRP + Error Handling + Async Correctness |
| **Complexidade** | BAIXA |
| **Risco** | BAIXO — Módulo isolado de logging |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | 1 arquivo de produção |

## Objetivo

Corrigir 4 problemas no `ServiceBusQueueAppender`:
1. **Fire-and-forget** — `queueClient.SendAsync(messages)` sem `await` (linha 80)
2. **Exception swallowing** — `catch (Exception) { //bypass }` (linhas 82-85)
3. **`async void`** — `OnClose` é `async void` (linha 88)
4. **`Task.Run` desnecessário** — `SendBuffer` envolve chamada síncrona em `Task.Run` (linha 98)

## Motivo

```csharp
// ANTES (src/Eaf.Log4NetServiceBus/Logging/ServiceBusQueueAppender.cs):

// Problema 1: Fire-and-forget — mensagens podem ser silenciosamente perdidas
queueClient.SendAsync(messages); // linha 80 — Task ignorada!

// Problema 2: Exception swallowing — bugs ficam invisíveis
catch (Exception)
{
    //bypass    // linha 82-85 — TUDO engolido, inclusive ArgumentNullException
}

// Problema 3: async void — exceção crasheia o processo
protected override async void OnClose() // linha 88 — async void!

// Problema 4: Task.Run — aloca thread pool para envolver chamada sync-friendly
protected override void SendBuffer(LoggingEvent[] events)
{
    Task.Run(() => AppendBuffer(events)); // linha 98 — Task.Run desnecessário
}
```

## Refatoração Proposta

```csharp
// ARQUIVO: src/Eaf.Log4NetServiceBus/Logging/ServiceBusQueueAppender.cs

// ── PROBLEMA 1 + 2: Fire-and-forget + Exception swallowing ──
protected void AppendBuffer(LoggingEvent[] events)
{
    try
    {
        if (string.IsNullOrEmpty(ConnectionString) || string.IsNullOrEmpty(QueueName)
            || string.IsNullOrEmpty(ApplicationName) || string.IsNullOrEmpty(StorageType))
            return;

        QueueClient queueClient = null;

        lock (_sync)
        {
            _serviceBusConnection ??= new ServiceBusConnection(ConnectionString);
            queueClient = new QueueClient(_serviceBusConnection, QueueName, ReceiveMode.PeekLock, RetryPolicy.Default);
        }

        var messages = new List<Message>();

        foreach (var loggingEvent in events)
        {
            var paramsFull = RenderLoggingEvent(loggingEvent);
            var log = new LogMessage()
            {
                StorageType = StorageType,
                EventDateUTC = DateTime.UtcNow,
                PurgeDateUTC = DateTime.UtcNow.AddDays(RetentionTime),
                RetentionTime = RetentionTime,
                ApplicationName = ApplicationName,
                Level = GetParams(0, paramsFull),
                ServerName = GetParams(1, paramsFull),
                Event = GetParams(2, paramsFull),
                Message = GetParams(3, paramsFull),
                JsonData = GetParams(4, paramsFull)
            };

            messages.Add(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(log))));
        }

        // FIX 1: Aguardar envio — não usar fire-and-forget
        queueClient.SendAsync(messages).GetAwaiter().GetResult();
    }
    catch (ServiceBusException ex)
    {
        // FIX 2: Log de erros de Service Bus (esperados — conectividade, throttling)
        System.Diagnostics.Debug.WriteLine($"ServiceBusQueueAppender: Service Bus error: {ex.Message}");
    }
    catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
    {
        // FIX 2: Log de erros inesperados — não engolir silenciosamente
        System.Diagnostics.Debug.WriteLine($"ServiceBusQueueAppender: Unexpected error: {ex.Message}");
    }
}

// ── PROBLEMA 3: async void → sync com try/catch ──
// NOTA: OnClose é chamado pelo log4net framework, que não suporta async
protected override void OnClose()
{
    try
    {
        if (_serviceBusConnection != null && !_serviceBusConnection.IsClosedOrClosing)
        {
            _serviceBusConnection.CloseAsync().GetAwaiter().GetResult();
        }
    }
    catch (Exception ex) when (ex is not OutOfMemoryException and not StackOverflowException)
    {
        System.Diagnostics.Debug.WriteLine($"ServiceBusQueueAppender: Error on close: {ex.Message}");
    }

    base.OnClose();
}

// ── PROBLEMA 4: Task.Run desnecessário ──
protected override void SendBuffer(LoggingEvent[] events)
{
    // FIX 4: Chamada direta — AppendBuffer é sync, Task.Run desperdiça thread
    AppendBuffer(events);
}

// ── Bonus: Renomear método para PascalCase ──
private string GetParams(int index, string message)
{
    // ... (era getParams — não segue C# naming conventions)
}
```

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.Log4NetServiceBus.Tests/ServiceBusQueueAppenderTests.cs

public class ServiceBusQueueAppenderBddTests
{
    [Fact]
    public void Dado_ConfiguracoesVazias_Quando_AppendBuffer_Entao_NaoDeveLancarExcecao()

    [Fact]
    public void Dado_ConnectionStringNull_Quando_AppendBuffer_Entao_DeveRetornarSemErro()

    [Fact]
    public void Dado_EventosValidos_Quando_AppendBuffer_Entao_DeveEnviarMensagens()

    [Fact]
    public void Dado_ServiceBusIndisponivel_Quando_AppendBuffer_Entao_DeveLogarErroSemLancar()

    [Fact]
    public void Dado_Appender_Quando_OnClose_Entao_DeveFecharConexaoSemAsyncVoid()

    [Fact]
    public void Dado_Appender_Quando_SendBuffer_Entao_NaoDeveUsarTaskRun()
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.Log4NetServiceBus/Eaf.Log4NetServiceBus.csproj --configuration Release
dotnet test test/Eaf.Log4NetServiceBus.Tests/Eaf.Log4NetServiceBus.Tests.csproj --collect:"XPlat Code Coverage"
dotnet build Eaf.sln --configuration Release
```

## Critérios de Aceite

1. `SendAsync` aguardado (`.GetAwaiter().GetResult()`) — não fire-and-forget
2. `catch (Exception)` substituído por catches tipados + logging
3. `async void OnClose` substituído por método síncrono com try/catch
4. `Task.Run` removido do `SendBuffer`
5. `getParams` renomeado para `GetParams` (PascalCase)
6. Todos os testes passam
7. XML docs mantidos/atualizados

## Notas para o Sub-Agent

- O `log4net` framework é inherentemente síncrono — `SendBuffer` e `OnClose` são chamados de forma síncrona
- `.GetAwaiter().GetResult()` é aceitável aqui porque log4net não suporta async
- `System.Diagnostics.Debug.WriteLine` é proposital — o appender É o sistema de logging, não pode usar a si mesmo
- NÃO usar `ILogger` dentro do appender — criaria dependência circular
- Se testes de Service Bus precisam de mock, usar `NSubstitute` para `IQueueClient`
- Se o package `Microsoft.Azure.ServiceBus` for muito antigo, documentar (mas não atualizar nesta tarefa)
