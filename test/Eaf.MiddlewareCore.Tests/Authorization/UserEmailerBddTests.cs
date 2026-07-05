using System.Threading.Tasks;
using Abp;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Net.Mail;
using Eaf.Middleware.Authorization.Users;
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
    }
}
