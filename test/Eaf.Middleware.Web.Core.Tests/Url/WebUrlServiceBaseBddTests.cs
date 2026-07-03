using Eaf.Middleware.Web.Url;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Url
{
    public class WebUrlServiceBaseBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarAbstrata_Entao_DeveSerAbstrata()
        {
            typeof(WebUrlServiceBase).IsAbstract.ShouldBeTrue();
        }

        [Fact]
        public void Dado_Constante_Quando_VerificarTenancyNamePlaceHolder_Entao_DeveSerCorreta()
        {
            WebUrlServiceBase.TenancyNamePlaceHolder.ShouldBe("{TENANCY_NAME}");
        }
    }
}
