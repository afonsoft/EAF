using Abp.RealTime;

namespace Eaf.Middleware.RealTime
{
    /// <summary>
    /// Representa a classe InMemoryOnlineClientStore.
    /// </summary>
    public class InMemoryOnlineClientStore<T> : InMemoryOnlineClientStore, IOnlineClientStore<T>
    {
    }
}