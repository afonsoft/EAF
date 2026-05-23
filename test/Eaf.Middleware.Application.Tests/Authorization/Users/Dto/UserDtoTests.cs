using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class UserDtoTests
    {
        [Fact]
        public void Dado_ChangeUserLanguageDto_Quando_DefinirLanguageName_Entao_DeveRetornarCorreto()
        {
            var dto = new ChangeUserLanguageDto
            {
                LanguageName = "pt-BR"
            };

            dto.LanguageName.ShouldBe("pt-BR");
        }
    }
}
