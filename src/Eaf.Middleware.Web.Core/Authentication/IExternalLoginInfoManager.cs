using Abp.Dependency;
using Eaf.Middleware.Core.Authentication.External;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;

namespace Eaf.Middleware.Web.Authentication
{
    /// <summary>
    /// Representa a interface IExternalLoginInfoManager.
    /// </summary>
    public interface IExternalLoginInfoManager : ITransientDependency
    {
        (string name, string surname) GetNameAndSurnameFromClaims(List<Claim> claims, IdentityOptions identityOptions);

        string GetUserNameFromClaims(List<Claim> claims);

        string GetUserNameFromExternalAuthUserInfo(ExternalAuthUserInfo userInfo);
    }
}