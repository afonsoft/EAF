using Eaf.AutoMapper;
using Eaf.Configuration.Startup;
using Eaf.Dependency;
using Eaf.MailKit;
using Eaf.Modules;
using Eaf.Net.Mail;
using Eaf.Net.Sms;
using Eaf.ProjectName.Debugging;
using Eaf.ProjectName.Localization;
using Eaf.TextTemplating;
using System;

namespace Eaf.ProjectName
{
    [DependsOn(
        typeof(EafAutoMapperModule),
        typeof(EafMailKitModule),
        typeof(EafTextTemplatingCoreModule)
        )]
    public class ProjectNameCoreModule : EafModule
    {
        public override void PreInitialize()
        {
            //Starting localization settings
            ProjectNameLocalizationConfigurer.Configure(Configuration.Localization);

            //Enable this line to create a multi-tenant application.
            Configuration.MultiTenancy.IsEnabled = ProjectNameConsts.MultiTenancyEnabled;

            if (ProjectNameDebugHelper.IsDebug)
            {
                //Disabling email/sms sending in debug mode
                Configuration.ReplaceService<IEmailSender, NullEmailSender>(DependencyLifeStyle.Transient);
                Configuration.ReplaceService<ISmsSender, NullSmsSender>(DependencyLifeStyle.Transient);
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ProjectNameCoreModule).GetAssembly());
        }
    }
}