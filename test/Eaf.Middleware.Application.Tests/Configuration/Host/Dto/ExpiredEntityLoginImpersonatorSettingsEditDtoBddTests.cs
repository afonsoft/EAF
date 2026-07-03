using Eaf.Configuration.Host.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Configuration.Host
{
    public class ExpiredEntityLoginImpersonatorSettingsEditDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new ExpiredEntityLoginImpersonatorSettingsEditDto();
            sut.ShouldNotBeNull();
        }
    }
}
