namespace Eaf.Middleware.Authorization.Users.Dto
{
    /// <summary>
    /// Representa a classe UserRoleDto.
    /// </summary>
    public class UserRoleDto
    {
        /// <summary>
        /// Obtém ou define IsAssigned.
        /// </summary>
        public bool IsAssigned { get; set; }
        /// <summary>
        /// Obtém ou define RoleDisplayName.
        /// </summary>
        public string RoleDisplayName { get; set; }
        /// <summary>
        /// Obtém ou define RoleId.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Obtém ou define RoleName.
        /// </summary>
        public string RoleName { get; set; }
    }
}