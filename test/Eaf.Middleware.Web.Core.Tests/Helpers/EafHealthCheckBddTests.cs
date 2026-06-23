using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Web.Core.Helpers
{
    /// <summary>
    /// Testes BDD para EafHealthCheck extensions seguindo o padrao Dado/Quando/Entao.
    /// </summary>
    public class EafHealthCheckBddTests
    {
        #region AddEafHealthChecks

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafHealthChecks_Entao_DeveRetornarHealthChecksBuilder()
        {
            // Dado
            var services = new ServiceCollection();

            // Quando
            var result = services.AddEafHealthChecks();

            // Entao
            result.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ServiceCollection_Quando_AddEafHealthChecks_Entao_DeveRegistrarServicosDeHealthCheck()
        {
            // Dado
            var services = new ServiceCollection();

            // Quando
            services.AddEafHealthChecks();

            // Entao
            services.Count.ShouldBeGreaterThan(0);
        }

        #endregion
    }
}
