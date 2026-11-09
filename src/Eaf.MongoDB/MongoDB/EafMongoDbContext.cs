using Abp.Dependency;
using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using MongoDB.Driver;

namespace Eaf.MongoDB
{
    /// <summary>
    /// Implementação de <see cref="IEafMongoDbContext"/> com prefixo de coleção por tenant.
    /// </summary>
    public class EafMongoDbContext : IEafMongoDbContext, ISingletonDependency
    {
        private readonly IMongoDatabase _database;
        private readonly EafMongoDbOptions _options;
        private readonly IAbpSession _session;
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        /// <summary>
        /// Cria o contexto MongoDB.
        /// </summary>
        public EafMongoDbContext(
            IMongoDatabase database,
            EafMongoDbOptions options,
            IAbpSession session,
            IMultiTenancyConfig multiTenancyConfig)
        {
            _database = database;
            _options = options;
            _session = session;
            _multiTenancyConfig = multiTenancyConfig;
        }

        /// <inheritdoc />
        public IMongoCollection<T> GetCollection<T>()
        {
            var name = typeof(T).Name;

            if (_multiTenancyConfig.IsEnabled && _options.UseTenantCollectionPrefix && _session.TenantId.HasValue)
            {
                name = $"t{_session.TenantId.Value}_{name}";
            }

            return _database.GetCollection<T>(name);
        }
    }
}
