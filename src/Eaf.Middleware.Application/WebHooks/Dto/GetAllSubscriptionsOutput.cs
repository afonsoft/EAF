using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Webhooks;
using System;
using System.Collections.Generic;

namespace Eaf.Middleware.WebHooks.Dto
{
    /// <summary>
    /// Representa a classe GetAllSubscriptionsOutput.
    /// </summary>
    [AutoMap(typeof(WebhookSubscription))]
    public class GetAllSubscriptionsOutput : EntityDto<Guid>
    {
        /// <summary>
        /// Is subscription active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Subscribed webhook definitions unique names. <see cref="WebhookDefinition.Name"/>
        /// </summary>
        public List<string> Webhooks { get; set; }

        /// <summary>
        /// Subscription webhook endpoint
        /// </summary>
        public string WebhookUri { get; set; }
    }
}