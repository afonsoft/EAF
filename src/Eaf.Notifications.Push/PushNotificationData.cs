using Abp.Notifications;

namespace Eaf.Notifications.Push
{
    /// <summary>
    /// Dados de notificação para envio push.
    /// </summary>
    public class PushNotificationData : MessageNotificationData
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="PushNotificationData"/>.
        /// </summary>
        public PushNotificationData() : base(string.Empty)
        {
        }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="PushNotificationData"/> com a mensagem.
        /// </summary>
        /// <param name="message">Mensagem/título da notificação push.</param>
        public PushNotificationData(string message) : base(message)
        {
        }

        /// <summary>
        /// Icon URL or path for the push notification.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Optional payload data sent to the client.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Tag used to group or replace notifications.
        /// </summary>
        public string Tag { get; set; }
    }
}
