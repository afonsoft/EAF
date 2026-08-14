using Abp.Dependency;
using Abp.MailKit;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Threading;
using Abp.UI;
using Castle.Core.Logging;
using Eaf.MailKit.Configuration;
using MailKit.Net.Smtp;
using System;
using System.IO;
using System.Net.Mail;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Eaf.MailKit
{
    /// <summary>
    /// Envio de e-mail via MailKit com retry, observabilidade e configurações EAF.
    /// </summary>
    public class EafMailKitEmailSender : MailKitEmailSender, ITransientDependency
    {
        private static readonly System.Diagnostics.ActivitySource _activitySource = new("Eaf.MailKit");

        private readonly EafMailKitConfiguration _configuration;

        /// <summary>
        /// Logger do Castle Core.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// EafMailKitEmailSender.
        /// </summary>
        /// <param name="smtpEmailSenderConfiguration">Configuração do remetente SMTP.</param>
        /// <param name="smtpBuilder">Construtor do cliente SMTP.</param>
        /// <param name="configuration">Configuração do EAF MailKit.</param>
        public EafMailKitEmailSender(
            IEmailSenderConfiguration smtpEmailSenderConfiguration,
            IMailKitSmtpBuilder smtpBuilder,
            EafMailKitConfiguration configuration)
            : base(smtpEmailSenderConfiguration, smtpBuilder)
        {
            _configuration = configuration;
            Logger = NullLogger.Instance;
        }

        /// <summary>
        /// Envia um e-mail assíncrono com retry.
        /// </summary>
        public override async Task SendAsync(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            using var activity = _activitySource.StartActivity("SendEmail");
            activity?.SetTag("email.to", to);
            activity?.SetTag("email.subject", subject);

            await SendWithRetryAsync(() => base.SendAsync(from, to, subject, body, isBodyHtml));
        }

        /// <summary>
        /// Envia um e-mail síncrono com retry.
        /// </summary>
        public override void Send(string from, string to, string subject, string body, bool isBodyHtml = true)
        {
            AsyncHelper.RunSync(() => SendAsync(from, to, subject, body, isBodyHtml));
        }

        /// <summary>
        /// Envia um e-mail a partir de um <see cref="MailMessage"/> com retry.
        /// </summary>
        protected override async Task SendEmailAsync(MailMessage mail)
        {
            using var activity = _activitySource.StartActivity("SendEmail");
            SetActivityTags(activity, mail);

            await SendWithRetryAsync(() => base.SendEmailAsync(mail));
        }

        /// <summary>
        /// Envia um e-mail a partir de um <see cref="MailMessage"/> com retry (síncrono).
        /// </summary>
        protected override void SendEmail(MailMessage mail)
        {
            AsyncHelper.RunSync(() => SendEmailAsync(mail));
        }

        private async Task SendWithRetryAsync(Func<Task> send)
        {
            var lastException = default(Exception);
            var retryCount = _configuration.RetryCount;
            var delay = _configuration.RetryDelayMilliseconds;

            for (var attempt = 0; attempt <= retryCount; attempt++)
            {
                try
                {
                    await send();
                    return;
                }
                catch (Exception ex) when (attempt < retryCount && IsTransientFailure(ex))
                {
                    lastException = ex;
                    Logger.Warn($"E-mail send attempt {attempt + 1} failed; retrying after transient failure.", ex);
                    await Task.Delay(TimeSpan.FromMilliseconds(delay * Math.Pow(2, attempt)));
                }
            }

            if (lastException != null)
            {
                Logger.Error("E-mail send failed after all retries.", lastException);
                throw lastException;
            }
        }

        private static bool IsTransientFailure(Exception exception)
        {
            if (exception == null)
            {
                return false;
            }

            if (IsFatalException(exception))
            {
                return false;
            }

            if (exception is UserFriendlyException)
            {
                return false;
            }

            if (exception is TaskCanceledException || exception is TimeoutException)
            {
                return true;
            }

            if (exception is OperationCanceledException)
            {
                return false;
            }

            if (exception is SmtpCommandException smtpException)
            {
                var statusCode = (int)smtpException.StatusCode;
                return statusCode >= 400 && statusCode < 500;
            }

            return exception is SmtpProtocolException
                || exception is IOException
                || exception is SocketException;
        }

        private static bool IsFatalException(Exception exception)
        {
            return exception is OutOfMemoryException
                || exception is StackOverflowException
                || exception is ThreadAbortException;
        }

        private static void SetActivityTags(System.Diagnostics.Activity activity, MailMessage mail)
        {
            if (activity == null || mail == null)
            {
                return;
            }

            if (mail.To.Count > 0)
            {
                activity.SetTag("email.to", mail.To[0].Address);
            }

            activity.SetTag("email.subject", mail.Subject);
        }
    }
}
