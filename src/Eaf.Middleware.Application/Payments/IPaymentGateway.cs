using Eaf.Middleware.Payments.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Interface de gateway de pagamento para assinaturas.
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Cria uma solicitação de pagamento no gateway.
        /// </summary>
        Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input);

        /// <summary>
        /// Processa a confirmação/retorno de um pagamento.
        /// </summary>
        Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input);
    }
}
