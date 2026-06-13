using Eaf.Middleware.Authorization.Roles.Dto;
using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    /// <summary>
    /// Testes BDD para DTOs de entrada de Users e Roles com Normalize seguindo o padrão Dado/Quando/Então
    /// </summary>
    public class UserInputDtoBddTests
    {
        #region GetUsersInput.Normalize

        [Fact]
        public void Dado_GetUsersInput_SemSorting_Quando_Normalize_Entao_DeveDefinirPadrao()
        {
            var input = new GetUsersInput();
            input.Normalize();
            input.Sorting.ShouldBe("Name,Surname");
        }

        [Fact]
        public void Dado_GetUsersInput_ComSorting_Quando_Normalize_Entao_DeveManterValor()
        {
            var input = new GetUsersInput { Sorting = "CreationTime DESC" };
            input.Normalize();
            input.Sorting.ShouldBe("CreationTime DESC");
        }

        [Fact]
        public void Dado_GetUsersInput_Quando_VerificarFilterPadrao_Entao_DeveSerVazio()
        {
            var input = new GetUsersInput();
            input.Filter.ShouldBe("");
        }

        #endregion

        #region GetRolesInput.Normalize

        [Fact]
        public void Dado_GetRolesInput_SemSorting_Quando_Normalize_Entao_DeveDefinirPadrao()
        {
            var input = new GetRolesInput();
            input.Normalize();
            input.Sorting.ShouldBe("Name");
        }

        [Fact]
        public void Dado_GetRolesInput_ComSorting_Quando_Normalize_Entao_DeveManterValor()
        {
            var input = new GetRolesInput { Sorting = "DisplayName DESC" };
            input.Normalize();
            input.Sorting.ShouldBe("DisplayName DESC");
        }

        [Fact]
        public void Dado_GetRolesInput_Quando_VerificarFilterPadrao_Entao_DeveSerVazio()
        {
            var input = new GetRolesInput();
            input.Filter.ShouldBe("");
        }

        #endregion

        #region ChangeUserLanguageDto

        [Fact]
        public void Dado_ChangeUserLanguageDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new ChangeUserLanguageDto { LanguageName = "pt-BR" };
            dto.LanguageName.ShouldBe("pt-BR");
        }

        #endregion

        #region UserLoginAttemptDto

        [Fact]
        public void Dado_UserLoginAttemptDto_Quando_DefinirPropriedades_Entao_DeveArmazenar()
        {
            var dto = new UserLoginAttemptDto
            {
                UserNameOrEmail = "admin@acme.com",
                ClientIpAddress = "192.168.1.1",
                ClientName = "Chrome",
                Result = "Success",
                BrowserInfo = "Chrome 120",
                TenancyName = "acme"
            };

            dto.UserNameOrEmail.ShouldBe("admin@acme.com");
            dto.ClientIpAddress.ShouldBe("192.168.1.1");
            dto.ClientName.ShouldBe("Chrome");
            dto.Result.ShouldBe("Success");
        }

        #endregion
    }
}
