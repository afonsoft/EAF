namespace Eaf.Middleware.Payments
{
    /// <summary>
    /// Status de um pagamento de assinatura.
    /// </summary>
    public enum SubscriptionPaymentStatus
    {
        /// <summary>
        /// Pagamento pendente.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Pagamento concluído com sucesso.
        /// </summary>
        Completed = 1,

        /// <summary>
        /// Pagamento cancelado ou reembolsado.
        /// </summary>
        Canceled = 2,

        /// <summary>
        /// Pagamento falhou.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Pagamento reembolsado.
        /// </summary>
        Refunded = 4,

        /// <summary>
        /// Pagamento atrasado / em inadimplência (recorrente).
        /// </summary>
        PastDue = 5,
    }
}
