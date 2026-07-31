namespace Eaf.Middleware.MassNotifications
{
    /// <summary>
    /// Argumentos para o background job de notificação em massa.
    /// </summary>
    public class MassNotificationJobArgs
    {
        /// <summary>
        /// Identificador da notificação em massa.
        /// </summary>
        public long MassNotificationId { get; set; }
    }
}
