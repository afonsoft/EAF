# Plano de Otimização e Hardening — EAF Módulos, Template API, Frontend e DevOps

> **Goal:** Reduzir latência, alocações e pressão de memória no EAF; adicionar hardening de segurança (headers, CSP, rate limit, CORS), melhorar observabilidade/LGPD e DevOps; e propor JWT HttpOnly/refresh token como item separado, dado o impacto no `Eaf.Middleware.Web.Core`.

**Architecture:** Cinco pilares: (1) EF Core & DbContext — read-only otimizado, batch delete e remoção de `Migrate()` do construtor; (2) Caching & Serialização — UTF-8 `System.Text.Json`, cache de DTOs, eliminação de sync-over-async; (3) Async/CPU/Strings — `async` puro, `Regex` estático/gerado e compressão otimizada; (4) Segurança & Frontend — headers, CSP, rate limit, CORS, HttpOnly, LGPD; (5) Observabilidade & DevOps — OpenTelemetry sem PII, CI/CD eficiente e containers seguros.

**Tech Stack:** .NET 10, ASP.NET Boilerplate 10.4.0, EF Core 10, Castle Windsor, Hangfire, Redis/SQL Server cache, System.Text.Json, Angular 18, Docker, GitHub Actions.

---

## Visão Geral das Oportunidades Identificadas

A análise percorreu os 14 módulos EAF (`src/Eaf.*`), os 4 Templates (API, Angular, Worker, Gateway), as docs do ABP em `docs/aspnetboilerplate/`, o repositório `aspnetboilerplate/aspnetboilerplate` e os workflows de CI/CD. Os gargalos e gaps críticos encontrados:

1. **EF Core & DbContext**
   - `ProjectNameDbContext` chama `Database.Migrate()` no construtor, causando atraso e concorrência na criação do primeiro contexto.
   - Queries read-only sem `AsNoTracking` (`AuditLogAppService`, `ChatAppService`, `UserFriendsCache`).
   - `Contains(filter, StringComparison.OrdinalIgnoreCase)` em `IQueryable` (`UserAppService`, `CommonLookupAppService`) — não traduzível pelo EF Core.
   - Cache de entidades `User` em `_usersCache` no `UserAppService` — viola recomendação do ABP (`Caching.md:149`).
   - N+1 em `UserFriendsCache.GetUserFriendsCacheItemInternal` e `ChatAppService`.

2. **Caching / Serialização**
   - `EafSqlServerCache` usa sync-over-async (`GetAwaiter().GetResult`) e serialização XML/ASCII.
   - `DistributedCacheEntryOptions` recriado a cada `Set`.
   - `TempFileCacheManager` armazena arquivos inteiros em cache com 5 min de expiração.

3. **Async / Threading / CPU**
   - `AsyncHelper.RunSync` / `.Result` / `.GetAwaiter().GetResult()` em `MiddlewareAppServiceBase`, `LanguageAppService`, `MiddlewareJwtSecurityTokenHandler`, `LdapAuthenticationSource`, `ServiceBusQueueAppender`, `OCIKeyVaultManager`.
   - `ValidationHelper.IsEmail` cria `new Regex(..., Compiled)` a cada chamada.
   - `WebLogAppService.GetLatestWebLogs` lê 10.000 linhas para filtrar 100.

4. **Background Jobs / Hangfire / Startup**
   - Hangfire `WorkerCount` fixo em `Environment.ProcessorCount * 4, max 16`.
   - `SqlServerStorageOptions.TransactionTimeout = 30 min`.
   - `RemoveOutdatedFailedJobs` carrega 1.000 failed jobs a cada startup.
   - `ExpiredAuditLogDeleterWorker` usa `Contains` com até 30.000 IDs.

5. **Segurança — Backend**
   - `ContentSecurityPolicyMiddleware` no Template API usa `default-src * 'unsafe-inline' 'unsafe-eval'`, o que praticamente desabilita a proteção CSP.
   - Faltam headers `X-Frame-Options`, `X-Content-Type-Options`, `Strict-Transport-Security`, `Referrer-Policy`, `Permissions-Policy`.
   - Não existe rate limiting global; CORS permite `AllowAnyHeader()`, `AllowAnyMethod()`, `AllowCredentials()` sem restrição de origem real (especialmente no Gateway).
   - `EnableDetailedErrors = true` no SignalR e no EF Core pode vazar informações sensíveis.

6. **Segurança — Autenticação/Frontend**
   - Token JWT é armazenado em cookie (`Eaf.AuthToken`) e em `localStorage` via `StorageService` — não é HttpOnly/SameSite=None correto em produção.
   - Não existe refresh token; a cada login um novo `accessToken` é emitido com `Expiration` fixo.
   - `AuthConfigurer` usa `IncludeErrorDetails = true` e `SaveToken = true` por padrão.

