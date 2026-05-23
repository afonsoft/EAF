using Abp.Configuration.Startup;

namespace Eaf.Middleware.Ldap.Configuration
{
    /// <summary>
    /// Extension methods for module middleware configurations.
    /// </summary>
    public static class ModuleMiddlewareLdapConfigurationExtensions
    {
        /// <summary>
        /// Configures Eaf Middleware LDAP module.
        /// </summary>
        /// <returns></returns>
        public static IEafMiddlewareLdapModuleConfig MiddlewareLdap(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<IEafMiddlewareLdapModuleConfig>();
        }
    }
}