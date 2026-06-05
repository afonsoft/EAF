# 04 — Converter Delete Individual para Batch Delete no AuditLog Worker

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 1 — Performance `src/` |
| **Complexidade** | MÉDIA |
| **Risco** | BAIXO — Otimização de performance sem alterar comportamento |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | 1 arquivo de produção + 1 arquivo de teste |

## Objetivo

Converter o loop de delete individual (`foreach → DeleteAsync`) no `ExpiredAuditLogDeleterWorker` para batch delete, reduzindo de N round-trips ao banco para 1.

## Motivo

- **Performance**: Cada registro expirado gera um `DELETE` individual ao banco (N round-trips)
- **Lock contention**: Com muitos registros (ex: 10.000 logs), bloqueia o banco durante minutos
- **Sync-over-async**: Usa `AsyncHelper.RunSync(() => DeleteAsync())` dentro do loop — bloqueia thread pool N vezes

## Arquivos Afetados

### Produção

**`src/Eaf.Middleware.Core/Auditing/ExpiredAuditLogDeleterWorker.cs`**

#### Símbolos a Modificar

```csharp
// ── ANTES (linhas 123-145) ──
private void DeleteAuditLogs(DateTime expireDate)
{
    var expiredEntryCount = _auditLogRepository.LongCount(l => l.ExecutionTime < expireDate);
    if (expiredEntryCount == 0) return;

    if (expiredEntryCount > MaxDeletionCount)
    {
        var deleteStartId = _auditLogRepository.GetAll().OrderBy(l => l.Id).Skip(MaxDeletionCount).Select(x => x.Id).First();
        var deleteItens = _auditLogRepository.GetAll().Where(l => l.Id < deleteStartId);
        foreach (var del in deleteItens)
            AsyncHelper.RunSync(() => _auditLogRepository.DeleteAsync(del));
            // ^^^^ N round-trips! Um DELETE por registro
    }
    else
    {
        var deleteItens = _auditLogRepository.GetAll().Where(l => l.ExecutionTime < expireDate);
        foreach (var del in deleteItens)
            AsyncHelper.RunSync(() => _auditLogRepository.DeleteAsync(del));
            // ^^^^ N round-trips! Um DELETE por registro
    }
}

// ── DEPOIS ──
private void DeleteAuditLogs(DateTime expireDate)
{
    var expiredEntryCount = _auditLogRepository.LongCount(l => l.ExecutionTime < expireDate);
    if (expiredEntryCount == 0) return;

    if (expiredEntryCount > MaxDeletionCount)
    {
        // Obter IDs para deletar em batch (limitado a MaxDeletionCount)
        var idsToDelete = _auditLogRepository.GetAll()
            .Where(l => l.ExecutionTime < expireDate)
            .OrderBy(l => l.Id)
            .Take(MaxDeletionCount)
            .Select(l => l.Id)
            .ToList();

        // Batch delete por IDs (único round-trip)
        _auditLogRepository.Delete(l => idsToDelete.Contains(l.Id));
    }
    else
    {
        // Batch delete direto por filtro (único round-trip)
        _auditLogRepository.Delete(l => l.ExecutionTime < expireDate);
    }
}
```

**NOTA**: O método `IRepository<T>.Delete(Expression<Func<T, bool>>)` do ABP suporta delete por expressão, que gera um único `DELETE WHERE` no SQL. Verificar se o repositório suporta essa sobrecarga.

**Alternativa** (se ABP não suportar delete por expressão):
```csharp
// Usar SQL direto via IRepository.GetDbContext()
var context = _auditLogRepository.GetDbContext();
context.Database.ExecuteSqlRaw(
    "DELETE FROM AbpAuditLogs WHERE ExecutionTime < {0}", expireDate);
```

### Teste

**`test/Eaf.Middleware.Core.Tests/Auditing/ExpiredAuditLogDeleterWorkerTests.cs`**

## Cenários de Teste

```csharp
public class ExpiredAuditLogDeleterWorkerDeleteTests
{
    [Fact]
    public void Dado_LogsExpirados_Quando_DeletarEmBatch_Entao_DeveRemoverTodosDeUmaVez()
    // 1. Criar mock _auditLogRepository com 100 logs expirados
    // 2. Chamar DeleteAuditLogs
    // 3. Verificar que Delete(expression) foi chamado UMA vez (não 100 vezes)

    [Fact]
    public void Dado_NenhumLogExpirado_Quando_Deletar_Entao_NaoDeveChamarDelete()
    // LongCount retorna 0 → Delete não é chamado

    [Fact]
    public void Dado_MaisQueMaxDeletionCount_Quando_Deletar_Entao_DeveLimitarQuantidade()
    // expiredEntryCount > MaxDeletionCount → deve deletar apenas MaxDeletionCount registros

    [Fact]
    public void Dado_MenosQueMaxDeletionCount_Quando_Deletar_Entao_DeveDeletarTodos()
    // expiredEntryCount < MaxDeletionCount → deve deletar todos os expirados

    [Fact]
    public void Dado_ErroNoBanco_Quando_Deletar_Entao_NaoDevePropagar()
    // Se Delete lançar exceção, o worker não deve crashar
}
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.Middleware.Core/Eaf.Middleware.Core.csproj --configuration Release
dotnet test test/Eaf.Middleware.Core.Tests/Eaf.Middleware.Core.Tests.csproj --collect:"XPlat Code Coverage"
```

## Critérios de Aceite

1. Zero loops `foreach` com `DeleteAsync` individual
2. Delete usa expressão ou SQL batch (1 round-trip)
3. Respeita `MaxDeletionCount` como limite
4. Todos os testes existentes passam
5. Novos testes verificam batch delete
6. Cobertura não diminuiu

## Notas para o Sub-Agent

- O ABP `IRepository<T>` tem sobrecarga `Delete(Expression<Func<T, bool>>)` — usar se disponível
- Se não estiver disponível, usar `GetDbContext().Database.ExecuteSqlRaw()`
- Verificar que `ExpiredEntityHistoryDeleterWorker` (se existir) tem o mesmo padrão — aplicar lá também
- O método `DoWork()` roda com UnitOfWork — o batch delete será commitado automaticamente
