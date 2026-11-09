using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.EntityHistory;
using Abp.Localization;
using Abp.Notifications;
using Abp.Organizations;
using Abp.UI.Inputs;
using Eaf.Middleware.Auditing.Dto;
using Eaf.Middleware.Authorization.Accounts.Dto;
using Eaf.Middleware.Authorization.Permissions.Dto;
using Eaf.Middleware.Authorization.Roles;
using Eaf.Middleware.Authorization.Roles.Dto;
using Eaf.Middleware.Authorization.Users;
using Eaf.Middleware.Authorization.Users.Dto;
using Eaf.Middleware.Authorization.Users.Profile.Dto;
using Eaf.Middleware.Chat;
using Eaf.Middleware.Chat.Dto;
using Eaf.Middleware.Core.Editions;
using Eaf.Middleware.Dashboard.Dto;
using Eaf.Middleware.Editions.Dto;
using Eaf.Middleware.Friendships;
using Eaf.Middleware.Friendships.Dto;
using Eaf.Middleware.Localization.Dto;
using Eaf.Middleware.MassNotifications;
using Eaf.Middleware.MassNotifications.Dto;
using Eaf.Middleware.MultiTenancy;
using Eaf.Middleware.MultiTenancy.Dto;
using Eaf.Middleware.Notifications.Dto;
using Eaf.Middleware.OrganizationUnits.Dto;
using Eaf.Middleware.Payments;
using Eaf.Middleware.Payments.Dto;
using Eaf.Middleware.Sessions.Dto;
using Eaf.Middleware.UserDelegations;
using Eaf.Middleware.UserDelegations.Dto;
using Riok.Mapperly.Abstractions;
using System.Collections.Generic;

namespace Eaf.Middleware.Mapping
{
    /// <summary>
    /// Source-generated Mapperly mapper for Middleware.Application DTOs.
    /// Pairs with custom AutoMapper configuration (ForMember/Ignore/Include) stay
    /// in <see cref="MiddlewareCustomDtoMapper"/> until they get an equivalent.
    /// </summary>
    // Lenient strategy matches the AutoMapper semantics this mapper replaces:
    // members without a counterpart keep their default value instead of warning.
    [Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public partial class MiddlewareApplicationMapperly
    {
        // Inputs
        public partial FeatureInputTypeDto ToDto(CheckboxInputType source);
        public partial FeatureInputTypeDto ToDto(SingleLineStringInputType source);
        public partial FeatureInputTypeDto ToDto(ComboboxInputType source);

        // Feature

        // Role
        public partial Role ToEntity(RoleEditDto source);
        public partial RoleEditDto ToEditDto(Role source);
        public partial RoleListDto ToListDto(Role source);
        public partial UserListRoleDto ToListDto(UserRole source);

        // Permission
        public partial FlatPermissionDto ToFlatDto(Permission source);
        public partial FlatPermissionWithLevelDto ToFlatWithLevelDto(Permission source);

        // Language
        public partial ApplicationLanguageListDto ToListDto(ApplicationLanguage source);
        public partial NotificationSubscriptionWithDisplayNameDto ToDto(NotificationDefinition source);

        // Tenant
        public partial TenantLoginInfoDto ToLoginInfoDto(Tenant source);
        public partial TenantListDto ToListDto(Tenant source);
        public partial Tenant ToEntity(TenantEditDto source);
        public partial TenantEditDto ToEditDto(Tenant source);
        public partial Tenant ToEntity(CurrentTenantInfoDto source);
        public partial CurrentTenantInfoDto ToCurrentDto(Tenant source);
        public partial AvailableTenantDto ToAvailableDto(Tenant source);

        // Edition
        public partial EditionDto ToEditionDto(SubscribableEdition source);
        public partial SubscribableEdition ToEntity(CreateEditionInput source);
        public partial SubscribableEdition ToEntity(UpdateEditionInput source);

        // OrganizationUnit
        public partial OrganizationUnitDto ToDto(OrganizationUnit source);
        public partial OrganizationUnitUserListDto ToDto(User source);

        // MassNotification
        public partial MassNotificationDto ToDto(MassNotification source);
        public partial MassNotification ToEntity(CreateMassNotificationInput source);

        // UserDelegation
        public partial UserDelegationDto ToDto(UserDelegation source);

        // Payments
        public partial SubscriptionPaymentDto ToDto(SubscriptionPayment source);
        public partial SubscriptionPaymentProductDto ToDto(SubscriptionPaymentProduct source);
        public partial SubscriptionPaymentProduct ToEntity(SubscriptionPaymentProductInput source);

        // User (Password-sensitive pairs stay in the AutoMapper profile)
        public partial UserLoginInfoDto ToLoginInfoDto(User source);
        public partial UserListDto ToListDto(User source);
        public partial Abp.Authorization.Users.UserLoginAttempt ToEntity(UserLoginAttemptDto source);
        public partial UserLoginAttemptDto ToDto(UserLoginAttempt source);

        // AuditLog
        public partial AuditLogListDto ToDto(AuditLog source);
        public partial EntityChangeListDto ToDto(EntityChange source);

        // Chat
        public partial FriendshipDto ToDto(Friendship source);
        public partial ChatMessageDto ToDto(ChatMessage source);

        // TenantJoinRequest
        public partial TenantJoinRequestDto ToDto(TenantJoinRequest source);

        // List mappings
        public partial List<FeatureInputTypeDto> ToDtoList(List<CheckboxInputType> source);
        public partial List<FlatFeatureDto> ToFlatDtoList(List<Feature> source);
        public partial List<RoleListDto> ToListDtoList(List<Role> source);
        public partial List<TenantListDto> ToListDtoList(List<Tenant> source);
        public partial List<EditionDto> ToEditionDtoList(List<SubscribableEdition> source);
        public partial List<OrganizationUnitDto> ToDtoList(List<OrganizationUnit> source);
        public partial List<MassNotificationDto> ToDtoList(List<MassNotification> source);
        public partial List<UserDelegationDto> ToDtoList(List<UserDelegation> source);
        public partial List<SubscriptionPaymentDto> ToDtoList(List<SubscriptionPayment> source);
        public partial List<UserListDto> ToListDtoList(List<User> source);
        public partial List<AuditLogListDto> ToDtoList(List<AuditLog> source);
        public partial List<FriendshipDto> ToDtoList(List<Friendship> source);
        public partial List<ChatMessageDto> ToDtoList(List<ChatMessage> source);
        public partial List<TenantJoinRequestDto> ToDtoList(List<TenantJoinRequest> source);
    }
}
