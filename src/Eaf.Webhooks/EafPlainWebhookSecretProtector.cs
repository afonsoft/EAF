namespace Eaf.Webhooks
{
    /// <summary>
    /// Protetor de segredos sem criptografia (fallback para testes ou ambientes sem Data Protection).
    /// </summary>
    internal class EafPlainWebhookSecretProtector : IWebhookSubscriptionSecretProtector
    {
        public string Protect(string plainText) => plainText;

        public string Unprotect(string cipherText) => cipherText;
    }
}
