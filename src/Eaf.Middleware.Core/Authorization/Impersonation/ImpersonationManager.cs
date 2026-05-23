using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Eaf.Middleware.Authorization.Users;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Eaf.Middleware.Authorization.Impersonation
{
    /// <summary>
    /// Representa a classe ImpersonationManager.
    /// </summary>
    public class ImpersonationManager : DomainService, IImpersonationManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly UserClaimsPrincipalFactory _principalFactory;
        private readonly UserManager _userManager;
        private readonly IRepository<UserToken, long> _userTokenRepository;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// ImpersonationManager.
        /// </summary>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <param name="userManager">Parâmetro userManager.</param>
        /// <param name="principalFactory">Parâmetro principalFactory.</param>
        /// <param name="userTokenRepository">Parâmetro userTokenRepository.</param>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public ImpersonationManager(
            ICacheManager cacheManager,
            UserManager userManager,
            UserClaimsPrincipalFactory principalFactory,
            IRepository<UserToken, long> userTokenRepository,
            ISettingManager settingManager)
        {
            _cacheManager = cacheManager;
            _userManager = userManager;
            _principalFactory = principalFactory;
            _userTokenRepository = userTokenRepository;
            _settingManager = settingManager;

            AbpSession = NullAbpSession.Instance;
            LocalizationSourceName = Localization.MiddlewareLocalizationHelper.DefaultSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        protected override string L(string name)
        {
            return Localization.MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        protected override string L(string name, params object[] args)
        {
            return Localization.MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        protected override string L(string name, CultureInfo culture)
        {
            return Localization.MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        /// <summary>
        /// Obtém ou define AbpSession.
        /// </summary>
        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// GetBackToImpersonatorToken.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public Task<string> GetBackToImpersonatorToken()
        {
            if (!AbpSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(L("NotImpersonatedLoginErrorMessage"));
            }

            return GenerateImpersonationTokenAsync(AbpSession.ImpersonatorTenantId, AbpSession.ImpersonatorUserId.Value, true);
        }

        /// <summary>
        /// GetImpersonatedUserAndIdentity.
        /// </summary>
        /// <param name="impersonationToken">Parâmetro impersonationToken.</param>
        /// <returns>Resultado da operação.</returns>
        public async Task<UserAndIdentity> GetImpersonatedUserAndIdentity(string impersonationToken)
        {
            var cacheItem = await _cacheManager.GetImpersonationCache().GetOrDefaultAsync(impersonationToken);
            if (cacheItem == null)
            {
                var userToken = await _userTokenRepository.FirstOrDefaultAsync(t => t.Name == impersonationToken && t.ExpireDate >= Clock.Now && t.LoginProvider == "TokenValidAdminLogin");

                if (userToken == null)
                    throw new UserFriendlyException(L("ImpersonationTokenErrorMessage"));

                cacheItem = new ImpersonationCacheItem()
                {
                    TargetTenantId = userToken.TenantId,
                    TargetUserId = userToken.UserId,
                    IsBackToImpersonator = string.IsNullOrEmpty(userToken.Value),
                    ImpersonatorTenantId = !string.IsNullOrEmpty(userToken.Value) ? Convert.ToInt32(userToken.Value.Split("-").First()) : null,
                    ImpersonatorUserId = !string.IsNullOrEmpty(userToken.Value) ? Convert.ToInt32(userToken.Value.Split("-").Last()) : 2
                };
            }

            CheckCurrentTenant(cacheItem.TargetTenantId);

            //Get the user from tenant
            var user = await _userManager.FindByIdAsync(cacheItem.TargetUserId.ToString());

            //Create identity

            var identity = (ClaimsIdentity)(await _principalFactory.CreateAsync(user)).Identity;

            if (!cacheItem.IsBackToImpersonator)
            {
                //Add claims for audit logging
                if (cacheItem.ImpersonatorTenantId.HasValue)
                {
                    identity.AddClaim(new Claim(AbpClaimTypes.ImpersonatorTenantId, cacheItem.ImpersonatorTenantId.Value.ToString(CultureInfo.InvariantCulture)));
                }

                identity.AddClaim(new Claim(AbpClaimTypes.ImpersonatorUserId, cacheItem.ImpersonatorUserId.ToString(CultureInfo.InvariantCulture)));
            }

            //Remove the cache item to prevent re-use
            await _cacheManager.GetImpersonationCache().RemoveAsync(impersonationToken);

            return new UserAndIdentity(user, identity);
        }

        /// <summary>
        /// GetImpersonationToken.
        /// </summary>
        /// <param name="userId">Parâmetro userId.</param>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<string> GetImpersonationToken(long userId, int? tenantId)
        {
            if (AbpSession.ImpersonatorUserId.HasValue)
            {
                throw new UserFriendlyException(L("CascadeImpersonationErrorMessage"));
            }

            if (AbpSession.TenantId.HasValue)
            {
                if (!tenantId.HasValue)
                {
                    throw new UserFriendlyException(L("FromTenantToHostImpersonationErrorMessage"));
                }

                if (tenantId.Value != AbpSession.TenantId.Value)
                {
                    throw new UserFriendlyException(L("DifferentTenantImpersonationErrorMessage"));
                }
            }

            return GenerateImpersonationTokenAsync(tenantId, userId, false);
        }

        private void CheckCurrentTenant(int? tenantId)
        {
            if (AbpSession.TenantId != tenantId)
            {
                throw new UserFriendlyException($"Current tenant is different than given tenant. AbpSession.TenantId: {AbpSession.TenantId}, given tenantId: {tenantId}");
            }
        }

        private async Task<string> GenerateImpersonationTokenAsync(int? tenantId, long userId, bool isBackToImpersonator)
        {
            //Create a cache item
            var cacheItem = new ImpersonationCacheItem(
                tenantId,
                userId,
                isBackToImpersonator
            );

            if (!isBackToImpersonator)
            {
                cacheItem.ImpersonatorTenantId = AbpSession.TenantId;
                cacheItem.ImpersonatorUserId = AbpSession.GetUserId();
            }

            //Create a random token and save to the cache
            var token = Guid.NewGuid().ToString();

            await _cacheManager
                .GetImpersonationCache()
                .SetAsync(token, cacheItem, TimeSpan.FromMinutes(1));

            try
            {
                EafUserToken userToken = new();
                userToken.ExpireDate = DateTime.UtcNow.AddHours(1);
                userToken.LoginProvider = "TokenValidAdminLogin";
                userToken.TenantId = tenantId;
                userToken.UserId = userId;
                userToken.Name = token;
                userToken.Value = (!isBackToImpersonator) ? AbpSession.TenantId.ToString() + "-" + AbpSession.GetUserId().ToString() : null;

                await _userTokenRepository.InsertAndGetIdAsync(userToken);

                return token;
            }
            catch (Exception ex)
            {
                Logger.WarnFormat(ex, "Error on save Login Impersonator in DataBase");
                return token;
            }

            return token;
        }
    }
}