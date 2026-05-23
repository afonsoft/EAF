using Abp.AutoMapper;
using Eaf.Middleware.Core.Authentication.External;
using System.Collections.Generic;

namespace Eaf.Middleware.Web.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public Dictionary<string, string> AdditionalParams { get; set; }
        /// <summary>
        /// Obtém ou define ClientId.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define TenantId.
        /// </summary>
        public string TenantId { get; set; }
    }
}