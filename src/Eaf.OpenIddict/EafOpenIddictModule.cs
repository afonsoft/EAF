using Abp.Modules;
using Eaf.Middleware.Web;

namespace Eaf.OpenIddict
{
    /// <summary>
    /// Módulo ABP del servidor OAuth2/OpenID Connect de EAF.
    /// El registro de servicios ASP.NET Core se realiza llamando a
    /// <see cref="EafOpenIddictServiceCollectionExtensions.AddEafOpenIddict"/>
    /// desde <c>ConfigureServices</c> del host.
    /// </summary>
    [DependsOn(typeof(MiddlewareWebCoreModule))]
    public class EafOpenIddictModule : AbpModule
    {
    }
}
