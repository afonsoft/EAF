using Eaf.Middleware.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Configuration
{
    public class HostingEnvironmentExtensionsTests
    {
        [Fact]
        public void Dado_IHostEnvironment_Quando_GetAppConfiguration_Entao_DeveRetornarConfigurationRoot()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "eaf-host-ext-" + Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), "{}");

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDir);
                env.EnvironmentName.Returns("Testing");

                var config = env.GetAppConfiguration();
                config.ShouldNotBeNull();
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
