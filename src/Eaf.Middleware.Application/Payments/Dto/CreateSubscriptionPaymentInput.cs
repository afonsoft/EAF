using Eaf.Middleware.Core.Editions;

namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Entrada para criação de um pagamento de assinatura.
    /// </summary>
    public class CreateSubscriptionPaymentInput
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
        /// Gateway de pagamento.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// Descrição opcional.
        /// </summary>
        public string Description { get; set; }
    }
}
