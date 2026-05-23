using System.Collections.Generic;

namespace Eaf.Middleware.Url
{
    /// <summary>
    /// Representa a interface IWebUrlService.
    /// </summary>
    public interface IWebUrlService
    {
        string ServerRootAddressFormat { get; }
        bool SupportsTenancyNameInUrl { get; }
        string WebSiteRootAddressFormat { get; }

        List<string> GetRedirectAllowedExternalWebSites();

        string GetServerRootAddress(string tenancyName = null);

        string GetSiteRootAddress(string tenancyName = null);
    }
}