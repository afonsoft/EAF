using Abp.Dependency;
using Abp.MailKit;
using Abp.Net.Mail.Smtp;
using Eaf.MailKit.Configuration;
using MailKit.Net.Smtp;

namespace Eaf.MailKit
{
    /// <summary>
    /// Constrói o cliente SMTP do MailKit com configurações adicionais do EAF.
    /// </summary>
    public class EafMailKitSmtpBuilder : DefaultMailKitSmtpBuilder, ITransientDependency
    {
        private readonly EafMailKitConfiguration _configuration;

        /// <summary>
        /// EafMailKitSmtpBuilder.
        /// </summary>
        /// <param name="smtpEmailSenderConfiguration">Configuração SMTP do ABP.</param>
        /// <param name="abpMailKitConfiguration">Configuração do MailKit no ABP.</param>
        /// <param name="configuration">Configuração específica do EAF.</param>
        public EafMailKitSmtpBuilder(
            ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration,
            IAbpMailKitConfiguration abpMailKitConfiguration,
            EafMailKitConfiguration configuration)
            : base(smtpEmailSenderConfiguration, abpMailKitConfiguration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configura o cliente SMTP, aplicando a validação de certificado conforme configuração EAF.
        /// </summary>
        /// <param name="client">Cliente SMTP do MailKit.</param>
        protected override void ConfigureClient(SmtpClient client)
        {
            if (_configuration.DisableCertificateValidation)
            {
#pragma warning disable S4830 // Server certificates should be verified during SSL/TLS connections
                client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
#pragma warning restore S4830 // Server certificates should be verified during SSL/TLS connections
            }

            base.ConfigureClient(client);
        }
    }
}
