using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Eaf.Middleware.Core.Authentication.External
{
    /// <summary>
    /// Representa a classe RemoteAuthenticationContextExtensions.
    /// </summary>
    public static class RemoteAuthenticationContextExtensions
    {
        public static void AddMappedClaims<TOptions>(
          this RemoteAuthenticationContext<TOptions> context,
          List<JsonClaimMap> mappings)
          where TOptions : RemoteAuthenticationOptions
        {
            if (!mappings.Any<JsonClaimMap>())
                return;
            foreach (JsonClaimMap mapping in mappings)
            {
                JsonClaimMap claimMapping = mapping;
                Claim claim = context.Principal.Claims.FirstOrDefault<Claim>((Func<Claim, bool>)(c => c.Type == claimMapping.Key));
                if (claim != null)
                    context.Principal.AddIdentity(new ClaimsIdentity((IEnumerable<Claim>)new List<Claim>()
          {
            new Claim(claimMapping.Claim, claim.Value)
          }));
            }
        }

        /// <summary>
        /// AddMappedClaims.
        /// </summary>
        /// <param name="principal">Parâmetro principal.</param>
        /// <param name="mappings">Parâmetro mappings.</param>
        public static void AddMappedClaims(this ClaimsPrincipal principal, List<JsonClaimMap> mappings)
        {
            if (!mappings.Any<JsonClaimMap>())
                return;
            foreach (JsonClaimMap mapping in mappings)
            {
                JsonClaimMap claimMapping = mapping;
                Claim claim = principal.Claims.FirstOrDefault<Claim>((Func<Claim, bool>)(c => c.Type == claimMapping.Key));
                if (claim != null)
                    principal.AddIdentity(new ClaimsIdentity((IEnumerable<Claim>)new List<Claim>()
          {
            new Claim(claimMapping.Claim, claim.Value)
          }));
            }
        }
    }
}