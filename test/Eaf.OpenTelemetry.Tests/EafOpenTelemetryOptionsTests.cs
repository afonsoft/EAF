using System;
using System.Collections.Generic;
using Eaf.AspNetCore.Configuration;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using Shouldly;
using Xunit;

namespace Eaf.OpenTelemetry.Tests
{
    public class EafOpenTelemetryOptionsTests
    {
        [Fact]
        public void Constructor_ShouldInitializeDefaultValues()
        {
            // Act
            var options = new EafOpenTelemetryOptions();

            // Assert
            options.ServiceName.ShouldBe("Eaf");
            options.RecordException.ShouldBeTrue();
            options.SetDbStatementForStoredProcedure.ShouldBeTrue();
            options.SetDbStatementForText.ShouldBeTrue();
            options.ConsoleExporter.ShouldBeFalse();
            options.OtlpExportProcessorType.ShouldBe(ExportProcessorType.Batch);
            options.OtlpProtocol.ShouldBe(OtlpExportProtocol.HttpProtobuf);
            options.OtlpVariables.ShouldNotBeNull();
            options.Value.ShouldBe(options);
        }

        [Fact]
        public void ServiceName_WhenSet_ShouldUpdateOtlpVariables()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();
            var serviceName = "TestService";

            // Act
            options.ServiceName = serviceName;

            // Assert
            options.ServiceName.ShouldBe(serviceName);
            options.OtlpVariables["OTEL_SERVICE_NAME"].ShouldBe(serviceName);
        }

        [Fact]
        public void OtlpEndpoint_WhenSet_ShouldUpdateOtlpVariables()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();
            var endpoint = "http://localhost:4317";

            // Act
            options.OtlpEndpoint = endpoint;

