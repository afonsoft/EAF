using Abp.Notifications;

namespace Eaf.Notifications.Sms
{
    /// <summary>
    /// Dados de notificação para envio por SMS.
    /// </summary>
    public class SmsNotificationData : MessageNotificationData
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="SmsNotificationData"/>.
        /// </summary>
        public SmsNotificationData() : base(string.Empty)
        {
        }

        /// <summary>
        /// Inicializa uma nova instância de <see cref="SmsNotificationData"/> com a mensagem.
        /// </summary>
        /// <param name="message">Mensagem do SMS.</param>
        public SmsNotificationData(string message) : base(message)
        {
        }

        /// <summary>
        /// Phone number to send the SMS to.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Optional sender identifier.
        /// </summary>
        public string From { get; set; }
    }
}