7. **Observabilidade / LGPD**
   - OpenTelemetry coleta `RecordException`, `SetDbStatementForText`, `IncludeFormattedMessage` — potencial para exfiltração de PII/senhas em traces/logs.
   - Não há sanitização de e-mail/CPF/senha nos logs.
   - `ServiceBusQueueAppender` e logs de `TokenAuthController` podem incluir tokens.

8. **DevOps / Containers / CI**
   - Dockerfiles `RUN apt-get update` sem pinagem e `USER app` sem verificar permissões de volumes.
   - Docker Compose não define `restart` limits, `mem_limit`, `cpus`, `read_only`.
   - `ci-build-test.yml` roda `dotnet restore ... --no-cache --force` sem cache de pacotes cross-job.
   - `publish-all.yml` não assina pacotes NuGet nem valida provenance.

---

## Parte 1 — Performance e Otimização de Memória

### Task 1.1 — Remover `Database.Migrate()` do construtor de `ProjectNameDbContext`

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContext.cs:26-56`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs:188-260` ou `Templates/Api/src/Eaf.ProjectName.Migrator`

**Symbols:**
```csharp
public class ProjectNameDbContext : AbpZeroDbContext<Tenant, Role, User, ProjectNameDbContext>
{
    public ProjectNameDbContext(DbContextOptions<ProjectNameDbContext> options) : base(options)
    {
        // Remover MigrateDatabase(Database);
    }
    private static void MigrateDatabase(DatabaseFacade database) { ... }
}
```

**Change:**
- Remover `MigrateDatabase(Database)` do construtor.
- Garantir `Migrate()` chamado uma única vez no startup/Migrator.
- `SkipMigrate` default `true` para produção.

**Why:** Construtor do `DbContext` é chamado a cada request; `Migrate()` bloqueante e não thread-safe.

---

### Task 1.2 — Habilitar `DbContext` pooling no Template API

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameEntityFrameworkCoreModule.cs:25-45`

**Symbols:**
```csharp
public class ProjectNameEntityFrameworkCoreModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Modules.AbpEfCore().AddDbContext<ProjectNameDbContext>(options =>
        {
            options.DbContextOptions.EnableDetailedErrors(true);
            options.DbContextOptions.EnableSensitiveDataLogging(false);
        });
    }
}
```

**Change:**
- Tornar `EnableDetailedErrors` condicional a `IsDevelopment()`.
- Documentar configuração de pooling se suportado pela infraestrutura ABP.

**Why:** Reduz alocação de `DbContext`. `EnableDetailedErrors` em produção vaza info e gera overhead.

---

### Task 1.3 — Otimizar queries read-only do Template API (`AirplanesAppService` / `AirplaneManager`)

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.Application/Airplanes/AirplanesAppService.cs:37-48`, `90-93`
- Modify: `Templates/Api/src/Eaf.ProjectName.Core/Airplanes/AirplaneManager.cs:18-45`

**Symbols:**
```csharp
public async Task<PagedResultDto<AirplaneDto>> GetAll(GetAirplanesInput input)
public async Task<FileDto> GetAirplanesToExcel()
public virtual IQueryable<Airplane> Airplanes { get; }
public async Task<Airplane> CreateAsync(Airplane airplane)
```

**Change:**
- Garantir `.AsNoTracking()` em todas as queries de leitura.
- `GetAirplanesToExcel` considerar streaming/chunked export para grandes volumes.
- `AirplaneManager.CreateAsync` substituir `ToLower()` por coluna normalizada ou verificação em memória.

---

### Task 1.4 — Corrigir `StringComparison` em queries EF Core

**Files:**
- Modify: `src/Eaf.Middleware.Application/Authorization/Users/UserAppService.cs:342-370`
- Modify: `src/Eaf.Middleware.Application/Common/CommonLookupAppService.cs:29-65`

**Symbols:**
```csharp
public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
public async Task<PagedResultDto<NameValueDto>> FindUsers(FindUsersInput input)
```

**Change:**
- Substituir `Contains(filter, StringComparison.OrdinalIgnoreCase)` por `EF.Functions.Like` ou `.ToLower().Contains(filter.ToLower())`.
- Revisar ordem `.AsNoTracking().AsQueryable()`.

---

### Task 1.5 — Substituir cache de entidades `User` por cache de DTOs

**Files:**
- Modify: `src/Eaf.Middleware.Application/Authorization/Users/UserAppService.cs:54, 90-91, 376-383, 401, 409, 134, 173`

**Symbols:**
```csharp
private readonly ITypedCache<string, List<User>> _usersCache;
public async Task<FileDto> GetUsersToExcel()
```

