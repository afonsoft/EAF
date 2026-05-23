namespace Eaf.Middleware.Authorization.Permissions.Dto
{
    /// <summary>
    /// Representa a classe FlatPermissionDto.
    /// </summary>
    public class FlatPermissionDto
    {
        /// <summary>
        /// Obtém ou define Description.
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// Obtém ou define DisplayName.
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Obtém ou define IsGrantedByDefault.
        /// </summary>
        public bool IsGrantedByDefault { get; set; }
        /// <summary>
        /// Obtém ou define Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Obtém ou define ParentName.
        /// </summary>
        public string ParentName { get; set; } = null;
    }
}