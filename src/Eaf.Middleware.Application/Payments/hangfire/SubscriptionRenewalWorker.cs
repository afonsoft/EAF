using Abp.Dependency;
using Hangfire.Console;
using Hangfire.Server;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments.hangfire
{
    /// <summary>
    /// Implementação do worker de renovação de assinaturas.
    /// </summary>
    public class SubscriptionRenewalWorker : ISubscriptionRenewalWorker, ITransientDependency
    {
        private readonly IPaymentManager _paymentManager;

        /// <summary>
        /// SubscriptionRenewalWorker.
        /// </summary>
        /// <param name="paymentManager">Gerenciador de pagamentos.</param>
        public SubscriptionRenewalWorker(IPaymentManager paymentManager)
        {
            _paymentManager = paymentManager;
        }

        /// <summary>
        /// Executa a renovação das assinaturas recorrentes.
        /// </summary>
        /// <param name="context">Contexto de execução do Hangfire.</param>
        public async Task DoWork(PerformContext context)
        {
            await _paymentManager.RenewActiveSubscriptionsAsync();
            context.WriteLine("Subscription renewal completed.");
        }
    }
}
