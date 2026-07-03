using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.HealthChecks
{
    public class EafHealthCheckApplicationBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(Microsoft.AspNetCore.Builder.EafHealthCheckApplicationBuilderExtensions).IsAbstract.ShouldBeTrue();
            typeof(Microsoft.AspNetCore.Builder.EafHealthCheckApplicationBuilderExtensions).IsSealed.ShouldBeTrue();
        }
    }
}
