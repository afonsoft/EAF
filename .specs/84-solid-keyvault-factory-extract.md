# 84 — Extrair Factory do KeyVaultSecretManager (OCP)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 5 — SOLID / Clean Architecture |
| **Princípio** | OCP — Open/Closed Principle + Factory Pattern |
| **Complexidade** | MÉDIA |
| **Risco** | MÉDIO — Altera criação de secret managers |
| **Dependências** | Executar APÓS tarefa 80 (Service Locator) |
| **Arquivos Modificados** | 3 arquivos de produção + 1 novo factory |

## Objetivo

Extrair a lógica de criação de `IKeyVaultManager` em um Factory Method, removendo `if/else` encadeado no startup.

## Motivo

Atualmente a criação do `IKeyVaultManager` é feita no `EafKeyVaultModule` com lógica condicional:

```csharp
// Padrão atual (simplificado):
if (options.Provider == "Azure")
    IocManager.Register<IKeyVaultManager, AzureKeyVaultManager>();
else if (options.Provider == "OCI")
    IocManager.Register<IKeyVaultManager, OciKeyVaultManager>();
else
    IocManager.Register<IKeyVaultManager, NullKeyVaultManager>();
```

**Problemas**:
- Adicionar novo provider (AWS Secrets Manager, HashiCorp Vault) exige modificar o módulo
- Viola OCP — classe não está fechada para modificação
- Lógica de criação misturada com lógica de módulo

## Refatoração Proposta

### 1. IKeyVaultManagerFactory (NOVO)

```csharp
// ARQUIVO: src/Eaf.KeyVault/IKeyVaultManagerFactory.cs
namespace Eaf.KeyVault
{
    /// <summary>
    /// Factory para criar instâncias de IKeyVaultManager baseado na configuração.
    /// </summary>
    public interface IKeyVaultManagerFactory
    {
        /// <summary>
        /// Cria uma instância de IKeyVaultManager baseada nas opções fornecidas.
        /// </summary>
        /// <param name="options">Opções de configuração do KeyVault.</param>
        /// <returns>Instância de IKeyVaultManager.</returns>
        IKeyVaultManager Create(EafKeyVaultOptions options);
    }
}
```

### 2. KeyVaultManagerFactory (NOVO)

```csharp
// ARQUIVO: src/Eaf.KeyVault/KeyVaultManagerFactory.cs
namespace Eaf.KeyVault
{
    /// <summary>
    /// Implementação da factory de KeyVaultManager.
    /// Responsável por criar a instância correta baseada no provider configurado.
    /// </summary>
    public class KeyVaultManagerFactory : IKeyVaultManagerFactory, ITransientDependency
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// KeyVaultManagerFactory.
        /// </summary>
        /// <param name="iocManager">Parâmetro iocManager.</param>
        public KeyVaultManagerFactory(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        /// <summary>
        /// Cria uma instância de IKeyVaultManager baseada nas opções fornecidas.
        /// </summary>
        /// <param name="options">Opções de configuração do KeyVault.</param>
        /// <returns>Instância de IKeyVaultManager.</returns>
        public IKeyVaultManager Create(EafKeyVaultOptions options)
        {
            return options.Provider?.ToUpperInvariant() switch
            {
                "AZURE" => _iocManager.Resolve<AzureKeyVaultManager>(),
                "OCI" => _iocManager.Resolve<OciKeyVaultManager>(),
                _ => new NullKeyVaultManager(options, NullLogger.Instance)
            };
        }
    }
}
```

### 3. Atualizar EafKeyVaultModule

```csharp
// ARQUIVO: src/Eaf.KeyVault/EafKeyVaultModule.cs
// ── ANTES ──
// Lógica condicional direta no Initialize()

// ── DEPOIS ──
public override void Initialize()
{
    IocManager.RegisterAssemblyByConvention(typeof(EafKeyVaultModule).GetAssembly());
    IocManager.Register<IKeyVaultManagerFactory, KeyVaultManagerFactory>();

    var factory = IocManager.Resolve<IKeyVaultManagerFactory>();
    var options = IocManager.Resolve<EafKeyVaultOptions>();
    var manager = factory.Create(options);

    IocManager.Register<IKeyVaultManager>(manager);
}
```

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.KeyVault.Tests/KeyVaultManagerFactoryTests.cs

public class KeyVaultManagerFactoryTests
{
    [Fact]
    public void Dado_ProviderAzure_Quando_CriarKeyVaultManager_Entao_DeveRetornarAzureManager()

    [Fact]
    public void Dado_ProviderNull_Quando_CriarKeyVaultManager_Entao_DeveRetornarNullManager()

    [Fact]
    public void Dado_ProviderDesconhecido_Quando_CriarKeyVaultManager_Entao_DeveRetornarNullManager()

    [Theory]
    [InlineData("azure")]
    [InlineData("AZURE")]
    [InlineData("Azure")]
    public void Dado_ProviderAzureCaseInsensitive_Quando_CriarKeyVaultManager_Entao_DeveRetornarAzureManager(string provider)
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.KeyVault/Eaf.KeyVault.csproj --configuration Release
dotnet test test/Eaf.KeyVault.Tests/Eaf.KeyVault.Tests.csproj --collect:"XPlat Code Coverage"
dotnet build Eaf.sln --configuration Release
```

## Critérios de Aceite

1. `IKeyVaultManagerFactory` e `KeyVaultManagerFactory` criados
2. `EafKeyVaultModule` usa factory ao invés de `if/else`
3. Padrão case-insensitive para nome do provider
4. `NullKeyVaultManager` como default (Null Object Pattern mantido)
5. Testes existentes passam
6. Novos testes para factory
7. XML docs em todas as APIs públicas

## Notas para o Sub-Agent

- Verificar como `EafKeyVaultModule.Initialize()` registra o provider atualmente — pode variar do exemplo
- Se o módulo já usa Strategy Pattern parcialmente, apenas completar a extração
- `NullKeyVaultManager` SEMPRE deve ser o default — nunca lançar exceção para provider desconhecido
- Se OCI provider não existe no código atual, ignorar e documentar
