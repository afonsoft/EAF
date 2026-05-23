using System;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe UserLoginAttemptDto.
    /// </summary>
    public class UserLoginAttemptDto
    {
        /// <summary>
        /// Obtém ou define BrowserInfo.
        /// </summary>
        public string BrowserInfo { get; set; }
        /// <summary>
        /// Obtém ou define ClientIpAddress.
        /// </summary>
        public string ClientIpAddress { get; set; }
        /// <summary>
        /// Obtém ou define ClientName.
        /// </summary>
        public string ClientName { get; set; }
        /// <summary>
        /// Obtém ou define CreationTime.
        /// </summary>
        public DateTime CreationTime { get; set; }
        /// <summary>
        /// Obtém ou define Result.
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// Obtém ou define TenancyName.
        /// </summary>
        public string TenancyName { get; set; }

        /// <summary>
        /// Obtém ou define UserNameOrEmail.
        /// </summary>
        public string UserNameOrEmail { get; set; }
    }
}