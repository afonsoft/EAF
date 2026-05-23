using Abp.Dependency;
using Eaf.Middleware.Url;
using Abp.MultiTenancy;
using System;
using Abp.Extensions;

namespace Eaf.Middleware.Web.Url
{
    /// <summary>
    /// Representa a classe AppUrlServiceBase.
    /// </summary>
    public abstract class AppUrlServiceBase : IAppUrlService, ITransientDependency
    {
        protected readonly ITenantCache TenantCache;
        protected readonly IWebUrlService WebUrlService;

        protected AppUrlServiceBase(IWebUrlService webUrlService, ITenantCache tenantCache)
        {
            WebUrlService = webUrlService;
            TenantCache = tenantCache;
        }

        /// <summary>
        /// Obtém ou define EmailActivationRoute.
        /// </summary>
        public abstract string EmailActivationRoute { get; }

        /// <summary>
        /// Obtém ou define PasswordResetRoute.
        /// </summary>
        public abstract string PasswordResetRoute { get; }

        /// <summary>
        /// CreateEmailActivationUrlFormat.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreateEmailActivationUrlFormat(int? tenantId)
        {
            return CreateEmailActivationUrlFormat(GetTenancyName(tenantId));
        }

        /// <summary>
        /// CreateEmailActivationUrlFormat.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreateEmailActivationUrlFormat(string tenancyName)
        {
            var activationLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + EmailActivationRoute + "?userId={userId}&confirmationCode={confirmationCode}";

            if (tenancyName != null)
            {
                activationLink += "&tenantId={tenantId}";
            }

            activationLink += "&authenticationSource={authenticationSource}";

            return activationLink;
        }

        /// <summary>
        /// CreatePasswordResetUrlFormat.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreatePasswordResetUrlFormat(int? tenantId)
        {
            return CreatePasswordResetUrlFormat(GetTenancyName(tenantId));
        }

        /// <summary>
        /// CreatePasswordResetUrlFormat.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public string CreatePasswordResetUrlFormat(string tenancyName)
        {
            var resetLink = WebUrlService.GetSiteRootAddress(tenancyName).EnsureEndsWith('/') + PasswordResetRoute + "?userId={userId}&resetCode={resetCode}";

            if (tenancyName != null)
            {
                resetLink += "&tenantId={tenantId}";
            }

            resetLink += "&authenticationSource={authenticationSource}";

            return resetLink;
        }

        private string GetTenancyName(int? tenantId)
        {
            return tenantId.HasValue ? TenantCache.Get(tenantId.Value).TenancyName : null;
        }
    }
}