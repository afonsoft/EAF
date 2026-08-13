using Eaf.Middleware.Payments.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Gateway de pagamento com suporte a assinaturas recorrentes.
    /// </summary>
    public interface ISubscriptionPaymentGateway : IPaymentGateway
    {
        /// <summary>
        /// Cancela uma assinatura recorrente no gateway.
        /// </summary>
        Task<PaymentResultDto> CancelSubscriptionAsync(string gatewaySubscriptionId);

        /// <summary>
        /// Obtém o status de uma assinatura recorrente.
        /// </summary>
        Task<SubscriptionStatusResult> GetSubscriptionStatusAsync(string gatewaySubscriptionId);

        /// <summary>
        /// Processa um webhook do gateway.
        /// </summary>
        Task<PaymentResultDto> ProcessWebhookAsync(string eventName, string json, string signature);
    }

    /// <summary>
    /// Resultado do status de uma assinatura recorrente.
    /// </summary>
    public class SubscriptionStatusResult
    {
        /// <summary>
        /// Indica se a assinatura está ativa.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Data de término do período atual.
        /// </summary>
        public System.DateTime? CurrentPeriodEnd { get; set; }

        /// <summary>
        /// Status raw do gateway.
        /// </summary>
        public string Status { get; set; }
    }
}
