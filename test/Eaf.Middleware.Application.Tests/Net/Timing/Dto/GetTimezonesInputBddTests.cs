using Eaf.Middleware.Timing.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Net.Timing
{
    public class GetTimezonesInputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetTimezonesInput();
            sut.ShouldNotBeNull();
        }
    }
}
