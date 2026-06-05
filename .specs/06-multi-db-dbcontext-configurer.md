# 06 — Implementar Switch de Provider no DbContextConfigurer

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 2 — Suporte Multi-Database |
| **Complexidade** | MÉDIA |
| **Risco** | BAIXO — Código aditivo, SQL Server permanece como default |
| **Dependências** | Executar antes de 07 e 08 |
| **Arquivos Modificados** | 1 arquivo de produção |

## Objetivo

Implementar a lógica de switch no `ProjectNameDbContextConfigurer` para que o parâmetro `databaseProvider` (que atualmente é IGNORADO) direcione para SQL Server, PostgreSQL ou MySQL.

## Motivo

- O parâmetro `databaseProvider` é aceito mas ignorado — sempre usa SQL Server (linha 12)
- Já existem TODOs no código indicando que suporte a PostgreSQL/MySQL era planejado
- `Database:Provider` já existe nos `appsettings.json` (Production, Staging, Local)
- Padrão de switch já existe em `HangFireConfigurer.cs` (linhas 67-69)

## Arquivos Afetados

### Produção

**`Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContextConfigurer.cs`**

```csharp
// ── ANTES (arquivo completo, 22 linhas) ──
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    public static class ProjectNameDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<ProjectNameDbContext> builder, string connectionString, string databaseProvider = "sqlserver")
        {
            // TODO: Add PostgreSQL support
            // TODO: Add MySQL support
            builder.UseSqlServer(connectionString); // IGNORA databaseProvider!
        }

        public static void Configure(DbContextOptionsBuilder<ProjectNameDbContext> builder, DbConnection connection, string databaseProvider = "sqlserver")
        {
            // TODO: Add PostgreSQL support
            // TODO: Add MySQL support
            builder.UseSqlServer(connection); // IGNORA databaseProvider!
        }
    }
}

// ── DEPOIS ──
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace Eaf.ProjectName.EntityFrameworkCore
{
    /// <summary>
    /// Configures the database provider for the application DbContext.
    /// Supported providers: SqlServer (default), PostgreSQL, MySQL.
    /// </summary>
    public static class ProjectNameDbContextConfigurer
    {
        /// <summary>
        /// Configures the DbContext with the specified database provider using a connection string.
        /// </summary>
        /// <param name="builder">The DbContext options builder.</param>
        /// <param name="connectionString">The database connection string.</param>
        /// <param name="databaseProvider">
        /// The database provider name. Supported values:
        /// "SqlServer" or "MSSQL" (default),
        /// "PostgreSQL", "Postgres", or "Npgsql",
        /// "MySQL", "MariaDB", or "Pomelo".
        /// </param>
        public static void Configure(
            DbContextOptionsBuilder<ProjectNameDbContext> builder,
            string connectionString,
            string databaseProvider = "SqlServer")
        {
            switch (databaseProvider?.ToUpperInvariant())
            {
                case "POSTGRESQL":
                case "POSTGRES":
                case "NPGSQL":
                    builder.UseNpgsql(connectionString);
                    break;

                case "MYSQL":
                case "MARIADB":
                case "POMELO":
                    builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                    break;

                case "SQLSERVER":
                case "MSSQL":
                case null:
                default:
                    builder.UseSqlServer(connectionString);
                    break;
            }
        }

        /// <summary>
        /// Configures the DbContext with the specified database provider using an existing connection.
        /// </summary>
        /// <param name="builder">The DbContext options builder.</param>
        /// <param name="connection">The existing database connection.</param>
        /// <param name="databaseProvider">The database provider name (see overload for values).</param>
        public static void Configure(
            DbContextOptionsBuilder<ProjectNameDbContext> builder,
            DbConnection connection,
            string databaseProvider = "SqlServer")
        {
            switch (databaseProvider?.ToUpperInvariant())
            {
                case "POSTGRESQL":
                case "POSTGRES":
                case "NPGSQL":
                    builder.UseNpgsql(connection);
                    break;

                case "MYSQL":
                case "MARIADB":
                case "POMELO":
                    builder.UseMySql(connection, ServerVersion.AutoDetect(connection.ConnectionString));
                    break;

                case "SQLSERVER":
                case "MSSQL":
                case null:
                default:
                    builder.UseSqlServer(connection);
                    break;
            }
        }
    }
}
```

### Teste

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.ProjectName.EntityFrameworkCore.Tests/ProjectNameDbContextConfigurerTests.cs

public class ProjectNameDbContextConfigurerTests
{
    [Theory]
    [InlineData("SqlServer")]
    [InlineData("MSSQL")]
    [InlineData("sqlserver")]
    [InlineData(null)]
    [InlineData("")]
    public void Dado_ProviderSqlServer_Quando_Configurar_Entao_DeveUsarSqlServer(string provider)
    // Verificar que builder.UseSqlServer foi chamado

    [Theory]
    [InlineData("PostgreSQL")]
    [InlineData("Postgres")]
    [InlineData("Npgsql")]
    [InlineData("postgresql")]
    public void Dado_ProviderPostgreSQL_Quando_Configurar_Entao_DeveUsarNpgsql(string provider)
    // Verificar que builder.UseNpgsql foi chamado

    [Theory]
    [InlineData("MySQL")]
    [InlineData("MariaDB")]
    [InlineData("Pomelo")]
    [InlineData("mysql")]
    public void Dado_ProviderMySQL_Quando_Configurar_Entao_DeveUsarMySql(string provider)
    // Verificar que builder.UseMySql foi chamado

    [Fact]
    public void Dado_ProviderDesconhecido_Quando_Configurar_Entao_DeveUsarSqlServerComoPadrao()
    // "OracleDB" → deve usar SQL Server (default)

    [Theory]
    [InlineData("SqlServer")]
    [InlineData("PostgreSQL")]
    [InlineData("MySQL")]
    public void Dado_DbConnection_Quando_ConfigurarComProvider_Entao_DeveUsarProviderCorreto(string provider)
    // Testar a segunda sobrecarga com DbConnection
}
```

## Comandos de Verificação

```bash
dotnet build Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/Eaf.ProjectName.EntityFrameworkCore.csproj --configuration Release
```

## Critérios de Aceite

1. Ambas as sobrecargas de `Configure` respeitam o parâmetro `databaseProvider`
2. SQL Server permanece como default (backward compatible)
3. Case-insensitive para nomes de provider
4. XML docs completos em todas as APIs públicas
5. Todos os testes passam

## Notas para o Sub-Agent

- **NÃO** alterar a assinatura dos métodos — manter o default parameter `"SqlServer"`
- `ServerVersion.AutoDetect()` do Pomelo detecta a versão do MySQL automaticamente
- Se `Pomelo.EntityFrameworkCore.MySql` não estiver disponível no .csproj, será adicionado na tarefa 07
- Verificar que imports `using Npgsql.EntityFrameworkCore.PostgreSQL` e `using Pomelo.EntityFrameworkCore.MySql` estão corretos
