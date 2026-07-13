using Castle.Core.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.KeyVault
{
    internal class NullKeyVaultManager : IKeyVaultManager
    {
        private const string NotImplementedLogMessage = "NullKeyVaultManager : NotImplementedException";

        private readonly EafKeyVaultOptions options;
        private readonly ILogger logger;

        /// <summary>
        /// NullKeyVaultManager.
        /// </summary>
        /// <param name="options">Parâmetro options.</param>
        /// <param name="logger">Parâmetro logger.</param>
        /// <returns>Resultado da operação.</returns>
        public NullKeyVaultManager(EafKeyVaultOptions options, ILogger logger)
        {
            this.options = options;
            this.logger = logger;
        }

        /// <summary>
        /// GetKeyValues.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public IDictionary<string, string> GetKeyValues()
        {
            logger.Debug(NotImplementedLogMessage);
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// GetKeyValuesAsync.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public Task<IDictionary<string, string>> GetKeyValuesAsync()
        {
            logger.Debug(NotImplementedLogMessage);
            return Task.FromResult(GetKeyValues());
        }

        /// <summary>
        /// GetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <returns>Resultado da operação.</returns>
        public string GetValue(string key)
        {
            logger.Debug(NotImplementedLogMessage);
            return null;
        }

        /// <summary>
        /// GetValueAsync.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<string> GetValueAsync(string key)
        {
            logger.Debug(NotImplementedLogMessage);
            return Task.FromResult(GetValue(key));
        }

        /// <summary>
        /// SetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        public void SetValue(string key, string value)
        {
            logger.Debug(NotImplementedLogMessage);
            //null
        }

        /// <summary>
        /// SetValueAsync.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        public Task SetValueAsync(string key, string value)
        {
            logger.Debug(NotImplementedLogMessage);
            SetValue(key, value);
            return Task.CompletedTask;
        }
    }
}