using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using global::OpenIddict.Server.AspNetCore;

namespace Eaf.OpenIddict.Controllers
{
    /// <summary>
    /// Endpoint <c>/connect/logout</c>: cierra la sesión de autorización
    /// y redirige al post_logout_redirect_uri registrado.
    /// </summary>
    public class LogoutController : MiddlewareControllerBase
    {
        /// <summary>
        /// Finaliza la sesión del servidor de autorización.
        /// </summary>
        [HttpPost("~/connect/logout")]
        [IgnoreAntiforgeryToken]
        public IActionResult Logout()
        {
            return SignOut(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
