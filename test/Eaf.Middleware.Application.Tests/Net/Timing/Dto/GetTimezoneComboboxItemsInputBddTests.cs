using Eaf.Middleware.Timing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Net.Timing
{
    public class GetTimezoneComboboxItemsInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetTimezoneComboboxItemsInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSelectedTimezoneId_Entao_DeveArmazenar()
        {
            var sut = new GetTimezoneComboboxItemsInput();
            sut.SelectedTimezoneId = "test_value";
            sut.SelectedTimezoneId.ShouldBe("test_value");
        }
    }
}
