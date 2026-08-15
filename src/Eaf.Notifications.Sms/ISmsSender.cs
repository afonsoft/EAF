using System.Threading.Tasks;

namespace Eaf.Notifications.Sms
{
    /// <summary>
    /// High-level sender used by application code to dispatch SMS messages.
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// Sends an SMS message through the configured provider.
        /// </summary>
        /// <param name="message">Message to send.</param>
        /// <returns>Result of the send attempt.</returns>
        Task<SmsSendResult> SendAsync(SmsMessage message);
    }
}
