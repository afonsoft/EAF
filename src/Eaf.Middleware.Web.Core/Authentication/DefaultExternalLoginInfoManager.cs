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
            var name = GetGivenName(claims);
            var surname = GetSurname(claims);

            if (name == null || surname == null)
            {
                (name, surname) = ExtractNameAndSurnameFromNameClaim(claims, identityOptions.ClaimsIdentity.UserNameClaimType);
            }

            return (name, surname);
        }

        private static string GetGivenName(List<Claim> claims)
        {
            var claim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            return claim != null && !claim.Value.IsNullOrEmpty() ? claim.Value : null;
        }

        private static string GetSurname(List<Claim> claims)
        {
            var claim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            return claim != null && !claim.Value.IsNullOrEmpty() ? claim.Value : null;
        }

        private static (string name, string surname) ExtractNameAndSurnameFromNameClaim(List<Claim> claims, string userNameClaimType)
        {
            var nameClaim = claims.FirstOrDefault(c => c.Type == userNameClaimType);
            if (nameClaim == null)
                return (null, null);

            var nameSurName = nameClaim.Value;
            if (nameSurName.IsNullOrEmpty())
                return (null, null);

            var lastSpaceIndex = nameSurName.LastIndexOf(' ');
            if (lastSpaceIndex < 1 || lastSpaceIndex > nameSurName.Length - 2)
                return (nameSurName, nameSurName);

            return (nameSurName[..lastSpaceIndex], nameSurName[(lastSpaceIndex)..]);
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