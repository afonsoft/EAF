namespace Eaf.Middleware.MassNotifications
{
    /// <summary>
    /// Status de uma notificação em massa.
    /// </summary>
    public enum MassNotificationStatus
    {
        /// <summary>
        /// Notificação agendada/pendente.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Notificação enviada.
        /// </summary>
        Sent = 1,

        /// <summary>
        /// Notificação cancelada.
        /// </summary>
        Canceled = 2,
    }
}
