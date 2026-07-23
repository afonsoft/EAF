using Abp.Configuration;
using Abp.Json;
using Abp.Zero.Configuration;
using Eaf.Middleware.Core.Authentication;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a classe AppSettingProvider.
    /// </summary>
    public class AppSettingProvider : SettingProvider
    {
        private const string FalseString = "false";
        private const string DefaultThemeName = "default";
        private const string DefaultLightSkin = "light";
        private const string DefaultFluidLayout = "fluid";

        private readonly IConfigurationRoot _appConfiguration;

        /// <summary>
        /// AppSettingProvider.
        /// </summary>
        /// <param name="configurationAccessor">Parâmetro configurationAccessor.</param>
        /// <returns>Resultado da operação.</returns>
        public AppSettingProvider(IAppConfigurationAccessor configurationAccessor)
        {
            _appConfiguration = configurationAccessor.Configuration;
        }

        /// <summary>
        /// GetSettingDefinitions.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        /// <returns>Resultado da operação.</returns>
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return GetHostSettings()
                .Union(GetTenantSettings())
                .Union(GetSharedSettings())
                //theme settings
                .Union(GetDefaultThemeSettings())
                .Union(GetTheme2Settings())
                .Union(GetTheme3Settings())
                .Union(GetTheme4Settings())
                .Union(GetExternalLoginProviderSettings());
        }

        private IEnumerable<SettingDefinition> GetDefaultThemeSettings()
        {
            var themeName = DefaultThemeName;

            return new[]
            {
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LayoutType, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LayoutType, DefaultFluidLayout), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ContentSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ContentSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.DesktopFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.DesktopFixedHeader, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.MobileFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.MobileFixedHeader, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.Skin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.Skin, DefaultLightSkin),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AsideSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AsideSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.FixedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.FixedAside, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideHiding, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideHiding, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultHiddenAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultHiddenAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ThemeColor, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ThemeColor, DefaultThemeName), isVisibleToClients: true, scopes: SettingScopes.All)
            };
        }

        private IEnumerable<SettingDefinition> GetExternalLoginProviderSettings()
        {
            return GetGoogleExternalLoginProviderSettings()
                .Union(GetMicrosoftExternalLoginProviderSettings())
                .Union(GetOpenIdConnectExternalLoginProviderSettings())
                .Union(GetAuthZeroExternalLoginProviderSettings());
        }

        private string GetFromAppSettings(string name, string defaultValue = null)
        {
            return GetFromSettings("App:" + name, defaultValue);
        }

        private string GetFromSettings(string name, string defaultValue = null)
        {
            return _appConfiguration[name] ?? defaultValue;
        }

        private SettingDefinition[] GetAuthZeroExternalLoginProviderSettings()
        {
            string clientId = GetFromSettings("Authentication:Autho:ClientId");
            string clientSecret = GetFromSettings("Authentication:Autho:ClientSecret");
            string userInfoEndPoint = GetFromSettings("Authentication:Autho:Endpoint");

            var googleExternalLoginProviderInfo = new AuthZeroExternalLoginProviderSettings()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Endpoint = userInfoEndPoint
            };

            return new[]
            {
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Host.AuthZero,
                    googleExternalLoginProviderInfo.ToJsonString(),
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled,
                    FalseString,
                    isVisibleToClients: true,
                    scopes: SettingScopes.Tenant | SettingScopes.Application
                ),
                new SettingDefinition( //default is empty for tenants
                    AppSettings.ExternalLoginProvider.Tenant.AuthZero,
                    "",
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
            };
        }

        private SettingDefinition[] GetGoogleExternalLoginProviderSettings()
        {
            string clientId = GetFromSettings("Authentication:Google:ClientId");
            string clientSecret = GetFromSettings("Authentication:Google:ClientSecret");
            string userInfoEndPoint = GetFromSettings("Authentication:Google:UserInfoEndpoint");

            var googleExternalLoginProviderInfo = new GoogleExternalLoginProviderSettings()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                UserInfoEndpoint = userInfoEndPoint
            };

            return new[]
            {
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Host.Google,
                    googleExternalLoginProviderInfo.ToJsonString(),
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled,
                    FalseString,
                    isVisibleToClients: true,
                    scopes: SettingScopes.Tenant | SettingScopes.Application
                ),
                new SettingDefinition( //default is empty for tenants
                    AppSettings.ExternalLoginProvider.Tenant.Google,
                    "",
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
            };
        }

        private IEnumerable<SettingDefinition> GetHostSettings()
        {
            return new[] {
                new SettingDefinition(AppSettings.UiManagement.Theme, GetFromAppSettings(AppSettings.UiManagement.Theme, DefaultThemeName), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims, "", isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Host.Google, "", isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Host.Microsoft, "", isVisibleToClients: true, scopes: SettingScopes.All),
                  new SettingDefinition(AppSettings.ExternalLoginProvider.Host.AuthZero, "", isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Host.OpenIdConnect, "", isVisibleToClients: true, scopes: SettingScopes.All)
            };
        }

        private SettingDefinition[] GetMicrosoftExternalLoginProviderSettings()
        {
            string consumerKey = GetFromSettings("Authentication:Microsoft:ConsumerKey");
            string consumerSecret = GetFromSettings("Authentication:Microsoft:ConsumerSecret");

            var microsoftExternalLoginProviderInfo = new MicrosoftExternalLoginProviderSettings()
            {
                ClientId = consumerKey,
                ClientSecret = consumerSecret
            };

            return new[]
            {
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Host.Microsoft,
                    microsoftExternalLoginProviderInfo.ToJsonString(),
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled,
                    FalseString,
                    isVisibleToClients: true,
                    scopes: SettingScopes.Tenant | SettingScopes.Application
                ),
                new SettingDefinition( //default is empty for tenants
                    AppSettings.ExternalLoginProvider.Tenant.Microsoft,
                    "",
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
            };
        }

        private SettingDefinition[] GetOpenIdConnectExternalLoginProviderSettings()
        {
            var clientId = GetFromSettings("Authentication:OpenId:ClientId");
            var clientSecret = GetFromSettings("Authentication:OpenId:ClientSecret");
            var authority = GetFromSettings("Authentication:OpenId:Authority");
            var validateIssuerStr = GetFromSettings("Authentication:OpenId:ValidateIssuer");

            bool.TryParse(validateIssuerStr, out bool validateIssuer);

            var openIdConnectExternalLoginProviderInfo = new OpenIdConnectExternalLoginProviderSettings()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Authority = authority,
                ValidateIssuer = validateIssuer
            };

            var jsonClaimMappings = new List<JsonClaimMapDto>();
            _appConfiguration.GetSection("Authentication:OpenId:ClaimsMapping").Bind(jsonClaimMappings);

            return new[]
            {
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Host.OpenIdConnect,
                    openIdConnectExternalLoginProviderInfo.ToJsonString(),
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled,
                    FalseString,
                    isVisibleToClients: true,
                    scopes: SettingScopes.Tenant | SettingScopes.Application
                ),
                new SettingDefinition( //default is empty for tenants
                    AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect,
                    "",
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant,
                    isEncrypted:true
                ),
                new SettingDefinition(
                    AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims,
                    jsonClaimMappings.ToJsonString(),
                    isVisibleToClients: false,
                    scopes: SettingScopes.Application | SettingScopes.Tenant
                )
            };
        }

        private IEnumerable<SettingDefinition> GetSharedSettings()
        {
            return new[]
            {
                new SettingDefinition(AppSettings.UserManagement.IsCookieConsentEnabled, GetFromAppSettings(AppSettings.UserManagement.IsCookieConsentEnabled, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.StoreExternalTokenInformation, GetFromAppSettings(AppSettings.UserManagement.StoreExternalTokenInformation, "true"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.TokenExpiration, GetFromAppSettings(AppSettings.UserManagement.TokenExpiration, "8640"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.RefreshTokenExpirationInDays, GetFromAppSettings(AppSettings.UserManagement.RefreshTokenExpirationInDays, "7"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.UseCaptchaOnLogin, GetFromAppSettings(AppSettings.UserManagement.UseCaptchaOnLogin, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.IsEmailConfirmationRequiredForLogin, GetFromAppSettings(AppSettings.UserManagement.IsEmailConfirmationRequiredForLogin, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, GetFromAppSettings(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.TwoFactorLogin.IsEnabled, GetFromAppSettings(AppSettings.UserManagement.TwoFactorLogin.IsEnabled, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.TwoFactorLogin.IsEmailProviderEnabled, GetFromAppSettings(AppSettings.UserManagement.TwoFactorLogin.IsEmailProviderEnabled, "true"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.TwoFactorLogin.IsSmsProviderEnabled, GetFromAppSettings(AppSettings.UserManagement.TwoFactorLogin.IsSmsProviderEnabled, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, GetFromAppSettings(AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, "true"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit, GetFromAppSettings(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase, GetFromAppSettings(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric, GetFromAppSettings(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase, GetFromAppSettings(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength, GetFromAppSettings(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength, "6"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(EafMiddlewareSettingNames.UserManagement.IsRegisterRequiredForLogin, GetFromAppSettings(EafMiddlewareSettingNames.UserManagement.IsRegisterRequiredForLogin, FalseString), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled, GetFromAppSettings(EafMiddlewareSettingNames.LoginImpersonator.IsEnabled, "true"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(EafMiddlewareSettingNames.LogDeleter.IsEnabled, GetFromAppSettings(EafMiddlewareSettingNames.LogDeleter.IsEnabled, "true"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(EafMiddlewareSettingNames.LogDeleter.ExpiredDays, GetFromAppSettings(EafMiddlewareSettingNames.LogDeleter.ExpiredDays, "180"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity, GetFromAppSettings(EafMiddlewareSettingNames.LogDeleter.DeletedQuantity, "30000"), scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
            };
        }

        private static IEnumerable<SettingDefinition> GetTenantSettings()
        {
            return new[] {
                new SettingDefinition(AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled, FalseString, isVisibleToClients: true, scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Tenant.Google, "", isVisibleToClients: true, scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Tenant.Microsoft, "", isVisibleToClients: true, scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled, FalseString, isVisibleToClients: true, scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect, "", isVisibleToClients: true, scopes: SettingScopes.Tenant),
                new SettingDefinition(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled, FalseString, isVisibleToClients: true, scopes: SettingScopes.Tenant)
            };
        }

        private IEnumerable<SettingDefinition> GetTheme2Settings()
        {
            var themeName = "theme2";

            return new[]
            {
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LayoutType, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LayoutType, DefaultFluidLayout), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ContentSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ContentSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.DesktopFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.DesktopFixedHeader, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.MobileFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.MobileFixedHeader, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.Skin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.Skin, DefaultLightSkin),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AsideSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AsideSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.FixedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.FixedAside, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideHiding, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideHiding, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultHiddenAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultHiddenAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ThemeColor, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ThemeColor, DefaultThemeName), isVisibleToClients: true, scopes: SettingScopes.All)
            };
        }

        private IEnumerable<SettingDefinition> GetTheme3Settings()
        {
            var themeName = "theme3";

            return new[]
            {
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LayoutType, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LayoutType, DefaultFluidLayout), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ContentSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ContentSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.DesktopFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.DesktopFixedHeader, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.MobileFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.MobileFixedHeader, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.Skin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.Skin, DefaultLightSkin),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AsideSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AsideSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.FixedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.FixedAside, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideHiding, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideHiding, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultHiddenAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultHiddenAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ThemeColor, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ThemeColor, DefaultThemeName), isVisibleToClients: true, scopes: SettingScopes.All)
            };
        }

        private IEnumerable<SettingDefinition> GetTheme4Settings()
        {
            var themeName = "theme4";

            return new[]
            {
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LayoutType, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LayoutType, DefaultFluidLayout), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ContentSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ContentSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.DesktopFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.DesktopFixedHeader, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.MobileFixedHeader, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.MobileFixedHeader, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.Header.Skin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.Header.Skin, DefaultLightSkin),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AsideSkin, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AsideSkin, DefaultLightSkin), isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.FixedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.FixedAside, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideMinimizing, "true"),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultMinimizedAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.AllowAsideHiding, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.AllowAsideHiding, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.LeftAside.DefaultHiddenAside, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.LeftAside.DefaultHiddenAside, FalseString),isVisibleToClients: true, scopes: SettingScopes.All),
                new SettingDefinition(themeName + "." + AppSettings.UiManagement.ThemeColor, GetFromAppSettings(themeName + "." +AppSettings.UiManagement.ThemeColor, DefaultThemeName), isVisibleToClients: true, scopes: SettingScopes.All)
            };
        }
    }
}