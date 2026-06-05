# 01 — Remover BinaryFormatter do EafSqlServerCache

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 1 — Performance `src/` |
| **Complexidade** | ALTA |
| **Risco** | MÉDIO — Altera serialização de cache, pode afetar dados existentes |
| **Dependências** | Nenhuma (pode executar isoladamente) |
| **Arquivos Modificados** | 1 arquivo de produção + 1 arquivo de teste |

## Objetivo

Remover completamente o uso de `BinaryFormatter` do módulo `Eaf.SqlServerCache`. O `BinaryFormatter` foi marcado como obsoleto no .NET 9 (SYSLIB0011) e **removido no .NET 10**, o que significa que o código atual **não compila** no runtime alvo.

## Motivo

- **Compilação**: `BinaryFormatter` foi removido no .NET 10 — o código não compila
- **Segurança**: `BinaryFormatter` é vulnerável a ataques de desserialização (CVE conhecidos)
- **Performance**: `ConfigurationContainer()` é criado a cada chamada de serialização — alocação desnecessária

## Arquivos Afetados

### Produção

**`src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCache.cs`**

#### Símbolos a Modificar

```csharp
// ── REMOVER imports (linhas 9-10) ──
using System.Runtime.Serialization;          // REMOVER
using System.Runtime.Serialization.Formatters.Binary; // REMOVER

// ── ADICIONAR import ──
using System.Text.Json;

// ── MODIFICAR: Tornar serializer estático (evita alocação por chamada) ──
// ANTES (criado em ObjectToByteArray e ByteArrayToObject, linhas 201, 232):
var serializer = new ConfigurationContainer().UseAutoFormatting()
    .UseOptimizedNamespaces()
    .Create();

// DEPOIS: Campo estático lazy singleton
private static readonly Lazy<IExtendedXmlSerializer> _xmlSerializer = new(() =>
    new ConfigurationContainer()
        .UseAutoFormatting()
        .UseOptimizedNamespaces()
        .Create());

// ── MODIFICAR: ObjectToByteArray (linha 192) ──
// ANTES: Fallback para BinaryFormatter no catch
private static byte[] ObjectToByteArray(object objData)
{
    // ... try com ExtendedXmlSerializer ...
    catch { return SerializeToStream(objData); } // SerializeToStream usa BinaryFormatter!
}

// DEPOIS: Fallback para System.Text.Json
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

// ── MODIFICAR: ByteArrayToObject (linha 223) ──
// ANTES: Fallback para BinaryFormatter no catch
private static object ByteArrayToObject(byte[] byteArray)
{
    // ... try com ExtendedXmlSerializer ...
    catch { return DeserializeFromStream(byteArray); } // DeserializeFromStream usa BinaryFormatter!
}

// DEPOIS: Fallback para System.Text.Json
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

// ── REMOVER COMPLETAMENTE (linhas 250-269) ──
#pragma warning disable SYSLIB0011
private static byte[] SerializeToStream(object objectType) { ... }   // REMOVER
private static object DeserializeFromStream(byte[] objectByte) { ... } // REMOVER
#pragma warning disable SYSLIB0011
```

### Teste

**`test/Eaf.SqlServerCache.Tests/` — Criar ou atualizar arquivo de teste**

## Cenários de Teste

### Testes Existentes (verificar que não quebram)

Executar todos os testes existentes do módulo SqlServerCache e garantir que passam.

### Novos Cenários de Teste (BDD — Dado/Quando/Então)

```csharp
// ARQUIVO: test/Eaf.SqlServerCache.Tests/EafSqlServerCacheSerializationTests.cs

public class EafSqlServerCacheSerializationTests
{
    [Fact]
    public void Dado_ObjetoSimples_Quando_SerializarEDeserializar_Entao_DeveRetornarObjetoEquivalente()
    // Serializa string "teste" → byte[] → deserializa → compara

    [Fact]
    public void Dado_ObjetoComplexo_Quando_SerializarEDeserializar_Entao_DevePreservarPropriedades()
    // Serializa objeto com múltiplas propriedades → byte[] → deserializa → compara cada prop

    [Fact]
    public void Dado_ObjetoNulo_Quando_Serializar_Entao_DeveRetornarDefault()
    // ObjectToByteArray(null) → deve retornar default(byte[])

    [Fact]
    public void Dado_ArrayVazio_Quando_Deserializar_Entao_DeveRetornarDefault()
    // ByteArrayToObject(new byte[0]) → deve retornar default

    [Fact]
    public void Dado_ArrayNulo_Quando_Deserializar_Entao_DeveRetornarDefault()
    // ByteArrayToObject(null) → deve retornar default

    [Fact]
    public void Dado_XmlInvalido_Quando_Deserializar_Entao_DeveFazerFallbackParaJson()
    // Cria byte[] com JSON válido mas XML inválido → deve deserializar via System.Text.Json

    [Fact]
    public void Dado_MultiplasThreads_Quando_SerializarConcorrentemente_Entao_NaoDeveLancarExcecao()
    // Teste de concorrência: 10 threads serializando simultaneamente → sem exceção
}
```

## Comandos de Verificação

```bash
# Build do módulo
dotnet build src/Eaf.SqlServerCache/Eaf.SqlServerCache.csproj --configuration Release

# Testes do módulo
dotnet test test/Eaf.SqlServerCache.Tests/Eaf.SqlServerCache.Tests.csproj --collect:"XPlat Code Coverage"

# Build completo (verificar que não quebrou nada)
dotnet build Eaf.sln --configuration Release
```

## Critérios de Aceite

1. ✅ Nenhuma referência a `BinaryFormatter` ou `System.Runtime.Serialization.Formatters.Binary`
2. ✅ Nenhum `#pragma warning disable SYSLIB0011`
3. ✅ `ConfigurationContainer` criado apenas uma vez (static lazy)
4. ✅ Fallback usa `System.Text.Json` em vez de `BinaryFormatter`
5. ✅ Todos os testes existentes passam
6. ✅ Novos testes de serialização passam
7. ✅ Build completo sem warnings de obsolescência
8. ✅ Cobertura não diminuiu

## Notas para o Sub-Agent

- O `ExtendedXmlSerializer` já é usado como serializer primário — apenas o fallback precisa mudar
- O campo estático `_xmlSerializer` é thread-safe por design (Lazy com LazyThreadSafetyMode padrão)
- Manter a compatibilidade com `CacheBase` (classe base do ABP)
- Não alterar a assinatura pública dos métodos `TryGetValue`, `Set`, `Remove`
- Se encontrar complexidade inesperada na desserialização de dados existentes, reportar e parar
