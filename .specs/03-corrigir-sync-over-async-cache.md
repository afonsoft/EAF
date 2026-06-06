# 03 — Corrigir Sync-over-Async no EafSqlServerCache

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 1 — Performance `src/` |
| **Complexidade** | MÉDIA |
| **Risco** | BAIXO — Corrige anti-pattern sem alterar interface pública |
| **Dependências** | Executar APÓS tarefa 01 (BinaryFormatter removal) |
| **Arquivos Modificados** | 1 arquivo de produção + 1 arquivo de teste |

## Objetivo

Corrigir 4 instâncias de sync-over-async no `EafSqlServerCache` que bloqueiam threads do pool via `.GetAwaiter().GetResult()`.

## Motivo

- **Thread starvation**: `.GetAwaiter().GetResult()` bloqueia uma thread do thread pool enquanto aguarda I/O
- **Deadlock potencial**: Em contextos com `SynchronizationContext` (ASP.NET), pode causar deadlock
- **Bug existente**: Linha 170 usa `.GetAwaiter()` sem `.GetResult()` — fire-and-forget silencioso, dados podem não ser gravados

## Arquivos Afetados

### Produção

**`src/Eaf.SqlServerCache/Runtime/Caching/SqlServer/EafSqlServerCache.cs`**

#### Símbolos a Modificar

```csharp
// ── BUG CRÍTICO: Set() fire-and-forget (linha 170) ──
// ANTES:
public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
{
    var encodedCurrent = ObjectToByteArray(value);
    _cache.SetAsync(FixKey(key), CompressBytesAsync(encodedCurrent).GetAwaiter().GetResult(),
        new DistributedCacheEntryOptions { ... }).GetAwaiter();
    // ^^^^^ BUG: .GetAwaiter() SEM .GetResult() — Task não é aguardada!
}

// DEPOIS (correção mínima — sync correto):
public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
{
    var encodedCurrent = ObjectToByteArray(value);
    var compressedData = CompressBytesAsync(encodedCurrent).GetAwaiter().GetResult();
    _cache.SetAsync(FixKey(key), compressedData,
        new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpireTime ?? DefaultAbsoluteExpireTime,
            SlidingExpiration = slidingExpireTime ?? DefaultSlidingExpireTime
        }).GetAwaiter().GetResult(); // CORRIGIDO: Agora aguarda resultado
}

// ── TryGetValue (linha 137) — Documentar anti-pattern ──
// ANTES:
public override bool TryGetValue(string key, out object value)
{
    var encodedCached = _cache.GetAsync(FixKey(key)).GetAwaiter().GetResult();
    // ...
    var cached = ByteArrayToObject(DecompressBytesAsync(encodedCached).GetAwaiter().GetResult());
}

// DEPOIS: Manter sync (CacheBase não tem TryGetValueAsync), mas adicionar comentário
// NOTA: CacheBase do ABP não define TryGetValueAsync, portanto sync é necessário aqui.
// A correção ideal requer alteração no ABP framework. Manter com documentação.

// ── Remove (linha 179) — Mesma situação ──
// Manter sync com documentação. CacheBase.Remove() é síncrono por design ABP.

// ── Clear (linha 185) ── 
// Já é no-op, sem problemas.
```

### Teste

**`test/Eaf.SqlServerCache.Tests/EafSqlServerCacheSetBugTests.cs`**

## Cenários de Teste

```csharp
public class EafSqlServerCacheSetBugTests
{
    [Fact]
    public void Dado_ValorValido_Quando_Set_Entao_DeveGravarNoCache()
    // Verifica que Set() realmente persiste o valor (corrige bug fire-and-forget)
    // 1. Criar mock IDistributedCache
    // 2. Chamar Set("key", "value")
    // 3. Verificar que SetAsync foi chamado no mock com .Received()

    [Fact]
    public void Dado_ValorValido_Quando_SetComSlidingExpire_Entao_DeveUsarExpireCorreto()
    // Verifica que opções de expiração são passadas corretamente

    [Fact]
    public void Dado_ValorValido_Quando_SetComAbsoluteExpire_Entao_DeveUsarExpireCorreto()

    [Fact]
    public void Dado_CacheDisponivel_Quando_TryGetValue_Entao_DeveRetornarTrue()
    // Mock IDistributedCache.GetAsync retorna dados válidos → TryGetValue retorna true

    [Fact]
    public void Dado_CacheVazio_Quando_TryGetValue_Entao_DeveRetornarFalse()
    // Mock IDistributedCache.GetAsync retorna null → TryGetValue retorna false

    [Fact]
    public void Dado_CacheComErro_Quando_TryGetValue_Entao_DeveRetornarFalseSemExcecao()
    // Mock IDistributedCache.GetAsync lança exceção → TryGetValue retorna false, não propaga
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.SqlServerCache/Eaf.SqlServerCache.csproj --configuration Release
dotnet test test/Eaf.SqlServerCache.Tests/Eaf.SqlServerCache.Tests.csproj --collect:"XPlat Code Coverage"
```

## Critérios de Aceite

1. Bug fire-and-forget corrigido: `Set()` agora aguarda `SetAsync` com `.GetResult()`
2. Sem deadlock em testes unitários
3. Todos os testes existentes passam
4. Novos testes verificam que dados são efetivamente gravados
5. Cobertura não diminuiu

## Notas para o Sub-Agent

- **Não tentar converter tudo para async** — `CacheBase` do ABP é síncrona por design
- O objetivo é corrigir o BUG (fire-and-forget) e documentar os sync-over-async inevitáveis
- Se precisar de métodos async, o ABP framework teria que fornecer `CacheBase` com suporte async
- Focar na correção do `Set()` que é o bug crítico — dados podem estar sendo perdidos
