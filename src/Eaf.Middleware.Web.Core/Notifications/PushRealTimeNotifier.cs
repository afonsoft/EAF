using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Notifications;
using Eaf.Notifications.Push;

namespace Eaf.Notifications
{
    /// <summary>
    /// Notificador em tempo real que envia notificações push para as subscriptions do usuário.
    /// </summary>
    public class PushRealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        /// <summary>
        /// Indica se o notificador deve ser usado apenas quando solicitado como destino.
        /// </summary>
        public bool UseOnlyIfRequestedAsTarget => false;

        private readonly IRepository<PushSubscription, long> _pushSubscriptionRepository;
        private readonly IPushNotificationSender _pushSender;

        /// <summary>
        /// PushRealTimeNotifier.
        /// </summary>
        /// <param name="pushSubscriptionRepository">Repositório de subscriptions push.</param>
        /// <param name="pushSender">Sender de notificações push.</param>
        public PushRealTimeNotifier(
            IRepository<PushSubscription, long> pushSubscriptionRepository,
            IPushNotificationSender pushSender)
        {
            _pushSubscriptionRepository = pushSubscriptionRepository;
            _pushSender = pushSender;
        }

        /// <summary>
        /// Envia notificações push para cada subscription ativa do destinatário.
        /// </summary>
        /// <param name="userNotifications">Notificações a serem enviadas.</param>
        public async Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                var message = CreateMessage(userNotification.Notification.Data);
                if (message == null)
                    continue;

                var subscriptions = await _pushSubscriptionRepository.GetAllListAsync(s =>
                    s.UserId == userNotification.UserId && s.TenantId == userNotification.TenantId);

                foreach (var subscription in subscriptions)
                {
                    try
                    {
                        await _pushSender.SendAsync(subscription, message);
                    }
                    catch (WebPush.WebPushException ex) when (ex.StatusCode == HttpStatusCode.Gone ||
                                                              ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        await _pushSubscriptionRepository.DeleteAsync(subscription);
                    }
                }
            }
        }

        private static PushNotificationMessage CreateMessage(object notificationData)
        {
            if (notificationData is PushNotificationData pushData)
            {
                return new PushNotificationMessage
                {
                    Title = pushData.Message,
                    Body = pushData.Message,
                    Icon = pushData.Icon,
                    Data = pushData.Data,
                    Tag = pushData.Tag
                };
            }

            if (notificationData is MessageNotificationData messageData)
            {
                return new PushNotificationMessage
                {
                    Title = messageData.Message,
                    Body = messageData.Message
                };
            }

            return null;
        }
    }
}
