using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.AzureActiveDirectory.Authentication;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.MultiTenancy;

namespace Eaf.Middleware.Authorization.AzureActiveDirectory
{
    /// <summary>
    /// Representa a classe AppAzureActiveDirectoryAuthenticationSource.
    /// </summary>
    public class AppAzureActiveDirectoryAuthenticationSource : AzureActiveDirectoryAuthenticationSource<Tenant, User>
    {
        /// <summary>
        /// AppAzureActiveDirectoryAuthenticationSource.
        /// </summary>
        /// <param name="settings">Parâmetro settings.</param>
        /// <param name="azureActiveDirectoryModuleConfig">Parâmetro azureActiveDirectoryModuleConfig.</param>
        /// <returns>Resultado da operação.</returns>
        public AppAzureActiveDirectoryAuthenticationSource(IAzureActiveDirectorySettings settings, IEafMiddlewareAzureActiveDirectoryModuleConfig azureActiveDirectoryModuleConfig)
            : base(settings, azureActiveDirectoryModuleConfig)
        {
        }
    }
}