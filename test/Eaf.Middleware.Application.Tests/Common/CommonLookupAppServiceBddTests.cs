using Eaf.Middleware.Common;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Common
{
    public class CommonLookupAppServiceBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CommonLookupAppService();
            sut.ShouldNotBeNull();
        }
    }
}
