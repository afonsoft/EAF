namespace Eaf.Middleware.Authorization.Permissions.Dto
{
    /// <summary>
    /// Representa a classe FlatPermissionWithLevelDto.
    /// </summary>
    public class FlatPermissionWithLevelDto : FlatPermissionDto
    {
        /// <summary>
        /// Obtém ou define Level.
        /// </summary>
        public int Level { get; set; }
    }
}