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

            // AutoMapper→Mapperly transition: register source-generated pairs with
            // the shared adapter in Eaf.Middleware.Core. Pairs with custom
            // AutoMapper configuration (ForMember/Ignore/Include) still fall back.
            var generated = new Eaf.Middleware.Mapping.MiddlewareApplicationMapperly();
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.UI.Inputs.CheckboxInputType, Editions.Dto.FeatureInputTypeDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.UI.Inputs.SingleLineStringInputType, Editions.Dto.FeatureInputTypeDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.UI.Inputs.ComboboxInputType, Editions.Dto.FeatureInputTypeDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Roles.Dto.RoleEditDto, Authorization.Roles.Role>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Roles.Role, Authorization.Roles.Dto.RoleEditDto>(generated.ToEditDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Roles.Role, Authorization.Roles.Dto.RoleListDto>(generated.ToListDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.Authorization.Users.UserRole, Authorization.Users.Dto.UserListRoleDto>(generated.ToListDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.Authorization.Permission, Authorization.Permissions.Dto.FlatPermissionWithLevelDto>(generated.ToFlatWithLevelDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.Localization.ApplicationLanguage, Localization.Dto.ApplicationLanguageListDto>(generated.ToListDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.Notifications.NotificationDefinition, Notifications.Dto.NotificationSubscriptionWithDisplayNameDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Eaf.Middleware.MultiTenancy.Tenant, Sessions.Dto.TenantLoginInfoDto>(generated.ToLoginInfoDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Eaf.Middleware.MultiTenancy.Tenant, MultiTenancy.Dto.TenantListDto>(generated.ToListDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<MultiTenancy.Dto.TenantEditDto, Eaf.Middleware.MultiTenancy.Tenant>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Eaf.Middleware.MultiTenancy.Tenant, MultiTenancy.Dto.TenantEditDto>(generated.ToEditDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Accounts.Dto.CurrentTenantInfoDto, Eaf.Middleware.MultiTenancy.Tenant>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Eaf.Middleware.MultiTenancy.Tenant, Authorization.Accounts.Dto.CurrentTenantInfoDto>(generated.ToCurrentDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Eaf.Middleware.MultiTenancy.Tenant, Authorization.Accounts.Dto.AvailableTenantDto>(generated.ToAvailableDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Core.Editions.SubscribableEdition, Editions.Dto.EditionDto>(generated.ToEditionDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Editions.Dto.CreateEditionInput, Core.Editions.SubscribableEdition>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Editions.Dto.UpdateEditionInput, Core.Editions.SubscribableEdition>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.Organizations.OrganizationUnit, OrganizationUnits.Dto.OrganizationUnitDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Users.User, OrganizationUnits.Dto.OrganizationUnitUserListDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<MassNotifications.MassNotification, MassNotifications.Dto.MassNotificationDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<MassNotifications.Dto.CreateMassNotificationInput, MassNotifications.MassNotification>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<UserDelegations.UserDelegation, UserDelegations.Dto.UserDelegationDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Payments.SubscriptionPayment, Payments.Dto.SubscriptionPaymentDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Payments.SubscriptionPaymentProduct, Payments.Dto.SubscriptionPaymentProductDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Payments.Dto.SubscriptionPaymentProductInput, Payments.SubscriptionPaymentProduct>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Users.User, Sessions.Dto.UserLoginInfoDto>(generated.ToLoginInfoDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Users.User, Authorization.Users.Dto.UserListDto>(generated.ToListDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Authorization.Users.Dto.UserLoginAttemptDto, Abp.Authorization.Users.UserLoginAttempt>(generated.ToEntity);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.Authorization.Users.UserLoginAttempt, Authorization.Users.Dto.UserLoginAttemptDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.Auditing.AuditLog, Auditing.Dto.AuditLogListDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<Abp.EntityHistory.EntityChange, Auditing.Dto.EntityChangeListDto>(generated.ToDto);
            Eaf.Middleware.Mapping.EafMapperlyObjectMapper.Register<MultiTenancy.TenantJoinRequest, Authorization.Accounts.Dto.TenantJoinRequestDto>(generated.ToDto);
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