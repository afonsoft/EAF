using Eaf.Notifications.Sms.Configuration;

namespace Eaf.Notifications.Sms
{
    /// <summary>
    /// Represents an SMS message to be sent by an <see cref="ISmsProvider"/>.
    /// </summary>
    public class SmsMessage
    {
        /// <summary>
        /// Destination phone number in E.164 or provider-specific format.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Message body. Providers may truncate or reject oversized bodies.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Optional sender identifier. When null, <see cref="SmsOptions.DefaultFrom"/> is used.
        /// </summary>
        public string From { get; set; }
    }
}
