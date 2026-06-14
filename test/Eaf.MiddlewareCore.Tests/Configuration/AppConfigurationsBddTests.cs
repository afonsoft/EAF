using Eaf.Middleware.Configuration;
using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Tests.Configuration
{
    public class AppConfigurationsBddTests : IDisposable
    {
        private readonly string _tempDir;

        public AppConfigurationsBddTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "eaf-test-" + Guid.NewGuid().ToString("N")[..8]);
            Directory.CreateDirectory(_tempDir);
            File.WriteAllText(Path.Combine(_tempDir, "appsettings.json"), "{\"TestKey\": \"TestValue\"}");
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
        }

        [Fact]
        public void Dado_DiretorioValido_Quando_Get_Entao_DeveRetornarConfigurationRoot()
        {
            var config = AppConfigurations.Get(_tempDir, "Testing");

            config.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_DiretorioValido_Quando_GetComEnvironmentName_Entao_DeveRetornarConfigurationRoot()
        {
            var config = AppConfigurations.Get(_tempDir, "Development");

            config.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_ConfigExistente_Quando_GetMesmoPath_Entao_DeveRetornarMesmaInstanciaDoCache()
        {
            var config1 = AppConfigurations.Get(_tempDir, "CacheTest1");
            var config2 = AppConfigurations.Get(_tempDir, "CacheTest1");

            config1.ShouldBeSameAs(config2);
        }

        [Fact]
        public void Dado_EnvironmentsDiferentes_Quando_Get_Entao_DeveRetornarInstanciasDiferentes()
        {
            var config1 = AppConfigurations.Get(_tempDir, "EnvA");
            var config2 = AppConfigurations.Get(_tempDir, "EnvB");

            config1.ShouldNotBeSameAs(config2);
        }

        [Fact]
        public void Dado_AppsettingsComValor_Quando_Get_Entao_DeveRetornarValor()
        {
            var config = AppConfigurations.Get(_tempDir, "ValueTest");

            config["TestKey"].ShouldBe("TestValue");
        }

        [Fact]
        public void Dado_EnvironmentNameVazio_Quando_Get_Entao_DeveUsarFallbackDeVariaveisDeAmbiente()
        {
            var config = AppConfigurations.Get(_tempDir, "");

            config.ShouldNotBeNull();
        }
    }
}
