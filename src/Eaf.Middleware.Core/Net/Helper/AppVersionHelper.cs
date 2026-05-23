using Abp.Reflection.Extensions;
using System;

using System.IO;

namespace Eaf.Middleware
{
    /// <summary>
    /// Central point for application version.
    /// </summary>
    public static class AppVersionHelper
    {
        /// <summary>
        /// Gets current version of the application. It's also shown in the web page.
        /// </summary>
        public static string Version => System.Diagnostics.FileVersionInfo.GetVersionInfo(typeof(AppVersionHelper).GetAssembly().Location).FileVersion;

        /// <summary>
        /// Gets release (last build) date of the application. It's shown in the web page.
        /// </summary>
        public static DateTime ReleaseDate => new FileInfo(typeof(AppVersionHelper).GetAssembly().Location).LastWriteTime;
    }
}