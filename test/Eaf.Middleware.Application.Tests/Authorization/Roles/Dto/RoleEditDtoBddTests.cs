using Eaf.Middleware.Authorization.Roles.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Roles
{
    public class RoleEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new RoleEditDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirId_Entao_DeveArmazenar()
        {
            var sut = new RoleEditDto();
            sut.Id = 42;
            sut.Id.ShouldBe(42);
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirIsDefault_Entao_DeveArmazenar()
        {
            var sut = new RoleEditDto();
            sut.IsDefault = true;
            sut.IsDefault.ShouldBe(true);
        }
    }
}
