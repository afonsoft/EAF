using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.RealTime;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Webhooks;
using Eaf.Middleware.Authentication.Passkeys;
using Eaf.Middleware.Authentication.Sessions;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Authorization.Impersonation;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Web.Authentication;
using Abp.MultiTenancy;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Storage;
using Eaf.Middleware.Web.Authentication.JwtBearer;
using Eaf.Middleware.Web.Models.TokenAuth;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Castle.Core.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eaf.Middleware.Web.Controllers
{
    /// <summary>
    /// Endpoints de passkey (WebAuthn): registro, login sem senha e gerenciamento.
    /// Herda de <see cref="TokenAuthController"/> para reutilizar a emissão de JWT
    /// (TokenValidityKey/SecurityStamp) e o registro de login attempts.
    /// </summary>
    [AbpAllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class PasskeyController : TokenAuthController
    {
        private const string CeremonyCacheName = "EafPasskeyCeremony";

        private readonly ICacheManager _passkeyCacheManager;
        private readonly IConfiguration _appConfiguration;
        private readonly IPasskeyManager _passkeyManager;
        private readonly ISettingManager _passkeySettingManager;
        private readonly IIocManager _passkeyIocManager;
        private readonly UserManager _passkeyUserManager;
        private readonly IAbpSession _abpSession;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        /// <summary>
        /// PasskeyController.
        /// </summary>
        public PasskeyController( // NOSONAR — ctor espelha o da base + deps de passkey
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
            IPasskeyManager passkeyManager,
            IConfiguration appConfiguration,
            IAbpSession abpSession,
            IUnitOfWorkManager unitOfWorkManager)
            : base(logInManager, AbpLoginResultTypeHelper, configuration, userManager, roleManager, tenantCache,
                   cacheManager, impersonationManager, identityOptions, logger, settingManager, externalAuthManager,
                   externalAuthConfiguration, iocManager, passwordHasher, emailSender, jwtOptions, notificationPublisher,
                   binaryObjectManager, notificationSubscriptionManager, webhookPublisher, principalAccessor, refreshTokenStore, sessionRegistry)
        {
            _passkeyCacheManager = cacheManager;
            _passkeyManager = passkeyManager;
            _passkeySettingManager = settingManager;
            _passkeyIocManager = iocManager;
            _passkeyUserManager = userManager;
            _appConfiguration = appConfiguration;
            _abpSession = abpSession;
            _unitOfWorkManager = unitOfWorkManager;
        }

        // ---------- models ----------

        /// <summary>Input de opções de registro.</summary>
        public class PasskeyRegisterOptionsInput
        {
            /// <summary>Nome amigável do dispositivo.</summary>
            public string Name { get; set; }
        }

        /// <summary>Input de finalização do registro.</summary>
        public class PasskeyRegisterInput
        {
            /// <summary>Id da cerimônia retornado por RegisterOptions.</summary>
            public string CeremonyId { get; set; }

            /// <summary>Nome amigável.</summary>
            public string Name { get; set; }

            /// <summary>Resposta do navigator.credentials.create().</summary>
            public AuthenticatorAttestationRawResponse Response { get; set; }
        }

        /// <summary>Input de opções de login.</summary>
        public class PasskeyAssertionOptionsInput
        {
            /// <summary>Opcional: restringe às credenciais do usuário.</summary>
            public string UserNameOrEmailAddress { get; set; }

            /// <summary>Tenancy do usuário (para credenciais por tenant).</summary>
            public string TenancyName { get; set; }
        }

        /// <summary>Input de finalização do login.</summary>
        public class PasskeyAuthenticateInput
        {
            /// <summary>Id da cerimônia retornado por AssertionOptions.</summary>
            public string CeremonyId { get; set; }

            /// <summary>Resposta do navigator.credentials.get().</summary>
            public AuthenticatorAssertionRawResponse Response { get; set; }
        }

        /// <summary>DTO de listagem de passkeys do usuário.</summary>
        public class PasskeyDto
        {
            /// <summary>Id da entidade.</summary>
            public long Id { get; set; }

            /// <summary>Nome amigável.</summary>
            public string Name { get; set; }

            /// <summary>Transports da credencial.</summary>
            public string Transports { get; set; }

            /// <summary>Se a credencial é syncable/backup.</summary>
            public bool IsBackupEligible { get; set; }

            /// <summary>Último uso UTC.</summary>
            public DateTime? LastUsedTime { get; set; }

            /// <summary>Criação UTC.</summary>
            public DateTime CreationTime { get; set; }
        }

        // ---------- registration (authenticated) ----------

        /// <summary>
        /// Inicia a cerimônia de registro: gera CredentialCreateOptions (challenge)
        /// e armazena em cache de curta duração, retornando o id da cerimônia.
        /// </summary>
        [AbpAuthorize]
        [HttpPost]
        public async Task<object> RegisterOptions([FromBody] PasskeyRegisterOptionsInput input)
        {
            await EnsurePasskeysEnabledAsync();

            var user = await _passkeyUserManager.GetUserByIdAsync(_abpSession.GetUserId());
            var existing = await _passkeyManager.GetForUserAsync(
                new UserIdentifier(_abpSession.TenantId, user.Id));

            var fidoUser = new Fido2User
            {
                Id = BitConverter.GetBytes(user.Id),
                Name = user.UserName,
                DisplayName = string.IsNullOrWhiteSpace(user.Name) ? user.UserName : $"{user.Name} {user.Surname}".Trim()
            };

            var exclude = existing
                .Select(k => new PublicKeyCredentialDescriptor(Convert.FromBase64String(Base64UrlToBase64(k.CredentialId))))
                .ToList();

            var fido = CreateFido();
            var options = fido.RequestNewCredential(new RequestNewCredentialParams
            {
                User = fidoUser,
                ExcludeCredentials = exclude,
                AuthenticatorSelection = new AuthenticatorSelection
                {
                    ResidentKey = ResidentKeyRequirement.Required,
                    UserVerification = UserVerificationRequirement.Preferred
                },
                AttestationPreference = AttestationConveyancePreference.None,
                Extensions = new AuthenticationExtensionsClientInputs()
            });

            var ceremonyId = Guid.NewGuid().ToString("N");
            await CeremonyCache().SetAsync(ceremonyId, options.ToJson(), TimeSpan.FromMinutes(10));

            return new { ceremonyId, options = JsonDocument.Parse(options.ToJson()).RootElement };
        }

        /// <summary>
        /// Conclui a cerimônia de registro verificando a attestation e persistindo a credencial.
        /// </summary>
        [AbpAuthorize]
        [HttpPost]
        public async Task<PasskeyDto> Register([FromBody] PasskeyRegisterInput input)
        {
            await EnsurePasskeysEnabledAsync();
            var options = await TakeCeremonyOptionsAsync<CredentialCreateOptions>(input.CeremonyId);

            var fido = CreateFido();
            var userId = _abpSession.GetUserId();
            var credential = await fido.MakeNewCredentialAsync(new MakeNewCredentialParams
            {
                AttestationResponse = input.Response,
                OriginalOptions = options,
                IsCredentialIdUniqueToUserCallback = async (args, ct) =>
                {
                    // Rejeita se a credencial já existir para QUALQUER usuário.
                    var existing = await _passkeyManager.FindByCredentialIdAsync(
                        Base64ToBase64Url(Convert.ToBase64String(args.CredentialId)));
                    return existing == null;
                }
            });

            if (credential == null)
            {
                throw new UserFriendlyException(L("PasskeyRegistrationFailed"));
            }

            var entity = new UserPasskey
            {
                TenantId = _abpSession.TenantId,
                UserId = userId,
                Name = string.IsNullOrWhiteSpace(input.Name) ? L("PasskeyDefaultName") : input.Name.Trim(),
                CredentialId = Base64ToBase64Url(Convert.ToBase64String(credential.Id)),
                PublicKey = credential.PublicKey,
                SignCount = credential.SignCount,
                Transports = credential.Transports == null ? null : string.Join(",", credential.Transports.Select(t => t.ToString())),
                AaGuid = credential.AaGuid.ToString(),
                IsBackupEligible = credential.IsBackupEligible,
                AttestationData = credential.AttestationObject == null
                    ? null
                    : Truncate(Convert.ToBase64String(credential.AttestationObject), UserPasskey.MaxAttestationDataLength)
            };

            await _passkeyManager.AddAsync(entity);
            return ToDto(entity);
        }

        /// <summary>Lista os passkeys do usuário autenticado.</summary>
        [AbpAuthorize]
        [HttpGet]
        public async Task<List<PasskeyDto>> GetMyPasskeys()
        {
            var keys = await _passkeyManager.GetForUserAsync(
                new UserIdentifier(_abpSession.TenantId, _abpSession.GetUserId()));
            return keys.Select(ToDto).ToList();
        }

        /// <summary>Remove um passkey do usuário autenticado.</summary>
        [AbpAuthorize]
        [HttpPost]
        public async Task Delete([FromBody] PasskeyDeleteInput input)
        {
            await _passkeyManager.DeleteAsync(
                new UserIdentifier(_abpSession.TenantId, _abpSession.GetUserId()), input.Id);
        }

        /// <summary>Input de remoção.</summary>
        public class PasskeyDeleteInput
        {
            /// <summary>Id do passkey.</summary>
            public long Id { get; set; }
        }

        // ---------- assertion / login (anonymous) ----------

        /// <summary>
        /// Inicia a cerimônia de login: gera AssertionOptions (challenge).
        /// Se username for informado, restringe às credenciais daquele usuário.
        /// </summary>
        [HttpPost]
        public async Task<object> AssertionOptions([FromBody] PasskeyAssertionOptionsInput input)
        {
            await EnsurePasskeysEnabledAsync();

            var allowed = new List<PublicKeyCredentialDescriptor>();
            if (!string.IsNullOrWhiteSpace(input?.UserNameOrEmailAddress))
            {
                var tenant = await GetTenantIdOrNull(input.TenancyName);
                var user = await FindUserForAssertionAsync(input.UserNameOrEmailAddress, tenant);
                if (user != null)
                {
                    var keys = await _passkeyManager.GetForUserAsync(
                        new UserIdentifier(user.TenantId, user.Id));
                    allowed = keys
                        .Select(k => new PublicKeyCredentialDescriptor(Convert.FromBase64String(Base64UrlToBase64(k.CredentialId))))
                        .ToList();
                }
            }

            var fido = CreateFido();
            var options = fido.GetAssertionOptions(new GetAssertionOptionsParams
            {
                AllowedCredentials = allowed,
                UserVerification = UserVerificationRequirement.Preferred,
                Extensions = new AuthenticationExtensionsClientInputs()
            });

            var ceremonyId = Guid.NewGuid().ToString("N");
            await CeremonyCache().SetAsync(ceremonyId, options.ToJson(), TimeSpan.FromMinutes(10));

            return new { ceremonyId, options = JsonDocument.Parse(options.ToJson()).RootElement };
        }

        /// <summary>
        /// Conclui a cerimônia de login: verifica a assinatura e emite o par de tokens JWT.
        /// </summary>
        [HttpPost]
        [UnitOfWork]
        public async Task<AuthenticateResultModel> AuthenticatePasskey([FromBody] PasskeyAuthenticateInput input)
        {
            await EnsurePasskeysEnabledAsync();
            var options = await TakeCeremonyOptionsAsync<AssertionOptions>(input.CeremonyId);

            var credentialIdB64 = Convert.ToBase64String(input.Response.RawId);
            var passkey = await _passkeyManager.FindByCredentialIdAsync(Base64ToBase64Url(credentialIdB64));
            if (passkey == null)
            {
                throw new UserFriendlyException(L("PasskeyNotRegistered"));
            }

            var fido = CreateFido();
            // Fido2 4.x lança exceção em falha de verificação (Status sempre 'ok' em sucesso).
            Fido2NetLib.Objects.VerifyAssertionResult verification;
            try
            {
                verification = await fido.MakeAssertionAsync(new MakeAssertionParams
            {
                AssertionResponse = input.Response,
                OriginalOptions = options,
                StoredPublicKey = passkey.PublicKey,
                StoredSignatureCounter = passkey.SignCount,
                IsUserHandleOwnerOfCredentialIdCallback = (args, ct) =>
                {
                    var handleUserId = BitConverter.ToInt64(args.UserHandle, 0);
                    return Task.FromResult(handleUserId == passkey.UserId);
                }
                });
            }
            catch (Exception ex)
            {
                Logger.Warn("Passkey assertion failed: " + ex.Message);
                throw new UserFriendlyException(L("PasskeyAssertionFailed"));
            }

            passkey.SignCount = verification.SignCount;
            passkey.LastUsedTime = DateTime.UtcNow;
            await _passkeyManager.UpdateAsync(passkey);

            var user = await _passkeyUserManager.FindByIdAsync(passkey.UserId.ToString());
            if (user == null || !user.IsActive)
            {
                throw new UserFriendlyException(L("UserIsNotActive"));
            }

            // Emite JWT idêntico ao fluxo de senha (TokenValidityKey/SecurityStamp/amr).
            ClaimsIdentity identity;
            using (var principalFactory = _passkeyIocManager.ResolveAsDisposable<UserClaimsPrincipalFactory>())
            {
                var principal = await principalFactory.Object.CreateAsync(user);
                identity = (ClaimsIdentity)principal.Identity;
            }

            var expirationSettings = await _passkeySettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.TokenExpiration);
            var expiration = TimeSpan.FromSeconds(expirationSettings);

            var refreshToken = await GenerateAndStoreRefreshTokenAsync(user);
            var accessToken = CreateAccessToken(await CreateJwtClaims(identity, user, "passkey", refreshToken.SessionId), expiration);
            AppendRefreshTokenCookie(refreshToken.Token, refreshToken.ExpireDate);

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                ExpireInSeconds = (int)expiration.TotalSeconds,
                EncryptedAccessToken = GetEncryptedAccessToken(accessToken),
                UserId = user.Id
            };
        }

        // ---------- helpers ----------

        private async Task EnsurePasskeysEnabledAsync()
        {
            var enabled = await _passkeySettingManager.GetSettingValueAsync(AppSettings.UserManagement.Passkeys.IsEnabled);
            if (!bool.TryParse(enabled, out var isEnabled) || !isEnabled)
            {
                throw new UserFriendlyException(L("PasskeysDisabled"));
            }
        }

        private Fido2 CreateFido()
        {
            var host = Request?.Host.Host ?? "localhost";
            var origin = $"{Request?.Scheme ?? "https"}://{Request?.Host.ToString() ?? host}";
            var configuredRpId = _appConfiguration["Authentication:Passkeys:RelyingPartyId"];
            var rpId = string.IsNullOrWhiteSpace(configuredRpId) ? host.Split(':')[0] : configuredRpId;

            var config = new Fido2Configuration
            {
                ServerDomain = rpId,
                ServerName = "EAF",
                Origins = new HashSet<string> { origin },
                TimestampDriftTolerance = 300000
            };
            return new Fido2(config);
        }

        private ICache CeremonyCache() => _passkeyCacheManager.GetCache(CeremonyCacheName);

        private async Task<T> TakeCeremonyOptionsAsync<T>(string ceremonyId)
        {
            if (string.IsNullOrWhiteSpace(ceremonyId))
            {
                throw new UserFriendlyException(L("PasskeyCeremonyInvalid"));
            }
            var cache = CeremonyCache();
            var json = (await cache.GetOrDefaultAsync(ceremonyId)) as string;
            if (string.IsNullOrEmpty(json))
            {
                throw new UserFriendlyException(L("PasskeyCeremonyExpired"));
            }
            await cache.RemoveAsync(ceremonyId);
            return JsonSerializer.Deserialize<T>(json);
        }

        private async Task<int?> GetTenantIdOrNull(string tenancyName)
        {
            if (string.IsNullOrWhiteSpace(tenancyName))
            {
                return null;
            }
            using (var tm = _passkeyIocManager.ResolveAsDisposable<TenantManager>())
            {
                var tenant = await tm.Object.FindByTenancyNameAsync(tenancyName);
                return tenant?.Id;
            }
        }

        private async Task<User> FindUserForAssertionAsync(string usernameOrEmail, int? tenantId)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId, switchMustHaveTenantEnableDisable: false))
            using (_unitOfWorkManager.Current.EnableFilter(AbpDataFilters.MayHaveTenant))
            {
                return await _passkeyUserManager.FindByNameOrEmailAsync(usernameOrEmail);
            }
        }

        private static PasskeyDto ToDto(UserPasskey k) => new PasskeyDto
        {
            Id = k.Id,
            Name = k.Name,
            Transports = k.Transports,
            IsBackupEligible = k.IsBackupEligible,
            LastUsedTime = k.LastUsedTime,
            CreationTime = k.CreationTime
        };

        private static string Truncate(string value, int max) =>
            value != null && value.Length > max ? value[..max] : value;

        private static string Base64ToBase64Url(string b64) =>
            b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');

        private static string Base64UrlToBase64(string b64url)
        {
            var s = b64url.Replace('-', '+').Replace('_', '/');
            return s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
        }
    }
}
