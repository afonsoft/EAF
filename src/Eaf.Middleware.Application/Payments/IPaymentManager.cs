using Eaf.Middleware.Payments.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Gerenciador de ciclo de vida de pagamentos de assinatura.
    /// </summary>
    public interface IPaymentManager
    {
        /// <summary>
        /// Cria um pagamento de assinatura pendente.
        /// </summary>
        Task<PaymentRequestDto> CreatePaymentAsync(CreateSubscriptionPaymentInput input);

        /// <summary>
        /// Processa o retorno de um pagamento e ativa a assinatura.
        /// </summary>
        Task<SubscriptionPaymentDto> ProcessPaymentAsync(long paymentId, ProcessPaymentInput input);

        /// <summary>
        /// Realiza upgrade/downgrade de edição com cálculo de prorração.
        /// </summary>
        Task<PaymentRequestDto> UpgradeSubscriptionAsync(UpgradeSubscriptionInput input);

        /// <summary>
        /// Cancela uma assinatura recorrente.
        /// </summary>
        Task<SubscriptionPaymentDto> CancelRecurringAsync(long paymentId);

        /// <summary>
        /// Renova/estende assinaturas recorrentes ativas consultando o gateway.
        /// </summary>
        Task RenewActiveSubscriptionsAsync();

        /// <summary>
        /// Processa um webhook do gateway de pagamento.
        /// </summary>
        Task<SubscriptionPaymentDto> ProcessWebhookAsync(string gateway, string json, string signature);
    }
}
