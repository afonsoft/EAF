using Abp.BackgroundJobs;
using Abp.Threading.BackgroundWorkers;
using System.Threading.Tasks;
using System;
using HangfireBackgroundJob = Hangfire.BackgroundJob;
using System.Threading;

namespace Eaf.Hangfire
{
    /// <summary>
    /// Representa a classe HangfireBackgroundJobManager.
    /// </summary>
    public class HangfireBackgroundJobManager : BackgroundWorkerBase, IBackgroundJobManager
    {
        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="jobId">Parâmetro jobId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual bool Delete(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            bool successfulDeletion = HangfireBackgroundJob.Delete(jobId);
            return successfulDeletion;
        }

        /// <summary>
        /// DeleteAsync.
        /// </summary>
        /// <param name="jobId">Parâmetro jobId.</param>
        /// <returns>Resultado da operação.</returns>
        public virtual Task<bool> DeleteAsync(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentNullException(nameof(jobId));
            }

            bool successfulDeletion = HangfireBackgroundJob.Delete(jobId);
            return Task.FromResult(successfulDeletion);
        }

        public virtual string Enqueue<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
            TimeSpan? delay = null) where TJob : IBackgroundJobBase<TArgs>
        {
            string jobUniqueIdentifier = string.Empty;

            if (!delay.HasValue)
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args));
                }
                else if (typeof(Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => ((Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args));
                }
                else if (typeof(Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => ((Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args, null, CancellationToken.None));
                }
            }
            else
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args), delay.Value);
                }
                else if (typeof(Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => ((Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args), delay.Value);
                }
                else if (typeof(Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => ((Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args, null, CancellationToken.None), delay.Value);
                }
            }

            return jobUniqueIdentifier;
        }

        public virtual Task<string> EnqueueAsync<TJob, TArgs>(TArgs args, BackgroundJobPriority priority = BackgroundJobPriority.Normal,
                                    TimeSpan? delay = null) where TJob : IBackgroundJobBase<TArgs>
        {
            string jobUniqueIdentifier = string.Empty;

            if (!delay.HasValue)
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args));
                }
                else if (typeof(Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => ((Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args));
                }
                else if (typeof(Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Enqueue<TJob>(job => ((Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args, null, CancellationToken.None));
                }
            }
            else
            {
                if (typeof(IBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => ((IBackgroundJob<TArgs>)job).Execute(args), delay.Value);
                }
                else if (typeof(Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => ((Abp.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args), delay.Value);
                }
                else if (typeof(Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>).IsAssignableFrom(typeof(TJob)))
                {
                    jobUniqueIdentifier = HangfireBackgroundJob.Schedule<TJob>(job => ((Eaf.BackgroundJobs.IAsyncBackgroundJob<TArgs>)job).ExecuteAsync(args, null, CancellationToken.None), delay.Value);
                }
            }

            return Task.FromResult(jobUniqueIdentifier);
        }
    }
}
