using Castle.Core.Logging;
using Abp.Application.Services;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Data;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Json;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.TwoFactor;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Security.Recaptcha;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Authentication;
using Eaf.Middleware.Web.Authentication.Identity;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Eaf.Middleware.Web.Models.TokenAuth;
using Eaf.Middleware.Web.Notifications;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.UI;
using Abp.Webhooks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Abp.Extensions;
using Abp;
using Abp.Runtime.Session;
using Abp.Runtime;
using Eaf.Security;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Controller responsável por endpoints de TokenAuth.
    /// </summary>
    [AbpAllowAnonymous]
    [Route("api/[controller]/[action]")]
    [EnableRateLimiting("EafAuth")]
    public class TokenAuthController : MiddlewareControllerBase, IApplicationService
    {
        private const string ExternalTokenInformationCacheName = "ExternalTokenInformationCache";

        private readonly ICacheManager _cacheManager;
        private readonly TokenAuthConfiguration _configuration;
        private readonly AbpLoginResultTypeHelper _AbpLoginResultTypeHelper;
        private readonly IEmailSender _emailSender;
        private readonly IRefreshTokenStore _refreshTokenStore;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly IdentityOptions _identityOptions;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IIocManager _iocManager;
        private readonly IOptions<JwtBearerOptions> _jwtOptions;
        private readonly LogInManager _logInManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ISettingManager _settingManager;
        private readonly ITenantCache _tenantCache;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly INotificationPublisher _notificationPublisher;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IWebhookPublisher _webhookPublisher;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IPrincipalAccessor _principalAccessor;

        /// <summary>
        /// TokenAuthController.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public TokenAuthController( // NOSONAR
            LogInManager logInManager,
            AbpLoginResultTypeHelper AbpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            UserManager userManager,
            RoleManager roleManager,
            ITenantCache tenantCache,
            ICacheManager cacheManager,
            IImpersonationManager impersonationManager,
            IOptions<IdentityOptions> identityOptions,
            ILogger logger,
            ISettingManager settingManager,
            IExternalAuthManager externalAuthManager,
            IExternalAuthConfiguration externalAuthConfiguration,
            IIocManager iocManager,
            IPasswordHasher<User> passwordHasher,
            IEmailSender emailSender,
            IOptions<JwtBearerOptions> jwtOptions,
            INotificationPublisher notificationPublisher,
            IBinaryObjectManager binaryObjectManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IWebhookPublisher webhookPublisher,
            IPrincipalAccessor principalAccessor,
            IRefreshTokenStore refreshTokenStore
        )
        {
            _logInManager = logInManager;
            _roleManager = roleManager;
            _tenantCache = tenantCache;
            _AbpLoginResultTypeHelper = AbpLoginResultTypeHelper;
            _configuration = configuration;
            _userManager = userManager;
            _cacheManager = cacheManager;
            _impersonationManager = impersonationManager;
            _identityOptions = identityOptions.Value;
            Logger = logger;
            _externalAuthConfiguration = externalAuthConfiguration;
            _settingManager = settingManager;
            _externalAuthManager = externalAuthManager;
            _iocManager = iocManager;
            _passwordHasher = passwordHasher;
            _emailSender = emailSender;
            _jwtOptions = jwtOptions;
            _notificationPublisher = notificationPublisher;
            _binaryObjectManager = binaryObjectManager;
            _webhookPublisher = webhookPublisher;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _principalAccessor = principalAccessor;
            _refreshTokenStore = refreshTokenStore;
            RecaptchaValidator = NullRecaptchaValidator.Instance;
        }

        /// <summary>
        /// Obtém ou define RecaptchaValidator.
        /// </summary>
        public IRecaptchaValidator RecaptchaValidator { get; set; }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            if (!ModelState.IsValid)
                throw new UserFriendlyException(L("InvalidRequest"));

            if (UseCaptchaOnLogin())
            {
                await ValidateReCaptcha(model.CaptchaResponse);
            }

            var expirationSettings = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
            var expiration = TimeSpan.FromSeconds(expirationSettings);
            var currentUserName = model.UserNameOrEmailAddress.ToLower().Trim();

            if (model.RememberClient)
                expiration = TimeSpan.FromDays(365);

            var loginResult = await GetLoginResultAsync(
                    currentUserName,
                    model.Password,
                    GetTenancyNameOrNull()
                );

            var returnUrl = model.ReturnUrl;

            if (model.SingleSignIn.HasValue && model.SingleSignIn.Value && loginResult.Result == AbpLoginResultType.Success)
            {
                loginResult.User.SetSignInToken((int)expiration.TotalSeconds);
                returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
            }

            //Password reset
            if (loginResult.User.ShouldChangePasswordOnNextLogin)
            {
                loginResult.User.SetNewPasswordResetCode();
                return new AuthenticateResultModel
                {
                    ShouldResetPassword = true,
                    PasswordResetCode = loginResult.User.PasswordResetCode,
                    UserId = loginResult.User.Id,
                    ReturnUrl = returnUrl
                };
            }

            //Two factor auth
            await _userManager.InitializeOptionsAsync(loginResult.Tenant?.Id);

            string twoFactorRememberClientToken = null;
            if (await IsTwoFactorAuthRequiredAsync(loginResult, model))
            {
                if (model.TwoFactorVerificationCode.IsNullOrEmpty())
                {
                    //Add a cache item which will be checked in SendTwoFactorAuthCode to prevent sending unwanted two factor code to users.
                    await _cacheManager
                        .GetTwoFactorCodeCache()
                        .SetAsync(
                            loginResult.User.ToUserIdentifier().ToString(),
                            new TwoFactorCodeCacheItem()
                        );

                    return new AuthenticateResultModel
                    {
                        RequiresTwoFactorVerification = true,
                        UserId = loginResult.User.Id,
                        TwoFactorAuthProviders = await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User),
                        ReturnUrl = returnUrl
                    };
                }

                twoFactorRememberClientToken = await TwoFactorAuthenticateAsync(loginResult.User, model);
            }

            // One Concurrent Login
            if (AllowOneConcurrentLoginPerUser())
            {
                var identityResult = await _userManager.UpdateSecurityStampAsync(loginResult.User);
                if (identityResult.Succeeded)
                {
                    loginResult.User.SecurityStamp = await _userManager.GetSecurityStampAsync(loginResult.User);
                    loginResult.Identity.ReplaceClaim(new Claim(MiddlewareCoreConsts.SecurityStampKey, loginResult.User.SecurityStamp));
                    loginResult.Identity.ReplaceClaim(new Claim(MiddlewareCoreConsts.TokenValidityValue, loginResult.User.SecurityStamp));
                }
            }

            //Login!
            var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User), expiration);
            var refreshToken = await GenerateAndStoreRefreshTokenAsync(loginResult.User);
            AppendRefreshTokenCookie(refreshToken.Token, refreshToken.ExpireDate);

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int)expiration.TotalSeconds,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                TwoFactorRememberClientToken = twoFactorRememberClientToken,
                UserId = loginResult.User.Id,
                ReturnUrl = returnUrl
            };
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public virtual async Task<List<AvailableTenantResult>> GetAvailableTenants([FromBody] AvailableTenantsModel model)
        {
            if (!ModelState.IsValid)
                throw new UserFriendlyException(L("InvalidRequest"));

            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                null);

            if (loginResult.User.TenantId.HasValue)
                throw new UserFriendlyException(L("OnlyHostUsersCanSelectTenant"));

            using (var tenantUserManager = _iocManager.ResolveAsDisposable<ITenantUserManager>())
            {
                var memberships = await tenantUserManager.Object.GetMembershipsAsync(loginResult.User.Id);

                var results = new List<AvailableTenantResult>();
                foreach (var membership in memberships)
                {
                    var tenant = _tenantCache.GetOrNull(membership.TenantId);
                    if (tenant == null)
                        continue;

                    results.Add(new AvailableTenantResult
                    {
                        TenantId = membership.TenantId,
                        TenantName = tenant.Name,
                        TenancyName = tenant.TenancyName,
                        IsDefault = membership.IsDefault
                    });
                }

                return results;
            }
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public virtual async Task<AuthenticateResultModel> SelectTenant([FromBody] SelectTenantModel model)
        {
            if (!ModelState.IsValid)
                throw new UserFriendlyException(L("InvalidRequest"));

            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                null);

            if (loginResult.User.TenantId.HasValue)
                throw new UserFriendlyException(L("OnlyHostUsersCanSelectTenant"));

            using (var tenantUserManager = _iocManager.ResolveAsDisposable<ITenantUserManager>())
            {
                var membership = await tenantUserManager.Object.EnsureMembershipAsync(loginResult.User.Id, model.TenantId);

                var expirationSettings = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
                var expiration = TimeSpan.FromSeconds(expirationSettings);

                using (CurrentUnitOfWork.SetTenantId(model.TenantId, switchMustHaveTenantEnableDisable: false))
                using (CurrentUnitOfWork.EnableFilter(AbpDataFilters.MayHaveTenant))
                {
                    var shadowUser = await _userManager.FindByIdAsync(membership.TenantUserId.ToString());
                    if (shadowUser == null)
                        throw new UserFriendlyException(L("ShadowUserNotFound"));

                    ClaimsIdentity identity;
                    using (var principalFactory = _iocManager.ResolveAsDisposable<UserClaimsPrincipalFactory>())
                    {
                        var principal = await principalFactory.Object.CreateAsync(shadowUser);
                        identity = (ClaimsIdentity)principal.Identity;
                    }

                    var accessToken = CreateAccessToken(await CreateJwtClaims(identity, shadowUser), expiration);
                    var refreshToken = await GenerateAndStoreRefreshTokenAsync(shadowUser);
                    AppendRefreshTokenCookie(refreshToken.Token, refreshToken.ExpireDate);

                    return new AuthenticateResultModel
                    {
                        AccessToken = accessToken,
                        ExpireInSeconds = (int)expiration.TotalSeconds,
                        EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                        UserId = shadowUser.Id
                    };
                }
            }
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate(
            [FromBody] ExternalAuthenticateModel model)
        {
            if (!ModelState.IsValid)
                throw new UserFriendlyException(L("InvalidRequest"));

            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(
                new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider),
                GetTenancyNameOrNull()
            );
            Logger.DebugFormat("ExternalAuthenticate {0}", loginResult.Result);

            var expirationSettings = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
            var expiration = TimeSpan.FromSeconds(expirationSettings);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    await UpdateExternalUserAsync(loginResult.User, externalUser);
                    return await GetExternalAuthenticateResultAsync(loginResult.User, loginResult.Identity, model, expiration);
                case AbpLoginResultType.UnknownExternalLogin:
                    return await HandleUnknownExternalLoginAsync(externalUser, model, expiration);
                default:
                    throw _AbpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        model.ProviderKey,
                        GetTenancyNameOrNull()
                    );
            }
        }

        private async Task<ExternalAuthenticateResultModel> GetExternalAuthenticateResultAsync(
            User user,
            ClaimsIdentity identity,
            ExternalAuthenticateModel model,
            TimeSpan expiration)
        {
            await _cacheManager
                .GetCache(ExternalTokenInformationCacheName)
                .SetAsync(user.ToUserIdentifier().ToString(),
                    model.ProviderAccessCode,
                    slidingExpireTime: TimeSpan.FromDays(1));

            var accessToken = CreateAccessToken(await CreateJwtClaims(identity, user, model.AuthProvider), expiration);

            var returnUrl = model.ReturnUrl;

            if (model.SingleSignIn.HasValue && model.SingleSignIn.Value)
            {
                user.SetSignInToken((int)expiration.TotalSeconds);
                returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, user.SignInToken, user.Id, user.TenantId);
            }

            return new ExternalAuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)expiration.TotalSeconds,
                ReturnUrl = returnUrl,
                WaitingForActivation = false,
                UserId = user.Id
            };
        }

        private async Task<ExternalAuthenticateResultModel> HandleUnknownExternalLoginAsync(
            ExternalAuthUserInfo externalUser,
            ExternalAuthenticateModel model,
            TimeSpan expiration)
        {
            var newUser = await RegisterExternalUserAsync(externalUser);
            if (!newUser.IsActive)
            {
                return new ExternalAuthenticateResultModel
                {
                    WaitingForActivation = true
                };
            }

            //Try to login again with newly registered user!
            var loginResult = await _logInManager.LoginAsync(
                new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider),
                GetTenancyNameOrNull()
            );

            Logger.DebugFormat("ExternalAuthenticate - UnknownExternalLogin {0}", loginResult.Result);

            if (loginResult.Result != AbpLoginResultType.Success)
            {
                loginResult = await _logInManager.CreateLoginResultAsync(newUser);
                if (loginResult.Result != AbpLoginResultType.Success)
                {
                    throw _AbpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                        loginResult.Result,
                        model.ProviderKey,
                        GetTenancyNameOrNull()
                    );
                }
            }

            return await GetExternalAuthenticateResultAsync(loginResult.User, loginResult.Identity, model, expiration);
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> TeamsAuthenticate(
           [FromBody] string idToken)
        {
            bool microsoftEnabled = await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled);

            if (!microsoftEnabled)
                throw new AbpException("Microsoft Provider is not enabled in HostSettings");

            var microsoftSettings = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.Host.Microsoft);

            if (microsoftSettings.IsNullOrWhiteSpace())
                throw new AbpException("Microsoft Provider is not configured in HostSettings");

            var microsoft = microsoftSettings.FromJsonString<MicrosoftExternalLoginProviderSettings>();

            var authenticationResult = await GetAccessTokenOnBehalfUserAsync(idToken, microsoft.ClientId, microsoft.ClientSecret, microsoft.TenantId);
            Logger.DebugFormat("TeamsAuthenticate - authenticationResult {0}", authenticationResult);

            if (authenticationResult == null
                || authenticationResult.AccessToken.IsNullOrWhiteSpace())
                throw new AbpException("authenticationResult is null in GetAccessTokenOnBehalfUser");

            var model = new ExternalAuthenticateModel
            {
                AuthProvider = "Microsoft",
                ReturnUrl = "",
                SingleSignIn = false,
                ProviderKey = authenticationResult.UniqueId ?? authenticationResult.IdToken,
                ProviderAccessCode = authenticationResult.AccessToken
            };

            Logger.DebugFormat("TeamsAuthenticate Final -> ExternalAuthenticate {0}", model);
            return await ExternalAuthenticate(model);
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public ProviderModel GetAuthenticationProviders(string usernameOrEmailAddress)
        {
            var provider = new ProviderModel
            {
                UsernameOrEmailAddress = usernameOrEmailAddress,
                AuthenticationSource = GetDefaultEnabledProvider()
            };
            User user = null;

            if (!string.IsNullOrEmpty(usernameOrEmailAddress))
            {
                usernameOrEmailAddress = usernameOrEmailAddress.ToUpperInvariant().Trim();

                user = _userManager.Users
                  .FirstOrDefault(u => u.NormalizedUserName == usernameOrEmailAddress
                                   || u.NormalizedEmailAddress == usernameOrEmailAddress);
                if (user != null)
                {
                    if (user.AuthenticationSource == null)
                        provider.AuthenticationSource = "System";
                    else
                        provider.AuthenticationSource = user.AuthenticationSource;
                }
            }
            provider.Tenant = GetTenant(user?.TenantId ?? 1);

            return provider;
        }

        private TenantModal GetTenant(int id)
        {
            TenantModal modal = new TenantModal { Id = id, Name = "Default", TenancyName = "Default" };
            var tenant = _tenantCache.Get(id);
            modal.Id = tenant?.Id ?? id;
            modal.Name = tenant?.Name ?? modal.Name;
            modal.TenancyName = tenant?.TenancyName ?? modal.TenancyName;
            return modal;
        }

        private string GetDefaultEnabledProvider()
        {
            if (SettingManager.GetSettingValueForApplication<bool>(LdapSettingNames.IsEnabled))
                return "LDAP";
            else if (SettingManager.GetSettingValueForApplication<bool>(AzureActiveDirectorySettingNames.IsEnabled))
                return "ActiveDirectory";
            else if (SettingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled))
                return "Microsoft";
            else if (SettingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled))
                return "Google";
            else if (SettingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled))
                return "AuthZero";
            else if (SettingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled))
                return "OpenIdConnect";
            else
                return "System";
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            var allProviders = _externalAuthConfiguration.ExternalLoginInfoProviders
                .Select(infoProvider => infoProvider.GetExternalLoginInfo())
                .Where(IsSchemeEnabled)
                .ToList();
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(allProviders);
        }

        [HttpPost]
        public async Task<ImpersonatedAuthenticateResultModel> ImpersonatedAuthenticate(string impersonationToken)
        {
            var expirationSettings = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
            var expiration = TimeSpan.FromSeconds(expirationSettings);

            var result = await _impersonationManager.GetImpersonatedUserAndIdentity(impersonationToken);
            var accessToken = CreateAccessToken(await CreateJwtClaims(result.Identity, result.User), expiration);

            return new ImpersonatedAuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)expiration.TotalSeconds
            };
        }

        [HttpGet]
        public async Task LogOut()
        {
            try
            {
                await RemoveEafSessionCacheAsync();
            }
            catch (Exception ex)
            {
                Logger.DebugFormat(ex, "Logout (EafSession): {0}", ex.Message);
            }

            try
            {
                await RemovePrincipalAccessorCacheAsync();
            }
            catch (Exception ex)
            {
                Logger.DebugFormat(ex, "Logout (IPrincipalAccessor): {0}", ex.Message);
            }

            try
            {
                await RemoveIdentityCacheAsync();
            }
            catch (Exception ex)
            {
                Logger.DebugFormat(ex, "Logout (Identity): {0}", ex.Message);
            }
        }

        private async Task RemoveEafSessionCacheAsync()
        {
            if (AbpSession?.UserId == null)
                return;

            var user = await _userManager.GetUserAsync(AbpSession.ToUserIdentifier());
            await _cacheManager.GetCache(ExternalTokenInformationCacheName).RemoveAsync(user.ToUserIdentifier().ToString());
            await _userManager.UpdateSecurityStampAsync(user);
        }

        private async Task RemovePrincipalAccessorCacheAsync()
        {
            var claims = _principalAccessor?.Principal;
            if (claims == null)
                return;

            var tokenValidityKeyInClaims = claims.Claims.FirstOrDefault(c => c.Type == MiddlewareCoreConsts.TokenValidityKey)?.Value ?? "";
            var userIdentifierString = claims.Claims.FirstOrDefault(c => c.Type == MiddlewareCoreConsts.UserIdentifier)?.Value ?? "";

            if (!string.IsNullOrEmpty(tokenValidityKeyInClaims))
            {
                await _cacheManager.GetCache(MiddlewareCoreConsts.TokenValidityKey).RemoveAsync(tokenValidityKeyInClaims);
                if (!string.IsNullOrEmpty(userIdentifierString))
                    await _userManager.RemoveTokenValidityKeyAsync(await _userManager.GetUserAsync(UserIdentifier.Parse(userIdentifierString)), tokenValidityKeyInClaims);
            }

            if (!string.IsNullOrEmpty(userIdentifierString))
            {
                await _cacheManager.GetCache(ExternalTokenInformationCacheName).RemoveAsync(UserIdentifier.Parse(userIdentifierString).ToString());
                var user = await _userManager.GetUserAsync(UserIdentifier.Parse(userIdentifierString));
                await _userManager.UpdateSecurityStampAsync(user);
            }
        }

        private async Task RemoveIdentityCacheAsync()
        {
            if (User?.Claims == null || !User.Claims.Any())
                return;

            var userIdentifier = User.Identity.GetUserIdentifierOrNull();
            var tokenValidityKeyInClaims = User.Claims.FirstOrDefault(c => c.Type == MiddlewareCoreConsts.TokenValidityKey)?.Value ?? "";

            if (!string.IsNullOrEmpty(tokenValidityKeyInClaims))
            {
                await _cacheManager.GetCache(MiddlewareCoreConsts.TokenValidityKey).RemoveAsync(tokenValidityKeyInClaims);
                if (userIdentifier != null)
                    await _userManager.RemoveTokenValidityKeyAsync(await _userManager.GetUserAsync(userIdentifier), tokenValidityKeyInClaims);
            }

            if (userIdentifier != null)
            {
                await _cacheManager.GetCache(ExternalTokenInformationCacheName).RemoveAsync(userIdentifier.ToString());
                var user = await _userManager.GetUserAsync(userIdentifier);
                await _userManager.UpdateSecurityStampAsync(user);
            }
        }

        [HttpPost]
        public async Task SendTwoFactorAuthCode([FromBody] SendTwoFactorAuthCodeModel model)
        {
            if (!ModelState.IsValid)
                throw new UserFriendlyException(L("InvalidRequest"));

            var cacheKey = new UserIdentifier(AbpSession.TenantId, model.UserId).ToString();

            var cacheItem = await _cacheManager
                .GetTwoFactorCodeCache()
                .GetOrDefaultAsync(cacheKey);

            if (cacheItem == null)
            {
                //There should be a cache item added in Authenticate method! This check is needed to prevent sending unwanted two factor code to users.
                throw new UserFriendlyException(L("SendSecurityCodeErrorMessage"));
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());

            cacheItem.Code = await _userManager.GenerateTwoFactorTokenAsync(user, model.Provider);
            var message = L("EmailSecurityCodeBody", cacheItem.Code);

            if (model.Provider == "Email")
            {
                await _emailSender.SendAsync(await _userManager.GetEmailAsync(user), L("EmailSecurityCodeSubject"),
                    message);
            }

            await _cacheManager.GetTwoFactorCodeCache().SetAsync(
                cacheKey,
                cacheItem
            );

            await _cacheManager.GetCache("ProviderCache").SetAsync(
                "Provider",
                model.Provider
            );
        }

        private static string GetEncryptedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, MiddlewareCoreConsts.DefaultPassPhrase);
        }

        private static bool ProviderKeysAreEqual(ExternalAuthenticateModel model, ExternalAuthUserInfo userInfo)
        {
            if (string.IsNullOrEmpty(userInfo?.ProviderKey) || string.IsNullOrEmpty(model?.ProviderKey))
                return false;

            if (userInfo.ProviderKey == model.ProviderKey)
                return true;

            return userInfo.ProviderKey.Replace("-", "").TrimStart('0').Trim().ToUpper() == model.ProviderKey.Replace("-", "").TrimStart('0').Trim().ToUpper();
        }

        private static string AddSingleSignInParametersToReturnUrl(string returnUrl, string signInToken, long userId, int? tenantId)
        {
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "";

            returnUrl += (returnUrl.Contains("?") ? "&" : "?") +
                         "accessToken=" + signInToken +
                         "&userId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));

            if (tenantId.HasValue)
                returnUrl += "&tenantId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(tenantId.Value.ToString()));

            return returnUrl;
        }

        private bool AllowOneConcurrentLoginPerUser()
        {
            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser);
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan expiration)
        {
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow.AddMinutes(-1),
                expires: DateTime.UtcNow.Add(expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        [UnitOfWork]
        private async Task<IEnumerable<Claim>> CreateJwtClaims(ClaimsIdentity identity, User user, string externalAuthProviderformation = "")
        {
            var expirationSettings = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
            var isTwoFactorEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.TwoFactorLogin.IsEnabled);

            var expiration = TimeSpan.FromSeconds(expirationSettings);

            var tokenValidityKey = Guid.NewGuid().ToString();
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == _identityOptions.ClaimsIdentity.UserIdClaimType);

            if (_identityOptions.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
            }

            var userIdentifier = new UserIdentifier(user.TenantId, Convert.ToInt64(nameIdClaim.Value));
            var guid = Guid.NewGuid();
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,guid.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(MiddlewareCoreConsts.TokenValidityKey, tokenValidityKey),
                new Claim(MiddlewareCoreConsts.UserIdentifier, userIdentifier.ToUserIdentifierString())
            });

            if (user.TenantId.HasValue)
            {
                claims.Add(new Claim("tenantid", user.TenantId.Value.ToString()));
            }

            if (!string.IsNullOrEmpty(externalAuthProviderformation))
                claims.ReplaceClaim(new Claim(EafClaimTypes.ExternalAuthProviderformation, externalAuthProviderformation));

            if (isTwoFactorEnabled || !string.IsNullOrEmpty(externalAuthProviderformation))
                claims.ReplaceClaim(new Claim("amr", "mfa"));
            else
                claims.ReplaceClaim(new Claim("amr", "pwd"));

            if (string.IsNullOrEmpty(user.SecurityStamp))
                user.SecurityStamp = SequentialGuidGenerator.Instance.Create().ToString();

            claims.Add(new Claim(MiddlewareCoreConsts.TokenValidityValue, user.SecurityStamp));

            await _userManager.UpdateAsync(user);
            await _userManager.AddTokenValidityKeyAsync(user, tokenValidityKey, DateTime.UtcNow.Add(expiration).AddSeconds(10));
            await CurrentUnitOfWork.SaveChangesAsync();

            await _cacheManager
              .GetCache(MiddlewareCoreConsts.TokenValidityKey)
              .SetAsync(tokenValidityKey, user.SecurityStamp,
              slidingExpireTime: expiration,
              absoluteExpireTime: DateTimeOffset.UtcNow.Add(expiration).AddHours(1));

            try
            {
                await _userManager.UpdateAsync(user);
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "Error on Update User {0}", user.UserName);
            }

            return claims;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (!ProviderKeysAreEqual(model, userInfo))
            {
                Logger.DebugFormat("ProviderKey Invalid model {0} != {1}", model.ProviderKey, userInfo.ProviderKey);
                throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            return loginResult.Result switch
            {
                AbpLoginResultType.Success => loginResult,
                _ => throw _AbpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName),
            };
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private bool IsSchemeEnabled(ExternalLoginProviderInfo scheme)
        {
            if (!AbpSession.TenantId.HasValue && !string.IsNullOrEmpty(scheme.ClientId) && !string.IsNullOrEmpty(scheme.ClientSecret))
                return true;

            if (string.IsNullOrEmpty(scheme.ClientId) || string.IsNullOrEmpty(scheme.ClientSecret))
                return false;

            return scheme.Name switch
            {
                "OpenIdConnect" => _settingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled),
                "Microsoft" => _settingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled),
                "Google" => _settingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled),
                "AuthZero" => _settingManager.GetSettingValueForApplication<bool>(AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled),
                _ => true,
            };
        }

        private async Task<bool> IsTwoFactorAuthRequiredAsync(AbpLoginResult<Tenant, User> loginResult,
           AuthenticateModel authenticateModel)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.TwoFactorLogin
                .IsEnabled))
            {
                return false;
            }

            if (!loginResult.User.IsTwoFactorEnabled)
            {
                return false;
            }

            if ((await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User)).Count <= 0)
            {
                return false;
            }

            if (await TwoFactorClientRememberedAsync(loginResult.User.ToUserIdentifier(), authenticateModel))
            {
                return false;
            }

            return true;
        }

        [UnitOfWork]
        private async Task UpdateExternalUserAsync(User user, ExternalAuthUserInfo externalLoginInfo)
        {
            await UpdateExternalProfileAsync(user, externalLoginInfo);
            await UpdateExternalProfilePictureAsync(user, externalLoginInfo);
        }

        private async Task UpdateExternalProfileAsync(User user, ExternalAuthUserInfo externalLoginInfo)
        {
            string name = externalLoginInfo.Name.Split(' ')[0];
            string surname = externalLoginInfo.Surname ?? externalLoginInfo.Name.Split(' ')[^1];

            try
            {
                if (user.Name != name
                    || user.Surname != surname
                    || user.ExternalAuthProviderformation != externalLoginInfo.Provider)
                {
                    user.Name = name;
                    user.Surname = surname;
                    user.ExternalAuthProviderformation = externalLoginInfo.Provider;
                    user.AuthenticationSource = externalLoginInfo.Provider;
                    await _userManager.UpdateAsync(user);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Error on Update External Profile from user {0}", user.FullName);
            }
        }

        private async Task UpdateExternalProfilePictureAsync(User user, ExternalAuthUserInfo externalLoginInfo)
        {
            if (externalLoginInfo.Picture.IsNullOrEmpty())
                return;

            try
            {
                var contentType = ".bmp";
                var fileName = $"{Guid.NewGuid()}.bmp";
                BinaryObject storedFile = null;

                var byteArray = Convert.FromBase64String(externalLoginInfo.Picture);

                using (CurrentUnitOfWork.SetTenantId(null))
                {
                    bool savePicture = false;
                    if (user.ProfilePictureId.HasValue)
                    {
                        var profilePictureBinary = await _binaryObjectManager.GetOrNullAsync(user.ProfilePictureId.Value);
                        if (profilePictureBinary != null && !ByteArrayCompare(profilePictureBinary.Bytes, byteArray))
                        {
                            await _binaryObjectManager.DeleteAsync(user.ProfilePictureId.Value);
                            savePicture = true;
                        }
                    }
                    else
                        savePicture = true;

                    if (savePicture)
                    {
                        storedFile = new BinaryObject(null, byteArray, contentType, fileName);
                        await _binaryObjectManager.SaveAsync(storedFile);
                        await CurrentUnitOfWork.SaveChangesAsync();
                    }
                }

                if (storedFile != null)
                {
                    user.ProfilePictureId = storedFile.Id;
                    await CurrentUnitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "Error on Update External Profile Picture from user {0}", user.FullName);
            }
        }

        [UnitOfWork]
        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalLoginInfo)
        {
            var user = await GetOrCreateExternalUserAsync(externalLoginInfo);
            await SaveExternalProfilePictureAsync(user, externalLoginInfo);
            return user;
        }

        private async Task<User> GetOrCreateExternalUserAsync(ExternalAuthUserInfo externalLoginInfo)
        {
            string username;
            using (var providerManager = _iocManager.ResolveAsDisposable<DefaultExternalLoginInfoManager>())
            {
                username = providerManager.Object.GetUserNameFromExternalAuthUserInfo(externalLoginInfo);
            }

            var randomPassword = Authorization.Users.User.CreateRandomPassword();

            var userExist = await _userManager.FindByNameOrEmailAsync(username);
            if (userExist == null)
                userExist = await _userManager.FindByNameOrEmailAsync(externalLoginInfo.EmailAddress);

            if (userExist == null)
                return await CreateNewExternalUserAsync(externalLoginInfo, username, randomPassword);

            return await UpdateExistingExternalUserAsync(externalLoginInfo, username, userExist);
        }

        private async Task<User> CreateNewExternalUserAsync(ExternalAuthUserInfo externalLoginInfo, string username, string randomPassword)
        {
            Logger.DebugFormat("RegisterExternalUser Create {0}:{1} ", username, externalLoginInfo.EmailAddress);
            var user = new User
            {
                TenantId = AbpSession.TenantId,
                UserName = username,
                EmailAddress = externalLoginInfo.EmailAddress,
                Name = externalLoginInfo.Name.Split(' ')[0],
                Surname = externalLoginInfo.Surname ?? externalLoginInfo.Name.Split(' ')[^1],
            };

            user.Password = _passwordHasher.HashPassword(user, randomPassword);
            user.AuthenticationSource = externalLoginInfo.Provider;
            user.ExternalAuthProviderformation = externalLoginInfo.Provider;
            user.IsActive = true;
            user.IsTwoFactorEnabled = false;
            user.IsEmailConfirmed = true;
            user.IsLockoutEnabled = false;
            user.IsDeleted = false;
            user.TenantId = AbpSession?.TenantId;

            user.SetNormalizedNames();

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                    {
                        LoginProvider = externalLoginInfo.Provider,
                        ProviderKey = externalLoginInfo.ProviderKey,
                        TenantId = user.TenantId
                    }
            };

            if (user.Roles == null)
            {
                user.Roles = new List<UserRole>();
                foreach (var defaultRole in _roleManager.Roles.Where(r => r.TenantId == user.TenantId && r.IsDefault).ToList())
                {
                    user.Roles.Add(new UserRole(AbpSession?.TenantId, user.Id, defaultRole.Id));
                }
            }

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded && result.Errors.Any())
            {
                throw new UserFriendlyException(Convert.ToInt32(result.Errors.First().Code), result.Errors.First().Description);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await NotificationNewUser(user);
            await WelcomeToTheApplicationAsync(user);
            return user;
        }

        private async Task<User> UpdateExistingExternalUserAsync(ExternalAuthUserInfo externalLoginInfo, string username, User userExist)
        {
            Logger.DebugFormat("RegisterExternalUser Update {0}:{1} ", username, externalLoginInfo.EmailAddress);
            userExist.EmailAddress = externalLoginInfo.EmailAddress;
            userExist.Name = externalLoginInfo.Name.Split(' ')[0];
            userExist.Surname = externalLoginInfo.Surname ?? externalLoginInfo.Name.Split(' ')[^1];
            userExist.UserName = username;
            userExist.AuthenticationSource = externalLoginInfo.Provider;
            userExist.ExternalAuthProviderformation = externalLoginInfo.Provider;
            userExist.IsActive = true;
            userExist.IsTwoFactorEnabled = false;
            userExist.IsEmailConfirmed = true;
            userExist.IsLockoutEnabled = false;
            userExist.IsDeleted = false;
            userExist.TenantId = AbpSession?.TenantId;

            userExist.SetNormalizedNames();

            userExist.Logins = new List<UserLogin>
            {
                new UserLogin
                    {
                        LoginProvider = externalLoginInfo.Provider,
                        ProviderKey = externalLoginInfo.ProviderKey,
                        TenantId = userExist.TenantId
                    }
            };

            if (userExist.Roles == null)
            {
                userExist.Roles = new List<UserRole>();
                foreach (var defaultRole in _roleManager.Roles.Where(r => r.TenantId == userExist.TenantId && r.IsDefault).ToList())
                {
                    if (!await _userManager.IsInRoleAsync(userExist, defaultRole.NormalizedName))
                        userExist.Roles.Add(new UserRole(AbpSession?.TenantId, userExist.Id, defaultRole.Id));
                }
            }

            var result = await _userManager.UpdateWithValidateAsync(userExist);

            if (!result.Succeeded && result.Errors.Any())
            {
                throw new UserFriendlyException(Convert.ToInt32(result.Errors.First().Code), result.Errors.First().Description);
            }

            await CurrentUnitOfWork.SaveChangesAsync();
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(userExist.ToUserIdentifier());
            await NotificationNewUser(userExist);
            await WelcomeToTheApplicationAsync(userExist);
            return userExist;
        }

        private async Task SaveExternalProfilePictureAsync(User userExist, ExternalAuthUserInfo externalLoginInfo)
        {
            if (externalLoginInfo.Picture.IsNullOrEmpty())
                return;

            try
            {
                var contentType = ".bmp";
                var fileName = $"{Guid.NewGuid()}.bmp";
                BinaryObject storedFile;

                var byteArray = Convert.FromBase64String(externalLoginInfo.Picture);

                using (CurrentUnitOfWork.SetTenantId(null))
                {
                    storedFile = new BinaryObject(null, byteArray, contentType, fileName);
                    if (userExist.ProfilePictureId.HasValue)
                        await _binaryObjectManager.DeleteAsync(userExist.ProfilePictureId.Value);
                    await _binaryObjectManager.SaveAsync(storedFile);
                    await CurrentUnitOfWork.SaveChangesAsync();
                }

                userExist.ProfilePictureId = storedFile.Id;
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "Error on Update External Profile Picture from user {0}", userExist.FullName);
            }
        }

        private async Task<string> TwoFactorAuthenticateAsync(User user, AuthenticateModel authenticateModel)
        {
            var twoFactorCodeCache = _cacheManager.GetTwoFactorCodeCache();
            var userIdentifier = user.ToUserIdentifier().ToString();
            var cachedCode = await twoFactorCodeCache.GetOrDefaultAsync(userIdentifier);

            if (cachedCode?.Code == null || cachedCode.Code != authenticateModel.TwoFactorVerificationCode)
            {
                throw new UserFriendlyException(L("InvalidSecurityCode"));
            }

            //Delete from the cache since it was a single usage code
            await twoFactorCodeCache.RemoveAsync(userIdentifier);

            if (authenticateModel.RememberClient &&
                await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
            {
                return CreateAccessToken(new[]
                    {
                            new Claim(EafClaimTypes.UserIdentifierClaimType, user.ToUserIdentifier().ToString())
                        },
                    TimeSpan.FromDays(365)
                );
            }

            return null;
        }

        private async Task<bool> TwoFactorClientRememberedAsync(UserIdentifier userIdentifier,
            AuthenticateModel authenticateModel)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticateModel.TwoFactorRememberClientToken))
            {
                return false;
            }

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = _configuration.Audience,
                    ValidIssuer = _configuration.Issuer,
                    IssuerSigningKey = _configuration.SecurityKey
                };

                foreach (var validator in _jwtOptions.Value.SecurityTokenValidators
                    .Where(v => v.CanReadToken(authenticateModel.TwoFactorRememberClientToken)))
                {
                    try
                    {
                        var principal = validator.ValidateToken(authenticateModel.TwoFactorRememberClientToken, validationParameters, out _);
                        var useridentifierClaim = principal.FindFirst(c => c.Type == EafClaimTypes.UserIdentifierClaimType);
                        if (useridentifierClaim == null)
                        {
                            return false;
                        }

                        return useridentifierClaim.Value == userIdentifier.ToString();
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString(), ex);
            }

            return false;
        }

        private bool UseCaptchaOnLogin()
        {
            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnLogin);
        }

        private async Task ValidateReCaptcha(string captchaResponse)
        {
            await RecaptchaValidator.ValidateAsync(captchaResponse);
        }

        private async Task NotificationNewUser(User user)
        {
            try
            {
                await _notificationPublisher.PublishAsync(
                  MiddlewareNotificationNames.NewUserRegistered,
                  new MessageNotificationData(L("NewUserRegistered", user.FullName)),
                  severity: NotificationSeverity.Info,
                  tenantIds: new[] { user.TenantId }
                  );
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "NotificationPublisher error {0}", ex.Message);
            }
            try
            {
                await _webhookPublisher.PublishAsync(EafWebHookNames.NewUserRegistered, user);
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "WebhookPublisher error {0}", ex.Message);
            }
        }

        private async Task WelcomeToTheApplicationAsync(User user)
        {
            await _notificationPublisher.PublishAsync(
               MiddlewareNotificationNames.WelcomeToTheApplication,
               new MessageNotificationData(L("WelcomeToTheApplicationNotificationMessage")),
               severity: NotificationSeverity.Success,
               userIds: new[] { user.ToUserIdentifier() }
               );
        }

        private static bool ByteArrayCompare(byte[]? a1, byte[]? a2)
        {
            try
            {
                if (a1 == null || a2 == null)
                    return false;

                if (a1.Length != a2.Length)
                    return false;

                for (int i = 0; i < a1.Length; i++)
                {
                    if (a1[i] != a2[i])
                        return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static async Task<AuthenticationResult> GetAccessTokenOnBehalfUserAsync(string idToken,
            string clientId,
            string clientSecret,
            string tenantId)
        {
            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder.Create(clientId)
                                            .WithClientSecret(clientSecret)
                                            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
                                            .Build();
            UserAssertion assert = new UserAssertion(idToken);
            List<string> scopes = new List<string>
            {
                "https://graph.microsoft.com/User.Read"
            };
            // Acquires an access token for this application (usually a Web API) from the authority configured in the application.
            var responseToken = await app.AcquireTokenOnBehalfOf(scopes, assert).ExecuteAsync();
            return responseToken;
        }

        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<AuthenticateResultModel> Refresh()
        {
            var refreshTokenValue = Request.Cookies["Eaf.RefreshToken"];
            if (string.IsNullOrEmpty(refreshTokenValue))
            {
                throw new UserFriendlyException(L("RefreshTokenIsMissing"));
            }

            var refreshToken = await _refreshTokenStore.GetAsync(refreshTokenValue);
            if (refreshToken == null || refreshToken.ExpireDate < DateTime.UtcNow)
            {
                throw new UserFriendlyException(L("InvalidRefreshToken"));
            }

            var user = await _userManager.GetUserByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                throw new UserFriendlyException(L("UserNotFound"));
            }

            var currentSecurityStamp = await _userManager.GetSecurityStampAsync(user);
            if (refreshToken.SecurityStamp != currentSecurityStamp)
            {
                throw new UserFriendlyException(L("InvalidRefreshToken"));
            }

            await _refreshTokenStore.RemoveAsync(refreshTokenValue);

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(_identityOptions.ClaimsIdentity.UserIdClaimType, user.Id.ToString()));
            identity.AddClaim(new Claim(_identityOptions.ClaimsIdentity.UserNameClaimType, user.UserName));
            identity.AddClaim(new Claim(EafClaimTypes.UserIdentifierClaimType, new UserIdentifier(AbpSession.TenantId, user.Id).ToUserIdentifierString()));

            var expirationSettings = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
            var expiration = TimeSpan.FromSeconds(expirationSettings);

            var accessToken = CreateAccessToken(await CreateJwtClaims(identity, user), expiration);
            var newRefreshToken = await GenerateAndStoreRefreshTokenAsync(user);
            AppendRefreshTokenCookie(newRefreshToken.Token, newRefreshToken.ExpireDate);

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int)expiration.TotalSeconds,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                UserId = user.Id
            };
        }

        private async Task<int> GetRefreshTokenExpirationDaysAsync()
        {
            try
            {
                var value = await SettingManager.GetSettingValueAsync(AppSettings.UserManagement.RefreshTokenExpirationInDays);
                if (int.TryParse(value, out var days))
                    return days;
            }
            catch (FormatException)
            {
                // Valor não numérico configurado (ex.: "false" em testes/mock).
            }

            return 7;
        }

        private async Task<RefreshTokenInfo> GenerateAndStoreRefreshTokenAsync(User user)
        {
            var refreshTokenExpirationDays = await GetRefreshTokenExpirationDaysAsync();
            var refreshTokenExpiration = TimeSpan.FromDays(refreshTokenExpirationDays > 0 ? refreshTokenExpirationDays : 7);

            var securityStamp = await _userManager.GetSecurityStampAsync(user);
            var refreshToken = new RefreshTokenInfo
            {
                Token = GenerateRefreshTokenValue(),
                UserId = user.Id,
                TenantId = AbpSession.TenantId,
                SecurityStamp = securityStamp,
                ExpireDate = DateTime.UtcNow.Add(refreshTokenExpiration)
            };

            await _refreshTokenStore.SetAsync(refreshToken);
            return refreshToken;
        }

        private static string GenerateRefreshTokenValue()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private void AppendRefreshTokenCookie(string token, DateTime expireDate)
        {
            if (Response == null)
                return;

            Response.Cookies.Append(
                "Eaf.RefreshToken",
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Lax,
                    Expires = new DateTimeOffset(expireDate),
                    Path = "/api/TokenAuth"
                }
            );
        }
    }
}