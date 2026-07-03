using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Localization
{
    public class LanguageTextListDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new LanguageTextListDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirKey_Entao_DeveArmazenar()
        {
            var sut = new LanguageTextListDto();
            sut.Key = "test_value";
            sut.Key.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTargetValue_Entao_DeveArmazenar()
        {
            var sut = new LanguageTextListDto();
            sut.TargetValue = "test_value";
            sut.TargetValue.ShouldBe("test_value");
        }
    }
}
