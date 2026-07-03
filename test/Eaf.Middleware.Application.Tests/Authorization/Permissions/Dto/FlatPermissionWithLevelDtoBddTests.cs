using Eaf.Middleware.Authorization.Permissions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Permissions
{
    public class FlatPermissionWithLevelDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new FlatPermissionWithLevelDto();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirLevel_Entao_DeveArmazenar()
        {
            var sut = new FlatPermissionWithLevelDto();
            sut.Level = 42;
            sut.Level.ShouldBe(42);
        }
    }
}
