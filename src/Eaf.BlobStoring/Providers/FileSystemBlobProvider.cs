using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Abp;
using Abp.BlobStoring;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Eaf.BlobStoring.Providers
{
    /// <summary>
    /// Provedor de armazenamento de BLOBs em sistema de arquivos para o EAF.
    /// </summary>
    public class FileSystemBlobProvider : BlobProviderBase, ITransientDependency
    {
        private readonly IAbpSession _session;

        /// <summary>
        /// Inicializa uma nova instância do <see cref="FileSystemBlobProvider"/>.
        /// </summary>
        /// <param name="session">Sessão atual do ABP.</param>
        public FileSystemBlobProvider(IAbpSession session)
        {
            _session = session;
        }

        /// <inheritdoc />
        public override async Task SaveAsync(BlobProviderSaveArgs args)
        {
            var path = GetBlobPath(args, args.BlobName);
            var fileInfo = new FileInfo(path);

            if (fileInfo.Exists && !args.OverrideExisting)
            {
                throw new BlobAlreadyExistsException($"BLOB '{args.BlobName}' already exists in container '{args.ContainerName}'.");
            }

            Directory.CreateDirectory(fileInfo.DirectoryName);

            using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
            {
                await args.BlobStream.CopyToAsync(fileStream, args.CancellationToken);
            }
        }

        /// <inheritdoc />
        public override Task<bool> DeleteAsync(BlobProviderDeleteArgs args)
        {
            var path = GetBlobPath(args, args.BlobName);

            if (!File.Exists(path))
            {
                return Task.FromResult(false);
            }

            File.Delete(path);
            return Task.FromResult(true);
        }

        /// <inheritdoc />
        public override Task<bool> ExistsAsync(BlobProviderExistsArgs args)
        {
            var path = GetBlobPath(args, args.BlobName);
            return Task.FromResult(File.Exists(path));
        }

        /// <inheritdoc />
        public override Task<Stream> GetOrNullAsync(BlobProviderGetArgs args)
        {
            var path = GetBlobPath(args, args.BlobName);

            if (!File.Exists(path))
            {
                return Task.FromResult<Stream>(null);
            }

            return Task.FromResult<Stream>(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true));
        }

        /// <summary>
        /// Calcula o caminho completo do arquivo para o BLOB informado.
        /// </summary>
        protected virtual string GetBlobPath(BlobProviderArgs args, string blobName)
        {
            var basePath = GetBasePath(args);
            var blobPath = Path.Combine(basePath, GetTenantPath(args));

            if (GetAppendContainerNameToBasePath(args))
            {
                blobPath = Path.Combine(blobPath, args.ContainerName);
            }

            blobPath = Path.Combine(blobPath, blobName.Replace('\\', '/'));

            var fullBase = Path.GetFullPath(basePath);
            var fullBlob = Path.GetFullPath(blobPath);

            if (!fullBlob.StartsWith(fullBase, StringComparison.OrdinalIgnoreCase))
            {
                throw new AbpException($"BLOB path '{fullBlob}' is outside the configured base path '{fullBase}'.");
            }

            return blobPath;
        }

        /// <summary>
        /// Obtém o caminho base configurado.
        /// </summary>
        protected virtual string GetBasePath(BlobProviderArgs args)
        {
            return args.Configuration.GetConfigurationOrDefault<string>(
                FileSystemBlobProviderConfiguration.BasePath,
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "Blobs"));
        }

        /// <summary>
        /// Obtém o caminho relativo do tenant ou host.
        /// </summary>
        protected virtual string GetTenantPath(BlobProviderArgs args)
        {
            if (!args.Configuration.IsMultiTenant)
            {
                return "host";
            }

            return _session.TenantId.HasValue
                ? Path.Combine("tenants", _session.TenantId.Value.ToString())
                : "host";
        }

        /// <summary>
        /// Indica se o nome do contêiner deve ser anexado ao caminho base.
        /// </summary>
        protected virtual bool GetAppendContainerNameToBasePath(BlobProviderArgs args)
        {
            return args.Configuration.GetConfigurationOrDefault<bool>(
                FileSystemBlobProviderConfiguration.AppendContainerNameToBasePath,
                true);
        }
    }
}
