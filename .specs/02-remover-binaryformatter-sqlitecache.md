# 02 — Remover BinaryFormatter do EafSqliteCache

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 1 — Performance `src/` |
| **Complexidade** | ALTA |
| **Risco** | MÉDIO — Altera serialização de cache |
| **Dependências** | Idealmente executar após tarefa 01 (mesmo padrão) |
| **Arquivos Modificados** | 1 arquivo de produção + 1 arquivo de teste |

## Objetivo

Remover `BinaryFormatter` do módulo `Eaf.SqliteCache`. Mesmo problema da tarefa 01: não compila no .NET 10.

## Motivo

- `BinaryFormatter` removido no .NET 10 — compilação falha
- Vulnerabilidade de segurança conhecida
- `ConfigurationContainer()` instanciado a cada chamada (linhas 347, 388) — alocação desnecessária

## Arquivos Afetados

### Produção

**`src/Eaf.SqliteCache/Runtime/Caching/Sqlite/EafSqliteCache.cs`**

#### Símbolos a Modificar

```csharp
// ── REMOVER imports (linhas 4-5) ──
using System.Runtime.Serialization;          // REMOVER
using System.Runtime.Serialization.Formatters.Binary; // REMOVER

// ── ADICIONAR import ──
using System.Text.Json;

// ── ADICIONAR: Campo estático (mesmo padrão da tarefa 01) ──
private static readonly Lazy<IExtendedXmlSerializer> _xmlSerializer = new(() =>
    new ConfigurationContainer()
        .UseAutoFormatting()
        .UseOptimizedNamespaces()
        .Create());

// ── MODIFICAR: ObjectToByteArray (~linha 340) ──
// ANTES: Cria ConfigurationContainer() e fallback BinaryFormatter
// DEPOIS: Usa _xmlSerializer estático e fallback System.Text.Json
private static byte[] ObjectToByteArray(object objData)
{
    if (objData == null) return default;
    try
    {
        using var contentStream = new MemoryStream();
        using (var writer = XmlWriter.Create(contentStream))
        {
            _xmlSerializer.Value.Serialize(writer, objData);
            writer.Flush();
        }
        contentStream.Seek(0, SeekOrigin.Begin);
        return Encoding.ASCII.GetBytes(new StreamReader(contentStream).ReadToEnd());
    }
    catch
    {
        return JsonSerializer.SerializeToUtf8Bytes(objData);
    }
}

// ── MODIFICAR: ByteArrayToObject (~linha 375) ──
// ANTES: Cria ConfigurationContainer() e fallback BinaryFormatter
// DEPOIS: Usa _xmlSerializer estático e fallback System.Text.Json
private static object ByteArrayToObject(byte[] byteArray)
{
    if (byteArray == null || !byteArray.Any()) return default;
    try
    {
        using var contentStream = new MemoryStream(byteArray);
        using var reader = XmlReader.Create(contentStream);
        return _xmlSerializer.Value.Deserialize(reader);
    }
    catch
    {
        return JsonSerializer.Deserialize<object>(byteArray);
    }
}

// ── REMOVER COMPLETAMENTE (linhas 406-425) ──
#pragma warning disable SYSLIB0011
private static byte[] SerializeToStream(object objectType) { ... }   // REMOVER
private static object DeserializeFromStream(byte[] objectByte) { ... } // REMOVER
#pragma warning disable SYSLIB0011
```

### Teste

**`test/Eaf.SqliteCache.Tests/` — Criar ou atualizar**

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.SqliteCache.Tests/EafSqliteCacheSerializationTests.cs

public class EafSqliteCacheSerializationTests
{
    [Fact]
    public void Dado_ObjetoSimples_Quando_SerializarEDeserializar_Entao_DeveRetornarObjetoEquivalente()

    [Fact]
    public void Dado_ObjetoNulo_Quando_Serializar_Entao_DeveRetornarDefault()

    [Fact]
    public void Dado_ArrayVazio_Quando_Deserializar_Entao_DeveRetornarDefault()

    [Fact]
    public void Dado_XmlInvalido_Quando_Deserializar_Entao_DeveFazerFallbackParaJson()

    [Fact]
    public void Dado_CacheItem_Quando_SetEGet_Entao_DeveRetornarValorCorreto()
    // Teste de integração: Set → TryGetValue → comparar valor

    [Fact]
    public void Dado_CacheItem_Quando_SetERemove_Entao_TryGetDeveRetornarFalse()
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.SqliteCache/Eaf.SqliteCache.csproj --configuration Release
dotnet test test/Eaf.SqliteCache.Tests/Eaf.SqliteCache.Tests.csproj --collect:"XPlat Code Coverage"
dotnet build Eaf.sln --configuration Release
```

## Critérios de Aceite

1. Zero referências a `BinaryFormatter` ou `System.Runtime.Serialization.Formatters.Binary`
2. Zero `#pragma warning disable SYSLIB0011`
3. `ConfigurationContainer` criado uma única vez (static lazy)
4. Fallback usa `System.Text.Json`
5. Todos os testes passam
6. Cobertura não diminuiu

## Notas para o Sub-Agent

- Aplicar exatamente o mesmo padrão da tarefa 01
- EafSqliteCache herda de `CacheBase` (ABP) — não alterar assinaturas públicas
- O módulo é totalmente síncrono (sem métodos async) — isso é esperado
- A classe usa `lock` para thread-safety — respeitar o padrão existente
