using Eaf.Middleware.Web.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using global::OpenIddict.Abstractions;
using global::OpenIddict.Validation.AspNetCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eaf.OpenIddict.Controllers
{
    /// <summary>
    /// Endpoint <c>/connect/userinfo</c>: devuelve los claims del usuario
    /// asociado al access token OpenIddict.
    /// </summary>
    public class UserInfoController : MiddlewareControllerBase
    {
        /// <summary>
        /// Devuelve sub, name, email y roles del titular del token.
        /// </summary>
        [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
        [HttpGet("~/connect/userinfo")]
        [HttpPost("~/connect/userinfo")]
        [Produces("application/json")]
        public Task<IActionResult> UserInfo()
        {
            var claims = new Dictionary<string, object>
            {
                [OpenIddictConstants.Claims.Subject] =
                    User.FindFirstValue(OpenIddictConstants.Claims.Subject),
            };

            var name = User.FindFirstValue(OpenIddictConstants.Claims.Name);
            if (name != null)
            {
                claims[OpenIddictConstants.Claims.Name] = name;
            }

            var email = User.FindFirstValue(OpenIddictConstants.Claims.Email);
            if (email != null)
            {
                claims[OpenIddictConstants.Claims.Email] = email;
            }

            var roles = User.FindAll(OpenIddictConstants.Claims.Role);
            var roleValues = new List<string>();
            foreach (var role in roles)
            {
                roleValues.Add(role.Value);
            }
            claims[OpenIddictConstants.Claims.Role] = roleValues;

            IActionResult result = Ok(claims);
            return Task.FromResult(result);
        }
    }
}
