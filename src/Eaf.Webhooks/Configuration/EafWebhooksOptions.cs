using System.Text.Json;

namespace Eaf.Webhooks.Configuration
{
    /// <summary>
    /// Opções de configuração do módulo Eaf.Webhooks.
    /// </summary>
    public class EafWebhooksOptions
    {
        /// <summary>
        /// Permite URLs HTTP não seguras. O padrão é false.
        /// </summary>
        public bool AllowHttp { get; set; }

        /// <summary>
        /// Timeout das requisições HTTP em segundos.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Número máximo de tentativas de envio (incluindo a primeira).
        /// </summary>
        public int MaxSendAttemptCount { get; set; } = 5;

        /// <summary>
        /// Desativa automaticamente assinaturas que falham consecutivamente.
        /// </summary>
        public bool IsAutomaticSubscriptionDeactivationEnabled { get; set; } = true;

        /// <summary>
        /// Número máximo de falhas consecutivas antes de desativar a assinatura.
        /// </summary>
        public int MaxConsecutiveFailCountBeforeDeactivateSubscription { get; set; } = 10;

        /// <summary>
        /// Nome do header de assinatura HMAC.
        /// </summary>
        public string SignatureHeaderName { get; set; } = "X-Eaf-Signature-256";

        /// <summary>
        /// Template do valor do header de assinatura.
        /// </summary>
        public string SignatureValueTemplate { get; set; } = "sha256={0}";

        /// <summary>
        /// Propósito usado pelo ASP.NET Core Data Protection para criptografar segredos.
        /// </summary>
        public string DataProtectionPurpose { get; set; } = "eaf-webhooks-subscription-secret";

        /// <summary>
        /// Opções de serialização JSON usadas no payload do webhook.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
