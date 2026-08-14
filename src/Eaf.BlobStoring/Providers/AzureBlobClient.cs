using System;
using System.IO;
using System.Threading.Tasks;
using Abp;
using Abp.BlobStoring;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Runtime.Session;
using Azure.Storage.Blobs;

namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Cliente de armazenamento de BLOBs no Azure Blob Storage para o EAF.
    /// </summary>
    public class AzureBlobClient : ICloudBlobClient, ITransientDependency
    {
        private readonly IAbpSession _session;
        private readonly IBlobNormalizeNamingService _normalizeNamingService;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="AzureBlobClient"/>.
        /// </summary>
        /// <param name="session">Sessão atual do ABP.</param>
        /// <param name="normalizeNamingService">Serviço de normalização de nomes.</param>
        public AzureBlobClient(IAbpSession session, IBlobNormalizeNamingService normalizeNamingService)
        {
            _session = session;
            _normalizeNamingService = normalizeNamingService;
        }

        /// <inheritdoc />
        public virtual async Task SaveAsync(BlobProviderSaveArgs args)
        {
            await CreateContainerIfNotExistsAsync(args);

            var blobName = GetBlobName(args);
            var blobClient = GetBlobClient(args, blobName);

            await blobClient.UploadAsync(args.BlobStream, overwrite: args.OverrideExisting, cancellationToken: args.CancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var blobName = GetBlobName(args);
            var blobClient = GetBlobClient(args, blobName);

            var response = await blobClient.DeleteIfExistsAsync(cancellationToken: args.CancellationToken);
            return response.Value;
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var blobName = GetBlobName(args);
            var blobClient = GetBlobClient(args, blobName);

            var response = await blobClient.ExistsAsync(args.CancellationToken);
            return response.Value;
        }

        /// <inheritdoc />
        public virtual async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var blobName = GetBlobName(args);
            var blobClient = GetBlobClient(args, blobName);

            var exists = await blobClient.ExistsAsync(args.CancellationToken);
            if (!exists.Value)
            {
                return null;
            }

            var downloadInfo = await blobClient.DownloadAsync(args.CancellationToken);
            var memoryStream = new MemoryStream();
            await downloadInfo.Value.Content.CopyToAsync(memoryStream, args.CancellationToken);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        /// <summary>
        /// Obtém o nome do BLOB com prefixo de tenant ou host.
        /// </summary>
        protected virtual string GetBlobName(BlobProviderArgs args)
        {
            var prefix = _session.TenantId.HasValue
                ? $"tenants/{_session.TenantId.Value}/"
                : "host/";

            return prefix + args.BlobName;
        }

        /// <summary>
        /// Obtém o cliente do contêiner do Azure.
        /// </summary>
        protected virtual BlobContainerClient GetBlobContainerClient(BlobProviderArgs args)
        {
            var connectionString = GetConnectionString(args);
            Check.NotNullOrWhiteSpace(connectionString, nameof(connectionString));

            var containerName = GetContainerName(args);
            Check.NotNullOrWhiteSpace(containerName, nameof(containerName));

            return new BlobContainerClient(connectionString, containerName);
        }

        /// <summary>
        /// Obtém o cliente do BLOB no Azure.
        /// </summary>
        protected virtual BlobClient GetBlobClient(BlobProviderArgs args, string blobName)
        {
            var containerClient = GetBlobContainerClient(args);
            return containerClient.GetBlobClient(blobName);
        }

        /// <summary>
        /// Cria o contêiner do Azure caso não exista e a configuração permita.
        /// </summary>
        protected virtual async Task CreateContainerIfNotExistsAsync(BlobProviderSaveArgs args)
        {
            var createIfNotExists = args.Configuration.GetConfigurationOrDefault<bool>(
                AzureBlobProviderConfiguration.CreateContainerIfNotExists,
                false);

            if (!createIfNotExists)
            {
                return;
            }

            var containerClient = GetBlobContainerClient(args);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: args.CancellationToken);
        }

        /// <summary>
        /// Obtém a connection string do Azure a partir da configuração do contêiner.
        /// </summary>
        protected virtual string GetConnectionString(BlobProviderArgs args)
        {
            return args.Configuration.GetConfigurationOrDefault<string>(
                AzureBlobProviderConfiguration.ConnectionString,
                null);
        }

        /// <summary>
        /// Obtém o nome do contêiner do Azure, normalizando quando necessário.
        /// </summary>
        protected virtual string GetContainerName(BlobProviderArgs args)
        {
            var containerName = args.Configuration.GetConfigurationOrDefault<string>(
                AzureBlobProviderConfiguration.ContainerName,
                null);

            if (containerName.IsNullOrWhiteSpace())
            {
                containerName = _normalizeNamingService.NormalizeContainerName(args.Configuration, args.ContainerName);
            }

            return containerName;
        }
    }
}
