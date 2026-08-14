using Abp;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MailKit;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using Eaf.MailKit.Configuration;
using Eaf.MailKit.Emailing;
using MailKit.Security;

namespace Eaf.MailKit
{
    /// <summary>
    /// Módulo EAF para envio de e-mails via MailKit.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule), typeof(AbpMailKitModule))]
    public class EafMailKitModule : AbpModule
    {
        /// <summary>
        /// Pré-inicializa o módulo registrando configurações e substituindo serviços do ABP.
        /// </summary>
        public override void PreInitialize()
        {
            if (!IocManager.IsRegistered<EafMailKitConfiguration>())
            {
                IocManager.IocContainer.Register(
                    Component.For<EafMailKitConfiguration>()
                             .UsingFactoryMethod(() => new EafMailKitConfiguration())
                             .LifestyleSingleton()
                );
            }

            Configuration.Settings.Providers.Add<EafMailKitSettingProvider>();

            Configuration.Modules.AbpMailKit().SecureSocketOption = SecureSocketOptions.Auto;

            Configuration.ReplaceService<IMailKitSmtpBuilder, EafMailKitSmtpBuilder>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IEmailSender, EafMailKitEmailSender>(DependencyLifeStyle.Transient);
        }

        /// <summary>
        /// Inicializa o módulo registrando componentes por convenção.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(EafMailKitModule).GetAssembly());
        }
    }
}
