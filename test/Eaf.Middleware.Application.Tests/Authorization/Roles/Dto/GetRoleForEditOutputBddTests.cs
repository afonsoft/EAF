using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Roles
{
    public class GetRoleForEditOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetRoleForEditOutput();
            sut.ShouldNotBeNull();
        }
    }
}
