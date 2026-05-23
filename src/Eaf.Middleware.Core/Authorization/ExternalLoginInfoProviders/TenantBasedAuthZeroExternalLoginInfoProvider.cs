using Abp.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Json;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.AuthZero;
using System;
using System.Collections.Generic;

namespace Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders
{
    /// <summary>
    /// Representa a classe TenantBasedAuthZeroExternalLoginInfoProvider.
    /// </summary>
    public class TenantBasedAuthZeroExternalLoginInfoProvider : TenantBasedExternalLoginInfoProviderBase,
         ISingletonDependency
    {
        private readonly IAbpSession _AbpSession;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// TenantBasedAuthZeroExternalLoginInfoProvider.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="eafSession">Parâmetro eafSession.</param>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public TenantBasedAuthZeroExternalLoginInfoProvider(
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
        public override string Name { get; } = AuthZeroAuthProviderApi.Name;

        protected override ExternalLoginProviderInfo GetHostInformation()
        {
            string settingValue = _settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.AuthZero);
            var settings = settingValue.FromJsonString<AuthZeroExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override ExternalLoginProviderInfo GetTenantInformation()
        {
            string settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.AuthZero, _AbpSession.TenantId.Value);
            var settings = settingValue.FromJsonString<AuthZeroExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override bool TenantHasSettings()
        {
            var settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.AuthZero, _AbpSession.TenantId.Value);
            return !settingValue.IsNullOrWhiteSpace();
        }

        private static ExternalLoginProviderInfo CreateExternalLoginInfo(AuthZeroExternalLoginProviderSettings settings)
        {
            return new ExternalLoginProviderInfo(
                AuthZeroAuthProviderApi.Name,
                settings.ClientId,
                settings.ClientSecret,
                null,
                typeof(AuthZeroAuthProviderApi),
                new Dictionary<string, string>
                {
                    {"Endpoint", settings.Endpoint}
                }
            );
        }
    }
}