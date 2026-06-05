# 83 — Segregar IEafWorkerBase em Interfaces Menores (ISP)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 5 — SOLID / Clean Architecture |
| **Princípio** | ISP — Interface Segregation Principle |
| **Complexidade** | MÉDIA |
| **Risco** | MÉDIO — Altera interface pública, pode quebrar implementações |
| **Dependências** | Executar APÓS tarefa 80 (Service Locator) |
| **Arquivos Modificados** | 3 arquivos de produção |

## Objetivo

Segregar `IEafWorkerBase` (5 propriedades com setters públicos) em interfaces menores e mais coesas.

## Motivo

A interface atual viola ISP:

```csharp
public interface IEafWorkerBase : IHostedService, IDomainService, ISingletonDependency
{
    public IIocManager IocManager { get; set; }           // DI container — não deveria ser exposto
    public IEventBus EventBus { get; set; }               // OK para workers que publicam eventos
    public ILocalizationManager LocalizationManager { set; } // OK para workers que localizam
    public ILogger Logger { get; set; }                   // OK — logging universal
    public IObjectMapper ObjectMapper { get; set; }       // OK para workers que mapeiam
}
```

**Problemas**:
1. `IIocManager` na interface pública — expõe Service Locator
2. **Setters públicos** para todas as propriedades — qualquer consumidor pode sobrescrever dependências
3. Worker simples (só logging + timer) é forçado a implementar 5 propriedades

## Refatoração Proposta

### 1. Remover `IIocManager` da interface

```csharp
// ARQUIVO: src/Eaf.Middleware.Worker/IEafWorkerBase.cs
// ── ANTES ──
public interface IEafWorkerBase : IHostedService, IDomainService, ISingletonDependency
{
    public IIocManager IocManager { get; set; }
    public IEventBus EventBus { get; set; }
    public ILocalizationManager LocalizationManager { set; }
    public ILogger Logger { get; set; }
    public IObjectMapper ObjectMapper { get; set; }
}

// ── DEPOIS ──
/// <summary>
/// Interface base para workers EAF com suporte a logging e infraestrutura ABP.
/// </summary>
public interface IEafWorkerBase : IHostedService, IDomainService, ISingletonDependency
{
    /// <summary>
    /// Obtém ou define o logger.
    /// </summary>
    ILogger Logger { get; set; }

    /// <summary>
    /// Obtém ou define o event bus.
    /// </summary>
    IEventBus EventBus { get; set; }

    /// <summary>
    /// Define o localization manager.
    /// </summary>
    ILocalizationManager LocalizationManager { set; }

    /// <summary>
    /// Obtém ou define o object mapper.
    /// </summary>
    IObjectMapper ObjectMapper { get; set; }
}
```

### 2. Manter `IIocManager` em `EafWorkerBase` (classe concreta)

```csharp
// ARQUIVO: src/Eaf.Middleware.Worker/EafWorkerBase.cs
// IocManager continua como propriedade da classe base, mas não da interface:
public abstract class EafWorkerBase : BackgroundService, IEafWorkerBase
{
    /// <summary>
    /// Obtém ou define IocManager (injetado por Castle Windsor).
    /// Não exposto na interface — uso interno.
    /// </summary>
    public IIocManager IocManager { get; set; }

    // ... resto mantido
}
```

### 3. Atualizar implementações que dependem de `IIocManager` via interface

```bash
# Encontrar quem usa IEafWorkerBase.IocManager:
grep -rn "IEafWorkerBase.*IocManager" src/ test/ --include="*.cs"
```

Se algum consumidor acessa `IocManager` via a interface `IEafWorkerBase`, converter para acesso via `EafWorkerBase` ou injetar a dependência diretamente.

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.Middleware.Worker.Tests/EafWorkerBaseTests.cs

[Fact]
public void Dado_IEafWorkerBase_Quando_VerificarInterface_Entao_NaoDeveConterIocManager()
{
    // Dado
    var interfaceType = typeof(IEafWorkerBase);
    // Quando
    var hasIocManager = interfaceType.GetProperty("IocManager");
    // Então
    hasIocManager.ShouldBeNull();
}

[Fact]
public void Dado_EafWorkerBase_Quando_CriarWorker_Entao_DeveImplementarIEafWorkerBase()
{
    // Dado/Quando
    var worker = new TestWorker(); // classe concreta de teste
    // Então
    worker.ShouldBeAssignableTo<IEafWorkerBase>();
    worker.Logger.ShouldNotBeNull();
}

[Fact]
public void Dado_EafWorkerBase_Quando_CriarSemInjecao_Entao_DeveUsarNullObjects()
{
    // Dado/Quando
    var worker = new TestWorker();
    // Então
    worker.Logger.ShouldBe(NullLogger.Instance);
    worker.EventBus.ShouldBe(NullEventBus.Instance);
    worker.ObjectMapper.ShouldBe(NullObjectMapper.Instance);
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.Middleware.Worker/Eaf.Middleware.Worker.csproj --configuration Release
dotnet build Eaf.sln --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage"

# Verificar que IocManager não está na interface:
grep -n "IocManager" src/Eaf.Middleware.Worker/IEafWorkerBase.cs
# Deve retornar 0 resultados
```

## Critérios de Aceite

1. `IIocManager` removido de `IEafWorkerBase` (interface)
2. `IIocManager` mantido em `EafWorkerBase` (classe concreta)
3. Nenhuma implementação de `IEafWorkerBase` quebrada
4. Testes existentes passam
5. Novos testes de interface adicionados
6. XML docs em todas as propriedades da interface

## Notas para o Sub-Agent

- `IIocManager` em interface pública é Service Locator disfarçado — remover
- Manter `IocManager` na classe concreta `EafWorkerBase` — Castle Windsor precisa para property injection
- Se algum teste acessa `IEafWorkerBase.IocManager`, ajustar o teste para usar a classe concreta
- Esta tarefa é relativamente isolada — baixo risco de cascata
