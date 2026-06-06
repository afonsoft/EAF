namespace Eaf.Runtime.Caching.Serialization
{
    /// <summary>
    /// Interface para serialização de objetos para cache.
    /// Permite trocar a implementação de serialização sem alterar as classes de cache.
    /// </summary>
    public interface ICacheSerializer
    {
        /// <summary>
        /// Serializa um objeto para array de bytes.
        /// </summary>
        /// <param name="obj">Objeto a serializar.</param>
        /// <returns>Array de bytes representando o objeto.</returns>
        byte[] Serialize(object obj);

        /// <summary>
        /// Desserializa um array de bytes para um objeto.
        /// </summary>
        /// <param name="data">Array de bytes.</param>
        /// <returns>Objeto desserializado.</returns>
        object Deserialize(byte[] data);
    }
}
