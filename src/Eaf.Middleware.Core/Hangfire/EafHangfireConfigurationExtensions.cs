using Abp.Auditing;
using Abp.BackgroundJobs;
using Abp.EntityHistory;
using Eaf.Auditing.hangfire;
using Hangfire;
using System;
using Abp.Configuration.Startup;

namespace Eaf.Hangfire
{
    /// <summary>
    /// Representa a classe EafHangfireConfigurationExtensions.
    /// </summary>
    public static class EafHangfireConfigurationExtensions
    {
        /// <summary>
        /// Configures to use Hangfire for background job management.
        /// </summary>
        public static void UseHangfire(this IBackgroundJobConfiguration backgroundJobConfiguration)
        {
            backgroundJobConfiguration.AbpConfiguration.ReplaceService<IBackgroundJobManager, HangfireBackgroundJobManager>();
            backgroundJobConfiguration.IsJobExecutionEnabled = true;
        }

        /// <summary>
        /// SetExpiredHistoryEntityWoker.
        /// </summary>
        /// <param name="entityHistoryConfiguration">Parâmetro entityHistoryConfiguration.</param>
        public static void SetExpiredHistoryEntityWoker(this IEntityHistoryConfiguration entityHistoryConfiguration)
        {
            if (entityHistoryConfiguration.IsEnabled)
            {
                RecurringJob.AddOrUpdate<IExpiredEntityLogDeleterWorker>("ExpiredEntityLogDeleterWorker", x => x.DoWork(null), Cron.Hourly, TimeZoneInfo.Local);
            }
        }

        /// <summary>
        /// SetExpiredAuditWoker.
        /// </summary>
        /// <param name="entityHistoryConfiguration">Parâmetro entityHistoryConfiguration.</param>
        public static void SetExpiredAuditWoker(this IAuditingConfiguration entityHistoryConfiguration)
        {
            if (entityHistoryConfiguration.IsEnabled)
            {
                RecurringJob.AddOrUpdate<IExpiredAuditLogDeleterWorker>("ExpiredAuditLogDeleterWorker", x => x.DoWork(null), Cron.Hourly, TimeZoneInfo.Local);
            }
        }
    }
}