# Eaf.SignalR

Módulo de integração SignalR do EAF (Enterprise Application Foundation) para ASP.NET Core.

## Descrição

O `Eaf.SignalR` encapsula a infraestrutura real-time do EAF, reutilizando `Abp.AspNetCore.SignalR` e adicionando configurações específicas, notificações e gerenciamento de clientes online.

## Componentes principais

- `EafSignalRModule` — módulo ABP com as dependências e registros do SignalR.
- `EafCommonHub` — hub comum do EAF, mapeado em `/signalr`.
- `EafSignalRRealTimeNotifier` — notificador em tempo real via SignalR.
- `EafOnlineClientManager` / `EafInMemoryOnlineClientStore` — gerenciadores de clientes online.
- `AddEafSignalR` — extensão para configurar o SignalR com Redis backplane.

## Dependências

- `Abp.AspNetCore.SignalR` 10.5.0
- `Microsoft.AspNetCore.SignalR.StackExchangeRedis` 10.0.7
- `Abp` 10.5.0

## Instalação

```bash
dotnet add package Eaf.SignalR
```

No módulo Web, substitua `AbpAspNetCoreSignalRModule` por `EafSignalRModule`:

```csharp
[DependsOn(typeof(EafSignalRModule))]
public class MyWebModule : AbpModule { }
```

E utilize a extensão no `Startup`:

```csharp
services.AddEafSignalR(configuration);
```

## Configuração

```json
{
  "EafSignalR": {
    "UseDetailedErrors": null,
    "HandshakeTimeoutSeconds": 30,
    "KeepAliveIntervalSeconds": 30,
    "ClientTimeoutIntervalSeconds": 60,
    "UseRedisBackplane": false,
    "RedisConnectionString": "",
    "RedisDatabase": null
  }
}
```

## Uso

Mapeie o `EafCommonHub` e o `ChatHub` no `Startup`:

```csharp
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<EafCommonHub>("/signalr");
    endpoints.MapHub<ChatHub>("/signalr-chat");
});
```

## Testes

```bash
dotnet test test/Eaf.SignalR.Tests
```

## Licença

GPL-3.0-or-later
