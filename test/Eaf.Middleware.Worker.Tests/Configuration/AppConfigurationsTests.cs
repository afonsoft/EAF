using Eaf.Middleware.Configuration;
using Shouldly;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Configuration
{
    public class AppConfigurationsTests
    {
        [Fact]
        public void Dado_AppConfigurations_Quando_GetComPathValido_Entao_DeveRetornarConfigurationRoot()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "eaf-test-" + Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), "{}");

            try
            {
                var config = AppConfigurations.Get(tempDir, "Test");
                config.ShouldNotBeNull();
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [Fact]
        public void Dado_AppConfigurations_Quando_GetComMesmoPath_Entao_DeveRetornarMesmaInstanciaDoCache()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), "eaf-test-cache-" + Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            File.WriteAllText(Path.Combine(tempDir, "appsettings.json"), "{}");

            try
            {
                var config1 = AppConfigurations.Get(tempDir, "CacheTest");
                var config2 = AppConfigurations.Get(tempDir, "CacheTest");
                config1.ShouldBeSameAs(config2);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
}
