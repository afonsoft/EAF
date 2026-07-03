using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class GeneralSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GeneralSettingsEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTimezoneForComparison_Entao_DeveArmazenar()
        {
            var sut = new GeneralSettingsEditDto();
            sut.TimezoneForComparison = "test_value";
            sut.TimezoneForComparison.ShouldBe("test_value");
        }
    }
}
