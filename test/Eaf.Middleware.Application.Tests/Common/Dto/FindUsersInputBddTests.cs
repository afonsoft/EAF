using Eaf.Middleware.Common.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Common
{
    public class FindUsersInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new FindUsersInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirTenantId_Entao_DeveArmazenar()
        {
            var sut = new FindUsersInput();
            sut.TenantId = 42;
            sut.TenantId.ShouldBe(42);
        }
    }
}
