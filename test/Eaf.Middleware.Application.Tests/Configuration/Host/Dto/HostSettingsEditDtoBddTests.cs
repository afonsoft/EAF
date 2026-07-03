using Eaf.Middleware.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class HostSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new HostSettingsEditDto();
            sut.ShouldNotBeNull();
        }
    }
}
