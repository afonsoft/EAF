using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Eaf.Middleware.Payments.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Serviço de aplicação para pagamentos de assinatura.
    /// </summary>
    public interface IPaymentAppService : IApplicationService
    {
        /// <summary>
        /// Obtém os pagamentos de assinatura paginados.
        /// </summary>
        Task<PagedResultDto<SubscriptionPaymentDto>> GetAllAsync(GetSubscriptionPaymentsInput input);

        /// <summary>
        /// Obtém um pagamento de assinatura pelo identificador.
        /// </summary>
        /// <param name="id">Identificador do pagamento.</param>
        /// <returns>DTO do pagamento de assinatura.</returns>
        Task<SubscriptionPaymentDto> GetPaymentAsync(long id);

        /// <summary>
        /// Cria uma solicitação de pagamento para assinatura.
        /// </summary>
        Task<PaymentRequestDto> CreatePaymentAsync(CreateSubscriptionPaymentInput input);

        /// <summary>
        /// Processa o retorno de pagamento e ativa a assinatura.
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
        /// Lista os gateways de pagamento disponíveis e suas configurações.
        /// </summary>
        Task<List<PaymentGatewayDto>> GetGatewayListAsync();

        /// <summary>
        /// Obtém as configurações dos gateways de pagamento.
        /// </summary>
        Task<PaymentGatewaySettingsDto> GetGatewaySettingsAsync();

        /// <summary>
        /// Atualiza as configurações dos gateways de pagamento.
        /// </summary>
        Task UpdateGatewaySettingsAsync(PaymentGatewaySettingsDto input);
    }
}
