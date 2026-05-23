using Hangfire.Server;
using Hangfire;

namespace Eaf.Auditing.hangfire
{
    /// <summary>
    /// Representa a interface IExpiredAuditLogDeleterWorker.
    /// </summary>
    public interface IExpiredAuditLogDeleterWorker
    {
        [DisableConcurrentExecution(900)]
        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("Expired Audit Log Deleter")]
        void DoWork(PerformContext context);
    }
}
