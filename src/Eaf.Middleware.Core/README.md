# Eaf.Middleware.Core

## Descrição Técnica

O **Eaf.Middleware.Core** é a camada de domínio central do Enterprise Application Foundation (EAF). Este módulo fornece as entidades, serviços, configurações, autorização, auditoria e funcionalidades base do framework, servindo como fundação para todos os outros módulos do EAF.

Este módulo implementa os padrões de Domain-Driven Design (DDD) e segue a arquitetura em camadas do ASP.NET Boilerplate (ABP).

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp.ZeroCore**: Fornece infraestrutura para multi-tenancy, usuários, roles e permissões
- **Abp.AutoMapper**: Configuração automática de mapeamento de objetos
- **Abp.MailKit**: Integração para envio de emails
- **Abp.HangFire.AspNetCore**: Processamento de jobs em background

### Dependências do EAF
- **Eaf.Middleware.AzureActiveDirectory**: Integração com Azure AD para autenticação externa
- **Eaf.Middleware.Ldap**: Autenticação via LDAP/Active Directory

### Principais Componentes

#### Autorização
- Gerenciamento de permissões e roles
- Autorização baseada em claims
- Integração com Azure AD e LDAP

#### Auditoria
- Rastreamento automático de operações
- Logs de alterações de entidades
- Histórico de ações dos usuários

#### Configurações
- Sistema de configurações multi-tenant
- Definições de features
- Gerenciamento de edições

#### Chat e Amizades
- Sistema de chat em tempo real
- Gerenciamento de amizades entre usuários
- Integração com SignalR

#### Hangfire
- Configuração avançada de jobs com seleção automática de storage
- Suporte a SQL Server, Redis (`Hangfire.Redis.StackExchange`) e InMemory
- Enum `HangfireStorageType` para identificação do tipo de armazenamento
- Logging aprimorado com Hangfire.Console
- Heartbeat para monitoramento

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.Core --version 9.4.1
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.Core\Eaf.Middleware.Core.csproj" />
```

## Exemplo Básico de Uso

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

## Estrutura do Módulo

```
Eaf.Middleware.Core/
├── Auditing/              # Configurações de auditoria
├── Authorization/         # Sistema de autorização e permissões
├── Cache/                 # Configurações de cache
├── Chat/                  # Sistema de chat
├── Configuration/         # Sistema de configurações
├── Editions/              # Gerenciamento de edições
├── Features/              # Sistema de features
├── Friendships/           # Gerenciamento de amizades
├── Hangfire/              # Configuração de Hangfire
├── Identity/              # Configurações de identidade
├── Localization/          # Arquivos de localização
├── MultiTenancy/         # Configurações multi-tenant
├── Net/                   # Utilitários de rede
└── Extensions/            # Extensões do framework
```

## Configurações Opcionais

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

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.MiddlewareCore.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.MiddlewareCore.Tests/Eaf.MiddlewareCore.Tests.csproj
```

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF