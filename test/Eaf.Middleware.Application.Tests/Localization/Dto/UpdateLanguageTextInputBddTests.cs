using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Localization
{
    public class UpdateLanguageTextInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UpdateLanguageTextInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirLanguageName_Entao_DeveArmazenar()
        {
            var sut = new UpdateLanguageTextInput();
            sut.LanguageName = "test_value";
            sut.LanguageName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirSourceName_Entao_DeveArmazenar()
        {
            var sut = new UpdateLanguageTextInput();
            sut.SourceName = "test_value";
            sut.SourceName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirValue_Entao_DeveArmazenar()
        {
            var sut = new UpdateLanguageTextInput();
            sut.Value = "test_value";
            sut.Value.ShouldBe("test_value");
        }
    }
}
