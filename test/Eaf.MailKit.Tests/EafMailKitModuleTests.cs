using Abp;
using Abp.Dependency;
using Abp.MailKit;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Reflection.Extensions;
using Eaf.MailKit.Configuration;
using Eaf.MailKit.Emailing;
using Shouldly;
using Xunit;

namespace Eaf.MailKit.Tests
{
    public class EafMailKitModuleTests
    {
        [Fact]
        public void Dado_EafMailKitModule_Quando_Inicializar_Entao_Servicos_Sao_Registrados()
        {
            using var bootstrapper = CriarBootstrapperIsolado();
            bootstrapper.Initialize();

            var container = bootstrapper.IocManager;
            container.Resolve<EafMailKitConfiguration>().ShouldNotBeNull();
            container.Resolve<IEmailTemplateManager>().ShouldNotBeNull();
            container.Resolve<IEmailTemplateStore>().ShouldNotBeNull();
        }

        [Fact]
        public void Dado_EafMailKitModule_Quando_Inicializar_Entao_Substitui_EmailSender_E_SmtpBuilder()
        {
            using var bootstrapper = CriarBootstrapperIsolado();
            bootstrapper.Initialize();

            var emailSender = bootstrapper.IocManager.Resolve<IEmailSender>();
            var smtpBuilder = bootstrapper.IocManager.Resolve<IMailKitSmtpBuilder>();

            emailSender.ShouldBeAssignableTo<EafMailKitEmailSender>();
            smtpBuilder.ShouldBeAssignableTo<EafMailKitSmtpBuilder>();
        }

        private static AbpBootstrapper CriarBootstrapperIsolado()
        {
            return AbpBootstrapper.Create<EafMailKitTestModule>(options =>
            {
                options.IocManager = new IocManager();
            });
        }

        [DependsOn(typeof(EafMailKitModule))]
        public class EafMailKitTestModule : AbpModule
        {
            public override void Initialize()
            {
                IocManager.RegisterAssemblyByConvention(typeof(EafMailKitTestModule).GetAssembly());
            }
        }
    }
}
