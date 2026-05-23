using System.Collections.Generic;
using System.Linq;
using Eaf.Configuration;
using Eaf.Configuration.Startup;

namespace Eaf.ProjectName.EntityHistory
{
    public class EntityHistoryConfigProvider : ICustomConfigProvider
    {
        private readonly IEafStartupConfiguration _eafStartupConfiguration;

        public EntityHistoryConfigProvider(
            IEafStartupConfiguration eafStartupConfiguration
        )
        {
            _eafStartupConfiguration = eafStartupConfiguration;
        }

        public Dictionary<string, object> GetConfig(CustomConfigProviderContext customConfigProviderContext)
        {
            if (!_eafStartupConfiguration.EntityHistory.IsEnabled)
            {
                return new Dictionary<string, object>
                {
                    { "EntityHistory", new { IsEnabled = false }}
                };
            }

            var entityHistoryEnabledEntities = new List<string>();

            foreach (var type in EntityHistoryHelper.TrackedTypes)
            {
                if (_eafStartupConfiguration.EntityHistory.Selectors.Any(s => s.Predicate(type)))
                {
                    entityHistoryEnabledEntities.Add(type.FullName);
                }
            }

            return new Dictionary<string, object>
            {
                { "EntityHistory", new { IsEnabled = true, EnabledEntities = entityHistoryEnabledEntities }}
            };
        }
    }
}
