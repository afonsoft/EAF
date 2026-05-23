using Eaf.Middleware.Localization;
using Shouldly;
using System.Globalization;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Localization
{
    public class CultureHelperTests
    {
        [Fact]
        public void Dado_NomeValido_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCultureCorreta()
        {
            var culture = CultureHelper.GetCultureInfoByChecking("en-US");
            culture.Name.ShouldBe("en-US");
        }

        [Fact]
        public void Dado_NomeInvalido_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCurrentCulture()
        {
            var culture = CultureHelper.GetCultureInfoByChecking("xx-INVALID-YY");
            culture.ShouldBe(CultureInfo.CurrentCulture);
        }

        [Fact]
        public void Dado_NomePtBR_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCulturePtBR()
        {
            var culture = CultureHelper.GetCultureInfoByChecking("pt-BR");
            culture.Name.ShouldBe("pt-BR");
        }

        [Fact]
        public void Dado_AllCultures_Quando_Verificar_Entao_DeveConterCultures()
        {
            CultureHelper.AllCultures.ShouldNotBeNull();
            CultureHelper.AllCultures.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_NomeVazio_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCultureInvariant()
        {
            var culture = CultureHelper.GetCultureInfoByChecking("");
            culture.ShouldNotBeNull();
        }
    }
}
