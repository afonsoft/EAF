using Abp.BackgroundJobs;
using Hangfire.Server;
using System.Threading.Tasks;
using System.Threading;

namespace Eaf.BackgroundJobs
{
    /// <summary>
    /// Representa a interface IAsyncBackgroundJob.
    /// </summary>
    public interface IAsyncBackgroundJob<in TArgs> : IBackgroundJobBase<TArgs>
    {
        /// <summary>
        /// Executes the job with the <paramref name="args"/>.
        /// </summary>
        /// <param name="args">Job arguments.</param>
        /// <param name="context">PerformContext.</param>
        /// <param name="token">CancellationToken.</param>
        Task ExecuteAsync(TArgs args, PerformContext context, CancellationToken token);
    }
}
