using Abp.Authorization;
using Abp.Localization;
using Eaf.Middleware.Authorization;
using Abp.Notifications;
using Abp;

namespace Eaf.Middleware.Web.Notifications
{
    /// <summary>
    /// Representa a classe MiddlewareNotificationProvider.
    /// </summary>
    public class MiddlewareNotificationProvider : NotificationProvider
    {
        /// <summary>
        /// SetNotifications.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        public override void SetNotifications(INotificationDefinitionContext context)
        {
            context.Manager.Add(
                new NotificationDefinition(
                    MiddlewareNotificationNames.NewUserRegistered,
                    displayName: L("NewUserRegisteredNotificationDefinition"),
                    permissionDependency: new SimplePermissionDependency(MiddlewarePermissions.Pages_Administration_Users)
                    )
                );

            context.Manager.Add(
               new NotificationDefinition(
                   MiddlewareNotificationNames.WelcomeToTheApplication,
                   displayName: L("WelcomeToTheApplication"),
                   permissionDependency: new SimplePermissionDependency(MiddlewarePermissions.Pages)
                   )
               );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpConsts.LocalizationSourceName);
        }
    }
}