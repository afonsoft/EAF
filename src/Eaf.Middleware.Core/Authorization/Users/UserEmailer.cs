using Abp;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Localization;
using Abp.Net.Mail;
using Abp.Runtime.Security;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Localization;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Net.Emailing;
using System;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Eaf.Middleware.Authorization.Users
{
    /// <summary>
    /// Used to send email to users.
    /// </summary>
    public class UserEmailer : DomainService, IUserEmailer, ITransientDependency
    {
        private const string HtmlBoldEndWithColon = "</b>: ";
        private const string HtmlBreak = "<br />";

        private readonly IEmailSender _emailSender;
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IRepository<User, long> _userRepository;

        /// <summary>
        /// UserEmailer.
        /// </summary>
        /// <param name="emailTemplateProvider">Parâmetro emailTemplateProvider.</param>
        /// <param name="emailSender">Parâmetro emailSender.</param>
        /// <param name="tenantRepository">Parâmetro tenantRepository.</param>
        /// <param name="unitOfWorkProvider">Parâmetro unitOfWorkProvider.</param>
        /// <param name="unitOfWorkManager">Parâmetro unitOfWorkManager.</param>
        /// <param name="userRepository">Parâmetro userRepository.</param>
        /// <param name="settingManager">Parâmetro settingManager.</param>
        /// <returns>Resultado da operação.</returns>
        public UserEmailer(
            IEmailTemplateProvider emailTemplateProvider,
            IEmailSender emailSender,
            IRepository<Tenant> tenantRepository,
            ICurrentUnitOfWorkProvider unitOfWorkProvider,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<User, long> userRepository,
            ISettingManager settingManager
        )
        {
            _emailTemplateProvider = emailTemplateProvider;
            _emailSender = emailSender;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            _unitOfWorkManager = unitOfWorkManager;
            _userRepository = userRepository;
            _settingManager = settingManager;

            LocalizationSourceName = Localization.MiddlewareLocalizationHelper.DefaultSourceName;
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources.
        /// </summary>
        protected override string L(string name)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources com formatação.
        /// </summary>
        protected override string L(string name, params object[] args)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, args);
        }

        /// <summary>
        /// Obtém a string localizada com fallback para múltiplos sources para uma cultura específica.
        /// </summary>
        protected override string L(string name, CultureInfo culture)
        {
            return MiddlewareLocalizationHelper.Localize(LocalizationManager, name, culture);
        }

        /// <summary>
        /// Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        /// Can be set to user's plain password to include it in the email.
        /// </param>
        [UnitOfWork]
        public virtual async Task SendEmailActivationLinkAsync(User user, string link, string plainPassword = null)
        {
            if (user.EmailConfirmationCode.IsNullOrEmpty())
            {
                throw new AbpException("EmailConfirmationCode should be set in order to send email activation link.");
            }

            link = link.Replace("{userId}", user.Id.ToString());
            link = link.Replace("{confirmationCode}", Uri.EscapeDataString(user.EmailConfirmationCode));

            if (user.TenantId.HasValue)
            {
                link = link.Replace("{tenantId}", user.TenantId.ToString());
            }

            link = EncryptQueryParameters(link);

            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("EmailActivation_Title"), L("EmailActivation_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + HtmlBoldEndWithColon + user.Name + " " + user.Surname + HtmlBreak);

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("TenancyName") + HtmlBoldEndWithColon + tenancyName + HtmlBreak);
            }

            mailMessage.AppendLine("<b>" + L("UserName") + HtmlBoldEndWithColon + user.UserName + HtmlBreak);

            if (!plainPassword.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("Password") + HtmlBoldEndWithColon + plainPassword + HtmlBreak);
            }

            mailMessage.AppendLine(HtmlBreak);
            mailMessage.AppendLine(L("EmailActivation_ClickTheLinkBelowToVerifyYourEmail") + "<br /><br />");
            mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");

            await ReplaceBodyAndSend(user.EmailAddress, L("EmailActivation_Subject"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Reset link</param>
        public async Task SendPasswordResetLinkAsync(User user, string link = null)
        {
            if (user.PasswordResetCode.IsNullOrEmpty())
            {
                throw new AbpException("PasswordResetCode should be set in order to send password reset link.");
            }
            var authenticationSource = user.AuthenticationSource;
            var tenancyName = GetTenancyNameOrNull(user.TenantId);
            var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("PasswordResetEmail_Title"), L("PasswordResetEmail_SubTitle"));
            var mailMessage = new StringBuilder();

            mailMessage.AppendLine("<b>" + L("NameSurname") + HtmlBoldEndWithColon + user.Name + " " + user.Surname + HtmlBreak);

            if (!tenancyName.IsNullOrEmpty())
            {
                mailMessage.AppendLine("<b>" + L("TenancyName") + HtmlBoldEndWithColon + tenancyName + HtmlBreak);
            }

            mailMessage.AppendLine("<b>" + L("UserName") + HtmlBoldEndWithColon + user.UserName + HtmlBreak);
            mailMessage.AppendLine("<b>" + L("ResetCode") + HtmlBoldEndWithColon + user.PasswordResetCode + HtmlBreak);

            if (!link.IsNullOrEmpty())
            {
                link = link.Replace("{userId}", user.Id.ToString());
                link = link.Replace("{resetCode}", Uri.EscapeDataString(user.PasswordResetCode));

                if (user.TenantId.HasValue)
                {
                    link = link.Replace("{tenantId}", user.TenantId.ToString());
                }

                link = link.Replace("{authenticationSource}", (string.IsNullOrEmpty(authenticationSource) ? "System" : authenticationSource));

                link = EncryptQueryParameters(link);

                mailMessage.AppendLine(HtmlBreak);
                mailMessage.AppendLine(L("PasswordResetEmail_ClickTheLinkBelowToResetYourPassword") + "<br /><br />");
                mailMessage.AppendLine("<a href=\"" + link + "\">" + link + "</a>");
            }

            await ReplaceBodyAndSend(user.EmailAddress, L("PasswordResetEmail_Subject"), emailTemplate, mailMessage);
        }

        /// <summary>
        /// TryToSendChatMessageMail.
        /// </summary>
        /// <param name="user">Parâmetro user.</param>
        /// <param name="senderUsername">Parâmetro senderUsername.</param>
        /// <param name="senderTenancyName">Parâmetro senderTenancyName.</param>
        /// <param name="chatMessage">Parâmetro chatMessage.</param>
        public async Task TryToSendChatMessageMail(User user, string senderUsername, string senderTenancyName, ChatMessage chatMessage)
        {
            try
            {
                var emailTemplate = GetTitleAndSubTitle(user.TenantId, L("NewChatMessageEmail_Title"), L("NewChatMessageEmail_SubTitle"));
                var mailMessage = new StringBuilder();

                mailMessage.AppendLine("<b>" + L("Sender") + HtmlBoldEndWithColon + senderTenancyName + "/" + senderUsername + HtmlBreak);
                mailMessage.AppendLine("<b>" + L("Time") + HtmlBoldEndWithColon + chatMessage.CreationTime.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss") + " UTC<br />");
                mailMessage.AppendLine("<b>" + L("Message") + HtmlBoldEndWithColon + chatMessage.Message + HtmlBreak);
                mailMessage.AppendLine(HtmlBreak);

                await ReplaceBodyAndSend(user.EmailAddress, L("NewChatMessageEmail_Subject"), emailTemplate, mailMessage);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        /// <summary>
        /// TryToSendSubscriptionExpireEmail.
        /// </summary>
        /// <param name="tenantId">Parâmetro tenantId.</param>
        /// <param name="utcNow">Parâmetro utcNow.</param>
        public async Task TryToSendSubscriptionExpireEmail(int tenantId, DateTime utcNow)
        {
            try
            {
                using (_unitOfWorkManager.Begin())
                {
                    using (_unitOfWorkManager.Current.SetTenantId(tenantId))
                    {
                        var tenantAdmin = await _userRepository.FirstOrDefaultAsync(u => u.UserName == AbpUserBase.AdminUserName);
                        if (tenantAdmin == null || string.IsNullOrEmpty(tenantAdmin.EmailAddress))
                        {
                            return;
                        }

                        var hostAdminLanguage = await _settingManager.GetSettingValueForUserAsync(LocalizationSettingNames.DefaultLanguage, tenantAdmin.TenantId, tenantAdmin.Id);
                        var culture = CultureHelper.GetCultureInfoByChecking(hostAdminLanguage);
                        var emailTemplate = GetTitleAndSubTitle(tenantId, L("SubscriptionExpire_Title"), L("SubscriptionExpire_SubTitle"));
                        var mailMessage = new StringBuilder();

                        mailMessage.AppendLine("<b>" + L("Message") + HtmlBoldEndWithColon + L("SubscriptionExpire_Email_Body", culture, utcNow.ToString("yyyy-MM-dd") + " UTC") + HtmlBreak);
                        mailMessage.AppendLine(HtmlBreak);

                        await ReplaceBodyAndSend(tenantAdmin.EmailAddress, L("SubscriptionExpire_Email_Subject"), emailTemplate, mailMessage);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
            }
        }

        private static string EncryptQueryParameters(string link, string encrptedParameterName = "c")
        {
            if (!link.Contains("?"))
            {
                return link;
            }

            var uri = new Uri(link);
            var basePath = link[..link.IndexOf('?')];
            var query = uri.Query.TrimStart('?');

            return basePath + "?" + encrptedParameterName + "=" + HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(query));
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {
                return _tenantRepository.Get(tenantId.Value).TenancyName;
            }
        }

        private StringBuilder GetTitleAndSubTitle(int? tenantId, string title, string subTitle)
        {
            var emailTemplate = new StringBuilder(_emailTemplateProvider.GetDefaultTemplate(tenantId));
            emailTemplate.Replace("{EMAIL_TITLE}", title);
            emailTemplate.Replace("{EMAIL_SUB_TITLE}", subTitle);

            return emailTemplate;
        }

        private async Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate, StringBuilder mailMessage)
        {
            emailTemplate.Replace("{EMAIL_BODY}", mailMessage.ToString());
            await _emailSender.SendAsync(new MailMessage
            {
                To = { emailAddress },
                Subject = subject,
                Body = emailTemplate.ToString(),
                IsBodyHtml = true
            });
        }
    }
}