using Abp.Dependency;
using Castle.Core.Logging;

namespace Eaf.KeyVault
{
    /// <summary>
    /// Implementação da factory de KeyVaultManager.
    /// Responsável por criar a instância correta baseada no provider configurado.
    /// </summary>
    public class KeyVaultManagerFactory : IKeyVaultManagerFactory, ITransientDependency
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Inicializa uma nova instância de KeyVaultManagerFactory.
        /// </summary>
        /// <param name="logger">Logger para registrar operações.</param>
        public KeyVaultManagerFactory(ILogger logger)
        {
            _logger = logger ?? NullLogger.Instance;
        }

        /// <summary>
        /// Cria uma instância de IKeyVaultManager baseada nas opções fornecidas.
        /// </summary>
        /// <param name="options">Opções de configuração do KeyVault.</param>
        /// <returns>Instância de IKeyVaultManager.</returns>
        public IKeyVaultManager Create(EafKeyVaultOptions options)
        {
            return options?.Provider switch
            {
                EnumKeyVault.Azure => new AzureKeyVaultManager(options, _logger),
                EnumKeyVault.OCI => new OCIKeyVaultManager(options, _logger),
                _ => new NullKeyVaultManager(options, _logger)
            };
        }
    }
}
