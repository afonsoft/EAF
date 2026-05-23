using Abp.Reflection.Extensions;
using System;
using System.IO;
using System.Linq;

namespace Eaf.Middleware.Web
{
    /// <summary>
    /// This class is used to find root path of the web project in; unit tests (to find views) and
    /// entity framework core command line commands (to find conn string).
    /// </summary>
    public static class WebContentDirectoryFinder
    {
        /// <summary>
        /// CalculateContentRootFolder.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public static string CalculateContentRootFolder()
        {
            var coreAssemblyDirectoryPath = Path.GetDirectoryName(typeof(MiddlewareCoreModule).GetAssembly().Location);
            if (coreAssemblyDirectoryPath == null)
            {
                throw new Exception("Could not find location of Eaf.Middleware.Core assembly!");
            }

            var directoryInfo = new DirectoryInfo(coreAssemblyDirectoryPath);
            while (!DirectoryContains(directoryInfo.FullName, "Eaf.sln")
                && !DirectoryContains(directoryInfo.FullName, "Eaf.ProjectName.sln")
                && !DirectoryContains(directoryInfo.FullName, "Web.Host.csproj"))
            {
                if (directoryInfo.Parent == null)
                {
                    throw new Exception("Could not find content root folder!");
                }

                directoryInfo = directoryInfo.Parent;
            }

            var webHostFolder = Path.Combine(directoryInfo.FullName, $"src{Path.DirectorySeparatorChar}Eaf.Middleware.Web.Host");
            if (Directory.Exists(webHostFolder))
            {
                return webHostFolder;
            }

            throw new Exception("Could not find root folder of the web project!");
        }

        private static bool DirectoryContains(string directory, string fileName)
        {
            return Directory.GetFiles(directory).Any(filePath => string.Equals(Path.GetFileName(filePath), fileName));
        }
    }
}