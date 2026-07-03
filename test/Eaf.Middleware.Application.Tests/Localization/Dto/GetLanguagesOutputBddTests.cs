using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Localization
{
    public class GetLanguagesOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetLanguagesOutput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDefaultLanguageName_Entao_DeveArmazenar()
        {
            var sut = new GetLanguagesOutput();
            sut.DefaultLanguageName = "test_value";
            sut.DefaultLanguageName.ShouldBe("test_value");
        }
    }
}
