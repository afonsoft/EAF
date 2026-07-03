using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class GetUsersInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetUsersInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirFilter_Entao_DeveArmazenar()
        {
            var sut = new GetUsersInput();
            sut.Filter = "test_value";
            sut.Filter.ShouldBe("test_value");
        }
    }
}
