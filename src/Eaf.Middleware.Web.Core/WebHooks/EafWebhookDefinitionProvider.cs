using Abp;
using Abp.Localization;
using Abp.Webhooks;
using Eaf.Middleware;

namespace Eaf.Middleware.Web.WebHooks
{
    /// <summary>
    /// Representa a classe EafWebhookDefinitionProvider.
    /// </summary>
    public class EafWebhookDefinitionProvider : WebhookDefinitionProvider
    {
        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, "EafCore");
        }

        /// <summary>
        /// SetWebhooks.
        /// </summary>
        /// <param name="context">Parâmetro context.</param>
        public override void SetWebhooks(IWebhookDefinitionContext context)
        {
            context.Manager.Add(new WebhookDefinition(
                name: EafWebHookNames.NewUserRegistered,
                displayName: L("NewUserRegisteredNotificationDefinition")
            ));
        }
    }
}