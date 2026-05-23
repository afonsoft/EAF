using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using System;
using System.Collections.Generic;

namespace Eaf.AspNetCore.Configuration
{
    /// <summary>
    /// Representa a classe EafOpenTelemetryOptions.
    /// </summary>
    public class EafOpenTelemetryOptions : IOptions<EafOpenTelemetryOptions>
    {
        public EafOpenTelemetryOptions Value => this;
        private readonly Dictionary<string, string> otlpVariables;

        /// <summary>
        /// EafOpenTelemetryOptions.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public EafOpenTelemetryOptions()
        {
            otlpVariables = new Dictionary<string, string>
            {
                { "OTEL_EXPORTER_OTLP_ENDPOINT", System.Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") },
                { "OTEL_EXPORTER_OTLP_PROTOCOL", System.Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL") ?? "http/protobuf" },
                { "OTEL_EXPORTER_OTLP_HEADERS", System.Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS") },
                { "OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT", System.Environment.GetEnvironmentVariable("OTEL_ATTRIBUTE_VALUE_LENGTH_LIMIT") ?? "4095" },
                { "OTEL_EXPOTEL_ATTRIBUTE_COUNT_LIMITORTER_OTLP_ENDPOINT", System.Environment.GetEnvironmentVariable("OTEL_ATTRIBUTE_COUNT_LIMIT") ?? "64" },
                { "OTEL_EXPORTER_OTLP_COMPRESSION", System.Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_COMPRESSION") ?? "gzip" },
                { "OTEL_EXPERIMENTAL_EXPORTER_OTLP_RETRY_ENABLED", System.Environment.GetEnvironmentVariable("OTEL_EXPERIMENTAL_EXPORTER_OTLP_RETRY_ENABLED") ?? "true" },
                { "OTEL_EXPORTER_OTLP_INSECURE", System.Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_INSECURE") ?? "false" },
                { "OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE", System.Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_TEMPORALITY_PREFERENCE") ?? "delta" },
                { "OTEL_EXPORTER_OTLP_METRICS_DEFAULT_HISTOGRAM_AGGREGATION", System.Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_DEFAULT_HISTOGRAM_AGGREGATION") ?? "base2_exponential_bucket_histogram" },
                { "OTEL_SERVICE_NAME", System.Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME") }
            };
        }

        private string _serviceName = "Eaf";

        /// <summary>
        /// Name of Application or Service
        /// </summary>
        public string ServiceName
        {
            get
            {
                if (string.IsNullOrEmpty(_serviceName))
                    _serviceName = otlpVariables["OTEL_SERVICE_NAME"];
                return _serviceName;
            }
            set
            {
                _serviceName = value;
                otlpVariables["OTEL_SERVICE_NAME"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the exception will be recorded as ActivityEvent or not. Default value: True.
        /// </summary>
        public bool RecordException { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether or not the EntityFrameworkInstrumentation should add the names of CommandType.StoredProcedure commands as the SemanticConventions.AttributeDbStatement tag. Default value: True.
        /// </summary>
        public bool SetDbStatementForStoredProcedure { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether or not the EntityFrameworkInstrumentation should add the text of CommandType.Text commands as the SemanticConventions.AttributeDbStatement tag. Default value: True.
        /// </summary>
        public bool SetDbStatementForText { get; set; } = true;

        /// <summary>
        /// Adds given activity source names to the list of subscribed sources.
        /// </summary>
        public string[] SourceName { get; set; }

        /// <summary>
        /// Adds given Meter names to the list of subscribed meters.
        /// </summary>
        public string[] MeterName { get; set; }

        private string _otlpEndpoint = null;

        /// <summary>
        /// Gets or sets the target to which the exporter is going to send telemetry. Must
        /// be a valid Uri with scheme (http or https) and host, and may contain a port and
        /// path.
        /// <see href="https://opentelemetry.io/docs/specs/otel/protocol/exporter/"/>
        /// </summary>
        public string OtlpEndpoint
        {
            get
            {
                if (string.IsNullOrEmpty(_otlpEndpoint))
                    _otlpEndpoint = otlpVariables["OTEL_EXPORTER_OTLP_ENDPOINT"];
                return _otlpEndpoint;
            }
            set
            {
                _otlpEndpoint = value;
                otlpVariables["OTEL_EXPORTER_OTLP_ENDPOINT"] = value;
            }
        }

        private string _otlpHeaders = null;
        public string OtlpHeaders
        {
            get
            {
                if (string.IsNullOrEmpty(_otlpHeaders))
                    _otlpHeaders = otlpVariables["OTEL_EXPORTER_OTLP_HEADERS"];
                return _otlpHeaders;
            }
            set
            {
                _otlpHeaders = value;
                otlpVariables["OTEL_EXPORTER_OTLP_HEADERS"] = value;
            }
        }

        private ExportProcessorType _otlpExportProcessorType = ExportProcessorType.Batch;
        public ExportProcessorType OtlpExportProcessorType
        {
            get
            {
                return _otlpExportProcessorType;
            }
            set
            {
                _otlpExportProcessorType = value;
            }
        }

        private OtlpExportProtocol _otlpProtocol = OtlpExportProtocol.HttpProtobuf;
        public OtlpExportProtocol OtlpProtocol
        {
            get
            {
                if (otlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] == "http/protobuf")
                    _otlpProtocol = OtlpExportProtocol.HttpProtobuf;
                if (otlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] == "grpc")
                    _otlpProtocol = OtlpExportProtocol.Grpc;
                return _otlpProtocol;
            }
            set
            {
                _otlpProtocol = value;
                if (value == OtlpExportProtocol.HttpProtobuf)
                    otlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/protobuf";
                else if (value == OtlpExportProtocol.Grpc)
                    otlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "grpc";
                else
                    otlpVariables["OTEL_EXPORTER_OTLP_PROTOCOL"] = "http/json";
            }
        }



        /// <summary>
        /// Adds Console exporter to the TracerProvider
        /// </summary>
        public bool ConsoleExporter { get; set; } = false;

        /// <summary>
        /// Variable of Otlp Exporter
        ///  <see href="https://opentelemetry.io/docs/specs/otel/protocol/exporter/"/>
        /// </summary>
        public Dictionary<string, string> OtlpVariables
        { get { return otlpVariables; } }
    }
}
