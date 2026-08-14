# Eaf.RedisCache

Módulo de cache distribuído do EAF baseado em Redis, utilizando `StackExchange.Redis` através do `Microsoft.Extensions.Caching.StackExchangeRedis`.

## Instalação

Adicione a referência ao projeto `Eaf.RedisCache` e configure o módulo no seu `EafModule`:

```csharp
[DependsOn(typeof(EafRedisCacheModule))]
public class YourModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Caching.UseRedis(options =>
        {
            options.ConnectionString = "localhost:6379";
            options.InstanceName = "EAF";
        });
    }
}
```

A string de conexão também pode ser obtida do `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "Eaf": {
    "RedisCache": {
      "ConnectionString": "localhost:6379"
    }
  }
}
```

> Observação: a leitura automática do `appsettings.json` pode ser feita no módulo de inicialização do seu projeto (por exemplo `Eaf.Middleware.Web.Core`) antes de invocar `UseRedis`.

## Características

- Implementação de `ICacheManager` e `CacheBase` para o EAF.
- Serialização JSON com prefixo do tipo e compressão GZip (compatível com `Eaf.SqlServerCache`).
- Chaves prefixadas com `<InstanceName>:<CacheName>_`.
- Operação `Clear` com scan e remoção por prefixo (best-effort).
- Fail-open: falhas no Redis são logadas e não derrubam o host.

## Dependências

- `Microsoft.Extensions.Caching.StackExchangeRedis`
- `StackExchange.Redis`
- `Abp`

## Exemplo de uso

```csharp
var cacheManager = Resolve<ICacheManager>();
var cache = cacheManager.GetCache("Users");

cache.Set("user:1", new UserDto { Id = 1, Name = "Alice" });
var user = cache.Get("user:1", "default");
```

## Testes

Execute os testes com:

```bash
dotnet test test/Eaf.RedisCache.Tests
```

Para os testes de integração, é necessário um Redis disponível em `localhost:6379` (iniciado automaticamente em ambientes com Docker).
