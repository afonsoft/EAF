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
        private const string AuthTokenCookieName = "Eaf.AuthToken";

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

        private static bool IsLoggedIn(DashboardContext context, string[] permissions)
        {
            try
            {
                if (IsLocalHost(context))
                    return true;

                var userIdentifier = GetUserIdentifier(context);
                if (userIdentifier == null)
                    return false;

                if (permissions == null || !permissions.Any())
                    return true;

                return IsPermissionGranted(context, userIdentifier, permissions);
            }
            catch (Exception ex)
            {
                LogHelper.Logger.WarnFormat(ex, "EafHangfireAuthorizationFilter: {0}", ex.Message);
                return false;
            }
        }

        private static bool IsLocalHost(DashboardContext context)
        {
            var host = context?.GetHttpContext()?.Request?.Host;
            return host.HasValue && host.Value.Host == "localhost";
        }

        private static UserIdentifier GetUserIdentifier(DashboardContext context)
        {
            var userIdentifier = GetUserIdentifierFromSession(context);
            if (userIdentifier != null)
                return userIdentifier;

            return GetUserIdentifierFromToken(context);
        }

        private static UserIdentifier GetUserIdentifierFromSession(DashboardContext context)
        {
            var eafSession = context.GetHttpContext().RequestServices.GetRequiredService<IAbpSession>();
            return eafSession?.UserId != null ? eafSession.ToUserIdentifier() : null;
        }

        private static UserIdentifier GetUserIdentifierFromToken(DashboardContext context)
        {
            string token = GetToken(context);
            if (token == null)
                return null;

            GetTenantIdClaim(token, out Claim id, out Claim tenanIdClaim, out Claim userIdentifierString);
            if (userIdentifierString != null)
                return UserIdentifier.Parse(userIdentifierString.Value);

            return BuildUserIdentifierFromClaims(id, tenanIdClaim);
        }

        private static UserIdentifier BuildUserIdentifierFromClaims(Claim id, Claim tenanIdClaim)
        {
            int? tenantId = null;
            long? userId = null;

            if (tenanIdClaim != null)
                tenantId = int.Parse(tenanIdClaim.Value);

            if (id != null && !string.IsNullOrEmpty(id.Value))
                userId = long.Parse(id.Value);

            if (userId != null)
                return new UserIdentifier(tenantId, userId.Value);

            return null;
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

            if (string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(request.Cookies[AuthTokenCookieName]))
                jwtToken = request.Cookies[AuthTokenCookieName];

            if (string.IsNullOrEmpty(jwtToken) && !string.IsNullOrEmpty(request.Headers[AuthTokenCookieName]))
                jwtToken = request.Headers[AuthTokenCookieName];

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

                context.GetHttpContext().Response.Cookies.Append(AuthTokenCookieName,
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
                    Uri.EscapeDataString(AuthTokenCookieName),
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