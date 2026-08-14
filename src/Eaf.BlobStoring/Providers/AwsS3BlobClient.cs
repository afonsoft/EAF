using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Abp;
using Abp.BlobStoring;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Runtime.Session;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Cliente de armazenamento de BLOBs no AWS S3 (ou serviço compatível com S3) para o EAF.
    /// </summary>
    public class AwsS3BlobClient : ICloudBlobClient, ITransientDependency
    {
        private readonly IAbpSession _session;
        private readonly IBlobNormalizeNamingService _normalizeNamingService;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="AwsS3BlobClient"/>.
        /// </summary>
        /// <param name="session">Sessão atual do ABP.</param>
        /// <param name="normalizeNamingService">Serviço de normalização de nomes.</param>
        public AwsS3BlobClient(IAbpSession session, IBlobNormalizeNamingService normalizeNamingService)
        {
            _session = session;
            _normalizeNamingService = normalizeNamingService;
        }

        /// <inheritdoc />
        public virtual async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var client = await GetS3ClientAsync(args);
            var bucketName = GetBucketName(args);
            var key = GetBlobKey(args);

            await CreateBucketIfNotExistsAsync(client, bucketName, args);

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = args.BlobStream,
                AutoCloseStream = false,
                CannedACL = S3CannedACL.Private
            };

            await client.PutObjectAsync(request, args.CancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var client = await GetS3ClientAsync(args);
            var bucketName = GetBucketName(args);
            var key = GetBlobKey(args);

            try
            {
                await client.DeleteObjectAsync(bucketName, key, args.CancellationToken);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var client = await GetS3ClientAsync(args);
            var bucketName = GetBucketName(args);
            var key = GetBlobKey(args);

            try
            {
                await client.GetObjectMetadataAsync(bucketName, key, args.CancellationToken);
                return true;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        /// <inheritdoc />
        public virtual async Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var client = await GetS3ClientAsync(args);
            var bucketName = GetBucketName(args);
            var key = GetBlobKey(args);

            try
            {
                using var response = await client.GetObjectAsync(bucketName, key, args.CancellationToken);
                var memoryStream = new MemoryStream();

                await response.ResponseStream.CopyToAsync(memoryStream, args.CancellationToken);
                memoryStream.Seek(0, SeekOrigin.Begin);

                return memoryStream;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        /// <summary>
        /// Cria o bucket S3 caso a configuração permita e ele não exista.
        /// </summary>
        protected virtual async Task CreateBucketIfNotExistsAsync(IAmazonS3 client, string bucketName, BlobProviderArgs args)
        {
            if (args is not BlobProviderSaveArgs saveArgs)
            {
                return;
            }

            var createIfNotExists = saveArgs.Configuration.GetConfigurationOrDefault<bool>(
                AwsS3BlobProviderConfiguration.CreateBucketIfNotExists,
                false);

            if (!createIfNotExists)
            {
                return;
            }

            try
            {
                await client.PutBucketAsync(new PutBucketRequest { BucketName = bucketName }, args.CancellationToken);
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                // BucketAlreadyExists ou BucketAlreadyOwnedByYou
            }
        }

        /// <summary>
        /// Obtém o cliente do AWS S3 baseado na configuração do contêiner.
        /// </summary>
        protected virtual Task<IAmazonS3> GetS3ClientAsync(BlobProviderArgs args)
        {
            var config = GetS3Config(args);
            var credentials = GetAwsCredentials(args);
            var client = credentials != null
                ? new AmazonS3Client(credentials, config)
                : new AmazonS3Client(config);

            return Task.FromResult<IAmazonS3>(client);
        }

        /// <summary>
        /// Obtém a configuração do cliente S3.
        /// </summary>
        protected virtual AmazonS3Config GetS3Config(BlobProviderArgs args)
        {
            var serviceUrl = args.Configuration.GetConfigurationOrDefault<string>(
                AwsS3BlobProviderConfiguration.ServiceUrl,
                null);

            var forcePathStyle = args.Configuration.GetConfigurationOrDefault<bool>(
                AwsS3BlobProviderConfiguration.ForcePathStyle,
                false);

            var region = args.Configuration.GetConfigurationOrDefault<string>(
                AwsS3BlobProviderConfiguration.Region,
                null);

            var config = new AmazonS3Config();

            if (!serviceUrl.IsNullOrWhiteSpace())
            {
                config.ServiceURL = serviceUrl;
                config.ForcePathStyle = forcePathStyle;
            }

            config.RegionEndpoint = !region.IsNullOrWhiteSpace()
                ? RegionEndpoint.GetBySystemName(region)
                : RegionEndpoint.USEast1;

            return config;
        }

        /// <summary>
        /// Obtém as credenciais AWS a partir da configuração, ou null para usar a cadeia padrão.
        /// </summary>
        protected virtual AWSCredentials GetAwsCredentials(BlobProviderArgs args)
        {
            var accessKey = args.Configuration.GetConfigurationOrDefault<string>(
                AwsS3BlobProviderConfiguration.AccessKeyId,
                null);

            var secretKey = args.Configuration.GetConfigurationOrDefault<string>(
                AwsS3BlobProviderConfiguration.SecretAccessKey,
                null);

            if (accessKey.IsNullOrWhiteSpace() || secretKey.IsNullOrWhiteSpace())
            {
                return null;
            }

            return new BasicAWSCredentials(accessKey, secretKey);
        }

        /// <summary>
        /// Obtém o nome do bucket S3, normalizando quando necessário.
        /// </summary>
        protected virtual string GetBucketName(BlobProviderArgs args)
        {
            var bucketName = args.Configuration.GetConfigurationOrDefault<string>(
                AwsS3BlobProviderConfiguration.BucketName,
                null);

            if (bucketName.IsNullOrWhiteSpace())
            {
                bucketName = _normalizeNamingService.NormalizeContainerName(args.Configuration, args.ContainerName);
            }

            Check.NotNullOrWhiteSpace(bucketName, nameof(bucketName));

            return bucketName;
        }

        /// <summary>
        /// Obtém a chave do objeto S3 com prefixo de tenant ou host.
        /// </summary>
        protected virtual string GetBlobKey(BlobProviderArgs args)
        {
            var prefix = _session.TenantId.HasValue
                ? $"tenants/{_session.TenantId.Value}/"
                : "host/";

            return prefix + args.BlobName;
        }
    }
}
