namespace Eaf.Webhooks
{
    /// <summary>
    /// Protege e recupera segredos de assinaturas de webhook.
    /// </summary>
    public interface IWebhookSubscriptionSecretProtector
    {
        /// <summary>
        /// Criptografa o segredo em texto plano.
        /// </summary>
        string Protect(string plainText);

        /// <summary>
        /// Descriptografa o segredo previamente protegido.
        /// </summary>
        string Unprotect(string cipherText);
    }
}
