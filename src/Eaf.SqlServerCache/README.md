# Eaf.SqlServerCache

## Descrição Técnica

O **Eaf.SqlServerCache** é um módulo de cache distribuído do Enterprise Application Foundation (EAF). Este módulo fornece implementação de cache distribuído usando SQL Server como backend, ideal para cenários de alta disponibilidade e multi-instances.

Este módulo permite que múltiplas instâncias da aplicação compartilhem o mesmo cache através do SQL Server, garantindo consistência e escalabilidade em ambientes distribuídos.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração
- **Abp.Runtime.Caching**: Sistema de cache do ABP

### Dependências Externas
- **Microsoft.Extensions.Caching.SqlServer**: Implementação de cache SQL Server da Microsoft
- **System.Data.SqlClient**: Cliente SQL Server

### Principais Componentes

#### SqlServerCache
Implementação de cache distribuído com SQL Server:
- Armazenamento persistente de cache
- Suporte a expiração de itens
- Compressão opcional de dados
- Performance otimizada

#### Cache Configuration
Configuração do cache:
- Connection string
- Schema e tabela personalizados
- Intervalo de expiração padrão
- Configurações de sliding expiration

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.5.0
- SQL Server 2012 ou superior

### Instalação via NuGet
```bash
dotnet add package Eaf.SqlServerCache --version 9.4.2
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.SqlServerCache\Eaf.SqlServerCache.csproj" />
```

## Exemplo Básico de Uso

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

## Estrutura do Módulo

```
Eaf.SqlServerCache/
├── Runtime/               # Implementações de runtime do cache
└── Eaf.SqlServerCache.csproj  # Projeto
```

## Configurações Opcionais

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

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.SqlServerCache.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.SqlServerCache.Tests/Eaf.SqlServerCache.Tests.csproj
```

**Cobertura Atual:** 86.6% (boa cobertura, mas pode ser expandida para atingir meta de 90%)

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF