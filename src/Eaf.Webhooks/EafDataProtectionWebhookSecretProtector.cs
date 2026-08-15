using System;
using Eaf.Webhooks.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace Eaf.Webhooks
{
    /// <summary>
    /// Protetor de segredos usando ASP.NET Core Data Protection.
    /// </summary>
    internal class EafDataProtectionWebhookSecretProtector : IWebhookSubscriptionSecretProtector
    {
        private readonly IDataProtector _protector;

        public EafDataProtectionWebhookSecretProtector(IDataProtectionProvider dataProtectionProvider, IOptions<EafWebhooksOptions> optionsAccessor)
        {
            if (dataProtectionProvider == null)
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            if (optionsAccessor?.Value == null)
                throw new ArgumentNullException(nameof(optionsAccessor));

            _protector = dataProtectionProvider.CreateProtector(optionsAccessor.Value.DataProtectionPurpose);
        }

        public string Protect(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            return _protector.Protect(plainText);
        }

        public string Unprotect(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            return _protector.Unprotect(cipherText);
        }
    }
}