            // Assert
            options.OtlpEndpoint.ShouldBe(endpoint);
            options.OtlpVariables["OTEL_EXPORTER_OTLP_ENDPOINT"].ShouldBe(endpoint);
        }

        [Fact]
        public void OtlpHeaders_WhenSet_ShouldUpdateOtlpVariables()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();
            var headers = "api-key=test123";

            // Act
            options.OtlpHeaders = headers;

            // Assert
            options.OtlpHeaders.ShouldBe(headers);
            options.OtlpVariables["OTEL_EXPORTER_OTLP_HEADERS"].ShouldBe(headers);
        }

        [Theory]
        [InlineData(OtlpExportProtocol.HttpProtobuf, "http/protobuf")]
        [InlineData(OtlpExportProtocol.Grpc, "grpc")]

        public void OtlpProtocol_WhenSet_ShouldUpdateOtlpVariables(OtlpExportProtocol protocol, string expectedValue)
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.OtlpProtocol = protocol;

            // Assert
            options.OtlpProtocol.ShouldBe(protocol);
            options.OtlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"].ShouldBe(expectedValue);
        }

        [Fact]
        public void OtlpExportProcessorType_CanBeSet()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.OtlpExportProcessorType = ExportProcessorType.Simple;

            // Assert
            options.OtlpExportProcessorType.ShouldBe(ExportProcessorType.Simple);
        }

        [Fact]
        public void SourceName_CanBeSet()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();
            var sourceNames = new[] { "Source1", "Source2" };

            // Act
            options.SourceName = sourceNames;

            // Assert
            options.SourceName.ShouldBe(sourceNames);
        }

        [Fact]
        public void MeterName_CanBeSet()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();
            var meterNames = new[] { "Meter1", "Meter2" };

            // Act
            options.MeterName = meterNames;

            // Assert
            options.MeterName.ShouldBe(meterNames);
        }

        [Fact]
        public void RecordException_CanBeSet()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.RecordException = false;

            // Assert
            options.RecordException.ShouldBeFalse();
        }

        [Fact]
        public void SetDbStatementForStoredProcedure_CanBeSet()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.SetDbStatementForStoredProcedure = false;

            // Assert
            options.SetDbStatementForStoredProcedure.ShouldBeFalse();
        }

        [Fact]
        public void SetDbStatementForText_CanBeSet()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.SetDbStatementForText = false;

            // Assert
            options.SetDbStatementForText.ShouldBeFalse();
        }

        [Fact]
        public void ConsoleExporter_CanBeSet()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.ConsoleExporter = true;

            // Assert
            options.ConsoleExporter.ShouldBeTrue();
        }

        [Fact]
        public void OtlpVariables_ShouldContainDefaultValues()
        {
            // Arrange & Act
            var options = new EafOpenTelemetryOptions();

            // Assert
            options.OtlpVariables.ShouldContainKey("OTEL_EXPORTER_OTLP_PROTOCOL");
            options.OtlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"].ShouldBe("http/protobuf");

            options.OtlpVariables.ShouldContainKey("OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT");
            options.OtlpVariables["OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT"].ShouldBe("4095");

            options.OtlpVariables.ShouldContainKey("OTEL_EXPORTER_OTLP_COMPRESSION");
            options.OtlpVariables["OTEL_EXPORTER_OTLP_COMPRESSION"].ShouldBe("gzip");

            options.OtlpVariables.ShouldContainKey("OTEL_EXPERIMENTAL_EXPORTER_OTLP_RETRY_ENABLED");
            options.OtlpVariables["OTEL_EXPERIMENTAL_EXPORTER_OTLP_RETRY_ENABLED"].ShouldBe("true");

            options.OtlpVariables.ShouldContainKey("OTEL_EXPORTER_OTLP_INSECURE");
            options.OtlpVariables["OTEL_EXPORTER_OTLP_INSECURE"].ShouldBe("false");
        }

        [Fact]
        public void OtlpProtocol_WithUnknownProtocol_ShouldSetHttpJsonValue()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act - Setting an unknown protocol should default to http/json
            options.OtlpProtocol = unchecked((OtlpExportProtocol)999); // Unknown protocol

            // Assert
            options.OtlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"].ShouldBe("http/json");
        }

        [Fact]
        public void ServiceName_WhenEmpty_ShouldUseEnvironmentVariable()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.ServiceName = "";
            var serviceName = options.ServiceName;

            // Assert
            // Should fallback to environment variable or null
            serviceName.ShouldBe(options.OtlpVariables["OTEL_SERVICE_NAME"]);
        }

        [Fact]
        public void OtlpEndpoint_WhenEmpty_ShouldUseEnvironmentVariable()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.OtlpEndpoint = "";
            var endpoint = options.OtlpEndpoint;

            // Assert
            // Should fallback to environment variable or null
            endpoint.ShouldBe(options.OtlpVariables["OTEL_EXPORTER_OTLP_ENDPOINT"]);
        }

        [Fact]
        public void OtlpHeaders_WhenEmpty_ShouldUseEnvironmentVariable()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.OtlpHeaders = "";
            var headers = options.OtlpHeaders;

            // Assert
            // Should fallback to environment variable or null
            headers.ShouldBe(options.OtlpVariables["OTEL_EXPORTER_OTLP_HEADERS"]);
        }

        [Fact]
        public void OtlpVariables_ShouldContainAllExpectedKeys()
        {
            // Arrange & Act
            var options = new EafOpenTelemetryOptions();

            // Assert
            var expectedKeys = new[]
            {
                "OTEL_EXPORTER_OTLP_ENDPOINT",
                "OTEL_EXPORTER_OTLP_PROTOCOL",
                "OTEL_EXPORTER_OTLP_HEADERS",
                "OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT",
                "OTEL_EXPOTEL_ATTRIBUTE_COUNT_LIMITORTER_OTLP_ENDPOINT",
                "OTEL_EXPORTER_OTLP_COMPRESSION",
                "OTEL_EXPERIMENTAL_EXPORTER_OTLP_RETRY_ENABLED",
                "OTEL_EXPORTER_OTLP_INSECURE",
                "OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE",
                "OTEL_EXPORTER_OTLP_METRICS_DEFAULT_HISTOGRAM_AGGREGATION",
                "OTEL_SERVICE_NAME"
            };

            foreach (var key in expectedKeys)
            {
                options.OtlpVariables.ShouldContainKey(key);
            }
        }

        [Fact]
        public void OtlpVariables_ShouldHaveCorrectDefaultValues()
        {
            // Arrange & Act
            var options = new EafOpenTelemetryOptions();

            // Assert
            options.OtlpVariables["OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE"].ShouldBe("delta");
            options.OtlpVariables["OTEL_EXPORTER_OTLP_METRICS_DEFAULT_HISTOGRAM_AGGREGATION"].ShouldBe("base2_exponential_bucket_histogram");
            options.OtlpVariables["OTEL_EXPOTEL_ATTRIBUTE_COUNT_LIMITORTER_OTLP_ENDPOINT"].ShouldBe("64");
        }

        [Theory]
        [InlineData("TestApp")]
        [InlineData("MyService")]
        [InlineData("")]
        [InlineData(null)]
        public void ServiceName_WithDifferentValues_ShouldHandleCorrectly(string? serviceName)
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.ServiceName = serviceName;

            // Assert
            if (string.IsNullOrEmpty(serviceName))
            {
                options.ServiceName.ShouldBe(options.OtlpVariables["OTEL_SERVICE_NAME"]);
            }
            else
            {
                options.ServiceName.ShouldBe(serviceName);
                options.OtlpVariables["OTEL_SERVICE_NAME"].ShouldBe(serviceName);
            }
        }

        [Theory]
        [InlineData("http://localhost:4317")]
        [InlineData("https://api.honeycomb.io:443")]
        [InlineData("")]
        [InlineData(null)]
        public void OtlpEndpoint_WithDifferentValues_ShouldHandleCorrectly(string? endpoint)
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.OtlpEndpoint = endpoint;

            // Assert
            if (string.IsNullOrEmpty(endpoint))
            {
                options.OtlpEndpoint.ShouldBe(options.OtlpVariables["OTEL_EXPORTER_OTLP_ENDPOINT"]);
            }
            else
            {
                options.OtlpEndpoint.ShouldBe(endpoint);
                options.OtlpVariables["OTEL_EXPORTER_OTLP_ENDPOINT"].ShouldBe(endpoint);
            }
        }

        [Theory]
        [InlineData("api-key=test123")]
        [InlineData("x-honeycomb-team=abc123,x-honeycomb-dataset=my-dataset")]
        [InlineData("")]
        [InlineData(null)]
        public void OtlpHeaders_WithDifferentValues_ShouldHandleCorrectly(string? headers)
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.OtlpHeaders = headers;

            // Assert
            if (string.IsNullOrEmpty(headers))
            {
                options.OtlpHeaders.ShouldBe(options.OtlpVariables["OTEL_EXPORTER_OTLP_HEADERS"]);
            }
            else
            {
                options.OtlpHeaders.ShouldBe(headers);
                options.OtlpVariables["OTEL_EXPORTER_OTLP_HEADERS"].ShouldBe(headers);
            }
        }

        [Theory]
        [InlineData(ExportProcessorType.Batch)]
        [InlineData(ExportProcessorType.Simple)]
        public void OtlpExportProcessorType_WithDifferentValues_ShouldSetCorrectly(ExportProcessorType processorType)
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.OtlpExportProcessorType = processorType;

            // Assert
            options.OtlpExportProcessorType.ShouldBe(processorType);
        }

        [Fact]
        public void MultiplePropertyChanges_ShouldMaintainConsistency()
        {
            // Arrange
            var options = new EafOpenTelemetryOptions();

            // Act
            options.ServiceName = "TestService";
            options.OtlpEndpoint = "http://localhost:4317";
            options.OtlpHeaders = "api-key=test123";
            options.OtlpProtocol = OtlpExportProtocol.Grpc;
            options.ConsoleExporter = true;
            options.RecordException = false;

            // Assert
            options.ServiceName.ShouldBe("TestService");
            options.OtlpEndpoint.ShouldBe("http://localhost:4317");
            options.OtlpHeaders.ShouldBe("api-key=test123");
            options.OtlpProtocol.ShouldBe(OtlpExportProtocol.Grpc);
            options.ConsoleExporter.ShouldBeTrue();
            options.RecordException.ShouldBeFalse();

            options.OtlpVariables["OTEL_SERVICE_NAME"].ShouldBe("TestService");
            options.OtlpVariables["OTEL_EXPORTER_OTLP_ENDPOINT"].ShouldBe("http://localhost:4317");
            options.OtlpVariables["OTEL_EXPORTER_OTLP_HEADERS"].ShouldBe("api-key=test123");
            options.OtlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"].ShouldBe("grpc");
        }
    }
}