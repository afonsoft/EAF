using System;
using System.IO;
using System.Threading.Tasks;
using Abp.BlobStoring;
using Abp.Dependency;
using Abp.Extensions;

namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Provider genérico de BLOBs em nuvem do EAF. Suporta Azure Blob Storage e AWS S3.
    /// </summary>
    public class EafCloudBlobProvider : BlobProviderBase, ITransientDependency
    {
        private readonly IIocManager _iocManager;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="EafCloudBlobProvider"/>.
        /// </summary>
        /// <param name="iocManager">Gerenciador de IoC do Castle Windsor.</param>
        public EafCloudBlobProvider(IIocManager iocManager)
        {
            _iocManager = iocManager;
        }

        /// <inheritdoc />
        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var client = GetClient(args.Configuration);
            await client.SaveAsync(args);
        }

        /// <inheritdoc />
        public override async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var client = GetClient(args.Configuration);
            return await client.DeleteAsync(args);
        }

        /// <inheritdoc />
        public override async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var client = GetClient(args.Configuration);
            return await client.ExistsAsync(args);
        }

        /// <inheritdoc />
        public override async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var client = GetClient(args.Configuration);
            return await client.GetOrNullAsync(args);
        }

        /// <summary>
        /// Resolve o cliente de nuvem adequado com base na configuração do contêiner.
        /// </summary>
        protected virtual ICloudBlobClient GetClient(BlobContainerConfiguration configuration)
        {
            var provider = configuration.GetConfigurationOrDefault<string>(
                CloudBlobProviderConfiguration.CloudProvider,
                "Azure");

            if (provider.IsNullOrWhiteSpace())
            {
                provider = "Azure";
            }

            switch (provider.ToLowerInvariant())
            {
                case "aws":
                case "s3":
                    return _iocManager.Resolve<AwsS3BlobClient>();
                case "azure":
                    return _iocManager.Resolve<AzureBlobClient>();
                default:
                    throw new NotSupportedException($"Cloud provider '{provider}' is not supported. Use 'Azure' or 'Aws'.");
            }
        }
    }
}
