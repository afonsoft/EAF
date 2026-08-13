using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// DTO para produto/linha de pagamento de assinatura.
    /// </summary>
    public class SubscriptionPaymentProductDto : EntityDto<long>
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Identificador do pagamento de assinatura.
        /// </summary>
        public long SubscriptionPaymentId { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Quantidade.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Valor unitário.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Valor total.
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
