using Microsoft.Extensions.Options;
using System;

namespace Eaf.MongoDB
{
    /// <summary>
    /// Opções do módulo Eaf.MongoDB.
    /// </summary>
    [Serializable]
    public class EafMongoDbOptions : IOptions<EafMongoDbOptions>
    {
        /// <summary>Connection string do MongoDB.</summary>
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";

        /// <summary>Nome do banco de dados padrão.</summary>
        public string DatabaseName { get; set; } = "Eaf";

        /// <summary>
        /// Quando true e multi-tenancy habilitado, coleções de tenant usam o prefixo
        /// <c>t{TenantId}_</c> (ex.: <c>t3_Orders</c>).
        /// </summary>
        public bool UseTenantCollectionPrefix { get; set; } = true;

        /// <summary>Obtém o valor das opções.</summary>
        public EafMongoDbOptions Value => this;
    }
}
