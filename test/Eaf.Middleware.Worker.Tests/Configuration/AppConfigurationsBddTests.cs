using Eaf.Middleware.Configuration;
using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Configuration
{
    /// <summary>
    /// Testes BDD para AppConfigurations do Worker.
    /// </summary>
    public class AppConfigurationsBddTests : IDisposable
    {
        private readonly string _tempDir;
        private readonly string? _originalAspNetCoreEnv;
        private readonly string? _originalEafEnv;
        private readonly string? _originalHostingEnv;
        private readonly string? _originalAspNetEnv;
        private readonly string? _originalDotNetEnv;

        public AppConfigurationsBddTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "eaf-worker-test-" + Path.GetRandomFileName());
            Directory.CreateDirectory(_tempDir);
            File.WriteAllText(Path.Combine(_tempDir, "appsettings.json"), "{}");

            _originalAspNetCoreEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            _originalEafEnv = Environment.GetEnvironmentVariable("EAF_ENVIRONMENT");
            _originalHostingEnv = Environment.GetEnvironmentVariable("Hosting:Environment");
            _originalAspNetEnv = Environment.GetEnvironmentVariable("ASPNET_ENV");
            _originalDotNetEnv = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        }

        public void Dispose()
        {
            try { Directory.Delete(_tempDir, true); } catch { }
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _originalAspNetCoreEnv);
            Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", _originalEafEnv);
            Environment.SetEnvironmentVariable("Hosting:Environment", _originalHostingEnv);
            Environment.SetEnvironmentVariable("ASPNET_ENV", _originalAspNetEnv);
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", _originalDotNetEnv);
        }

        [Fact]
        public void Dado_EnvironmentNameNuloComAspNetCoreEnv_Quando_Get_Entao_DeveUsarAspNetCoreEnvironment()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging");
            Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);

            var config = AppConfigurations.Get(_tempDir, null);

            config.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_EnvironmentNameNuloComEafEnv_Quando_Get_Entao_DeveUsarEafEnvironment()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", "Staging");

            var config = AppConfigurations.Get(_tempDir, null);

            config.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_EnvironmentNameNuloComHostingEnv_Quando_Get_Entao_DeveUsarHostingEnvironment()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("Hosting:Environment", "Staging");

            var config = AppConfigurations.Get(_tempDir, null);

            config.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_EnvironmentNameNuloComDotNetEnv_Quando_Get_Entao_DeveUsarDotNetEnvironment()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("Hosting:Environment", null);
            Environment.SetEnvironmentVariable("ASPNET_ENV", null);
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Staging");

            var config = AppConfigurations.Get(_tempDir, null);

            config.ShouldNotBeNull();
        }

        [Fact]
        public void Dado_EnvironmentNameNuloSemVariaveis_Quando_Get_Entao_DeveRetornarConfigurationRoot()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("EAF_ENVIRONMENT", null);
            Environment.SetEnvironmentVariable("Hosting:Environment", null);
            Environment.SetEnvironmentVariable("ASPNET_ENV", null);
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", null);

            var config = AppConfigurations.Get(_tempDir, null);

            config.ShouldNotBeNull();
        }
    }
}
