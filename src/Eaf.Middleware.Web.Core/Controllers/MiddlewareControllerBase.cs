using Abp;
using Abp.Application.Services;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Eaf.Middleware.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Globalization;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Representa a classe MiddlewareControllerBase.
    /// </summary>
    public abstract class MiddlewareControllerBase : AbpController, IApplicationService
    {
        protected MiddlewareControllerBase()
        {
            LocalizationSourceName = MiddlewareAppConsts.LocalizationSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        /// <param name="name">Chave de localização</param>
        /// <returns>Texto localizado</returns>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        /// <param name="name">Chave de localização</param>
        /// <param name="args">Argumentos de formatação</param>
        /// <returns>Texto localizado formatado</returns>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        /// <param name="name">Chave de localização</param>
        /// <param name="culture">Cultura para localização</param>
        /// <returns>Texto localizado</returns>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        protected void SetTenantIdCookie(int? tenantId)
        {
            Response.Cookies.Append(
                "Abp-TenantId",
                tenantId?.ToString(),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(5),
                    Path = "/",
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax
                }
            );
        }
    }
}