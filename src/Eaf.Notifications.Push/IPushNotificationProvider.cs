using System.Threading;
using System.Threading.Tasks;

namespace Eaf.Notifications.Push
{
    /// <summary>
    /// Abstraction for a push notification provider.
    /// </summary>
    public interface IPushNotificationProvider
    {
        /// <summary>
        /// Provider name used to select it from configuration.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sends the push notification to the given subscription.
        /// </summary>
        /// <param name="subscription">Target subscription.</param>
        /// <param name="message">Notification message.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the send attempt.</returns>
        Task<PushSendResult> SendAsync(PushSubscription subscription, PushNotificationMessage message, CancellationToken cancellationToken = default);
    }
}
