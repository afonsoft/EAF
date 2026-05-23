using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Eaf.Middleware.Web.Authentication.Identity
{
    /// <summary>
    /// Representa a classe IdentityExtensions.
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Returns a new IEnumerable that replaces claim based on newClaim.Type.
        /// </summary>
        /// <param name="claimsIdentity"></param>
        /// <param name="newClaim"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> ReplaceClaim(this IEnumerable<Claim> claimsIdentity, Claim newClaim)
        {
            return claimsIdentity.Select(claim => claim.Type == newClaim.Type ? newClaim : claim);
        }

        /// <summary>
        /// ReplaceClaim.
        /// </summary>
        /// <param name="claimsIdentity">Parâmetro claimsIdentity.</param>
        /// <param name="newClaim">Parâmetro newClaim.</param>
        public static void ReplaceClaim(this ClaimsIdentity claimsIdentity, Claim newClaim)
        {
            var claim = claimsIdentity.FindFirst(newClaim.Type);
            if (claim != null)
            {
                claimsIdentity.RemoveClaim(claim);
            }

            claimsIdentity.AddClaim(newClaim);
        }
    }
}