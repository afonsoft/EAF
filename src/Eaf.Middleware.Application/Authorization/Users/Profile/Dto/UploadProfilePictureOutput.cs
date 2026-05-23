using Abp.Web.Models;


namespace Eaf.Middleware.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// Representa a classe UploadProfilePictureOutput.
    /// </summary>
    public class UploadProfilePictureOutput : ErrorInfo
    {
        /// <summary>
        /// UploadProfilePictureOutput.
        /// </summary>
        /// <returns>Resultado da operação.</returns>
        public UploadProfilePictureOutput()
        {
        }

        /// <summary>
        /// UploadProfilePictureOutput.
        /// </summary>
        /// <param name="error">Parâmetro error.</param>
        /// <returns>Resultado da operação.</returns>
        public UploadProfilePictureOutput(ErrorInfo error)
        {
            Code = error.Code;
            Details = error.Details;
            Message = error.Message;
            ValidationErrors = error.ValidationErrors;
        }

        /// <summary>
        /// Obtém ou define FileName.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Obtém ou define FileToken.
        /// </summary>
        public string FileToken { get; set; }
        /// <summary>
        /// Obtém ou define FileType.
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// Obtém ou define Height.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Obtém ou define Width.
        /// </summary>
        public int Width { get; set; }
    }
}