using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Notifications;
using Eaf.Notifications.Sms;

namespace Eaf.Notifications
{
    /// <summary>
    /// Notificador em tempo real que envia SMS usando os providers configurados.
    /// </summary>
    public class SmsRealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        /// <summary>
        /// Indica se o notificador deve ser usado apenas quando solicitado como destino.
        /// </summary>
        public bool UseOnlyIfRequestedAsTarget => false;

        private readonly ISmsSender _smsSender;

        /// <summary>
        /// SmsRealTimeNotifier.
        /// </summary>
        /// <param name="smsSender">Sender de SMS configurado.</param>
        public SmsRealTimeNotifier(ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }

        /// <summary>
        /// Envia notificações por SMS quando os dados forem <see cref="SmsNotificationData"/>.
        /// </summary>
        /// <param name="userNotifications">Notificações a serem enviadas.</param>
        public async Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                if (userNotification.Notification.Data is SmsNotificationData data &&
                    !string.IsNullOrWhiteSpace(data.PhoneNumber))
                {
                    await _smsSender.SendAsync(new SmsMessage
                    {
                        PhoneNumber = data.PhoneNumber,
                        Body = data.Message,
                        From = data.From
                    });
                }
            }
        }
    }
}
