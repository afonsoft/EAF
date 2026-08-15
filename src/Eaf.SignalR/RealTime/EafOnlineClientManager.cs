using Abp.RealTime;

namespace Eaf.SignalR.RealTime
{
    /// <summary>
    /// Representa a classe EafOnlineClientManager.
    /// </summary>
    public class EafOnlineClientManager : OnlineClientManager
    {
        /// <summary>
        /// EafOnlineClientManager.
        /// </summary>
        /// <param name="store">Parâmetro store.</param>
        public EafOnlineClientManager(IOnlineClientStore store) : base(store)
        {
        }
    }

    /// <summary>
    /// Representa a classe EafOnlineClientManager.
    /// </summary>
    /// <typeparam name="T">Tipo do escopo de clientes.</typeparam>
    public class EafOnlineClientManager<T> : EafOnlineClientManager, IOnlineClientManager<T>
    {
        /// <summary>
        /// EafOnlineClientManager.
        /// </summary>
        /// <param name="store">Parâmetro store.</param>
        public EafOnlineClientManager(IOnlineClientStore<T> store) : base(store)
        {
        }
    }
}
