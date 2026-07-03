using Eaf.Middleware.Web.UiCustomization.Metronic;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.UiCustomization.Metronic
{
    public class Theme3UiCustomizerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveHerdarDeUiThemeCustomizerBase()
        {
            typeof(Theme3UiCustomizer).BaseType.ShouldBe(typeof(UiThemeCustomizerBase));
        }
    }
}
