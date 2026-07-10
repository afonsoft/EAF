using Eaf.Middleware.Localization;
using Shouldly;
using System.Globalization;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Localization
{
    /// <summary>
    /// Testes BDD para CultureHelper seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class CultureHelperBddTests
    {
        [Fact]
        public void Dado_NomeCulturaValido_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCultura()
        {
            // Dado
            var name = "pt-BR";

            // Quando
            var result = CultureHelper.GetCultureInfoByChecking(name);

            // Então
            result.ShouldNotBeNull();
            result.Name.ShouldBe("pt-BR");
        }

        [Fact]
        public void Dado_NomeCulturaInvalido_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCulturaAtual()
        {
            // Dado
            var name = "cultura-inexistente";

            // Quando
            var result = CultureHelper.GetCultureInfoByChecking(name);

            // Então
            result.ShouldNotBeNull();
            result.ShouldBe(CultureInfo.CurrentCulture);
        }

        [Fact]
        public void Dado_CulturaNaoDisponivelGlobalmente_Quando_GetCultureInfoByChecking_Entao_DeveRetornarCulturaAtual()
        {
            // Dado
            var name = "zz-ZZ";

            // Quando
            var result = CultureHelper.GetCultureInfoByChecking(name);

            // Então
            result.ShouldNotBeNull();
            result.ShouldBe(CultureInfo.CurrentCulture);
        }

        [Fact]
        public void Dado_NomeCulturaVazio_Quando_GetCultureInfoByChecking_Entao_DeveRetornarInvariantCulture()
        {
            // Dado
            var name = "";

            // Quando
            var result = CultureHelper.GetCultureInfoByChecking(name);

            // Então
            result.ShouldNotBeNull();
            result.Name.ShouldBe(CultureInfo.InvariantCulture.Name);
        }

        [Fact]
        public void Dado_CulturaHelper_Quando_VerificarIsRtl_Entao_DeveRetornarBool()
        {
            // Dado / Quando
            var result = CultureHelper.IsRtl;

            // Então
            result.ShouldBe(CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft);
        }

        [Fact]
        public void Dado_CulturaHelper_Quando_VerificarUsingLunarCalendar_Entao_DeveRetornarBool()
        {
            // Dado / Quando
            var result = CultureHelper.UsingLunarCalendar;

            // Então
            result.ShouldBe(CultureInfo.CurrentUICulture.DateTimeFormat.Calendar.AlgorithmType == CalendarAlgorithmType.LunarCalendar);
        }
    }
}
