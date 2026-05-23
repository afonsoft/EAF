using Hangfire.Server;
using Hangfire;

namespace Eaf.Auditing.hangfire
{
    /// <summary>
    /// Representa a interface IExpiredEntityLogDeleterWorker.
    /// </summary>
    public interface IExpiredEntityLogDeleterWorker
    {
        [DisableConcurrentExecution(900)]
        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("Expired Entity Log Deleter")]
        void DoWork(PerformContext context);
    }
}
