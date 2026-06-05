# 82 — Extrair TokenAuthController em Services (SRP)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 5 — SOLID / Clean Architecture |
| **Princípio** | SRP — Single Responsibility Principle |
| **Complexidade** | MUITO ALTA |
| **Risco** | MUITO ALTO — Controller principal de autenticação |
| **Dependências** | Executar APÓS tarefa 80 (Service Locator) e 81 (WebCore extract) |
| **Arquivos Modificados** | 1 refatorado + 3 novos services |

## Objetivo

Extrair responsabilidades do `TokenAuthController` (1215 linhas, 22 dependências no construtor) em services dedicados:
1. `IAuthenticationService` — login, token generation, refresh
2. `IExternalAuthenticationService` — login Google/Microsoft/AuthZero
3. `IImpersonationService` — impersonation de usuários/tenants

## Motivo

- **1215 linhas** — classe God Object
- **22 parâmetros** no construtor — sintoma claro de SRP violado
- **Impossível testar** um fluxo de autenticação sem configurar todas as 22 dependências
- **3 responsabilidades distintas** misturadas em um controller

## Análise das Responsabilidades Atuais

| # | Responsabilidade | Linhas (aprox.) | Dependências |
|---|-----------------|----------------|-------------|
| 1 | Login + Token | ~400 linhas | LogInManager, TokenAuthConfiguration, UserManager, IdentityOptions, JwtBearerOptions |
| 2 | External Auth | ~300 linhas | IExternalAuthManager, IExternalAuthConfiguration |
| 3 | Impersonation | ~200 linhas | IImpersonationManager, ITenantCache |
| 4 | Account Mgmt | ~200 linhas | IPasswordHasher, IEmailSender, IBinaryObjectManager |
| 5 | Infra (logging, notif) | ~115 linhas | ILogger, INotificationPublisher, IWebhookPublisher |

## Arquivos Afetados

### 1. IAuthenticationService + AuthenticationService (NOVO)

```csharp
// ARQUIVO: src/Eaf.Middleware.Web.Core/Authentication/IAuthenticationService.cs
namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Serviço de autenticação local (login, token, refresh).
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Autentica um usuário local e gera tokens JWT.
        /// </summary>
        Task<AuthenticateResultModel> AuthenticateAsync(AuthenticateModel model);

        /// <summary>
        /// Renova o access token usando o refresh token.
        /// </summary>
        Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Gera um par de tokens (access + refresh) para o usuário.
        /// </summary>
        Task<string> CreateAccessTokenAsync(IEnumerable<Claim> claims, TimeSpan expiration);
    }
}

// ARQUIVO: src/Eaf.Middleware.Web.Core/Authentication/AuthenticationService.cs
namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Implementação do serviço de autenticação local.
    /// </summary>
    public class AuthenticationService : MiddlewareDomainServiceBase, IAuthenticationService, ITransientDependency
    {
        private readonly LogInManager _logInManager;
        private readonly TokenAuthConfiguration _configuration;
        private readonly UserManager _userManager;
        // ... apenas as dependências necessárias para autenticação local

        public AuthenticationService(
            LogInManager logInManager,
            TokenAuthConfiguration configuration,
            UserManager userManager,
            IOptions<IdentityOptions> identityOptions,
            IOptions<JwtBearerOptions> jwtOptions)
        {
            // ...
        }
    }
}
```

### 2. IExternalAuthenticationService + ExternalAuthenticationService (NOVO)

```csharp
// ARQUIVO: src/Eaf.Middleware.Web.Core/Authentication/IExternalAuthenticationService.cs
namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Serviço de autenticação externa (Google, Microsoft, AuthZero).
    /// </summary>
    public interface IExternalAuthenticationService
    {
        /// <summary>
        /// Autentica usando provedor externo.
        /// </summary>
        Task<ExternalAuthenticateResultModel> ExternalAuthenticateAsync(ExternalAuthenticateModel model);

        /// <summary>
        /// Obtém provedores externos configurados.
        /// </summary>
        Task<List<ExternalLoginProviderInfoModel>> GetExternalAuthenticationProvidersAsync();
    }
}
```

### 3. IImpersonationService + ImpersonationService (NOVO)

