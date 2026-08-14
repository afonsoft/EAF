namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Chaves de configuração do provedor FileSystem.
    /// </summary>
    public static class FileSystemBlobProviderConfiguration
    {
        /// <summary>
        /// Caminho base de armazenamento.
        /// </summary>
        public const string BasePath = "FileSystem.BasePath";

        /// <summary>
        /// Indica se o nome do contêiner deve ser anexado ao caminho base.
        /// </summary>
        public const string AppendContainerNameToBasePath = "FileSystem.AppendContainerNameToBasePath";

        /// <summary>
        /// Estratégia de isolamento (Host, Tenant, HostAndTenant).
        /// </summary>
        public const string Isolation = "FileSystem.Isolation";
    }
}
