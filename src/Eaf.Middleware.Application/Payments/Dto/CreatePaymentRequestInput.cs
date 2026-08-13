using Eaf.Middleware.Core.Editions;
using System.Collections.Generic;

namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Entrada para criação de uma solicitação de pagamento.
    /// </summary>
    public class CreatePaymentRequestInput
    {
        /// <summary>
        /// Identificador da edição.
        /// </summary>
        public int EditionId { get; set; }

        /// <summary>
        /// Tipo de pagamento da edição.
        /// </summary>
        public EditionPaymentType EditionPaymentType { get; set; }

        /// <summary>
        /// Período de pagamento.
        /// </summary>
        public PaymentPeriodType PaymentPeriodType { get; set; }

        /// <summary>
        /// Valor a ser pago.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Indica se o pagamento deve ser recorrente.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Descrição do pagamento.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gateway de pagamento desejado.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de sucesso.
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de erro/cancelamento.
        /// </summary>
        public string ErrorUrl { get; set; }

        /// <summary>
        /// Produtos/linhas do pagamento.
        /// </summary>
        public List<SubscriptionPaymentProductInput> Products { get; set; } = new List<SubscriptionPaymentProductInput>();
    }
}
