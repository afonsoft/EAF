using Abp.Application.Services.Dto;
using Eaf.Middleware.Core.Editions;
using System;
using System.Collections.Generic;

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
        /// Indica se o pagamento é recorrente.
        /// </summary>
        public bool IsRecurring { get; set; }

        /// <summary>
        /// Indica se o pagamento é de prorrogação/up/down grade.
        /// </summary>
        public bool IsProrationPayment { get; set; }

        /// <summary>
        /// Gateway utilizado.
        /// </summary>
        public string Gateway { get; set; }

        /// <summary>
        /// Identificador externo do pagamento.
        /// </summary>
        public string ExternalPaymentId { get; set; }

        /// <summary>
        /// Identificador externo da assinatura (recorrente).
        /// </summary>
        public string GatewaySubscriptionId { get; set; }

        /// <summary>
        /// Número da fatura.
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Descrição do pagamento.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Propriedades extras serializadas.
        /// </summary>
        public string ExtraProperties { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de sucesso.
        /// </summary>
        public string SuccessUrl { get; set; }

        /// <summary>
        /// URL de redirecionamento em caso de erro.
        /// </summary>
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
        public List<SubscriptionPaymentProductDto> Products { get; set; } = new List<SubscriptionPaymentProductDto>();
    }
}
