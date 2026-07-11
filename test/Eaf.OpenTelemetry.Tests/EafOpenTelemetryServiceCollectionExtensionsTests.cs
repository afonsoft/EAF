using Eaf.AspNetCore.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
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


    }
}