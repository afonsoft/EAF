using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Localization
{
    public class ApplicationLanguageEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ApplicationLanguageEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageEditDto();
            sut.Id = 42;
            sut.Id.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsEnabled_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageEditDto();
            sut.IsEnabled = true;
            sut.IsEnabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageEditDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }
    }
}
