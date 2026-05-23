using Microsoft.Extensions.Primitives;
using System.IO;

namespace Eaf.Middleware.Worker.VirtualFileSystem
{
    /* Inspired from the Microsoft.Extensions.FileProviders.Physical package. */

    /// <summary>
    /// Representa a classe PathUtils.
    /// </summary>
    public static class PathUtils
    {
        private static readonly char[] PathSeparators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar };

        /// <summary>
        /// PathNavigatesAboveRoot.
        /// </summary>
        /// <param name="path">Parâmetro path.</param>
        /// <returns>Resultado da operação.</returns>
        public static bool PathNavigatesAboveRoot(string path)
        {
            var tokenizer = new StringTokenizer(path, PathSeparators);
            var depth = 0;

            foreach (var segment in tokenizer)
            {
                if (segment.Equals(".") || segment.Equals(""))
                {
                    continue;
                }

                if (segment.Equals(".."))
                {
                    depth--;

                    if (depth == -1)
                    {
                        return true;
                    }
                }
                else
                {
                    depth++;
                }
            }

            return false;
        }
    }
}