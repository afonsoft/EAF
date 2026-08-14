namespace Eaf.MailKit.Configuration
{
    /// <summary>
    /// Nomes das configurações (settings) do módulo EAF MailKit.
    /// </summary>
    public static class EafMailKitSettingNames
    {
        /// <summary>
        /// Quantidade máxima de tentativas de envio em caso de falha transitória.
        /// </summary>
        public const string RetryCount = "Eaf.MailKit.RetryCount";

        /// <summary>
        /// Tempo base entre tentativas, em milissegundos.
        /// </summary>
        public const string RetryDelayMilliseconds = "Eaf.MailKit.RetryDelayMilliseconds";

        /// <summary>
        /// Desabilita a validação do certificado do servidor SMTP.
        /// </summary>
        public const string DisableCertificateValidation = "Eaf.MailKit.DisableCertificateValidation";
    }
}
