# 14 — Correções Menores de Performance

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 4 — Performance API Template |
| **Complexidade** | BAIXA |
| **Risco** | BAIXO — Correções pontuais sem impacto estrutural |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | 4 arquivos de produção |

## Objetivo

Aplicar correções menores de performance em diversos módulos:
1. `NullKeyVaultManager.SetValueAsync` — `Task.Run` desnecessário → `Task.CompletedTask`
2. `UserManager.GetUser` / `GetUserOrNull` — marcar como `[Obsolete]`
3. `MiddlewareAppServiceBase.GetCurrentUser` — marcar como `[Obsolete]`
4. `FriendshipAppService` — adicionar `await` faltante

## 1. NullKeyVaultManager — Task.CompletedTask

**`src/Eaf.KeyVault/KeyVault/NullKeyVaultManager.cs` (linha 82-86)**

```csharp
// ── ANTES ──
public Task SetValueAsync(string key, string value)
{
    logger.Debug("NullKeyVaultManager : NotImplementedException");
    return Task.Run(() => { SetValue(key, value); });
    // ^^^^ Task.Run desnecessário — SetValue é sync e no-op
}

// ── DEPOIS ──
public Task SetValueAsync(string key, string value)
{
    logger.Debug("NullKeyVaultManager : NotImplementedException");
    SetValue(key, value);
    return Task.CompletedTask;
}
```

**Motivo**: `Task.Run` aloca thread do pool para executar um no-op síncrono. `Task.CompletedTask` retorna imediatamente sem alocação.

## 2. UserManager — [Obsolete] Sync Methods

**`src/Eaf.Middleware.Core/Authorization/Users/UserManager.cs` (linhas 93-96, 132-135)**

```csharp
// ── ANTES ──
public User GetUser(UserIdentifier userIdentifier)
{
    return AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));
}

public User GetUserOrNull(UserIdentifier userIdentifier)
{
    return AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));
}

// ── DEPOIS ──
/// <summary>
/// GetUser (síncrono).
/// </summary>
/// <param name="userIdentifier">Parâmetro userIdentifier.</param>
/// <returns>Resultado da operação.</returns>
[Obsolete("Use GetUserAsync instead. Sync-over-async causes thread pool starvation.")]
public User GetUser(UserIdentifier userIdentifier)
{
    return AsyncHelper.RunSync(() => GetUserAsync(userIdentifier));
}

/// <summary>
/// GetUserOrNull (síncrono).
/// </summary>
/// <param name="userIdentifier">Parâmetro userIdentifier.</param>
/// <returns>Resultado da operação.</returns>
[Obsolete("Use GetUserOrNullAsync instead. Sync-over-async causes thread pool starvation.")]
public User GetUserOrNull(UserIdentifier userIdentifier)
{
    return AsyncHelper.RunSync(() => GetUserOrNullAsync(userIdentifier));
}
```

**Motivo**: Não podemos remover estes métodos (breaking change), mas podemos marcá-los como obsoletos para incentivar migração gradual para as versões async.

## 3. MiddlewareAppServiceBase — [Obsolete]

**`src/Eaf.Middleware.Application/MiddlewareAppServiceBase.cs`**

```csharp
// Encontrar o método GetCurrentUser e adicionar:
[Obsolete("Use GetCurrentUserAsync instead. Sync-over-async causes thread pool starvation.")]
protected virtual User GetCurrentUser()
{
    // ... implementação existente mantida
}
```

## 4. FriendshipAppService — Await Faltante

**`src/Eaf.Middleware.Application/Friendships/FriendshipAppService.cs`**

```csharp
// Encontrar chamada async sem await:
// grep -n "Async(" src/Eaf.Middleware.Application/Friendships/FriendshipAppService.cs
// Procurar por: someMethod.SomeAsync(...) sem await

// ANTES:
// someRepository.SomeAsync(param); // SEM AWAIT — fire-and-forget!

// DEPOIS:
// await someRepository.SomeAsync(param);
```

**NOTA**: Verificar compilador warnings CS4014 ("Because this call is not awaited...") para localizar exatamente.

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.KeyVault.Tests/ (existente — verificar que passa)
[Fact]
public async Task Dado_NullKeyVaultManager_Quando_SetValueAsync_Entao_DeveRetornarTaskCompletada()
{
    // Dado
    var manager = new NullKeyVaultManager(options, logger);
    // Quando
    await manager.SetValueAsync("key", "value");
    // Então — não deve lançar exceção
}

// Verificar warning de [Obsolete] nos callers:
// dotnet build Eaf.sln → deve mostrar CS0618 warnings para callers dos métodos obsoletos
```

## Comandos de Verificação

```bash
dotnet build src/Eaf.KeyVault/Eaf.KeyVault.csproj --configuration Release
dotnet build src/Eaf.Middleware.Core/Eaf.Middleware.Core.csproj --configuration Release
dotnet build src/Eaf.Middleware.Application/Eaf.Middleware.Application.csproj --configuration Release
dotnet test Eaf.sln --collect:"XPlat Code Coverage"
```

## Critérios de Aceite

1. `NullKeyVaultManager.SetValueAsync` usa `Task.CompletedTask`
2. `UserManager.GetUser` e `GetUserOrNull` marcados com `[Obsolete]`
3. `MiddlewareAppServiceBase.GetCurrentUser` marcado com `[Obsolete]`
4. FriendshipAppService await corrigido (se encontrado)
5. Todos os testes passam
6. Cobertura não diminuiu

## Notas para o Sub-Agent

- `[Obsolete]` com mensagem em inglês (padrão de código)
- Não remover os métodos obsoletos — apenas marcar
- Se `FriendshipAppService` não tiver await faltante, pular (pode ter sido corrigido)
- Verificar se `NullKeyVaultManagerBddTests` já tem teste para `SetValueAsync` — se sim, verificar que passa