**Change:**
- Alterar para `ITypedCache<string, IReadOnlyList<UserListDto>>`.
- Factory async com projeção `UserListDto` e roles em batch.
- Invalidar cache em mutações.

**Why:** ABP: "Do not directly store entities in the cache". `User` possui dados sensíveis e navegações.

---

### Task 1.6 — Otimizar `ChatAppService`

**Files:**
- Modify: `src/Eaf.Middleware.Application/Chat/ChatAppService.cs:54-109`, `130-177`, `179-192`

**Symbols:**
```csharp
public async Task<GetUserChatFriendsWithSettingsOutput> GetUserChatFriendsWithSettingsAsync()
private async Task<ListResultDto<ChatMessageDto>> GetUserChatMessagesAsync(...)
private async Task<ListResultDto<ChatMessageDto>> GetGroupChatMessagesAsync(...)
private async Task SetTargetUserNamesAsync(List<ChatMessageDto> messages)
```

**Change:**
- Consolidar contagens em query agregada.
- `SetTargetUserNamesAsync` com `ToDictionaryAsync` e `Where(id in ids)`.
- Adicionar `.AsNoTracking()` e considerar cache de unread count.

---

### Task 1.7 — Otimizar `UserFriendsCache`

**Files:**
- Modify: `src/Eaf.Middleware.Core/Friendships/Cache/UserFriendsCache.cs:188-247`, `52-186`

**Symbols:**
```csharp
protected virtual UserWithFriendsCacheItem GetUserFriendsCacheItemInternal(UserIdentifier userIdentifier)
```

**Change:**
- Substituir N+1 `FindById` por query única com `ToDictionaryAsync`.
- Mover unread count para subquery/projetação.
- Avaliar `MayHaveTenantEntityCache<Friendship, FriendCacheItem>`.

---

### Task 1.8 — Otimizar `AuditLogAppService`

**Files:**
- Modify: `src/Eaf.Middleware.Application/Auditing/AuditLogAppService.cs:78-108`, `151-182`, `188-222`, `229-253`, `122-140`, `267-284`

**Symbols:**
```csharp
public async Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input)
public async Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input)
public async Task<PagedResultDto<EntityChangeListDto>> GetEntityChanges(GetEntityChangeInput input)
public async Task<PagedResultDto<EntityChangeListDto>> GetEntityTypeChanges(GetEntityTypeChangeInput input)
private IQueryable<AuditLogAndUser> CreateAuditLogAndUsersQuery(...)
private IQueryable<EntityChangeAndUser> CreateEntityChangesAndUsersQuery(...)
```

**Change:**
- `.AsNoTracking()` em todas as queries read-only.
- Manter joins no banco (evitar materialização precoce de `GetAllAsync()`).
- Excel com chunked export / `IAsyncEnumerable`.

---

### Task 1.9 — Refatorar `EafSqlServerCache` para async e JSON UTF-8

**Files:**
- Modify: `src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCache.cs:141-186`, `207-258`

**Symbols:**
```csharp
public override bool TryGetValue(string key, out object value)
public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
public override void Remove(string key)
private static byte[] ObjectToByteArray(object objData)
private static object ByteArrayToObject(byte[] byteArray)
```

**Change:**
- Tornar async (ou isolar `Task.Run` em background threads, nunca ASP.NET thread pool).
- Substituir XML/ASCII por `System.Text.Json` UTF-8 (`JsonCacheSerializer`).
- Reutilizar `DistributedCacheEntryOptions` e `JsonSerializerOptions` estáticos.

---

### Task 1.10 — Otimizar lifecycle de `EafSqlServerCache` / `EafSqliteCache`

**Files:**
- Modify: `src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCacheManager.cs`
- Modify: `src/Eaf.SqliteCache/Runtime/Caching/Sqlite/EafSqliteCacheManager.cs`

**Symbols:**
```csharp
public class EafSqlServerCacheManager : CacheManagerBase<ICache>, ICacheManager
public class EafSqliteCacheManager : CacheManagerBase<ICache>, ICacheManager
```

**Change:**
- Injetar `ICacheSerializer` nos caches e reutilizar.
- Avaliar lifecycle `Singleton`/`Scoped`.

---

### Task 1.11 — Otimizar `ExpiredAuditLogDeleterWorker`

**Files:**
- Modify: `src/Eaf.Middleware.Core/Auditing/ExpiredAuditLogDeleterWorker.cs:100-147`

**Symbols:**
```csharp
protected override void DoWork()
private void DeleteAuditLogs(DateTime expireDate)
```

