# Eaf.Middleware.AzureActiveDirectory

## Descrição Técnica

O **Eaf.Middleware.AzureActiveDirectory** é um módulo de autenticação Azure Active Directory do Enterprise Application Foundation (EAF). Este módulo fornece integração completa com Azure AD para autenticação externa e sincronização de usuários, permitindo que usuários autentiquem usando suas credenciais Microsoft 365.

Este módulo suporta OpenID Connect, OAuth 2.0 e sincronização automática de usuários, grupos e propriedades do Azure AD.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração
- **Abp.Zero.Common**: Funcionalidades comuns do ABP Zero

### Dependências Externas
- **Microsoft.Identity.Web**: Biblioteca principal para autenticação Microsoft
- **Microsoft.Identity.Web.MicrosoftGraph**: Integração com Microsoft Graph API
- **Microsoft.Graph**: SDK do Microsoft Graph

### Principais Componentes

#### AzureActiveDirectoryAuthenticationSource
Implementação de autenticação externa via Azure AD:
- Integração com OpenID Connect
- Validação de tokens JWT
- Sincronização de usuários
- Mapeamento de claims

#### AzureActiveDirectorySettings
Configurações de conexão Azure AD:
- Tenant ID
- Client ID
- Client Secret
- Callback URL
- Scopes e permissões

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Azure AD Tenant configurado
- App Registration no Azure AD

### Instalação via NuGet
```bash
dotnet add package Eaf.Middleware.AzureActiveDirectory --version 10.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.Middleware.AzureActiveDirectory\Eaf.Middleware.AzureActiveDirectory.csproj" />
```

## Exemplo Básico de Uso

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

## Estrutura do Módulo

```
Eaf.Middleware.AzureActiveDirectory/
├── AzureActiveDirectory/  # Implementações Azure AD
│   ├── AzureActiveDirectoryAuthenticationSource.cs
│   ├── AzureActiveDirectorySettings.cs
│   └── AzureActiveDirectoryUserManager.cs
└── MiddlewareAzureActiveDirectoryModule.cs  # Módulo ABP
```

## Configurações Opcionais

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

## Testes

Os testes para este módulo devem ser criados seguindo o padrão dos outros módulos do EAF.

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF
