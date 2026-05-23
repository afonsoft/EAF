using Abp.Configuration.Startup;

namespace Eaf.Middleware.AzureActiveDirectory.Configuration
{
    /// <summary>
    /// Extension methods for module middleware configurations.
    /// </summary>
    public static class ModuleMiddlewareAzureActiveDirectoryConfigurationExtensions
    {
        /// <summary>
        /// Configures Eaf Middleware AzureActiveDirectory module.
        /// </summary>
        /// <returns></returns>
        public static IEafMiddlewareAzureActiveDirectoryModuleConfig MiddlewareAzureActiveDirectory(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<IEafMiddlewareAzureActiveDirectoryModuleConfig>();
        }
    }
}