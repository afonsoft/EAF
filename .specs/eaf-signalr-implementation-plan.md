# Plano de Implementação — Eaf.SignalR

## Objetivo

Criar o módulo `Eaf.SignalR` para concentrar infraestrutura real-time do EAF, reutilizando `Abp.AspNetCore.SignalR` (sem copiá-lo) e adicionando configuração EAF, notificador e gerenciamento de clientes online. O módulo deve ser referenciado corretamente por `Eaf.Middleware.Web.Core` e pelos templates.

## Princípios

- **Não copiar ABP**: herdar `AbpCommonHub`, `OnlineClientHubBase`, `OnlineClientManager`, `InMemoryOnlineClientStore`.
- **Não alterar contratos**: métodos do cliente (`getNotification`, `register`) e rotas (`/signalr`, `/signalr-chat`) permanecem iguais.
- **Referências corretas**: `Eaf.Middleware.Web.Core` depende de `Eaf.SignalR`; `Eaf.SignalR` depende de `Abp.AspNetCore.SignalR`.

## O que já existe e será movido/reaproveitado

| Onde hoje | O que é | Destino |
|---|---|---|
| `src/Eaf.Middleware.Application/RealTime/OnlineClientManager.cs` | `OnlineClientManager<T>` wrapper ABP | `src/Eaf.SignalR/RealTime/EafOnlineClientManager.cs` (genérico + não-genérico) |
| `src/Eaf.Middleware.Application/RealTime/InMemoryOnlineClientStore.cs` | `InMemoryOnlineClientStore<T>` wrapper ABP | `src/Eaf.SignalR/RealTime/EafInMemoryOnlineClientStore.cs` (genérico + não-genérico) |
| `src/Eaf.Middleware.Application/MiddlewareApplicationModule.cs` | Registrations `IOnlineClientStore<ChatChannel>` / `IOnlineClientManager<ChatChannel>` | `EafSignalRModule` |
| `src/Eaf.Middleware.Application/Friendships/ChatUserStateWatcher.cs` | Chamada `.Initialize()` | `MiddlewareWebCoreModule.PostInitialize` |
| `Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs` | `services.AddSignalR(...)` | `services.AddEafSignalR(configuration)` |
| Templates `Startup.cs` | `endpoints.MapHub<AbpCommonHub>("/signalr")` | `endpoints.MapHub<EafCommonHub>("/signalr")` |

## Novos arquivos

```text
src/Eaf.SignalR/
  Eaf.SignalR.csproj
  EafSignalRModule.cs
  README.md
  Configuration/
    EafSignalROptions.cs
    EafSignalRServiceCollectionExtensions.cs
  Hubs/
    EafCommonHub.cs
  Notifications/
    EafSignalRRealTimeNotifier.cs
  RealTime/
    EafOnlineClientManager.cs
    EafInMemoryOnlineClientStore.cs

test/Eaf.SignalR.Tests/
  Eaf.SignalR.Tests.csproj
  EafSignalRTestModule.cs
  EafSignalRModuleTests.cs
  Notifications/
    EafSignalRRealTimeNotifierTests.cs
  RealTime/
    EafOnlineClientManagerTests.cs
  Configuration/
    EafSignalRServiceCollectionExtensionsTests.cs
```

## Estrutura do módulo

### `EafSignalROptions`

```csharp
public class EafSignalROptions
{
    public bool? UseDetailedErrors { get; set; }
    public int HandshakeTimeoutSeconds { get; set; } = 30;
    public int KeepAliveIntervalSeconds { get; set; } = 30;
    public int ClientTimeoutIntervalSeconds { get; set; } = 60;
    public bool UseRedisBackplane { get; set; }
    public string RedisConnectionString { get; set; } = string.Empty;
    public int? RedisDatabase { get; set; }
}
```

### `AddEafSignalR` (substitui `services.AddSignalR`)

- Lê `EafSignalR` do `IConfiguration`.
- `UseDetailedErrors` → config ou ambiente.
- Configura `HandshakeTimeout`, `KeepAliveInterval`, `ClientTimeoutInterval`.
- Se `UseRedisBackplane` true, usa `RedisConnectionString` ou fallback `RedisCache:ConnectionString` e chama `AddStackExchangeRedis`.
- Registra `EafSignalROptions` via `services.Configure`.

### `EafSignalRModule`

```csharp
[DependsOn(typeof(AbpAspNetCoreSignalRModule))]
public class EafSignalRModule : AbpModule
{
    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(EafSignalRModule).GetAssembly());

        // Cliente online default
        IocManager.IocContainer.Register(
            Component.For(typeof(IOnlineClientStore<>)).ImplementedBy(typeof(EafInMemoryOnlineClientStore<>)).LifestyleSingleton().IsDefault(),
            Component.For(typeof(IOnlineClientManager<>)).ImplementedBy(typeof(EafOnlineClientManager<>)).LifestyleSingleton().IsDefault(),
            Component.For<IOnlineClientStore>().ImplementedBy<EafInMemoryOnlineClientStore>().LifestyleSingleton().IsDefault(),
            Component.For<IOnlineClientManager>().ImplementedBy<EafOnlineClientManager>().LifestyleSingleton().IsDefault()
        );

        // Hub comum e notificador
        IocManager.IocContainer.Register(Component.For<EafCommonHub>().LifestyleTransient());
        IocManager.IocContainer.Register(
            Component.For<IRealTimeNotifier>().ImplementedBy<EafSignalRRealTimeNotifier>().LifestyleTransient().IsDefault()
        );

        // Substitui notificador ABP
        Configuration.Notifications.Notifiers.Remove<SignalRRealTimeNotifier>();
        Configuration.Notifications.Notifiers.Add<EafSignalRRealTimeNotifier>();
    }
}
```

