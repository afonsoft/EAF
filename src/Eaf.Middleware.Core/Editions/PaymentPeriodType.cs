namespace Eaf.Middleware.Core.Editions
{
    /// <summary>
    /// Representa a enumeração PaymentPeriodType.
    /// </summary>
    public enum PaymentPeriodType
    {
        /// <summary>
        /// Pagamento diário.
        /// </summary>
        Daily = 1,

        /// <summary>
        /// Pagamento semanal.
        /// </summary>
        Weekly = 7,

        /// <summary>
        /// Pagamento mensal.
        /// </summary>
        Monthly = 30,

        /// <summary>
        /// Pagamento trimestral.
        /// </summary>
        Quarterly = 90,

        /// <summary>
        /// Pagamento semestral.
        /// </summary>
        Biannual = 180,

        /// <summary>
        /// Pagamento anual.
        /// </summary>
        Annual = 365,

        /// <summary>
        /// Pagamento permanente (sem data de expiração).
        /// </summary>
        Permanent = 99999
    }
}
