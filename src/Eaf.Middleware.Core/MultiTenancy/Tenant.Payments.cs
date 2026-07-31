using System;

namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Propriedades de pagamento/assinatura para o Tenant.
    /// </summary>
    public partial class Tenant
    {
        /// <summary>
        /// Data/hora de término da assinatura (UTC).
        /// </summary>
        public DateTime? SubscriptionEndDateUtc { get; set; }
    }
}
