using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Localization
{
    public class CreateOrUpdateLanguageInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CreateOrUpdateLanguageInput();
            sut.ShouldNotBeNull();
        }
    }
}
