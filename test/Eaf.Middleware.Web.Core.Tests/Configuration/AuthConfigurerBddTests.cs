using Eaf.Middleware.Web.Startup;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class AuthConfigurerBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(AuthConfigurer).IsAbstract.ShouldBeTrue();
            typeof(AuthConfigurer).IsSealed.ShouldBeTrue();
        }
    }
}
