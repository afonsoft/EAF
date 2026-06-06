using Abp.Dependency;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders;

namespace Eaf.Middleware.Web.Configuration
{
    /// <summary>
    /// Configura provedores de autenticação externa (Google, Microsoft, AuthZero, OpenIdConnect).
    /// </summary>
    internal static class ExternalAuthConfigurer
    {
        /// <summary>
        /// Registra todos os provedores de autenticação externa disponíveis.
        /// </summary>
        /// <param name="iocManager">Gerenciador de IoC.</param>
        public static void Configure(IIocManager iocManager)
        {
            var externalAuthConfiguration = iocManager.Resolve<ExternalAuthConfiguration>();
            if (externalAuthConfiguration != null)
            {
                externalAuthConfiguration.ExternalLoginInfoProviders.Add(iocManager.Resolve<TenantBasedOpenIdConnectExternalLoginInfoProvider>());
                externalAuthConfiguration.ExternalLoginInfoProviders.Add(iocManager.Resolve<TenantBasedGoogleExternalLoginInfoProvider>());
                externalAuthConfiguration.ExternalLoginInfoProviders.Add(iocManager.Resolve<TenantBasedMicrosoftExternalLoginInfoProvider>());
                externalAuthConfiguration.ExternalLoginInfoProviders.Add(iocManager.Resolve<TenantBasedAuthZeroExternalLoginInfoProvider>());
            }
        }
    }
}
