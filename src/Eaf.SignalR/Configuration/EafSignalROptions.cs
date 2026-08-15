namespace Eaf.SignalR.Configuration
{
    /// <summary>
    /// Opções de configuração do Eaf.SignalR.
    /// </summary>
    public class EafSignalROptions
    {
        /// <summary>
        /// Habilita erros detalhados. Quando null, utiliza o ambiente (true em Development).
        /// </summary>
        public bool? UseDetailedErrors { get; set; }

        /// <summary>
        /// Tempo limite para handshake em segundos.
        /// </summary>
        public int HandshakeTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Intervalo de keep-alive em segundos.
        /// </summary>
        public int KeepAliveIntervalSeconds { get; set; } = 30;

        /// <summary>
        /// Tempo limite de timeout do cliente em segundos.
        /// </summary>
        public int ClientTimeoutIntervalSeconds { get; set; } = 60;

        /// <summary>
        /// Habilita o Redis backplane para scale-out.
        /// </summary>
        public bool UseRedisBackplane { get; set; }

        /// <summary>
        /// Connection string do Redis. Quando vazio, tenta utilizar RedisCache:ConnectionString.
        /// </summary>
        public string RedisConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Número do banco Redis (opcional).
        /// </summary>
        public int? RedisDatabase { get; set; }
    }
}
