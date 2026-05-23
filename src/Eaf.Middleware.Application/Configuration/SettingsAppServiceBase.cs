using Abp.Net.Mail;
using Eaf.Middleware.Configuration.Host.Dto;
using System.Threading.Tasks;

namespace Eaf.Middleware.Configuration
{
    /// <summary>
    /// Representa a classe SettingsAppServiceBase.
    /// </summary>
    public abstract class SettingsAppServiceBase : MiddlewareAppServiceBase
    {
        private readonly IEmailSender _emailSender;

        protected SettingsAppServiceBase(
            IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        /// <summary>
        /// SendTestEmail.
        /// </summary>
        /// <param name="input">Parâmetro input.</param>
        public async Task SendTestEmail(SendTestEmailInput input)
        {
            await _emailSender.SendAsync(
                input.EmailAddress,
                L("TestEmail_Subject"),
                L("TestEmail_Body")
            );
        }
    }
}