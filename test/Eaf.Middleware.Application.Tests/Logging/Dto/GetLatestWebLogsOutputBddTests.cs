using Eaf.Middleware.Logging.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Logging
{
    public class GetLatestWebLogsOutputBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new GetLatestWebLogsOutput();
            sut.ShouldNotBeNull();
        }
    }
}
