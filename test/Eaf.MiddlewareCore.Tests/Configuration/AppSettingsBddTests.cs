using Eaf.Middleware.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para AppSettings — constantes de configuração
    /// </summary>
    public class AppSettingsBddTests
    {
        [Fact]
        public void Dado_CacheKeys_Quando_Verificar_Entao_DeveConterTenantRegistrationCache()
        {
            AppSettings.CacheKeys.TenantRegistrationCache.ShouldBe("TenantRegistrationCache");
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            AppSettings.ExternalLoginProvider.Host.Google.ShouldBe("ExternalLoginProvider.Google");
            AppSettings.ExternalLoginProvider.Host.AuthZero.ShouldBe("ExternalLoginProvider.AuthZero");
            AppSettings.ExternalLoginProvider.Host.Microsoft.ShouldBe("ExternalLoginProvider.Microsoft");
            AppSettings.ExternalLoginProvider.Host.OpenIdConnect.ShouldBe("ExternalLoginProvider.OpenIdConnect");
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenant_Quando_Verificar_Entao_DevemTerPrefixoCorreto()
        {
            AppSettings.ExternalLoginProvider.Tenant.Google.ShouldStartWith("ExternalLoginProvider.Google");
            AppSettings.ExternalLoginProvider.Tenant.AuthZero.ShouldStartWith("ExternalLoginProvider.AuthZero");
            AppSettings.ExternalLoginProvider.Tenant.Microsoft.ShouldStartWith("ExternalLoginProvider.Microsoft");
            AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect.ShouldStartWith("ExternalLoginProvider.OpenIdConnect");
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenantIsEnabled_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled.ShouldBe("ExternalLoginProvider.Google.IsEnabled");
            AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled.ShouldBe("ExternalLoginProvider.AuthZero.IsEnabled");
            AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled.ShouldBe("ExternalLoginProvider.Microsoft.IsEnabled");
            AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled.ShouldBe("ExternalLoginProvider.OpenIdConnect.IsEnabled");
        }

        [Fact]
        public void Dado_OpenIdConnectMappedClaims_Quando_Verificar_Entao_DeveTerValorCorreto()
        {
            AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims
                .ShouldBe("ExternalLoginProvider.OpenIdConnect.MappedClaims");
        }

        [Fact]
        public void Dado_UiManagement_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            AppSettings.UiManagement.Theme.ShouldBe("App.UiManagement.Theme");
            AppSettings.UiManagement.LayoutType.ShouldBe("App.UiManagement.LayoutType");
            AppSettings.UiManagement.ContentSkin.ShouldBe("App.UiManagement.ContentSkin");
            AppSettings.UiManagement.ThemeColor.ShouldBe("App.UiManagement.ThemeColor");
        }

        [Fact]
        public void Dado_UiManagementHeader_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            AppSettings.UiManagement.Header.DesktopFixedHeader.ShouldBe("App.UiManagement.Header.DesktopFixedHeader");
            AppSettings.UiManagement.Header.MobileFixedHeader.ShouldBe("App.UiManagement.Header.MobileFixedHeader");
            AppSettings.UiManagement.Header.Skin.ShouldBe("App.UiManagement.Header.Skin");
        }

        [Fact]
        public void Dado_UiManagementLeftAside_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            AppSettings.UiManagement.LeftAside.FixedAside.ShouldBe("App.UiManagement.Left.FixedAside");
            AppSettings.UiManagement.LeftAside.AllowAsideMinimizing.ShouldBe("App.UiManagement.Left.AllowAsideMinimizing");
            AppSettings.UiManagement.LeftAside.AllowAsideHiding.ShouldBe("App.UiManagement.Left.AllowAsideHiding");
            AppSettings.UiManagement.LeftAside.DefaultMinimizedAside.ShouldBe("App.UiManagement.Left.DefaultMinimizedAside");
            AppSettings.UiManagement.LeftAside.DefaultHiddenAside.ShouldBe("App.UiManagement.Left.DefaultHiddenAside");
            AppSettings.UiManagement.LeftAside.Position.ShouldBe("App.UiManagement.Left.Position");
            AppSettings.UiManagement.LeftAside.AsideSkin.ShouldBe("App.UiManagement.Left.AsideSkin");
        }

        [Fact]
        public void Dado_UserManagement_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            AppSettings.UserManagement.AllowOneConcurrentLoginPerUser.ShouldBe("App.UserManagement.AllowOneConcurrentLoginPerUser");
            AppSettings.UserManagement.IsCookieConsentEnabled.ShouldBe("App.UserManagement.IsCookieConsentEnabled");
            AppSettings.UserManagement.IsEmailConfirmationRequiredForLogin.ShouldBe("App.UserManagement.IsEmailConfirmationRequiredForLogin");
            AppSettings.UserManagement.StoreExternalTokenInformation.ShouldBe("App.UserManagement.StoreExternalTokenInformation");
            AppSettings.UserManagement.TokenExpiration.ShouldBe("App.UserManagement.TokenExpiration");
            AppSettings.UserManagement.UseCaptchaOnLogin.ShouldBe("App.UserManagement.UseCaptchaOnLogin");
        }

        [Fact]
        public void Dado_TwoFactorLogin_Quando_Verificar_Entao_DevemTerValoresCorretos()
        {
            AppSettings.UserManagement.TwoFactorLogin.IsEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsEmailProviderEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsEmailProviderEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsSmsProviderEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsSmsProviderEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled");
        }
    }
}
