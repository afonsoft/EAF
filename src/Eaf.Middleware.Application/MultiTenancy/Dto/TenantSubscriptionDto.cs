using System;

namespace Eaf.Middleware.MultiTenancy.Dto
{
    /// <summary>
    /// DTO com informações da assinatura de um tenant.
    /// </summary>
    public class TenantSubscriptionDto
    {
        /// <summary>
        /// Identificador do tenant.
        /// </summary>
        public int TenantId { get; set; }

        /// <summary>
        /// Identificador da edição atribuída.
        /// </summary>
        public int? EditionId { get; set; }

        /// <summary>
        /// Nome de exibição da edição.
        /// </summary>
        public string EditionDisplayName { get; set; }

        /// <summary>
        /// Data de término da assinatura em UTC.
        /// </summary>
        public DateTime? SubscriptionEndDateUtc { get; set; }

        /// <summary>
        /// Dias restantes até a expiração.
        /// </summary>
        public int? RemainingDays { get; set; }

        /// <summary>
        /// Indica se a assinatura está ativa.
        /// </summary>
        public bool IsActive { get; set; }
    }
}
