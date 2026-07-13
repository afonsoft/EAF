using Abp.Authorization;
using Abp.Logging;
using Eaf.Middleware;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Abp;

namespace Eaf.AspNetCore.Hangfire
{
    /// <summary>
    /// Utilizado para efetuar a autenticação para acessar o Dashboard do Hangfire
    /// </summary>
    public class EafHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string[] _requiredPermissionName;
        private const string TokenValidityKey = "HangFireCache";

        /// <summary>
        /// Quais são as permissões para o acesso ao hangfire
        /// </summary>
        /// <param name="requiredPermissionName"></param>
        public EafHangfireAuthorizationFilter(params string[] requiredPermissionName)
        {
            _requiredPermissionName = requiredPermissionName;
        }

        /// <summary>
        ///  Default permissions "Pages.Administration", "Pages.Administration.HangfireDashboard"
        /// </summary>
        public EafHangfireAuthorizationFilter()
        {
            _requiredPermissionName = new[] { "Pages.Administration", "Pages.Administration.HangfireDashboard" };
        }

        /// <summary>
        /// Authorize.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        /// <returns>Resultado da operação.</returns>
        public bool Authorize(DashboardContext context)
        {
            return IsLoggedIn(context, _requiredPermissionName);
        }

        private bool IsLoggedIn(DashboardContext context, string[] permissions)
        {
            try
            {
                if (context?.GetHttpContext()?.Request?.Host != null &&
                    context.GetHttpContext().Request.Host.Host == "localhost")
                    return true;

                UserIdentifier userIdentifier = null;
                var eafSession = context.GetHttpContext().RequestServices.GetRequiredService<IAbpSession>();
                if (eafSession?.UserId != null)
                {
                    userIdentifier = eafSession.ToUserIdentifier();
                }

                if (userIdentifier == null)
                {
                    string token = GetToken(context);
                    if (token != null)
                    {
                        GetTenantIdClaim(token, out Claim id, out Claim tenanIdClaim, out Claim userIdentifierString);
                        if (userIdentifierString != null)
                        {
                            userIdentifier = UserIdentifier.Parse(userIdentifierString.Value);
                        }
                        else
                        {
                            int? tenanId = null;
                            long? userId = null;
                            if (tenanIdClaim != null)
                                tenanId = int.Parse(tenanIdClaim.Value);

                            if (id != null && !string.IsNullOrEmpty(id.Value))
                            {
                                userId = long.Parse(id.Value);
                            }
                            if (userId != null)
                                userIdentifier = new UserIdentifier(tenanId, userId.Value);
                        }
                    }
                }

                if (userIdentifier != null)
                {
                    if (permissions == null || !permissions.Any())
                        return true;

                    return IsPermissionGranted(context, userIdentifier, permissions);
                }

                return false;
            }
            catch (Exception ex)
            {
                LogHelper.Logger.WarnFormat(ex, "EafHangfireAuthorizationFilter: {0}", ex.Message);
                return false;
            }
        }

        private static void GetTenantIdClaim(string jwtToken, out Claim id, out Claim tenanIdClaim, out Claim userIdentifierString)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken securityToken = handler.ReadToken(jwtToken) as JwtSecurityToken;

            userIdentifierString = securityToken.Claims.FirstOrDefault(claim => claim.Type == MiddlewareCoreConsts.UserIdentifier);
            id = securityToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            tenanIdClaim = securityToken.Claims.FirstOrDefault(claim => claim.Type == AbpClaimTypes.TenantId);
        }

        private static bool IsPermissionGranted(DashboardContext context, UserIdentifier userIdentifier, params string[] requiredPermissionName)
        {
            if (userIdentifier == null)
                return false;

            if (!requiredPermissionName.Any())
                requiredPermissionName = new[] { "Pages.Administration", "Pages.Administration.HangfireDashboard" };

            var permissionChecker = context.GetHttpContext().RequestServices.GetRequiredService<IPermissionChecker>();
            if (permissionChecker != null)
                return permissionChecker.IsGranted(userIdentifier, false, requiredPermissionName);
            return true;
        }

        private static string GetToken(DashboardContext context)
        {
            string jwtToken = "";
            string[] auths = { "auth", "authtoken", "token", "accesstoken", "eaf.authtoken" };
            var request = context.GetHttpContext().Request;

            #region GetToken

            jwtToken = request.Query.FirstOrDefault(x => auths.Contains(x.Key.ToLower())).Value;

            if (string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(request.Cookies["Eaf.AuthToken"]))
                jwtToken = request.Cookies["Eaf.AuthToken"];

            if (string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(request.Headers["Eaf.AuthToken"]))
                jwtToken = request.Headers["Eaf.AuthToken"];

            var cacheManager = context.GetHttpContext().RequestServices.GetRequiredService<ICacheManager>();
            var remoteIpAddress = context.Request?.RemoteIpAddress ?? context.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString();

            if (string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(remoteIpAddress))
            {
                var tokenValidityKeyInCache = cacheManager
               .GetCache(TokenValidityKey)
               .GetOrDefault(remoteIpAddress);

                if (tokenValidityKeyInCache != null)
                    jwtToken = tokenValidityKeyInCache as string;
            }

            #endregion GetToken

            if (!string.IsNullOrEmpty(jwtToken))
            {
                if (!string.IsNullOrEmpty(remoteIpAddress))
                {
                    cacheManager
                    .GetCache(TokenValidityKey)
                    .Set(remoteIpAddress, jwtToken, TimeSpan.FromHours(1));
                }

                context.GetHttpContext().Response.Cookies.Append("Eaf.AuthToken",
                    jwtToken,

                    new CookieOptions()
                    {
                        Expires = DateTimeOffset.UtcNow.AddHours(1),
                        Path = "/",
                        HttpOnly = true,
                        Secure = true,
                        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                        IsEssential = true
                    });

                var setCookieHeaderValue = new SetCookieHeaderValue(
                    Uri.EscapeDataString("Eaf.AuthToken"),
                    Uri.EscapeDataString(jwtToken))
                {
                    Path = "/",
                    Expires = DateTimeOffset.UtcNow.AddHours(1),
                    Secure = true,
                    SameSite = Microsoft.Net.Http.Headers.SameSiteMode.Lax,
                    HttpOnly = true
                };

                var cookieValue = setCookieHeaderValue.ToString();
                context.GetHttpContext().Response.Headers[HeaderNames.SetCookie] = StringValues.Concat(context.GetHttpContext().Response.Headers[HeaderNames.SetCookie], cookieValue);
            }

            return jwtToken;
        }
    }
}