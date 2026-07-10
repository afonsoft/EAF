using Eaf.Middleware.Configuration;
using Eaf.Middleware.Web.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Configuration
{
    public class AppConfigurationAccessorBddTests
    {
        [Fact]
        public void Dado_AmbienteValido_Quando_CriarAcessor_Entao_DeveTerConfigurationRoot()
        {
            // Dado
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Test");

            // Quando
            var accessor = new AppConfigurationAccessor(hostEnvironment);

            // Então
            accessor.ShouldNotBeNull();
            accessor.Configuration.ShouldNotBeNull();
            accessor.ShouldBeAssignableTo<IAppConfigurationAccessor>();
        }

        [Fact]
        public void Dado_AmbienteDeTestes_Quando_CriarAcessor_Entao_ConfigurationDeveSerMesmaInstancia()
        {
            // Dado
            var tempDir = Path.GetTempPath();
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.ContentRootPath.Returns(tempDir);
            hostEnvironment.EnvironmentName.Returns("Development");

            // Quando
            var accessor = new AppConfigurationAccessor(hostEnvironment);
            var configuration = accessor.Configuration;

            // Então
            configuration.ShouldNotBeNull();
            accessor.Configuration.ShouldBeSameAs(configuration);
        }
    }
}
