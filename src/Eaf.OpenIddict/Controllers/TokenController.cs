using Abp.MultiTenancy;
using Abp.Runtime.Session;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Web.Controllers;
using Eaf.Middleware.Web.Authentication;
using Eaf.OpenIddict.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using global::OpenIddict.Abstractions;
using global::OpenIddict.Server.AspNetCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static global::OpenIddict.Abstractions.OpenIddictConstants;

namespace Eaf.OpenIddict.Controllers
{
    /// <summary>
    /// Endpoint <c>/connect/token</c> del servidor OAuth2/OIDC.
    /// Soporta los grant types <c>password</c> y <c>client_credentials</c>;
    /// los access tokens son JWT firmados con la misma clave simétrica del
    /// JwtBearer de EAF, por lo que los endpoints <c>[AbpAuthorize]</c> los
    /// aceptan cuando el issuer se agrega a
    /// <c>Authentication:JwtBearer:AdditionalIssuers</c>.
    /// </summary>
    public class TokenController : MiddlewareControllerBase
    {
        private readonly LogInManager _logInManager;
        private readonly UserManager _userManager;
        private readonly ITenantCache _tenantCache;
        private readonly IAbpSession _abpSession;
        private readonly EafOpenIddictOptions _options;

        /// <summary>
        /// TokenController.
        /// </summary>
        public TokenController(
            LogInManager logInManager,
            UserManager userManager,
            ITenantCache tenantCache,
            IAbpSession abpSession,
            IOptions<EafOpenIddictOptions> options)
        {
            _logInManager = logInManager;
            _userManager = userManager;
            _tenantCache = tenantCache;
            _abpSession = abpSession;
            _options = options.Value;
        }

        /// <summary>
        /// Emite un access token JWT según el grant type solicitado.
        /// </summary>
        [HttpPost("~/connect/token")]
        [IgnoreAntiforgeryToken]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            if (request == null)
            {
                return BadRequest(new OpenIddictResponse
                {
                    Error = Errors.InvalidRequest,
                    ErrorDescription = "The OpenID Connect request cannot be retrieved.",
                });
            }

            if (request.IsClientCredentialsGrantType())
            {
                var identity = new ClaimsIdentity(
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(Claims.Subject, request.ClientId));
                identity.AddClaim(new Claim(Claims.Name, request.ClientId));

                var principal = new ClaimsPrincipal(identity);
                principal.SetScopes(request.GetScopes());
                principal.SetResources(_options.Audience);
                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(Destinations.AccessToken);
                }

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            if (request.IsPasswordGrantType())
            {
                var loginResult = await _logInManager.LoginAsync(
                    request.Username,
                    request.Password,
                    GetTenancyNameOrNull());

                if (loginResult.Result != Abp.Authorization.AbpLoginResultType.Success)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                        {
                            Items =
                            {
                                [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                                [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                                    "The username/password couple is invalid.",
                            },
                        });
                }

                var user = loginResult.User;
                var roles = await _userManager.GetRolesAsync(user);

                var identity = new ClaimsIdentity(
                    OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(Claims.Subject, user.Id.ToString()));
                identity.AddClaim(new Claim(Claims.Name, user.UserName));
                identity.AddClaim(new Claim(Claims.PreferredUsername, user.UserName));
                if (!string.IsNullOrEmpty(user.EmailAddress))
                {
                    identity.AddClaim(new Claim(Claims.Email, user.EmailAddress));
                }
                identity.AddClaims(roles.Select(r => new Claim(Claims.Role, r)));

                var principal = new ClaimsPrincipal(identity);
                principal.SetScopes(request.GetScopes());
                principal.SetResources(_options.Audience);
                foreach (var claim in principal.Claims)
                {
                    claim.SetDestinations(Destinations.AccessToken, Destinations.IdentityToken);
                }

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            return BadRequest(new OpenIddictResponse
            {
                Error = Errors.UnsupportedGrantType,
                ErrorDescription = "The specified grant type is not supported.",
            });
        }

        private string GetTenancyNameOrNull()
        {
            if (!_abpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(_abpSession.TenantId.Value)?.TenancyName;
        }
    }
}
