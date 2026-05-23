using Abp.Configuration;
using Abp.Dependency;
using Abp.Json;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.Authentication.External;
using Eaf.Middleware.Core.Authentication.External.OpenIdConnect;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using Abp.Extensions;

namespace Eaf.Middleware.Core.Authentication.ExternalLoginInfoProviders
{
    /// <summary>
    /// Representa a classe TenantBasedOpenIdConnectExternalLoginInfoProvider.
    /// </summary>
    public class TenantBasedOpenIdConnectExternalLoginInfoProvider : TenantBasedExternalLoginInfoProviderBase, ISingletonDependency
    {
        private readonly IAbpSession _AbpSession;
        private readonly ISettingManager _settingManager;

        /// <summary>
        /// TenantBasedOpenIdConnectExternalLoginInfoProvider.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <param name="eafSession">Parâmetro eafSession.</param>
        /// <param name="cacheManager">Parâmetro cacheManager.</param>
        /// <returns>Resultado da operação.</returns>
        public TenantBasedOpenIdConnectExternalLoginInfoProvider(
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
        public override string Name { get; } = OpenIdConnectAuthProviderApi.Name;

        protected override ExternalLoginProviderInfo GetHostInformation()
        {
            string settingValue = _settingManager.GetSettingValueForApplication(AppSettings.ExternalLoginProvider.Host.OpenIdConnect);
            var settings = settingValue.FromJsonString<OpenIdConnectExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override ExternalLoginProviderInfo GetTenantInformation()
        {
            string settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect, _AbpSession.TenantId.Value);
            var settings = settingValue.FromJsonString<OpenIdConnectExternalLoginProviderSettings>();
            return CreateExternalLoginInfo(settings);
        }

        protected override bool TenantHasSettings()
        {
            var settingValue = _settingManager.GetSettingValueForTenant(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect, _AbpSession.TenantId.Value);
            return !settingValue.IsNullOrWhiteSpace();
        }

        private ExternalLoginProviderInfo CreateExternalLoginInfo(OpenIdConnectExternalLoginProviderSettings settings)
        {
            var mappingSettings = _settingManager.GetSettingValue(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims);
            var jsonClaimMappings = mappingSettings.FromJsonString<List<JsonClaimMap>>();

            return new ExternalLoginProviderInfo(
                OpenIdConnectAuthProviderApi.Name,
                settings.ClientId,
                settings.ClientSecret,
                null,
                typeof(OpenIdConnectAuthProviderApi),
                new Dictionary<string, string>
                {
                    {"Authority", settings.Authority},
                    {"LoginUrl", settings.LoginUrl},
                    {"ValidateIssuer", settings.ValidateIssuer.ToString()}
                },
                jsonClaimMappings
            );
        }
    }
}