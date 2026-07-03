using Eaf.Middleware.Web.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class AppConfigurationAccessorBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarNome_Entao_DeveSerCorreto()
        {
            typeof(AppConfigurationAccessor).Name.ShouldBe("AppConfigurationAccessor");
        }
    }
}
