using Eaf.Middleware.Core.Editions;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// Entrada para atribuir uma edição a um tenant.
    /// </summary>
    public class AssignEditionToTenantInput
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int TenantId { get; set; }

        /// <summary>
        /// Identificador da edição.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int EditionId { get; set; }

        /// <summary>
        /// Período de pagamento desejado.
        /// </summary>
        public PaymentPeriodType PaymentPeriodType { get; set; }
    }
}
