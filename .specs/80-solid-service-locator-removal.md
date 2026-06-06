# 80 — Remover Service Locator Anti-Pattern (IocManager.Instance.Resolve)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 5 — SOLID / Clean Architecture |
| **Princípio** | DIP — Dependency Inversion Principle |
| **Complexidade** | ALTA |
| **Risco** | ALTO — Altera resolução de dependências, pode quebrar DI |
| **Dependências** | Executar APÓS todas as fases 1-4 |
| **Arquivos Modificados** | 5+ arquivos de produção + testes |

## Objetivo

Substituir chamadas a `IocManager.Instance.Resolve<T>()` (Service Locator) por injeção via construtor em 5+ classes.

## Motivo

- **DIP violado**: Service Locator inverte o controle na direção errada — a classe busca suas dependências em vez de recebê-las
- **Testabilidade**: Classes com Service Locator são difíceis de testar — não há como injetar mocks sem configurar o container global
- **Acoplamento**: Todas as classes ficam acopladas a `IocManager` (ABP container)
- **20+ ocorrências** encontradas no codebase

## Arquivos Afetados

### 1. EafWorkerBase — PRINCIPAL

**`src/Eaf.Middleware.Worker/EafWorkerBase.cs` (linhas 35-55)**

```csharp
// ── ANTES ──
protected EafWorkerBase()
{
    Logger = NullLogger.Instance;
    LocalizationManager = NullLocalizationManager.Instance;
    EventBus = NullEventBus.Instance;
    ObjectMapper = NullObjectMapper.Instance;
    LocalizationSourceName = DefaultLocalizationSourceName;
    SetDependencies(); // Service Locator!
}

private void SetDependencies()
{
    if (IocManager.IsRegistered<ILoggerFactory>())
    {
        Logger = IocManager.Resolve<ILoggerFactory>().Create(typeof(EafWorkerBase));
    }
    if (IocManager.IsRegistered<IEventBus>())
    {
        EventBus = IocManager.Resolve<IEventBus>();
    }
    if (IocManager.IsRegistered<ILocalizationManager>())
    {
        LocalizationManager = IocManager.Resolve<ILocalizationManager>();
    }
    if (IocManager.IsRegistered<IObjectMapper>())
    {
        ObjectMapper = IocManager.Resolve<IObjectMapper>();
    }
}

// ── DEPOIS ──
// NOTA: EafWorkerBase herda de BackgroundService (Microsoft.Extensions.Hosting)
// BackgroundService não recebe parâmetros no construtor por design
// Castle Windsor injeta via Property Injection (IocManager { get; set; })
// A solução é MANTER property injection MAS usar IIocManager injetado ao invés de IocManager estático

// Opção 1 — Mover SetDependencies para PostInitialize/Lifecycle:
protected EafWorkerBase()
{
    Logger = NullLogger.Instance;
    LocalizationManager = NullLocalizationManager.Instance;
    EventBus = NullEventBus.Instance;
    ObjectMapper = NullObjectMapper.Instance;
    LocalizationSourceName = DefaultLocalizationSourceName;
    // REMOVIDO: SetDependencies() — dependências injetadas via property
}

// Castle Windsor faz property injection APÓS o construtor:
// public IIocManager IocManager { get; set; } — JÁ EXISTE!
// public IEventBus EventBus { get; set; } — JÁ EXISTE!
// public ILogger Logger { get; set; } — JÁ EXISTE!
// public ILocalizationManager LocalizationManager { set; } — JÁ EXISTE!
// public IObjectMapper ObjectMapper { get; set; } — JÁ EXISTE!

// REMOVER: private void SetDependencies() — não é mais necessário
```

**ATENÇÃO**: Castle Windsor no ABP faz property injection automática para propriedades públicas com setter. As propriedades já existem em `EafWorkerBase` e em `IEafWorkerBase`. O `SetDependencies()` está duplicando o trabalho que Castle Windsor já faz.

