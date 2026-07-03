using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class TwoFactorLoginSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new TwoFactorLoginSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEnabled_Entao_DeveArmazenar()
        {
            var sut = new TwoFactorLoginSettingsEditDto();
            sut.IsEnabled = true;
            sut.IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEnabledForApplication_Entao_DeveArmazenar()
        {
            var sut = new TwoFactorLoginSettingsEditDto();
            sut.IsEnabledForApplication = true;
            sut.IsEnabledForApplication.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsGoogleAuthenticatorEnabled_Entao_DeveArmazenar()
        {
            var sut = new TwoFactorLoginSettingsEditDto();
            sut.IsGoogleAuthenticatorEnabled = true;
            sut.IsGoogleAuthenticatorEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsRememberBrowserEnabled_Entao_DeveArmazenar()
        {
            var sut = new TwoFactorLoginSettingsEditDto();
            sut.IsRememberBrowserEnabled = true;
            sut.IsRememberBrowserEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsSmsProviderEnabled_Entao_DeveArmazenar()
        {
            var sut = new TwoFactorLoginSettingsEditDto();
            sut.IsSmsProviderEnabled = true;
            sut.IsSmsProviderEnabled.ShouldBe(true);
        }
    }
}