```csharp
// ARQUIVO: src/Eaf.Middleware.Web.Core/Authentication/IImpersonationService.cs
namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Serviço de impersonação de usuários e tenants.
    /// </summary>
    public interface IImpersonationService
    {
        /// <summary>
        /// Inicia impersonação de um usuário específico.
        /// </summary>
        Task<ImpersonatedAuthenticateResultModel> ImpersonateUserAsync(ImpersonateInput input);

        /// <summary>
        /// Inicia impersonação de um tenant específico.
        /// </summary>
        Task<ImpersonatedAuthenticateResultModel> ImpersonateTenantAsync(ImpersonateTenantInput input);

        /// <summary>
        /// Volta à identidade original após impersonação.
        /// </summary>
        Task<ImpersonatedAuthenticateResultModel> BackToImpersonatorAsync();
    }
}
```

### 4. TokenAuthController Refatorado

```csharp
// ARQUIVO: src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs
// ANTES: 1215 linhas, 22 dependências
// DEPOIS: ~200 linhas, 5 dependências (delegates para services)

public TokenAuthController(
    IAuthenticationService authService,
    IExternalAuthenticationService externalAuthService,
    IImpersonationService impersonationService,
    ILogger logger,
    ICacheManager cacheManager)
{
    _authService = authService;
    _externalAuthService = externalAuthService;
    _impersonationService = impersonationService;
    _logger = logger;
    _cacheManager = cacheManager;
}

// Cada endpoint delega para o service correspondente
[HttpPost]
public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
{
    return await _authService.AuthenticateAsync(model);
}
```

## Processo de Execução (INCREMENTAL)

**IMPORTANTE**: Esta tarefa é MUITO ALTA complexidade. Executar em 3 etapas com build/test entre cada uma.

### Etapa 1: Extrair IAuthenticationService
1. Criar interface + implementação
2. Mover métodos de login/token do controller
3. Atualizar controller para delegar
4. Build + Test

### Etapa 2: Extrair IExternalAuthenticationService
1. Criar interface + implementação
2. Mover métodos de external auth
3. Atualizar controller
4. Build + Test

### Etapa 3: Extrair IImpersonationService
1. Criar interface + implementação
2. Mover métodos de impersonation
3. Atualizar controller
4. Build + Test

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.Middleware.Web.Core.Tests/Authentication/AuthenticationServiceTests.cs

public class AuthenticationServiceTests
{
    [Fact]
    public async Task Dado_CredenciaisValidas_Quando_Autenticar_Entao_DeveRetornarToken()

    [Fact]
    public async Task Dado_CredenciaisInvalidas_Quando_Autenticar_Entao_DeveLancarExcecao()

    [Fact]
    public async Task Dado_RefreshTokenValido_Quando_Renovar_Entao_DeveRetornarNovoToken()

    [Fact]
    public async Task Dado_RefreshTokenExpirado_Quando_Renovar_Entao_DeveLancarExcecao()
}

public class ExternalAuthenticationServiceTests
{
    [Fact]
    public async Task Dado_TokenGoogleValido_Quando_AutenticarExterno_Entao_DeveRetornarToken()

    [Fact]
    public async Task Dado_ProviderInexistente_Quando_AutenticarExterno_Entao_DeveLancarExcecao()
}

public class ImpersonationServiceTests
{
    [Fact]
    public async Task Dado_AdminComPermissao_Quando_ImpersonarUsuario_Entao_DeveRetornarTokenImpersonado()

    [Fact]
    public async Task Dado_UsuarioSemPermissao_Quando_ImpersonarUsuario_Entao_DeveLancarExcecao()
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.Middleware.Web.Core/Eaf.Middleware.Web.Core.csproj --configuration Release
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage"
```

## Critérios de Aceite

1. `TokenAuthController` tem ≤300 linhas (reduzido de 1215)
2. Construtor tem ≤6 parâmetros (reduzido de 22)
3. 3 novos services criados com interfaces
4. Zero mudança de comportamento nos endpoints
5. Todos os testes existentes passam
6. Novos testes para cada service
7. Cobertura não diminuiu

## Notas para o Sub-Agent

- **MUITO ALTA COMPLEXIDADE**: Se falhar 3x em qualquer etapa, PARAR e reportar
- Executar em 3 etapas incrementais — NUNCA refatorar tudo de uma vez
- Manter os endpoints HTTP exatamente iguais (URLs, verbs, DTOs)
- Os services devem ser `ITransientDependency` para Castle Windsor registrar automaticamente
- Se um teste existente falhar, é provável que a assinatura de um método mudou — verificar
- Os DTOs (`AuthenticateModel`, `ExternalAuthenticateModel`, etc.) NÃO devem mudar
- **Se complexidade exceder 4 horas, reportar progresso parcial e criar nova tarefa para continuar**
