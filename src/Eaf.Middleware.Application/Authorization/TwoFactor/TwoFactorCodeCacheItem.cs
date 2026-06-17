using System;

namespace Eaf.Middleware.Authorization.TwoFactor
{
    /// <summary>
    /// Item de cache para armazenamento de TwoFactorCode.
    /// </summary>
    [Serializable]
    public class TwoFactorCodeCacheItem
    {
        public const string CacheName = "AppTwoFactorCodeCache";

        /// <summary>
        /// FromHours.
        /// </summary>
        public static readonly TimeSpan DefaultSlidingExpireTime = TimeSpan.FromHours(1);

        /// <summary>
        /// TwoFactorCodeCacheItem.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public TwoFactorCodeCacheItem()
        {
        }

        /// <summary>
        /// TwoFactorCodeCacheItem.
        /// </summary>
        /// <param name="code">Parâmetro code.</param>
        /// <returns>Resultado da operação.</returns>
        public TwoFactorCodeCacheItem(string code)
        {
            Code = code;
        }

        /// <summary>
        /// Obtém ou define Code.
        /// </summary>
        public string Code { get; set; }
    }
}