using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Produto/linha de um pagamento de assinatura.
    /// </summary>
    [Table("EafSubscriptionPaymentProducts")]
    public class SubscriptionPaymentProduct : FullAuditedEntity<long>, IMayHaveTenant
    {
        public const int MaxDescriptionLength = 512;

        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Identificador do pagamento de assinatura.
        /// </summary>
        public long SubscriptionPaymentId { get; set; }

        /// <summary>
        /// Pagamento de assinatura pai.
        /// </summary>
        public virtual SubscriptionPayment SubscriptionPayment { get; set; }

        /// <summary>
        /// Descrição do produto.
        /// </summary>
        [StringLength(MaxDescriptionLength)]
        public string Description { get; set; }

        /// <summary>
        /// Quantidade do produto.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Valor unitário do produto.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Valor total (Amount * Count).
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
