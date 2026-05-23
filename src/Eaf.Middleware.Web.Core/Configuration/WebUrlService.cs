using Abp.Dependency;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Url;

namespace Eaf.Middleware.Web.Url
{
    /// <summary>
    /// Representa a classe WebUrlService.
    /// </summary>
    public class WebUrlService : WebUrlServiceBase, IWebUrlService, ITransientDependency
    {
        /// <summary>
        /// WebUrlService.
        /// </summary>
        /// <param name="configurationAccessor">Parâmetro configurationAccessor.</param>
        /// <returns>Resultado da operação.</returns>
        public WebUrlService(
            IAppConfigurationAccessor configurationAccessor) :
            base(configurationAccessor)
        {
        }

        public override string ServerRootAddressFormatKey => "App:ServerRootAddress";
        public override string WebSiteRootAddressFormatKey => "App:ClientRootAddress";
    }
}