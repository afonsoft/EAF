using Abp.BackgroundJobs;
using Hangfire.Server;
using System.Threading.Tasks;
using System.Threading;

namespace Eaf.BackgroundJobs
{
    /// <summary>
    /// AsyncBackgroundJob with HangFire
    /// </summary>
    /// <typeparam name="TArgs">args</typeparam>
    public abstract class AsyncBackgroundJob<TArgs> : BackgroundJobBase<TArgs>, IAsyncBackgroundJob<TArgs>
    {
        /// <summary>
        /// ExecuteAsync
        /// </summary>
        /// <param name="args">TArgs</param>
        /// <param name="context">PerformContext HangFire</param>
        /// <param name="token">CancellationToken</param>
        /// <returns></returns>
        public abstract Task ExecuteAsync(TArgs args, PerformContext context, CancellationToken token);
    }
}
