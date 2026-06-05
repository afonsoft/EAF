# 13 — Aplicar AsNoTracking em Queries Read-Only

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 4 — Performance API Template |
| **Complexidade** | BAIXA |
| **Risco** | BAIXO — Otimização para queries de leitura |
| **Dependências** | Nenhuma |
| **Arquivos Modificados** | Variável (Application Services com queries de leitura) |

## Objetivo

Adicionar `.AsNoTracking()` a queries EF Core que são apenas de leitura (GET endpoints, listagens, lookups).

## Motivo

- **Performance**: `.AsNoTracking()` desabilita change tracker — ~30% mais rápido para leitura
- **Memory**: Sem change tracking, menos objetos em memória
- **Padrão**: ABP `GetAll()` retorna queryable tracked — precisa de opt-out explícito

## Padrão a Aplicar

```csharp
// ── ANTES ──
public async Task<PagedResultDto<AirplaneListDto>> GetAll(GetAirplanesInput input)
{
    var query = _airplaneRepository.GetAll()
        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), a => a.Name.Contains(input.Filter));
    // ... paginação
}

// ── DEPOIS ──
public async Task<PagedResultDto<AirplaneListDto>> GetAll(GetAirplanesInput input)
{
    var query = _airplaneRepository.GetAll()
        .AsNoTracking()
        .WhereIf(!input.Filter.IsNullOrWhiteSpace(), a => a.Name.Contains(input.Filter));
    // ... paginação
}
```

## Onde Aplicar

1. **Métodos `GetAll`** — listagens sempre são read-only
2. **Métodos `Get` por ID** que retornam DTO (não entity para edição)
3. **Lookups e dropdowns** — sempre read-only
4. **Contagens** (`Count`, `LongCount`) — não precisam de tracking

## Onde NÃO Aplicar

1. **Métodos que fazem `Update`/`Delete`** após a query
2. **Métodos que retornam entity para edição** (Create/Update endpoints)
3. **Queries dentro de UnitOfWork que modificam entities**

## Processo de Execução

```bash
# Encontrar candidatos:
grep -rn "GetAll()" Templates/Api/src --include="*.cs" | grep -v "bin/" | grep -v "obj/"
grep -rn "\.GetAllList" Templates/Api/src --include="*.cs" | grep -v "bin/" | grep -v "obj/"
```

## Cenários de Teste

```csharp
// Testes existentes devem continuar passando — AsNoTracking não altera resultado
// Apenas verificar que build compila
```

## Comandos de Verificação

```bash
dotnet build Templates/Api/Eaf.ProjectNameApi.sln --configuration Release
```

## Critérios de Aceite

1. Todas as queries read-only usam `.AsNoTracking()`
2. Nenhuma query de escrita tem `.AsNoTracking()`
3. Build compila sem erros
4. Testes existentes passam

## Notas para o Sub-Agent

- `.AsNoTracking()` deve ser chamado ANTES de qualquer materialização (`.ToList()`, `.FirstOrDefault()`)
- Se um método faz read E write, NÃO adicionar AsNoTracking
- Se não tiver certeza se é read-only, NÃO adicionar — é melhor perder otimização que quebrar
- `_repository.Get(id)` do ABP não aceita `.AsNoTracking()` diretamente — usar `_repository.GetAll().AsNoTracking().FirstOrDefault(x => x.Id == id)`
