# Eaf.Dapper

Integração Dapper com o Unit of Work do ABP: reutiliza conexão e transação do `DbContext` corrente.

## Uso

```csharp
[DependsOn(typeof(EafDapperModule))]
public class MyModule : AbpModule { }
```

`IDapperConnectionProvider.GetConnection(typeof(MyDbContext))` retorna a conexão do UoW ativo (exige UoW — caso contrário lança `UserFriendlyException`).

Repositórios baseados em `DapperRepository<TDbContext>` (do `Abp.Dapper`) fornecem `QueryAsync`, `ExecuteAsync`, `ExecuteScalarAsync` e `GetActiveTransaction()` na mesma transação do UoW.
