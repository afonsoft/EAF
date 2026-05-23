using Abp.Dependency;
using Eaf.Middleware.Authorization.Users;
using Abp.Net.Mail;
using System.Threading.Tasks;
using Abp.Notifications;

namespace Eaf.Notifications
{
    /// <summary>
    /// Representa a classe EmailRealTimeNotifier.
    /// </summary>
    public class EmailRealTimeNotifier : IRealTimeNotifier, ITransientDependency
    {
        /// <summary>
        /// If true, this real time notifier will be used for sending real time notifications when it is requested. Otherwise it will not be used.
        /// <para>
        /// If false, this realtime notifier will notify any notifications.
        /// </para>
        /// </summary>
        public bool UseOnlyIfRequestedAsTarget => false;

        private readonly IEmailSender _emailSender;
        private readonly UserManager _userManager;

        /// <summary>
        /// EmailRealTimeNotifier.
        /// </summary>
        /// <param name="emailSender">Parâmetro emailSender.</param>
        /// <param name="userManager">Parâmetro userManager.</param>
        /// <returns>Resultado da operação.</returns>
        public EmailRealTimeNotifier(
            IEmailSender emailSender,
            UserManager userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        /// <summary>
        /// SendNotificationsAsync.
        /// </summary>
        /// <param name="userNotifications">Parâmetro userNotifications.</param>
        public async Task SendNotificationsAsync(UserNotification[] userNotifications)
        {
            foreach (var userNotification in userNotifications)
            {
                if (userNotification.Notification.Data is MessageNotificationData data)
                {
                    var user = await _userManager.GetUserByIdAsync(userNotification.UserId);

                    _emailSender.Send(
                        to: user.EmailAddress,
                        subject: "You have a new notification!",
                        body: data.Message,
                        isBodyHtml: true
                    );
                }
            }
        }
    }
}