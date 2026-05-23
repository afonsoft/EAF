using Abp.RealTime;

namespace Eaf.Middleware.RealTime
{
    /// <summary>
    /// Representa a classe OnlineClientManager.
    /// </summary>
    public class OnlineClientManager<T> : OnlineClientManager, IOnlineClientManager<T>
    {
        /// <summary>
        /// OnlineClientManager.
        /// </summary>
        /// <param name="store">Parâmetro store.</param>
        /// <returns>Resultado da operação.</returns>
        public OnlineClientManager(IOnlineClientStore<T> store) : base(store)
        {
        }
    }
}