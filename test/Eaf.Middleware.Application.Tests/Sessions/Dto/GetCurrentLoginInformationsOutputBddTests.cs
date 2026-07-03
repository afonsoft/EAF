using Eaf.Middleware.Sessions.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Sessions
{
    public class GetCurrentLoginInformationsOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetCurrentLoginInformationsOutput();
            sut.ShouldNotBeNull();
        }
    }
}
