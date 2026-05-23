using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.Microsoft;
using System;

namespace Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders
{
    /// <summary>
    /// Representa a classe TenantBasedMicrosoftExternalLoginInfoProvider.
    /// </summary>
    public class TenantBasedMicrosoftExternalLoginInfoProvider : TenantBasedExternalLoginInfoProviderBase,
        ISingletonDependency
    {
        private readonly IAbpSession _AbpSession;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// TenantBasedMicrosoftExternalLoginInfoProvider.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="eafSession">Parâmetro eafSession.</param>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public TenantBasedMicrosoftExternalLoginInfoProvider(
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
        public override string Name { get; } = MicrosoftAuthProviderApi.Name;

        protected override ExternalLoginProviderInfo GetHostInformation()
        {
            string settingValue = _settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.Microsoft);
            var settings = settingValue.FromJsonString<MicrosoftExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override ExternalLoginProviderInfo GetTenantInformation()
        {
            string settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.Microsoft, _AbpSession.TenantId.Value);
            var settings = settingValue.FromJsonString<MicrosoftExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override bool TenantHasSettings()
        {
            var settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.Microsoft, _AbpSession.TenantId.Value);
            return !settingValue.IsNullOrWhiteSpace();
        }

        private static ExternalLoginProviderInfo CreateExternalLoginInfo(MicrosoftExternalLoginProviderSettings settings)
        {
            return new ExternalLoginProviderInfo(
                MicrosoftAuthProviderApi.Name,
                settings.ClientId,
                settings.ClientSecret,
                settings.TenantId,
                typeof(MicrosoftAuthProviderApi)
            );
        }
    }
}