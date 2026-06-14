using Eaf.Middleware.Localization;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Localization
{
    /// <summary>
    /// Testes BDD para FamFamFamFlagsHelper seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class FamFamFamFlagsHelperBddTests
    {
        [Fact]
        public void Dado_FamFamFamFlagsHelper_Quando_AcessarFlagClassNames_Entao_DeveConterBandeiras()
        {
            FamFamFamFlagsHelper.FlagClassNames.ShouldNotBeNull();
            FamFamFamFlagsHelper.FlagClassNames.Count.ShouldBeGreaterThan(200);
            FamFamFamFlagsHelper.FlagClassNames.ShouldContain("famfamfam-flags br");
            FamFamFamFlagsHelper.FlagClassNames.ShouldContain("famfamfam-flags us");
        }

        [Theory]
        [InlineData("famfamfam-flags br", "br")]
        [InlineData("famfamfam-flags us", "us")]
        [InlineData("famfamfam-flags wales", "wales")]
        [InlineData("famfamfam-flags zw", "zw")]
        public void Dado_FlagName_Quando_GetCountryCode_Entao_DeveRetornarCodigo(string flagName, string expected)
        {
            FamFamFamFlagsHelper.GetCountryCode(flagName).ShouldBe(expected);
        }

        [Fact]
        public void Dado_FamFamFamFlagsHelper_Quando_VerificarPrimeiro_Entao_DeveSerZW()
        {
            FamFamFamFlagsHelper.FlagClassNames[0].ShouldBe("famfamfam-flags zw");
        }

        [Fact]
        public void Dado_FamFamFamFlagsHelper_Quando_VerificarUltimo_Entao_DeveSerAD()
        {
            var last = FamFamFamFlagsHelper.FlagClassNames[^1];
            last.ShouldBe("famfamfam-flags ch");
        }
    }
}
