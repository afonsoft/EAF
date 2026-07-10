using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Net.Mail;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Chat;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Net.Emailing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Eaf.Middleware.Tests.Authorization
{
    /// <summary>
    /// Testes BDD para UserEmailer seguindo o padrão Dado/Quando/Então.
    /// </summary>
    public class UserEmailerBddTests
    {
        private static UserEmailer CriarSut()
        {
            return new UserEmailer(
                Substitute.For<IEmailTemplateProvider>(),
                Substitute.For<IEmailSender>(),
                Substitute.For<IRepository<Tenant>>(),
                Substitute.For<ICurrentUnitOfWorkProvider>(),
                Substitute.For<IUnitOfWorkManager>(),
                Substitute.For<IRepository<User, long>>(),
                Substitute.For<ISettingManager>());
        }

        [Fact]
        public void Dado_Dependencias_Quando_CriarUserEmailer_Entao_DeveImplementarIUserEmailer()
        {
            var sut = CriarSut();

            sut.ShouldBeAssignableTo<IUserEmailer>();
        }

        [Fact]
        public async Task Dado_UsuarioSemEmailConfirmationCode_Quando_SendEmailActivationLinkAsync_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut();
            var user = new User { EmailConfirmationCode = null };

            await Should.ThrowAsync<AbpException>(() => sut.SendEmailActivationLinkAsync(user, "http://link"));
        }

        [Fact]
        public async Task Dado_UsuarioSemPasswordResetCode_Quando_SendPasswordResetLinkAsync_Entao_DeveLancarAbpException()
        {
            var sut = CriarSut();
            var user = new User { PasswordResetCode = null };

            await Should.ThrowAsync<AbpException>(() => sut.SendPasswordResetLinkAsync(user));
        }

        [Fact]
        public async Task Dado_UsuarioComEmailConfirmationCode_Quando_SendEmailActivationLinkAsync_Entao_DeveEnviarEmail()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var tenant = new Tenant("tenant1", "Tenant 1");
            tenantRepository.Get(1).Returns(tenant);

            var currentUnitOfWork = Substitute.For<IUnitOfWork>();
            currentUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            currentUowProvider.Current.Returns(currentUnitOfWork);

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            var user = new User
            {
                EmailConfirmationCode = "code",
                TenantId = 1,
                Name = "Nome",
                Surname = "Sobrenome",
                UserName = "usuario",
                EmailAddress = "usuario@example.com"
            };

            // Quando
            await sut.SendEmailActivationLinkAsync(user, "http://link");

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "EmailActivation_Subject" &&
                m.To[0].Address == "usuario@example.com"));
        }

        [Fact]
        public async Task Dado_UsuarioSemTenant_Quando_SendEmailActivationLinkAsync_Entao_DeveEnviarEmailSemTenantId()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var currentUnitOfWork = Substitute.For<IUnitOfWork>();
            currentUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            currentUowProvider.Current.Returns(currentUnitOfWork);

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            var user = new User
            {
                EmailConfirmationCode = "code",
                TenantId = null,
                Name = "Nome",
                Surname = "Sobrenome",
                UserName = "usuario",
                EmailAddress = "usuario@example.com"
            };

            // Quando
            await sut.SendEmailActivationLinkAsync(user, "http://link");

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "EmailActivation_Subject" &&
                m.To[0].Address == "usuario@example.com"));
        }

        [Fact]
        public async Task Dado_UsuarioComPasswordResetCode_Quando_SendPasswordResetLinkAsync_Entao_DeveEnviarEmail()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var tenant = new Tenant("tenant1", "Tenant 1");
            tenantRepository.Get(1).Returns(tenant);

            var currentUnitOfWork = Substitute.For<IUnitOfWork>();
            currentUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            currentUowProvider.Current.Returns(currentUnitOfWork);

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            var user = new User
            {
                PasswordResetCode = "reset",
                TenantId = 1,
                Name = "Nome",
                Surname = "Sobrenome",
                UserName = "usuario",
                EmailAddress = "usuario@example.com"
            };

            // Quando
            await sut.SendPasswordResetLinkAsync(user, "http://link");

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "PasswordResetEmail_Subject" &&
                m.To[0].Address == "usuario@example.com"));
        }

        [Fact]
        public async Task Dado_UsuarioComPasswordResetCodeSemLink_Quando_SendPasswordResetLinkAsync_Entao_DeveEnviarEmail()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var tenant = new Tenant("tenant1", "Tenant 1");
            tenantRepository.Get(1).Returns(tenant);

            var currentUnitOfWork = Substitute.For<IUnitOfWork>();
            currentUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            currentUowProvider.Current.Returns(currentUnitOfWork);

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            var user = new User
            {
                PasswordResetCode = "reset",
                TenantId = 1,
                Name = "Nome",
                Surname = "Sobrenome",
                UserName = "usuario",
                EmailAddress = "usuario@example.com"
            };

            // Quando
            await sut.SendPasswordResetLinkAsync(user);

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "PasswordResetEmail_Subject" &&
                m.To[0].Address == "usuario@example.com"));
        }

        [Fact]
        public async Task Dado_UsuarioEChatMessage_Quando_TryToSendChatMessageMail_Entao_DeveEnviarEmail()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            var user = new User
            {
                TenantId = 1,
                EmailAddress = "usuario@example.com"
            };

            var chatMessage = new ChatMessage(
                new UserIdentifier(1, 1),
                new UserIdentifier(1, 2),
                ChatSide.Sender,
                "Ola",
                ChatMessageReadState.Unread,
                Guid.NewGuid(),
                ChatMessageReadState.Unread);

            // Quando
            await sut.TryToSendChatMessageMail(user, "remetente", "tenant1", chatMessage);

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "NewChatMessageEmail_Subject" &&
                m.To[0].Address == "usuario@example.com"));
        }

        [Fact]
        public async Task Dado_TenantComAdmin_Quando_TryToSendSubscriptionExpireEmail_Entao_DeveEnviarEmail()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());

            unitOfWorkManager.Begin().Returns(Substitute.For<IUnitOfWorkCompleteHandle>());
            unitOfWorkManager.Current.Returns(activeUnitOfWork);

            var tenantAdmin = new User
            {
                TenantId = 1,
                Id = 1,
                UserName = Abp.Authorization.Users.AbpUserBase.AdminUserName,
                EmailAddress = "admin@example.com"
            };
            userRepository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
                .Returns(Task.FromResult(tenantAdmin));

            settingManager.GetSettingValueForUserAsync(
                LocalizationSettingNames.DefaultLanguage,
                tenantAdmin.TenantId,
                tenantAdmin.Id)
                .Returns(Task.FromResult("pt-BR"));

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            // Quando
            await sut.TryToSendSubscriptionExpireEmail(1, DateTime.UtcNow);

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "SubscriptionExpire_Email_Subject" &&
                m.To[0].Address == "admin@example.com"));
        }

        [Fact]
        public async Task Dado_TenantSemAdmin_Quando_TryToSendSubscriptionExpireEmail_Entao_NaoDeveEnviarEmail()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var activeUnitOfWork = Substitute.For<IActiveUnitOfWork>();
            activeUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());

            unitOfWorkManager.Begin().Returns(Substitute.For<IUnitOfWorkCompleteHandle>());
            unitOfWorkManager.Current.Returns(activeUnitOfWork);

            userRepository.FirstOrDefaultAsync(Arg.Any<System.Linq.Expressions.Expression<Func<User, bool>>>())
                .Returns(Task.FromResult<User>(null!));

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            // Quando
            await sut.TryToSendSubscriptionExpireEmail(1, DateTime.UtcNow);

            // Então
            await emailSender.DidNotReceive().SendAsync(Arg.Any<MailMessage>());
        }

        [Fact]
        public async Task Dado_UsuarioComPlainPassword_Quando_SendEmailActivationLinkAsync_Entao_DeveIncluirSenhaNoEmail()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var currentUnitOfWork = Substitute.For<IUnitOfWork>();
            currentUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            currentUowProvider.Current.Returns(currentUnitOfWork);

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            var user = new User
            {
                EmailConfirmationCode = "code",
                TenantId = 1,
                Name = "Nome",
                Surname = "Sobrenome",
                UserName = "usuario",
                EmailAddress = "usuario@example.com"
            };

            // Quando
            await sut.SendEmailActivationLinkAsync(user, "http://link", "plain-password");

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "EmailActivation_Subject" &&
                m.Body.Contains("plain-password")));
        }

        [Fact]
        public async Task Dado_UsuarioComAuthenticationSource_Quando_SendPasswordResetLinkAsync_Entao_DeveCriptografarLink()
        {
            // Dado
            var emailTemplateProvider = Substitute.For<IEmailTemplateProvider>();
            var emailSender = Substitute.For<IEmailSender>();
            var tenantRepository = Substitute.For<IRepository<Tenant>>();
            var currentUowProvider = Substitute.For<ICurrentUnitOfWorkProvider>();
            var unitOfWorkManager = Substitute.For<IUnitOfWorkManager>();
            var userRepository = Substitute.For<IRepository<User, long>>();
            var settingManager = Substitute.For<ISettingManager>();

            var sut = new UserEmailer(
                emailTemplateProvider,
                emailSender,
                tenantRepository,
                currentUowProvider,
                unitOfWorkManager,
                userRepository,
                settingManager);

            var currentUnitOfWork = Substitute.For<IUnitOfWork>();
            currentUnitOfWork.SetTenantId(Arg.Any<int?>()).Returns(Substitute.For<IDisposable>());
            currentUowProvider.Current.Returns(currentUnitOfWork);

            emailTemplateProvider.GetDefaultTemplate(Arg.Any<int?>())
                .Returns("<html>{EMAIL_TITLE}-{EMAIL_SUB_TITLE}-{EMAIL_BODY}</html>");

            emailSender.SendAsync(Arg.Any<MailMessage>()).Returns(Task.CompletedTask);

            var user = new User
            {
                PasswordResetCode = "reset",
                TenantId = 1,
                Name = "Nome",
                Surname = "Sobrenome",
                UserName = "usuario",
                EmailAddress = "usuario@example.com",
                AuthenticationSource = "Google"
            };

            // Quando
            await sut.SendPasswordResetLinkAsync(user, "http://link?resetCode={resetCode}&userId={userId}&tenantId={tenantId}&authenticationSource={authenticationSource}");

            // Então
            await emailSender.Received(1).SendAsync(Arg.Is<MailMessage>(m =>
                m.Subject == "PasswordResetEmail_Subject" &&
                m.Body.Contains("c=")));
        }
    }
}
