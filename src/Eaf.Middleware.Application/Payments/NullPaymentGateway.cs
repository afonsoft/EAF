using Abp.Dependency;
using Eaf.Middleware.Payments.Dto;
using System;
using System.Threading.Tasks;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Implementação nula do gateway de pagamento para cenários sem gateway configurado.
    /// </summary>
    public class NullPaymentGateway : IPaymentGateway, ITransientDependency
    {
        /// <summary>
        /// Cria uma solicitação de pagamento simulada.
        /// </summary>
        public Task<PaymentRequestDto> CreatePaymentAsync(CreatePaymentRequestInput input)
        {
            return Task.FromResult(new PaymentRequestDto
            {
                PaymentId = Guid.NewGuid().ToString("N"),
                Gateway = "Null",
                CheckoutUrl = null,
                IsSuccess = true,
            });
        }

        /// <summary>
        /// Processa o retorno de pagamento simulado.
        /// </summary>
        public Task<PaymentResultDto> ProcessPaymentAsync(ProcessPaymentInput input)
        {
            return Task.FromResult(new PaymentResultDto
            {
                ExternalPaymentId = input.ExternalPaymentId,
                Gateway = input.Gateway,
                IsSuccess = true,
            });
        }
    }
}
