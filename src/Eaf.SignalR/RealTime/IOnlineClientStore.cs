using Abp.RealTime;

namespace Eaf.SignalR.RealTime
{
    /// <summary>
    /// Representa o armazenamento de clientes online tipado por escopo.
    /// </summary>
    /// <typeparam name="T">Tipo do escopo de clientes.</typeparam>
    public interface IOnlineClientStore<T> : IOnlineClientStore
    {
    }
}
