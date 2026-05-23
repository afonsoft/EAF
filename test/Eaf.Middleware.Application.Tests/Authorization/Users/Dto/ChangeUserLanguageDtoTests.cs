using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class ChangeUserLanguageDtoTests
    {
        [Fact]
        public void Dado_ChangeUserLanguageDto_Quando_Criado_Entao_LanguageNameDeveSerNulo()
        {
            var dto = new ChangeUserLanguageDto();
            dto.LanguageName.ShouldBeNull();
        }

        [Fact]
        public void Dado_ChangeUserLanguageDto_Quando_AtribuirLanguageName_Entao_DeveRetornarValor()
        {
            var dto = new ChangeUserLanguageDto { LanguageName = "pt-BR" };
            dto.LanguageName.ShouldBe("pt-BR");
        }

        [Fact]
        public void Dado_ChangeUserLanguageDto_Quando_Verificado_Entao_LanguageNameDeveConterRequiredAttribute()
        {
            var prop = typeof(ChangeUserLanguageDto).GetProperty(nameof(ChangeUserLanguageDto.LanguageName));
            var attr = prop!.GetCustomAttributes(typeof(RequiredAttribute), false).FirstOrDefault();
            attr.ShouldNotBeNull();
        }
    }
}
