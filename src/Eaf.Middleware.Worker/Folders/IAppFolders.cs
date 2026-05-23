using Microsoft.Extensions.FileProviders;

namespace Eaf.Middleware.Worker.Folders
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
        string DataFolder { get; set; }

        /// <summary>
        /// wwwroot/Downloads
        /// </summary>
        string DownloadFolder { get; set; }

        /// <summary>
        /// wwwroot/logs
        /// </summary>
        string LogsFolder { get; set; }

        /// <summary>
        /// Path.GetTempPath()
        /// </summary>
        string TempFolder { get; set; }

        /// <summary>
        /// Obtém ou define RootFileProvider.
        /// </summary>
        public CompositeFileProvider RootFileProvider { get; set; }
    }
}