# Eaf.KeyVault.AspNetCore

## Descrição Técnica

O **Eaf.KeyVault.AspNetCore** é um módulo de integração ASP.NET Core para o Eaf.KeyVault. Este módulo fornece carregamento automático de configurações e segredos do Azure Key Vault ou Oracle Cloud Infrastructure (OCI) Vault diretamente no sistema de configuração do ASP.NET Core.

Este módulo simplifica a integração de segredos gerenciados em nuvem com aplicações ASP.NET Core, permitindo que configurações sensíveis sejam carregadas automaticamente durante o startup da aplicação.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração

### Dependências do EAF
- **Eaf.KeyVault**: Módulo base de gerenciamento de segredos

### Dependências Externas
- **Microsoft.Extensions.Hosting.Abstractions**: Integração com hosting do ASP.NET Core
- **Microsoft.Extensions.Configuration.Abstractions**: Sistema de configuração do ASP.NET Core

### Principais Componentes

#### KeyVault Configuration Builder
Builder de configuração que carrega segredos do Key Vault:
- Integração com IConfigurationBuilder
- Carregamento automático durante startup
- Suporte a recarregamento de configurações

#### Hosting Integration
Integração com ASP.NET Core Hosting:
- Configuração automática no startup
- Injeção de dependência
- Gerenciamento de ciclo de vida

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.4.0
- Eaf.KeyVault 10.4.0
- Azure Key Vault ou OCI Vault configurado

### Instalação via NuGet
```bash
dotnet add package Eaf.KeyVault.AspNetCore --version 10.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.KeyVault.AspNetCore\Eaf.KeyVault.AspNetCore.csproj" />
```

## Exemplo Básico de Uso

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

## Estrutura do Módulo

```
Eaf.KeyVault.AspNetCore/
├── Hosting/               # Integração com ASP.NET Core Hosting
├── EafKeyVaultAspNetCoreModule.cs  # Módulo ABP
└── Eaf.KeyVault.AspNetCore.csproj  # Projeto
```

## Configurações Opcionais

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

## Testes

Os testes para este módulo devem ser criados seguindo o padrão dos outros módulos do EAF.

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF
