# 08 — Corrigir DbContext (Warnings, Migrate) e Appsettings

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 2 — Suporte Multi-Database |
| **Complexidade** | MÉDIA |
| **Risco** | MÉDIO — Remover Database.Migrate() altera comportamento de startup |
| **Dependências** | Executar APÓS tarefas 06 e 07 |
| **Arquivos Modificados** | 2 arquivos de produção |

## Objetivo

1. Tornar warnings SQL Server-specific condicionais (não aplicar para PostgreSQL/MySQL)
2. Remover `Database.Migrate()` do construtor do `DbContext` (anti-pattern)
3. Atualizar `appsettings.json` com documentação de providers

## Motivo

- `SqlServerEventId.SavepointsDisabledBecauseOfMARS` não existe para PostgreSQL/MySQL — lança exceção
- `Database.Migrate()` no construtor bloqueia startup e é anti-pattern (deve ser feito no `Program.cs`)
- `appsettings.json` tem flags legacy (`IsOracleEnabled`, `IsMySqlEnabled`) em vez de `Database:Provider`

## Arquivos Afetados

### 1. ProjectNameDbContext

**`Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContext.cs`**

```csharp
// ── MODIFICAR OnConfiguring (linha 52-57) ──
// ANTES:
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
    // ^^^^ SqlServerEventId só é válido para SQL Server!
    optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    base.OnConfiguring(optionsBuilder);
}

// DEPOIS:
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (Database.IsSqlServer())
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
    }
    optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    base.OnConfiguring(optionsBuilder);
}

// NOTA: Database.IsSqlServer() é extension method de
// Microsoft.EntityFrameworkCore.SqlServer — já está referenciado.

// ── MODIFICAR Construtor (linhas 22-41) ──
// ANTES:
public ProjectNameDbContext(DbContextOptions<ProjectNameDbContext> options) : base(options)
{
    if (!_created)
    {
        try
        {
            _created = true;
            if (!SkipMigrate)
            {
                Logger.Trace("Database Migrate started...");
                Database.Migrate(); // BLOQUEIA STARTUP!
            }
        }
        catch (Exception ex)
        {
            _created = false;
            Logger.Warn("Database Migrate started Error ...", ex);
        }
    }
}

// DEPOIS:
public ProjectNameDbContext(DbContextOptions<ProjectNameDbContext> options) : base(options)
{
    if (!_created)
    {
        try
        {
            _created = true;
            if (!SkipMigrate)
            {
                Logger.Trace("Database Migrate started...");
                Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            _created = false;
            Logger.Warn("Database Migrate started Error ...", ex);
        }
    }
}
// NOTA: Manter Database.Migrate() por enquanto — é o comportamento atual do template.
// Remover quebraria projetos existentes que dependem da auto-migration.
// Adicionar comentário XML documentando que o ideal é migrar via CLI:
//   dotnet ef database update
// A remoção será feita em uma versão futura com breaking change documentado.

// ── MODIFICAR OnModelCreating (linha 98) ──
// ANTES:
modelBuilder.Entity<Abp.Auditing.AuditLog>(b =>
{
    b.Property(e => e.Parameters).HasColumnType("nvarchar(max)");
    // ^^^^ nvarchar(max) é SQL Server-specific!
});

// DEPOIS: Tornar condicional
if (Database.IsSqlServer())
{
    modelBuilder.Entity<Abp.Auditing.AuditLog>(b =>
    {
        b.Property(e => e.Parameters).HasColumnType("nvarchar(max)");
    });
}
else if (Database.IsNpgsql())
{
    modelBuilder.Entity<Abp.Auditing.AuditLog>(b =>
    {
        b.Property(e => e.Parameters).HasColumnType("text");
    });
}
// MySQL: text é o padrão, não precisa de configuração específica
```

### 2. Appsettings

**`Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/appsettings.json`** (e variantes)

```json
// ── ANTES (se existir) ──
{
  "Database": {
    "IsOracleEnabled": "false",
    "IsMySqlEnabled": "false"
  }
}

// ── DEPOIS ──
{
  "Database": {
    "Provider": "SqlServer"
  }
}

// Adicionar comentários de exemplo nos appsettings de cada ambiente:
// SqlServer: "Server=localhost;Database=ProjectNameDb;Trusted_Connection=True;"
// PostgreSQL: "Host=localhost;Database=ProjectNameDb;Username=postgres;Password=...;"
// MySQL: "Server=localhost;Database=ProjectNameDb;User=root;Password=...;"
```

### Teste

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.ProjectName.EntityFrameworkCore.Tests/ProjectNameDbContextTests.cs

public class ProjectNameDbContextTests
{
    [Fact]
    public void Dado_ProviderSqlServer_Quando_OnConfiguring_Entao_DeveIgnorarSavepointWarning()
    // Usar InMemory ou SqlServer provider → verificar que warning é ignorado

    [Fact]
    public void Dado_ProviderNaoSqlServer_Quando_OnConfiguring_Entao_NaoDeveIgnorarSavepointWarning()
    // Usar Npgsql/InMemory → verificar que SqlServerEventId NÃO é ignorado

    [Fact]
    public void Dado_ProviderSqlServer_Quando_OnModelCreating_Entao_DeveUsarNvarcharMax()
    // Verificar HasColumnType("nvarchar(max)") para AuditLog.Parameters

    [Fact]
    public void Dado_ProviderPostgreSQL_Quando_OnModelCreating_Entao_DeveUsarText()
    // Verificar HasColumnType("text") para AuditLog.Parameters
}
```

## Comandos de Verificação

```bash
dotnet build Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/Eaf.ProjectName.EntityFrameworkCore.csproj --configuration Release
dotnet build Eaf.sln --configuration Release
```

## Critérios de Aceite

1. `SqlServerEventId` warnings só aplicados quando provider é SQL Server
2. `nvarchar(max)` condicional por provider
3. `appsettings.json` usa `Database:Provider` em vez de flags legacy
4. Todos os testes passam
5. Build sem erros com todos os providers

## Notas para o Sub-Agent

- `Database.IsSqlServer()` e `Database.IsNpgsql()` são extension methods disponíveis quando os packages estão referenciados
- Se `Database.IsSqlServer()` não funcionar em `OnConfiguring` (antes do provider ser definido), usar `optionsBuilder.Options.Extensions.OfType<SqlServerOptionsExtension>().Any()` como alternativa
- Manter `Database.Migrate()` no construtor (decisão documentada — remover seria breaking change)
- As migrações existentes são SQL Server-specific — documentar que novos providers precisam de novas migrações
- Se o sub-agent encontrar dificuldade com o acesso a `Database` no `OnConfiguring`, reportar
