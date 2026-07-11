using Eaf.Middleware.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Shouldly;
using System;
using System.IO;
using Xunit;

namespace Eaf.MiddlewareCore.Tests.Configuration
{
    public class HostingEnvironmentExtensionsBddTests
    {
        [Fact]
        public void Dado_Tipo_Quando_VerificarClasse_Entao_DeveSerStatica()
        {
            typeof(Eaf.Middleware.Configuration.HostingEnvironmentExtensions).IsAbstract.ShouldBeTrue();
            typeof(Eaf.Middleware.Configuration.HostingEnvironmentExtensions).IsSealed.ShouldBeTrue();
        }

        [Fact]
        public void Dado_WebHostEnvironment_Quando_GetAppConfiguration_Entao_DeveRetornarConfigurationRoot()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IWebHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Development");

                var config = env.GetAppConfiguration();

                config.ShouldNotBeNull();
                config.ShouldBeAssignableTo<IConfigurationRoot>();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostEnvironment_Quando_GetAppConfiguration_Entao_DeveRetornarConfigurationRoot()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);

            try
            {
                var env = Substitute.For<IHostEnvironment>();
                env.ContentRootPath.Returns(tempDirectory);
                env.EnvironmentName.Returns("Production");

                var config = env.GetAppConfiguration();

                config.ShouldNotBeNull();
                config.ShouldBeAssignableTo<IConfigurationRoot>();
            }
            finally
            {
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }
    }
}
