using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using MailKit.Net.Smtp;

namespace Eaf.Middleware.Worker.Emailing
{
    /// <summary>
    /// Representa a classe MiddlewareMailKitSmtpBuilder.
    /// </summary>
    public class MiddlewareMailKitSmtpBuilder : DefaultMailKitSmtpBuilder
    {
        /// <summary>
        /// MiddlewareMailKitSmtpBuilder.
        /// </summary>
        /// <param name="smtpEmailSenderConfiguration">Parâmetro smtpEmailSenderConfiguration.</param>
        /// <param name="eafMailKitConfiguration">Parâmetro eafMailKitConfiguration.</param>
        /// <returns>Resultado da operação.</returns>
        public MiddlewareMailKitSmtpBuilder(
            ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
            IAbpMailKitConfiguration eafMailKitConfiguration) : base(smtpEmailSenderConfiguration, eafMailKitConfiguration)
        {
        }

        protected override void ConfigureClient(SmtpClient client)
        {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
            client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
            base.ConfigureClient(client);
        }
    }
}