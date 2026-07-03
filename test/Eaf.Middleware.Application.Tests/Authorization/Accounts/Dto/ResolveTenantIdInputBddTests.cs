using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class ResolveTenantIdInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ResolveTenantIdInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_Definirc_Entao_DeveArmazenar()
        {
            var sut = new ResolveTenantIdInput();
            sut.c = "test_value";
            sut.c.ShouldBe("test_value");
        }
    }
}
