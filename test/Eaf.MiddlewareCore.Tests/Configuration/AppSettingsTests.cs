using Eaf.Middleware.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Configuration
{
    public class AppSettingsTests
    {
        [Fact]
        public void Dado_CacheKeys_Quando_VerificarTenantRegistrationCache_Entao_DeveSerCorreto()
        {
            AppSettings.CacheKeys.TenantRegistrationCache.ShouldBe("TenantRegistrationCache");
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarGoogle_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Host.Google.ShouldBe("ExternalLoginProvider.Google");
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarAuthZero_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Host.AuthZero.ShouldBe("ExternalLoginProvider.AuthZero");
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarMicrosoft_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Host.Microsoft.ShouldBe("ExternalLoginProvider.Microsoft");
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarOpenIdConnect_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Host.OpenIdConnect.ShouldBe("ExternalLoginProvider.OpenIdConnect");
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenant_Quando_VerificarGoogle_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Tenant.Google.ShouldBe("ExternalLoginProvider.Google.Tenant");
            AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled.ShouldBe("ExternalLoginProvider.Google.IsEnabled");
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenant_Quando_VerificarAuthZero_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Tenant.AuthZero.ShouldBe("ExternalLoginProvider.AuthZero.Tenant");
            AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled.ShouldBe("ExternalLoginProvider.AuthZero.IsEnabled");
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenant_Quando_VerificarMicrosoft_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Tenant.Microsoft.ShouldBe("ExternalLoginProvider.Microsoft.Tenant");
            AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled.ShouldBe("ExternalLoginProvider.Microsoft.IsEnabled");
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenant_Quando_VerificarOpenIdConnect_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect.ShouldBe("ExternalLoginProvider.OpenIdConnect.Tenant");
            AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled.ShouldBe("ExternalLoginProvider.OpenIdConnect.IsEnabled");
        }

        [Fact]
        public void Dado_ExternalLoginProvider_Quando_VerificarMappedClaims_Entao_DeveSerCorreto()
        {
            AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims.ShouldBe("ExternalLoginProvider.OpenIdConnect.MappedClaims");
        }

        [Fact]
        public void Dado_UiManagement_Quando_VerificarConstantes_Entao_DeveSerCorreto()
        {
            AppSettings.UiManagement.Theme.ShouldBe("App.UiManagement.Theme");
            AppSettings.UiManagement.ThemeColor.ShouldBe("App.UiManagement.ThemeColor");
            AppSettings.UiManagement.LayoutType.ShouldBe("App.UiManagement.LayoutType");
            AppSettings.UiManagement.ContentSkin.ShouldBe("App.UiManagement.ContentSkin");
        }

        [Fact]
        public void Dado_UiManagementHeader_Quando_VerificarConstantes_Entao_DeveSerCorreto()
        {
            AppSettings.UiManagement.Header.DesktopFixedHeader.ShouldBe("App.UiManagement.Header.DesktopFixedHeader");
            AppSettings.UiManagement.Header.MobileFixedHeader.ShouldBe("App.UiManagement.Header.MobileFixedHeader");
            AppSettings.UiManagement.Header.Skin.ShouldBe("App.UiManagement.Header.Skin");
        }

        [Fact]
        public void Dado_UiManagementLeftAside_Quando_VerificarConstantes_Entao_DeveSerCorreto()
        {
            AppSettings.UiManagement.LeftAside.FixedAside.ShouldBe("App.UiManagement.Left.FixedAside");
            AppSettings.UiManagement.LeftAside.Position.ShouldBe("App.UiManagement.Left.Position");
            AppSettings.UiManagement.LeftAside.AsideSkin.ShouldBe("App.UiManagement.Left.AsideSkin");
            AppSettings.UiManagement.LeftAside.AllowAsideMinimizing.ShouldBe("App.UiManagement.Left.AllowAsideMinimizing");
            AppSettings.UiManagement.LeftAside.AllowAsideHiding.ShouldBe("App.UiManagement.Left.AllowAsideHiding");
            AppSettings.UiManagement.LeftAside.DefaultMinimizedAside.ShouldBe("App.UiManagement.Left.DefaultMinimizedAside");
            AppSettings.UiManagement.LeftAside.DefaultHiddenAside.ShouldBe("App.UiManagement.Left.DefaultHiddenAside");
        }

        [Fact]
        public void Dado_UserManagement_Quando_VerificarConstantes_Entao_DeveSerCorreto()
        {
            AppSettings.UserManagement.AllowOneConcurrentLoginPerUser.ShouldBe("App.UserManagement.AllowOneConcurrentLoginPerUser");
            AppSettings.UserManagement.IsCookieConsentEnabled.ShouldBe("App.UserManagement.IsCookieConsentEnabled");
            AppSettings.UserManagement.IsEmailConfirmationRequiredForLogin.ShouldBe("App.UserManagement.IsEmailConfirmationRequiredForLogin");
            AppSettings.UserManagement.UseCaptchaOnLogin.ShouldBe("App.UserManagement.UseCaptchaOnLogin");
            AppSettings.UserManagement.TokenExpiration.ShouldBe("App.UserManagement.TokenExpiration");
            AppSettings.UserManagement.StoreExternalTokenInformation.ShouldBe("App.UserManagement.StoreExternalTokenInformation");
        }

        [Fact]
        public void Dado_TwoFactorLogin_Quando_VerificarConstantes_Entao_DeveSerCorreto()
        {
            AppSettings.UserManagement.TwoFactorLogin.IsEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsEmailProviderEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsEmailProviderEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsSmsProviderEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsSmsProviderEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled");
        }
    }
}
