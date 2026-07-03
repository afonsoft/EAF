using Eaf.Middleware.Editions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class LocalizableComboboxItemDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new LocalizableComboboxItemDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirValue_Entao_DeveArmazenar()
        {
            var sut = new LocalizableComboboxItemDto();
            sut.Value = "test_value";
            sut.Value.ShouldBe("test_value");
        }
    }
}
