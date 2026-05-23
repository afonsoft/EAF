using Microsoft.Extensions.FileProviders;

namespace Eaf.Middleware
{
    /// <summary>
    /// Representa a interface IAppFolders.
    /// </summary>
    public interface IAppFolders
    {
        /// <summary>
        /// wwwroot/ProfileImages
        /// </summary>
        string ProfileImagesFolder { get; set; }

        /// <summary>
        /// wwwroot
        /// </summary>
        string WebDataFolder { get; set; }

        /// <summary>
        /// wwwroot/Downloads
        /// </summary>
        string WebDownloadFolder { get; set; }

        /// <summary>
        /// wwwroot/logs
        /// </summary>
        string WebLogsFolder { get; set; }

        /// <summary>
        /// Path.GetTempPath()
        /// </summary>
        string WebTempFolder { get; set; }

        /// <summary>
        /// Obtém ou define WebRootFileProvider.
        /// </summary>
        public CompositeFileProvider WebRootFileProvider { get; set; }
    }
}