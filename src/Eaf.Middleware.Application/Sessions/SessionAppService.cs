using Abp;
using Abp.Auditing;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Eaf.Middleware.Authorization.TwoFactor;
using Eaf.Middleware.Sessions.Dto;
using Eaf.Middleware.UiCustomization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Eaf.Middleware.Sessions
{
    /// <summary>
    /// Representa a classe SessionAppService.
    /// </summary>
    public class SessionAppService : MiddlewareAppServiceBase, ISessionAppService
    {
        private readonly IUiThemeCustomizerFactory _uiThemeCustomizerFactory;

        /// <summary>
        /// SessionAppService.
        /// </summary>
        /// <param name="uiThemeCustomizerFactory">Parâmetro uiThemeCustomizerFactory.</param>
        /// <returns>Resultado da operação.</returns>
        public SessionAppService(IUiThemeCustomizerFactory uiThemeCustomizerFactory)
        {
            _uiThemeCustomizerFactory = uiThemeCustomizerFactory;
        }

        [DisableAuditing]
        [UnitOfWork]
        public async Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations()
        {
            var output = new GetCurrentLoginInformationsOutput
            {
                Application = new ApplicationInfoDto
                {
                    Version = AppVersionHelper.Version,
                    ReleaseDate = AppVersionHelper.ReleaseDate,
                    Features = new Dictionary<string, bool>(),
                    Currency = "BRL",
                    CurrencySign = "R$",
                    TwoFactorCodeExpireSeconds = TwoFactorCodeCacheItem.DefaultSlidingExpireTime.TotalSeconds
                }
            };

            var uiCustomizer = await _uiThemeCustomizerFactory.GetCurrentUiCustomizer();
            output.Theme = await uiCustomizer.GetUiSettings();

            if (AbpSession.TenantId.HasValue)
            {
                output.Tenant = ObjectMapper
                    .Map<TenantLoginInfoDto>(await TenantManager
                        .Tenants
                        .FirstAsync(t => t.Id == AbpSession.GetTenantId()));
            }

            if (AbpSession.UserId.HasValue)
            {
                output.User = ObjectMapper.Map<UserLoginInfoDto>(await GetCurrentUserAsync());
            }

            return output;
        }

        /// <summary>
        /// UpdateUserSignInToken.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public async Task<UpdateUserSignInTokenOutput> UpdateUserSignInToken()
        {
            if (AbpSession.UserId <= 0)
            {
                throw new AbpException(L("ThereIsNoLoggedInUser"));
            }

            var user = await UserManager.GetUserAsync(AbpSession.ToUserIdentifier());
            user.SetSignInToken();
            return new UpdateUserSignInTokenOutput
            {
                SignInToken = user.SignInToken,
                EncodedUserId = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Id.ToString())),
                EncodedTenantId = user.TenantId.HasValue
                    ? Convert.ToBase64String(Encoding.UTF8.GetBytes(user.TenantId.Value.ToString()))
                    : ""
            };
        }
    }
}