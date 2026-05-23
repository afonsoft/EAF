using Eaf.Middleware.Url;
using Abp.MultiTenancy;

namespace Eaf.Middleware.Web.Url
{
    /// <summary>
    /// Representa a classe AngularAppUrlService.
    /// </summary>
    public class AngularAppUrlService : AppUrlServiceBase
    {
        /// <summary>
        /// AngularAppUrlService.
        /// </summary>
        /// <param name="webUrlService">Parâmetro webUrlService.</param>
        /// <param name="tenantCache">Parâmetro tenantCache.</param>
        /// <returns>Resultado da operação.</returns>
        public AngularAppUrlService(
                IWebUrlService webUrlService,
                ITenantCache tenantCache
            ) : base(
                webUrlService,
                tenantCache
            )
        {
        }

        public override string EmailActivationRoute => "account/confirm-email";

        public override string PasswordResetRoute => "account/reset-password";
    }
}