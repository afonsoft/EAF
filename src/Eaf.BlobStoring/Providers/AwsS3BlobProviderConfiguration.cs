namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Chaves de configuração do provider de BLOBs AWS S3.
    /// </summary>
    public static class AwsS3BlobProviderConfiguration
    {
        /// <summary>
        /// Access Key ID da AWS.
        /// </summary>
        public const string AccessKeyId = "Aws.AccessKeyId";

        /// <summary>
        /// Secret Access Key da AWS.
        /// </summary>
        public const string SecretAccessKey = "Aws.SecretAccessKey";

        /// <summary>
        /// Região AWS (ex: us-east-1).
        /// </summary>
        public const string Region = "Aws.Region";

        /// <summary>
        /// Nome do bucket S3.
        /// </summary>
        public const string BucketName = "Aws.BucketName";

        /// <summary>
        /// URL de serviço personalizada para S3-compatível (MinIO, LocalStack, Wasabi, etc).
        /// </summary>
        public const string ServiceUrl = "Aws.ServiceUrl";

        /// <summary>
        /// Força o estilo de path no endpoint S3 (necessário para MinIO/LocalStack).
        /// </summary>
        public const string ForcePathStyle = "Aws.ForcePathStyle";

        /// <summary>
        /// Cria o bucket automaticamente caso não exista.
        /// </summary>
        public const string CreateBucketIfNotExists = "Aws.CreateBucketIfNotExists";
    }
}
