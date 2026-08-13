using System;

namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Resultado do processamento de um pagamento.
    /// </summary>
    public class PaymentResultDto
    {
        /// <summary>
        /// Identificador externo do pagamento.
        /// </summary>
        public string ExternalPaymentId { get; set; }

        /// <summary>
        /// Identificador externo da assinatura (recorrente).
        /// </summary>
        public string GatewaySubscriptionId { get; set; }

        /// <summary>
        /// Número da fatura/invoice gerada.
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Data de término da assinatura (quando informada pelo gateway).
        /// </summary>
        public DateTime? SubscriptionEndDate { get; set; }

        /// <summary>
        /// Gateway utilizado.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// Indica se o pagamento foi confirmado.
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}
