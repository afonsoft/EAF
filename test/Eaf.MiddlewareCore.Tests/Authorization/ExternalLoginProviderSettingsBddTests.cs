using Eaf.Middleware.Core.Authentication;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para ExternalLoginProviderSettings seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class ExternalLoginProviderSettingsBddTests
    {
        #region GoogleExternalLoginProviderSettings

        [Fact]
        public void Dado_GoogleSettings_ComClientIdESecret_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "google-id",
                ClientSecret = "google-secret"
            };
            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_GoogleSettings_SemClientId_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "",
                ClientSecret = "google-secret"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_GoogleSettings_SemSecret_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                ClientId = "google-id",
                ClientSecret = ""
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_GoogleSettings_Quando_DefinirUserInfoEndpoint_Entao_DeveArmazenar()
        {
            var settings = new GoogleExternalLoginProviderSettings
            {
                UserInfoEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo"
            };
            settings.UserInfoEndpoint.ShouldBe("https://www.googleapis.com/oauth2/v2/userinfo");
        }

        #endregion

        #region MicrosoftExternalLoginProviderSettings

        [Fact]
        public void Dado_MicrosoftSettings_ComClientIdESecret_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = "ms-id",
                ClientSecret = "ms-secret"
            };
            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_MicrosoftSettings_SemClientId_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = null,
                ClientSecret = "ms-secret"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_MicrosoftSettings_SemSecret_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new MicrosoftExternalLoginProviderSettings
            {
                ClientId = "ms-id",
                ClientSecret = null
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_MicrosoftSettings_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var settings = new MicrosoftExternalLoginProviderSettings { TenantId = "tenant-123" };
            settings.TenantId.ShouldBe("tenant-123");
        }

        #endregion

        #region AuthZeroExternalLoginProviderSettings

        [Fact]
        public void Dado_AuthZeroSettings_ComTodosPreenchidos_Quando_IsValid_Entao_DeveRetornarTrue()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "az-id",
                ClientSecret = "az-secret",
                Endpoint = "https://acme.auth0.com"
            };
            settings.IsValid().ShouldBeTrue();
        }

        [Fact]
        public void Dado_AuthZeroSettings_SemClientId_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "",
                ClientSecret = "az-secret",
                Endpoint = "https://acme.auth0.com"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_AuthZeroSettings_SemSecret_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "az-id",
                ClientSecret = "",
                Endpoint = "https://acme.auth0.com"
            };
            settings.IsValid().ShouldBeFalse();
        }

        [Fact]
        public void Dado_AuthZeroSettings_SemEndpoint_Quando_IsValid_Entao_DeveRetornarFalse()
        {
            var settings = new AuthZeroExternalLoginProviderSettings
            {
                ClientId = "az-id",
                ClientSecret = "az-secret",
                Endpoint = ""
            };
            settings.IsValid().ShouldBeFalse();
        }

        #endregion
    }
}
