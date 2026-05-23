using System;
using System.Linq;

namespace Eaf.Middleware.StringExtensions
{
    /// <summary>
    /// Representa a classe StringExtensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Format bytes in string with Unit (kB, MB, GB, TB)
        /// </summary>
        /// <param name="fileSizeInBytes">long or int in bytes</param>
        /// <returns>XXX kB XXX MB</returns>
        public static string FormatSize(this int fileSizeInBytes)
        {
            return FormatSize((long)fileSizeInBytes);
        }

        /// <summary>
        /// Format bytes in string with Unit (kB, MB, GB, TB)
        /// </summary>
        /// <param name="fileSizeInBytes">long or int in bytes</param>
        /// <returns>XXX kB XXX MB</returns>
        public static string FormatSize(this long fileSizeInBytes)
        {
            if (fileSizeInBytes < 0) return "0 kB";
            if (fileSizeInBytes == 0) return "0 kB";

            string[] units = { "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            double number = fileSizeInBytes;
            int unitIndex = -1;

            // Para valores < 1024 bytes (excluindo 0, já tratado), deve ser "0 kB"
            if (number < 1024)
            {
                return "0 kB";
            }

            // Loop para encontrar a unidade correta
            // Começamos a dividir para kB, MB, etc.
            do
            {
                number /= 1024.0;
                unitIndex++;
            } while (number >= 1024.0 && unitIndex < units.Length - 1);

            return Math.Truncate(number) + " " + units[unitIndex];
        }

        /// <summary>
        /// Check if an item Contains a item in the list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        public static bool IsContains(this string item, params string[] list)
        {
            return list.Any(x => item.Contains(x));
        }
    }
}