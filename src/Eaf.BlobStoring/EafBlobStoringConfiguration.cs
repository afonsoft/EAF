using System;
using System.Collections.Generic;
using System.IO;
using Eaf.BlobStoring.Naming;
using Eaf.BlobStoring.Providers;

namespace Eaf.BlobStoring
{
    /// <summary>
    /// Configuração do módulo de armazenamento de BLOBs do EAF.
    /// </summary>
    public interface IEafBlobStoringConfiguration
    {
        /// <summary>
        /// Tipo do provedor padrão de BLOBs.
        /// </summary>
        Type DefaultProvider { get; set; }

        /// <summary>
        /// Caminho base para o provedor FileSystem.
        /// </summary>
        string FileSystemBasePath { get; set; }

        /// <summary>
        /// Indica se o nome do contêiner deve ser anexado ao caminho base.
        /// </summary>
        bool FileSystemAppendContainerNameToBasePath { get; set; }

        /// <summary>
        /// Estratégia de isolamento para o provedor FileSystem.
        /// </summary>
        string FileSystemIsolation { get; set; }

        /// <summary>
        /// Connection string do Azure Blob Storage.
        /// </summary>
        string AzureConnectionString { get; set; }

        /// <summary>
        /// Nome do contêiner do Azure Blob Storage.
        /// </summary>
        string AzureContainerName { get; set; }

        /// <summary>
        /// Indica se o contêiner do Azure deve ser criado caso não exista.
        /// </summary>
        bool AzureCreateContainerIfNotExists { get; set; }

        /// <summary>
        /// Tipo de provider de nuvem para o EafCloudBlobProvider (Azure ou Aws).
        /// </summary>
        string CloudProvider { get; set; }

        /// <summary>
        /// Access Key ID da AWS.
        /// </summary>
        string AwsAccessKeyId { get; set; }

        /// <summary>
        /// Secret Access Key da AWS.
        /// </summary>
        string AwsSecretAccessKey { get; set; }

        /// <summary>
        /// Região AWS (ex: us-east-1).
        /// </summary>
        string AwsRegion { get; set; }

        /// <summary>
        /// Nome do bucket S3.
        /// </summary>
        string AwsBucketName { get; set; }

        /// <summary>
        /// URL de serviço personalizada para S3-compatível (MinIO, LocalStack, Wasabi, etc).
        /// </summary>
        string AwsServiceUrl { get; set; }

        /// <summary>
        /// Força o estilo de path no endpoint S3 (necessário para MinIO/LocalStack).
        /// </summary>
        bool AwsForcePathStyle { get; set; }

        /// <summary>
        /// Cria o bucket S3 automaticamente caso não exista.
        /// </summary>
        bool AwsCreateBucketIfNotExists { get; set; }

        /// <summary>
        /// Lista de normalizadores de nomes de BLOBs.
        /// </summary>
        IList<Type> NamingNormalizers { get; }
    }

    /// <summary>
    /// Implementação padrão da configuração do módulo de armazenamento de BLOBs do EAF.
    /// </summary>
    public class EafBlobStoringConfiguration : IEafBlobStoringConfiguration
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="EafBlobStoringConfiguration"/>.
        /// </summary>
        public EafBlobStoringConfiguration()
        {
            DefaultProvider = typeof(FileSystemBlobProvider);
            FileSystemBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Blobs");
            FileSystemAppendContainerNameToBasePath = true;
            FileSystemIsolation = "HostAndTenant";
            AzureContainerName = "eaf-blobs";
            AzureCreateContainerIfNotExists = false;
            CloudProvider = "Azure";
            AwsRegion = "us-east-1";
            AwsForcePathStyle = false;
            AwsCreateBucketIfNotExists = false;
            NamingNormalizers = new List<Type> { typeof(EafDefaultBlobNamingNormalizer) };
        }

        /// <inheritdoc />
        public Type DefaultProvider { get; set; }

        /// <inheritdoc />
        public string FileSystemBasePath { get; set; }

        /// <inheritdoc />
        public bool FileSystemAppendContainerNameToBasePath { get; set; }

        /// <inheritdoc />
        public string FileSystemIsolation { get; set; }

        /// <inheritdoc />
        public string AzureConnectionString { get; set; }

        /// <inheritdoc />
        public string AzureContainerName { get; set; }

        /// <inheritdoc />
        public bool AzureCreateContainerIfNotExists { get; set; }

        /// <inheritdoc />
        public string CloudProvider { get; set; }

        /// <inheritdoc />
        public string AwsAccessKeyId { get; set; }

        /// <inheritdoc />
        public string AwsSecretAccessKey { get; set; }

        /// <inheritdoc />
        public string AwsRegion { get; set; }

        /// <inheritdoc />
        public string AwsBucketName { get; set; }

        /// <inheritdoc />
        public string AwsServiceUrl { get; set; }

        /// <inheritdoc />
        public bool AwsForcePathStyle { get; set; }

        /// <inheritdoc />
        public bool AwsCreateBucketIfNotExists { get; set; }

        /// <inheritdoc />
        public IList<Type> NamingNormalizers { get; }
    }
}