**Change:**
- Usar `ExecuteDeleteAsync` (EF Core 7+) se suportado, ou batch de 5.000-10.000 IDs em loop.
- `MaxDeletionCount` configurável.
- Adicionar `CancellationToken`.

---

### Task 1.12 — Eliminar sync-over-async em hot paths

**Files:**
- Modify: `src/Eaf.Middleware.Application/MiddlewareAppServiceBase.cs:111-115`
- Modify: `src/Eaf.Middleware.Application/Localization/LanguageAppService.cs:78-86`
- Modify: `src/Eaf.Middleware.Web.Core/Authentication/JwtBearer/MiddlewareJwtSecurityTokenHandler.cs`
- Modify: `src/Eaf.Middleware.Ldap/Ldap/Authentication/LdapAuthenticationSource.cs`
- Modify: `src/Eaf.Log4NetServiceBus/Logging/ServiceBusQueueAppender.cs`
- Modify: `src/Eaf.KeyVault/KeyVault/OCI/OCIKeyVaultManager.cs`

**Symbols:**
```csharp
protected virtual User GetCurrentUser()
public async Task<List<LanguageInfo>> GetAllLanguages()
// LdapAuthenticationSource.FillUsersLdap(...).Result
// ServiceBusQueueAppender queueClient.SendAsync(...).GetAwaiter().GetResult()
// OCIKeyVaultManager.GetKeyValues()
```

**Change:**
- Tornar `GetCurrentUser` obsoleto; introduzir `GetCurrentUserAsync`.
- Refatorar métodos para async puro.

---

### Task 1.13 — Otimizar `ValidationHelper.IsEmail`

**Files:**
- Modify: `src/Eaf.Middleware.Core/Net/Helper/ValidationHelper.cs`

**Symbols:**
```csharp
public static class ValidationHelper
{
    public static bool IsEmail(string value)
}
```

