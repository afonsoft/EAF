using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MailKit;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Reflection.Extensions;
using Eaf.ProjectName.Debugging;
using Eaf.ProjectName.Localization;
using System;

namespace Eaf.ProjectName
{
    [DependsOn(
        typeof(AbpAutoMapperModule),
        typeof(AbpMailKitModule)
        )]
    public class ProjectNameCoreModule : AbpModule
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
            }
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ProjectNameCoreModule).GetAssembly());
        }
    }
}