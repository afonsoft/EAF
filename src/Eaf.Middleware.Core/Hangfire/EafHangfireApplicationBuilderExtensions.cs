using Abp.Configuration.Startup;
using Abp.Hangfire;
using Abp.Logging;
using Eaf.Hangfire;
using Hangfire;
using Hangfire.Heartbeat.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Eaf.AspNetCore.Hangfire.Configuration
{
    /// <summary>
    /// Representa a classe EafHangfireApplicationBuilderExtensions.
    /// </summary>
    public static class EafHangfireApplicationBuilderExtensions
    {
        /// <summary>
        /// UseEafHangfire.
        /// </summary>
        /// <param name="app">Parâmetro app.</param>
        /// <param name="options">Parâmetro options.</param>
        public static void UseEafHangfire(this IApplicationBuilder app, Action<EafHangFireOptions> options = null)
        {
            var configuration = app.ApplicationServices.GetService<IAbpStartupConfiguration>();
            if (!configuration.BackgroundJobs.IsJobExecutionEnabled)
                return;

            EafHangFireOptions _options = new EafHangFireOptions();
            if (options != null)
                options.Invoke(_options);

            if (_options.IsEnabled)
            {
                var dashboardOptions = new DashboardOptions
                {
                    DisplayStorageConnectionString = false,
                    IgnoreAntiforgeryToken = true,
                    Authorization = new[] { new EafHangfireAuthorizationFilter(_options.RequiredPermissionName) },
                    DisplayNameFunc = (context, job) => EafDisplayNameExtensions.Format(context, job),
                    StatsPollingInterval = 5000
                };

                dashboardOptions.AppPath = _options.AppPath ?? dashboardOptions.AppPath;
                dashboardOptions.PrefixPath = _options.PrefixPath ?? dashboardOptions.PrefixPath;
                dashboardOptions.DashboardTitle = _options.DashboardTitle ?? dashboardOptions.DashboardTitle;

                app.UseHangfireDashboard(_options.PathMatch, dashboardOptions, JobStorage.Current);

                app.UseHangfireServer(new BackgroundJobServerOptions
                {
                    WorkerCount = _options.WorkerCount,
                    Queues = _options.Queues
                },
                additionalProcesses: new[] { new ProcessMonitor(checkInterval: TimeSpan.FromSeconds(5)) },
                storage: JobStorage.Current);

                LogHelper.Logger.DebugFormat("SwaggerUI {0}", _options.PathMatch);
            }
        }
    }
}