**Change:**
- `Regex` estático compilado ou `[GeneratedRegex]` (C# 11+).
- Avaliar `EmailAddressAttribute` para validações triviais.

---

### Task 1.14 — Configurabilidade e limites do Hangfire

**Files:**
- Modify: `src/Eaf.Middleware.Core/Hangfire/EafHangFireOptions.cs`
- Modify: `src/Eaf.Middleware.Web.Core/MiddlewareWebCoreModule.cs:132-162`, `196-207`

**Symbols:**
```csharp
public class EafHangFireOptions : IOptions<EafHangFireOptions>
public int WorkerCount { get; set; }
private void ConfigureHangfireStorage()
private void RemoveOutdatedFailedJobs()
```

**Change:**
- Adicionar `MaxWorkerCount`, `TransactionTimeout`, `MaxFailedJobsToScan`, `FailedJobRetentionDays`.
- `TransactionTimeout` default 5 min.
- Limitar scan de failed jobs a 100 e filtrar por `FailedAt` no storage.

---

### Task 1.15 — Otimizar `WebLogAppService`

**Files:**
- Modify: `src/Eaf.Middleware.Application/Logging/WebLogAppService.cs:72-116`

**Symbols:**
```csharp
public GetLatestWebLogsOutput GetLatestWebLogs()
```

**Change:**
- Limitar leitura a 1.000 linhas, iterar com `IEnumerable<string>` e `TakeWhile`.
- Evitar `.Reverse().Take(10000).ToList()` seguido de `.Take(100).Reverse()`.

---

### Task 1.16 — Ajustar `MemoryCache` e `ResponseCompression`

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/WebHostModule.cs:62-65`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs:149-174`

**Symbols:**
```csharp
Configuration.Caching.MemoryCacheOptions = new MemoryCacheOptions { SizeLimit = 256 };
services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Fastest; });
```

**Change:**
- Adicionar `CompactionPercentage = 0.25` e `SizeLimit` via configuração.
- `CompressionLevel.Optimal` para Brotli/Gzip em produção; `Fastest` apenas em Development.
- Adicionar `SetSize` para entries grandes.

---

### Task 1.17 — Otimizar `TokenAuthController`

**Files:**
- Modify: `src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs:147-200`, `448-455`, `921-962`, `966-1015`

**Symbols:**
```csharp
public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
private async Task<User> RegisterExternalUserAsync(...)
private async Task<User> UpdateExistingExternalUserAsync(...)
```

**Change:**
- Reduzir chamadas repetidas a `_userManager.InitializeOptionsAsync` e `GetLoginResultAsync`.
- `GetExternalAuthenticationProviders` retornar `IEnumerable` projetado sem `.ToList()` intermediário.
- Carregar roles default em uma única query.

---

### Task 1.18 — Ajustes finos no Template API

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.Core/Airplanes/AirplaneManager.cs`
- Modify: `Templates/Api/src/Eaf.ProjectName.Core/Airplanes/jobs/AirplaneJob.cs`
- Modify: `Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContextConfigurer.cs`
- Modify: `src/Eaf.Middleware.Core/Net/Storage/TempFileCacheManager.cs`

**Symbols:**
```csharp
public async Task<Airplane> CreateAsync(Airplane airplane)
public override Task ExecuteAsync(string args, PerformContext context, CancellationToken token)
public static void Configure(DbContextOptionsBuilder<ProjectNameDbContext> builder, string connectionString, string databaseProvider = "SqlServer")
public void SetFile(string token, byte[] content)
```

**Change:**
- `AirplaneManager.CreateAsync`: unicidade via coluna normalizada sem `ToLower()`.
- `AirplaneJob`: `Task.CompletedTask` e exemplo mínimo documentado.
- `ProjectNameDbContextConfigurer`: `EnableRetryOnFailure` por provider.
- `TempFileCacheManager`: limite de tamanho e `MemoryCache` entry size.

---

## Parte 2 — Segurança (Headers, CSP, Rate Limit, CORS)

### Task 2.1 — Hardening do `ContentSecurityPolicyMiddleware`

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Middleware/ContentSecurityPolicyMiddleware.cs`
- Modify: `Templates/Api/test/Eaf.ProjectName.Tests/Middleware/ContentSecurityPolicyMiddleware_Tests.cs`

**Symbols:**
```csharp
public class ContentSecurityPolicyMiddleware
{
    private const string ContentSecurityPolicy = "default-src * ...";
    public async Task Invoke(HttpContext httpContext)
}
```

**Change:**
- Substituir CSP permissivo por política estrita, configurável via `appsettings`:
  ```
  default-src 'self'; script-src 'self' 'unsafe-inline' (apenas se necessário); style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; connect-src 'self'; font-src 'self'; frame-ancestors 'none'; base-uri 'self'; form-action 'self';
  ```
- Adicionar `Content-Security-Policy-Report-Only` opcional.
- Adicionar `nonce`/`hash` para scripts inline.
- Remover `X-Content-Security-Policy` legado (não suportado).

**Why:** `default-src * 'unsafe-inline' 'unsafe-eval'` anula a proteção CSP.

---

### Task 2.2 — Criar `SecurityHeadersMiddleware` completo

**Files:**
- Create: `src/Eaf.Middleware.Web.Core/Security/Headers/SecurityHeadersMiddleware.cs`
- Modify: `src/Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs:193-200`

**Symbols:**
```csharp
public class SecurityHeadersMiddleware
{
    public async Task Invoke(HttpContext context)
}
```

**Change:**
- Adicionar headers em todas as respostas:
  - `X-Content-Type-Options: nosniff`
  - `X-Frame-Options: DENY` (ou `SAMEORIGIN` configurável)
  - `Referrer-Policy: strict-origin-when-cross-origin`
  - `Permissions-Policy: geolocation=(), microphone=(), camera=()`
  - `Strict-Transport-Security: max-age=31536000; includeSubDomains` (somente HTTPS/prod)
  - `X-XSS-Protection: 0` (deprecado, mas recomendado desabilitar)
- Permitir override por config (`App:SecurityHeaders`).

---

### Task 2.3 — Implementar Rate Limiting global

**Files:**
- Create: `src/Eaf.Middleware.Web.Core/Security/RateLimiting/EafRateLimitOptions.cs`
- Create: `src/Eaf.Middleware.Web.Core/Security/RateLimiting/RateLimitingExtensions.cs`
- Modify: `src/Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs:193-200`

**Symbols:**
```csharp
public class EafRateLimitOptions : IOptions<EafRateLimitOptions>
{
    public int RequestsPerMinute { get; set; } = 60;
    public int LoginAttemptsPerMinute { get; set; } = 5;
}
public static class RateLimitingExtensions
{
    public static IServiceCollection AddEafRateLimiting(this IServiceCollection services, IConfiguration configuration)
    public static IApplicationBuilder UseEafRateLimiting(this IApplicationBuilder app)
}
```

**Change:**
- Usar `Microsoft.AspNetCore.RateLimiting` (.NET 7+) ou `AspNetCoreRateLimit`.
- Políticas diferenciadas por endpoint (`/api/TokenAuth/Authenticate` limitado, `/api/services` geral).
- Armazenar contadores em Redis quando `RedisCache` habilitado (distributed).

---

### Task 2.4 — Revisar e endurecer CORS

**Files:**
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs:89-113`
- Modify: `Templates/Eaf.Gateways.API/src/Program.cs:78-88`

**Symbols:**
```csharp
services.AddCors(options =>
{
    options.AddPolicy(ProjectNameConsts.DefaultCorsPolicyName, builder => { ... });
});
```

**Change:**
- No Template API: rejeitar `*` e `SetIsOriginAllowed((host) => true)` em produção.
- Validar origens contra lista explícita.
- Limitar headers expostos e métodos (não `AllowAnyHeader()`/`AllowAnyMethod()` sem necessidade).
- Gateway: corrigir `EafGateWayCorsPolicy` que permite qualquer origem.

---

### Task 2.5 — Revisar `AuthConfigurer` e validação JWT

**Files:**
- Modify: `src/Eaf.Middleware.Web.Core/Configuration/AuthConfigurer.cs`

**Symbols:**
```csharp
public static class AuthConfigurer
{
    private static TokenAuthConfiguration ConfigureTokenAuth(IConfiguration configuration)
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    public static Task QueryStringTokenResolver(MessageReceivedContext context)
}
```

**Change:**
- `IncludeErrorDetails = false` em produção.
- `SaveToken = false` se não for necessário inspecionar token no handler.
- Garantir `ValidateIssuerSigningKey`, `ValidateLifetime`, `ValidateAudience`, `ValidateIssuer`.
- `ClockSkew` via configuração.

---

### Task 2.6 — Desativar `EnableDetailedErrors` em produção

**Files:**
- Modify: `src/Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs:33-39`
- Modify: `Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameEntityFrameworkCoreModule.cs:27-44`

**Symbols:**
```csharp
services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});
options.DbContextOptions.EnableDetailedErrors(true);
```

**Change:**
- Tornar condicional a `IsDevelopment()`.
- Default `EnableDetailedErrors = false`.

---

## Parte 3 — JWT HttpOnly / Refresh Token (Item Separado)

### Task 3.1 — Proposta: implementar refresh token com cookies HttpOnly

> **Nota:** Esta task exige alteração no contrato de autenticação do `Eaf.Middleware.Web.Core` e no frontend Angular. Deve ser tratada como feature separada.

**Files:**
- Modify: `src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs:147-240`
- Modify: `src/Eaf.Middleware.Web.Core/Models/TokenAuth/AuthenticateResultModel.cs`
- Modify: `src/Eaf.Middleware.Web.Core/Configuration/AuthConfigurer.cs`
- Modify: `src/Eaf.Middleware.Web.Core/Authentication/JwtBearer/MiddlewareJwtSecurityTokenHandler.cs`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/auth/token.service.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/utils/storage.service.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/account/login/login.service.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/eafHttpInterceptor.ts`

