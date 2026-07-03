using Eaf.Middleware.UiCustomization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.UiCustomization
{
    public class UiCustomizationSettingsDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UiCustomizationSettingsDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsLeftMenuUsed_Entao_DeveArmazenar()
        {
            var sut = new UiCustomizationSettingsDto();
            sut.IsLeftMenuUsed = true;
            sut.IsLeftMenuUsed.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsTabMenuUsed_Entao_DeveArmazenar()
        {
            var sut = new UiCustomizationSettingsDto();
            sut.IsTabMenuUsed = true;
            sut.IsTabMenuUsed.ShouldBe(true);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsTopMenuUsed_Entao_DeveArmazenar()
        {
            var sut = new UiCustomizationSettingsDto();
            sut.IsTopMenuUsed = true;
            sut.IsTopMenuUsed.ShouldBe(true);
        }
    }
}
