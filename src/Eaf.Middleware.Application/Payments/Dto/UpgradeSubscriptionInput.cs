using Eaf.Middleware.Core.Editions;

namespace Eaf.Middleware.Payments.Dto
{
    /// <summary>
    /// Entrada para upgrade/downgrade de assinatura.
    /// </summary>
    public class UpgradeSubscriptionInput
    {
        /// <summary>
        /// Identificador do tenant. Se não informado, utiliza o tenant atual.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// Identificador da nova edição.
        /// </summary>
        public int NewEditionId { get; set; }

        /// <summary>
        /// Período de pagamento desejado.
        /// </summary>
        public PaymentPeriodType PaymentPeriodType { get; set; }

        /// <summary>
        /// Gateway de pagamento para cobrança da diferença.
        /// </summary>
        public string Gateway { get; set; }
    }
}
