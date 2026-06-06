# Spec: Corrigir Erros do Template API e Aumentar Cobertura

> **Referência**: [API Template Tests](https://github.com/afonsoft/EAF#api-template-tests) | [DeepWiki](https://deepwiki.com/afonsoft/EAF) | [SonarCloud](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main)

---

## Problema

O template API (`Templates/Api/`) possui **212 testes**, dos quais **142 falham** com o mesmo erro:

```
System.InvalidOperationException: An attempt was made to use the context instance
while it is being configured. A DbContext instance cannot be used inside 'OnConfiguring'
since it is still being configured at this point.
```

### Causa Raiz

Em `ProjectNameDbContext.OnConfiguring()` (linha 54), a chamada `Database.IsSqlServer()` tenta acessar `DbContext.ContextServices`, que ainda não está inicializado durante a fase de configuração. Isso é uma violação do ciclo de vida do EF Core 10.

```csharp
// ❌ QUEBRA em EF Core 10 com InMemory provider
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    if (Database.IsSqlServer())  // ← ERRO: Database não acessível aqui
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
    }
}
```

### Fluxo do Erro

```
Test → ProjectNameTestBase..ctor()
  → UsingDbContext() → Resolve<ProjectNameDbContext>()
    → EF Core chama OnConfiguring()
      → Database.IsSqlServer() → get_ContextServices() → BOOM 💥
```

---

## Solução Proposta

### 1. Fix `OnConfiguring` — Usar `optionsBuilder.Options.Extensions`

```csharp
// ✅ CORRETO: Verificar provider via Extensions do builder
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    var isSqlServer = optionsBuilder.Options.Extensions
        .Any(e => e.GetType().FullName?.Contains("SqlServer") == true);

    if (isSqlServer)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
    }
    optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    base.OnConfiguring(optionsBuilder);
}
```

### 2. Fix `OnModelCreating` — Mesmo padrão para consistência

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    // ... entity configs ...

    // Usar optionsBuilder não disponível aqui, mas Database está OK em OnModelCreating
    // porque o contexto já foi configurado. Manter Database.IsSqlServer() aqui.
    if (Database.IsSqlServer()) { /* configurar colunas SQL Server */ }
    else if (Database.IsNpgsql()) { /* configurar colunas PostgreSQL */ }
}
```

> **Nota**: `Database` é acessível em `OnModelCreating` porque o contexto já está configurado nesse ponto.

### 3. Proteger o Construtor — `SkipMigrate` nos testes

Adicionar `ProjectNameDbContext.SkipMigrate = true` no `ProjectNameTestModule.PreInitialize()` para evitar chamadas a `Database.Migrate()` com InMemory.

---

## Módulos para Aumentar Cobertura

| Módulo | Cobertura Atual | Meta | Estratégia |
|--------|----------------|------|-----------|
| **Eaf.Log4NetServiceBus** | 62.2% | 80%+ | Testar `ServiceBusQueueAppender` com mocks de `ITopicClient` |
| **Eaf.Middleware.Worker** | 33.3% | 60%+ | Testar lifecycle hooks, `EafWorkerBase`, email builders |
| **Eaf.Middleware.Application** | 23.6% | 50%+ | Testar DTOs, AppServices com mocks de repositórios |
| **Eaf.Middleware.AzureActiveDirectory** | 7.4% | 40%+ | Testar `AzureActiveDirectoryAuthenticationSource` com mock de `IGraphClient` |
| **Eaf.Middleware.Ldap** | 6.0% | 40%+ | Testar `LdapAuthenticationSource` com mock de `DirectoryEntry` |
| **Eaf.Middleware.Web.Core** | 4.9% | 30%+ | Testar controllers, filtros, middleware components |
| **Eaf.Middleware.Core** | 0.1% | 20%+ | Testar entidades, serviços de domínio, configurações |

### Estratégias por Módulo

#### Eaf.Log4NetServiceBus (62.2% → 80%)
- Testar serialização/deserialização de `LogMessage`
- Testar fallback quando Azure Service Bus não disponível
- Testar formatação de mensagens com diferentes níveis de log

#### Eaf.Middleware.Worker (33.3% → 60%)
- Testar `MiddlewareSmtpEmailSenderConfiguration` com diferentes settings
- Testar `MiddlewareMailKitSmtpBuilder` com configurações SSL/TLS
- Testar virtual file system e `PathUtils`
- Testar lifecycle do módulo Worker

#### Eaf.Middleware.Application (23.6% → 50%)
- Testar DTOs de Authorization (Roles, Users, Permissions)
- Testar validações de input/output
- Testar AppServices com NSubstitute para repositórios
- Testar mappers e conversões

#### Eaf.Middleware.AzureActiveDirectory (7.4% → 40%)
- Testar configuração de Azure AD (client ID, tenant, etc.)
- Testar resolução de claims
- Testar cenários de falha de autenticação

#### Eaf.Middleware.Ldap (6.0% → 40%)
- Testar `LdapSettings` com valores válidos e inválidos
- Testar resolução de grupos/usuários
- Testar cenários de conexão falha (timeout, credentials inválidas)

#### Eaf.Middleware.Web.Core (4.9% → 30%)
- Testar Swagger filters (tag descriptions, operation filters)
- Testar `TokenAuthController` com mocks de identity
- Testar middleware de impersonação
- Testar configuração de startup

#### Eaf.Middleware.Core (0.1% → 20%)
- Testar entidades base (User, Role, Tenant)
- Testar configurações e settings
- Testar serviços de domínio com mocks
- Testar extensões e helpers

---

## Pré-requisitos

- .NET 10.0 SDK
- Pacote `Microsoft.EntityFrameworkCore.InMemory`
- `xUnit`, `Shouldly`, `NSubstitute`

## Verificação

```bash
# Após o fix, todos os 212 testes devem passar:
dotnet test Templates/Api/test/Eaf.ProjectName.Tests/Eaf.ProjectName.Tests.csproj
dotnet test Templates/Api/test/Eaf.ProjectName.Web.Tests/Eaf.ProjectName.Web.Tests.csproj

# Cobertura dos módulos middleware:
dotnet test Eaf.sln --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

## Referências

- [EF Core — OnConfiguring lifecycle](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/#onconfiguring)
- [DeepWiki EAF](https://deepwiki.com/afonsoft/EAF)
- [SonarCloud EAF](https://sonarcloud.io/summary/overall?id=afonsoft_EAF2&branch=main)
