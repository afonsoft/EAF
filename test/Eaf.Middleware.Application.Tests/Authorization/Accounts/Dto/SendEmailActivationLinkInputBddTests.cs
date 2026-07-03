using Eaf.Middleware.Authorization.Accounts.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization.Accounts
{
    public class SendEmailActivationLinkInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new SendEmailActivationLinkInput();
            sut.ShouldNotBeNull();
        }
    }
}
