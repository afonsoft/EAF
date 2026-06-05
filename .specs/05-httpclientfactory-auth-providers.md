# 05 — Substituir `new HttpClient()` por IHttpClientFactory nos Auth Providers

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 1 — Performance `src/` |
| **Complexidade** | MÉDIA |
| **Risco** | BAIXO — Melhoria aditiva, sem alterar comportamento funcional |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | 3 arquivos de produção + 1 base class + testes |

## Objetivo

Substituir `new HttpClient()` por `IHttpClientFactory` nos 3 providers de autenticação externa (Microsoft, Google, AuthZero) para evitar socket exhaustion.

## Motivo

- **Socket exhaustion**: `new HttpClient()` em loop (por request) não reutiliza connections, causa exaustão de sockets TCP
- **DNS stale**: `HttpClient` manual não atualiza DNS — se IP do endpoint mudar, continua usando o antigo
- **Best practice**: Microsoft recomenda `IHttpClientFactory` desde .NET Core 2.1

## Arquivos Afetados

### Produção

**1. `src/Eaf.Middleware.Core/Authorization/External/Microsoft/MicrosoftAuthProviderApi.cs`**

```csharp
// ── ANTES (linhas 23-26 + 36) ──
public class MicrosoftAuthProviderApi : ExternalAuthProviderApiBase
{
    public MicrosoftAuthProviderApi(ILogger logger)
    {
        Logger = logger;
    }

    public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
    {
        using (HttpClient client = new HttpClient()) // ANTI-PATTERN
        { ... }
    }
}

// ── DEPOIS ──
public class MicrosoftAuthProviderApi : ExternalAuthProviderApiBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MicrosoftAuthProviderApi(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        Logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
    {
        using var client = _httpClientFactory.CreateClient("ExternalAuth");
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        client.Timeout = TimeSpan.FromSeconds(30.0);
        client.MaxResponseContentBufferSize = 10485760L;
        // ... resto do código mantido
    }
}
```

**2. `src/Eaf.Middleware.Core/Authorization/External/Google/GoogleAuthProviderApi.cs`**

```csharp
// Mesma mudança: adicionar IHttpClientFactory ao construtor, usar CreateClient("ExternalAuth")
public class GoogleAuthProviderApi : ExternalAuthProviderApiBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GoogleAuthProviderApi(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        Logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    // GetUserInfo: substituir `new HttpClient()` por `_httpClientFactory.CreateClient("ExternalAuth")`
}
```

**3. `src/Eaf.Middleware.Core/Authorization/External/AuthZero/AuthZeroAuthProviderApi.cs`**

```csharp
// Mesma mudança: adicionar IHttpClientFactory ao construtor, usar CreateClient("ExternalAuth")
public class AuthZeroAuthProviderApi : ExternalAuthProviderApiBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthZeroAuthProviderApi(ILogger logger, IHttpClientFactory httpClientFactory)
    {
        Logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    // GetUserInfo: substituir `new HttpClient()` por `_httpClientFactory.CreateClient("ExternalAuth")`
}
```

**4. Registrar HttpClient no DI (se necessário)**

Verificar se `IHttpClientFactory` já está registrado no DI container. Se não:

```csharp
// Em MiddlewareCoreModule.PreInitialize() ou Startup.ConfigureServices():
services.AddHttpClient("ExternalAuth", client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.MaxResponseContentBufferSize = 10485760L;
});
```

**NOTA**: Se a config de headers for centralizada no `AddHttpClient`, remover dos métodos `GetUserInfo`.

### Teste

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.Middleware.Core.Tests/Authorization/External/ExternalAuthProviderApiTests.cs

public class MicrosoftAuthProviderApiTests
{
    [Fact]
    public void Dado_HttpClientFactoryValido_Quando_CriarProvider_Entao_DeveInicializar()
    // Construtor não lança exceção com mocks válidos

    [Fact]
    public async Task Dado_AccessCodeValido_Quando_GetUserInfo_Entao_DeveUsarHttpClientFactory()
    // Mock IHttpClientFactory.CreateClient → verificar que CreateClient("ExternalAuth") foi chamado

    [Fact]
    public async Task Dado_RespostaValida_Quando_GetUserInfo_Entao_DeveRetornarUserInfo()
    // Mock resposta HTTP com JSON válido → verificar ExternalAuthUserInfo preenchido
}

public class GoogleAuthProviderApiTests
{
    [Fact]
    public async Task Dado_AccessCodeValido_Quando_GetUserInfo_Entao_DeveUsarHttpClientFactory()

    [Fact]
    public async Task Dado_EndpointNaoConfigurado_Quando_GetUserInfo_Entao_DeveLancarAbpException()
}

public class AuthZeroAuthProviderApiTests
{
    [Fact]
    public async Task Dado_AccessCodeValido_Quando_GetUserInfo_Entao_DeveUsarHttpClientFactory()

    [Fact]
    public async Task Dado_EndpointNaoConfigurado_Quando_GetUserInfo_Entao_DeveLancarAbpException()
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.Middleware.Core/Eaf.Middleware.Core.csproj --configuration Release
dotnet test test/Eaf.Middleware.Core.Tests/Eaf.Middleware.Core.Tests.csproj --collect:"XPlat Code Coverage"
```

## Critérios de Aceite

1. Zero instâncias de `new HttpClient()` nos 3 providers
2. Todos usam `IHttpClientFactory.CreateClient("ExternalAuth")`
3. `IHttpClientFactory` injetado via construtor (DI)
4. Todos os testes passam
5. Cobertura não diminuiu

## Notas para o Sub-Agent

- Castle Windsor (ABP DI) pode não registrar `IHttpClientFactory` automaticamente
- Verificar se `Microsoft.Extensions.Http` já é referenciado no `.csproj` do Eaf.Middleware.Core
- Se não, adicionar: `<PackageReference Include="Microsoft.Extensions.Http" Version="10.0.*" />`
- Os providers herdam de `ExternalAuthProviderApiBase` — verificar se a base class precisa de alteração
- `ExternalAuthProviderApiBase` é resolvida via `IocResolverExtensions.ResolveAsDisposable` no `ExternalAuthManager` — Castle Windsor resolve dependências automaticamente
- Se o sub-agent encontrar complexidade no DI registration (Castle vs MS DI), reportar e parar
