using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.RealTime;
using Abp.Reflection.Extensions;
using Eaf.Middleware.Authorization;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Configuration;
using Eaf.Middleware.Friendships;
using System;

namespace Eaf.Middleware
{
    /// <summary>
    /// Módulo ABP que configura e inicializa MiddlewareApplication.
    /// </summary>
    [DependsOn(
        typeof(MiddlewareCoreModule)
        )]
    public class MiddlewareApplicationModule : AbpModule
    {
        /// <summary>
        /// Initialize.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(MiddlewareApplicationModule).GetAssembly());
        }

        /// <summary>
        /// PostInitialize.
        /// </summary>
        public override void PostInitialize()
        {
            IocManager.RegisterIfNot<IChatCommunicator, NullChatCommunicator>();
            IocManager.RegisterIfNot<IAuditingStore, AuditingStore>(DependencyLifeStyle.Singleton);
        }

        /// <summary>
        /// PreInitialize.
        /// </summary>
        public override void PreInitialize()
        {
            //Adding authorization providers
            Configuration.Authorization.Providers.Add<MiddlewareAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(MiddlewareCustomDtoMapper.CreateMappings);

            //Adding a Google Analytics and TagManager
            Configuration.Settings.Providers.Add<GoogleProvider>();

            //https://learn.microsoft.com/pt-br/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only
            AppContext.SetSwitch("System.Drawing.EnableUnixSupport", true);
        }
    }
}