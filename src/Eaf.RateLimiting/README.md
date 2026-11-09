# Eaf.RateLimiting

Módulo de rate limiting por IP ou usuário autenticado, com regras configuráveis por endpoint.

## Uso

```csharp
[DependsOn(typeof(EafRateLimitingModule))]
public class MyModule : AbpModule { }
```

```csharp
// ConfigureServices do host:
services.Configure<EafRateLimitingOptions>(o => o.Rules.Add(new EafRateLimitRule
{
    Endpoint = "/api/auth/*",
    Period = "1m",
    Limit = 10
}));

// Configure do host (após UseAuthentication):
app.UseEafRateLimiting();
```

Períodos aceitos: `Ns`, `Nm`, `Nh`, `Nd`. Store padrão em memória (`InMemoryRateLimitingStore`); implemente `IRateLimitingStore` para backends distribuídos.
