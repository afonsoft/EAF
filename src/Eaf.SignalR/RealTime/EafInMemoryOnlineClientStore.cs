using Abp.RealTime;

namespace Eaf.SignalR.RealTime
{
    /// <summary>
    /// Representa a classe EafInMemoryOnlineClientStore.
    /// </summary>
    public class EafInMemoryOnlineClientStore : InMemoryOnlineClientStore
    {
    }

    /// <summary>
    /// Representa a classe EafInMemoryOnlineClientStore.
    /// </summary>
    /// <typeparam name="T">Tipo do escopo de clientes.</typeparam>
    public class EafInMemoryOnlineClientStore<T> : EafInMemoryOnlineClientStore, IOnlineClientStore<T>
    {
    }
}
