using Eaf.Middleware.Localization;
using Shouldly;
using System.Globalization;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Localization
{
    public class CultureHelperBddTests
    {
        [Fact]
        public void Dado_CulturaValida_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCulturaCorreta()
        {
            var result = CultureHelper.GetCultureInfoByChecking("en-US");
            result.Name.ShouldBe("en-US");
        }

        [Fact]
        public void Dado_CulturaInvalida_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCurrentCulture()
        {
            var result = CultureHelper.GetCultureInfoByChecking("cultura-inexistente-xyz");
            result.Name.ShouldBe(CultureInfo.CurrentCulture.Name);
        }

        [Fact]
        public void Dado_CulturaInexistenteNaLista_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCurrentCulture()
        {
            var result = CultureHelper.GetCultureInfoByChecking("en_US");
            result.Name.ShouldBe(CultureInfo.CurrentCulture.Name);
        }

        [Fact]
        public void Dado_CulturaEmMaiusculas_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCulturaIgnorandoCase()
        {
            var result = CultureHelper.GetCultureInfoByChecking("EN-us");
            result.Name.ShouldBe("en-US");
        }

        [Fact]
        public void Dado_CultureHelper_Quando_AcessarAllCultures_Entao_DeveConterCulturas()
        {
            CultureHelper.AllCultures.Length.ShouldBeGreaterThan(0);
        }

        [Fact]
        public void Dado_CultureHelper_Quando_AcessarIsRtl_Entao_DeveRetornarBoolean()
        {
            var isRtl = CultureHelper.IsRtl;
            isRtl.ShouldBe(false);
        }

        [Fact]
        public void Dado_CultureHelper_Quando_AcessarUsingLunarCalendar_Entao_DeveRetornarBoolean()
        {
            var usingLunarCalendar = CultureHelper.UsingLunarCalendar;
            usingLunarCalendar.ShouldBe(false);
        }

        [Fact]
        public void Dado_CulturaComCaracteresInvalidos_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCurrentCulture()
        {
            var result = CultureHelper.GetCultureInfoByChecking("\n");
            result.Name.ShouldBe(CultureInfo.CurrentCulture.Name);
        }
    }
}