**Symbols (backend):**
```csharp
public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
private string CreateAccessToken(IEnumerable<Claim> claims)
private async Task<IEnumerable<Claim>> CreateJwtClaims(ClaimsIdentity identity, User user, string externalAuthProviderformation = "")
```

**Symbols (frontend):**
```typescript
export class TokenService {
  getToken(): string;
  setToken(authToken: string, expireDate?: Date): void;
  clearToken(): void;
}
export class StorageService {
  setCookieValue(...)
  getCookieValue(...)
}
```

**Change proposta:**
1. Backend:
   - `Authenticate` retorna apenas `accessToken` curto (5-15 min) e define cookie `refreshToken` HttpOnly, Secure, SameSite=Lax/Strict.
   - Criar entidade `UserRefreshToken` (UserId, TokenHash, Expiry, CreatedAt, IsRevoked).
   - Criar endpoint `POST api/TokenAuth/Refresh` que valida cookie `refreshToken` e emite novo `accessToken`.
   - `MiddlewareJwtSecurityTokenHandler` aceitar token do header `Authorization` ou do cookie `accessToken` (configurável).
   - Revogação em logout/mudança de senha.

2. Frontend:
   - `TokenService` passa a ler `accessToken` de cookie não-HttpOnly (se necessário) ou do header de resposta.
   - `StorageService` remove `localStorage` para token de acesso; armazena apenas refresh em HttpOnly cookie (controlado pelo backend).
   - `EafHttpInterceptor` adiciona handler `401 → /TokenAuth/Refresh → retry` sem expor refresh token no JS.

3. Configuração:
   - `Authentication:JwtBearer:AccessTokenExpirationSeconds` (curto).
   - `Authentication:JwtBearer:RefreshTokenExpirationDays` (7-30 dias).
   - `Authentication:JwtBearer:UseHttpOnlyRefreshToken` (`true` por padrão).

**Why:** Token em `localStorage` é vulnerável a XSS. HttpOnly + refresh token é o padrão moderno para SPA.

---

## Parte 4 — Frontend, Observabilidade e LGPD

### Task 4.1 — Remover token de `localStorage` no frontend

**Files:**
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/utils/storage.service.ts`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/assets/lib/eaf-ng2-module/src/auth/token.service.ts`

