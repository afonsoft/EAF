using System.ComponentModel.DataAnnotations;

namespace Eaf.Middleware.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// Representa a classe UpdateProfilePictureInput.
    /// </summary>
    public class UpdateProfilePictureInput
    {
        [Required]
        [MaxLength(400)]
        public string FileToken { get; set; }

        /// <summary>
        /// Obtém ou define Height.
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Obtém ou define Width.
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Obtém ou define X.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Obtém ou define Y.
        /// </summary>
        public int Y { get; set; }
    }
}