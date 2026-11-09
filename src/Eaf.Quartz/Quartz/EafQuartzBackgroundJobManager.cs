using Abp.BackgroundJobs;
using Abp.Dependency;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eaf.Quartz
{
    /// <summary>
    /// Implementação de <see cref="IBackgroundJobManager"/> baseada em Quartz.NET.
    /// Alternativa opt-in ao Hangfire: cada <c>Enqueue</c> cria um job Quartz one-shot;
    /// <see cref="ScheduleAsync{TJob,TArgs}"/> cria um job recorrente via expressão cron.
    /// </summary>
    public class EafQuartzBackgroundJobManager : IBackgroundJobManager, ISingletonDependency
    {
        private readonly IScheduler _scheduler;

        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafQuartzBackgroundJobManager"/>.
        /// </summary>
        public EafQuartzBackgroundJobManager(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        /// <inheritdoc />
        public async Task<string> EnqueueAsync<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJobBase<TArgs>
        {
            var jobId = Guid.NewGuid().ToString("N");
            var jobKey = new JobKey(jobId, typeof(TJob).Name);

            var job = JobBuilder.Create<EafQuartzJobAdapter<TJob, TArgs>>()
                .WithIdentity(jobKey)
                .UsingJobData(EafQuartzJobKeys.Arguments, JsonSerializer.Serialize(args))
                .StoreDurably(false)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(jobId + "-trigger", typeof(TJob).Name)
                .StartAt(delay.HasValue ? DateTimeOffset.UtcNow.Add(delay.Value) : DateTimeOffset.UtcNow)
                .Build();

            await _scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
            return jobId;
        }

        /// <inheritdoc />
        public string Enqueue<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal, TimeSpan? delay = null)
            where TJob : IBackgroundJobBase<TArgs>
        {
            return EnqueueAsync<TJob, TArgs>(args, priority, delay).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Agenda um job recorrente via expressão cron.
        /// </summary>
        /// <typeparam name="TJob">Tipo do background job ABP.</typeparam>
        /// <typeparam name="TArgs">Tipo dos argumentos.</typeparam>
        /// <param name="args">Argumentos serializados no <see cref="JobDataMap"/>.</param>
        /// <param name="cronExpression">Expressão cron no formato Quartz (ex.: <c>0/30 * * * * ?</c>).</param>
        /// <returns>Identificador do job agendado.</returns>
        public async Task<string> ScheduleAsync<TJob, TArgs>(TArgs args, string cronExpression)
            where TJob : IBackgroundJobBase<TArgs>
        {
            if (!CronExpression.IsValidExpression(cronExpression))
            {
                throw new Abp.UI.UserFriendlyException($"Expressão cron inválida: '{cronExpression}'.");
            }

            var jobId = Guid.NewGuid().ToString("N");
            var jobKey = new JobKey(jobId, typeof(TJob).Name);

            var job = JobBuilder.Create<EafQuartzJobAdapter<TJob, TArgs>>()
                .WithIdentity(jobKey)
                .UsingJobData(EafQuartzJobKeys.Arguments, JsonSerializer.Serialize(args))
                .StoreDurably(true)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(jobId + "-cron", typeof(TJob).Name)
                .WithCronSchedule(cronExpression)
                .Build();

            await _scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
            return jobId;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAsync(string jobId)
        {
            // jobId is globally unique; the group holds the job type name, so we
            // locate the JobKey via an AnyGroup matcher before deleting.
            var keys = await _scheduler
                .GetJobKeys(GroupMatcher<JobKey>.AnyGroup())
                .ConfigureAwait(false);

            foreach (var key in keys)
            {
                if (key.Name == jobId)
                {
                    return await _scheduler.DeleteJob(key).ConfigureAwait(false);
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool Delete(string jobId)
        {
            return DeleteAsync(jobId).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public void Start()
        {
            if (!_scheduler.IsStarted)
            {
                _scheduler.Start().GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            if (_scheduler.IsStarted)
            {
                _scheduler.Shutdown(waitForJobsToComplete: false).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public void WaitToStop()
        {
            _scheduler.Shutdown(waitForJobsToComplete: true).GetAwaiter().GetResult();
        }
    }
}
