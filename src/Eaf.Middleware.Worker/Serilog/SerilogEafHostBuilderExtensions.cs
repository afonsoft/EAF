using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Settings.Configuration;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Reflection;

namespace Eaf.Middleware.Serilog
{
    /// <summary>
    /// Representa a classe SerilogEafHostBuilderExtensions.
    /// </summary>
    public static class SerilogEafHostBuilderExtensions
    {
        private const string ElasticSearchUrlKey = "ElasticSearch:Url";
        private const string SeqUrlKey = "Seq:Url";
        private const string SeqApiKeyKey = "Seq:ApiKey";

        private static readonly ConfigurationReaderOptions options = new ConfigurationReaderOptions(typeof(ConsoleLoggerConfigurationExtensions).Assembly);

        /// <summary>
        /// UseEafSerilog.
        /// </summary>
        /// <param name="builder">Parâmetro builder.</param>
        /// <param name="level">Parâmetro level.</param>
        /// <returns>Resultado da operação.</returns>
        public static IHostBuilder UseEafSerilog(this IHostBuilder builder, LogEventLevel level)
        {
            string pathToLog = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");
            void configureLogger(HostBuilderContext ctx, LoggerConfiguration config)
            {
                config.MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", level)
               .MinimumLevel.Override("System", level)
               .MinimumLevel.Override("Hangfire", level)
               .MinimumLevel.Override("Eaf", level < LogEventLevel.Information ? level : LogEventLevel.Information)
               .MinimumLevel.Override("Microsoft.AspNetCore", level)
               .MinimumLevel.Override("Microsoft.EntityFrameworkCore", level)
               .ReadFrom.Configuration(ctx.Configuration, options)
               .Enrich.WithEnvironment("ASPNETCORE_ENVIRONMENT")
               .Enrich.FromLogContext()
               .Enrich.WithProcessName()
               .Enrich.WithMachineName()
               .Enrich.WithThreadId()
               .Enrich.WithProcessId()
               .Enrich.WithExceptionDetails()
               .WriteTo.File(pathToLog, restrictedToMinimumLevel: level, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 41943040, shared: true, outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}][{ThreadId}] {Message:lj} {Properties:j} {Exception} {NewLine}")
               .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}][{ThreadId}] {Message:lj} {Exception} {NewLine}");

                if (!string.IsNullOrEmpty(ctx.Configuration[ElasticSearchUrlKey]))
                {
                    config.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(ctx.Configuration[ElasticSearchUrlKey]))
                    {
                        AutoRegisterTemplate = true,
                        AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                        IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{ctx.HostingEnvironment?.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                    });
                }

                if (!string.IsNullOrEmpty(ctx.Configuration[SeqUrlKey]))
                {
                    if (!string.IsNullOrEmpty(ctx.Configuration[SeqApiKeyKey]))
                        config.WriteTo.Seq(ctx.Configuration[SeqUrlKey], apiKey: ctx.Configuration[SeqApiKeyKey], restrictedToMinimumLevel: level);
                    else
                        config.WriteTo.Seq(ctx.Configuration[SeqUrlKey], restrictedToMinimumLevel: level);
                }
            }

            return builder.UseSerilog(configureLogger);
        }

        /// <summary>
        /// UseEafSerilog.
        /// </summary>
        /// <param name="builder">Parâmetro builder.</param>
        /// <param name="configureLogger">Parâmetro configureLogger.</param>
        /// <returns>Resultado da operação.</returns>
        public static IHostBuilder UseEafSerilog(this IHostBuilder builder, Action<HostBuilderContext, LoggerConfiguration> configureLogger = null)
        {
            if (configureLogger == null)
            {
                string pathToLog = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "log.txt");
                configureLogger = (ctx, config) =>
                {
                    config.MinimumLevel.Debug()
                   .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                   .MinimumLevel.Override("System", LogEventLevel.Error)
                   .MinimumLevel.Override("Hangfire", LogEventLevel.Warning)
                   .MinimumLevel.Override("Eaf", LogEventLevel.Debug)
                   .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
                   .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                   .ReadFrom.Configuration(ctx.Configuration, options)
                   .Enrich.WithEnvironment("ASPNETCORE_ENVIRONMENT")
                   .Enrich.FromLogContext()
                   .Enrich.WithProcessName()
                   .Enrich.WithMachineName()
                   .Enrich.WithThreadId()
                   .Enrich.WithProcessId()
                   .Enrich.WithExceptionDetails()
                   .WriteTo.File(pathToLog, restrictedToMinimumLevel: LogEventLevel.Error, rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, fileSizeLimitBytes: 41943040, shared: true, outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}][{ThreadId}] {Message:lj} {Properties:j} {Exception} {NewLine}")
                   .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}][{ThreadId}] {Message:lj} {Exception} {NewLine}");

                    if (!string.IsNullOrEmpty(ctx.Configuration[ElasticSearchUrlKey]))
                    {
                        config.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(ctx.Configuration[ElasticSearchUrlKey]))
                        {
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                            IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{ctx.HostingEnvironment?.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}"
                        });
                    }

                    if (!string.IsNullOrEmpty(ctx.Configuration[SeqUrlKey]))
                    {
                        if (!string.IsNullOrEmpty(ctx.Configuration[SeqApiKeyKey]))
                            config.WriteTo.Seq(ctx.Configuration[SeqUrlKey], apiKey: ctx.Configuration[SeqApiKeyKey]);
                        else
                            config.WriteTo.Seq(ctx.Configuration[SeqUrlKey]);
                    }
                };
            }
            return builder.UseSerilog(configureLogger);
        }
    }
}