**Symbols:**
```typescript
export class StorageService {
  public setValue(key: string, value: any): void;
  public getValue(key: string): any;
  public setCookieValue(key: string, value: string, expireDate?: Date, path?: string, domain?: string): void;
}
export class TokenService {
  getToken(): string;
  setToken(authToken: string, expireDate?: Date): void;
  clearToken(): void;
}
```

**Change:**
- `setCookieValue` para token usar `HttpOnly`/`Secure`/`SameSite` conforme config (se o backend gerenciar o cookie); senão, pelo menos não persistir em `localStorage`.
- `Clear()` não limpar `localStorage` inteiro indiscriminadamente (pode afetar outros apps no mesmo domínio).

---

### Task 4.2 — Sanitização de PII em logs e traces

**Files:**
- Create: `src/Eaf.OpenTelemetry/Logging/SensitiveDataEnricher.cs` (ou `EafLogSanitizer`)
- Modify: `src/Eaf.OpenTelemetry/AspNetCore/Configuration/EafOpenTelemetryServiceCollectionExtensions.cs:92-105`, `137-141`
- Modify: `src/Eaf.Castle.Serilog/Serilog/*` (adicionar enricher)

**Symbols:**
```csharp
public static class EafLogSanitizer
{
    public static string Sanitize(string input)
}
public class SensitiveDataEnricher : ILogEventEnricher
```

**Change:**
- Mascarar padrões: e-mail, CPF, senhas, tokens, cartões, `Authorization` header.
- OpenTelemetry:
  - Desabilitar `SetDbStatementForText`/`SetDbStatementForStoredProcedure` em produção.
  - Desabilitar `IncludeFormattedMessage`/`ParseStateValues` se contiverem PII.
- Adicionar `HttpClientInstrumentation` redaction para headers Authorization/Cookie.

---

### Task 4.3 — Configurações de LGPD/GDPR

**Files:**
- Create: `src/Eaf.Middleware.Core/Privacy/LgpdOptions.cs`
- Modify: `src/Eaf.Middleware.Web.Core/Configuration/EafServiceCollectionMiddlewareExtensions.cs`
- Modify: `Templates/Api/src/Eaf.ProjectName.Web.Host/Startup/Startup.cs`

**Symbols:**
```csharp
public class LgpdOptions : IOptions<LgpdOptions>
{
    public bool EnableConsentBanner { get; set; }
    public string[] EssentialCookies { get; set; }
    public string PrivacyPolicyUrl { get; set; }
}
```

**Change:**
- Adicionar `AddDataProtection` com chave persistente (Redis/Azure Blob) quando em cluster.
- Configurar `CookiePolicyOptions` para `SameSite` adequado e consentimento.
- Marcar campos sensíveis do `User` com `[PersonalData]` ou `[ProtectedPersonalData]`.
- Implementar endpoint `DELETE /api/services/app/Profile/DeletePersonalData` (direito ao esquecimento).
- Retenção de logs/audit configurável.

---

### Task 4.4 — Melhorar OpenTelemetry para produção

**Files:**
- Modify: `src/Eaf.OpenTelemetry/AspNetCore/Configuration/EafOpenTelemetryOptions.cs`
- Modify: `src/Eaf.OpenTelemetry/AspNetCore/Configuration/EafOpenTelemetryServiceCollectionExtensions.cs`

**Symbols:**
```csharp
public class EafOpenTelemetryOptions : IOptions<EafOpenTelemetryOptions>
{
    public bool RecordException { get; set; } = true;
    public bool SetDbStatementForText { get; set; } = true;
    public bool SetDbStatementForStoredProcedure { get; set; } = true;
    public bool ConsoleExporter { get; set; }
}
```

**Change:**
- Adicionar `EnableSensitiveDataRedaction` default `true`.
- Configurar `TraceIdRatioBased` sampler para alta carga.
- Adicionar `AddProcessInstrumentation` e `HttpClient`/`AspNetCore` redaction.
- Garantir que `OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT` seja respeitado (já presente, mas validar).

---

### Task 4.5 — Hardering de segurança no Angular

**Files:**
- Modify: `Templates/Angular/Eaf.ProjectName.UI/src/index.html`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/angular.json`

**Change:**
- Adicionar `<meta http-equiv="Content-Security-Policy" ...>` no `index.html` para build estático.
- Configurar `angular.json` para gerar hashes de scripts inline (NG_APP_CSP_NONCE).
- Revisar dependências conhecidas por vulnerabilidades (`npm audit`).

---

## Parte 5 — DevOps, Containers e CI/CD

### Task 5.1 — Hardening de Dockerfiles

**Files:**
- Modify: `Templates/Api/Dockerfile`
- Modify: `Templates/Angular/Eaf.ProjectName.UI/dockerfile`
- Modify: `Templates/Worker/Dockerfile`
- Modify: `Templates/Eaf.Gateways.API/Dockerfile`

**Symbols (Dockerfile):**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -yq ...
USER app
ENTRYPOINT ["dotnet", "Eaf.ProjectName.Web.Host.dll"]
```

