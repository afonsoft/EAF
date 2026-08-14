using System.IO;
using System.Threading.Tasks;
using Abp.BlobStoring;
using Abp.Dependency;

namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Provedor de armazenamento de BLOBs no Azure Blob Storage para o EAF.
    /// </summary>
    public class AzureBlobProvider : BlobProviderBase, ITransientDependency
    {
        private readonly AzureBlobClient _azureBlobClient;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="AzureBlobProvider"/>.
        /// </summary>
        /// <param name="azureBlobClient">Cliente do Azure Blob Storage.</param>
        public AzureBlobProvider(AzureBlobClient azureBlobClient)
        {
            _azureBlobClient = azureBlobClient;
        }

        /// <inheritdoc />
        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            await _azureBlobClient.SaveAsync(args);
        }

        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            return await _azureBlobClient.DeleteAsync(args);
        }

        /// <inheritdoc />
        public override async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            return await _azureBlobClient.ExistsAsync(args);
        }

        /// <inheritdoc />
        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            return await _azureBlobClient.GetOrNullAsync(args);
        }
    }
}
