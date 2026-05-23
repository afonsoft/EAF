using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Application.Tests.Authorization.Users.Dto
{
    public class GetUsersInputTests
    {
        [Fact]
        public void Dado_GetUsersInput_Quando_Criado_Entao_FilterDeveSerStringVazia()
        {
            var input = new GetUsersInput();
            input.Filter.ShouldBe("");
        }

        [Fact]
        public void Dado_GetUsersInput_Quando_SortingNulo_Entao_NormalizeDeveDefinirComoNameSurname()
        {
            var input = new GetUsersInput();
            input.Normalize();
            input.Sorting.ShouldBe("Name,Surname");
        }

        [Fact]
        public void Dado_GetUsersInput_Quando_SortingDefinido_Entao_NormalizeNaoDeveAlterar()
        {
            var input = new GetUsersInput { Sorting = "UserName ASC" };
            input.Normalize();
            input.Sorting.ShouldBe("UserName ASC");
        }
    }
}
