using System.Threading;
using System.Threading.Tasks;
using Eaf.Notifications.Sms.Configuration;

namespace Eaf.Notifications.Sms
{
    /// <summary>
    /// Abstraction for an SMS gateway provider.
    /// </summary>
    public interface ISmsProvider
    {
        /// <summary>
        /// Provider name used to select it from <see cref="SmsOptions.Provider"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sends the SMS message.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of the send attempt.</returns>
        Task<SmsSendResult> SendAsync(SmsMessage message, CancellationToken cancellationToken = default);
    }
}
