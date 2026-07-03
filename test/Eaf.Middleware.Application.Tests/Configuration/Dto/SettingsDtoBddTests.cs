using Eaf.Middleware.Configuration.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration
{
    public class SettingsDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SettingsDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirValue_Entao_DeveArmazenar()
        {
            var sut = new SettingsDto();
            sut.Value = "test_value";
            sut.Value.ShouldBe("test_value");
        }
    }
}
