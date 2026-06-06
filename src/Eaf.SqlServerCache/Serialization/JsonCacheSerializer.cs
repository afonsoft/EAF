using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eaf.Runtime.Caching.Serialization
{
    /// <summary>
    /// Implementação de ICacheSerializer usando System.Text.Json.
    /// </summary>
    public class JsonCacheSerializer : ICacheSerializer
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Serializa um objeto para array de bytes usando System.Text.Json.
        /// </summary>
        /// <param name="obj">Objeto a serializar.</param>
        /// <returns>Array de bytes representando o objeto.</returns>
        public byte[] Serialize(object obj)
        {
            if (obj == null) return null;
            return JsonSerializer.SerializeToUtf8Bytes(obj, _options);
        }

        /// <summary>
        /// Desserializa um array de bytes para um objeto usando System.Text.Json.
        /// </summary>
        /// <param name="data">Array de bytes.</param>
        /// <returns>Objeto desserializado.</returns>
        public object Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0) return null;
            return JsonSerializer.Deserialize<object>(data, _options);
        }
    }
}
