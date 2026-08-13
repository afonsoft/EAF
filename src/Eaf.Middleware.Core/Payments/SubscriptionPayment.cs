using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Eaf.Middleware.Core.Editions;
using System;
using System.Collections.Generic;
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
        public const int MaxInvoiceNoLength = 128;
        public const int MaxExtraPropertiesLength = 4000;
        public const int MaxUrlLength = 512;

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
        /// Indica se o pagamento é recorrente.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Indica se o pagamento é de prorrogação/up/down grade.
        /// </summary>
        public bool IsProrationPayment { get; set; }

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
        /// Identificador externo da assinatura (recorrente).
        /// </summary>
        [StringLength(MaxExternalPaymentIdLength)]
        public string GatewaySubscriptionId { get; set; }

        /// <summary>
        /// Número da nota fiscal ou fatura.
        /// </summary>
        [StringLength(MaxInvoiceNoLength)]
        public string InvoiceNo { get; set; }

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
        /// Propriedades extras serializadas (JSON).
        /// </summary>
        [StringLength(MaxExtraPropertiesLength)]
        public string ExtraProperties { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de sucesso.
        /// </summary>
        [StringLength(MaxUrlLength)]
        public string SuccessUrl { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de erro.
        /// </summary>
        [StringLength(MaxUrlLength)]
        public string ErrorUrl { get; set; }

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

        /// <summary>
        /// Produtos/linhas do pagamento.
        /// </summary>
        public virtual ICollection<SubscriptionPaymentProduct> Products { get; set; } = new List<SubscriptionPaymentProduct>();
    }
}
