using System;
using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Dto
{
    /// <summary>
    /// Representa a classe FileDto.
    /// </summary>
    public class FileDto
    {
        /// <summary>
        /// FileDto.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public FileDto()
        {
        }

        /// <summary>
        /// FileDto.
        /// </summary>
        /// <param name="fileName">Parâmetro fileName.</param>
        /// <param name="fileType">Parâmetro fileType.</param>
        /// <returns>Resultado da operação.</returns>
        public FileDto(string fileName, string fileType)
        {
            FileName = fileName;
            FileType = fileType;
            FileToken = Guid.NewGuid().ToString("N");
        }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FileToken { get; set; }

        /// <summary>
        /// Obtém ou define FileType.
        /// </summary>
        public string FileType { get; set; }
    }
}