using Abp;
using Abp.Domain.Entities.Auditing;
using Abp.EntityHistory;
using System.Linq;

namespace Eaf.Middleware.Auditing
{
    /// <summary>
    /// Representa a classe EntityHistoryConfigurationExtensions.
    /// </summary>
    public static class EntityHistoryConfigurationExtensions
    {
        public const string AllEntitiesSelectorName = "Eaf.Audited.All";

        /// <summary>
        /// AddAllAuditedEntities.
        /// </summary>
        /// <param name="entityHistoryConfiguration">Parâmetro entityHistoryConfiguration.</param>
        public static void AddAllAuditedEntities(this IEntityHistoryConfiguration entityHistoryConfiguration)
        {
            if (entityHistoryConfiguration.IsEnabled)
            {
                if (entityHistoryConfiguration.Selectors.Any(s => s.Name == AllEntitiesSelectorName))
                    return;

                entityHistoryConfiguration.Selectors.Add(new NamedTypeSelector(AllEntitiesSelectorName, t => typeof(IAudited).IsAssignableFrom(t)));
            }
        }
    }
}