namespace Eaf.Middleware.Authorization.Users.Profile.Dto
{
    /// <summary>
    /// Representa a classe GetProfilePictureOutput.
    /// </summary>
    public class GetProfilePictureOutput
    {
        /// <summary>
        /// GetProfilePictureOutput.
        /// </summary>
        /// <param name="profilePicture">Parâmetro profilePicture.</param>
        /// <returns>Resultado da operação.</returns>
        public GetProfilePictureOutput(string profilePicture)
        {
            ProfilePicture = profilePicture;
        }

        /// <summary>
        /// Obtém ou define ProfilePicture.
        /// </summary>
        public string ProfilePicture { get; set; }
    }
}