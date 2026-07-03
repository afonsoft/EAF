using Eaf.Middleware.Url;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Application.Net.Url
{
    public class NullAppUrlServiceBddTests
    {
        [Fact]
        public void Dado_Instance_Quando_Acessar_Entao_DeveRetornarInstancia()
        {
            var sut = NullAppUrlService.Instance;
            sut.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_Instance_Quando_AcessarDuasVezes_Entao_DeveSerMesmaInstancia()
        {
            var sut1 = NullAppUrlService.Instance;
            var sut2 = NullAppUrlService.Instance;
            sut1.ShouldBeSameAs(sut2);
        }
    }
}
