using Hangfire;
using Hangfire.Server;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments.hangfire
{
    /// <summary>
    /// Worker recorrente para renovação de assinaturas.
    /// </summary>
    public interface ISubscriptionRenewalWorker
    {
        /// <summary>
        /// Executa a renovação das assinaturas recorrentes.
        /// </summary>
        [DisableConcurrentExecution(900)]
        [AutomaticRetry(Attempts = 0)]
        [JobDisplayName("Subscription Renewal")]
        Task DoWork(PerformContext context);
    }
}
