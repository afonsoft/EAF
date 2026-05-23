using Abp.Extensions;
using Eaf.Middleware.Core.Authentication.External;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Representa a classe DefaultExternalLoginInfoManager.
    /// </summary>
    public class DefaultExternalLoginInfoManager : IExternalLoginInfoManager
    {
        /// <summary>
        /// Obtém nome e sobrenome a partir das claims de autenticação externa.
        /// </summary>
        /// <param name="claims">Lista de claims do usuário autenticado.</param>
        /// <param name="identityOptions">Opções de identidade.</param>
        /// <returns>Tupla com nome e sobrenome extraídos.</returns>
        public (string name, string surname) GetNameAndSurnameFromClaims(List<Claim> claims, IdentityOptions identityOptions)
        {
            string name = null;
            string surname = null;

            var givenNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (givenNameClaim != null && !givenNameClaim.Value.IsNullOrEmpty())
            {
                name = givenNameClaim.Value;
            }

            var surnameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            if (surnameClaim != null && !surnameClaim.Value.IsNullOrEmpty())
            {
                surname = surnameClaim.Value;
            }

            if (name == null || surname == null)
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == identityOptions.ClaimsIdentity.UserNameClaimType);
                if (nameClaim != null)
                {
                    var nameSurName = nameClaim.Value;
                    if (!nameSurName.IsNullOrEmpty())
                    {
                        var lastSpaceIndex = nameSurName.LastIndexOf(' ');
                        if (lastSpaceIndex < 1 || lastSpaceIndex > (nameSurName.Length - 2))
                        {
                            name = surname = nameSurName;
                        }
                        else
                        {
                            name = nameSurName[..lastSpaceIndex];
                            surname = nameSurName[(lastSpaceIndex)..];
                        }
                    }
                }
            }

            return (name, surname);
        }

        /// <summary>
        /// GetUserNameFromClaims.
        /// </summary>
        /// <param name="claims">Parâmetro claims.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual string GetUserNameFromClaims(List<Claim> claims)
        {
            return claims.First(c => c.Type == ClaimTypes.Email)?.Value.Split('@')[0];
        }

        /// <summary>
        /// GetUserNameFromExternalAuthUserInfo.
        /// </summary>
        /// <param name="userInfo">Parâmetro userInfo.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual string GetUserNameFromExternalAuthUserInfo(ExternalAuthUserInfo userInfo)
        {
            return userInfo.EmailAddress.Split('@')[0];
        }
    }
}