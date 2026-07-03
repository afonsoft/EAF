using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class ExternalLoginProviderSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ExternalLoginProviderSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirGoogle_IsEnabled_Entao_DeveArmazenar()
        {
            var sut = new ExternalLoginProviderSettingsEditDto();
            sut.Google_IsEnabled = true;
            sut.Google_IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirMicrosoft_IsEnabled_Entao_DeveArmazenar()
        {
            var sut = new ExternalLoginProviderSettingsEditDto();
            sut.Microsoft_IsEnabled = true;
            sut.Microsoft_IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirOpenIdConnect_IsEnabled_Entao_DeveArmazenar()
        {
            var sut = new ExternalLoginProviderSettingsEditDto();
            sut.OpenIdConnect_IsEnabled = true;
            sut.OpenIdConnect_IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirAuthZero_IsEnabled_Entao_DeveArmazenar()
        {
            var sut = new ExternalLoginProviderSettingsEditDto();
            sut.AuthZero_IsEnabled = true;
            sut.AuthZero_IsEnabled.ShouldBe(true);
        }
    }
}
