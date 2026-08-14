using System;

namespace Eaf.MailKit.Configuration
{
    /// <summary>
    /// Configurações do módulo EAF para MailKit.
    /// </summary>
    [Serializable]
    public class EafMailKitConfiguration
    {
        /// <summary>
        /// Quantidade máxima de tentativas de envio em caso de falha transitória.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Tempo base entre tentativas, em milissegundos (backoff exponencial).
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 500;

        /// <summary>
        /// Desabilita a validação do certificado do servidor SMTP.
        /// Deve ser usado apenas em ambiente de desenvolvimento/teste.
        /// </summary>
        public bool DisableCertificateValidation { get; set; } = false;
    }
}
