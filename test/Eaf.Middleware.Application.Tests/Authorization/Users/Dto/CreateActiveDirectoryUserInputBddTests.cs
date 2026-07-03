using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class CreateActiveDirectoryUserInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CreateActiveDirectoryUserInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsActive_Entao_DeveArmazenar()
        {
            var sut = new CreateActiveDirectoryUserInput();
            sut.IsActive = true;
            sut.IsActive.ShouldBe(true);
        }
    }
}
