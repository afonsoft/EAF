using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MailKit;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Net.Mail.Smtp;
using Abp.Reflection.Extensions;
using Abp.Timing;
using Abp.Zero;
using Abp.Zero.Configuration;
using Castle.MicroKernel.Registration;
using Eaf.Middleware.Authorization.AzureActiveDirectory;
using Eaf.Middleware.Authorization.Ldap;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.AzureActiveDirectory;
using Eaf.Middleware.AzureActiveDirectory.Configuration;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Core.DynamicEntityProperties;
using Eaf.Middleware.Friendships.Cache;
using Eaf.Middleware.Ldap;
using Eaf.Middleware.Ldap.Configuration;
using Eaf.Middleware.Localization;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.Net.Emailing;
using Eaf.Middleware.Timing;
using MailKit.Security;
using System;
using System.Transactions;

namespace Eaf.Middleware
{
    [DependsOn(
        typeof(AbpZeroCoreModule),
        typeof(EafMiddlewareAzureActiveDirectoryModule),
        typeof(EafMiddlewareLdapModule),
        typeof(AbpAutoMapperModule),
        typeof(AbpMailKitModule))]
    public class MiddlewareCoreModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareCoreModule).GetAssembly());
        }

        /// <summary>
        /// PostInitialize.
        /// </summary>
        public override void PostInitialize()
        {
            IocManager.Resolve<AppTimes>().StartupTime = Clock.Now;
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            AppContext.SetSwitch("Microsoft.EntityFrameworkCore.Issue9825", true);

            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            //Declare entity types
            Configuration.Modules.Zero().EntityTypes.Tenant = typeof(Tenant);
            Configuration.Modules.Zero().EntityTypes.Role = typeof(Role);
            Configuration.Modules.Zero().EntityTypes.User = typeof(User);

            MiddlewareLocalizationConfigurer.Configure(Configuration.Localization);

            //Adding setting providers
            Configuration.Settings.Providers.Add<AppSettingProvider>();

            //Enable Azure Active Directory authentication
            Configuration.Modules.MiddlewareAzureActiveDirectory().Enable(typeof(AppAzureActiveDirectoryAuthenticationSource));

            //Enable LDAP authentication
            Configuration.Modules.MiddlewareLdap().Enable(typeof(AppLdapAuthenticationSource));

            //Adding DynamicEntityParameters definition providers
            Configuration.DynamicEntityProperties.Providers.Add<AppDynamicEntityPropertyDefinitionProvider>();

            // MailKit configuration
            Configuration.Modules.AbpMailKit().SecureSocketOption = SecureSocketOptions.Auto;
            Configuration.ReplaceService<IMailKitSmtpBuilder, MiddlewareMailKitSmtpBuilder>(DependencyLifeStyle.Transient);

            //Configure roles
            AppRoleConfig.Configure(Configuration.Modules.Zero().RoleManagement);

            Configuration.ReplaceService(typeof(IEmailSenderConfiguration), () =>
            {
                Configuration.IocManager.IocContainer.Register(
                    Component.For<IEmailSenderConfiguration, ISmtpEmailSenderConfiguration>()
                             .ImplementedBy<MiddlewareSmtpEmailSenderConfiguration>()
                             .LifestyleTransient()
                );
            });

            Configuration.Caching.Configure(FriendCacheItem.CacheName, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(30);
            });
        }
    }
}