using Eaf.Middleware.Web.Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Eaf.Middleware.Tests.WebCore.Serilog
{
    public class SerilogEafHostBuilderExtensionsBddTests
    {
        [Fact]
        public void Dado_HostBuilder_Quando_UsarEafSerilogComNivel_Entao_DeveRetornarMesmoBuilder()
        {
            var tempDirectory = CriarTempDirectory();
            var originalDirectory = Directory.GetCurrentDirectory();
            var originalLog = Log.Logger;

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
                LimparTempDirectory(tempDirectory);
                Log.CloseAndFlush();
                Log.Logger = originalLog;
            }
        }

        [Fact]
        public void Dado_HostBuilderReal_Quando_UsarEafSerilogComNivelEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = CriarTempDirectory();
            var originalDirectory = Directory.GetCurrentDirectory();
            var originalLog = Log.Logger;

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config => config.AddInMemoryCollection(new Dictionary<string, string?>()))
                    .UseEafSerilog(LogEventLevel.Information);

                using var host = builder.Build();

                host.ShouldNotBeNull();
                Log.Logger.Information("Test");
                var logsDir = Path.Combine(tempDirectory, "Logs");
                Log.CloseAndFlush();
                Directory.Exists(logsDir).ShouldBeTrue();
                Directory.GetFiles(logsDir, "log*.txt").Length.ShouldBeGreaterThan(0);
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                LimparTempDirectory(tempDirectory);
                Log.CloseAndFlush();
                Log.Logger = originalLog;
            }
        }

        [Fact]
        public void Dado_HostBuilderComConfigElastic_Quando_UsarEafSerilog_Entao_DeveRetornarMesmoBuilder()
        {
            var tempDirectory = CriarTempDirectory();
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
                LimparTempDirectory(tempDirectory);
            }
        }

        [Fact]
        public void Dado_HostBuilderComConfigSeq_Quando_UsarEafSerilog_Entao_DeveRetornarMesmoBuilder()
        {
            var tempDirectory = CriarTempDirectory();
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
                LimparTempDirectory(tempDirectory);
            }
        }

        [Fact]
        public void Dado_HostBuilderComConfigElastic_Quando_UsarEafSerilogComNivelEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = CriarTempDirectory();
            var originalDirectory = Directory.GetCurrentDirectory();
            var originalLog = Log.Logger;

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config =>
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "ElasticSearch:Url", "http://localhost:9200" }
                        }))
                    .UseEafSerilog(LogEventLevel.Information);

                using var host = builder.Build();
                host.ShouldNotBeNull();
                Log.CloseAndFlush();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                LimparTempDirectory(tempDirectory);
                Log.CloseAndFlush();
                Log.Logger = originalLog;
            }
        }

        [Fact]
        public void Dado_HostBuilderComConfigSeq_Quando_UsarEafSerilogComNivelEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = CriarTempDirectory();
            var originalDirectory = Directory.GetCurrentDirectory();
            var originalLog = Log.Logger;

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config =>
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "Seq:Url", "http://localhost:5341" }
                        }))
                    .UseEafSerilog(LogEventLevel.Information);

                using var host = builder.Build();
                host.ShouldNotBeNull();
                Log.CloseAndFlush();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                LimparTempDirectory(tempDirectory);
                Log.CloseAndFlush();
                Log.Logger = originalLog;
            }
        }

        [Fact]
        public void Dado_HostBuilderComConfigSeqApiKey_Quando_UsarEafSerilogComNivelEBuild_Entao_DeveCriarHost()
        {
            var tempDirectory = CriarTempDirectory();
            var originalDirectory = Directory.GetCurrentDirectory();
            var originalLog = Log.Logger;

            try
            {
                Directory.SetCurrentDirectory(tempDirectory);
                var builder = new HostBuilder()
                    .ConfigureHostConfiguration(config =>
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            { "Seq:Url", "http://localhost:5341" },
                            { "Seq:ApiKey", "test-api-key" }
                        }))
                    .UseEafSerilog(LogEventLevel.Information);

                using var host = builder.Build();
                host.ShouldNotBeNull();
                Log.CloseAndFlush();
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
                LimparTempDirectory(tempDirectory);
                Log.CloseAndFlush();
                Log.Logger = originalLog;
            }
        }

        private static string CriarTempDirectory()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }

        private static void LimparTempDirectory(string tempDirectory)
        {
            try { Directory.Delete(tempDirectory, true); } catch { }
        }
    }
}