**Change:**
- Usar imagens `chiseled`/`distroless` quando disponíveis.
- Pin de versões de pacotes `apt-get`.
- Remover `CORECLR_ENABLE_PROFILING` e `CORECLR_PROFILER` se não usado em produção (profiling ativo consome CPU).
- Criar usuário `app` com UID fixo, usar `--chown=app:app` no `COPY`.
- Definir `ASPNETCORE_ENVIRONMENT` via runtime, não no Dockerfile.
- Adicionar `HEALTHCHECK` e `read_only` rootfs.

---

### Task 5.2 — Melhorar Docker Compose

**Files:**
- Modify: `docker-compose.yml`
- Modify: `Templates/Api/docker-compose.yml`
- Modify: `Templates/Api/docker-compose.infra.yml`

**Change:**
- Adicionar `mem_limit`, `cpus`, `restart: unless-stopped`, `read_only: true`.
- Mapear volumes com `noexec`, `nosuid`, `nodev`.
- Adicionar `depends_on` com `condition: service_healthy`.
- Configurar redes internas (não expor bancos na internet).
- Incluir `seq`, `redis`, `mssql` no `docker-compose.infra.yml` com healthchecks.

---

### Task 5.3 — Otimizar CI/CD (`ci-build-test.yml`)

**Files:**
- Modify: `.github/workflows/ci-build-test.yml`

**Change:**
- Usar `dotnet restore` com `--locked-mode` se houver `packages.lock.json`.
- Cache de NuGet compartilhado entre jobs (atualmente cada job cria cache próprio).
- Paralelização de testes por projeto.
- Adicionar `dotnet format --verify-no-changes`.
- Falhar CI se `npm audit --audit-level=high` encontrar vulnerabilidades críticas.
- Rodar `dotnet test` com `--blame-crash` para diagnosticar hangs.

---

### Task 5.4 — Melhorar `publish-all.yml`

**Files:**
- Modify: `.github/workflows/publish-all.yml`

**Change:**
- Assinar pacotes NuGet com `dotnet sign` / trusted signing.
- Gerar SBOM (`dotnet sbom-tool` ou `cyclonedx`).
- Publicar imagens Docker com `provenance` e `sbom`.
- Verificar tags de release antes de publicar.
- Adicionar `permissions: contents: read, packages: write`.

---

### Task 5.5 — Adicionar scan de secrets e IaC

**Files:**
- Modify: `.github/workflows/security-scan.yml`
- Create: `.github/workflows/secrets-scan.yml`

**Change:**
- Adicionar `trufflehog` ou `gitleaks` no CI.
- Adicionar `checkov`/`trivy` para Dockerfiles e docker-compose.
- Adicionar `dependency-review-action` para PRs.

---

## Ordem Recomendada de Implementação

1. **Performance crítica** (Parte 1): Tasks 1.1, 1.2, 1.9, 1.12, 1.13, 1.14 — cold-start, cache, thread pool.
2. **EF Core hot paths** (Parte 1): Tasks 1.4, 1.5, 1.6, 1.7, 1.8, 1.11 — queries e N+1.
3. **Segurança básica** (Parte 2): Tasks 2.1, 2.2, 2.3, 2.4, 2.5, 2.6 — headers, CSP, rate limit, CORS.
4. **Frontend/LGPD** (Parte 4): Tasks 4.1, 4.2, 4.4, 4.5 — logs PII, localStorage, CSP Angular.
5. **JWT HttpOnly/Refresh** (Parte 3): Task 3.1 — feature separada, requer testes integrados frontend+backend.
6. **DevOps/Hardening** (Parte 5): Tasks 5.1, 5.2, 5.3, 5.4, 5.5 — containers, CI/CD.
7. **Ajustes finos** (Parte 1/2): Tasks 1.3, 1.10, 1.15, 1.16, 1.17, 1.18, 4.3.

---

## Verification

- Build: `dotnet build Eaf.sln --configuration Release`
- Tests: `dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings`
- Coverage: não diminuir baseline
- Static analysis: SonarCloud + Qodana sem novos issues críticos
- Smoke test: subir Template API localmente, verificar login, listagem de Airplanes, cache distribuído, headers de segurança e rate limit
- Container scan: `trivy image` e `docker buildx build --provenance=true --sbom=true`
