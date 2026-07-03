using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class CreateLdapUserInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CreateLdapUserInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsActive_Entao_DeveArmazenar()
        {
            var sut = new CreateLdapUserInput();
            sut.IsActive = true;
            sut.IsActive.ShouldBe(true);
        }
    }
}
