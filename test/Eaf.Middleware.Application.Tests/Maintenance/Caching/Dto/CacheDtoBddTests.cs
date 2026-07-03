using Eaf.Middleware.Maintenance.Caching.Dto;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Maintenance.Caching
{
    public class CacheDtoBddTests
    {
        [Fact]
        public void Dado_NovaInstancia_Quando_Criar_Entao_DeveInicializarCorretamente()
        {
            var sut = new CacheDto();
            sut.ShouldNotBeNull();
        }
    }
}
