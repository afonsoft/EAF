using Eaf.Middleware.Editions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application
{
    public class LocalizableComboboxItemSourceDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new LocalizableComboboxItemSourceDto();
            sut.ShouldNotBeNull();
        }
    }
}
