using Eaf.Middleware.Authorization;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Authorization
{
    public class AbpLoginResultTypeHelperBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new AbpLoginResultTypeHelper();
            sut.ShouldNotBeNull();
        }
    }
}
