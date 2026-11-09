using Abp;
using Abp.Dependency;
using Eaf.Middleware.Authentication.ApiKeys;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Authentication.ApiKeys
{
    /// <summary>
    /// Opções do esquema de autenticação por API key.
    /// </summary>
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    /// <summary>
    /// Handler de autenticação que valida o header <c>X-API-Key</c> contra o hash persistido,
    /// respeitando revogação, expiração e allowlist de IP, e produz o principal do usuário dono da chave.
    /// </summary>
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        /// <summary>
        /// ApiKeyAuthenticationHandler.
        /// </summary>
        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var keyValue = Request.Headers[ApiKeyConsts.HeaderName].ToString();
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                return AuthenticateResult.NoResult();
            }

            var iocManager = Context.RequestServices.GetService(typeof(IIocManager)) as IIocManager
                ?? IocManager.Instance;

            var isEnabled = await IsApiKeyAuthEnabledAsync(iocManager);
            if (!isEnabled)
            {
                return AuthenticateResult.Fail("API key authentication is disabled.");
            }

            var remoteIp = Context.Connection.RemoteIpAddress;

            using (var manager = iocManager.ResolveAsDisposable<IApiKeyManager>())
            {
                var result = await manager.Object.ValidateAsync(keyValue, remoteIp);
                if (!result.Succeeded)
                {
                    return AuthenticateResult.Fail($"Invalid API key ({result.Error}).");
                }

                var apiKey = result.ApiKey;

                using (var userManager = iocManager.ResolveAsDisposable<UserManager>())
                {
                    var user = await userManager.Object.FindByIdAsync(apiKey.UserId.ToString());
                    if (user == null || !user.IsActive)
                    {
                        return AuthenticateResult.Fail("API key owner is unavailable.");
                    }

                    using (var principalFactory = iocManager.ResolveAsDisposable<UserClaimsPrincipalFactory>())
                    {
                        var principal = await principalFactory.Object.CreateAsync(user);
                        var identity = (ClaimsIdentity)principal.Identity;

                        identity.AddClaim(new Claim(ApiKeyConsts.ApiKeyIdClaim, apiKey.Id.ToString()));
                        var scopes = apiKey.GetPermissionScopes();
                        if (scopes != null)
                        {
                            identity.AddClaim(new Claim(ApiKeyConsts.PermissionScopeClaim, string.Join(",", scopes)));
                        }

                        var apiKeyIdentity = new ClaimsIdentity(
                            identity.Claims,
                            ApiKeyConsts.SchemeName,
                            identity.NameClaimType,
                            identity.RoleClaimType);

                        var ticket = new AuthenticationTicket(
                            new ClaimsPrincipal(apiKeyIdentity),
                            ApiKeyConsts.SchemeName);

                        return AuthenticateResult.Success(ticket);
                    }
                }
            }
        }

        private static async Task<bool> IsApiKeyAuthEnabledAsync(IIocManager iocManager)
        {
            using (var settingManager = iocManager.ResolveAsDisposable<Abp.Configuration.ISettingManager>())
            {
                var raw = await settingManager.Object.GetSettingValueAsync(AppSettings.UserManagement.ApiKeys.IsEnabled);
                return !string.Equals(raw, "false", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
