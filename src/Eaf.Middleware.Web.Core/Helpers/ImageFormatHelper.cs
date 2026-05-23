using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;

namespace Eaf.Middleware.Web.Helpers
{
    /// <summary>
    /// Representa a classe ImageFormatHelper.
    /// </summary>
    public static class ImageFormatHelper
    {
        /// <summary>
        /// GetRawImageFormat.
        /// </summary>
        /// <param name="fileBytes">Parâmetro fileBytes.</param>
        /// <returns>Resultado da operação.</returns>
        public static IImageFormat GetRawImageFormat(byte[] fileBytes)
        {
            using (var ms = new MemoryStream(fileBytes))
            {
                return Image.DetectFormat(ms);
            }
        }
    }
}