using Eaf.Middleware.Localization;
using Eaf.Middleware.Localization.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Localization.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de localização seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class LocalizationDtoBddTests
    {
        #region GetLanguageTextsInput

        [Fact]
        public void Dado_GetLanguageTextsInput_SemTargetValueFilter_Quando_Normalize_Entao_DeveDefinirALL()
        {
            var input = new GetLanguageTextsInput
            {
                SourceName = "Eaf",
                TargetLanguageName = "pt-BR"
            };

            input.Normalize();
            input.TargetValueFilter.ShouldBe("ALL");
        }

        [Fact]
        public void Dado_GetLanguageTextsInput_ComTargetValueFilter_Quando_Normalize_Entao_DeveManterValor()
        {
            var input = new GetLanguageTextsInput
            {
                SourceName = "Eaf",
                TargetLanguageName = "en",
                TargetValueFilter = "EMPTY"
            };

            input.Normalize();
            input.TargetValueFilter.ShouldBe("EMPTY");
        }

        [Fact]
        public void Dado_GetLanguageTextsInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new GetLanguageTextsInput
            {
                SourceName = "EafModule",
                TargetLanguageName = "pt-BR",
                BaseLanguageName = "en",
                FilterText = "admin",
                MaxResultCount = 50,
                SkipCount = 10,
                Sorting = "Key ASC"
            };

            input.SourceName.ShouldBe("EafModule");
            input.TargetLanguageName.ShouldBe("pt-BR");
            input.BaseLanguageName.ShouldBe("en");
            input.FilterText.ShouldBe("admin");
            input.MaxResultCount.ShouldBe(50);
            input.SkipCount.ShouldBe(10);
            input.Sorting.ShouldBe("Key ASC");
        }

        #endregion

        #region ApplicationLanguageEditDto

        [Fact]
        public void Dado_ApplicationLanguageEditDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ApplicationLanguageEditDto
            {
                Id = 1,
                Name = "pt-BR",
                Icon = "famfamfam-flags br",
                IsEnabled = true
            };

            dto.Id.ShouldBe(1);
            dto.Name.ShouldBe("pt-BR");
            dto.Icon.ShouldBe("famfamfam-flags br");
            dto.IsEnabled.ShouldBeTrue();
        }

        [Fact]
        public void Dado_ApplicationLanguageEditDto_SemId_Quando_Verificar_Entao_DeveSerNull()
        {
            var dto = new ApplicationLanguageEditDto { Name = "en" };
            dto.Id.ShouldBeNull();
        }

        #endregion

        #region ApplicationLanguageListDto

        [Fact]
        public void Dado_ApplicationLanguageListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ApplicationLanguageListDto
            {
                Id = 5,
                Name = "es",
                DisplayName = "Español",
                Icon = "famfamfam-flags es",
                IsDisabled = false
            };

            dto.Id.ShouldBe(5);
            dto.Name.ShouldBe("es");
            dto.DisplayName.ShouldBe("Español");
            dto.IsDisabled.ShouldBeFalse();
        }

        #endregion

        #region FamFamFamFlagsHelper

        [Fact]
        public void Dado_FamFamFamFlagsHelper_Quando_VerificarLista_Entao_NaoDeveSerVazia()
        {
            FamFamFamFlagsHelper.FlagClassNames.ShouldNotBeEmpty();
            FamFamFamFlagsHelper.FlagClassNames.Count.ShouldBeGreaterThan(100);
        }

        [Fact]
        public void Dado_FamFamFamFlagsHelper_Quando_VerificarBrasil_Entao_DeveConterBR()
        {
            FamFamFamFlagsHelper.FlagClassNames.ShouldContain("famfamfam-flags br");
        }

        [Fact]
        public void Dado_FamFamFamFlagsHelper_Quando_VerificarUS_Entao_DeveConterUS()
        {
            FamFamFamFlagsHelper.FlagClassNames.ShouldContain("famfamfam-flags us");
        }

        #endregion

        #region LanguageTextListDto

        [Fact]
        public void Dado_LanguageTextListDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new LanguageTextListDto
            {
                Key = "LoginPage.Title",
                BaseValue = "Login",
                TargetValue = "Entrar"
            };

            dto.Key.ShouldBe("LoginPage.Title");
            dto.BaseValue.ShouldBe("Login");
            dto.TargetValue.ShouldBe("Entrar");
        }

        #endregion

        #region CreateOrUpdateLanguageInput

        [Fact]
        public void Dado_CreateOrUpdateLanguageInput_Quando_DefinirLanguage_Entao_DeveArmazenar()
        {
            var input = new CreateOrUpdateLanguageInput
            {
                Language = new ApplicationLanguageEditDto
                {
                    Name = "fr",
                    IsEnabled = true
                }
            };

            input.Language.ShouldNotBeNull();
            input.Language.Name.ShouldBe("fr");
        }

        #endregion

        #region UpdateLanguageTextInput

        [Fact]
        public void Dado_UpdateLanguageTextInput_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var input = new UpdateLanguageTextInput
            {
                LanguageName = "pt-BR",
                SourceName = "EafModule",
                Key = "LoginPage.Title",
                Value = "Entrar"
            };

            input.LanguageName.ShouldBe("pt-BR");
            input.SourceName.ShouldBe("EafModule");
            input.Key.ShouldBe("LoginPage.Title");
            input.Value.ShouldBe("Entrar");
        }

        #endregion
    }
}
