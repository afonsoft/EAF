namespace Eaf.Middleware.Contracts
{
    /// <summary>
    /// Public error contract returned by EAF APIs to SDK clients.
    /// </summary>
    public class PublicErrorContract
    {
        /// <summary>
        /// Stable machine-readable error code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates whether the client may retry the same request.
        /// </summary>
        public bool Retryable { get; set; }

        /// <summary>
        /// Correlation identifier for distributed tracing.
        /// </summary>
        public string CorrelationId { get; set; }
    }
}
