using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Eaf.Middleware.Core.Editions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Representa um pagamento de assinatura de um tenant.
    /// </summary>
    [Table("EafSubscriptionPayments")]
    public class SubscriptionPayment : FullAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxDescriptionLength = 1024;
        public const int MaxExternalPaymentIdLength = 256;
        public const int MaxGatewayResponseLength = 4000;

        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Identificador da edição assinada.
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
        public SubscriptionPaymentStatus Status { get; set; }

        /// <summary>
        /// Gateway de pagamento utilizado.
        /// </summary>
        [StringLength(MaxExternalPaymentIdLength)]
        public string Gateway { get; set; }

        /// <summary>
        /// Identificador externo do pagamento.
        /// </summary>
        [StringLength(MaxExternalPaymentIdLength)]
        public string ExternalPaymentId { get; set; }

        /// <summary>
        /// Descrição ou comentário do pagamento.
        /// </summary>
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        /// <summary>
        /// Resposta do gateway (JSON ou texto).
        /// </summary>
        [StringLength(MaxGatewayResponseLength)]
        public string GatewayResponse { get; set; }

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