**Risco**: Testar se Castle Windsor realmente injeta em classes que herdam de `BackgroundService`. Se não injetar, manter `SetDependencies()` e documentar a limitação.

### 2. KeyVaultSecretManager

**`src/Eaf.KeyVault/KeyVaultSecretManager.cs` (linhas 41-43)**

```csharp
// ── ANTES ──
public KeyVaultSecretManager(EafKeyVaultOptions options)
{
    Logger = NullLogger.Instance;
    if (IocManager.Instance.IsRegistered<ILoggerFactory>())
    {
        Logger = IocManager.Instance.Resolve<ILoggerFactory>().Create(typeof(KeyVaultSecretManager));
    }
    // ...
}

// ── DEPOIS ──
public KeyVaultSecretManager(EafKeyVaultOptions options, ILoggerFactory loggerFactory = null)
{
    Logger = loggerFactory?.Create(typeof(KeyVaultSecretManager)) ?? NullLogger.Instance;
    // ...
}
```

### 3. TokenAuthController (linha 520)

**`src/Eaf.Middleware.Web.Core/Controllers/TokenAuthController.cs` (linha 520)**

```csharp
// ── ANTES ──
var claims = IocManager.Instance.Resolve<IPrincipalAccessor>()?.Principal;

// ── DEPOIS (adicionar ao construtor) ──
private readonly IPrincipalAccessor _principalAccessor;

// No construtor:
// IPrincipalAccessor principalAccessor  ← adicionar ao construtor
_principalAccessor = principalAccessor;

// No método:
var claims = _principalAccessor?.Principal;
```

### 4. Outros Service Locators (menor prioridade)

Executar busca e corrigir:
```bash
grep -rn "IocManager.Instance.Resolve" src/ --include="*.cs" | grep -v "bin/" | grep -v "obj/" | grep -v "Test"
grep -rn "IocManager.Instance.IsRegistered" src/ --include="*.cs" | grep -v "bin/" | grep -v "obj/" | grep -v "Test"
```

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.Middleware.Worker.Tests/EafWorkerBaseTests.cs (se existir)
[Fact]
public void Dado_EafWorkerBase_Quando_CriarSemDependencias_Entao_DeveUsarNullInstances()
// Logger = NullLogger.Instance, EventBus = NullEventBus.Instance, etc.

[Fact]
public void Dado_EafWorkerBase_Quando_InjetarLogger_Entao_DeveUsarLoggerInjetado()
// Simular property injection → verificar que Logger correto é usado

// ARQUIVO: test/Eaf.KeyVault.Tests/KeyVaultSecretManagerTests.cs (existente)
[Fact]
public void Dado_LoggerFactory_Quando_CriarKeyVaultSecretManager_Entao_DeveUsarLogger()

[Fact]
public void Dado_SemLoggerFactory_Quando_CriarKeyVaultSecretManager_Entao_DeveUsarNullLogger()
```

## Comandos de Verificação

```bash
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage"

# Verificar que não restou Service Locator:
grep -rn "IocManager.Instance.Resolve" src/ --include="*.cs" | grep -v "bin/" | grep -v "obj/"
```

## Critérios de Aceite

1. Zero chamadas `IocManager.Instance.Resolve<T>()` nas classes modificadas
2. Dependências injetadas via construtor ou property injection
3. Null Object Pattern mantido (NullLogger, NullEventBus) para defaults
4. Todos os testes passam
5. Build compila sem erros
6. Cobertura não diminuiu

## Notas para o Sub-Agent

- **ALTA COMPLEXIDADE**: Testar cada mudança ANTES de passar para a próxima classe
- Castle Windsor faz property injection automática — verificar se funciona para `BackgroundService`
- Se Castle Windsor NÃO faz property injection para `BackgroundService`, manter `SetDependencies()` e documentar
- O construtor de `TokenAuthController` já tem 22 parâmetros — adicionar mais 1 é OK neste ponto (será refatorado na tarefa 82)
- **Se falhar 3x, voltar ao início e reportar complexidade**
