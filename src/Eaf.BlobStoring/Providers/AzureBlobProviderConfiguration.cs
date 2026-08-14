namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Chaves de configuração do provedor Azure Blob Storage.
    /// </summary>
    public static class AzureBlobProviderConfiguration
    {
        /// <summary>
        /// Connection string do Azure Blob Storage.
        /// </summary>
        public const string ConnectionString = "Azure.ConnectionString";

        /// <summary>
        /// Nome do contêiner do Azure.
        /// </summary>
        public const string ContainerName = "Azure.ContainerName";

        /// <summary>
        /// Indica se o contêiner deve ser criado caso não exista.
        /// </summary>
        public const string CreateContainerIfNotExists = "Azure.CreateContainerIfNotExists";
    }
}
