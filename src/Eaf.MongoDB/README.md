# Eaf.MongoDB

Contexto MongoDB com isolamento por tenant (prefixo `t{TenantId}_` nas coleções) sobre `IMongoClient`/`IMongoDatabase` do DI.

## Uso

```csharp
[DependsOn(typeof(EafMongoDbModule))]
public class MyModule : AbpModule { }
```

Configure `EafMongoDbOptions` (`ConnectionString`, `DatabaseName`, `UseTenantCollectionPrefix`).

`IEafMongoDbContext.GetCollection<T>()` retorna `IMongoCollection<T>` — com prefixo por tenant quando multi-tenancy habilitado. Repositórios podem usar `MongoDbRepository<TMongoDbContext,TEntity>` do `Abp.MongoDB` para CRUD/IQueryable.
