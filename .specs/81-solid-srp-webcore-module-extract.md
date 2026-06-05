# 81 — Extrair Responsabilidades do MiddlewareWebCoreModule (SRP)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 5 — SOLID / Clean Architecture |
| **Princípio** | SRP — Single Responsibility Principle |
| **Complexidade** | ALTA |
| **Risco** | ALTO — Modifica módulo central do framework, pode quebrar startup |
| **Dependências** | Executar APÓS tarefa 80 (Service Locator) |
| **Arquivos Modificados** | 1 arquivo refatorado + 3 novos configurers |

## Objetivo

Extrair 7 responsabilidades distintas do `MiddlewareWebCoreModule` (323 linhas) em configurers dedicados.

## Motivo

- **SRP violado**: Uma classe faz: Hangfire setup, Redis cache, SQL cache, external auth, audit config, app folders, background workers
- **Testabilidade**: Impossível testar uma responsabilidade sem carregar todas
- **Manutenibilidade**: 323 linhas com config de 7 subsistemas diferentes

## Análise das Responsabilidades Atuais

| # | Responsabilidade | Linhas | Método |
|---|-----------------|--------|--------|
| 1 | Environment detection | 60-74 | Construtor |
| 2 | Controller registration | 189-200 | PreInitialize |
| 3 | Hangfire storage setup | 93-183 | PostInitialize |
| 4 | Redis cache config | 220-234 | PreInitialize |
| 5 | SQL Server cache config | 236-246 | PreInitialize |
| 6 | External auth providers | 99 + ConfigureExternalAuthProviders() | PostInitialize |
| 7 | Audit/EntityHistory config | 248-253 | PreInitialize |

## Arquivos Afetados

### Refatoração Proposta

**Manter em `MiddlewareWebCoreModule.cs`:**
- Construtor (environment detection) — responsabilidade do módulo
- Controller registration — responsabilidade do módulo
- `Initialize()` — registro de assemblies
- Chamadas delegadas para os configurers

**Extrair para novos arquivos:**

#### 1. `src/Eaf.Middleware.Web.Core/Configuration/CacheConfigurer.cs` (NOVO)

```csharp
namespace Eaf.Middleware.Web.Configuration
{
    /// <summary>
    /// Configures cache providers (Redis, SQL Server) based on application settings.
    /// </summary>
    internal static class CacheConfigurer
    {
        /// <summary>
        /// Configures the cache subsystem based on application configuration.
        /// Supports Redis and SQL Server cache providers.
        /// </summary>
        /// <param name="configuration">ABP startup configuration.</param>
        /// <param name="appConfiguration">Application configuration root.</param>
        /// <param name="iocManager">IoC container manager.</param>
        public static void Configure(
            IAbpStartupConfiguration configuration,
            IConfigurationRoot appConfiguration,
            IIocManager iocManager)
        {
            // Configuration for all caches
            configuration.Caching.ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
            });

            ConfigureRedis(configuration, appConfiguration, iocManager);
            ConfigureSqlServer(configuration, appConfiguration);
        }

        private static void ConfigureRedis(
            IAbpStartupConfiguration configuration,
            IConfigurationRoot appConfiguration,
            IIocManager iocManager)
        {
            // Extrair linhas 220-234 do módulo atual
        }

        private static void ConfigureSqlServer(
            IAbpStartupConfiguration configuration,
            IConfigurationRoot appConfiguration)
        {
            // Extrair linhas 236-246 do módulo atual
        }
    }
}
```

#### 2. `src/Eaf.Middleware.Web.Core/Configuration/ExternalAuthConfigurer.cs` (NOVO)

```csharp
namespace Eaf.Middleware.Web.Configuration
{
    /// <summary>
    /// Configures external authentication providers (Google, Microsoft, AuthZero).
    /// </summary>
    internal static class ExternalAuthConfigurer
    {
        /// <summary>
        /// Registers external authentication providers based on application settings.
        /// </summary>
        public static void Configure(
            IIocManager iocManager,
            IConfigurationRoot appConfiguration)
        {
            // Extrair o método ConfigureExternalAuthProviders() existente
        }
    }
}
```

#### 3. Hangfire já tem `HangFireConfigurer.cs` — apenas mover lógica de PostInitialize

O `HangFireConfigurer.cs` já existe em `src/Eaf.Middleware.Web.Core/Startup/HangFireConfigurer.cs`. A lógica de Hangfire storage do `PostInitialize` (linhas 93-183) deve ser delegada para este configurer.

