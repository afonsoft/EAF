using Eaf.KeyVault;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Eaf.Hosting.Configuration
{
    /// <summary>
    /// Representa a classe EafKeyVaultConfigurationProvider.
    /// </summary>
    public class EafKeyVaultConfigurationProvider : ConfigurationProvider
    {
        private readonly EafKeyVaultOptions options;

        /// <summary>
        /// EafKeyVaultConfigurationProvider.
        /// </summary>
        /// <param name="options">Parâmetro options.</param>
        /// <returns>Resultado da operação.</returns>
        public EafKeyVaultConfigurationProvider(EafKeyVaultOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Load.
        /// </summary>
        public override void Load()
        {
            //Metodo para ler todos as chaves do Key Vault e jogar nas IConfiguracion
            if (options.Provider == EnumKeyVault.None)
                return;
            try
            {
                var manager = new KeyVaultSecretManager(options);
                Data = manager.GetKeyValues();
            }
            catch
            {
                //Ignorar os erros aqui para o APP subir sem erros.
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}