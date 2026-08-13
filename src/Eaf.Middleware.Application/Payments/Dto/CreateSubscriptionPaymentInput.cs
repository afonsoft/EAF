using Eaf.Middleware.Core.Editions;
using System.Collections.Generic;

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
        /// Indica se o pagamento deve ser recorrente.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de sucesso.
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de erro.
        /// </summary>
        public string ErrorUrl { get; set; }

        /// <summary>
        /// Descrição opcional.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Produtos/linhas do pagamento. Se omitido, será gerado a partir da edição/período.
        /// </summary>
        public List<SubscriptionPaymentProductInput> Products { get; set; } = new List<SubscriptionPaymentProductInput>();
    }
}
