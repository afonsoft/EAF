using Abp.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System;
using System.Linq;

namespace Eaf.AspNetCore.Configuration
{
    /// <summary>
    /// Enable for OpenTelemetry in IServiceCollection
    /// </summary>
    public static class EafOpenTelemetryServiceCollectionExtensions
    {
        /// <summary>
        /// Add MapPrometheusScrapingEndpoint
        /// </summary>
        /// <param name="endpoints">IEndpointRouteBuilder</param>
        /// <returns>IEndpointRouteBuilder</returns>
        public static IEndpointConventionBuilder MapEafOpenTelemetryMetrics(this IEndpointRouteBuilder endpoints)
        {
            LogHelper.Logger.DebugFormat("PrometheusEndpoint {0}", "/metrics");
            return endpoints.MapPrometheusScrapingEndpoint();
        }

        /// <summary>
        /// Add OpenTelemetry for IServiceCollection with AspNetCoreInstrumentation, EntityFrameworkCoreInstrumentation, HangfireInstrumentation, HttpClientInstrumentation
        /// </summary>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        public static OpenTelemetryBuilder AddEafOpenTelemetry(this IServiceCollection services, Action<EafOpenTelemetryOptions> optionsAction = null)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            var options = ConfigureOptions(services, optionsAction);
            SetOtlpEnvironmentVariables(options);
            ConfigureLoggingServices(services, options);

            return services.AddOpenTelemetry()
                .ConfigureResource(builder => builder
                    .AddEnvironmentVariableDetector()
                    .AddTelemetrySdk()
                    .AddService(serviceName: options.ServiceName))
                .WithTracing(builder => ConfigureTracing(builder, options))
                .WithLogging(builder => ConfigureLoggingProvider(builder, options))
                .WithMetrics(builder => ConfigureMetrics(builder, options));
        }

        private static EafOpenTelemetryOptions ConfigureOptions(IServiceCollection services, Action<EafOpenTelemetryOptions> optionsAction)
        {
            var options = new EafOpenTelemetryOptions();
            if (optionsAction != null)
            {
                optionsAction.Invoke(options);
                services.Configure<EafOpenTelemetryOptions>(optionsAction);
            }

            if (options.SourceName == null || !options.SourceName.Any())
                options.SourceName = new[] { "Eaf.OpenTelemetry", "Eaf" };

            if (options.MeterName == null || !options.MeterName.Any())
                options.MeterName = new[] { "Microsoft.AspNetCore.Hosting", "Eaf" };

            return options;
        }

        private static void SetOtlpEnvironmentVariables(EafOpenTelemetryOptions options)
        {
            foreach (var otlp in options.OtlpVariables)
            {
                try
                {
                    if (!string.IsNullOrEmpty(otlp.Value) && !string.IsNullOrEmpty(otlp.Key))
                    {
                        System.Environment.SetEnvironmentVariable(otlp.Key, otlp.Value);
                    }
                }
                catch
                {
                    //Ignore
                }
            }
        }

        private static void ConfigureLoggingServices(IServiceCollection services, EafOpenTelemetryOptions options)
        {
            services.AddLogging(configure =>
            {
                configure.AddOpenTelemetry(builder =>
                {
                    builder.IncludeFormattedMessage = true;
                    builder.ParseStateValues = true;
                    builder.IncludeScopes = true;

                    AddOtlpExporter(builder, options);
                    AddConsoleExporter(builder, options);
                });
            });
        }

        private static void ConfigureTracing(TracerProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            builder
                .AddSource("Eaf")
                .AddSource("Eaf.*")
                .AddSource(options.SourceName)
                .AddSource("Eaf.Middleware.Web.Core")
                .AddAspNetCoreInstrumentation(o =>
                {
                    o.RecordException = options.RecordException;
                })
                .AddEntityFrameworkCoreInstrumentation(o =>
                {
                    o.SetDbStatementForStoredProcedure = options.SetDbStatementForStoredProcedure;
                    o.SetDbStatementForText = options.SetDbStatementForText;
                })
                .AddHangfireInstrumentation(o =>
                {
                    o.RecordException = options.RecordException;
                })
                .AddHttpClientInstrumentation(o =>
                {
                    o.RecordException = options.RecordException;
                });

            AddOtlpExporter(builder, options);
            AddConsoleExporter(builder, options);
        }

        private static void ConfigureLoggingProvider(LoggerProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            AddOtlpExporter(builder, options);
            AddConsoleExporter(builder, options);
        }

        private static void ConfigureMetrics(MeterProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            builder
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddMeter("Eaf")
                .AddMeter("Eaf.*")
                .AddMeter("Eaf.Middleware.Web.Core")
                .AddMeter(options.MeterName)
                .AddPrometheusExporter();

            AddOtlpExporter(builder, options);
            AddConsoleExporter(builder, options);
        }

        private static void AddOtlpExporter(TracerProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            if (!string.IsNullOrEmpty(options.OtlpEndpoint))
                builder.AddOtlpExporter(otlpOptions => ConfigureOtlpExporterOptions(otlpOptions, options));
            else
                builder.AddOtlpExporter();
        }

        private static void AddOtlpExporter(OpenTelemetryLoggerOptions builder, EafOpenTelemetryOptions options)
        {
            if (!string.IsNullOrEmpty(options.OtlpEndpoint))
                builder.AddOtlpExporter(otlpOptions => ConfigureOtlpExporterOptions(otlpOptions, options));
            else
                builder.AddOtlpExporter();
        }

        private static void AddOtlpExporter(MeterProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            if (!string.IsNullOrEmpty(options.OtlpEndpoint))
                builder.AddOtlpExporter(otlpOptions => ConfigureOtlpExporterOptions(otlpOptions, options));
            else
                builder.AddOtlpExporter();
        }

        private static void AddOtlpExporter(LoggerProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            if (!string.IsNullOrEmpty(options.OtlpEndpoint))
                builder.AddOtlpExporter(otlpOptions => ConfigureOtlpExporterOptions(otlpOptions, options));
            else
                builder.AddOtlpExporter();
        }

        private static void AddConsoleExporter(TracerProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            if (options.ConsoleExporter)
                builder.AddConsoleExporter();
        }

        private static void AddConsoleExporter(LoggerProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            if (options.ConsoleExporter)
                builder.AddConsoleExporter();
        }

        private static void AddConsoleExporter(OpenTelemetryLoggerOptions builder, EafOpenTelemetryOptions options)
        {
            if (options.ConsoleExporter)
                builder.AddConsoleExporter();
        }

        private static void AddConsoleExporter(MeterProviderBuilder builder, EafOpenTelemetryOptions options)
        {
            if (options.ConsoleExporter)
                builder.AddConsoleExporter();
        }

        private static void ConfigureOtlpExporterOptions(OtlpExporterOptions otlpOptions, EafOpenTelemetryOptions options)
        {
            otlpOptions.Endpoint = new Uri(options.OtlpEndpoint);
            otlpOptions.Headers = options.OtlpHeaders;
            otlpOptions.Protocol = options.OtlpProtocol;
            otlpOptions.ExportProcessorType = options.OtlpExportProcessorType;
        }
    }
}