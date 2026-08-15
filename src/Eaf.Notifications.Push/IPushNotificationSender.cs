using System.Threading.Tasks;

namespace Eaf.Notifications.Push
{
    /// <summary>
    /// High-level sender used by application code to dispatch push notifications.
    /// </summary>
    public interface IPushNotificationSender
    {
        /// <summary>
        /// Sends a push notification through the configured provider.
        /// </summary>
        /// <param name="subscription">Target subscription.</param>
        /// <param name="message">Notification message.</param>
        /// <returns>Result of the send attempt.</returns>
        Task<PushSendResult> SendAsync(PushSubscription subscription, PushNotificationMessage message);
    }
}
