using Castle.Core.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eaf.KeyVault
{
    internal class NullKeyVaultManager : IKeyVaultManager
    {
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
            logger.Debug("NullKeyVaultManager : NotImplementedException");
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// GetKeyValuesAsync.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public Task<IDictionary<string, string>> GetKeyValuesAsync()
        {
            logger.Debug("NullKeyVaultManager : NotImplementedException");
            return Task.FromResult(GetKeyValues());
        }

        /// <summary>
        /// GetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <returns>Resultado da operação.</returns>
        public string GetValue(string key)
        {
            logger.Debug("NullKeyVaultManager : NotImplementedException");
            return null;
        }

        /// <summary>
        /// GetValueAsync.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <returns>Resultado da operação.</returns>
        public Task<string> GetValueAsync(string key)
        {
            logger.Debug("NullKeyVaultManager : NotImplementedException");
            return Task.FromResult(GetValue(key));
        }

        /// <summary>
        /// SetValue.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        public void SetValue(string key, string value)
        {
            logger.Debug("NullKeyVaultManager : NotImplementedException");
            //null
        }

        /// <summary>
        /// SetValueAsync.
        /// </summary>
        /// <param name="key">Parâmetro key.</param>
        /// <param name="value">Parâmetro value.</param>
        public Task SetValueAsync(string key, string value)
        {
            logger.Debug("NullKeyVaultManager : NotImplementedException");
            SetValue(key, value);
            return Task.CompletedTask;
        }
    }
}