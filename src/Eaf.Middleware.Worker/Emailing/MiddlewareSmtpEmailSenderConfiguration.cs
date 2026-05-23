using Abp.Configuration;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Runtime.Security;

namespace Eaf.Middleware.Worker.Emailing
{
    /// <summary>
    /// Representa a classe MiddlewareSmtpEmailSenderConfiguration.
    /// </summary>
    public class MiddlewareSmtpEmailSenderConfiguration : SmtpEmailSenderConfiguration
    {
        /// <summary>
        /// MiddlewareSmtpEmailSenderConfiguration.
        /// </summary>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public MiddlewareSmtpEmailSenderConfiguration(
            ISettingManager settingManager
        ) : base(settingManager)
        {
        }

        /// <summary>
        /// Decrypt.
        /// </summary>
        public override string Domain => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Domain));
        /// <summary>
        /// Decrypt.
        /// </summary>
        public override string Host => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Host));
        /// <summary>
        /// Decrypt.
        /// </summary>
        public override string Password => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.Password));
        /// <summary>
        /// Decrypt.
        /// </summary>
        public override string UserName => SimpleStringCipher.Instance.Decrypt(GetNotEmptySettingValue(EmailSettingNames.Smtp.UserName));
    }
}