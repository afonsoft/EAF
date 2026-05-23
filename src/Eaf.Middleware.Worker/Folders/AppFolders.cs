using Abp.Dependency;
using Microsoft.Extensions.FileProviders;

namespace Eaf.Middleware.Worker.Folders
{
    /// <summary>
    /// Representa a classe AppFolders.
    /// </summary>
    public class AppFolders : IAppFolders, ISingletonDependency
    {
        /// <summary>
        /// Obtém ou define ProfileImagesFolder.
        /// </summary>
        public string ProfileImagesFolder { get; set; }
        /// <summary>
        /// Obtém ou define DataFolder.
        /// </summary>
        public string DataFolder { get; set; }
        /// <summary>
        /// Obtém ou define DownloadFolder.
        /// </summary>
        public string DownloadFolder { get; set; }
        /// <summary>
        /// Obtém ou define LogsFolder.
        /// </summary>
        public string LogsFolder { get; set; }
        /// <summary>
        /// Obtém ou define TempFolder.
        /// </summary>
        public string TempFolder { get; set; }

        /// <summary>
        /// Obtém ou define RootFileProvider.
        /// </summary>
        public CompositeFileProvider RootFileProvider { get; set; }
    }
}