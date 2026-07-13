using Castle.Facilities.Logging;
using Eaf.AspNetCore.Configuration;
using Eaf.Castle.Logging.SerilogIntegration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Serilog;
using Eaf.Middleware.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Eaf.ProjectName.WorkerService
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt-BR");
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}][{ThreadId}] {Message:lj} {Exception} {NewLine}")
                .CreateLogger();

                var app = CreateHostBuilder(args).Build();
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal Error in Main : {0}", ex.Message);
                Environment.Exit(1);
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
            .UseEafSerilog()
            .UseEafConfiguration("ProjectName_")
            .ConfigureServices((hostContext, services) =>
            {
                services.AddEafOpenTelemetry(options =>
                {
                    options.ConsoleExporter = true;
                    options.ServiceName = "ProjectName";
                    options.SourceName = new[]
                    {
                        "Eaf.ProjectName.Core",
                        "Eaf.ProjectName.EntityFrameworkCore",
                        "Eaf.ProjectName.WorkerService",
                    };
                });

                services.Configure<HostOptions>(hostOptions =>
                {
                    hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
                    hostOptions.ShutdownTimeout = TimeSpan.FromSeconds(800);
                });

                //Bootstrap Eaf Starting
                services.AddEaf<WorkerModule>(options =>
                    options.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseEafSerilog()));

                //Add Many Worker Services
                services.AddHostedService<Worker>();
            })
            //For Windows Services
            .UseWindowsService(x => x.ServiceName = "Eaf.ProjectName Service");
        }
    }
}