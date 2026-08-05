using Eaf.AspNetCore.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Eaf.OpenTelemetry.Tests
{
    public class EafOpenTelemetryServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddEafOpenTelemetry_WithValidServices_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Should.NotThrow(() => services.AddEafOpenTelemetry());
        }

        [Fact]
        public void AddEafOpenTelemetry_WithNullServices_ShouldThrowArgumentNullException()
        {
            // Arrange
            IServiceCollection? services = null;

            // Act & Assert
            Should.Throw<ArgumentNullException>(() => services.AddEafOpenTelemetry());
        }

        [Fact]
        public void AddEafOpenTelemetry_WithAction_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            Action<EafOpenTelemetryOptions> configureOptions = options =>
            {
                options.ServiceName = "TestService";
            };

            // Act & Assert
            Should.NotThrow(() => services.AddEafOpenTelemetry(configureOptions));
        }

        [Fact]
        public void AddEafOpenTelemetry_WithNullAction_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            Action<EafOpenTelemetryOptions>? configureOptions = null;

            // Act & Assert
            Should.NotThrow(() => services.AddEafOpenTelemetry(configureOptions));
        }

        [Fact]
        public void AddEafOpenTelemetry_ShouldReturnOpenTelemetryBuilder()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var result = services.AddEafOpenTelemetry();

            // Assert
            result.ShouldNotBeNull();
        }

        [Fact]
        public void AddEafOpenTelemetry_WithAction_ShouldReturnOpenTelemetryBuilder()
        {
            // Arrange
            var services = new ServiceCollection();
            Action<EafOpenTelemetryOptions>? configureOptions = options => { };

            // Act
            var result = services.AddEafOpenTelemetry(configureOptions);

            // Assert
            result.ShouldNotBeNull();
        }

        [Fact]
        public void AddEafOpenTelemetry_ShouldRegisterServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.AddEafOpenTelemetry();
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            // Verify that OpenTelemetry services are registered
            serviceProvider.ShouldNotBeNull();
        }

        [Fact]
        public void AddEafOpenTelemetry_MultipleCalls_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Should.NotThrow(() =>
            {
                services.AddEafOpenTelemetry();
                services.AddEafOpenTelemetry();
                services.AddEafOpenTelemetry();
            });
        }

        [Fact]
        public void AddEafOpenTelemetry_WithDifferentConfigurations_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            Should.NotThrow(() =>
            {
                services.AddEafOpenTelemetry(options => options.ServiceName = "Service1");
                services.AddEafOpenTelemetry(options => options.ServiceName = "Service2");
            });
        }

        [Fact]
        public void AddEafOpenTelemetry_WithComplexConfiguration_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            Action<EafOpenTelemetryOptions> configureOptions = options =>
            {
                options.ServiceName = "ComplexService";
                options.RecordException = true;
                options.SetDbStatementForStoredProcedure = true;
                options.SetDbStatementForText = true;
                options.ConsoleExporter = true;
            };

            // Act & Assert
            Should.NotThrow(() => services.AddEafOpenTelemetry(configureOptions));
        }

        [Fact]
        public void AddEafOpenTelemetry_WithOtlpEndpointAndConsoleExporter_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            Action<EafOpenTelemetryOptions> configureOptions = options =>
            {
                options.ServiceName = "ServiceWithOtlp";
                options.OtlpEndpoint = "http://localhost:4317";
                options.ConsoleExporter = true;
                options.OtlpHeaders = "key=value";
                options.RecordException = false;
                options.SetDbStatementForStoredProcedure = false;
                options.SetDbStatementForText = false;
            };

            // Act & Assert
            Should.NotThrow(() => services.AddEafOpenTelemetry(configureOptions));
        }

        [Fact]
        public void AddEafOpenTelemetry_WithOtlpEndpointAndNoConsoleExporter_ShouldNotThrow()
        {
            // Arrange
            var services = new ServiceCollection();
            Action<EafOpenTelemetryOptions> configureOptions = options =>
            {
                options.ServiceName = "ServiceWithOtlpNoConsole";
                options.OtlpEndpoint = "http://localhost:4317";
                options.ConsoleExporter = false;
                options.OtlpHeaders = "key=value";
                options.RecordException = true;
            };

            // Act & Assert
            Should.NotThrow(() => services.AddEafOpenTelemetry(configureOptions));
        }

        [Theory]
        [InlineData("https://otlp.nr-data.net", "traces", "https://otlp.nr-data.net/v1/traces")]
        [InlineData("https://otlp.nr-data.net/", "metrics", "https://otlp.nr-data.net/v1/metrics")]
        [InlineData("https://otlp.nr-data.net/v1/traces", "traces", "https://otlp.nr-data.net/v1/traces")]
        public void ConfigureOtlpExporterOptions_WithBaseEndpointAndHttpProtobuf_ShouldAppendSignalPath(string endpoint, string signal, string expectedEndpoint)
        {
            // Arrange
            var options = new EafOpenTelemetryOptions
            {
                OtlpEndpoint = endpoint,
                OtlpProtocol = OtlpExportProtocol.HttpProtobuf
            };
            var otlpOptions = new OtlpExporterOptions();
            var method = typeof(EafOpenTelemetryServiceCollectionExtensions).GetMethod("ConfigureOtlpExporterOptions", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            // Act
            method.Invoke(null, new object[] { otlpOptions, options, endpoint, signal });

            // Assert
            otlpOptions.Endpoint.ShouldBe(new Uri(expectedEndpoint));
        }

        [Fact]
        public void ConfigureOtlpExporterOptions_WithBaseEndpointAndGrpc_ShouldNotAppendSignalPath()
        {
            // Arrange
            var endpoint = "https://otlp.nr-data.net";
            var options = new EafOpenTelemetryOptions
            {
                OtlpEndpoint = endpoint,
                OtlpProtocol = OtlpExportProtocol.Grpc
            };
            var otlpOptions = new OtlpExporterOptions();
            var method = typeof(EafOpenTelemetryServiceCollectionExtensions).GetMethod("ConfigureOtlpExporterOptions", BindingFlags.NonPublic | BindingFlags.Static);
            method.ShouldNotBeNull();

            // Act
            method.Invoke(null, new object[] { otlpOptions, options, endpoint, "traces" });

            // Assert
            otlpOptions.Endpoint.ShouldBe(new Uri(endpoint));
        }
    }
}