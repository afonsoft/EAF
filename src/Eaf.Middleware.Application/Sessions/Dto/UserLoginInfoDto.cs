using Abp.Application.Services.Dto;

namespace Eaf.Middleware.Sessions.Dto
{
    /// <summary>
    /// Representa a classe UserLoginInfoDto.
    /// </summary>
    public class UserLoginInfoDto : EntityDto<long>
    {
        /// <summary>
        /// Obtém ou define AuthenticationSource.
        /// </summary>
        public string AuthenticationSource { get; set; }
        /// <summary>
        /// Obtém ou define EmailAddress.
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Obtém ou define ProfilePictureId.
        /// </summary>
        public string ProfilePictureId { get; set; }
        /// <summary>
        /// Obtém ou define Surname.
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Obtém ou define UserName.
        /// </summary>
        public string UserName { get; set; }
    }
}