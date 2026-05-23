using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.Google;
using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders
{
    /// <summary>
    /// Representa a classe TenantBasedGoogleExternalLoginInfoProvider.
    /// </summary>
    public class TenantBasedGoogleExternalLoginInfoProvider : TenantBasedExternalLoginInfoProviderBase,
         ISingletonDependency
    {
        private readonly IAbpSession _AbpSession;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// TenantBasedGoogleExternalLoginInfoProvider.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="eafSession">Parâmetro eafSession.</param>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public TenantBasedGoogleExternalLoginInfoProvider(
            ISettingManager settingManager,
            IAbpSession eafSession,
            ICacheManager cacheManager) : base(eafSession, cacheManager)
        {
            _settingManager = settingManager;
            _AbpSession = eafSession;
        }

        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public override string Name { get; } = GoogleAuthProviderApi.Name;

        protected override ExternalLoginProviderInfo GetHostInformation()
        {
            string settingValue = _settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.Google);
            var settings = settingValue.FromJsonString<GoogleExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override ExternalLoginProviderInfo GetTenantInformation()
        {
            string settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.Google, _AbpSession.TenantId.Value);
            var settings = settingValue.FromJsonString<GoogleExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override bool TenantHasSettings()
        {
            var settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.Google, _AbpSession.TenantId.Value);
            return !settingValue.IsNullOrWhiteSpace();
        }

        private static ExternalLoginProviderInfo CreateExternalLoginInfo(GoogleExternalLoginProviderSettings settings)
        {
            return new ExternalLoginProviderInfo(
                GoogleAuthProviderApi.Name,
                settings.ClientId,
                settings.ClientSecret,
                null,
                typeof(GoogleAuthProviderApi),
                new Dictionary<string, string>
                {
                    {"UserInfoEndpoint", settings.UserInfoEndpoint}
                }
            );
        }
    }
}