```csharp
// Adicionar método a HangFireConfigurer:
/// <summary>
/// Configures Hangfire job storage and cleanup outdated jobs.
/// </summary>
public static void ConfigureJobStorage(
    IAbpStartupConfiguration configuration,
    IConfigurationRoot appConfiguration,
    string connectionString,
    ILogger logger)
{
    // Extrair linhas 116-183 do PostInitialize
}
```

### MiddlewareWebCoreModule Refatorado

```csharp
public override void PreInitialize()
{
    AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);

    Configuration.Modules.AbpAspNetCore()
        .CreateControllersForAppServices(typeof(MiddlewareApplicationModule).GetAssembly());

    Configuration.IocManager.RegisterIfNot<IPerRequestSessionCache, PerRequestSessionCache>();
    Configuration.ReplaceService<IAppConfigurationAccessor, AppConfigurationAccessor>();
    Configuration.SetConfiguration(_appConfiguration.GetChildren());
    Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:ServerRootAddress"];

    if (Configuration.BackgroundJobs.IsJobExecutionEnabled
        && _appConfiguration["Hangfire:IsEnabled"] != null
        && bool.Parse(_appConfiguration["Hangfire:IsEnabled"]))
    {
        Configuration.BackgroundJobs.UseHangfire();
    }

    // Delegated to CacheConfigurer
    CacheConfigurer.Configure(Configuration, _appConfiguration, IocManager);

    // Audit/EntityHistory config (small, keep inline)
    Configuration.Auditing.IsEnabledForAnonymousUsers = false;
    Configuration.Auditing.IsEnabled = true;
    Configuration.EntityHistory.IsEnabled = true;
    Configuration.EntityHistory.IsEnabledForAnonymousUsers = true;
    Configuration.EntityHistory.AddAllAuditedEntities();

    // ... remaining small configs inline
}

public override void PostInitialize()
{
    SetAppFolders();

    // Delegated to ExternalAuthConfigurer
    ExternalAuthConfigurer.Configure(IocManager, _appConfiguration);

    if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
    {
        HangFireConfigurer.ConfigureJobStorage(Configuration, _appConfiguration,
            Configuration.DefaultNameOrConnectionString, Logger);
    }
}
```

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.Middleware.Web.Core.Tests/Configuration/CacheConfigurerTests.cs

public class CacheConfigurerTests
{
    [Fact]
    public void Dado_RedisHabilitado_Quando_Configurar_Entao_DeveRegistrarRedisCache()

    [Fact]
    public void Dado_RedisDesabilitado_Quando_Configurar_Entao_NaoDeveRegistrarRedisCache()

    [Fact]
    public void Dado_SqlServerHabilitado_Quando_Configurar_Entao_DeveConfigurarSqlServerCache()

    [Fact]
    public void Dado_NenhumCacheHabilitado_Quando_Configurar_Entao_DeveUsarCacheDefault()
}

public class ExternalAuthConfigurerTests
{
    [Fact]
    public void Dado_GoogleConfigurado_Quando_Configurar_Entao_DeveRegistrarGoogleProvider()

    [Fact]
    public void Dado_NenhumProviderConfigurado_Quando_Configurar_Entao_NaoDeveRegistrarNenhum()
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.Middleware.Web.Core/Eaf.Middleware.Web.Core.csproj --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage"
```

## Critérios de Aceite

1. `MiddlewareWebCoreModule` tem ≤150 linhas (reduzido de 323)
2. `CacheConfigurer` e `ExternalAuthConfigurer` criados e testados
3. Hangfire config delegado ao `HangFireConfigurer` existente
4. Todos os testes passam
5. Startup funciona exatamente como antes (zero mudança de comportamento)
6. Cobertura não diminuiu (novos testes adicionados)

## Notas para o Sub-Agent

- **RISCO ALTO**: Este módulo é o coração da startup — QUALQUER erro quebra toda a aplicação
- Testar a cada extração (extrair um configurer, build, testar, commit)
- Manter configurers como `internal static` — são detalhes de implementação
- Não alterar a ordem de execução PreInitialize → Initialize → PostInitialize
- Se o startup falhar após uma extração, REVERTER IMEDIATAMENTE
- **Se falhar 3x, reportar complexidade e voltar ao início**
- Garantir que `[DependsOn]` não precisa mudar — os configurers são helpers, não módulos ABP
