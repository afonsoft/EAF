using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class ImpersonateInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ImpersonateInput();
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instancia_Quando_DefinirUserId_Entao_DeveArmazenar()
        {
            var sut = new ImpersonateInput();
            sut.UserId = 100L;
            sut.UserId.ShouldBe(100L);
        }
    }
}
