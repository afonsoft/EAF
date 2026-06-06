using Abp.Configuration.Startup;

namespace Eaf.Middleware.Web.Configuration
{
    /// <summary>
    /// Configura auditoria e histórico de entidades.
    /// </summary>
    internal static class AuditConfigurer
    {
        /// <summary>
        /// Configura as opções de auditoria e histórico de entidades.
        /// </summary>
        /// <param name="configuration">Configuração de startup do ABP.</param>
        public static void Configure(IAbpStartupConfiguration configuration)
        {
            configuration.Auditing.IsEnabledForAnonymousUsers = false;
            configuration.Auditing.IsEnabled = true;
            configuration.EntityHistory.IsEnabled = true;
            configuration.EntityHistory.IsEnabledForAnonymousUsers = true;
            configuration.EntityHistory.AddAllAuditedEntities();
        }
    }
}
