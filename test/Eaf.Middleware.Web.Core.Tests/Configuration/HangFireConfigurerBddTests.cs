using Eaf.Middleware.Web.Startup;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class HangFireConfigurerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(HangFireConfigurer).IsAbstract.ShouldBeTrue();
            typeof(HangFireConfigurer).IsSealed.ShouldBeTrue();
        }
    }
}
