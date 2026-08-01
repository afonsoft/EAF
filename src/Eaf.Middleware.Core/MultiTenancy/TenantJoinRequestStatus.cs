namespace Eaf.Middleware.MultiTenancy
{
    /// <summary>
    /// Representa o status de uma solicitação para ingressar em um tenant.
    /// </summary>
    public enum TenantJoinRequestStatus
    {
        /// <summary>
        /// Pendente de aprovação.
        /// </summary>
        Pending,

        /// <summary>
        /// Aprovada.
        /// </summary>
        Approved,

        /// <summary>
        /// Rejeitada.
        /// </summary>
        Rejected
    }
}
