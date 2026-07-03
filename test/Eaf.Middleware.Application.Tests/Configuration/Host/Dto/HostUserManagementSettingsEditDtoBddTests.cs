using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class HostUserManagementSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new HostUserManagementSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsCookieConsentEnabled_Entao_DeveArmazenar()
        {
            var sut = new HostUserManagementSettingsEditDto();
            sut.IsCookieConsentEnabled = true;
            sut.IsCookieConsentEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEmailConfirmationRequiredForLogin_Entao_DeveArmazenar()
        {
            var sut = new HostUserManagementSettingsEditDto();
            sut.IsEmailConfirmationRequiredForLogin = true;
            sut.IsEmailConfirmationRequiredForLogin.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsRegisterRequiredForLogin_Entao_DeveArmazenar()
        {
            var sut = new HostUserManagementSettingsEditDto();
            sut.IsRegisterRequiredForLogin = true;
            sut.IsRegisterRequiredForLogin.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirStoreExternalTokenInformation_Entao_DeveArmazenar()
        {
            var sut = new HostUserManagementSettingsEditDto();
            sut.StoreExternalTokenInformation = true;
            sut.StoreExternalTokenInformation.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTokenExpiration_Entao_DeveArmazenar()
        {
            var sut = new HostUserManagementSettingsEditDto();
            sut.TokenExpiration = 42;
            sut.TokenExpiration.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUseCaptchaOnLogin_Entao_DeveArmazenar()
        {
            var sut = new HostUserManagementSettingsEditDto();
            sut.UseCaptchaOnLogin = true;
            sut.UseCaptchaOnLogin.ShouldBe(true);
        }
    }
}
