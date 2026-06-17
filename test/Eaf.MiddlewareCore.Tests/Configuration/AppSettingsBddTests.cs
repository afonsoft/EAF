using Eaf.Middleware.Configuration;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Configuration
{
    public class AppSettingsBddTests
    {
        #region CacheKeys

        [Fact]
        public void Dado_AppSettings_Quando_VerificarTenantRegistrationCache_Entao_DeveEstarCorreto()
        {
            AppSettings.CacheKeys.TenantRegistrationCache.ShouldBe("TenantRegistrationCache");
        }

        #endregion

        #region ExternalLoginProvider

        [Fact]
        public void Dado_ExternalLoginProvider_Quando_VerificarOpenIdConnectMappedClaims_Entao_DeveEstarCorreto()
        {
            AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims
                .ShouldBe("ExternalLoginProvider.OpenIdConnect.MappedClaims");
        }

        [Theory]
        [InlineData("ExternalLoginProvider.Google")]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarGoogle_Entao_DeveEstarCorreto(string expected)
        {
            AppSettings.ExternalLoginProvider.Host.Google.ShouldBe(expected);
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarAuthZero_Entao_DeveEstarCorreto()
        {
            AppSettings.ExternalLoginProvider.Host.AuthZero.ShouldBe("ExternalLoginProvider.AuthZero");
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarMicrosoft_Entao_DeveEstarCorreto()
        {
            AppSettings.ExternalLoginProvider.Host.Microsoft.ShouldBe("ExternalLoginProvider.Microsoft");
        }

        [Fact]
        public void Dado_ExternalLoginProviderHost_Quando_VerificarOpenIdConnect_Entao_DeveEstarCorreto()
        {
            AppSettings.ExternalLoginProvider.Host.OpenIdConnect.ShouldBe("ExternalLoginProvider.OpenIdConnect");
        }

        [Theory]
        [InlineData("ExternalLoginProvider.Google.Tenant")]
        public void Dado_ExternalLoginProviderTenant_Quando_VerificarGoogle_Entao_DeveEstarCorreto(string expected)
        {
            AppSettings.ExternalLoginProvider.Tenant.Google.ShouldBe(expected);
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenant_Quando_VerificarGoogleIsEnabled_Entao_DeveEstarCorreto()
        {
            AppSettings.ExternalLoginProvider.Tenant.Google_IsEnabled
                .ShouldBe("ExternalLoginProvider.Google.IsEnabled");
        }

        [Fact]
        public void Dado_ExternalLoginProviderTenant_Quando_VerificarTodosProviders_Entao_DevemEstarCorretos()
        {
            AppSettings.ExternalLoginProvider.Tenant.AuthZero.ShouldBe("ExternalLoginProvider.AuthZero.Tenant");
            AppSettings.ExternalLoginProvider.Tenant.AuthZero_IsEnabled.ShouldBe("ExternalLoginProvider.AuthZero.IsEnabled");
            AppSettings.ExternalLoginProvider.Tenant.Microsoft.ShouldBe("ExternalLoginProvider.Microsoft.Tenant");
            AppSettings.ExternalLoginProvider.Tenant.Microsoft_IsEnabled.ShouldBe("ExternalLoginProvider.Microsoft.IsEnabled");
            AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect.ShouldBe("ExternalLoginProvider.OpenIdConnect.Tenant");
            AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect_IsEnabled.ShouldBe("ExternalLoginProvider.OpenIdConnect.IsEnabled");
        }

        #endregion

        #region UiManagement

        [Fact]
        public void Dado_UiManagement_Quando_VerificarConstants_Entao_DevemEstarCorretos()
        {
            AppSettings.UiManagement.ContentSkin.ShouldBe("App.UiManagement.ContentSkin");
            AppSettings.UiManagement.LayoutType.ShouldBe("App.UiManagement.LayoutType");
            AppSettings.UiManagement.Theme.ShouldBe("App.UiManagement.Theme");
            AppSettings.UiManagement.ThemeColor.ShouldBe("App.UiManagement.ThemeColor");
        }

        [Fact]
        public void Dado_UiManagementHeader_Quando_VerificarConstants_Entao_DevemEstarCorretos()
        {
            AppSettings.UiManagement.Header.DesktopFixedHeader.ShouldBe("App.UiManagement.Header.DesktopFixedHeader");
            AppSettings.UiManagement.Header.MobileFixedHeader.ShouldBe("App.UiManagement.Header.MobileFixedHeader");
            AppSettings.UiManagement.Header.Skin.ShouldBe("App.UiManagement.Header.Skin");
        }

        [Fact]
        public void Dado_UiManagementLeftAside_Quando_VerificarConstants_Entao_DevemEstarCorretos()
        {
            AppSettings.UiManagement.LeftAside.AllowAsideHiding.ShouldBe("App.UiManagement.Left.AllowAsideHiding");
            AppSettings.UiManagement.LeftAside.AllowAsideMinimizing.ShouldBe("App.UiManagement.Left.AllowAsideMinimizing");
            AppSettings.UiManagement.LeftAside.AsideSkin.ShouldBe("App.UiManagement.Left.AsideSkin");
            AppSettings.UiManagement.LeftAside.DefaultHiddenAside.ShouldBe("App.UiManagement.Left.DefaultHiddenAside");
            AppSettings.UiManagement.LeftAside.DefaultMinimizedAside.ShouldBe("App.UiManagement.Left.DefaultMinimizedAside");
            AppSettings.UiManagement.LeftAside.FixedAside.ShouldBe("App.UiManagement.Left.FixedAside");
            AppSettings.UiManagement.LeftAside.Position.ShouldBe("App.UiManagement.Left.Position");
        }

        #endregion

        #region UserManagement

        [Fact]
        public void Dado_UserManagement_Quando_VerificarConstants_Entao_DevemEstarCorretos()
        {
            AppSettings.UserManagement.AllowOneConcurrentLoginPerUser.ShouldBe("App.UserManagement.AllowOneConcurrentLoginPerUser");
            AppSettings.UserManagement.IsCookieConsentEnabled.ShouldBe("App.UserManagement.IsCookieConsentEnabled");
            AppSettings.UserManagement.IsEmailConfirmationRequiredForLogin.ShouldBe("App.UserManagement.IsEmailConfirmationRequiredForLogin");
            AppSettings.UserManagement.StoreExternalTokenInformation.ShouldBe("App.UserManagement.StoreExternalTokenInformation");
            AppSettings.UserManagement.TokenExpiration.ShouldBe("App.UserManagement.TokenExpiration");
            AppSettings.UserManagement.UseCaptchaOnLogin.ShouldBe("App.UserManagement.UseCaptchaOnLogin");
        }

        [Fact]
        public void Dado_TwoFactorLogin_Quando_VerificarConstants_Entao_DevemEstarCorretos()
        {
            AppSettings.UserManagement.TwoFactorLogin.IsEmailProviderEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsEmailProviderEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled");
            AppSettings.UserManagement.TwoFactorLogin.IsSmsProviderEnabled.ShouldBe("App.UserManagement.TwoFactorLogin.IsSmsProviderEnabled");
        }

        #endregion
    }
}
