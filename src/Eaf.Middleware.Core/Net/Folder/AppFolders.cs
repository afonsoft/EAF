using Abp.Dependency;
using Microsoft.Extensions.FileProviders;

namespace Eaf.Middleware
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
        /// Obtém ou define WebDataFolder.
        /// </summary>
        public string WebDataFolder { get; set; }
        /// <summary>
        /// Obtém ou define WebDownloadFolder.
        /// </summary>
        public string WebDownloadFolder { get; set; }
        /// <summary>
        /// Obtém ou define WebLogsFolder.
        /// </summary>
        public string WebLogsFolder { get; set; }
        /// <summary>
        /// Obtém ou define WebTempFolder.
        /// </summary>
        public string WebTempFolder { get; set; }

        /// <summary>
        /// Obtém ou define WebRootFileProvider.
        /// </summary>
        public CompositeFileProvider WebRootFileProvider { get; set; }
    }
}