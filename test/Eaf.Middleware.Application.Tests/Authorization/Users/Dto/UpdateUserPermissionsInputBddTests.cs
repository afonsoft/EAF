using Eaf.Middleware.Authorization.Users.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Users
{
    public class UpdateUserPermissionsInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new UpdateUserPermissionsInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var sut = new UpdateUserPermissionsInput();
            sut.Id = 100L;
            sut.Id.ShouldBe(100L);
        }
    }
}
