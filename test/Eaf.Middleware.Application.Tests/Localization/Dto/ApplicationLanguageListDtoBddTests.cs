using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Localization
{
    public class ApplicationLanguageListDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ApplicationLanguageListDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirDisplayName_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageListDto();
            sut.DisplayName = "test_value";
            sut.DisplayName.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIcon_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageListDto();
            sut.Icon = "test_value";
            sut.Icon.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsDisabled_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageListDto();
            sut.IsDisabled = true;
            sut.IsDisabled.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirName_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageListDto();
            sut.Name = "test_value";
            sut.Name.ShouldBe("test_value");
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var sut = new ApplicationLanguageListDto();
            sut.TenantId = 42;
            sut.TenantId.ShouldBe(42);
        }
    }
}