### `EafCommonHub`

```csharp
public class EafCommonHub : AbpCommonHub
{
    public EafCommonHub(IOnlineClientManager onlineClientManager, IOnlineClientInfoProvider clientInfoProvider)
        : base(onlineClientManager, clientInfoProvider) { }
}
```

### `EafSignalRRealTimeNotifier`

- Implementa `IRealTimeNotifier, ITransientDependency`.
- Recebe `IOnlineClientManager` (não-genérico) e `IHubContext<EafCommonHub>`.
- `SendNotificationsAsync` envia `getNotification` para cada conexão do destinatário, com log silencioso em falhas.

### `EafOnlineClientManager` / `EafInMemoryOnlineClientStore`

- Não-genéricos herdam direto de ABP.
- Genéricos `EafOnlineClientManager<T>` e `EafInMemoryOnlineClientStore<T>` implementam `IOnlineClientManager<T>` / `IOnlineClientStore<T>`.
- Mantêm thread-safety e eventos `UserConnected`/`UserDisconnected` do ABP.

## Ajustes nos projetos existentes

### `Eaf.Middleware.Application`

- Remover pasta `RealTime/` (`OnlineClientManager.cs`, `InMemoryOnlineClientStore.cs`).
- Em `MiddlewareApplicationModule.PostInitialize`:
  - Manter `IocManager.RegisterIfNot<IChatCommunicator, NullChatCommunicator>();`
  - Manter `IocManager.RegisterIfNot<IAuditingStore, AuditingStore>(DependencyLifeStyle.Singleton);`
  - Remover registrations de `IOnlineClientStore<ChatChannel>` e `IOnlineClientManager<ChatChannel>`.
  - Remover `IocManager.Resolve<ChatUserStateWatcher>().Initialize();`.

### `Eaf.Middleware.Web.Core`

- `MiddlewareWebCoreModule`:
  - `DependsOn`: substituir `typeof(AbpAspNetCoreSignalRModule)` por `typeof(EafSignalRModule)`.
  - `PostInitialize`: adicionar `IocManager.Resolve<ChatUserStateWatcher>().Initialize();`.
- `EafServiceCollectionMiddlewareExtensions`:
  - Substituir bloco `services.AddSignalR(...)` por `services.AddEafSignalR(configuration);`.
- `Eaf.Middleware.Web.Core.csproj`:
  - Adicionar `<ProjectReference Include="..\Eaf.SignalR\Eaf.SignalR.csproj" />`.
  - Remover `<PackageReference Include="Abp.AspNetCore.SignalR" />` (mantido transitivamente via `Eaf.SignalR`).

### Templates

- `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs`:
  - Alterar `endpoints.MapHub<AbpCommonHub>("/signalr")` para `endpoints.MapHub<EafCommonHub>("/signalr")`.
- `Templates/Api/test/Eaf.ProjectName.Web.Tests/Startup.cs`: idem.
- `docs/aspnetboilerplate/SignalR-AspNetCore-Integration.md`: atualizar exemplo.

### Testes

- `Eaf.Middleware.Application.Tests/Middleware/MiddlewareApplicationModuleIntegrationTests.cs`:
  - Remover asserts de `IOnlineClientStore<ChatChannel>` e `IOnlineClientManager<ChatChannel>`.
- `Eaf.SignalR.Tests`:
  - Lifecycle do `EafSignalRModule` sem erros.
  - `EafOnlineClientManager` adiciona/remove/retorna clientes por usuário.
  - `EafSignalRRealTimeNotifier` envia notificação para conexões ativas e ignora usuários offline.
  - `AddEafSignalR` aplica opções e ativa Redis backplane quando configurado.

### Solução

- `dotnet sln add src/Eaf.SignalR/Eaf.SignalR.csproj`
- `dotnet sln add test/Eaf.SignalR.Tests/Eaf.SignalR.Tests.csproj`

## Configuração sugerida em `appsettings.json`

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

## Verificação

1. `dotnet build Eaf.sln --configuration Release`
2. `dotnet test test/Eaf.SignalR.Tests --configuration Release --no-build`
3. `dotnet test test/Eaf.Middleware.Web.Core.Tests --configuration Release --no-build`
4. `dotnet test test/Eaf.Middleware.Application.Tests --configuration Release --no-build`
5. `dotnet test Eaf.sln --configuration Release --no-build` (regressão completa)

## Riscos e mitigação

| Risco | Mitigação |
|---|---|
| `EafCommonHub` não ser mapeado em templates legados | Atualizar todos os `Startup.cs` que mapeiam `/signalr` |
| Conflito de `IRealTimeNotifier` com ABP | Substituir tipo na lista `Configuration.Notifications.Notifiers` e marcar EAF como `IsDefault` |
| `IOnlineClientManager` não resolvido no `ChatUserStateWatcher` | Registrations genéricos e não-genéricos no `EafSignalRModule` |
| Testes existentes quebrarem por falta de `IOnlineClientStore<ChatChannel>` | Ajustar asserts no `MiddlewareApplicationModuleIntegrationTests` |

## Branch sugerida

`feature/eaf-signalr`
