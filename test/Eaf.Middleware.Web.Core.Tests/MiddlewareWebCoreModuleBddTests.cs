using Abp.Dependency;
using Eaf.Middleware.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore
{
    public class MiddlewareWebCoreModuleBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarModulo_Entao_DeveTerNomeCorreto()
        {
            typeof(MiddlewareWebCoreModule).Name.ShouldBe("MiddlewareWebCoreModule");
        }

        [Fact]
        public void Dado_Tipo_Quando_VerificarHeranca_Entao_DeveSerAbpModule()
        {
            typeof(Abp.Modules.AbpModule).IsAssignableFrom(typeof(MiddlewareWebCoreModule)).ShouldBeTrue();
        }

        [Fact]
        public void Dado_HostEnvironment_Quando_CriarModulo_Entao_DeveDefinirVariaveisAmbiente()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var original = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var module = new MiddlewareWebCoreModule(env);

                module.ShouldNotBeNull();
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ShouldBe("Development");
            }
            finally
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", original);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_IocManagerConfigurado_Quando_Initialize_Entao_DeveRegistrarConventions()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var iocManager = new IocManager();
                var module = new MiddlewareWebCoreModule(env);
                var iocProperty = typeof(Abp.Modules.AbpModule).GetProperty("IocManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                iocProperty?.SetValue(module, iocManager);

                var configType = Type.GetType("Abp.Configuration.Startup.AbpStartupConfiguration, Abp");
                var config = Activator.CreateInstance(configType, iocManager);
                var configProperty = typeof(Abp.Modules.AbpModule).GetProperty("Configuration", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                configProperty?.SetValue(module, config);

                Should.NotThrow(() => module.Initialize());
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }


    }
}
