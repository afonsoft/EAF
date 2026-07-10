using Eaf.Middleware.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Eaf.Middleware.Worker.Tests.Serilog
{
    public class SerilogEafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_HostBuilder_Quando_UsarEafSerilogComNivel_Entao_DeveRetornarMesmoBuilder()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>()));

                var result = builder.UseEafSerilog(LogEventLevel.Information);

                result.ShouldBeSameAs(builder);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostBuilderComConfigElastic_Quando_UsarEafSerilog_Entao_DeveRetornarMesmoBuilder()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config =>
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "ElasticSearch:Url", "http://localhost:9200" }
                        }));

                var result = builder.UseEafSerilog();

                result.ShouldBeSameAs(builder);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }

        [Fact]
        public void Dado_HostBuilderComConfigSeq_Quando_UsarEafSerilog_Entao_DeveRetornarMesmoBuilder()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            var originalDirectory = Directory.GetCurrentDirectory();

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config =>
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "Seq:Url", "http://localhost:5341" }
                        }));

                var result = builder.UseEafSerilog();

                result.ShouldBeSameAs(builder);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                try { Directory.Delete(tempDirectory, true); } catch { }
            }
        }
    }
}
