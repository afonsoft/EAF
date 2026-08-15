using Abp.RealTime;

namespace Eaf.SignalR.RealTime
{
    /// <summary>
    /// Representa a classe EafInMemoryOnlineClientStore.
    /// </summary>
    public class EafInMemoryOnlineClientStore : InMemoryOnlineClientStore
    {
        /// <summary>
        /// Cria uma nova instância do armazenamento em memória de clientes online.
        /// </summary>
        public EafInMemoryOnlineClientStore()
        {
        }
    }

    /// <summary>
    /// Representa a classe EafInMemoryOnlineClientStore.
    /// </summary>
    /// <typeparam name="T">Tipo do escopo de clientes.</typeparam>
    public class EafInMemoryOnlineClientStore<T> : EafInMemoryOnlineClientStore, IOnlineClientStore<T>
    {
    }
}
