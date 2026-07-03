using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Roles
{
    public class CreateOrUpdateRoleInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CreateOrUpdateRoleInput();
            sut.ShouldNotBeNull();
        }
    }
}
