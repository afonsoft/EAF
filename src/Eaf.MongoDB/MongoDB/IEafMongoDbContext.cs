using MongoDB.Driver;

namespace Eaf.MongoDB
{
    /// <summary>
    /// Contexto MongoDB do EAF: resolve coleções aplicando isolamento por tenant.
    /// </summary>
    public interface IEafMongoDbContext
    {
        /// <summary>
        /// Retorna a coleção de <typeparamref name="T"/> — com prefixo <c>t{TenantId}_</c>
        /// quando multi-tenancy está habilitado e <see cref="EafMongoDbOptions.UseTenantCollectionPrefix"/> é true.
        /// </summary>
        IMongoCollection<T> GetCollection<T>();
    }
}
