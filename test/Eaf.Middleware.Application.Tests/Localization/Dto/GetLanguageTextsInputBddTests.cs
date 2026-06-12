using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Localization.Dto
{
    /// <summary>
    /// Testes BDD para GetLanguageTextsInput seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class GetLanguageTextsInputBddTests
    {
        [Fact]
        public void Dado_TargetValueFilterVazio_Quando_Normalize_Entao_DeveDefinirComoALL()
        {
            var input = new GetLanguageTextsInput();
            input.Normalize();
            input.TargetValueFilter.ShouldBe("ALL");
        }

        [Fact]
        public void Dado_TargetValueFilterNull_Quando_Normalize_Entao_DeveDefinirComoALL()
        {
            var input = new GetLanguageTextsInput { TargetValueFilter = null };
            input.Normalize();
            input.TargetValueFilter.ShouldBe("ALL");
        }

        [Fact]
        public void Dado_TargetValueFilterPreenchido_Quando_Normalize_Entao_NaoDeveAlterar()
        {
            var input = new GetLanguageTextsInput { TargetValueFilter = "EMPTY" };
            input.Normalize();
            input.TargetValueFilter.ShouldBe("EMPTY");
        }

        [Fact]
        public void Dado_GetLanguageTextsInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetLanguageTextsInput
            {
                SourceName = "EafCore",
                BaseLanguageName = "en",
                TargetLanguageName = "pt-BR",
                FilterText = "Hello",
                MaxResultCount = 100,
                SkipCount = 0,
                Sorting = "Key"
            };

            input.SourceName.ShouldBe("EafCore");
            input.BaseLanguageName.ShouldBe("en");
            input.TargetLanguageName.ShouldBe("pt-BR");
            input.FilterText.ShouldBe("Hello");
            input.MaxResultCount.ShouldBe(100);
            input.SkipCount.ShouldBe(0);
            input.Sorting.ShouldBe("Key");
        }
    }
}
