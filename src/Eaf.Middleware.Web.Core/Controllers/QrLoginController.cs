using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.RealTime;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Castle.Core.Logging;
using Eaf.Middleware.Authentication.QrLogin;
using Eaf.Middleware.Authentication.Sessions;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Authentication;
using Eaf.Middleware.Web.Authentication.Identity;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Eaf.Middleware.Web.Models.TokenAuth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Login por QR code: o browser anônimo exibe um QR com o id do token;
    /// um dispositivo autenticado aprova; o polling do browser consome e
    /// estabelece a sessão com o IP/UA do browser exibidor.
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class QrLoginController : TokenAuthController
    {
        private readonly IQrLoginManager _qrLoginManager;
        private readonly ISettingManager _qrSettingManager;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager _qrUserManager;
        private readonly IOptions<IdentityOptions> _qrIdentityOptions;

        /// <summary>
        /// QrLoginController.
        /// </summary>
        public QrLoginController( // NOSONAR — ctor espelha o da base + deps de QR login
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
            IRefreshTokenStore refreshTokenStore,
            ISessionRegistry sessionRegistry,
            IQrLoginManager qrLoginManager,
            IAbpSession abpSession,
            IUnitOfWorkManager unitOfWorkManager)
            : base(logInManager, AbpLoginResultTypeHelper, configuration, userManager, roleManager, tenantCache,
                   cacheManager, impersonationManager, identityOptions, logger, settingManager, externalAuthManager,
                   externalAuthConfiguration, iocManager, passwordHasher, emailSender, jwtOptions, notificationPublisher,
                   binaryObjectManager, notificationSubscriptionManager, webhookPublisher, principalAccessor, refreshTokenStore, sessionRegistry)
        {
            _qrLoginManager = qrLoginManager;
            _qrSettingManager = settingManager;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
            _qrUserManager = userManager;
            _qrIdentityOptions = identityOptions;
        }

        /// <summary>Indica se o QR login está habilitado.</summary>
        [AbpAllowAnonymous]
        [HttpGet]
        public async Task<bool> IsEnabled()
        {
            return await IsQrLoginEnabledAsync();
        }

        /// <summary>Resultado de <see cref="Create"/>.</summary>
        public class QrLoginCreateResult
        {
            /// <summary>Id do token (conteúdo do QR).</summary>
            public string TokenId { get; set; }

            /// <summary>TTL em segundos.</summary>
            public int ExpiresInSeconds { get; set; }
        }

        /// <summary>Cria um token pendente para o QR exibido.</summary>
        [AbpAllowAnonymous]
        [HttpPost]
        public async Task<QrLoginCreateResult> Create()
        {
            await EnsureQrLoginEnabledAsync();

            var token = await _qrLoginManager.CreateAsync();
            return new QrLoginCreateResult { TokenId = token.TokenId, ExpiresInSeconds = QrLoginManager.TokenTimeToLiveSeconds };
        }

        /// <summary>Input de <see cref="Status"/> e <see cref="Approve"/>.</summary>
        public class QrLoginTokenInput
        {
            /// <summary>Id do token.</summary>
            public string TokenId { get; set; }
        }

        /// <summary>Resultado de <see cref="Status"/>: "pending" | "expired" | "approved".</summary>
        public class QrLoginStatusResult
        {
            /// <summary>pending | expired | approved.</summary>
            public string Status { get; set; }

            /// <summary>Resultado de autenticação quando aprovado.</summary>
            public AuthenticateResultModel AuthResult { get; set; }
        }

        /// <summary>
        /// Polling do browser exibidor. Quando aprovado, consome o token (single-use)
        /// e emite JWT + refresh bound à request do browser (IP + UA).
        /// </summary>
        [AbpAllowAnonymous]
        [HttpGet]
        public async Task<QrLoginStatusResult> Status(string tokenId)
        {
            await EnsureQrLoginEnabledAsync();

            var token = await _qrLoginManager.GetOrNullAsync(tokenId);
            if (token == null || token.Status == QrLoginStatus.Consumed)
            {
                return new QrLoginStatusResult { Status = "expired" };
            }

            if (token.Status == QrLoginStatus.Pending)
            {
                return new QrLoginStatusResult { Status = "pending" };
            }

            token = await _qrLoginManager.ConsumeAsync(tokenId);
            if (token == null)
            {
                return new QrLoginStatusResult { Status = "expired" };
            }

            var authResult = await IssueSessionAsync(token);
            return new QrLoginStatusResult { Status = "approved", AuthResult = authResult };
        }

        /// <summary>Aprovação pelo dispositivo autenticado que escaneou o QR.</summary>
        [AbpAuthorize]
        [HttpPost]
        public async Task Approve([FromBody] QrLoginTokenInput input)
        {
            await EnsureQrLoginEnabledAsync();

            var approved = await _qrLoginManager.ApproveAsync(input.TokenId, _abpSession.UserId.Value, _abpSession.TenantId);
            if (!approved)
            {
                throw new UserFriendlyException(L("QrLoginTokenExpired"));
            }
        }

        private async Task<AuthenticateResultModel> IssueSessionAsync(QrLoginToken token)
        {
            User user;
            using (_unitOfWorkManager.Current.SetTenantId(token.ApprovedTenantId))
            {
                user = await _qrUserManager.FindByIdAsync(token.ApprovedUserId.Value.ToString());
            }

            if (user == null)
            {
                throw new UserFriendlyException(L("QrLoginTokenExpired"));
            }

            var expirationSettings = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
            var expiration = TimeSpan.FromSeconds(expirationSettings);

            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(_qrIdentityOptions.Value.ClaimsIdentity.UserIdClaimType, user.Id.ToString()));
            identity.AddClaim(new Claim(_qrIdentityOptions.Value.ClaimsIdentity.UserNameClaimType, user.UserName));

            var refreshToken = await GenerateAndStoreRefreshTokenAsync(user);
            var accessToken = CreateAccessToken(await CreateJwtClaims(identity, user, "qr", sessionId: refreshToken.SessionId), expiration);
            AppendRefreshTokenCookie(refreshToken.Token, refreshToken.ExpireDate);

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                ExpireInSeconds = (int)expiration.TotalSeconds,
                UserId = user.Id,
            };
        }

        private async Task EnsureQrLoginEnabledAsync()
        {
            if (!await IsQrLoginEnabledAsync())
            {
                throw new UserFriendlyException(L("QrLoginDisabled"));
            }
        }

        private async Task<bool> IsQrLoginEnabledAsync()
        {
            var value = await _qrSettingManager.GetSettingValueAsync(AppSettings.UserManagement.QrLogin.IsEnabled);
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
