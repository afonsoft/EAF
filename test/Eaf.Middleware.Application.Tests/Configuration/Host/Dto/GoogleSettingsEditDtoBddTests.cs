using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class GoogleSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GoogleSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirRecaptchaSiteKey_Entao_DeveArmazenar()
        {
            var sut = new GoogleSettingsEditDto();
            sut.RecaptchaSiteKey = "test_value";
            sut.RecaptchaSiteKey.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTag_Entao_DeveArmazenar()
        {
            var sut = new GoogleSettingsEditDto();
            sut.Tag = "test_value";
            sut.Tag.ShouldBe("test_value");
        }
    }
}
