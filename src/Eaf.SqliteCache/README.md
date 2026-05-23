# Eaf.SqliteCache

## Descrição Técnica

O **Eaf.SqliteCache** é um módulo de cache local do Enterprise Application Foundation (EAF). Este módulo fornece implementação de cache local usando SQLite como backend, ideal para cenários de desenvolvimento, testes e aplicações de baixa escala.

Este módulo utiliza o SQLite como armazenamento persistente de cache, oferecendo uma solução leve e sem dependências de servidor de banco de dados, perfeita para ambientes de desenvolvimento e aplicações standalone.

## Relação com o EAF e ASP.NET Boilerplate

### Integração com ABP
- **Abp**: Framework base para injeção de dependência e configuração
- **Abp.Runtime.Caching**: Sistema de cache do ABP

### Dependências Externas
- **Microsoft.Extensions.Caching.Sqlite**: Implementação de cache SQLite da Microsoft
- **Microsoft.Data.Sqlite**: Cliente SQLite

### Principais Componentes

#### SqliteCache
Implementação de cache local com SQLite:
- Armazenamento persistente em arquivo
- Suporte a expiração de itens
- Performance otimizada para cenários locais
- Sem dependências de servidor externo

#### Cache Configuration
Configuração do cache:
- Caminho do arquivo SQLite
- Configurações de expiração
- Configurações de sliding expiration

## Guia de Instalação

### Pré-requisitos
- .NET 10.0 SDK ou superior
- ASP.NET Boilerplate 10.4.0

### Instalação via NuGet
```bash
dotnet add package Eaf.SqliteCache --version 10.4.0
```

### Instalação via Referência de Projeto
Adicione a referência ao seu arquivo `.csproj`:
```xml
<ProjectReference Include="..\Eaf.SqliteCache\Eaf.SqliteCache.csproj" />
```

## Exemplo Básico de Uso

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

## Estrutura do Módulo

```
Eaf.SqliteCache/
├── Runtime/               # Implementações de runtime do cache
└── Eaf.SqliteCache.csproj  # Projeto
```

## Configurações Opcionais

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

## Quando Usar SQLite Cache

### Cenários Ideais:
- **Desenvolvimento Local**: Cache rápido e sem dependências
- **Aplicações Standalone**: Aplicações que rodam em uma única instância
- **Testes Unitários**: Cache isolado para testes
- **Prototipagem**: Desenvolvimento rápido de protótipos

### Quando NÃO Usar:
- **Ambientes de Produção Multi-Instance**: Use SQL Server Cache ou Redis
- **Alta Disponibilidade**: SQLite não suporta múltiplas instâncias simultâneas
- **Cache Distribuído**: Use Redis ou SQL Server Cache

## Testes

Os testes para este módulo estão localizados em:
```
test/Eaf.SqliteCache.Tests/
```

Para executar os testes:
```bash
dotnet test test/Eaf.SqliteCache.Tests/Eaf.SqliteCache.Tests.csproj
```

**Cobertura Atual:** 75.5% linha, 34.9% branch (necessita expansão para atingir meta de 90%)

## Licença

Este projeto faz parte do Enterprise Application Foundation (EAF) e está licenciado sob os mesmos termos do projeto principal.

## Suporte

Para issues e perguntas, consulte o repositório principal do EAF:
https://github.com/afonsoft/EAF
