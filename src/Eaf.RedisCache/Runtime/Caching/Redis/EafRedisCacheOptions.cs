using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using System;

namespace Eaf.Runtime.Caching.Redis
{
    /// <summary>
    /// Opções de configuração para o cache Redis do EAF.
    /// Expõe <c>ConnectionString</c> como alias para <see cref="RedisCacheOptions.Configuration"/>
    /// e mantém <c>InstanceName</c> como prefixo das chaves.
    /// </summary>
    public class EafRedisCacheOptions : RedisCacheOptions, IOptions<EafRedisCacheOptions>, IOptions<RedisCacheOptions>
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafRedisCacheOptions"/> com valores padrão.
        /// </summary>
        public EafRedisCacheOptions()
        {
            InstanceName = "EAF";
        }

        /// <summary>
        /// String de conexão do Redis.
        /// </summary>
        public string ConnectionString
        {
            get => Configuration ?? string.Empty;
            set => Configuration = value;
        }

        /// <summary>
        /// Instância do objeto de opções (auto-referência para o padrão <see cref="IOptions{TOptions}"/>).
        /// </summary>
        EafRedisCacheOptions IOptions<EafRedisCacheOptions>.Value => this;

        /// <summary>
        /// Instância do objeto de opções como <see cref="RedisCacheOptions"/>.
        /// </summary>
        RedisCacheOptions IOptions<RedisCacheOptions>.Value => this;
    }
}
