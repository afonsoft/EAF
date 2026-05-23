using Abp.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Eaf.Middleware.IO
{
    /// <summary>
    /// Representa a classe AppFileHelper.
    /// </summary>
    public static class AppFileHelper
    {
        /// <summary>
        /// DeleteFilesInFolderIfExists.
        /// </summary>
        /// <param name="folderPath">Parâmetro folderPath.</param>
        /// <param name="fileNameWithoutExtension">Parâmetro fileNameWithoutExtension.</param>
        public static void DeleteFilesInFolderIfExists(string folderPath, string fileNameWithoutExtension)
        {
            var directory = new DirectoryInfo(folderPath);
            var tempUserProfileImages = directory.GetFiles(fileNameWithoutExtension + ".*", SearchOption.AllDirectories).ToList();
            foreach (var tempUserProfileImage in tempUserProfileImages)
            {
                FileHelper.DeleteIfExists(tempUserProfileImage.FullName);
            }
        }

        /// <summary>
        /// ReadLines.
        /// </summary>
        /// <param name="path">Parâmetro path.</param>
        /// <returns>Resultado da operação.</returns>
        public static IEnumerable<string> ReadLines(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 0x1000, FileOptions.SequentialScan))
            using (var sr = new StreamReader(fs, Encoding.UTF8))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}