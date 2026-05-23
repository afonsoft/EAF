using System;

namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe GetUserForEditOutput.
    /// </summary>
    public class GetUserForEditOutput
    {
        public Guid? ProfilePictureId { get; set; }

        public UserRoleDto[] Roles { get; set; }
        /// <summary>
        /// Obtém ou define User.
        /// </summary>
        public UserEditDto User { get; set; }
    }
}