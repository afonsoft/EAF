using System.IO;
using System.Threading.Tasks;
using Abp.BlobStoring;

namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Cliente genérico de armazenamento de BLOBs em nuvem (Azure, AWS S3, S3-compatível).
    /// </summary>
    public interface ICloudBlobClient
    {
        /// <summary>
        /// Salva o stream como um BLOB.
        /// </summary>
        Task SaveAsync(BlobProviderSaveArgs args);

        /// <summary>
        /// Deleta um BLOB existente.
        /// </summary>
        /// <returns>True se o BLOB existia e foi removido; false caso contrário.</returns>
        Task<bool> DeleteAsync(BlobProviderDeleteArgs args);

        /// <summary>
        /// Verifica se um BLOB existe.
        /// </summary>
        Task<bool> ExistsAsync(BlobProviderExistsArgs args);

        /// <summary>
        /// Obtém o stream do BLOB, ou null se não existir.
        /// </summary>
        Task<Stream> GetOrNullAsync(BlobProviderGetArgs args);
    }
}
