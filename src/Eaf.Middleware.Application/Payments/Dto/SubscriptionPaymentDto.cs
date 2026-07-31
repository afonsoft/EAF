using Abp.Application.Services.Dto;
using Eaf.Middleware.Core.Editions;
using System;

namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// DTO para representar um pagamento de assinatura.
    /// </summary>
    public class SubscriptionPaymentDto : EntityDto<long>
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int? TenantId { get; set; }

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
        /// Valor pago.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Status do pagamento.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gateway utilizado.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// Identificador externo do pagamento.
        /// </summary>
        public string ExternalPaymentId { get; set; }

        /// <summary>
        /// Data/hora de pagamento confirmado.
        /// </summary>
        public DateTime? PaymentTime { get; set; }

        /// <summary>
        /// Data/hora de início da assinatura.
        /// </summary>
        public DateTime? SubscriptionStartDate { get; set; }

        /// <summary>
        /// Data/hora de término da assinatura.
        /// </summary>
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
