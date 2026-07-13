using Abp.Collections.Extensions;
using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Web.Url
{
    /// <summary>
    /// Representa a classe WebUrlServiceBase.
    /// </summary>
    public abstract class WebUrlServiceBase
    {
        public const string TenancyNamePlaceHolder = "{TENANCY_NAME}";

        private readonly IConfigurationRoot _appConfiguration;

        /// <summary>
        /// WebUrlServiceBase.
        /// </summary>
        /// <param name="configurationAccessor">Parâmetro configurationAccessor.</param>
        /// <returns>Resultado da operação.</returns>
        protected WebUrlServiceBase(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }

        public string ServerRootAddressFormat => _appConfiguration[ServerRootAddressFormatKey] ?? "http://localhost:8001/";
        /// <summary>
        /// Obtém ou define ServerRootAddressFormatKey.
        /// </summary>
        public abstract string ServerRootAddressFormatKey { get; }

        public bool SupportsTenancyNameInUrl
        {
            get
            {
                var siteRootFormat = WebSiteRootAddressFormat;
                return !siteRootFormat.IsNullOrEmpty() && siteRootFormat.Contains(TenancyNamePlaceHolder);
            }
        }

        public string WebSiteRootAddressFormat => _appConfiguration[WebSiteRootAddressFormatKey] ?? "http://localhost:8000/";
        /// <summary>
        /// Obtém ou define WebSiteRootAddressFormatKey.
        /// </summary>
        public abstract string WebSiteRootAddressFormatKey { get; }

        /// <summary>
        /// GetRedirectAllowedExternalWebSites.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public List<string> GetRedirectAllowedExternalWebSites()
        {
            var values = _appConfiguration["App:RedirectAllowedExternalWebSites"];
            return values?.Split(',').ToList() ?? new List<string>();
        }

        /// <summary>
        /// GetServerRootAddress.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public string GetServerRootAddress(string tenancyName = null)
        {
            return ReplaceTenancyNameInUrl(ServerRootAddressFormat, tenancyName);
        }

        /// <summary>
        /// GetSiteRootAddress.
        /// </summary>
        /// <param name="tenancyName">Parâmetro tenancyName.</param>
        /// <returns>Resultado da operação.</returns>
        public string GetSiteRootAddress(string tenancyName = null)
        {
            return ReplaceTenancyNameInUrl(WebSiteRootAddressFormat, tenancyName);
        }

        private static string ReplaceTenancyNameInUrl(string siteRootFormat, string tenancyName)
        {
            if (!siteRootFormat.Contains(TenancyNamePlaceHolder))
            {
                return siteRootFormat;
            }

            if (siteRootFormat.Contains(TenancyNamePlaceHolder + "."))
            {
                siteRootFormat = siteRootFormat.Replace(TenancyNamePlaceHolder + ".", TenancyNamePlaceHolder);
            }

            if (tenancyName.IsNullOrEmpty())
            {
                return siteRootFormat.Replace(TenancyNamePlaceHolder, "");
            }

            return siteRootFormat.Replace(TenancyNamePlaceHolder, tenancyName + ".");
        }
    }
}