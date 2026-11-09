using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using global::OpenIddict.Abstractions;
using global::OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.Threading.Tasks;
using static global::OpenIddict.Abstractions.OpenIddictConstants;

namespace Eaf.OpenIddict.Controllers
{
    /// <summary>
    /// Endpoint <c>/connect/authorize</c> (authorization code flow).
    /// Requiere un usuario previamente autenticado (p. ej. vía JWT bearer);
    /// el cliente debe completar PKCE.
    /// </summary>
    public class AuthorizationController : MiddlewareControllerBase
    {
        /// <summary>
        /// Emite un authorization code hacia la redirect_uri registrada.
        /// </summary>
        [HttpGet("~/connect/authorize")]
        [HttpPost("~/connect/authorize")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Authorize()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                return BadRequest(new global::OpenIddict.Abstractions.OpenIddictResponse
                {
                    Error = Errors.InvalidRequest,
                    ErrorDescription = "The OpenID Connect request cannot be retrieved.",
                });
            }

            var authentication = await HttpContext.AuthenticateAsync(
                JwtBearerDefaults.AuthenticationScheme);

            if (!authentication.Succeeded)
            {
                // El cliente debe autenticar al usuario primero (login interactivo
                // o bearer). Se devuelve un challenge estándar.
                return Challenge(JwtBearerDefaults.AuthenticationScheme);
            }

            var user = authentication.Principal;
            var identity = new ClaimsIdentity(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            var subject = user.FindFirstValue(Claims.Subject)
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? user.FindFirstValue("sub");
            identity.AddClaim(new Claim(Claims.Subject, subject));
            identity.AddClaim(new Claim(Claims.Name,
                user.FindFirstValue(Claims.Name)
                ?? user.FindFirstValue(ClaimTypes.Name)
                ?? subject));

            var principal = new ClaimsPrincipal(identity);
            principal.SetScopes(request.GetScopes());
            foreach (var claim in principal.Claims)
            {
                claim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
            }

            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
