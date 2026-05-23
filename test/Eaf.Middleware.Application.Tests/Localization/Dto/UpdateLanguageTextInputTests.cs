using Eaf.Middleware.Localization.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Localization.Dto
{
    public class UpdateLanguageTextInputTests
    {
        [Fact]
        public void Dado_UpdateLanguageTextInput_Quando_Criado_Entao_PropriedadesDevemSerNulas()
        {
            var input = new UpdateLanguageTextInput();

            input.Key.ShouldBeNull();
            input.LanguageName.ShouldBeNull();
            input.SourceName.ShouldBeNull();
            input.Value.ShouldBeNull();
        }

        [Fact]
        public void Dado_UpdateLanguageTextInput_Quando_AtribuirPropriedades_Entao_DevemSerRetornadas()
        {
            var input = new UpdateLanguageTextInput
            {
                Key = "HomePage.Title",
                LanguageName = "pt-BR",
                SourceName = "EafMiddleware",
                Value = "Página Inicial"
            };

            input.Key.ShouldBe("HomePage.Title");
            input.LanguageName.ShouldBe("pt-BR");
            input.SourceName.ShouldBe("EafMiddleware");
            input.Value.ShouldBe("Página Inicial");
        }

        [Theory]
        [InlineData(nameof(UpdateLanguageTextInput.Key))]
        [InlineData(nameof(UpdateLanguageTextInput.LanguageName))]
        [InlineData(nameof(UpdateLanguageTextInput.SourceName))]
        [InlineData(nameof(UpdateLanguageTextInput.Value))]
        public void Dado_UpdateLanguageTextInput_Quando_Verificado_Entao_PropriedadeDeveConterRequiredAttribute(string propertyName)
        {
            var prop = typeof(UpdateLanguageTextInput).GetProperty(propertyName);
            prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault().ShouldNotBeNull();
        }
    }
}
