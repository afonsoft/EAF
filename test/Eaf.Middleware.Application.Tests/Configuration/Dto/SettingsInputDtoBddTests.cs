using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration
{
    public class SettingsInputDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SettingsInputDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFilter_Entao_DeveArmazenar()
        {
            var sut = new SettingsInputDto();
            sut.Filter = "test_value";
            sut.Filter.ShouldBe("test_value");
        }
    }
}
