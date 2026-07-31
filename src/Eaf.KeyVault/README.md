# Eaf.KeyVault

## Descrição Técnica

O **Eaf.KeyVault** é um módulo de gerenciamento de segredos do Enterprise Application Foundation (EAF). Este módulo fornece integração com Azure Key Vault e Oracle Cloud Infrastructure (OCI) para armazenamento seguro de credenciais, chaves de API, strings de conexão e outros segredos sensíveis.

Este módulo abstrai a complexidade de acessar serviços de gerenciamento de segredos, fornecendo uma interface unificada que suporta múltiplos provedores de nuvem.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração
- **Microsoft.Extensions.Hosting.Abstractions**: Integração com hosting do ASP.NET Core

### Dependências Externas
- **Azure.Identity**: Autenticação com Azure AD
- **Azure.Security.KeyVault.Secrets**: Cliente de Key Vault do Azure
- **Azure.Extensions.AspNetCore.Configuration.Secrets**: Integração com configuração do ASP.NET Core
- **OCI.DotNetSDK.Identity**: SDK de identidade da Oracle Cloud
- **OCI.DotNetSDK.Secrets**: SDK de segredos da Oracle Cloud
- **Microsoft.IdentityModel.JsonWebTokens**: Processamento de tokens JWT
- **System.IdentityModel.Tokens.Jwt**: Validação de tokens JWT

### Principais Componentes

#### IKeyVaultSecretManager
Interface principal para gerenciamento de segredos, fornecendo métodos para:
- Recuperar segredos
- Definir segredos
- Remover segredos
- Listar segredos

#### KeyVaultSecretManager
Implementação concreta que suporta:
- Azure Key Vault
- Oracle Cloud Infrastructure (OCI) Vault
- Cache local de segredos para performance
- Retries automáticos em falhas

#### Hosting
Integração com ASP.NET Core Hosting para:
- Configuração automática no startup
- Injeção de dependência
- Configuração de logging

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- Azure Key Vault ou OCI Vault configurado
- Credenciais de acesso (Azure AD ou OCI)

### Instalação via NuGet
```bash
dotnet add package Eaf.KeyVault --version 9.4.2
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.KeyVault\Eaf.KeyVault.csproj" />
```

## Exemplo Básico de Uso

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

## Estrutura do Módulo

```
Eaf.KeyVault/
├── Hosting/               # Integração com ASP.NET Core Hosting
├── KeyVault/             # Implementações específicas do KeyVault
├── IKeyVaultSecretManager.cs  # Interface principal
├── KeyVaultSecretManager.cs   # Implementação concreta
└── EafKeyVaultModule.cs   # Módulo ABP
```

## Configurações Opcionais

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

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.KeyVault.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.KeyVault.Tests/Eaf.KeyVault.Tests.csproj
```

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF