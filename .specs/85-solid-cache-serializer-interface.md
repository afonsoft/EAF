# 85 — Extrair ICacheSerializer para Open/Closed (OCP)

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 5 — SOLID / Clean Architecture |
| **Princípio** | OCP — Open/Closed Principle |
| **Complexidade** | MÉDIA |
| **Risco** | MÉDIO — Altera serialização do cache (afeta dados em produção) |
| **Dependências** | Executar APÓS tarefas 01 e 02 (BinaryFormatter removal) |
| **Arquivos Modificados** | 4 arquivos de produção + 1 nova interface |

## Objetivo

Extrair a lógica de serialização `ObjectToByteArray` / `ByteArrayToObject` em uma interface `ICacheSerializer`, permitindo trocar a implementação sem modificar as classes de cache.

## Motivo

Após a remoção do `BinaryFormatter` (tarefas 01/02), tanto `EafSqlServerCache` quanto `EafSqliteCache` terão métodos idênticos de serialização (`System.Text.Json`). Este código duplicado viola DRY e dificulta a troca futura de serializador (ex: MessagePack, Protobuf).

## Refatoração Proposta

### 1. ICacheSerializer (NOVO)

```csharp
// ARQUIVO: src/Eaf.SqlServerCache/Serialization/ICacheSerializer.cs
// (ou local compartilhado se existir projeto comum)
namespace Eaf.Runtime.Caching.Serialization
{
    /// <summary>
    /// Interface para serialização de objetos para cache.
    /// Permite trocar a implementação de serialização sem alterar as classes de cache.
    /// </summary>
    public interface ICacheSerializer
    {
        /// <summary>
        /// Serializa um objeto para array de bytes.
        /// </summary>
        /// <param name="obj">Objeto a serializar.</param>
        /// <returns>Array de bytes representando o objeto.</returns>
        byte[] Serialize(object obj);

        /// <summary>
        /// Desserializa um array de bytes para um objeto.
        /// </summary>
        /// <param name="data">Array de bytes.</param>
        /// <returns>Objeto desserializado.</returns>
        object Deserialize(byte[] data);
    }
}
```

### 2. JsonCacheSerializer (NOVO)

```csharp
// ARQUIVO: src/Eaf.SqlServerCache/Serialization/JsonCacheSerializer.cs
namespace Eaf.Runtime.Caching.Serialization
{
    /// <summary>
    /// Implementação de ICacheSerializer usando System.Text.Json.
    /// </summary>
    public class JsonCacheSerializer : ICacheSerializer
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Serializa um objeto para array de bytes usando System.Text.Json.
        /// </summary>
        /// <param name="obj">Objeto a serializar.</param>
        /// <returns>Array de bytes representando o objeto.</returns>
        public byte[] Serialize(object obj)
        {
            if (obj == null) return null;
            return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
        }

        /// <summary>
        /// Desserializa um array de bytes para um objeto usando System.Text.Json.
        /// </summary>
        /// <param name="data">Array de bytes.</param>
        /// <returns>Objeto desserializado.</returns>
        public object Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            return JsonSerializer.Deserialize<object>(data, _options);
        }
    }
}
```

### 3. Atualizar EafSqlServerCache

```csharp
// ARQUIVO: src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCache.cs
// ── ANTES ──
public class EafSqlServerCache : CacheBase
{
    private byte[] ObjectToByteArray(object obj) { /* inline JSON */ }
    private object ByteArrayToObject(byte[] data) { /* inline JSON */ }
}

// ── DEPOIS ──
public class EafSqlServerCache : CacheBase
{
    private readonly ICacheSerializer _serializer;

    public EafSqlServerCache(/* params existentes */, ICacheSerializer serializer)
    {
        _serializer = serializer ?? new JsonCacheSerializer();
    }

    // Substituir chamadas a ObjectToByteArray/ByteArrayToObject por:
    // _serializer.Serialize(obj)
    // _serializer.Deserialize(data)
}
```

### 4. Atualizar EafSqliteCache (mesmo padrão)

```csharp
// ARQUIVO: src/Eaf.SqliteCache/Caching/Sqlite/EafSqliteCache.cs
// Mesmo padrão: injetar ICacheSerializer, delegar serialização
```

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.SqlServerCache.Tests/Serialization/JsonCacheSerializerTests.cs

public class JsonCacheSerializerBddTests
{
    [Fact]
    public void Dado_ObjetoValido_Quando_Serializar_Entao_DeveRetornarBytesValidos()

    [Fact]
    public void Dado_BytesSerializados_Quando_Desserializar_Entao_DeveRetornarObjetoEquivalente()

    [Fact]
    public void Dado_ObjetoNull_Quando_Serializar_Entao_DeveRetornarNull()

    [Fact]
    public void Dado_BytesVazio_Quando_Desserializar_Entao_DeveRetornarNull()

    [Fact]
    public void Dado_ObjetoComPropriedadesNull_Quando_Serializar_Entao_DeveIgnorarNulls()

    [Fact]
    public void Dado_SerializerCustom_Quando_InjetarEmCache_Entao_DeveUsarSerializerCustom()
    // Verificar que ICacheSerializer pode ser substituído
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.SqlServerCache/Eaf.SqlServerCache.csproj --configuration Release
dotnet build src/Eaf.SqliteCache/Eaf.SqliteCache.csproj --configuration Release
dotnet test test/Eaf.SqlServerCache.Tests/Eaf.SqlServerCache.Tests.csproj --collect:"XPlat Code Coverage"
dotnet test test/Eaf.SqliteCache.Tests/Eaf.SqliteCache.Tests.csproj --collect:"XPlat Code Coverage"
dotnet build Eaf.sln --configuration Release
```

## Critérios de Aceite

1. `ICacheSerializer` interface criada
2. `JsonCacheSerializer` implementação criada
3. `EafSqlServerCache` e `EafSqliteCache` usam `ICacheSerializer` injetado
4. `JsonCacheSerializer` é o default quando não injetado
5. Código duplicado de serialização removido
6. Todos os testes passam (existentes + novos)
7. XML docs em todas as APIs públicas

## Notas para o Sub-Agent

- Esta tarefa depende de 01/02 (BinaryFormatter removal) — os métodos de serialização devem já estar usando System.Text.Json
- Se 01/02 não foram executadas ainda, NÃO executar esta tarefa (dependência)
- `ICacheSerializer` deve ficar no projeto `Eaf.SqlServerCache` (ou em um projeto compartilhado se existir)
- O default `new JsonCacheSerializer()` no construtor garante backward compatibility
- Se a classe de cache não recebe parâmetros via construtor (ABP gerencia), usar property injection
