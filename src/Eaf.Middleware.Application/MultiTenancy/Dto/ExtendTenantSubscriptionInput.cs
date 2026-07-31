using Eaf.Middleware.Core.Editions;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Entrada para estender a assinatura de um tenant.
    /// </summary>
    public class ExtendTenantSubscriptionInput
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int TenantId { get; set; }

        /// <summary>
        /// Período de pagamento a ser adicionado.
        /// </summary>
        public PaymentPeriodType PaymentPeriodType { get; set; }
    }
}
