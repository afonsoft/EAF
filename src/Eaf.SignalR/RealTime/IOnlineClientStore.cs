using Abp.RealTime;

namespace Eaf.SignalR.RealTime
{
    /// <summary>
    /// Representa o armazenamento de clientes online tipado por escopo.
    /// </summary>
    /// <typeparam name="T">Tipo do escopo de clientes.</typeparam>
    public interface IOnlineClientStore<T> : IOnlineClientStore // NOSONAR S2326: tipo genérico T é marcador de escopo para DI.
    {
    }
}
