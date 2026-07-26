namespace Eaf.ProjectName.Contracts
{
    /// <summary>
    /// Envelope de erro seguro para APIs consumidoras.
    /// </summary>
    public sealed class PublicErrorContract
    {
        /// <summary>
        /// Obtém ou define o código estável do erro.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Obtém ou define a mensagem localizada para a interface.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Obtém ou define um valor que indica se a operação pode ser repetida.
        /// </summary>
        public bool Retryable { get; set; }

        /// <summary>
        /// Obtém ou define o identificador de correlação.
        /// </summary>
        public string CorrelationId { get; set; }
    }
}
