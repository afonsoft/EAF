using Abp.Application.Services.Dto;
using Eaf.Middleware.Payments.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Serviço de aplicação para pagamentos de assinatura.
    /// </summary>
    public interface IPaymentAppService
    {
        /// <summary>
        /// Obtém os pagamentos de assinatura paginados.
        /// </summary>
        Task<PagedResultDto<SubscriptionPaymentDto>> GetAllAsync(GetSubscriptionPaymentsInput input);

        /// <summary>
        /// Cria uma solicitação de pagamento para assinatura.
        /// </summary>
        Task<PaymentRequestDto> CreatePaymentAsync(CreateSubscriptionPaymentInput input);

        /// <summary>
        /// Processa o retorno de pagamento e ativa a assinatura.
        /// </summary>
        Task<SubscriptionPaymentDto> ProcessPaymentAsync(long paymentId, ProcessPaymentInput input);
    }
}
