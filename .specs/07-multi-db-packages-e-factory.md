# 07 — Adicionar NuGet Packages e Atualizar DbContextFactory

## Metadados

| Campo | Valor |
|-------|-------|
| **Fase** | 2 — Suporte Multi-Database |
| **Complexidade** | BAIXA |
| **Risco** | BAIXO — Adição de packages e leitura de config |
| **Dependências** | Executar JUNTO com ou APÓS tarefa 06 |
| **Arquivos Modificados** | 2 arquivos |

## Objetivo

1. Adicionar packages NuGet para PostgreSQL e MySQL ao projeto EntityFrameworkCore
2. Atualizar `ProjectNameDbContextFactory` para ler `Database:Provider` da configuração

## Motivo

- Sem os packages NuGet, `UseNpgsql()` e `UseMySql()` da tarefa 06 não compilam
- `ProjectNameDbContextFactory` é usado pelo `dotnet ef` CLI — precisa ler o provider da config

## Arquivos Afetados

### 1. Adicionar NuGet Packages

**`Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/Eaf.ProjectName.EntityFrameworkCore.csproj`**

```xml
<!-- ── ANTES ──
Deve conter algo como:
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="..." />
-->

<!-- ── DEPOIS: Adicionar após o SqlServer ── -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="10.0.1" />
<PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="9.0.1" />

<!-- NOTA sobre Pomelo:
   - v10.0.0-rc.2 existe mas NÃO é estável
   - v9.0.1 é a última estável, mas é para EF Core 9
   - Se build falhar com Pomelo 9.0.1 no EF Core 10, usar Condition:
     <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="10.0.0-rc.2" Condition="..." />
   - Alternativa: omitir MySQL e documentar que será adicionado quando Pomelo 10 estável sair
-->
```

### 2. Atualizar DbContextFactory

**`Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/EntityFrameworkCore/ProjectNameDbContextFactory.cs`**

```csharp
// ── ANTES (linhas 18-25) ──
public ProjectNameDbContext CreateDbContext(string[] args)
{
    var builder = new DbContextOptionsBuilder<ProjectNameDbContext>();
    var configuration = GetConfigurationRoot();
    ProjectNameDbContextConfigurer.Configure(builder, configuration.GetConnectionString(ProjectNameConsts.ConnectionStringName));
    // ^^^^ NÃO passa databaseProvider!
    return new ProjectNameDbContext(builder.Options);
}

// ── DEPOIS ──
public ProjectNameDbContext CreateDbContext(string[] args)
{
    var builder = new DbContextOptionsBuilder<ProjectNameDbContext>();
    var configuration = GetConfigurationRoot();
    var databaseProvider = configuration["Database:Provider"] ?? "SqlServer";
    ProjectNameDbContextConfigurer.Configure(
        builder,
        configuration.GetConnectionString(ProjectNameConsts.ConnectionStringName),
        databaseProvider);
    return new ProjectNameDbContext(builder.Options);
}
```

### Teste

## Cenários de Teste

```csharp
// ARQUIVO: test/Eaf.ProjectName.EntityFrameworkCore.Tests/ProjectNameDbContextFactoryTests.cs

public class ProjectNameDbContextFactoryTests
{
    [Fact]
    public void Dado_ConfiguracaoValida_Quando_CriarDbContext_Entao_NaoDeveLancarExcecao()
    // Verificar que CreateDbContext não lança exceção

    [Fact]
    public void Dado_ProviderNaoConfigurado_Quando_CriarDbContext_Entao_DeveUsarSqlServer()
    // Quando Database:Provider não está no config → default "SqlServer"
}
```

## Comandos de Verificação

```bash
# Restaurar packages
dotnet restore Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/Eaf.ProjectName.EntityFrameworkCore.csproj

# Build
dotnet build Templates/Api/src/Eaf.ProjectName.EntityFrameworkCore/Eaf.ProjectName.EntityFrameworkCore.csproj --configuration Release
```

## Critérios de Aceite

1. Packages `Npgsql.EntityFrameworkCore.PostgreSQL` e `Pomelo.EntityFrameworkCore.MySql` adicionados
2. `ProjectNameDbContextFactory.CreateDbContext` lê `Database:Provider` da config
3. Default "SqlServer" mantido para backward compatibility
4. Build compila sem erros
5. Se Pomelo 9.0.x não for compatível com EF Core 10, documentar e omitir MySQL

## Notas para o Sub-Agent

- Verificar versão exata do `Microsoft.EntityFrameworkCore.SqlServer` no `.csproj` existente
- Alinhar versão do Npgsql com a versão do EF Core (10.0.x → Npgsql 10.0.x)
- Se Pomelo causar conflito de versão, é aceitável comentar e adicionar TODO
- O `GetConfigurationRoot()` já existe e retorna `IConfigurationRoot` — apenas usar `["Database:Provider"]`
- `ProjectNameConsts.ConnectionStringName` é "Default" — não alterar
